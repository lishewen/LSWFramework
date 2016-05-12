Imports LSW.Image.Captcha.Classification
Imports LSW.Image.Captcha.Utility
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports LSW.Image.Captcha.Utility.ImgGrayTrans
Imports LSW.Exceptions

Namespace Image.Captcha
    Public Class CaptchaRecognition
        Inherits CaptchaRecognitionBase
        ' Methods
        Public Sub New()
            MyBase.Classifier = New NearestDistanceClassification
            Me.m_SegmentCharType = SegmentCharType.ConnectedComponent
            Me.CharCount = 0
        End Sub

        Private Function Classify(ByVal Features As List(Of List(Of Double))) As String
            Return MyBase.Classifier.Classify(Features)
        End Function

        Private Function GetFeatures(ByVal ccs As List(Of List(Of Point))) As List(Of List(Of Double))
            Dim list As New List(Of List(Of Double))
            Dim graphics As Graphics = graphics.FromImage(MyBase.DebuggingImg)
            Dim i As Integer
            For i = 0 To ccs.Count - 1
                Dim points As List(Of Point) = ccs.Item(i)
                If (points.Count >= 5) Then
                    Dim j As Integer
                    For j = 0 To points.Count - 1
                        Dim point As Point = points.Item(j)
                        Dim point2 As Point = points.Item(j)
                        MyBase.m_bm.SetPixel(point.X, point2.Y, Color.FromArgb(&HFF, 0, 0))
                    Next j
                    Dim boundingRect As Rectangle = PicHelper.GetBoundingRect(points)
                    graphics.DrawRectangle(New Pen(Color.FromArgb(0, 0, &HFF)), boundingRect)
                    Dim item As List(Of Double) = PicHelper.GetFeature(points, 5, 5)
                    Dim k As Integer
                    For k = 0 To item.Count - 1
                        Dim num4 As Double = item.Item(k)
                        If num4.Equals(Double.NaN) Then
                            item.Item(k) = 0
                        End If
                    Next k
                    list.Add(item)
                End If
            Next i
            graphics.Dispose()
            Return list
        End Function

        Private Function GetRanges(ByVal StatisticsRet As List(Of Double), ByVal Threshold As Double) As List(Of List(Of Integer))
            Dim list As New List(Of List(Of Integer))
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim num3 As Integer = 0
            Dim i As Integer
            For i = 0 To StatisticsRet.Count - 1
                Dim num5 As Double = StatisticsRet.Item(i)
                Select Case num
                    Case 0
                        If (num5 <= Threshold) Then
                            Exit Select
                        End If
                        num = 1
                        Continue For
                    Case 1
                        If (num5 <= Threshold) Then
                            num = 2
                            num2 = i
                        End If
                        Continue For
                    Case 2
                        If (num5 > Threshold) Then
                            num = 1
                            num3 = i
                            list.Add(New List(Of Integer) From {num2, num3})
                        End If
                        Continue For
                    Case Else
                        Continue For
                End Select
                If (num5 <= Threshold) Then
                    num = 2
                    num2 = i
                End If
            Next i
            If (num = 2) Then
                list.Add(New List(Of Integer) From {num2, StatisticsRet.Count})
            End If
            Return list
        End Function

        Public Overrides Sub LoadSamples(ByVal strSamples As String)
        End Sub

        Private Sub PreprocessImage(ByVal bm As Bitmap)
            ImgGrayTrans.Threshold(bm, 140, &HFF, ThresholdType.Binary)
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim i As Integer
            For i = 0 To bm.Width - 1
                Dim j As Integer
                For j = 0 To bm.Height - 1
                    If (bm.GetPixel(i, j).R = 0) Then
                        num += 1
                    Else
                        num2 += 1
                    End If
                Next j
            Next i
            If (num > num2) Then
                ImgGrayTrans.Reverse(bm)
            End If
        End Sub

        Public Overrides Function Recognize(ByVal bm As Bitmap) As String
            MyBase.m_Chronograph.Begin()
            MyBase.Recognize(bm)
            Me.PreprocessImage(MyBase.m_bm)
            Dim ccs As List(Of List(Of Point)) = Nothing
            If (Me.m_SegmentCharType = SegmentCharType.ConnectedComponent) Then
                ccs = Me.SegmentChar_ConnectedComponent(MyBase.m_bm)
            ElseIf (Me.m_SegmentCharType = SegmentCharType.Fixed) Then
                ccs = Me.SegmentChar_Fixed(MyBase.m_bm)
            End If
            Dim features As List(Of List(Of Double)) = Me.GetFeatures(ccs)
            Dim i As Integer
            For i = 0 To features.Count - 1
                Dim str As String = ClassificationBase.FeatureToString(features.Item(i))
                MyBase.RuntimeInfo = (MyBase.RuntimeInfo & str & ChrW(13) & ChrW(10))
            Next i
            Dim str2 As String = Me.Classify(features)
            Dim elapsed As Long = MyBase.m_Chronograph.Elapsed
            MyBase.RuntimeInfo = (MyBase.RuntimeInfo & ChrW(13) & ChrW(10) & "耗时：" & elapsed)
            Return str2
        End Function

        Private Function SegmentChar_ConnectedComponent(ByVal bm As Bitmap) As List(Of List(Of Point))
            Dim connectedComponents As List(Of List(Of Point)) = New ConnectedComponent() With { _
                .Roi = New Rectangle(1, 1, (bm.Width - 2), (bm.Height - 2)) _
            }.GetConnectedComponents(bm)
            If (connectedComponents.Count < Me.CharCount) Then
                Dim num As Integer = (Me.CharCount - connectedComponents.Count)
                Dim num2 As Integer = 0
                Dim index As Integer = -1
                Dim rectangle As New Rectangle
                Dim j As Integer
                For j = 0 To connectedComponents.Count - 1
                    Dim boundingRect As Rectangle = PicHelper.GetBoundingRect(connectedComponents.Item(j))
                    If (boundingRect.Width > num2) Then
                        num2 = boundingRect.Width
                        index = j
                        rectangle = boundingRect
                    End If
                Next j
                Dim num5 As Integer = ((Me.CharCount - connectedComponents.Count) + 1)
                Dim width As Integer = (num2 / num5)
                num = (num2 Mod num5)
                Dim list2 As New List(Of Rectangle)
                Dim num7 As Integer = 0
                Dim k As Integer
                For k = 0 To (num5 - 1) - 1
                    list2.Add(New Rectangle((rectangle.X + num7), rectangle.Y, width, rectangle.Height))
                    num7 = (num7 + width)
                Next k
                list2.Add(New Rectangle((rectangle.X + num7), rectangle.Y, (width + num), rectangle.Height))
                Dim collection As New List(Of List(Of Point))
                Dim m As Integer
                For m = 0 To list2.Count - 1
                    collection.Add(New List(Of Point))
                Next m
                Dim point As Point
                For Each point In connectedComponents.Item(index)
                    Dim n As Integer
                    For n = 0 To list2.Count - 1
                        Dim rectangle5 As Rectangle = list2.Item(n)
                        If (point.X >= rectangle5.X) Then
                            Dim rectangle6 As Rectangle = list2.Item(n)
                            If (point.X < rectangle6.Right) Then
                                collection.Item(n).Add(point)
                            End If
                        End If
                    Next n
                Next
                connectedComponents.RemoveAt(index)
                connectedComponents.InsertRange(index, collection)
            End If
            Dim i As Integer
            For i = 0 To connectedComponents.Count - 1
                Try
                    Dim rectangle3 As Rectangle = PicHelper.GetBoundingRect(connectedComponents.Item(i))
                    Dim num12 As Integer
                    For num12 = (i + 1) To connectedComponents.Count - 1
                        Try
                            If ((connectedComponents.Item(num12).Count < 5) OrElse (connectedComponents.Item(i).Count < 5)) Then
                                Dim rectangle4 As Rectangle = PicHelper.GetBoundingRect(connectedComponents.Item(num12))
                                If (((rectangle3.X <= rectangle4.Right) AndAlso (rectangle3.Right >= rectangle4.Left)) OrElse ((rectangle4.X <= rectangle3.Right) AndAlso (rectangle4.Right >= rectangle3.Left))) Then
                                    connectedComponents.Item(i).AddRange(connectedComponents.Item(num12))
                                    connectedComponents.RemoveAt(num12)
                                End If
                            End If
                        Catch ex As Exception
                            Dim e As New LSWFrameworkException(ex)
                        End Try
                    Next num12
                Catch ex As Exception
                    Dim e As New LSWFrameworkException(ex)
                End Try
            Next i
            Return connectedComponents
        End Function

        Private Function SegmentChar_Fixed(ByVal bm As Bitmap) As List(Of List(Of Point))
            Dim list As New List(Of List(Of Point))
            Dim graphics As Graphics = graphics.FromImage(bm)
            Dim rectangle As Rectangle
            For Each rectangle In Me.m_FixedRects
                Dim item As New List(Of Point)
                Dim j As Integer
                For j = rectangle.X To rectangle.Right - 1
                    Dim k As Integer
                    For k = rectangle.Y To rectangle.Bottom - 1
                        If (bm.GetPixel(j, k).R = 0) Then
                            item.Add(New Point(j, k))
                        End If
                    Next k
                Next j
                list.Add(item)
            Next
            Dim pen As New Pen(Color.FromArgb(0, 0, &HFF))
            Dim i As Integer
            For i = 0 To Me.m_FixedRects.Count - 1
                graphics.DrawRectangle(pen, Me.m_FixedRects.Item(i))
            Next i
            graphics.Dispose()
            Return list
        End Function

        Private Function SegmentChar_Project(ByVal bm As Bitmap) As List(Of List(Of Point))
            Dim list As New List(Of List(Of Point))
            Dim statisticsRet As List(Of Double) = PicHelper.ProjectHorizontally(bm)
            Dim list3 As List(Of Double) = PicHelper.ProjectVertically(bm)
            Dim threshold As Double = 0
            Dim num2 As Double = 0
            Dim i As Integer
            For i = 0 To statisticsRet.Count - 1
                threshold = (threshold + statisticsRet.Item(i))
            Next i
            threshold = (threshold / CDbl(statisticsRet.Count))
            Dim ranges As List(Of List(Of Integer)) = Me.GetRanges(statisticsRet, threshold)
            Dim j As Integer
            For j = 0 To list3.Count - 1
                num2 = (num2 + list3.Item(j))
            Next j
            num2 = (num2 / CDbl(list3.Count))
            Me.GetRanges(list3, num2)
            Dim graphics As Graphics = graphics.FromImage(bm)
            Dim pen As New Pen(Color.FromArgb(0, 0, &HFF))
            Dim k As Integer
            For k = 0 To ranges.Count - 1
                graphics.DrawLine(pen, ranges.Item(k).Item(0), 0, (ranges.Item(k).Item(0) - 1), bm.Width)
                graphics.DrawLine(pen, ranges.Item(k).Item(1), 0, (ranges.Item(k).Item(1) - 1), bm.Width)
            Next k
            graphics.Dispose()
            Return list
        End Function

        ' Properties
        Public Property CharCount As Integer

        Public Property FixedRects As List(Of Rectangle)
            Get
                Return Me.m_FixedRects
            End Get
            Set(ByVal value As List(Of Rectangle))
                Me.m_FixedRects = value
            End Set
        End Property

        Public Property SegmentCharType As SegmentCharType
            Get
                Return Me.m_SegmentCharType
            End Get
            Set(ByVal value As SegmentCharType)
                Me.m_SegmentCharType = value
            End Set
        End Property


        ' Fields
        Private m_FixedRects As List(Of Rectangle) = New List(Of Rectangle)
        Private m_SegmentCharType As SegmentCharType
    End Class
End Namespace


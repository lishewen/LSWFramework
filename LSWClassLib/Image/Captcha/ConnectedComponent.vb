Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Image.Captcha
    Friend Class ConnectedComponent
        ' Methods

        Private Function Dfs(ByVal stk As Stack(Of Point), ByVal bm As Bitmap, ByRef pos As Point, ByRef ValidRect As stRect, ByVal Visited(,) As Boolean) As List(Of Point)
            If (pos.X = 10) Then
                Dim y As Integer = pos.Y
            End If
            If Visited(pos.Y, pos.X) Then
                Return Nothing
            End If
            Visited(pos.Y, pos.X) = True
            Dim pixel As Color = bm.GetPixel(pos.X, pos.Y)
            If Not Me.m_fnCompareColor.Invoke(pixel) Then
                Return Nothing
            End If
            stk.Push(pos)
            Dim list As New List(Of Point)
            Do While (stk.Count > 0)
                pos = stk.Pop
                list.Add(pos)
                Me.GetConnectivity8PosList(pos)
                Dim point As Point
                For Each point In Me.m_Positions
                    If ((((point.X >= ValidRect.Left) AndAlso (point.X <= ValidRect.Right)) AndAlso ((point.Y >= ValidRect.Top) AndAlso (point.Y <= ValidRect.Bottom))) AndAlso Not Visited(point.Y, point.X)) Then
                        Visited(point.Y, point.X) = True
                        pixel = bm.GetPixel(point.X, point.Y)
                        If Me.m_fnCompareColor.Invoke(pixel) Then
                            stk.Push(point)
                        End If
                    End If
                Next
            Loop
            Return list
        End Function

        Public Function GetConnectedComponents(ByVal bm As Bitmap) As List(Of List(Of Point))
            Dim rect As stRect
            Dim width As Integer = bm.Width
            Dim height As Integer = bm.Height
            Dim stk As New Stack(Of Point)
            Dim list As New List(Of List(Of Point))
            If (((Me.m_rectRoi.Left = 0) AndAlso (Me.m_rectRoi.Right = 0)) AndAlso ((Me.m_rectRoi.Top = 0) AndAlso (Me.m_rectRoi.Bottom = 0))) Then
                rect.Left = 0
                rect.Right = (width - 1)
                rect.Top = 0
                rect.Bottom = (height - 1)
            Else
                rect.Left = Me.m_rectRoi.Left
                rect.Right = Me.m_rectRoi.Right
                rect.Top = Me.m_rectRoi.Top
                rect.Bottom = Me.m_rectRoi.Bottom
            End If
            Dim visited(,) As Boolean = New Boolean(height - 1, width - 1) {}
            Dim i As Integer
            For i = 0 To height - 1
                Dim k As Integer
                For k = 0 To width - 1
                    visited(i, k) = False
                Next k
            Next i
            Dim j As Integer
            For j = 0 To width - 1
                Dim m As Integer
                For m = 0 To height - 1
                    Dim pos As New Point(j, m)
                    Dim item As List(Of Point) = Me.Dfs(stk, bm, pos, rect, visited)
                    If (Not item Is Nothing) Then
                        list.Add(item)
                    End If
                Next m
            Next j
            Return list
        End Function

        Private Sub GetConnectivity8PosList(ByRef pos As Point)
            Me.m_Positions(0).X = pos.X
            Me.m_Positions(0).Y = (pos.Y - 1)
            Me.m_Positions(1).X = pos.X
            Me.m_Positions(1).Y = (pos.Y + 1)
            Me.m_Positions(2).X = (pos.X - 1)
            Me.m_Positions(2).Y = pos.Y
            Me.m_Positions(3).X = (pos.X + 1)
            Me.m_Positions(3).Y = pos.Y
            Me.m_Positions(4).X = (pos.X - 1)
            Me.m_Positions(4).Y = (pos.Y - 1)
            Me.m_Positions(5).X = (pos.X + 1)
            Me.m_Positions(5).Y = (pos.Y - 1)
            Me.m_Positions(6).X = (pos.X - 1)
            Me.m_Positions(6).Y = (pos.Y + 1)
            Me.m_Positions(7).X = (pos.X + 1)
            Me.m_Positions(7).Y = (pos.Y + 1)
        End Sub


        ' Properties
        Public Property CompareColorCallback As CompareColor
            Get
                Return Me.m_fnCompareColor
            End Get
            Set(ByVal value As CompareColor)
                Me.m_fnCompareColor = value
            End Set
        End Property

        Public Property Roi As Rectangle
            Get
                Return Me.m_rectRoi
            End Get
            Set(ByVal value As Rectangle)
                Me.m_rectRoi = value
            End Set
        End Property


        ' Fields
        Private m_fnCompareColor As CompareColor = Function(ByRef cr) ((cr.R = 0) AndAlso (cr.G = 0)) AndAlso (cr.B = 0) 'cr >= (((cr.R = 0) AndAlso (cr.G = 0)) AndAlso (cr.B = 0))
        Private m_Positions As Point() = New Point(8 - 1) {}
        Private m_rectRoi As Rectangle = New Rectangle(0, 0, 0, 0)

        ' Nested Types
        Public Delegate Function CompareColor(ByRef cr As Color) As Boolean

        Public Enum ExtractConnectedComponentType
            ' Fields
            Connectivity_4 = 1
            Connectivity_8 = 2
        End Enum

        <StructLayout(LayoutKind.Sequential)> _
        Private Structure stPoint
            Public X As Integer
            Public Y As Integer
            Public Sub New(ByVal Row As Integer, ByVal Col As Integer)
                Me.Y = Row
                Me.X = Col
            End Sub
        End Structure

        <StructLayout(LayoutKind.Sequential)> _
        Private Structure stRect
            Public Left As Integer
            Public Right As Integer
            Public Top As Integer
            Public Bottom As Integer
        End Structure
    End Class
End Namespace


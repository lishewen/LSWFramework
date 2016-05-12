Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text

Namespace Image.Captcha.Classification
    Public Class ClassificationBase
        ' Methods
        Public Overridable Function Classify(ByVal Feature As List(Of Double)) As String
            Return ""
        End Function

        Public Overridable Function Classify(ByVal Features As List(Of List(Of Double))) As String
            Return ""
        End Function

        Public Shared Function FeatureToString(ByVal Feature As List(Of Double)) As String
            Dim builder As New StringBuilder
            Dim num As Integer = (Feature.Count - 1)
            Dim i As Integer
            For i = 0 To num - 1
                builder.Append(Feature.Item(i).ToString)
                builder.Append(",")
            Next i
            If (Feature.Count > 0) Then
                builder.Append(Feature.Item(num).ToString)
            End If
            Return builder.ToString
        End Function

        Public Overridable Sub LoadSamplesFromFile(ByVal strFileName As String)
            Using reader As StreamReader = New StreamReader(strFileName, Encoding.Default)
                Dim strSamples As String = reader.ReadToEnd
                Me.LoadSamplesFromText(strSamples)
            End Using
        End Sub

        Public Overridable Sub LoadSamplesFromText(ByVal strSamples As String)
            Try
                Me.m_Classifications.Clear()
                Dim str As String
                For Each str In strSamples.Split(New String() {ChrW(10)}, StringSplitOptions.RemoveEmptyEntries)
                    Dim strArray2 As String() = str.Split(New String() {"="}, StringSplitOptions.RemoveEmptyEntries)
                    If (strArray2.Length = 2) Then
                        Dim list2 As New List(Of Double)
                        Dim key As String = strArray2(0).Trim
                        Dim strArray3 As String() = strArray2(1).Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
                        If Not Me.m_Classifications.ContainsKey(key) Then
                            Me.m_Classifications.Item(key) = New List(Of List(Of Double))
                        End If
                        Dim list As List(Of List(Of Double)) = Me.m_Classifications.Item(key)
                        'list2 = New List(Of Double) From {list2}
                        Dim str3 As String
                        For Each str3 In strArray3
                            If (str3.IndexOf("非数字") <> -1) Then
                                list2.Add(0)
                            Else
                                list2.Add(Double.Parse(str3))
                            End If
                        Next
                        list.Add(list2)
                    End If
                Next
            Catch exception1 As Exception
            End Try
        End Sub


        ' Fields
        Protected m_Classifications As Dictionary(Of String, List(Of List(Of Double))) = New Dictionary(Of String, List(Of List(Of Double)))
    End Class
End Namespace


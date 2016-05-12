Imports System
Imports System.Collections.Generic

Namespace Image.Captcha.Classification
    Friend Class NearestDistanceClassification
        Inherits ClassificationBase
        ' Methods
        Public Overrides Function Classify(ByVal Features As List(Of List(Of Double))) As String
            Dim str As String = ""
            Dim i As Integer
            For i = 0 To Features.Count - 1
                Dim feature As List(Of Double) = Features.Item(i)
                Dim str2 As String = Me.Classify(feature)
                str = (str & str2)
            Next i
            Return str
        End Function

        Public Overrides Function Classify(ByVal Feature As List(Of Double)) As String
            Dim num As Double = 268435455
            Dim key As String = ""
            Dim pair As KeyValuePair(Of String, List(Of List(Of Double)))
            For Each pair In MyBase.m_Classifications
                Dim list As List(Of Double)
                For Each list In pair.Value
                    Dim num2 As Double = Me.Match(Feature, list)
                    If (num2 < num) Then
                        num = num2
                        key = pair.Key
                    End If
                Next
            Next
            Return key
        End Function

        Private Function Match(ByVal Feature1 As List(Of Double), ByVal Feature2 As List(Of Double)) As Double
            Dim num As Double = 0
            Dim count As Integer = Feature1.Count
            Dim i As Integer
            For i = 0 To count - 1
                Dim num4 As Double = (Feature1.Item(i) - Feature2.Item(i))
                num = (num + (num4 * num4))
            Next i
            Return num
        End Function

    End Class
End Namespace


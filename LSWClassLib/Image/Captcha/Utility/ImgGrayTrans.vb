Imports System
Imports System.Drawing

Namespace Image.Captcha.Utility
    Friend Class ImgGrayTrans
        ' Methods
        Public Shared Function GetGrayScale(ByVal cr As Color) As Integer
            Return ((((cr.R * &H4C8B) + (cr.G * &H9645)) + (cr.B * &H1D30)) >> &H10)
        End Function

        Public Shared Sub Gray(ByVal bm As Bitmap)
            Dim i As Integer
            For i = 0 To bm.Width - 1
                Dim j As Integer
                For j = 0 To bm.Height - 1
                    Dim grayScale As Integer = ImgGrayTrans.GetGrayScale(bm.GetPixel(i, j))
                    bm.SetPixel(i, j, Color.FromArgb(grayScale, grayScale, grayScale))
                Next j
            Next i
        End Sub

        Public Shared Sub Reverse(ByVal bm As Bitmap)
            Dim width As Integer = bm.Width
            Dim height As Integer = bm.Height
            Dim i As Integer
            For i = 0 To width - 1
                Dim j As Integer
                For j = 0 To height - 1
                    Dim pixel As Color = bm.GetPixel(i, j)
                    bm.SetPixel(i, j, Color.FromArgb((&HFF - pixel.R), (&HFF - pixel.G), (&HFF - pixel.B)))
                Next j
            Next i
        End Sub

        Public Shared Sub Threshold(ByVal bm As Bitmap, ByVal nThreshold As Integer)
            ImgGrayTrans.Threshold(bm, nThreshold, &HFF, ThresholdType.Binary)
        End Sub

        Public Shared Sub Threshold(ByVal bm As Bitmap, ByVal nThreshold As Integer, ByVal MaxValue As Integer, ByVal ThresholdType As ThresholdType)
            Dim num4 As Integer
            Select Case ThresholdType
                Case ThresholdType.Binary
                    Dim i As Integer
                    For i = 0 To bm.Width - 1
                        Dim j As Integer
                        For j = 0 To bm.Height - 1
                            If (ImgGrayTrans.GetGrayScale(bm.GetPixel(i, j)) > nThreshold) Then
                                bm.SetPixel(i, j, Color.FromArgb(MaxValue, MaxValue, MaxValue))
                            Else
                                bm.SetPixel(i, j, Color.FromArgb(0, 0, 0))
                            End If
                        Next j
                    Next i
                    Return
                Case ThresholdType.BinaryInversely
                    num4 = 0
                    Exit Select
                Case Else
                    Return
            End Select
            Do While (num4 < bm.Width)
                Dim k As Integer
                For k = 0 To bm.Height - 1
                    If (ImgGrayTrans.GetGrayScale(bm.GetPixel(num4, k)) > nThreshold) Then
                        bm.SetPixel(num4, k, Color.FromArgb(0, 0, 0))
                    Else
                        bm.SetPixel(num4, k, Color.FromArgb(MaxValue, MaxValue, MaxValue))
                    End If
                Next k
                num4 += 1
            Loop
        End Sub


        ' Nested Types
        Public Enum ThresholdType
            ' Fields
            Binary = 0
            BinaryInversely = 1
        End Enum
    End Class
End Namespace


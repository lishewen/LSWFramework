Namespace Flash
    Public Class FlashUpload
        Public Function FlashDataDecode(s As String) As Byte()
            Dim r As Byte() = New Byte(s.Length \ 2 - 1) {}
            Dim l As Integer = s.Length
            Dim i As Integer = 0
            While i < l
                Dim k1 As Integer = (AscW(s(i))) - 48
                k1 -= If(k1 > 9, 7, 0)
                Dim k2 As Integer = (AscW(s(i + 1))) - 48
                k2 -= If(k2 > 9, 7, 0)
                r(i \ 2) = CByte(k1 << 4 Or k2)
                i = i + 2
            End While
            Return r
        End Function
    End Class
End Namespace
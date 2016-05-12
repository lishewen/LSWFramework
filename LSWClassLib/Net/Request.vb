Namespace Net
    Public Class Request
        Sub New(content As String)
            Dim lines = content.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries)
            Method = lines(0).Split(" ")(0)
            RawUrl = lines(0).Split(" ")(1)
        End Sub

        Public Property RawUrl As String
        Public Property Method As String
    End Class
End Namespace
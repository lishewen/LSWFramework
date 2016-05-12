Namespace Web
    Public Module UrlMake
        Function getUrlEncodel(ByVal Url As String) As String
            Dim i, code As Integer
            getUrlEncodel = ""
            If Trim(Url) = "" Then Exit Function
            For i = 1 To Len(Url)
                code = Asc(Mid(Url, i, 1))
                If code < 0 Then code = code + 65536
                If code > 255 Then
                    getUrlEncodel = getUrlEncodel & "%" & Left(Hex(code), 2) & "%" & Right(Hex(code), 2)
                Else
                    getUrlEncodel = getUrlEncodel & Mid(Url, i, 1)
                End If
            Next
        End Function
    End Module
End Namespace
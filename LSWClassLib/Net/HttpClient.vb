Imports System.Net

Namespace Net
    Public Class HttpClient
        Inherits WebClient
        ' Methods
        Public Sub New()
            Me.Cookies = New CookieContainer
        End Sub

        Public Sub New(ByVal cookies As CookieContainer)
            Me.Cookies = cookies
        End Sub

        Protected Overrides Function GetWebRequest(ByVal address As Uri) As WebRequest
            Dim webRequest As WebRequest = MyBase.GetWebRequest(address)
            If TypeOf webRequest Is HttpWebRequest Then
                Dim request2 As HttpWebRequest = TryCast(webRequest, HttpWebRequest)
                request2.CookieContainer = Me.Cookies
            End If
            Return webRequest
        End Function

        Public Function GetCookieString(url As String) As String
            Dim cookieString = ""
            For Each cookie In Cookies.GetCookies(New Uri(url))
                cookieString &= cookie.Name & "=" & cookie.Value & ";"
            Next
            Return cookieString
        End Function

        ' Properties
        Public Property Cookies As CookieContainer
    End Class
End Namespace

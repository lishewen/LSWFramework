Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Web

Namespace Extension
    Public Module CookiesHelper
        <Extension()> _
        Public Function ToCookieContainer(cookieCollection As CookieCollection) As CookieContainer
            'Initializing a instance of cookie container        
            Dim cookies As New CookieContainer()
            'Loops through the cookie collection items  
            For Each c In cookieCollection
                'Get the cookie from http request      
                Dim requestCookie As Cookie = c
                'Creating a cookie       
                Dim cookie As New Cookie(requestCookie.Name, requestCookie.Value, requestCookie.Path)
                'Sets the expiration    
                cookie.Expires = requestCookie.Expires
                'Sets the security level    
                cookie.Secure = requestCookie.Secure
                'Sets the URI for which the System.Net.Cookie is valid   
                cookie.Domain = If(String.IsNullOrEmpty(requestCookie.Domain), HttpContext.Current.Server.MachineName, requestCookie.Domain)
                'Add a cookie to cookie container      
                cookies.Add(cookie)
            Next
            'returns the cookies collection       
            Return cookies
        End Function
    End Module
End Namespace
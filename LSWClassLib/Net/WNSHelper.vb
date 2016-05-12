Imports System.Runtime.Serialization
Imports System.IO
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Web
Imports System.Net
Imports LSW.Exceptions

Namespace Net
    Public Module WNSHelper
        <DataContract>
        Public Class OAuthToken
            <DataMember(Name:="access_token")>
            Public Property AccessToken() As String
            <DataMember(Name:="token_type")>
            Public Property TokenType() As String
        End Class

        Public Function GetOAuthTokenFromJson(jsonString As String) As OAuthToken
            Using ms = New MemoryStream(Encoding.Unicode.GetBytes(jsonString))
                Dim ser = New DataContractJsonSerializer(GetType(OAuthToken))
                Dim oAuthToken = DirectCast(ser.ReadObject(ms), OAuthToken)
                Return oAuthToken
            End Using
        End Function

        Public Function GetAccessToken(sid As String, secret As String) As OAuthToken
            Dim urlEncodedSecret = HttpUtility.UrlEncode(secret)
            Dim urlEncodedSid = HttpUtility.UrlEncode(sid)

            Dim body = String.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", urlEncodedSid, urlEncodedSecret)

            Dim response As String
            Using client = New WebClient()
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
                response = client.UploadString("https://login.live.com/accesstoken.srf", body)
            End Using
            Return GetOAuthTokenFromJson(response)
        End Function

        Public Function PostToWns(sid As String, secret As String, uri As String, data As String, type As String) As String
            Try
                Dim accessToken = GetAccessToken(sid, secret)
                Dim request = DirectCast(HttpWebRequest.Create(uri), HttpWebRequest)
                request.Method = "POST"
                request.Headers.Add("X-WNS-Type", type)
                request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken.AccessToken))
                Dim bs = Encoding.UTF8.GetBytes(data)
                Using rs = request.GetRequestStream
                    rs.Write(bs, 0, bs.Length)
                End Using
                Using res = CType(request.GetResponse, HttpWebResponse)
                    Return res.StatusCode.ToString
                End Using
            Catch ex As Exception
                Dim e As New LSWFrameworkException(ex)
                Return "异常：" & e.Message
            End Try
        End Function
    End Module
End Namespace
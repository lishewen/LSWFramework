Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Text.RegularExpressions
Imports LSW.Extension
Imports System.Threading.Tasks
Imports System.Drawing

Namespace Net
    ''' <summary>
    ''' URL处理相关类
    ''' </summary>
    Public Module Url
        Public UserAgent As String = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; Trident/6.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729; Zune 4.7; InfoPath.3)"
        Public Referer As String = "http://www.lishewen.com/"
#Region "远程获取url地址的页面源代码"
        ''' <summary>
        ''' 远程获取url地址的页面源代码
        ''' </summary>
        ''' <param name="url">要获取页面的URL</param>
        ''' <returns>返回HTML代码</returns>
        Public Function GetHtml(url As String) As String
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(url), HttpWebRequest)
                request.UserAgent = UserAgent

                request.Timeout = 20000
                request.AllowAutoRedirect = True
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then
                    reader = New StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8)
                    Dim html As String = reader.ReadToEnd()
                    Return html
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return String.Empty
        End Function

        ''' <summary>
        ''' 远程获取url地址的页面源代码
        ''' </summary>
        ''' <param name="url">要获取页面的URL</param>
        ''' <returns>返回HTML代码</returns>
        Public Function GetHtml(url As String, cc As CookieContainer) As String
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(url), HttpWebRequest)
                request.UserAgent = "www.lishewen.com"

                request.Timeout = 20000
                request.AllowAutoRedirect = True
                request.CookieContainer = cc

                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then
                    reader = New StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8)
                    Dim html As String = reader.ReadToEnd()
                    Return html
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return String.Empty
        End Function

        Public Function GetCookies(url As String, cc As CookieContainer) As CookieContainer
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(url), HttpWebRequest)
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; Trident/6.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729; Zune 4.7; InfoPath.3)"
                'request.Referer = "http://bbs.haoshi.cc/index.php?m=u&c=login"
                'request.Host = "bbs.haoshi.cc"
                'request.Headers.Add("DNT", "1")

                request.Timeout = 20000
                request.AllowAutoRedirect = True
                request.CookieContainer = cc

                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then
                    Return response.Cookies.ToCookieContainer
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return New CookieContainer
        End Function

        Public Function GetBitmap(url As String) As Bitmap
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(url), HttpWebRequest)
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; Trident/6.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729; Zune 4.7; InfoPath.3)"

                request.Timeout = 20000
                request.AllowAutoRedirect = True

                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then
                    Return DirectCast(Bitmap.FromStream(response.GetResponseStream), Bitmap)
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return New Bitmap(0, 0)
        End Function

        Public Function GetBitmapAndCookies(url As String, cc As CookieContainer) As WebInfo
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(url), HttpWebRequest)
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.2; Trident/6.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729; Zune 4.7; InfoPath.3)"

                request.Timeout = 20000
                request.AllowAutoRedirect = True
                request.CookieContainer = cc

                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then
                    Dim web As New WebInfo
                    web.Bmp = DirectCast(Bitmap.FromStream(response.GetResponseStream), Bitmap)
                    web.cookieContainer = response.Cookies.ToCookieContainer
                    Return web
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return Nothing
        End Function
#End Region

#Region "远程获取url地址的页面源代码"
        ''' <summary>
        ''' 远程获取url地址的页面源代码
        ''' </summary>
        ''' <param name="url">要获取页面的URL</param>
        ''' <returns>返回HTML代码</returns>
        Public Function GetHtml(url As String, ucoid As String) As String
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(url), HttpWebRequest)
                request.UserAgent = "www.lishewen.com"


                request.Timeout = 20000
                request.AllowAutoRedirect = True
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then

                    reader = New StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(ucoid))

                    Dim html As String = reader.ReadToEnd()
                    Return html
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return String.Empty
        End Function
#End Region

#Region "远程获取url地址的页面源代码"
        ''' <summary>
        ''' 远程获取url地址的页面源代码
        ''' </summary>
        ''' <param name="url">要获取页面的URL</param>
        ''' <returns>返回HTML代码</returns>
        Public Function GetHtml(url As String, ucoid As String, cc As CookieContainer) As String
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(url), HttpWebRequest)
                request.UserAgent = "www.lishewen.com"
                request.CookieContainer = cc



                request.Timeout = 20000
                request.AllowAutoRedirect = True
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then
                    If ucoid = "utf-8" Then
                        reader = New StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8)
                    ElseIf ucoid = "gb2312" Then
                        reader = New StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("gb2312"))
                    End If
                    Dim html As String = reader.ReadToEnd()
                    Return html
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return String.Empty
        End Function


#End Region

#Region "将数据提交到指定URL"
        ''' <summary>
        ''' 将数据提交到指定URL
        ''' </summary>
        ''' <param name="postVars">要提交的表单数据</param>
        ''' <param name="Url">目标URL</param>
        ''' <returns>URL响应出的字符串</returns>
        Public Function Post(postVars As System.Collections.Specialized.NameValueCollection, Url As String, encode As System.Text.Encoding) As String
            Dim WebClientObj As New System.Net.WebClient()
            Try
                Dim byRemoteInfo As Byte() = WebClientObj.UploadValues(Url, "Post", postVars)

                Dim sRemoteInfo As String = encode.GetString(byRemoteInfo)
                'System.Text.Encoding.Default.GetString(byRemoteInfo);
                Return sRemoteInfo
            Catch
                Return String.Empty
            End Try
        End Function

        Public Function Post(postVars As System.Collections.Specialized.NameValueCollection, Url As String) As String
            Dim WebClientObj As New System.Net.WebClient()
            Try
                Dim byRemoteInfo As Byte() = WebClientObj.UploadValues(Url, "Post", postVars)

                Dim sRemoteInfo As String = System.Text.Encoding.[Default].GetString(byRemoteInfo)
                Return sRemoteInfo
            Catch
                Return String.Empty
            End Try
        End Function

        Public Function Post(postVars As System.Collections.Specialized.NameValueCollection, Url As String, encode As System.Text.Encoding, cc As CookieContainer) As String
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(Url), HttpWebRequest)
                'request.SendChunked = true;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2)"
                request.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*"
                ' request.TransferEncoding = "gzip, deflate";
                request.Referer = "http://www.lishewen.com/"
                'request.Connection = "Keep-Alive";
                request.CookieContainer = cc
                request.Method = "post"
                'request.Timeout = 99999999

                Dim param As String = ""

                For i As Integer = 0 To postVars.Count - 1
                    param += postVars.Keys(i) & "=" & postVars(i)
                    If i <> postVars.Count - 1 Then
                        param += "&"
                    End If
                Next

                Dim bs As Byte() = encode.GetBytes(param)

                request.ContentType = "application/x-www-form-urlencoded"
                request.ContentLength = bs.Length


                request.Timeout = 20000
                request.AllowAutoRedirect = True
                Using reqStream As Stream = request.GetRequestStream()
                    reqStream.Write(bs, 0, bs.Length)
                End Using

                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then

                    reader = New StreamReader(response.GetResponseStream(), encode)
                    Dim html As String = reader.ReadToEnd()
                    Return html
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return String.Empty
        End Function

        Public Function Post(postVars As System.Collections.Specialized.NameValueCollection, Url As String, encode As System.Text.Encoding, cc As CookieContainer, Accept As String, Referer As String, UserAgent As String) As String
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(Url), HttpWebRequest)
                request.UserAgent = UserAgent
                request.Accept = Accept
                request.Referer = Referer
                request.CookieContainer = cc
                request.Method = "post"

                Dim param As String = ""

                For i As Integer = 0 To postVars.Count - 1
                    param += postVars.Keys(i) & "=" & postVars(i)
                    If i <> postVars.Count - 1 Then
                        param += "&"
                    End If
                Next

                Dim bs As Byte() = encode.GetBytes(param)

                request.ContentType = "application/x-www-form-urlencoded"
                request.ContentLength = bs.Length


                request.Timeout = 20000
                request.AllowAutoRedirect = True
                Using reqStream As Stream = request.GetRequestStream()
                    reqStream.Write(bs, 0, bs.Length)
                End Using

                response = DirectCast(request.GetResponse(), HttpWebResponse)


                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then

                    reader = New StreamReader(response.GetResponseStream(), encode)
                    Dim html As String = reader.ReadToEnd()
                    Return html
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return String.Empty
        End Function

        Public Function PostAndGetCookie(postVars As System.Collections.Specialized.NameValueCollection, Url As String, encode As Encoding, cookieContainer As CookieContainer) As CookieCollection
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(Url), HttpWebRequest)
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2)"
                request.Referer = "http://www.lishewen.com/"
                request.CookieContainer = cookieContainer
                request.Method = "post"

                Dim param As String = ""

                For i As Integer = 0 To postVars.Count - 1
                    param += postVars.Keys(i) & "=" & postVars(i)
                    If i <> postVars.Count - 1 Then
                        param += "&"
                    End If
                Next

                Dim bs As Byte() = encode.GetBytes(param)

                request.ContentType = "application/x-www-form-urlencoded"
                request.ContentLength = bs.Length


                request.Timeout = 20000
                request.AllowAutoRedirect = True
                Using reqStream As Stream = request.GetRequestStream()
                    reqStream.Write(bs, 0, bs.Length)
                End Using

                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK AndAlso response.ContentLength < 1024 * 1024 Then
                    reader = New StreamReader(response.GetResponseStream(), encode)
                    Dim html As String = reader.ReadToEnd()
                    Return response.Cookies
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return Nothing

        End Function

        ''' <summary>
        ''' 提交数据获取Cookie
        ''' </summary>
        ''' <param name="postVars"></param>
        ''' <param name="Url"></param>
        ''' <param name="encode"></param>
        ''' <returns></returns>
        Public Function PostAndGetCookie(postVars As System.Collections.Specialized.NameValueCollection, Url As String, encode As Encoding) As CookieCollection
            Return PostAndGetCookie(postVars, Url, encode, New CookieContainer())
        End Function

        ''' <summary>
        ''' 提交数据获取Cookie和Html
        ''' </summary>
        ''' <param name="postVars"></param>
        ''' <param name="Url"></param>
        ''' <param name="encode"></param>
        ''' <returns></returns>
        Public Function PostGetCookieAndHtml(postVars As System.Collections.Specialized.NameValueCollection, Url As String, encode As Encoding) As WebInfo
            Return PostGetCookieAndHtml(postVars, Url, encode, New CookieContainer())
        End Function

        ''' <summary>
        ''' 提交数据获取Cookie和Html
        ''' </summary>
        ''' <param name="postVars"></param>
        ''' <param name="Url"></param>
        ''' <param name="encode"></param>
        ''' <returns></returns>
        Public Function PostGetCookieAndHtml(postVars As System.Collections.Specialized.NameValueCollection, Url As String, encode As Encoding, cookieContainer As CookieContainer) As WebInfo
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader = Nothing
            Try
                request = DirectCast(WebRequest.Create(Url), HttpWebRequest)
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2)"
                request.Referer = "http://www.lishewen.com/"
                request.CookieContainer = cookieContainer
                request.Method = "post"
                request.AllowAutoRedirect = False

                Dim param As String = ""

                For i As Integer = 0 To postVars.Count - 1
                    param += postVars.Keys(i) & "=" & postVars(i)
                    If i <> postVars.Count - 1 Then
                        param += "&"
                    End If
                Next

                Dim bs As Byte() = encode.GetBytes(param)

                request.ContentType = "application/x-www-form-urlencoded"
                request.ContentLength = bs.Length


                request.Timeout = 20000
                'request.AllowAutoRedirect = true;
                Using reqStream As Stream = request.GetRequestStream()
                    reqStream.Write(bs, 0, bs.Length)
                End Using

                response = DirectCast(request.GetResponse(), HttpWebResponse)
                If (response.StatusCode = HttpStatusCode.OK OrElse response.StatusCode = HttpStatusCode.Found) AndAlso response.ContentLength < 1024 * 1024 Then

                    reader = New StreamReader(response.GetResponseStream(), encode)
                    Dim html As String = reader.ReadToEnd()

                    Dim web As New WebInfo()
                    web.cookieContainer = response.Cookies.ToCookieContainer()
                    web.Html = html

                    Return web
                End If
            Catch
            Finally
                If response IsNot Nothing Then
                    response.Close()
                    response = Nothing
                End If
                If reader IsNot Nothing Then
                    reader.Close()
                End If
                If request IsNot Nothing Then
                    request = Nothing
                End If
            End Try
            Return Nothing
        End Function

        Public Function PostRequest(postData As String, uriStr As String, encode As Encoding, ByRef cookieContainer As CookieContainer) As String
            Dim req As HttpWebRequest = DirectCast(WebRequest.Create(uriStr), HttpWebRequest)
            req.UserAgent = UserAgent
            req.Referer = Referer
            req.Method = "POST"
            req.ContentType = "application/x-www-form-urlencoded"
            req.AllowAutoRedirect = False
            Dim postDatas As Byte() = encode.GetBytes(postData)
            req.ContentLength = postDatas.Length
            req.CookieContainer = cookieContainer
            Using sm As Stream = req.GetRequestStream()
                sm.Write(postDatas, 0, postDatas.Length)
            End Using
            Dim strResult As String
            Dim res As HttpWebResponse = DirectCast(req.GetResponse(), HttpWebResponse)
            res.Cookies = cookieContainer.GetCookies(req.RequestUri)
            Using smRes As Stream = res.GetResponseStream()
                Using sr As New StreamReader(smRes, encode)
                    strResult = sr.ReadToEnd()
                End Using
            End Using
            cookieContainer = res.Cookies.ToCookieContainer
            res.Close()
            Return strResult
        End Function

        Public Function PostRequest(postData As String, uriStr As String, encode As Encoding) As String
            Return PostRequest(postData, uriStr, encode, New CookieContainer)
        End Function

        Public Function PostRequest(postData As String, uriStr As String, ByRef cookieContainer As CookieContainer) As String
            Return PostRequest(postData, uriStr, Encoding.UTF8, cookieContainer)
        End Function

        Public Function PostRequest(postData As String, uriStr As String) As String
            Return PostRequest(postData, uriStr, Encoding.UTF8)
        End Function
#End Region

        Public Function GetLinks(html As String) As List(Of String)
            Const pattern As String = "http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"
            Dim r As New Regex(pattern, RegexOptions.IgnoreCase)
            Dim m As MatchCollection = r.Matches(html)
            Dim links As New List(Of String)

            For Each i As Match In m
                links.Add(i.Value)
            Next
            Return links
        End Function

        Public Function UrlAvailable(url As String, unload As Dictionary(Of String, Integer), loaded As Dictionary(Of String, Integer)) As Boolean
            If unload.ContainsKey(url) OrElse loaded.ContainsKey(url) Then
                Return False
            End If
            If url.Contains(".jpg") OrElse url.Contains(".gif") OrElse url.Contains(".png") OrElse url.Contains(".css") OrElse url.Contains(".js") Then
                Return False
            End If
            Return True
        End Function

        ''' <summary>
        ''' 获取页面响应码
        ''' </summary>
        Public Function GetStatusCode(ByVal url As String, method As String, timetick As Integer) As Integer
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Try
                request = CType(WebRequest.Create(url), HttpWebRequest)
                request.ContentType = "application/x-www-form-urlencoded"
                request.UserAgent = "UrlStatusSpider/1.0 (urlstatus-lsw)"
                request.UserAgent = "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.186 Safari/535.1"
                request.Timeout = timetick * 1000 '超时时间10秒，默认100秒
                request.Method = method
                '如果请允许跳转，请求的地址跳转到最后一个页面的地址状态
                request.AllowAutoRedirect = False
                response = CType(request.GetResponse(), HttpWebResponse)
                Return CInt(response.StatusCode)
            Catch webex As WebException
                response = CType(webex.Response, HttpWebResponse)
                If response IsNot Nothing Then
                    Return CInt(response.StatusCode)
                End If
                Return -1
            Catch
                Return -1
            Finally
                If response IsNot Nothing Then
                    response.Close()
                End If
            End Try
        End Function

        Public Function GetStatusCode(ByVal url As String) As Integer
            Return GetStatusCode(url, "HEAD", 10)
        End Function

        Public Function ValidateUrlAsync(url As String) As Task(Of Boolean)
            Dim tcs As New TaskCompletionSource(Of Boolean)
            Try
                Dim request = CType(WebRequest.Create(url), HttpWebRequest)
                request.Method = "HEAD"
                request.BeginGetResponse(Sub(iar)
                                             Dim response As HttpWebResponse = Nothing
                                             Try
                                                 response = CType(request.EndGetResponse(iar), HttpWebResponse)
                                                 tcs.SetResult(response.StatusCode = HttpStatusCode.OK)
                                             Catch ex As Exception
                                                 tcs.SetException(ex)
                                             Finally
                                                 If response IsNot Nothing Then
                                                     response.Close()
                                                 End If
                                             End Try
                                         End Sub, Nothing)
            Catch ex As Exception
                tcs.SetException(ex)
            End Try
            Return tcs.Task
        End Function
    End Module

#Region "用户抓取操作返回数据类，如Cookie，HTML代码等"
    ''' <summary>
    ''' 用户抓取操作返回数据类，如Cookie，HTML代码等
    ''' </summary>
    Public Class WebInfo
        ''' <summary>
        ''' Cookie
        ''' </summary>
        Public Property cookieContainer As CookieContainer

        ''' <summary>
        ''' Html源文件
        ''' </summary>
        Public Property Html As String

        Public Property Bmp As Bitmap
    End Class
#End Region
End Namespace
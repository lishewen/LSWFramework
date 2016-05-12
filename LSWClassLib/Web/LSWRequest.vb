Imports System.Web

Namespace Web
    ''' <summary> 
    ''' Request操作类 
    ''' </summary>
    Public Class LSWRequest
        ''' <summary> 
        ''' 判断当前页面是否接收到了Post请求 
        ''' </summary> 
        ''' <returns>是否接收到了Post请求</returns> 
        Public Shared Function IsPost() As Boolean
            Return HttpContext.Current.Request.HttpMethod.Equals("POST")
        End Function
        ''' <summary> 
        ''' 判断当前页面是否接收到了Get请求 
        ''' </summary> 
        ''' <returns>是否接收到了Get请求</returns> 
        Public Shared Function IsGet() As Boolean
            Return HttpContext.Current.Request.HttpMethod.Equals("GET")
        End Function

        ''' <summary> 
        ''' 返回指定的服务器变量信息 
        ''' </summary> 
        ''' <param name="strName">服务器变量名</param> 
        ''' <returns>服务器变量信息</returns> 
        Public Shared Function GetServerString(ByVal strName As String) As String
            ' 
            If HttpContext.Current.Request.ServerVariables(strName) Is Nothing Then
                Return ""
            End If
            Return HttpContext.Current.Request.ServerVariables(strName).ToString()
        End Function

        ''' <summary> 
        ''' 返回上一个页面的地址 
        ''' </summary> 
        ''' <returns>上一个页面的地址</returns> 
        Public Shared Function GetUrlReferrer() As String
            Dim retVal As String = Nothing

            Try
                retVal = HttpContext.Current.Request.UrlReferrer.ToString()
            Catch
            End Try

            If retVal Is Nothing Then
                Return ""
            End If

            Return retVal

        End Function

        ''' <summary> 
        ''' 得到当前完整主机头 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function GetCurrentFullHost() As String
            Dim request As HttpRequest = System.Web.HttpContext.Current.Request
            If Not request.Url.IsDefaultPort Then
                Return String.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString())
            End If
            Return request.Url.Host
        End Function

        ''' <summary> 
        ''' 得到主机头 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function GetHost() As String
            Return HttpContext.Current.Request.Url.Host
        End Function


        ''' <summary> 
        ''' 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在)) 
        ''' </summary> 
        ''' <returns>原始 URL</returns> 
        Public Shared Function GetRawUrl() As String
            Return HttpContext.Current.Request.RawUrl
        End Function

        ''' <summary> 
        ''' 判断当前访问是否来自浏览器软件 
        ''' </summary> 
        ''' <returns>当前访问是否来自浏览器软件</returns> 
        Public Shared Function IsBrowserGet() As Boolean
            Dim BrowserName As String() = {"ie", "opera", "netscape", "mozilla", "konqueror", "firefox"}
            Dim curBrowser As String = HttpContext.Current.Request.Browser.Type.ToLower()
            For i As Integer = 0 To BrowserName.Length - 1
                If curBrowser.IndexOf(BrowserName(i)) >= 0 Then
                    Return True
                End If
            Next
            Return False
        End Function

        ''' <summary> 
        ''' 判断是否来自搜索引擎链接 
        ''' </summary> 
        ''' <returns>是否来自搜索引擎链接</returns> 
        Public Shared Function IsSearchEnginesGet() As Boolean
            If HttpContext.Current.Request.UrlReferrer Is Nothing Then
                Return False
            End If
            Dim SearchEngine As String() = {"google", "yahoo", "msn", "baidu", "sogou", "sohu", _
            "sina", "163", "lycos", "tom", "yisou", "iask", _
            "soso", "gougou", "zhongsou"}
            Dim tmpReferrer As String = HttpContext.Current.Request.UrlReferrer.ToString().ToLower()
            For i As Integer = 0 To SearchEngine.Length - 1
                If tmpReferrer.IndexOf(SearchEngine(i)) >= 0 Then
                    Return True
                End If
            Next
            Return False
        End Function

        ''' <summary> 
        ''' 获得当前完整Url地址 
        ''' </summary> 
        ''' <returns>当前完整Url地址</returns> 
        Public Shared Function GetUrl() As String
            Return HttpContext.Current.Request.Url.ToString()
        End Function


        ''' <summary> 
        ''' 获得指定Url参数的值 
        ''' </summary> 
        ''' <param name="strName">Url参数</param> 
        ''' <returns>Url参数的值</returns> 
        Public Shared Function GetQueryString(ByVal strName As String) As String
            If HttpContext.Current.Request.QueryString(strName) Is Nothing Then
                Return ""
            End If
            Return HttpContext.Current.Request.QueryString(strName)
        End Function

        ''' <summary> 
        ''' 获得当前页面的名称 
        ''' </summary> 
        ''' <returns>当前页面的名称</returns> 
        Public Shared Function GetPageName() As String
            Dim urlArr As String() = HttpContext.Current.Request.Url.AbsolutePath.Split("/"c)
            Return urlArr(urlArr.Length - 1).ToLower()
        End Function

        ''' <summary> 
        ''' 返回表单或Url参数的总个数 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function GetParamCount() As Integer
            Return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count
        End Function


        ''' <summary> 
        ''' 获得指定表单参数的值 
        ''' </summary> 
        ''' <param name="strName">表单参数</param> 
        ''' <returns>表单参数的值</returns> 
        Public Shared Function GetFormString(ByVal strName As String) As String
            If HttpContext.Current.Request.Form(strName) Is Nothing Then
                Return ""
            End If
            Return HttpContext.Current.Request.Form(strName)
        End Function

        ''' <summary> 
        ''' 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值 
        ''' </summary> 
        ''' <param name="strName">参数</param> 
        ''' <returns>Url或表单参数的值</returns> 
        Public Shared Function GetString(ByVal strName As String) As String
            If "".Equals(GetQueryString(strName)) Then
                Return GetFormString(strName)
            Else
                Return GetQueryString(strName)
            End If
        End Function


        ''' <summary> 
        ''' 获得指定Url参数的int类型值 
        ''' </summary> 
        ''' <param name="strName">Url参数</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>Url参数的int类型值</returns> 
        Public Shared Function GetQueryInt(ByVal strName As String, ByVal defValue As Integer) As Integer
            Return Utils.StrToInt(HttpContext.Current.Request.QueryString(strName), defValue)
        End Function


        ''' <summary> 
        ''' 获得指定表单参数的int类型值 
        ''' </summary> 
        ''' <param name="strName">表单参数</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>表单参数的int类型值</returns> 
        Public Shared Function GetFormInt(ByVal strName As String, ByVal defValue As Integer) As Integer
            Return Utils.StrToInt(HttpContext.Current.Request.Form(strName), defValue)
        End Function

        ''' <summary> 
        ''' 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值 
        ''' </summary> 
        ''' <param name="strName">Url或表单参数</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>Url或表单参数的int类型值</returns> 
        Public Shared Function GetInt(ByVal strName As String, ByVal defValue As Integer) As Integer
            If GetQueryInt(strName, defValue) = defValue Then
                Return GetFormInt(strName, defValue)
            Else
                Return GetQueryInt(strName, defValue)
            End If
        End Function

        ''' <summary> 
        ''' 获得指定Url参数的float类型值 
        ''' </summary> 
        ''' <param name="strName">Url参数</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>Url参数的int类型值</returns> 
        Public Shared Function GetQueryFloat(ByVal strName As String, ByVal defValue As Single) As Single
            Return Utils.StrToFloat(HttpContext.Current.Request.QueryString(strName), defValue)
        End Function


        ''' <summary> 
        ''' 获得指定表单参数的float类型值 
        ''' </summary> 
        ''' <param name="strName">表单参数</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>表单参数的float类型值</returns> 
        Public Shared Function GetFormFloat(ByVal strName As String, ByVal defValue As Single) As Single
            Return Utils.StrToFloat(HttpContext.Current.Request.Form(strName), defValue)
        End Function

        ''' <summary> 
        ''' 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值 
        ''' </summary> 
        ''' <param name="strName">Url或表单参数</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>Url或表单参数的int类型值</returns> 
        Public Shared Function GetFloat(ByVal strName As String, ByVal defValue As Single) As Single
            If GetQueryFloat(strName, defValue) = defValue Then
                Return GetFormFloat(strName, defValue)
            Else
                Return GetQueryFloat(strName, defValue)
            End If
        End Function

        ''' <summary> 
        ''' 获得当前页面客户端的IP 
        ''' </summary> 
        ''' <returns>当前页面客户端的IP</returns> 
        Public Shared Function GetIP() As String


            Dim result As String = [String].Empty

            result = HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(result) Then
                result = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
            End If

            If String.IsNullOrEmpty(result) Then
                result = HttpContext.Current.Request.UserHostAddress
            End If

            If String.IsNullOrEmpty(result) OrElse Not Utils.IsIP(result) Then
                Return "127.0.0.1"
            End If

            Return result

        End Function

        ''' <summary> 
        ''' 保存用户上传的文件 
        ''' </summary> 
        ''' <param name="path">保存路径</param> 
        Public Shared Sub SaveRequestFile(ByVal path As String)
            If HttpContext.Current.Request.Files.Count > 0 Then
                HttpContext.Current.Request.Files(0).SaveAs(path)
            End If
        End Sub
    End Class

End Namespace
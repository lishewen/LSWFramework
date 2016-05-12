Imports System.Net
Imports System.IO
Imports System.Globalization
Imports System.Text

Namespace API.UpYunLibrary
    Public Class UpYun
        Private bucketname As String
        Private username As String
        Private password As String
        Private upAuth As Boolean = False
        Private api_domain As String = "v0.api.upyun.com"
        Private DL As String = "/"
        Private tmp_infos As New Hashtable()
        Private file_secret As String
        Private content_md5 As String
        Private auto_mkdir As Boolean = False

        Public Function version() As String
            Return "1.0.1"
        End Function

        ''' <summary>
        ''' 初始化 UpYun 存储接口
        ''' </summary>
        ''' <param name="bucketname">空间名称</param>
        ''' <param name="username">操作员名称</param>
        ''' <param name="password">密码</param>
        ''' <remarks></remarks>
        Public Sub New(bucketname As String, username As String, password As String)
            Me.bucketname = bucketname
            Me.username = username
            Me.password = password
        End Sub

        ''' <summary>
        ''' 切换 API 接口的域名
        ''' </summary>
        ''' <param name="domain">{默认 v0.api.upyun.com 自动识别, v1.api.upyun.com 电信, v2.api.upyun.com 联通, v3.api.upyun.com 移动}</param>
        ''' <remarks></remarks>
        Public Sub setApiDomain(domain As String)
            Me.api_domain = domain
        End Sub

        ''' <summary>
        ''' 是否启用 又拍签名认证
        ''' </summary>
        ''' <param name="upAuth">{默认 false 不启用(直接使用basic auth)，true 启用又拍签名认证}</param>
        ''' <remarks></remarks>
        Public Sub setAuthType(upAuth As Boolean)
            Me.upAuth = upAuth
        End Sub

        Private Sub upyunAuth(headers As Hashtable, method As String, uri As String, request As HttpWebRequest)
            Dim dt As DateTime = DateTime.UtcNow
            Dim [date] As String = dt.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", CultureInfo.CreateSpecificCulture("en-US"))
            request.[Date] = dt
            'headers.Add("Date", date);
            Dim auth As String
            If request.ContentLength <> -1 Then
                auth = md5(method & "&"c & uri & "&"c & [date] & "&"c & Convert.ToString(request.ContentLength) & "&"c & md5(Me.password))
            Else
                auth = md5(method & "&"c & uri & "&"c & [date] & "&"c & 0 & "&"c & md5(Me.password))
            End If
            headers.Add("Authorization", "UpYun " & Me.username & ":"c & auth)
        End Sub

        Private Function md5(str As String) As String
            Return LSW.Web.Utils.MD5(str).ToLower
        End Function

        Private Function delete(path As String, headers As Hashtable) As Boolean
            Dim resp As HttpWebResponse
            Dim a As Byte() = Nothing
            resp = newWorker("DELETE", DL & Me.bucketname & path, a, headers)
            If CInt(resp.StatusCode) = 200 Then
                resp.Close()
                Return True
            Else
                resp.Close()
                Return False
            End If
        End Function

        Private Function newWorker(method As String, Url As String, postData As Byte(), headers As Hashtable) As HttpWebResponse
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create("http://" & api_domain & Url), HttpWebRequest)


            request.Method = method

            If Me.auto_mkdir = True Then
                headers.Add("mkdir", "true")
                Me.auto_mkdir = False
            End If

            If postData IsNot Nothing Then
                request.ContentLength = postData.Length
                request.KeepAlive = True
                If Me.content_md5 IsNot Nothing Then
                    request.Headers.Add("Content-MD5", Me.content_md5)
                    Me.content_md5 = Nothing
                End If
                If Me.file_secret IsNot Nothing Then
                    request.Headers.Add("Content-Secret", Me.file_secret)
                    Me.file_secret = Nothing
                End If
            End If

            If Me.upAuth Then
                upyunAuth(headers, method, Url, request)
            Else
                request.Headers.Add("Authorization", "Basic " & Convert.ToBase64String(New System.Text.ASCIIEncoding().GetBytes(Me.username & ":" & Me.password)))
            End If
            For Each var As DictionaryEntry In headers
                request.Headers.Add(var.Key.ToString(), var.Value.ToString())
            Next

            If postData IsNot Nothing Then
                Dim dataStream As Stream = request.GetRequestStream()
                dataStream.Write(postData, 0, postData.Length)
                dataStream.Close()
            End If
            Dim response As HttpWebResponse
            Try
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                Me.tmp_infos = New Hashtable()
                For Each hl In response.Headers
                    Dim name As String = DirectCast(hl, String)
                    If name.Length > 7 AndAlso name.Substring(0, 7) = "x-upyun" Then
                        Me.tmp_infos.Add(name, response.Headers(name))
                    End If
                Next
            Catch e As Exception
                Throw e
            End Try

            Return response
        End Function

        ''' <summary>
        ''' 获取总体空间的占用信息
        ''' </summary>
        ''' <param name="url">目标路径</param>
        ''' <returns>空间占用量，失败返回 0</returns>
        ''' <remarks></remarks>
        Public Function getFolderUsage(url As String) As Integer
            Dim headers As New Hashtable()
            Dim size As Integer
            Dim a As Byte() = Nothing
            Dim resp As HttpWebResponse = newWorker("GET", DL & Me.bucketname & url & "?usage", a, headers)
            Try
                Dim sr As New StreamReader(resp.GetResponseStream(), Encoding.UTF8)
                Dim strhtml As String = sr.ReadToEnd()
                resp.Close()
                size = Integer.Parse(strhtml)
            Catch generatedExceptionName As Exception
                size = 0
            End Try
            Return size
        End Function

        ''' <summary>
        ''' 获取某个子目录的占用信息
        ''' </summary>
        ''' <returns>空间占用量，失败返回 0</returns>
        ''' <remarks></remarks>
        Public Function getBucketUsage() As Integer
            Return getFolderUsage("")
        End Function

        ''' <summary>
        ''' 创建目录
        ''' </summary>
        ''' <param name="path">目录路径</param>
        ''' <param name="auto_mkdir"></param>
        ''' <returns>true or false</returns>
        ''' <remarks></remarks>
        Public Function mkDir(path As String, auto_mkdir As Boolean) As Boolean
            Me.auto_mkdir = auto_mkdir
            Dim headers As New Hashtable()
            headers.Add("folder", "create")
            Dim resp As HttpWebResponse
            Dim a As Byte() = New Byte(-1) {}
            resp = newWorker("POST", DL & Me.bucketname & path, a, headers)
            If CInt(resp.StatusCode) = 200 Then
                resp.Close()
                Return True
            Else
                resp.Close()
                Return False
            End If
        End Function

        ''' <summary>
        ''' 删除目录
        ''' </summary>
        ''' <param name="path">目录路径</param>
        ''' <returns>true or false</returns>
        ''' <remarks></remarks>
        Public Function rmDir(path As String) As Boolean
            Dim headers As New Hashtable()
            Return delete(path, headers)
        End Function

        ''' <summary>
        ''' 读取目录列表
        ''' </summary>
        ''' <param name="url">目录路径</param>
        ''' <returns>array 数组 或 null</returns>
        ''' <remarks></remarks>
        Public Function readDir(url As String) As List(Of FolderItem)
            Dim headers As New Hashtable()
            Dim a As Byte() = Nothing
            Dim resp As HttpWebResponse = newWorker("GET", DL & Me.bucketname & url, a, headers)
            Dim sr As New StreamReader(resp.GetResponseStream(), Encoding.UTF8)
            Dim strhtml As String = sr.ReadToEnd()
            resp.Close()
            strhtml = strhtml.Replace(vbTab, "\")
            strhtml = strhtml.Replace(vbLf, "\")
            Dim ss As String() = strhtml.Split("\"c)
            Dim i As Integer = 0
            Dim AL As New List(Of FolderItem)
            If ss.Length < 4 Then Return AL
            While i < ss.Length
                Dim fi As New FolderItem(ss(i), ss(i + 1), Integer.Parse(ss(i + 2)), Integer.Parse(ss(i + 3)))
                AL.Add(fi)
                i += 4
            End While
            Return AL
        End Function

        ''' <summary>
        ''' 上传文件
        ''' </summary>
        ''' <param name="path">文件路径（包含文件名）</param>
        ''' <param name="data">文件内容 或 文件IO数据流</param>
        ''' <param name="auto_mkdir"></param>
        ''' <returns>true or false</returns>
        ''' <remarks></remarks>
        Public Function writeFile(path As String, data As Byte(), auto_mkdir As Boolean) As Boolean
            Dim headers As New Hashtable()
            Me.auto_mkdir = auto_mkdir
            Dim resp As HttpWebResponse
            resp = newWorker("POST", DL & Me.bucketname & path, data, headers)
            If CInt(resp.StatusCode) = 200 Then
                resp.Close()
                Return True
            Else
                resp.Close()
                Return False
            End If
        End Function

        ''' <summary>
        ''' 删除文件
        ''' </summary>
        ''' <param name="path">文件路径（包含文件名）</param>
        ''' <returns>true or false</returns>
        ''' <remarks></remarks>
        Public Function deleteFile(path As String) As Boolean
            Dim headers As New Hashtable()
            Return delete(path, headers)
        End Function

        ''' <summary>
        ''' 读取文件
        ''' </summary>
        ''' <param name="path">文件路径（包含文件名）</param>
        ''' <returns>文件内容 或 null</returns>
        ''' <remarks></remarks>
        Public Function readFile(path As String) As Byte()
            Dim headers As New Hashtable()
            Dim a As Byte() = Nothing

            Dim resp As HttpWebResponse = newWorker("GET", DL & Me.bucketname & path, a, headers)
            Dim sr As New StreamReader(resp.GetResponseStream(), Encoding.UTF8)
            Dim br As New BinaryReader(sr.BaseStream)
            Dim by As Byte() = br.ReadBytes(1024 * 1024 * 100)
            '又拍云存储最大文件限制 100Mb，对于普通用户可以改写该值，以减少内存消耗
            resp.Close()
            Return by
        End Function

        ''' <summary>
        ''' 设置待上传文件的 Content-MD5 值（如又拍云服务端收到的文件MD5值与用户设置的不一致，将回报 406 Not Acceptable 错误）
        ''' </summary>
        ''' <param name="str">（文件 MD5 校验码）</param>
        ''' <remarks></remarks>
        Public Sub setContentMD5(str As String)
            Me.content_md5 = str
        End Sub

        ''' <summary>
        ''' 设置待上传文件的 访问密钥（注意：仅支持图片空！，设置密钥后，无法根据原文件URL直接访问，需带 URL 后面加上 （缩略图间隔标志符+密钥） 进行访问）
        '''	如缩略图间隔标志符为 ! ，密钥为 bac，上传文件路径为 /folder/test.jpg ，那么该图片的对外访问地址为： http://空间域名/folder/test.jpg!bac
        ''' </summary>
        ''' <param name="str">（文件 MD5 校验码）</param>
        ''' <remarks></remarks>
        Public Sub setFileSecret(str As String)
            Me.file_secret = str
        End Sub

        ''' <summary>
        ''' 获取文件信息
        ''' </summary>
        ''' <param name="file">文件路径（包含文件名）</param>
        ''' <returns>array('type'=> file | folder, 'size'=> file size, 'date'=> unix time) 或 null</returns>
        ''' <remarks></remarks>
        Public Function getFileInfo(file As String) As Hashtable
            Dim headers As New Hashtable()
            Dim a As Byte() = Nothing
            Dim resp As HttpWebResponse = newWorker("HEAD", DL & Me.bucketname & file, a, headers)
            resp.Close()
            Dim ht As Hashtable
            Try
                ht = New Hashtable()
                ht.Add("type", Me.tmp_infos("x-upyun-file-type"))
                ht.Add("size", Me.tmp_infos("x-upyun-file-size"))
                ht.Add("date", Me.tmp_infos("x-upyun-file-date"))
            Catch generatedExceptionName As Exception
                ht = New Hashtable()
            End Try
            Return ht
        End Function

        ''' <summary>
        ''' 获取上传后的图片信息（仅图片空间有返回数据）
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getWritedFileInfo(key As String) As Object
            If Me.tmp_infos Is New Hashtable() Then
                Return ""
            End If
            Return Me.tmp_infos(key)
        End Function

        ''' <summary>
        ''' 计算文件的MD5码
        ''' </summary>
        ''' <param name="pathName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function md5_file(pathName As String) As String
            Dim strResult As String = ""
            Dim strHashData As String = ""

            Dim arrbytHashValue As Byte()
            Dim oFileStream As System.IO.FileStream = Nothing

            Dim oMD5Hasher As New System.Security.Cryptography.MD5CryptoServiceProvider()

            Try
                oFileStream = New System.IO.FileStream(pathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite)
                arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream)
                '计算指定Stream 对象的哈希值
                oFileStream.Close()
                '由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                strHashData = System.BitConverter.ToString(arrbytHashValue)
                '替换-
                strHashData = strHashData.Replace("-", "")
                strResult = strHashData
            Catch ex As System.Exception
                Throw ex
            End Try

            Return strResult.ToLower()
        End Function
    End Class
End Namespace
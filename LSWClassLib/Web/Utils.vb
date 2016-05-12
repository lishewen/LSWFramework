Imports System.Text.RegularExpressions
Imports System.Text
Imports System.IO
Imports System.Web
Imports System.Security.Cryptography
Imports System.Reflection
Imports System.Web.UI
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Net

Namespace Web
    ''' <summary> 
    ''' 工具类 
    ''' </summary> 
    Public Class Utils
        Public Const ASSEMBLY_VERSION As String = "2.5.0"

        Private Shared RegexBr As New Regex("(\r\n)", RegexOptions.IgnoreCase)
        Public Shared RegexFont As New Regex("<font color=" & """.*?""" & ">([\s\S]+?)</font>", Utils.GetRegexCompiledOptions())

        Private Shared AssemblyFileVersion As FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)

        Private Shared TemplateCookieName As String = String.Format("dnttemplateid_{0}_{1}_{2}", AssemblyFileVersion.FileMajorPart, AssemblyFileVersion.FileMinorPart, AssemblyFileVersion.FileBuildPart)


        ''' <summary> 
        ''' 得到正则编译参数设置 
        ''' </summary> 
        ''' <returns>参数设置</returns> 
        Public Shared Function GetRegexCompiledOptions() As RegexOptions
#If NET1 Then
            Return RegexOptions.Compiled 
#Else
            Return RegexOptions.None
#End If
        End Function

        ''' <summary> 
        ''' 返回字符串真实长度, 1个汉字长度为2 
        ''' </summary> 
        ''' <returns>字符长度</returns> 
        Public Shared Function GetStringLength(ByVal str As String) As Integer
            Return Encoding.[Default].GetBytes(str).Length
        End Function

        Public Shared Function IsCompriseStr(ByVal str As String, ByVal stringarray__1 As String, ByVal strsplit As String) As Boolean
            If stringarray__1 = "" OrElse stringarray__1 Is Nothing Then
                Return False
            End If

            str = str.ToLower()
            Dim stringArray__2 As String() = Utils.SplitString(stringarray__1.ToLower(), strsplit)
            For i As Integer = 0 To stringArray__2.Length - 1
                'string t1 = str; 
                'string t2 = stringArray[i]; 
                If str.IndexOf(stringArray__2(i)) > -1 Then
                    Return True
                End If
            Next
            Return False
        End Function

        ''' <summary> 
        ''' 判断指定字符串在指定字符串数组中的位置 
        ''' </summary> 
        ''' <param name="strSearch">字符串</param> 
        ''' <param name="stringArray">字符串数组</param> 
        ''' <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param> 
        ''' <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns> 
        Public Shared Function GetInArrayID(ByVal strSearch As String, ByVal stringArray As String(), ByVal caseInsensetive As Boolean) As Integer
            For i As Integer = 0 To stringArray.Length - 1
                If caseInsensetive Then
                    If strSearch.ToLower() = stringArray(i).ToLower() Then
                        Return i
                    End If
                Else
                    If strSearch = stringArray(i) Then
                        Return i
                    End If
                End If

            Next
            Return -1
        End Function


        ''' <summary> 
        ''' 判断指定字符串在指定字符串数组中的位置 
        ''' </summary> 
        ''' <param name="strSearch">字符串</param> 
        ''' <param name="stringArray">字符串数组</param> 
        ''' <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns> 
        Public Shared Function GetInArrayID(ByVal strSearch As String, ByVal stringArray As String()) As Integer
            Return GetInArrayID(strSearch, stringArray, True)
        End Function

        ''' <summary> 
        ''' 判断指定字符串是否属于指定字符串数组中的一个元素 
        ''' </summary> 
        ''' <param name="strSearch">字符串</param> 
        ''' <param name="stringArray">字符串数组</param> 
        ''' <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function InArray(ByVal strSearch As String, ByVal stringArray As String(), ByVal caseInsensetive As Boolean) As Boolean
            Return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0
        End Function

        ''' <summary> 
        ''' 判断指定字符串是否属于指定字符串数组中的一个元素 
        ''' </summary> 
        ''' <param name="str">字符串</param> 
        ''' <param name="stringarray">字符串数组</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function InArray(ByVal str As String, ByVal stringarray As String()) As Boolean
            Return InArray(str, stringarray, False)
        End Function

        ''' <summary> 
        ''' 判断指定字符串是否属于指定字符串数组中的一个元素 
        ''' </summary> 
        ''' <param name="str">字符串</param> 
        ''' <param name="stringarray">内部以逗号分割单词的字符串</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function InArray(ByVal str As String, ByVal stringarray As String) As Boolean
            Return InArray(str, SplitString(stringarray, ","), False)
        End Function

        ''' <summary> 
        ''' 判断指定字符串是否属于指定字符串数组中的一个元素 
        ''' </summary> 
        ''' <param name="str">字符串</param> 
        ''' <param name="stringarray">内部以逗号分割单词的字符串</param> 
        ''' <param name="strsplit">分割字符串</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function InArray(ByVal str As String, ByVal stringarray As String, ByVal strsplit As String) As Boolean
            Return InArray(str, SplitString(stringarray, strsplit), False)
        End Function

        ''' <summary> 
        ''' 判断指定字符串是否属于指定字符串数组中的一个元素 
        ''' </summary> 
        ''' <param name="str">字符串</param> 
        ''' <param name="stringarray">内部以逗号分割单词的字符串</param> 
        ''' <param name="strsplit">分割字符串</param> 
        ''' <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function InArray(ByVal str As String, ByVal stringarray As String, ByVal strsplit As String, ByVal caseInsensetive As Boolean) As Boolean
            Return InArray(str, SplitString(stringarray, strsplit), caseInsensetive)
        End Function


        ''' <summary> 
        ''' 删除字符串尾部的回车/换行/空格 
        ''' </summary> 
        ''' <param name="str"></param> 
        ''' <returns></returns> 
        Public Shared Function RTrim(ByVal str As String) As String
            For i As Integer = str.Length To 0 Step -1
                If str(i).Equals(" ") OrElse str(i).Equals(vbCr) OrElse str(i).Equals(vbLf) Then
                    str.Remove(i, 1)
                End If
            Next
            Return str
        End Function


        ''' <summary> 
        ''' 清除给定字符串中的回车及换行符 
        ''' </summary> 
        ''' <param name="str">要清除的字符串</param> 
        ''' <returns>清除后返回的字符串</returns> 
        Public Shared Function ClearBR(ByVal str As String) As String
            'Regex r = null; 
            Dim m As Match = Nothing

            'r = new Regex(@"(\r\n)",RegexOptions.IgnoreCase); 
            m = RegexBr.Match(str)
            While m.Success
                str = str.Replace(m.Groups(0).ToString(), "")
                m = m.NextMatch()
            End While


            Return str
        End Function

        ''' <summary> 
        ''' 从字符串的指定位置截取指定长度的子字符串 
        ''' </summary> 
        ''' <param name="str">原字符串</param> 
        ''' <param name="startIndex">子字符串的起始位置</param> 
        ''' <param name="length">子字符串的长度</param> 
        ''' <returns>子字符串</returns> 
        Public Shared Function CutString(ByVal str As String, ByVal startIndex As Integer, ByVal length As Integer) As String
            If startIndex >= 0 Then
                If length < 0 Then
                    length = length * -1
                    If startIndex - length < 0 Then
                        length = startIndex
                        startIndex = 0
                    Else
                        startIndex = startIndex - length
                    End If
                End If


                If startIndex > str.Length Then
                    Return ""
                End If


            Else
                If length < 0 Then
                    Return ""
                Else
                    If length + startIndex > 0 Then
                        length = length + startIndex
                        startIndex = 0
                    Else
                        Return ""
                    End If
                End If
            End If

            If str.Length - startIndex < length Then
                length = str.Length - startIndex
            End If

            Return str.Substring(startIndex, length)
        End Function

        ''' <summary> 
        ''' 从字符串的指定位置开始截取到字符串结尾的了符串 
        ''' </summary> 
        ''' <param name="str">原字符串</param> 
        ''' <param name="startIndex">子字符串的起始位置</param> 
        ''' <returns>子字符串</returns> 
        Public Shared Function CutString(ByVal str As String, ByVal startIndex As Integer) As String
            Return CutString(str, startIndex, str.Length)
        End Function



        ''' <summary> 
        ''' 获得当前绝对路径 
        ''' </summary> 
        ''' <param name="strPath">指定的路径</param> 
        ''' <returns>绝对路径</returns> 
        Public Shared Function GetMapPath(ByVal strPath As String) As String
            If HttpContext.Current IsNot Nothing Then
                Return HttpContext.Current.Server.MapPath(strPath)
            Else
                '非web程序引用 
                strPath = strPath.Replace("/", "\")
                If strPath.StartsWith("\") Then
                    strPath = strPath.Substring(strPath.IndexOf("\"c, 1)).TrimStart("\"c)
                End If
                Return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath)
            End If
        End Function



        ''' <summary> 
        ''' 返回文件是否存在 
        ''' </summary> 
        ''' <param name="filename">文件名</param> 
        ''' <returns>是否存在</returns> 
        Public Shared Function FileExists(ByVal filename As String) As Boolean
            Return System.IO.File.Exists(filename)
        End Function



        ''' <summary> 
        ''' 以指定的ContentType输出指定文件文件 
        ''' </summary> 
        ''' <param name="filepath">文件路径</param> 
        ''' <param name="filename">输出的文件名</param> 
        ''' <param name="filetype">将文件输出时设置的ContentType</param> 
        Public Shared Sub ResponseFile(ByVal filepath As String, ByVal filename As String, ByVal filetype As String)
            Dim iStream As Stream = Nothing

            ' 缓冲区为10k 
            Dim buffer As Byte() = New Byte(9999) {}

            ' 文件长度 
            Dim length As Integer

            ' 需要读的数据长度 
            Dim dataToRead As Long

            Try
                ' 打开文件 
                iStream = New FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)


                ' 需要读的数据长度 
                dataToRead = iStream.Length

                HttpContext.Current.Response.ContentType = filetype
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" & Utils.UrlEncode(filename.Trim()).Replace("+", " "))

                While dataToRead > 0
                    ' 检查客户端是否还处于连接状态 
                    If HttpContext.Current.Response.IsClientConnected Then
                        length = iStream.Read(buffer, 0, 10000)
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length)
                        HttpContext.Current.Response.Flush()
                        buffer = New Byte(9999) {}
                        dataToRead = dataToRead - length
                    Else
                        ' 如果不再连接则跳出死循环 
                        dataToRead = -1
                    End If
                End While
            Catch ex As Exception
                HttpContext.Current.Response.Write("Error : " & ex.Message)
            Finally
                If iStream IsNot Nothing Then
                    ' 关闭文件 
                    iStream.Close()
                End If
            End Try
            HttpContext.Current.Response.[End]()
        End Sub

        ''' <summary> 
        ''' 判断文件名是否为浏览器可以直接显示的图片文件名 
        ''' </summary> 
        ''' <param name="filename">文件名</param> 
        ''' <returns>是否可以直接显示</returns> 
        Public Shared Function IsImgFilename(ByVal filename As String) As Boolean
            filename = filename.Trim()
            If filename.EndsWith(".") OrElse filename.IndexOf(".") = -1 Then
                Return False
            End If
            Dim extname As String = filename.Substring(filename.LastIndexOf(".") + 1).ToLower()
            Return (extname = "jpg" OrElse extname = "jpeg" OrElse extname = "png" OrElse extname = "bmp" OrElse extname = "gif")
        End Function


        ''' <summary> 
        ''' int型转换为string型 
        ''' </summary> 
        ''' <returns>转换后的string类型结果</returns> 
        Public Shared Function IntToStr(ByVal intValue As Integer) As String
            ' 
            Return Convert.ToString(intValue)
        End Function
        ''' <summary> 
        ''' MD5函数 
        ''' </summary> 
        ''' <param name="str">原始字符串</param> 
        ''' <returns>MD5结果</returns> 
        Public Shared Function MD5(ByVal str As String) As String
            Dim b As Byte() = Encoding.[Default].GetBytes(str)
            b = New MD5CryptoServiceProvider().ComputeHash(b)
            Dim ret As String = ""
            For i As Integer = 0 To b.Length - 1
                ret += b(i).ToString("x").PadLeft(2, "0"c)
            Next
            Return ret
        End Function

        Public Shared Function SHA1(ByVal str As String) As String
            Dim b As Byte() = Encoding.UTF8.GetBytes(str)
            b = New SHA1CryptoServiceProvider().ComputeHash(b)
            Dim ret As String = ""
            For i As Integer = 0 To b.Length - 1
                ret += b(i).ToString("x").PadLeft(2, "0"c)
            Next
            Return ret
        End Function

        ''' <summary> 
        ''' salt MD5函数 
        ''' </summary> 
        ''' <param name="str">原始字符串</param> 
        ''' <param name="salt">安全码</param> 
        ''' <returns>MD5结果</returns> 
        Public Shared Function MD5(str As String, salt As String) As String
            Return MD5(MD5(str) & salt)
        End Function

        ''' <summary> 
        ''' SHA256函数 
        ''' </summary> 
        ''' /// <param name="str">原始字符串</param> 
        ''' <returns>SHA256结果</returns> 
        Public Shared Function SHA256(ByVal str As String) As String
            Dim SHA256Data As Byte() = Encoding.UTF8.GetBytes(str)
            Dim Sha256m As New SHA256Managed()
            Dim Result As Byte() = Sha256m.ComputeHash(SHA256Data)
            Return Convert.ToBase64String(Result)
            '返回长度为44字节的字符串 
        End Function


        ''' <summary> 
        ''' 字符串如果操过指定长度则将超出的部分用指定字符串代替 
        ''' </summary> 
        ''' <param name="p_SrcString">要检查的字符串</param> 
        ''' <param name="p_Length">指定长度</param> 
        ''' <param name="p_TailString">用于替换的字符串</param> 
        ''' <returns>截取后的字符串</returns> 
        Public Shared Function GetSubString(ByVal p_SrcString As String, ByVal p_Length As Integer, ByVal p_TailString As String) As String
            Return GetSubString(p_SrcString, 0, p_Length, p_TailString)
            'return GetSubStrings(p_SrcString, p_Length*2, p_TailString); 
        End Function

        Public Shared Function GetUnicodeSubString(ByVal str As String, ByVal len As Integer, ByVal p_TailString As String) As String
            Dim result As String = String.Empty
            ' 最终返回的结果 
            Dim byteLen As Integer = System.Text.Encoding.[Default].GetByteCount(str)
            ' 单字节字符长度 
            Dim charLen As Integer = str.Length
            ' 把字符平等对待时的字符串长度 
            Dim byteCount As Integer = 0
            ' 记录读取进度 
            Dim pos As Integer = 0
            ' 记录截取位置 
            If byteLen > len Then
                For i As Integer = 0 To charLen - 1
                    If Convert.ToInt32(str.ToCharArray()(i)) > 255 Then
                        ' 按中文字符计算加2 
                        byteCount += 2
                    Else
                        ' 按英文字符计算加1 
                        byteCount += 1
                    End If
                    If byteCount > len Then
                        ' 超出时只记下上一个有效位置 
                        pos = i
                        Exit For
                    ElseIf byteCount = len Then
                        ' 记下当前位置 
                        pos = i + 1
                        Exit For
                    End If
                Next

                If pos >= 0 Then
                    result = str.Substring(0, pos) + p_TailString
                End If
            Else
                result = str
            End If

            Return result
        End Function

        ''' <summary> 
        ''' 取指定长度的字符串 
        ''' </summary> 
        ''' <param name="p_SrcString">要检查的字符串</param> 
        ''' <param name="p_StartIndex">起始位置</param> 
        ''' <param name="p_Length">指定长度</param> 
        ''' <param name="p_TailString">用于替换的字符串</param> 
        ''' <returns>截取后的字符串</returns> 
        Public Shared Function GetSubString(ByVal p_SrcString As String, ByVal p_StartIndex As Integer, ByVal p_Length As Integer, ByVal p_TailString As String) As String
            Dim myResult As String = p_SrcString

            Dim bComments As Byte() = Encoding.UTF8.GetBytes(p_SrcString)
            For Each c As Char In Encoding.UTF8.GetChars(bComments)
                '当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3) 
                If (c > "ࠀ"c AndAlso c < "一"c) OrElse (c > "가"c AndAlso c < "힣"c) Then
                    'if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") || System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+")) 
                    '当截取的起始位置超出字段串长度时 
                    If p_StartIndex >= p_SrcString.Length Then
                        Return ""
                    Else
                        Return p_SrcString.Substring(p_StartIndex, If(((p_Length + p_StartIndex) > p_SrcString.Length), (p_SrcString.Length - p_StartIndex), p_Length))
                    End If
                End If
            Next


            If p_Length >= 0 Then
                Dim bsSrcString As Byte() = Encoding.[Default].GetBytes(p_SrcString)

                '当字符串长度大于起始位置 
                If bsSrcString.Length > p_StartIndex Then
                    Dim p_EndIndex As Integer = bsSrcString.Length

                    '当要截取的长度在字符串的有效长度范围内 
                    If bsSrcString.Length > (p_StartIndex + p_Length) Then
                        p_EndIndex = p_Length + p_StartIndex
                    Else
                        '当不在有效范围内时,只取到字符串的结尾 
                        p_Length = bsSrcString.Length - p_StartIndex
                        p_TailString = ""
                    End If



                    Dim nRealLength As Integer = p_Length
                    Dim anResultFlag As Integer() = New Integer(p_Length - 1) {}
                    Dim bsResult As Byte() = Nothing

                    Dim nFlag As Integer = 0
                    For i As Integer = p_StartIndex To p_EndIndex - 1

                        If bsSrcString(i) > 127 Then
                            nFlag += 1
                            If nFlag = 3 Then
                                nFlag = 1
                            End If
                        Else
                            nFlag = 0
                        End If

                        anResultFlag(i) = nFlag
                    Next

                    If (bsSrcString(p_EndIndex - 1) > 127) AndAlso (anResultFlag(p_Length - 1) = 1) Then
                        nRealLength = p_Length + 1
                    End If

                    bsResult = New Byte(nRealLength - 1) {}

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength)

                    myResult = Encoding.[Default].GetString(bsResult)

                    myResult = myResult + p_TailString
                End If
            End If

            Return myResult
        End Function

        ''' <summary> 
        ''' 自定义的替换字符串函数 
        ''' </summary> 
        Public Shared Function ReplaceString(ByVal SourceString As String, ByVal SearchString As String, ByVal ReplaceStrings As String, ByVal IsCaseInsensetive As Boolean) As String
            Return Regex.Replace(SourceString, Regex.Escape(SearchString), ReplaceStrings, If(IsCaseInsensetive, RegexOptions.IgnoreCase, RegexOptions.None))
        End Function

        ''' <summary> 
        ''' 生成指定数量的html空格符号 
        ''' </summary> 
        Public Shared Function GetSpacesString(ByVal spacesCount As Integer) As String
            Dim sb As New StringBuilder()
            For i As Integer = 0 To spacesCount - 1
                sb.Append(" &nbsp;&nbsp;")
            Next
            Return sb.ToString()
        End Function

        ''' <summary> 
        ''' 检测是否符合email格式 
        ''' </summary> 
        ''' <param name="strEmail">要判断的email字符串</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function IsValidEmail(ByVal strEmail As String) As Boolean
            Return Regex.IsMatch(strEmail, "^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
        End Function

        Public Shared Function IsValidDoEmail(ByVal strEmail As String) As Boolean
            Return Regex.IsMatch(strEmail, "^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
        End Function

        ''' <summary> 
        ''' 检测是否是正确的Url 
        ''' </summary> 
        ''' <param name="strUrl">要验证的Url</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function IsURL(ByVal strUrl As String) As Boolean
            Return Regex.IsMatch(strUrl, "^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$")
        End Function

        Public Shared Function GetEmailHostName(ByVal strEmail As String) As String
            If strEmail.IndexOf("@") < 0 Then
                Return ""
            End If
            Return strEmail.Substring(strEmail.LastIndexOf("@")).ToLower()
        End Function

        ''' <summary> 
        ''' 判断是否为base64字符串 
        ''' </summary> 
        ''' <param name="str"></param> 
        ''' <returns></returns> 
        Public Shared Function IsBase64String(ByVal str As String) As Boolean
            'A-Z, a-z, 0-9, +, /, = 
            Return Regex.IsMatch(str, "[A-Za-z0-9\+\/\=]")
        End Function
        ''' <summary> 
        ''' 检测是否有Sql危险字符 
        ''' </summary> 
        ''' <param name="str">要判断字符串</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function IsSafeSqlString(ByVal str As String) As Boolean

            Return Not Regex.IsMatch(str, "[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']")
        End Function

        ''' <summary> 
        ''' 检测是否有危险的可能用于链接的字符串 
        ''' </summary> 
        ''' <param name="str">要判断字符串</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function IsSafeUserInfoString(ByVal str As String) As Boolean
            Return Not Regex.IsMatch(str, "^\s*$|^c:\\con\\con$|[%,\*" & """" & "\s\t\<\>\&]|游客|^Guest")
        End Function

        ''' <summary> 
        ''' 清理字符串 
        ''' </summary> 
        Public Shared Function CleanInput(ByVal strIn As String) As String
            Return Regex.Replace(strIn.Trim(), "[^\w\.@-]", "")
        End Function

        ''' <summary> 
        ''' 返回URL中结尾的文件名 
        ''' </summary> 
        Public Shared Function GetFilename(ByVal url As String) As String
            If url Is Nothing Then
                Return ""
            End If
            Dim strs1 As String() = url.Split(New Char() {"/"c})
            Return strs1(strs1.Length - 1).Split(New Char() {"?"c})(0)
        End Function

        ''' <summary> 
        ''' 根据阿拉伯数字返回月份的名称(可更改为某种语言) 
        ''' </summary> 
        Public Shared ReadOnly Property Monthes() As String()
            Get
                Return New String() {"January", "February", "March", "April", "May", "June", _
                "July", "August", "September", "October", "November", "December"}
            End Get
        End Property

        ''' <summary> 
        ''' 替换回车换行符为html换行符 
        ''' </summary> 
        Public Shared Function StrFormat(ByVal str As String) As String
            Dim str2 As String

            If str Is Nothing Then
                str2 = ""
            Else
                str = str.Replace(vbCr & vbLf, "<br />")
                str = str.Replace(vbLf, "<br />")
                str2 = str
            End If
            Return str2
        End Function

        ''' <summary> 
        ''' 返回标准日期格式string 
        ''' </summary> 
        Public Shared Function GetDate() As String
            Return DateTime.Now.ToString("yyyy-MM-dd")
        End Function

        ''' <summary> 
        ''' 返回指定日期格式 
        ''' </summary> 
        Public Shared Function GetDate(ByVal datetimestr As String, ByVal replacestr As String) As String
            If datetimestr Is Nothing Then
                Return replacestr
            End If

            If datetimestr.Equals("") Then
                Return replacestr
            End If

            Try
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr)
            Catch
                Return replacestr
            End Try
            Return datetimestr
        End Function

        ''' <summary> 
        ''' 返回标准时间格式string 
        ''' </summary> 
        Public Shared Function GetTime() As String
            Return DateTime.Now.ToString("HH:mm:ss")
        End Function

        ''' <summary> 
        ''' 返回标准时间格式string 
        ''' </summary> 
        Public Shared Function GetDateTime() As String
            Return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        End Function

        ''' <summary> 
        ''' 返回相对于当前时间的相对天数 
        ''' </summary> 
        Public Shared Function GetDateTime(ByVal relativeday As Integer) As String
            Return DateTime.Now.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss")
        End Function

        ''' <summary> 
        ''' 返回标准时间格式string 
        ''' </summary> 
        Public Shared Function GetDateTimeF() As String
            Return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff")
        End Function

        ''' <summary> 
        ''' 返回标准时间 
        ''' </summary> 
        Public Shared Function GetStandardDateTime(ByVal fDateTime As String, ByVal formatStr As String) As String
            If fDateTime = "0000-0-0 0:00:00" Then
                Return fDateTime
            End If
            Dim s As DateTime = Convert.ToDateTime(fDateTime)
            Return s.ToString(formatStr)
        End Function

        ''' <summary> 
        ''' 返回标准时间 yyyy-MM-dd HH:mm:ss 
        ''' </summary> 
        Public Shared Function GetStandardDateTime(ByVal fDateTime As String) As String
            Return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss")
        End Function

        ''' <summary> 
        ''' 返回标准时间 yyyy-MM-dd 
        ''' </summary> 
        Public Shared Function GetStandardDate(ByVal fDate As String) As String
            Return GetStandardDateTime(fDate, "yyyy-MM-dd")
        End Function

        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function IsTime(ByVal timeval As String) As Boolean
            Return Regex.IsMatch(timeval, "^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$")
        End Function


        Public Shared Function GetRealIP() As String
            Dim ip As String = LSWRequest.GetIP()

            Return ip
        End Function

        ''' <summary> 
        ''' 改正sql语句中的转义字符 
        ''' </summary> 
        Public Shared Function MashSQL(ByVal str As String) As String
            Dim str2 As String

            If str Is Nothing Then
                str2 = ""
            Else
                str = str.Replace("'", "'")
                str2 = str
            End If
            Return str2
        End Function

        ''' <summary> 
        ''' 替换sql语句中的有问题符号 
        ''' </summary> 
        Public Shared Function ChkSQL(ByVal str As String) As String
            Dim str2 As String

            If str Is Nothing Then
                str2 = ""
            Else
                str = str.Replace("'", "''")
                str2 = str
            End If
            Return str2
        End Function

        Public Shared Function SqlEncodeLike(ByVal inputString As String) As String
            If ((inputString Is Nothing) OrElse (inputString.Trim.Length <= 0)) Then
                Return String.Empty
            End If
            Dim str As String = "and|or|exec|execute|insert|select|delete|update|alter|create|drop|" & ChrW(13) & ChrW(10) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & "count|\*|chr|char|asc|mid|substring|master|truncate|declare|xp_cmdshell|" & ChrW(13) & ChrW(10) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & ChrW(9) & "restore|backup|net +user|net +localgroup +administrators"
            Dim matchs As MatchCollection = New Regex(("\b(" & str & ")\b"), RegexOptions.IgnoreCase).Matches(inputString)
            Dim i As Integer
            For i = 0 To matchs.Count - 1
                inputString = inputString.Replace(matchs.Item(i).Value, ("[" & matchs.Item(i).Value & "]"))
            Next i
            Return inputString.Replace("'", "''").Replace("-", "[-]").Trim
        End Function

        ''' <summary> 
        ''' 转换为静态html 
        ''' </summary> 
        Public Sub transHtml(ByVal path As String, ByVal outpath As String)
            Dim page As New Page()
            Dim writer As New StringWriter()
            page.Server.Execute(path, writer)
            Dim fs As FileStream
            If File.Exists((page.Server.MapPath("") & "\") + outpath) Then
                File.Delete((page.Server.MapPath("") & "\") + outpath)
                fs = File.Create((page.Server.MapPath("") & "\") + outpath)
            Else
                fs = File.Create((page.Server.MapPath("") & "\") + outpath)
            End If
            Dim bt As Byte() = Encoding.[Default].GetBytes(writer.ToString())
            fs.Write(bt, 0, bt.Length)
            fs.Close()
        End Sub

        ''' <summary> 
        ''' 转换为简体中文 
        ''' </summary> 
        Public Shared Function ToSChinese(ByVal str As String) As String
            Return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0)
        End Function

        ''' <summary> 
        ''' 转换为繁体中文 
        ''' </summary> 
        Public Shared Function ToTChinese(ByVal str As String) As String
            Return Strings.StrConv(str, VbStrConv.TraditionalChinese, 0)
        End Function

        ''' <summary> 
        ''' 分割字符串 
        ''' </summary> 
        Public Shared Function SplitString(ByVal strContent As String, ByVal strSplit As String) As String()
            If Not Utils.StrIsNullOrEmpty(strContent) Then
                If strContent.IndexOf(strSplit) < 0 Then
                    Dim tmp As String() = {strContent}
                    Return tmp
                End If
                Return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase)
            Else
                Return New String(-1) {}
            End If
        End Function

        ''' <summary> 
        ''' 分割字符串 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function SplitString(ByVal strContent As String, ByVal strSplit As String, ByVal count As Integer) As String()
            Dim result As String() = New String(count - 1) {}

            Dim splited As String() = SplitString(strContent, strSplit)

            For i As Integer = 0 To count - 1
                If i < splited.Length Then
                    result(i) = splited(i)
                Else
                    result(i) = String.Empty
                End If
            Next

            Return result
        End Function

        ''' <summary> 
        ''' 过滤字符串数组中每个元素为合适的大小 
        ''' 当长度小于minLength时，忽略掉,-1为不限制最小长度 
        ''' 当长度大于maxLength时，取其前maxLength位 
        ''' 如果数组中有null元素，会被忽略掉 
        ''' </summary> 
        ''' <param name="minLength">单个元素最小长度</param> 
        ''' <param name="maxLength">单个元素最大长度</param> 
        ''' <returns></returns> 
        Public Shared Function PadStringArray(ByVal strArray As String(), ByVal minLength As Integer, ByVal maxLength As Integer) As String()
            If minLength > maxLength Then
                Dim t As Integer = maxLength
                maxLength = minLength
                minLength = t
            End If

            Dim iMiniStringCount As Integer = 0
            For i_1 = 0 To strArray.Length - 1
                If minLength > -1 AndAlso strArray(i_1).Length < minLength Then
                    strArray(i_1) = Nothing
                    Continue For
                End If
                If strArray(i_1).Length > maxLength Then
                    strArray(i_1) = strArray(i_1).Substring(0, maxLength)
                End If
                iMiniStringCount += 1
            Next

            Dim result As String() = New String(iMiniStringCount - 1) {}
            Dim i As Integer = 0, j As Integer = 0
            While i < strArray.Length AndAlso j < result.Length
                If strArray(i) IsNot Nothing AndAlso strArray(i) <> String.Empty Then
                    result(j) = strArray(i)
                    j += 1
                End If
                i += 1
            End While


            Return result
        End Function

        ''' <summary> 
        ''' 分割字符串 
        ''' </summary> 
        ''' <param name="strContent">被分割的字符串</param> 
        ''' <param name="strSplit">分割符</param> 
        ''' <param name="ignoreRepeatItem">忽略重复项</param> 
        ''' <param name="maxElementLength">单个元素最大长度</param> 
        ''' <returns></returns> 
        Public Shared Function SplitString(ByVal strContent As String, ByVal strSplit As String, ByVal ignoreRepeatItem As Boolean, ByVal maxElementLength As Integer) As String()
            Dim result As String() = SplitString(strContent, strSplit)

            Return If(ignoreRepeatItem, DistinctStringArray(result, maxElementLength), result)
        End Function

        Public Shared Function SplitString(ByVal strContent As String, ByVal strSplit As String, ByVal ignoreRepeatItem As Boolean, ByVal minElementLength As Integer, ByVal maxElementLength As Integer) As String()
            Dim result As String() = SplitString(strContent, strSplit)

            If ignoreRepeatItem Then
                result = DistinctStringArray(result)
            End If
            Return PadStringArray(result, minElementLength, maxElementLength)
        End Function

        ''' <summary> 
        ''' 分割字符串 
        ''' </summary> 
        ''' <param name="strContent">被分割的字符串</param> 
        ''' <param name="strSplit">分割符</param> 
        ''' <param name="ignoreRepeatItem">忽略重复项</param> 
        ''' <returns></returns> 
        Public Shared Function SplitString(ByVal strContent As String, ByVal strSplit As String, ByVal ignoreRepeatItem As Boolean) As String()
            Return SplitString(strContent, strSplit, ignoreRepeatItem, 0)
        End Function

        ''' <summary> 
        ''' 清除字符串数组中的重复项 
        ''' </summary> 
        ''' <param name="strArray">字符串数组</param> 
        ''' <param name="maxElementLength">字符串数组中单个元素的最大长度</param> 
        ''' <returns></returns> 
        Public Shared Function DistinctStringArray(ByVal strArray As String(), ByVal maxElementLength As Integer) As String()
            Dim h As New Hashtable()

            For Each s As String In strArray
                Dim k As String = s
                If maxElementLength > 0 AndAlso k.Length > maxElementLength Then
                    k = k.Substring(0, maxElementLength)
                End If
                h(k.Trim()) = s
            Next

            Dim result As String() = New String(h.Count - 1) {}

            h.Keys.CopyTo(result, 0)

            Return result
        End Function

        ''' <summary> 
        ''' 清除字符串数组中的重复项 
        ''' </summary> 
        ''' <param name="strArray">字符串数组</param> 
        ''' <returns></returns> 
        Public Shared Function DistinctStringArray(ByVal strArray As String()) As String()
            Return DistinctStringArray(strArray, 0)
        End Function

        ''' <summary> 
        ''' 替换html字符 
        ''' </summary> 
        Public Shared Function EncodeHtml(ByVal strHtml As String) As String
            If strHtml <> "" Then
                strHtml = strHtml.Replace(",", "&def")
                strHtml = strHtml.Replace("'", "&dot")
                strHtml = strHtml.Replace(";", "&dec")
                Return strHtml
            End If
            Return ""
        End Function

        'public static string ClearHtml(string strHtml) 
        '{ 
        ' if (strHtml != "") 
        ' { 

        ' r = Regex.Replace(@"<\/?[^>]*>",RegexOptions.IgnoreCase); 
        ' for (m = r.Match(strHtml); m.Success; m = m.NextMatch()) 
        ' { 
        ' strHtml = strHtml.Replace(m.Groups[0].ToString(),""); 
        ' } 
        ' } 
        ' return strHtml; 
        '} 

        ''' <summary> 
        ''' 进行指定的替换(脏字过滤) 
        ''' </summary> 
        Public Shared Function StrFilter(ByVal str As String, ByVal bantext As String) As String
            Dim text1 As String = ""
            Dim text2 As String = ""
            Dim textArray1 As String() = SplitString(bantext, vbCr & vbLf)
            For num1 As Integer = 0 To textArray1.Length - 1
                text1 = textArray1(num1).Substring(0, textArray1(num1).IndexOf("="))
                text2 = textArray1(num1).Substring(textArray1(num1).IndexOf("=") + 1)
                str = str.Replace(text1, text2)
            Next
            Return str
        End Function

        ''' <summary> 
        ''' 获得伪静态页码显示链接 
        ''' </summary> 
        ''' <param name="curPage">当前页数</param> 
        ''' <param name="countPage">总页数</param> 
        ''' <param name="url">超级链接地址</param> 
        ''' <param name="extendPage">周边页码显示个数上限</param> 
        ''' <returns>页码html</returns> 
        Public Shared Function GetStaticPageNumbers(ByVal curPage As Integer, ByVal countPage As Integer, ByVal url As String, ByVal expname As String, ByVal extendPage As Integer) As String
            Dim startPage As Integer = 1
            Dim endPage As Integer = 1

            Dim t1 As String = ("<a href=""" & url & "-1") + expname & """>&laquo;</a>"
            Dim t2 As String = ("<a href=""" & url & "-") + countPage + expname & """>&raquo;</a>"

            If countPage < 1 Then
                countPage = 1
            End If
            If extendPage < 3 Then
                extendPage = 2
            End If

            If countPage > extendPage Then
                If curPage - (extendPage / 2) > 0 Then
                    If curPage + (extendPage / 2) < countPage Then
                        startPage = curPage - (extendPage / 2)
                        endPage = startPage + extendPage - 1
                    Else
                        endPage = countPage
                        startPage = endPage - extendPage + 1
                        t2 = ""
                    End If
                Else
                    endPage = extendPage
                    t1 = ""
                End If
            Else
                startPage = 1
                endPage = countPage
                t1 = ""
                t2 = ""
            End If

            Dim s As New StringBuilder("")

            s.Append(t1)
            For i As Integer = startPage To endPage
                If i = curPage Then
                    s.Append("<span>")
                    s.Append(i)
                    s.Append("</span>")
                Else
                    s.Append("<a href=""")
                    s.Append(url)
                    s.Append("-")
                    s.Append(i)
                    s.Append(expname)
                    s.Append(""">")
                    s.Append(i)
                    s.Append("</a>")
                End If
            Next
            s.Append(t2)

            Return s.ToString()
        End Function

        ''' <summary> 
        ''' 获得帖子的伪静态页码显示链接 
        ''' </summary> 
        ''' <param name="expname"></param> 
        ''' <param name="countPage">总页数</param> 
        ''' <param name="url">超级链接地址</param> 
        ''' <param name="extendPage">周边页码显示个数上限</param> 
        ''' <returns>页码html</returns> 
        Public Shared Function GetPostPageNumbers(ByVal countPage As Integer, ByVal url As String, ByVal expname As String, ByVal extendPage As Integer) As String
            Dim startPage As Integer = 1
            Dim endPage As Integer = 1
            Dim curPage As Integer = 1

            Dim t1 As String = ("<a href=""" & url & "-1") + expname & """>&laquo;</a>"
            Dim t2 As String = ("<a href=""" & url & "-") + countPage + expname & """>&raquo;</a>"

            If countPage < 1 Then
                countPage = 1
            End If
            If extendPage < 3 Then
                extendPage = 2
            End If

            If countPage > extendPage Then
                If curPage - (extendPage / 2) > 0 Then
                    If curPage + (extendPage / 2) < countPage Then
                        startPage = curPage - (extendPage / 2)
                        endPage = startPage + extendPage - 1
                    Else
                        endPage = countPage
                        startPage = endPage - extendPage + 1
                        t2 = ""
                    End If
                Else
                    endPage = extendPage
                    t1 = ""
                End If
            Else
                startPage = 1
                endPage = countPage
                t1 = ""
                t2 = ""
            End If

            Dim s As New StringBuilder("")

            s.Append(t1)
            For i As Integer = startPage To endPage
                s.Append("<a href=""")
                s.Append(url)
                s.Append("-")
                s.Append(i)
                s.Append(expname)
                s.Append(""">")
                s.Append(i)
                s.Append("</a>")
            Next
            s.Append(t2)

            Return s.ToString()
        End Function

        ''' <summary> 
        ''' 获得页码显示链接 
        ''' </summary> 
        ''' <param name="curPage">当前页数</param> 
        ''' <param name="countPage">总页数</param> 
        ''' <param name="url">超级链接地址</param> 
        ''' <param name="extendPage">周边页码显示个数上限</param> 
        ''' <returns>页码html</returns> 
        Public Shared Function GetPageNumbers(ByVal curPage As Integer, ByVal countPage As Integer, ByVal url As String, ByVal extendPage As Integer) As String
            Return GetPageNumbers(curPage, countPage, url, extendPage, "page")
        End Function

        ''' <summary> 
        ''' 获得页码显示链接 
        ''' </summary> 
        ''' <param name="curPage">当前页数</param> 
        ''' <param name="countPage">总页数</param> 
        ''' <param name="url">超级链接地址</param> 
        ''' <param name="extendPage">周边页码显示个数上限</param> 
        ''' <param name="pagetag">页码标记</param> 
        ''' <returns>页码html</returns> 
        Public Shared Function GetPageNumbers(ByVal curPage As Integer, ByVal countPage As Integer, ByVal url As String, ByVal extendPage As Integer, ByVal pagetag As String) As String
            Return GetPageNumbers(curPage, countPage, url, extendPage, pagetag, Nothing)
        End Function

        ''' <summary> 
        ''' 获得页码显示链接 
        ''' </summary> 
        ''' <param name="curPage">当前页数</param> 
        ''' <param name="countPage">总页数</param> 
        ''' <param name="url">超级链接地址</param> 
        ''' <param name="extendPage">周边页码显示个数上限</param> 
        ''' <param name="pagetag">页码标记</param> 
        ''' <param name="anchor">锚点</param> 
        ''' <returns>页码html</returns> 
        Public Shared Function GetPageNumbers(ByVal curPage As Integer, ByVal countPage As Integer, ByVal url As String, ByVal extendPage As Integer, ByVal pagetag As String, ByVal anchor As String) As String
            If pagetag = "" Then
                pagetag = "page"
            End If
            Dim startPage As Integer = 1
            Dim endPage As Integer = 1

            If url.IndexOf("?") > 0 Then
                url = url & "&"
            Else
                url = url & "?"
            End If

            Dim t1 As String = ("<a href=""" & url & "&") + pagetag & "=1"
            Dim t2 As String = (("<a href=""" & url & "&") + pagetag & "=") + countPage
            If anchor IsNot Nothing Then
                t1 += anchor
                t2 += anchor
            End If
            t1 += """>&laquo;</a>"
            t2 += """>&raquo;</a>"

            If countPage < 1 Then
                countPage = 1
            End If
            If extendPage < 3 Then
                extendPage = 2
            End If

            If countPage > extendPage Then
                If curPage - (extendPage / 2) > 0 Then
                    If curPage + (extendPage / 2) < countPage Then
                        startPage = curPage - (extendPage / 2)
                        endPage = startPage + extendPage - 1
                    Else
                        endPage = countPage
                        startPage = endPage - extendPage + 1
                        t2 = ""
                    End If
                Else
                    endPage = extendPage
                    t1 = ""
                End If
            Else
                startPage = 1
                endPage = countPage
                t1 = ""
                t2 = ""
            End If

            Dim s As New StringBuilder("")

            s.Append(t1)
            For i As Integer = startPage To endPage
                If i = curPage Then
                    s.Append("<span>")
                    s.Append(i)
                    s.Append("</span>")
                Else
                    s.Append("<a href=""")
                    s.Append(url)
                    s.Append(pagetag)
                    s.Append("=")
                    s.Append(i)
                    If anchor IsNot Nothing Then
                        s.Append(anchor)
                    End If
                    s.Append(""">")
                    s.Append(i)
                    s.Append("</a>")
                End If
            Next
            s.Append(t2)

            Return s.ToString()
        End Function

        ''' <summary> 
        ''' 返回 HTML 字符串的编码结果 
        ''' </summary> 
        ''' <param name="str">字符串</param> 
        ''' <returns>编码结果</returns> 
        Public Shared Function HtmlEncode(ByVal str As String) As String
            Return HttpUtility.HtmlEncode(str)
        End Function

        ''' <summary> 
        ''' 返回 HTML 字符串的解码结果 
        ''' </summary> 
        ''' <param name="str">字符串</param> 
        ''' <returns>解码结果</returns> 
        Public Shared Function HtmlDecode(ByVal str As String) As String
            Return HttpUtility.HtmlDecode(str)
        End Function

        ''' <summary> 
        ''' 返回 URL 字符串的编码结果 
        ''' </summary> 
        ''' <param name="str">字符串</param> 
        ''' <returns>编码结果</returns> 
        Public Shared Function UrlEncode(ByVal str As String) As String
            Return HttpUtility.UrlEncode(str)
        End Function

        ''' <summary> 
        ''' 返回 URL 字符串的编码结果 
        ''' </summary> 
        ''' <param name="str">字符串</param> 
        ''' <returns>解码结果</returns> 
        Public Shared Function UrlDecode(ByVal str As String) As String
            Return HttpUtility.UrlDecode(str)
        End Function

        ''' <summary> 
        ''' 返回指定目录下的非 UTF8 字符集文件 
        ''' </summary> 
        ''' <param name="Path">路径</param> 
        ''' <returns>文件名的字符串数组</returns> 
        Public Shared Function FindNoUTF8File(ByVal Path As String) As String()
            'System.IO.StreamReader reader = null; 
            Dim filelist As New StringBuilder()
            Dim Folder As New DirectoryInfo(Path)
            'System.IO.DirectoryInfo[] subFolders = Folder.GetDirectories(); 
            ' 
            ' for (int i=0;i<subFolders.Length;i++) 
            ' { 
            ' FindNoUTF8File(subFolders[i].FullName); 
            ' } 
            ' 

            Dim subFiles As FileInfo() = Folder.GetFiles()
            For j As Integer = 0 To subFiles.Length - 1
                If subFiles(j).Extension.ToLower().Equals(".htm") Then
                    Dim fs As New FileStream(subFiles(j).FullName, FileMode.Open, FileAccess.Read)
                    Dim bUtf8 As Boolean = IsUTF8(fs)
                    fs.Close()
                    If Not bUtf8 Then
                        filelist.Append(subFiles(j).FullName)
                        filelist.Append(vbCr & vbLf)
                    End If
                End If
            Next
            Return Utils.SplitString(filelist.ToString(), vbCr & vbLf)

        End Function

        '0000 0000-0000 007F - 0xxxxxxx (ascii converts to 1 octet!) 
        '0000 0080-0000 07FF - 110xxxxx 10xxxxxx ( 2 octet format) 
        '0000 0800-0000 FFFF - 1110xxxx 10xxxxxx 10xxxxxx (3 octet format) 

        ''' <summary> 
        ''' 判断文件流是否为UTF8字符集 
        ''' </summary> 
        ''' <param name="sbInputStream">文件流</param> 
        ''' <returns>判断结果</returns> 
        Private Shared Function IsUTF8(ByVal sbInputStream As FileStream) As Boolean
            Dim i As Integer
            Dim cOctets As Byte
            ' octets to go in this UTF-8 encoded character 
            Dim chr As Byte
            Dim bAllAscii As Boolean = True
            Dim iLen As Long = sbInputStream.Length

            cOctets = 0
            For i = 0 To iLen - 1
                chr = CByte(sbInputStream.ReadByte())

                If (chr And &H80) <> 0 Then
                    bAllAscii = False
                End If

                If cOctets = 0 Then
                    If chr >= &H80 Then
                        Do
                            chr <<= 1
                            cOctets += 1
                        Loop While (chr And &H80) <> 0

                        cOctets -= 1
                        If cOctets = 0 Then
                            Return False
                        End If
                    End If
                Else
                    If (chr And &HC0) <> &H80 Then
                        Return False
                    End If
                    cOctets -= 1
                End If
            Next

            If cOctets > 0 Then
                Return False
            End If

            If bAllAscii Then
                Return False
            End If

            Return True

        End Function

        ''' <summary> 
        ''' 格式化字节数字符串 
        ''' </summary> 
        ''' <param name="bytes"></param> 
        ''' <returns></returns> 
        Public Shared Function FormatBytesStr(ByVal bytes As Integer) As String
            If bytes > 1073741824 Then
                Return CDbl((bytes / 1073741824)).ToString("0") & "G"
            End If
            If bytes > 1048576 Then
                Return CDbl((bytes / 1048576)).ToString("0") & "M"
            End If
            If bytes > 1024 Then
                Return CDbl((bytes / 1024)).ToString("0") & "K"
            End If
            Return bytes.ToString() & "Bytes"
        End Function

        ''' <summary> 
        ''' 将long型数值转换为Int32类型 
        ''' </summary> 
        ''' <param name="objNum"></param> 
        ''' <returns></returns> 
        Public Shared Function SafeInt32(ByVal objNum As Object) As Integer
            If objNum Is Nothing Then
                Return 0
            End If
            Dim strNum As String = objNum.ToString()
            If IsNumeric(strNum) Then

                If strNum.ToString().Length > 9 Then
                    If strNum.StartsWith("-") Then
                        Return Integer.MinValue
                    Else
                        Return Integer.MaxValue
                    End If
                End If
                Return Int32.Parse(strNum)
            Else
                Return 0
            End If
        End Function

        ''' <summary> 
        ''' 返回相差的秒数 
        ''' </summary> 
        ''' <param name="Time"></param> 
        ''' <param name="Sec"></param> 
        ''' <returns></returns> 
        Public Shared Function StrDateDiffSeconds(ByVal Time As String, ByVal Sec As Integer) As Integer
            Dim ts As TimeSpan = DateTime.Now - DateTime.Parse(Time).AddSeconds(Sec)
            If ts.TotalSeconds > Integer.MaxValue Then
                Return Integer.MaxValue
            ElseIf ts.TotalSeconds < Integer.MinValue Then
                Return Integer.MinValue
            End If
            Return CInt(ts.TotalSeconds)
        End Function

        ''' <summary> 
        ''' 返回相差的分钟数 
        ''' </summary> 
        ''' <param name="time"></param> 
        ''' <param name="minutes"></param> 
        ''' <returns></returns> 
        Public Shared Function StrDateDiffMinutes(ByVal time As String, ByVal minutes As Integer) As Integer
            If time = "" OrElse time Is Nothing Then
                Return 1
            End If
            Dim ts As TimeSpan = DateTime.Now - DateTime.Parse(time).AddMinutes(minutes)
            If ts.TotalMinutes > Integer.MaxValue Then
                Return Integer.MaxValue
            ElseIf ts.TotalMinutes < Integer.MinValue Then
                Return Integer.MinValue
            End If
            Return CInt(ts.TotalMinutes)
        End Function

        ''' <summary> 
        ''' 返回相差的小时数 
        ''' </summary> 
        ''' <param name="time"></param> 
        ''' <param name="hours"></param> 
        ''' <returns></returns> 
        Public Shared Function StrDateDiffHours(ByVal time As String, ByVal hours As Integer) As Integer
            If time = "" OrElse time Is Nothing Then
                Return 1
            End If
            Dim ts As TimeSpan = DateTime.Now - DateTime.Parse(time).AddHours(hours)
            If ts.TotalHours > Integer.MaxValue Then
                Return Integer.MaxValue
            ElseIf ts.TotalHours < Integer.MinValue Then
                Return Integer.MinValue
            End If
            Return CInt(ts.TotalHours)
        End Function

        ''' <summary> 
        ''' 建立文件夹 
        ''' </summary> 
        ''' <param name="name"></param> 
        ''' <returns></returns> 
        Public Shared Function CreateDir(ByVal name As String) As Boolean
            Return Utils.MakeSureDirectoryPathExists(name)
        End Function

        ''' <summary> 
        ''' 为脚本替换特殊字符串 
        ''' </summary> 
        ''' <param name="str"></param> 
        ''' <returns></returns> 
        Public Shared Function ReplaceStrToScript(ByVal str As String) As String
            str = str.Replace("\", "\\")
            str = str.Replace("'", "\'")
            str = str.Replace("""", "\""")
            Return str
        End Function

        ''' <summary> 
        ''' 是否为ip 
        ''' </summary> 
        ''' <param name="ip"></param> 
        ''' <returns></returns> 
        Public Shared Function IsIP(ByVal ip As String) As Boolean
            Return Regex.IsMatch(ip, "^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$")
        End Function

        Public Shared Function IsIPSect(ByVal ip As String) As Boolean
            Return Regex.IsMatch(ip, "^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$")
        End Function

        ''' <summary> 
        ''' 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.* 
        ''' </summary> 
        ''' <param name="ip"></param> 
        ''' <param name="iparray"></param> 
        ''' <returns></returns> 
        Public Shared Function InIPArray(ByVal ip As String, ByVal iparray As String()) As Boolean

            Dim userip As String() = Utils.SplitString(ip, ".")
            For ipIndex As Integer = 0 To iparray.Length - 1
                Dim tmpip As String() = Utils.SplitString(iparray(ipIndex), ".")
                Dim r As Integer = 0
                For i As Integer = 0 To tmpip.Length - 1
                    If tmpip(i) = "*" Then
                        Return True
                    End If

                    If userip.Length > i Then
                        If tmpip(i) = userip(i) Then
                            r += 1
                        Else
                            Exit For
                        End If
                    Else
                        Exit For
                    End If

                Next
                If r = 4 Then
                    Return True
                End If


            Next
            Return False
        End Function

        ''' <summary> 
        ''' 获得Assembly版本号 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function GetAssemblyVersion() As String
            Return String.Format("{0}.{1}.{2}", AssemblyFileVersion.FileMajorPart, AssemblyFileVersion.FileMinorPart, AssemblyFileVersion.FileBuildPart)
        End Function

        ''' <summary> 
        ''' 获得Assembly产品名称 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function GetAssemblyProductName() As String
            Return AssemblyFileVersion.ProductName
        End Function

        ''' <summary> 
        ''' 获得Assembly产品版权 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function GetAssemblyCopyright() As String
            Return AssemblyFileVersion.LegalCopyright
        End Function
        ''' <summary> 
        ''' 创建目录 
        ''' </summary> 
        ''' <param name="name">名称</param> 
        ''' <returns>创建是否成功</returns> 
        <DllImport("dbgHelp", SetLastError:=True)> _
        Private Shared Function MakeSureDirectoryPathExists(ByVal name As String) As Boolean
        End Function

        ''' <summary> 
        ''' 写cookie值 
        ''' </summary> 
        ''' <param name="strName">名称</param> 
        ''' <param name="strValue">值</param> 
        Public Shared Sub WriteCookie(ByVal strName As String, ByVal strValue As String)
            Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies(strName)
            If cookie Is Nothing Then
                cookie = New HttpCookie(strName)
            End If
            cookie.Value = strValue
            HttpContext.Current.Response.AppendCookie(cookie)
        End Sub

        ''' <summary> 
        ''' 写cookie值 
        ''' </summary> 
        ''' <param name="strName">名称</param> 
        ''' <param name="strValue">值</param> 
        Public Shared Sub WriteCookie(ByVal strName As String, ByVal key As String, ByVal strValue As String)
            Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies(strName)
            If cookie Is Nothing Then
                cookie = New HttpCookie(strName)
            End If
            cookie(key) = strValue
            HttpContext.Current.Response.AppendCookie(cookie)
        End Sub
        ''' <summary> 
        ''' 写cookie值 
        ''' </summary> 
        ''' <param name="strName">名称</param> 
        ''' <param name="strValue">值</param> 
        ''' <param name="expires">过期时间(分钟)</param> 
        Public Shared Sub WriteCookie(ByVal strName As String, ByVal strValue As String, ByVal expires As Integer)
            Dim cookie As HttpCookie = HttpContext.Current.Request.Cookies(strName)
            If cookie Is Nothing Then
                cookie = New HttpCookie(strName)
            End If
            cookie.Value = strValue
            cookie.Expires = DateTime.Now.AddMinutes(expires)
            HttpContext.Current.Response.AppendCookie(cookie)

        End Sub

        ''' <summary> 
        ''' 读cookie值 
        ''' </summary> 
        ''' <param name="strName">名称</param> 
        ''' <returns>cookie值</returns> 
        Public Shared Function GetCookie(ByVal strName As String) As String
            If HttpContext.Current.Request.Cookies IsNot Nothing AndAlso HttpContext.Current.Request.Cookies(strName) IsNot Nothing Then
                Return HttpContext.Current.Request.Cookies(strName).Value.ToString()
            End If

            Return ""
        End Function

        ''' <summary> 
        ''' 读cookie值 
        ''' </summary> 
        ''' <param name="strName">名称</param> 
        ''' <returns>cookie值</returns> 
        Public Shared Function GetCookie(ByVal strName As String, ByVal key As String) As String
            If HttpContext.Current.Request.Cookies IsNot Nothing AndAlso HttpContext.Current.Request.Cookies(strName) IsNot Nothing AndAlso HttpContext.Current.Request.Cookies(strName)(key) IsNot Nothing Then
                Return HttpContext.Current.Request.Cookies(strName)(key).ToString()
            End If

            Return ""
        End Function

        ''' <summary> 
        ''' 得到论坛的真实路径 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function GetTrueForumPath() As String
            Dim forumPath As String = HttpContext.Current.Request.Path
            If forumPath.LastIndexOf("/") <> forumPath.IndexOf("/") Then
                forumPath = forumPath.Substring(forumPath.IndexOf("/"), forumPath.LastIndexOf("/") + 1)
            Else
                forumPath = "/"
            End If
            Return forumPath
        End Function

        ''' <summary> 
        ''' 判断字符串是否是yy-mm-dd字符串 
        ''' </summary> 
        ''' <param name="str">待判断字符串</param> 
        ''' <returns>判断结果</returns> 
        Public Shared Function IsDateString(ByVal str As String) As Boolean
            Return Regex.IsMatch(str, "(\d{4})-(\d{1,2})-(\d{1,2})")
        End Function

        ''' <summary> 
        ''' 移除Html标记 
        ''' </summary> 
        ''' <param name="content"></param> 
        ''' <returns></returns> 
        Public Shared Function RemoveHtml(ByVal content As String) As String
            Dim regexstr As String = "<[^>]*>"
            Return Regex.Replace(content, regexstr, String.Empty, RegexOptions.IgnoreCase)
        End Function

        ''' <summary> 
        ''' 过滤HTML中的不安全标签 
        ''' </summary> 
        ''' <param name="content"></param> 
        ''' <returns></returns> 
        Public Shared Function RemoveUnsafeHtml(ByVal content As String) As String
            content = Regex.Replace(content, "(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase)
            content = Regex.Replace(content, "(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase)
            Return content
        End Function

        ''' <summary> 
        ''' 将用户组Title中的font标签去掉 
        ''' </summary> 
        ''' <param name="title">用户组Title</param> 
        ''' <returns></returns> 
        Public Shared Function RemoveFontTag(ByVal title As String) As String
            Dim m As Match = RegexFont.Match(title)
            If m.Success Then
                Return m.Groups(1).Value
            End If
            Return title
        End Function

        ''' <summary> 
        ''' 判断对象是否为Int32类型的数字 
        ''' </summary> 
        ''' <param name="Expression"></param> 
        ''' <returns></returns> 
        Public Shared Function IsNumeric(ByVal Expression As Object) As Boolean
            Return TypeParse.IsNumeric(Expression)
        End Function
        ''' <summary> 
        ''' 从HTML中获取文本,保留br,p,img 
        ''' </summary> 
        ''' <param name="HTML"></param> 
        ''' <returns></returns> 
        Public Shared Function GetTextFromHTML(ByVal HTML As String) As String
            Dim regEx As New System.Text.RegularExpressions.Regex("</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)

            Return regEx.Replace(HTML, "")
        End Function

        Public Shared Function IsDouble(ByVal Expression As Object) As Boolean
            Return TypeParse.IsDouble(Expression)
        End Function

        ''' <summary> 
        ''' object型转换为bool型 
        ''' </summary> 
        ''' <param name="expression">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的bool类型结果</returns> 
        Public Shared Function StrToBool(ByVal expression As Object, ByVal defValue As Boolean) As Boolean
            Return TypeParse.StrToBool(expression, defValue)
        End Function

        ''' <summary> 
        ''' string型转换为bool型 
        ''' </summary> 
        ''' <param name="expression">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的bool类型结果</returns> 
        Public Shared Function StrToBool(ByVal expression As String, ByVal defValue As Boolean) As Boolean
            Return TypeParse.StrToBool(expression, defValue)
        End Function

        ''' <summary> 
        ''' 将对象转换为Int32类型 
        ''' </summary> 
        ''' <param name="expression">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的int类型结果</returns> 
        Public Shared Function StrToInt(ByVal expression As Object, ByVal defValue As Integer) As Integer
            Return TypeParse.StrToInt(expression, defValue)
        End Function

        ''' <summary> 
        ''' 将字符串转换为Int32类型 
        ''' </summary> 
        ''' <param name="expression">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的int类型结果</returns> 
        Public Shared Function StrToInt(ByVal expression As String, ByVal defValue As Integer) As Integer
            Return TypeParse.StrToInt(expression, defValue)
        End Function

        ''' <summary> 
        ''' Object型转换为float型 
        ''' </summary> 
        ''' <param name="strValue">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的int类型结果</returns> 
        Public Shared Function StrToFloat(ByVal strValue As Object, ByVal defValue As Single) As Single
            Return TypeParse.StrToFloat(strValue, defValue)
        End Function

        ''' <summary> 
        ''' string型转换为float型 
        ''' </summary> 
        ''' <param name="strValue">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的int类型结果</returns> 
        Public Shared Function StrToFloat(ByVal strValue As String, ByVal defValue As Single) As Single
            Return TypeParse.StrToFloat(strValue, defValue)
        End Function

        ''' <summary> 
        ''' 判断给定的字符串数组(strNumber)中的数据是不是都为数值型 
        ''' </summary> 
        ''' <param name="strNumber">要确认的字符串数组</param> 
        ''' <returns>是则返加true 不是则返回 false</returns> 
        Public Shared Function IsNumericArray(ByVal strNumber As String()) As Boolean
            Return TypeParse.IsNumericArray(strNumber)
        End Function


        Public Shared Function AdDeTime(ByVal times As Integer) As String
            Dim newtime As String = (DateTime.Now).AddMinutes(times).ToString()
            Return newtime

        End Function
        ''' <summary> 
        ''' 验证是否为正整数 
        ''' </summary> 
        ''' <param name="str"></param> 
        ''' <returns></returns> 
        Public Shared Function IsInt(ByVal str As String) As Boolean

            Return Regex.IsMatch(str, "^[0-9]*$")
        End Function

        Public Shared Function IsRuleTip(ByVal NewHash As Hashtable, ByVal ruletype As String, ByRef key As String) As Boolean
            key = ""
            For Each str As DictionaryEntry In NewHash

                Try
                    Dim [single] As String() = SplitString(str.Value.ToString(), vbCr & vbLf)

                    For Each strs As String In [single]
                        If strs <> "" Then


                            Select Case ruletype.Trim().ToLower()
                                Case "email"
                                    If IsValidDoEmail(strs.ToString()) = False Then
                                        Throw New Exception()
                                    End If
                                    Exit Select

                                Case "ip"
                                    If IsIPSect(strs.ToString()) = False Then
                                        Throw New Exception()
                                    End If
                                    Exit Select

                                Case "timesect"
                                    Dim splitetime As String() = strs.Split("-"c)
                                    If Utils.IsTime(splitetime(1).ToString()) = False OrElse Utils.IsTime(splitetime(0).ToString()) = False Then
                                        Throw New Exception()
                                    End If
                                    Exit Select

                            End Select
                        End If

                    Next


                Catch
                    key = str.Key.ToString()
                    Return False
                End Try
            Next
            Return True

        End Function

        ''' <summary> 
        ''' 删除最后一个字符 
        ''' </summary> 
        ''' <param name="str"></param> 
        ''' <returns></returns> 
        Public Shared Function ClearLastChar(ByVal str As String) As String
            If str = "" Then
                Return ""
            Else
                Return str.Substring(0, str.Length - 1)
            End If
        End Function

        ''' <summary> 
        ''' 备份文件 
        ''' </summary> 
        ''' <param name="sourceFileName">源文件名</param> 
        ''' <param name="destFileName">目标文件名</param> 
        ''' <param name="overwrite">当目标文件存在时是否覆盖</param> 
        ''' <returns>操作是否成功</returns> 
        Public Shared Function BackupFile(ByVal sourceFileName As String, ByVal destFileName As String, ByVal overwrite As Boolean) As Boolean
            If Not System.IO.File.Exists(sourceFileName) Then
                Throw New FileNotFoundException(sourceFileName & "文件不存在！")
            End If
            If Not overwrite AndAlso System.IO.File.Exists(destFileName) Then
                Return False
            End If
            Try
                System.IO.File.Copy(sourceFileName, destFileName, True)
                Return True
            Catch e As Exception
                Throw e
            End Try
        End Function


        ''' <summary> 
        ''' 备份文件,当目标文件存在时覆盖 
        ''' </summary> 
        ''' <param name="sourceFileName">源文件名</param> 
        ''' <param name="destFileName">目标文件名</param> 
        ''' <returns>操作是否成功</returns> 
        Public Shared Function BackupFile(ByVal sourceFileName As String, ByVal destFileName As String) As Boolean
            Return BackupFile(sourceFileName, destFileName, True)
        End Function


        ''' <summary> 
        ''' 恢复文件 
        ''' </summary> 
        ''' <param name="backupFileName">备份文件名</param> 
        ''' <param name="targetFileName">要恢复的文件名</param> 
        ''' <param name="backupTargetFileName">要恢复文件再次备份的名称,如果为null,则不再备份恢复文件</param> 
        ''' <returns>操作是否成功</returns> 
        Public Shared Function RestoreFile(ByVal backupFileName As String, ByVal targetFileName As String, ByVal backupTargetFileName As String) As Boolean
            Try
                If Not System.IO.File.Exists(backupFileName) Then
                    Throw New FileNotFoundException(backupFileName & "文件不存在！")
                End If
                If backupTargetFileName IsNot Nothing Then
                    If Not System.IO.File.Exists(targetFileName) Then
                        Throw New FileNotFoundException(targetFileName & "文件不存在！无法备份此文件！")
                    Else
                        System.IO.File.Copy(targetFileName, backupTargetFileName, True)
                    End If
                End If
                System.IO.File.Delete(targetFileName)
                System.IO.File.Copy(backupFileName, targetFileName)
            Catch e As Exception
                Throw e
            End Try
            Return True
        End Function

        Public Shared Function RestoreFile(ByVal backupFileName As String, ByVal targetFileName As String) As Boolean
            Return RestoreFile(backupFileName, targetFileName, Nothing)
        End Function

        ''' <summary> 
        ''' 获取记录模板id的cookie名称 
        ''' </summary> 
        ''' <returns></returns> 
        Public Shared Function GetTemplateCookieName() As String
            Return TemplateCookieName
        End Function

        ''' <summary> 
        ''' 将全角数字转换为数字 
        ''' </summary> 
        ''' <param name="SBCCase"></param> 
        ''' <returns></returns> 
        Public Shared Function SBCCaseToNumberic(ByVal SBCCase As String) As String
            Dim c As Char() = SBCCase.ToCharArray()
            For i As Integer = 0 To c.Length - 1
                Dim b As Byte() = System.Text.Encoding.Unicode.GetBytes(c, i, 1)
                If b.Length = 2 Then
                    If b(1) = 255 Then
                        b(0) = CByte((b(0) + 32))
                        b(1) = 0
                        c(i) = System.Text.Encoding.Unicode.GetChars(b)(0)
                    End If
                End If
            Next
            Return New String(c)
        End Function

        ''' <summary> 
        ''' 将字符串转换为Color 
        ''' </summary> 
        ''' <param name="color__1"></param> 
        ''' <returns></returns> 
        Public Shared Function ToColor(ByVal color__1 As String) As Color
            Dim red As Integer, green As Integer, blue As Integer = 0
            Dim rgb As Char()
            color__1 = color__1.TrimStart("#"c)
            color__1 = Regex.Replace(color__1.ToLower(), "[g-zG-Z]", "")
            Select Case color__1.Length
                Case 3
                    rgb = color__1.ToCharArray()
                    red = Convert.ToInt32(rgb(0).ToString() + rgb(0).ToString(), 16)
                    green = Convert.ToInt32(rgb(1).ToString() + rgb(1).ToString(), 16)
                    blue = Convert.ToInt32(rgb(2).ToString() + rgb(2).ToString(), 16)
                    Return Color.FromArgb(red, green, blue)
                Case 6
                    rgb = color__1.ToCharArray()
                    red = Convert.ToInt32(rgb(0).ToString() + rgb(1).ToString(), 16)
                    green = Convert.ToInt32(rgb(2).ToString() + rgb(3).ToString(), 16)
                    blue = Convert.ToInt32(rgb(4).ToString() + rgb(5).ToString(), 16)
                    Return Color.FromArgb(red, green, blue)
                Case Else
                    Return Color.FromName(color__1)

            End Select
        End Function
        ''' <summary> 
        ''' 转换长文件名为短文件名 
        ''' </summary> 
        ''' <param name="fullname"></param> 
        ''' <param name="repstring"></param> 
        ''' <param name="leftnum"></param> 
        ''' <param name="rightnum"></param> 
        ''' <param name="charnum"></param> 
        ''' <returns></returns> 
        Public Shared Function ConvertSimpleFileName(ByVal fullname As String, ByVal repstring As String, ByVal leftnum As Integer, ByVal rightnum As Integer, ByVal charnum As Integer) As String
            Dim simplefilename As String = "", leftstring As String = "", rightstring As String = "", filename As String = ""

            Dim extname As String = GetFileExtName(fullname)
            If extname = "" OrElse extname Is Nothing Then

                Throw New Exception("字符串不含有扩展名信息")
            End If
            Dim filelength As Integer = 0, dotindex As Integer = 0

            dotindex = fullname.LastIndexOf("."c)
            filename = fullname.Substring(0, dotindex)
            filelength = filename.Length
            If dotindex > charnum Then
                leftstring = filename.Substring(0, leftnum)
                rightstring = filename.Substring(filelength - rightnum, rightnum)
                If repstring = "" OrElse repstring Is Nothing Then
                    simplefilename = (leftstring + rightstring & ".") + extname
                Else
                    simplefilename = (leftstring + repstring + rightstring & ".") + extname
                End If
            Else

                simplefilename = fullname
            End If
            Return simplefilename

        End Function

        ''' <summary> 
        ''' 将数据表转换成JSON类型串 
        ''' </summary> 
        ''' <param name="dt">要转换的数据表</param> 
        ''' <returns></returns> 
        Public Shared Function DataTableToJSON(ByVal dt As System.Data.DataTable) As StringBuilder
            Return DataTableToJSON(dt, True)
        End Function

        ''' <summary> 
        ''' 将数据表转换成JSON类型串 
        ''' </summary> 
        ''' <param name="dt">要转换的数据表</param> 
        ''' <param name="dt_dispose">数据表转换结束后是否dispose掉</param> 
        ''' <returns></returns> 
        Public Shared Function DataTableToJson(ByVal dt As System.Data.DataTable, ByVal dt_dispose As Boolean) As StringBuilder
            Dim stringBuilder As New StringBuilder()
            stringBuilder.Append("[" & vbCr & vbLf)

            '数据表字段名和类型数组 
            Dim dt_field As String() = New String(dt.Columns.Count - 1) {}
            Dim i As Integer = 0
            Dim formatStr As String = "{{"
            Dim fieldtype As String = ""
            For Each dc As System.Data.DataColumn In dt.Columns
                dt_field(i) = dc.Caption.ToLower().Trim()
                formatStr += "'" & dc.Caption.ToLower().Trim() & "':"
                fieldtype = dc.DataType.ToString().Trim().ToLower()
                If fieldtype.IndexOf("int") > 0 OrElse fieldtype.IndexOf("deci") > 0 OrElse fieldtype.IndexOf("floa") > 0 OrElse fieldtype.IndexOf("doub") > 0 OrElse fieldtype.IndexOf("bool") > 0 Then
                    formatStr += "{" & i & "}"
                Else
                    formatStr += "'{" & i & "}'"
                End If
                formatStr += ","
                i += 1
            Next

            If formatStr.EndsWith(",") Then
                formatStr = formatStr.Substring(0, formatStr.Length - 1)
                '去掉尾部","号 
            End If
            formatStr += "}},"

            i = 0
            Dim objectArray As Object() = New Object(dt_field.Length - 1) {}
            For Each dr As System.Data.DataRow In dt.Rows

                For Each fieldname As String In dt_field
                    '对 \ , ' 符号进行转换 
                    objectArray(i) = dr(dt_field(i)).ToString().Trim().Replace("\", "\\").Replace("'", "\'")
                    Select Case objectArray(i).ToString()
                        Case "True"
                            If True Then
                                objectArray(i) = "true"
                                Exit Select
                            End If
                        Case "False"
                            If True Then
                                objectArray(i) = "false"
                                Exit Select
                            End If
                        Case Else
                            Exit Select
                    End Select
                    i += 1
                Next
                i = 0
                stringBuilder.Append(String.Format(formatStr, objectArray))
            Next
            If stringBuilder.ToString().EndsWith(",") Then
                stringBuilder.Remove(stringBuilder.Length - 1, 1)
                '去掉尾部","号 
            End If

            If dt_dispose Then
                dt.Dispose()
            End If
            Return stringBuilder.Append(vbCr & vbLf & "];")
        End Function

        ''' <summary> 
        ''' 字段串是否为Null或为""(空) 
        ''' </summary> 
        ''' <param name="str"></param> 
        ''' <returns></returns> 
        Public Shared Function StrIsNullOrEmpty(ByVal str As String) As Boolean
            '#if NET1 
            If str Is Nothing OrElse str.Trim() = "" Then
                Return True
            End If
            '#else 
            ' if (string.IsNullOrEmpty(str)) 
            ' { 
            ' return true; 
            ' } 
            '#endif 

            Return False
        End Function

        ''' <summary> 
        ''' 是否为数值串列表，各数值间用","间隔 
        ''' </summary> 
        ''' <param name="numList"></param> 
        ''' <returns></returns> 
        Public Shared Function IsNumericList(ByVal numList As String) As Boolean
            If numList = "" Then
                Return False
            End If
            For Each num As String In numList.Split(","c)
                If Not IsNumeric(num) Then
                    Return False
                End If
            Next
            Return True
        End Function

        ''' <summary> 
        ''' 检查颜色值是否为3/6位的合法颜色 
        ''' </summary> 
        ''' <param name="color">待检查的颜色</param> 
        ''' <returns></returns> 
        Public Shared Function CheckColorValue(ByVal color As String) As Boolean
            If StrIsNullOrEmpty(color) Then
                Return False
            End If

            color = color.Trim().Trim("#"c)

            If color.Length <> 3 AndAlso color.Length <> 6 Then
                Return False
            End If
            '不包含0-9 a-f以外的字符 
            If Not Regex.IsMatch(color, "[^0-9a-f]", RegexOptions.IgnoreCase) Then
                Return True
            End If
            Return False
        End Function

        ''' <summary> 
        ''' 获取ajax形式的分页链接 
        ''' </summary> 
        ''' <param name="curPage">当前页数</param> 
        ''' <param name="countPage">总页数</param> 
        ''' <param name="callback">回调函数</param> 
        ''' <param name="extendPage">周边页码显示个数上限</param> 
        ''' <returns></returns> 
        Public Shared Function GetAjaxPageNumbers(ByVal curPage As Integer, ByVal countPage As Integer, ByVal callback As String, ByVal extendPage As Integer) As String
            Dim pagetag As String = "page"
            Dim startPage As Integer = 1
            Dim endPage As Integer = 1

            Dim t1 As String = "<a href=""###"" onclick=""" & String.Format(callback, "&" & pagetag & "=1")
            Dim t2 As String = "<a href=""###"" onclick=""" & String.Format(callback, ("&" & pagetag & "=") + countPage)

            t1 += """>&laquo;</a>"
            t2 += """>&raquo;</a>"

            If countPage < 1 Then
                countPage = 1
            End If
            If extendPage < 3 Then
                extendPage = 2
            End If

            If countPage > extendPage Then
                If curPage - (extendPage / 2) > 0 Then
                    If curPage + (extendPage / 2) < countPage Then
                        startPage = curPage - (extendPage / 2)
                        endPage = startPage + extendPage - 1
                    Else
                        endPage = countPage
                        startPage = endPage - extendPage + 1
                        t2 = ""
                    End If
                Else
                    endPage = extendPage
                    t1 = ""
                End If
            Else
                startPage = 1
                endPage = countPage
                t1 = ""
                t2 = ""
            End If

            Dim s As New StringBuilder("")

            s.Append(t1)
            For i As Integer = startPage To endPage
                If i = curPage Then
                    s.Append("<span>")
                    s.Append(i)
                    s.Append("</span>")
                Else
                    s.Append("<a href=""###"" onclick=""")
                    s.Append(String.Format(callback, (pagetag & "=") + i))
                    s.Append(""">")
                    s.Append(i)
                    s.Append("</a>")
                End If
            Next
            s.Append(t2)

            Return s.ToString()
        End Function

        ''' <summary> 
        ''' 根据Url获得源文件内容 
        ''' </summary> 
        ''' <param name="url">合法的Url地址</param> 
        ''' <returns></returns> 
        Public Shared Function GetSourceTextByUrl(ByVal url As String) As String
            Dim request As WebRequest = WebRequest.Create(url)
            request.Timeout = 20000
            '20秒超时 
            Dim response As WebResponse = request.GetResponse()

            Dim resStream As Stream = response.GetResponseStream()
            Dim sr As New StreamReader(resStream)
            Return sr.ReadToEnd()
        End Function

        ''' <summary>
        ''' 转换时间为unix时间戳
        ''' </summary>
        ''' <param name="d">需要传递UTC时间,避免时区误差,例:DataTime.UTCNow</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ConvertToUnixTimestamp(ByVal d As DateTime) As Double
            Dim origin As New DateTime(1970, 1, 1, 0, 0, 0, 0)
            Dim diff = d - origin
            Return System.Math.Floor(diff.TotalSeconds)
        End Function

        ''' <summary>
        ''' 合并字符
        ''' </summary>
        ''' <param name="source">要合并的源字符串</param>
        ''' <param name="target">要被合并到的目的字符串</param>
        ''' <param name="mergechar">合并符</param>
        ''' <returns>并到字符串</returns>
        ''' <remarks></remarks>
        Public Shared Function MergeString(ByVal source As String, ByVal target As String, ByVal mergechar As String) As String
            If Utils.StrIsNullOrEmpty(target) Then
                target = source
            Else
                target += mergechar + source
            End If
            Return target
        End Function

        ''' <summary>
        ''' 弹出JavaScript小窗口
        ''' </summary>
        ''' <param name="message">窗口信息</param>
        ''' <param name="p"></param>
        ''' <remarks></remarks>
        Public Shared Sub Alert(ByVal message As String, ByVal p As Page)
            Dim js = "<Script language='JavaScript'>alert('" + message + "');</Script>"
            If Not p.ClientScript.IsStartupScriptRegistered(p.GetType(), "alert") Then
                p.ClientScript.RegisterStartupScript(p.GetType(), "alert", js)
            End If
        End Sub

        ''' <summary>
        ''' 转向到新的URL
        ''' </summary>
        ''' <param name="toURL">连接地址</param>
        ''' <param name="p"></param>
        ''' <remarks></remarks>
        Public Shared Sub Redirect(ByVal toURL As String, ByVal p As Page)
            Dim js = "<script language='javascript'>window.location.replace('{0}');</script>"
            If Not p.ClientScript.IsStartupScriptRegistered(p.GetType(), "Redirect") Then
                p.ClientScript.RegisterStartupScript(p.GetType(), "Redirect", String.Format(js, toURL))
            End If
        End Sub

        ''' <summary>
        ''' 弹出消息框并且转向到新的URL
        ''' </summary>
        ''' <param name="toURL">连接地址</param>
        ''' <param name="p"></param>
        ''' <remarks></remarks>
        Public Shared Sub AlertAndRedirect(ByVal message As String, ByVal toURL As String, ByVal p As Page)
            Dim js = "<Script language='JavaScript'>alert('" + message + "');window.location.replace('{0}');</Script>"
            If Not p.ClientScript.IsStartupScriptRegistered(p.GetType(), "Redirect") Then
                p.ClientScript.RegisterClientScriptBlock(p.GetType(), "Redirect", String.Format(js, toURL))
            End If
        End Sub

        ''' <summary>
        ''' hack tip:通过更新web.config文件方式来重启IIS进程池（注：iis中web园数量须大于1,且为非虚拟主机用户才可调用该方法）
        ''' </summary>
        Public Shared Sub RestartIISProcess()
            Try
                Dim xmldoc As New System.Xml.XmlDocument()
                xmldoc.Load(Utils.GetMapPath("~/web.config"))
                Dim writer As New System.Xml.XmlTextWriter(Utils.GetMapPath("~/web.config"), Nothing)
                writer.Formatting = System.Xml.Formatting.Indented
                xmldoc.WriteTo(writer)
                writer.Flush()
                writer.Close()
            Catch

            End Try
        End Sub

#Region "Private Methods"
        Private Shared browerNames As String() = {"MSIE", "Firefox", "Opera", "Netscape", "Safari", "Lynx", _
         "Konqueror", "Mozilla"}
        'private const string[] osNames = { "Win", "Mac", "Linux", "FreeBSD", "SunOS", "OS/2", "AIX", "Bot", "Crawl", "Spider" };

        ''' <summary>
        ''' 获得浏览器信息
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetClientBrower() As String
            Dim agent As String = HttpContext.Current.Request.ServerVariables("HTTP_USER_AGENT")
            If Not String.IsNullOrEmpty(agent) Then
                For Each name As String In browerNames
                    If agent.Contains(name) Then
                        Return name
                    End If
                Next
            End If
            Return "Other"
        End Function

        ''' <summary>
        ''' 判断是否在微信浏览
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsWeiXinBrower() As Boolean
            Dim agent As String = HttpContext.Current.Request.ServerVariables("HTTP_USER_AGENT")
            If Not String.IsNullOrEmpty(agent) Then
                If agent.Contains("MicroMessenger") Then
                    Return True
                End If
            End If
            Return False
        End Function

        ''' <summary>
        ''' 获得操作系统信息
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetClientOS() As String
            Dim os As String = String.Empty
            Dim agent As String = System.Web.HttpContext.Current.Request.ServerVariables("HTTP_USER_AGENT")
            If agent Is Nothing Then
                Return "Other"
            End If

            If agent.IndexOf("Win") > -1 Then
                os = "Windows"
            ElseIf agent.IndexOf("Mac") > -1 Then
                os = "Mac"
            ElseIf agent.IndexOf("Linux") > -1 Then
                os = "Linux"
            ElseIf agent.IndexOf("FreeBSD") > -1 Then
                os = "FreeBSD"
            ElseIf agent.IndexOf("SunOS") > -1 Then
                os = "SunOS"
            ElseIf agent.IndexOf("OS/2") > -1 Then
                os = "OS/2"
            ElseIf agent.IndexOf("AIX") > -1 Then
                os = "AIX"
            ElseIf System.Text.RegularExpressions.Regex.IsMatch(agent, "(Bot|Crawl|Spider)") Then
                os = "Spiders"
            Else
                os = "Other"
            End If
            Return os
        End Function
#End Region

        ''' <summary>
        ''' 清除UBB标签
        ''' </summary>
        ''' <param name="sDetail">帖子内容</param>
        ''' <returns>帖子内容</returns>
        Public Shared Function ClearUBB(sDetail As String) As String
            Return Regex.Replace(sDetail, "\[[^\]]*?\]", String.Empty, RegexOptions.IgnoreCase)
        End Function

        ''' <summary>
        ''' 获取站点根目录URL
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetRootUrl(forumPath As String) As String
            Dim port As Integer = HttpContext.Current.Request.Url.Port
            Return String.Format("{0}://{1}{2}{3}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host.ToString(), If((port = 80 OrElse port = 0), "", ":" & port), forumPath)
        End Function

        ''' <summary>
        ''' 获取指定文件的扩展名
        ''' </summary>
        ''' <param name="fileName">指定文件名</param>
        ''' <returns>扩展名</returns>
        Public Shared Function GetFileExtName(fileName As String) As String
            If Utils.StrIsNullOrEmpty(fileName) OrElse fileName.IndexOf("."c) <= 0 Then
                Return ""
            End If

            fileName = fileName.ToLower().Trim()

            Return fileName.Substring(fileName.LastIndexOf("."c), fileName.Length - fileName.LastIndexOf("."c))
        End Function

        Public Shared Function GetHttpWebResponse(url As String) As String
            Return GetHttpWebResponse(url, String.Empty)
        End Function

        ''' <summary>
        ''' http POST请求url
        ''' </summary>
        ''' <param name="postData"></param>
        ''' <returns></returns>
        Public Shared Function GetHttpWebResponse(url As String, postData As String) As String
            Dim request As HttpWebRequest = DirectCast(HttpWebRequest.Create(url), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = postData.Length
            request.Timeout = 20000

            Dim response As HttpWebResponse = Nothing

            Try
                Dim swRequestWriter As New StreamWriter(request.GetRequestStream())
                swRequestWriter.Write(postData)

                If swRequestWriter IsNot Nothing Then
                    swRequestWriter.Close()
                End If

                response = DirectCast(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream(), Encoding.UTF8)
                    Return reader.ReadToEnd()
                End Using
            Finally
                If response IsNot Nothing Then
                    response.Close()
                End If
            End Try
        End Function

        ''' <summary>
        ''' 根据字符串获取枚举值
        ''' </summary>
        ''' <typeparam name="T">枚举类型</typeparam>
        ''' <param name="value">字符串枚举值</param>
        ''' <param name="defValue">缺省值</param>
        ''' <returns></returns>
        Public Shared Function GetEnum(Of T)(value As String, defValue As T) As T
            Try
                Return DirectCast([Enum].Parse(GetType(T), value, True), T)
            Catch generatedExceptionName As ArgumentException
                Return defValue
            End Try
        End Function


        ''' <summary>
        ''' 将8位日期型整型数据转换为日期字符串数据
        ''' </summary>
        ''' <param name="date">整型日期</param>
        ''' <param name="chnType">是否以中文年月日输出</param>
        ''' <returns></returns>
        Public Shared Function FormatDate([date] As Integer, chnType As Boolean) As String
            Dim dateStr As String = [date].ToString()

            If [date] <= 0 OrElse dateStr.Length <> 8 Then
                Return dateStr
            End If

            If chnType Then
                Return dateStr.Substring(0, 4) & "年" & dateStr.Substring(4, 2) & "月" & dateStr.Substring(6) & "日"
            End If
            Return dateStr.Substring(0, 4) & "-" & dateStr.Substring(4, 2) & "-" & dateStr.Substring(6)
        End Function

        Public Shared Function FormatDate([date] As Integer) As String
            Return FormatDate([date], False)
        End Function

        Public Shared Function FilterString(ByVal inputString As String) As String
            Return RemoveUnsafeHtml(MashSQL(inputString))
        End Function

        ''' <summary>
        ''' 基于baseUrl，补全html代码中的链接
        ''' </summary>
        ''' <param name="baseUrl"></param>
        ''' <param name="html"></param>
        Public Shared Function FixUrl(baseUrl As String, html As String) As String
            html = Regex.Replace(html, "(?is)(href|src)=(""|')([^(""|')]+)(""|')", Function(match)
                                                                                       Dim org As String = match.Value
                                                                                       Dim link As String = match.Groups(3).Value
                                                                                       If link.StartsWith("http") Then
                                                                                           Return org
                                                                                       End If


                                                                                       Try
                                                                                           Dim uri As New Uri(baseUrl)
                                                                                           Dim thisUri As New Uri(uri, link)
                                                                                           Dim fullUrl As String = String.Format("{0}=""{1}""", match.Groups(1).Value, thisUri.AbsoluteUri)
                                                                                           Return fullUrl
                                                                                       Catch generatedExceptionName As Exception
                                                                                           Return org
                                                                                       End Try
                                                                                   End Function)
            Return html
        End Function
    End Class
End Namespace
Imports System.Text
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports LSW.Web

Namespace Extension
    Public Module StringHelper
        <Extension()> _
        Public Function Multiplication(ByVal str As String, ByVal i As Integer) As String
            Dim result = ""
            For j = 1 To i
                result &= str
            Next
            Return result
        End Function

        <Extension()> _
        Public Function StringKutter(ByVal str As String, ByVal count As Integer) As String
            Dim out = False
            If str.Length > count Then
                str = str.Substring(0, count)
                out = True
            ElseIf str.Length <= count / 2 Then
                Return str
            End If

            Dim fCount As Integer = System.Text.RegularExpressions.Regex.Matches(str, "[^\x00-\xff]").Count
            If str.Length + fCount <= count Then
                Return str
            Else
                out = True
            End If

            Dim sb As New StringBuilder(str)
            While fCount >= 0
                If System.Text.RegularExpressions.Regex.IsMatch(sb(sb.Length - 1).ToString(), "[^\x00-\xff]") Then
                    fCount -= 2
                Else
                    fCount -= 1
                End If
                sb.Remove(sb.Length - 1, 1)
            End While
            Return sb.ToString() & IIf(out, "..", "")
        End Function

        <Extension()> _
        Public Function IsValidEmailAddress(ByVal email As String) As Boolean
            Dim regex As New Regex("^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
            Return regex.IsMatch(email)
        End Function

        ''' <summary>
        ''' 验证单个手机号码是否正确
        ''' </summary>
        ''' <param name="strMobile">手机号码</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function IsMobileNo(ByVal strMobile As String) As Boolean
            strMobile = strMobile.Trim
            strMobile = strMobile.TrimStart("0")

            Dim strRegMobile As String = "^1(([3][456789])|([5][012789])|([8][278]))[0-9]{8}$"
            Dim strRegUnicom As String = "^1(([3][012])|([5][56])|([8][56]))[0-9]{8}$"
            Dim strRegTelecom As String = "^1(([3][3])|([5][3])|([8][09]))[0-9]{8}$"

            Dim regMobile As Regex = New Regex(strRegMobile)
            Dim regUnicom As Regex = New Regex(strRegUnicom)
            Dim regTelecom As Regex = New Regex(strRegTelecom)
            If regMobile.IsMatch(strMobile) Then
                Return True
            End If
            If regUnicom.IsMatch(strMobile) Then
                Return True
            End If
            If regTelecom.IsMatch(strMobile) Then
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' 生成日期随机码
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRamCode() As String
            Return Now.ToString("yyyyMMddHHmmssffff")
        End Function

        <Extension()>
        Public Function ContainsInArray(source As String, strs As String()) As Boolean
            For Each s In strs
                If source.Contains(s) Then Return True
            Next
            Return False
        End Function

        <Extension()> _
        Public Function shExpMatch(source As String, matstr As String) As Boolean
            If source Like matstr Then
                Return True
            Else
                Return False
            End If
        End Function

        <Extension()> _
        Public Function shExpMatch(sources() As String, matstr As String) As Integer
            Dim i = 0
            For Each source In sources
                If source Like matstr Then
                    i += 1
                End If
            Next
            Return i
        End Function

        <Extension()> _
        Public Function shExpMatchCallBackList(sources() As String, matstr As String) As List(Of String)
            Dim list As New List(Of String)
            For Each source In sources
                If source Like matstr Then
                    list.Add(source)
                End If
            Next
            Return list
        End Function

        ''' <summary>
        ''' 通过正则表达式，获取要得到的信息。
        ''' </summary>
        ''' <param name="pattern">传入正则表达式</param>
        ''' <param name="THtml">传入被正则的html页</param>
        ''' <param name="Col">要被正则的Html有几列。</param>
        ''' <returns></returns>
        <Extension>
        Public Function GetPatternHtml(ByVal pattern As String, ByVal THtml As String, ByVal Col As Integer) As String
            Dim r As New Regex(pattern, RegexOptions.Multiline Or RegexOptions.IgnoreCase)
            Dim mc As MatchCollection = r.Matches(THtml)
            Dim strTempContent As String = ""
            If mc.Count > 0 Then
                Dim num As Integer = 1
                Dim matI As Match
                For Each matI In mc
                    strTempContent += matI.Groups(0).Value + "$"
                    If num Mod Col = 0 Then
                        strTempContent += "~"
                    End If
                    num += 1
                Next
            Else
                strTempContent = "wrong"
            End If
            Return strTempContent
        End Function

        ''' <summary>
        ''' 对抓取到的网页进行分析组合成有规律的数组，不过滤HTml
        ''' </summary>
        ''' <param name="result">result：抓取后待分析的网页</param>
        ''' <param name="regexStr">regexStr：对整个网页进行正则截取时，正则开始标签</param>
        ''' <param name="regexEnd">regexEnd：对整个网页进行正则截取时，正则结束标签</param>
        ''' <param name="regexTab">regexTab：确定抓取范围后匹配某列的正则
        ''' 例如：<tr>（?<content></content>.+?）</tr>，表示获取参数2和参数3之间的字符串中所有<tr>开始，</tr>结束的所有字符串，这些字符串用$~拼接
        ''' </param>
        ''' <param name="ColNum">共有几列</param>
        ''' <param name="IsRemoveHtml">是否移除Html</param>
        ''' <returns></returns>
        <Extension>
        Public Function ResolverAndOutput(ByVal result As String, ByVal regexStr As String, ByVal regexEnd As String, ByVal regexTab As String, ByVal ColNum As Integer, ByVal IsRemoveHtml As Boolean) As String
            Dim strTempContent As String = ""
            Dim patternStart As String = regexStr  '表达式开始标签,regexStr
            Dim patternEnd As String = regexEnd  '表达式结束标签,regexEnd
            Dim regex As String = patternStart + "([\s\S]*)" + patternEnd  '组合后的表达式 
            'regex = "http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";           //匹配http:
            strTempContent = GetPatternHtml(regex, result, ColNum)        '通过正则表达式获得所需信息的大table
            If strTempContent <> "wrong" Then
                strTempContent = strTempContent.Replace(vbCrLf, "")            '去掉\n符
                strTempContent = strTempContent.Replace("></td>", "> </td>")            '在<td></td>之间加入空字符，以便被正则
                'strTempContent = strTempContent.Replace("\r", "");          //去掉\r符
                Dim regex2 As String = regexTab  '确定抓取范围后匹配某列的正则，regexTab
                strTempContent = GetPatternHtml(regex2, strTempContent, ColNum) '正则找到每列值
                If IsRemoveHtml = True Then
                    strTempContent = LSW.Web.Utils.RemoveHtml(strTempContent)         '正则移除html标签
                End If
                If strTempContent <> "wrong" Then
                    Return strTempContent
                Else
                    Dim tmpNull As String = ""
                    Dim i As Integer
                    For i = 0 To ColNum - 1 Step i + 1
                        tmpNull = tmpNull + "Null$"
                    Next
                    Return tmpNull + "?"
                End If
            Else
                Dim retNull As String = ""
                Dim i As Integer
                For i = 0 To ColNum - 1 Step i + 1
                    retNull = retNull + "Null$"
                Next
                Return retNull + "?"
            End If
        End Function

        ''' <summary>
        ''' 对抓取到的网页进行分析并输出
        ''' </summary>
        ''' <param name="result">result：抓取后待分析的网页</param>
        ''' <param name="regexStr">regexStr：对整个网页进行正则截取时，正则开始标签</param>
        ''' <param name="regexEnd">regexEnd：对整个网页进行正则截取时，正则结束标签</param>
        ''' <param name="regexTab">regexTab：确定抓取范围后匹配某列的正则</param>
        ''' <param name="ColNum">共有几列</param>
        ''' <returns></returns>
        <Extension>
        Public Function ResolverAndOutput(ByVal result As String, ByVal regexStr As String, ByVal regexEnd As String, ByVal regexTab As String, ByVal ColNum As Integer) As String
            Return ResolverAndOutput(result, regexStr, regexEnd, regexTab, ColNum, True)
        End Function

        ''' <summary>
        ''' 返回截取html后的字符串
        ''' </summary>
        ''' <param name="pageHtml">要截取的字符串</param>
        ''' <param name="starts">截取起点</param>
        ''' <param name="ends">截取终点</param>
        ''' <returns>截取后的字符串</returns>
        <Extension>
        Public Function CutOut(ByVal pageHtml As String, ByVal starts As String, ByVal ends As String) As String
            Dim keyText As String = ""
            Dim StrLen As Integer = starts.Length
            If starts.Trim() <> "" Or ends.Trim() <> "" Then
                Dim m As Integer = pageHtml.IndexOf(starts.Trim())  '找出starts的位置
                If m = -1 Then
                    Return "没找到当前指定的START"                              '没有查找到数据，直接返回
                End If
                Dim pageText As String = pageHtml.Remove(0, m + StrLen)  '删除starts以上的html文本
                If Not String.IsNullOrEmpty(ends) Then
                    Dim n As Integer = pageText.IndexOf(ends.Trim())  '找出ends的位置
                    keyText = pageText.Remove(n - 0) '删除ends以下的html文本
                Else
                    keyText = pageText
                End If
            Else
                keyText = pageHtml
            End If
            keyText = keyText.Replace(vbCrLf, "")
            Return keyText
        End Function

        <Extension>
        Public Function RemoveVBCrLf(str As String) As String
            Return str.Replace(vbCr, "").Replace(vbLf, "").Trim
        End Function

        <Extension>
        Public Function RemoveWhiteSpace(str As String) As String
            Return str.Replace(Space(1), "").Replace("　", "")
        End Function

        <Extension>
        Public Function ToPostDataString(Of T)(obj As T) As String
            Return String.Join("&", GetType(T).GetProperties.Select(Function(p) Utils.UrlEncode(p.Name) & "=" & Utils.UrlEncode(p.GetValue(obj, Nothing).ToString)))
        End Function

        <Extension>
        Public Function HexStringsToBytes(str() As String) As Byte()
            Return str.Where(Function(s) Not String.IsNullOrWhiteSpace(s.RemoveVBCrLf)).Select(Function(s) Convert.ToByte(s.RemoveVBCrLf, 16)).ToArray
        End Function

        <Extension>
        Public Function CharCount(str As String) As Dictionary(Of Char, Integer)
            Return str.GroupBy(Function(w) w).Select(Function(w) New KeyValuePair(Of Char, Integer)(w.Key, w.Count)).OrderBy(Function(w) w.Key)
        End Function

        <Extension>
        Public Function CharCount(str As String, c As Char) As Integer
            Return str.Where(Function(w) w = c).Count
        End Function

        <Extension>
        Public Function MakeTypoglycemia(text As String) As String
            Dim random As New Random
            Return String.Join(" ", text.Split(" ").Select(Function(s) IIf(s.Length <= 3, s, s(0)) & s.Substring(1, s.Length - 2).OrderBy(Function() random.Next()).ToArray() & s(s.Length - 1)))
        End Function

        ''' <summary>
        ''' 转全角(SBC case)
        ''' </summary>
        ''' <param name="input">任意字符串</param>
        ''' <returns>全角字符串</returns>
        <Extension> _
        Public Function ToSBC(input As String) As String
            Dim c As Char() = input.ToCharArray()
            For i As Integer = 0 To c.Length - 1
                If AscW(c(i)) = 32 Then
                    c(i) = ChrW(12288)
                    Continue For
                End If
                If AscW(c(i)) < 127 Then
                    c(i) = ChrW(AscW(c(i)) + 65248)
                End If
            Next
            Return New String(c)
        End Function
        ''' <summary>
        ''' 转半角(DBC case)
        ''' </summary>
        ''' <param name="input">任意字符串</param>
        ''' <returns>半角字符串</returns>
        <Extension> _
        Public Function ToDBC(input As String) As String
            Dim c As Char() = input.ToCharArray()
            For i As Integer = 0 To c.Length - 1
                If AscW(c(i)) = 12288 Then
                    c(i) = ChrW(32)
                    Continue For
                End If
                If AscW(c(i)) > 65280 AndAlso AscW(c(i)) < 65375 Then
                    c(i) = ChrW(AscW(c(i)) - 65248)
                End If
            Next
            Return New String(c)
        End Function

        <Extension> _
        Public Function IsNullOrEmpty(s As String) As Boolean
            Return String.IsNullOrEmpty(s)
        End Function

        <Extension> _
        Public Function FormatWith(format As String, ParamArray args() As Object) As String
            Return String.Format(format, args)
        End Function

        ''' <summary>
        ''' KMP算法查找字符串
        ''' </summary>
        ''' <param name="operateStr">操作字符串</param>
        ''' <param name="findStr">要查找的字符串</param>
        ''' <returns>字符串第一次出现的位置索引</returns>
        Public Function Arithmetic_KMP(operateStr As String, findStr As String) As Integer
            Dim index As Integer = -1
            '正确匹配的开始索引
            Dim tableValue As Integer() = GetPartialMatchTable(findStr)
            Dim i As Integer = 0, j As Integer = 0
            '操作字符串和匹配字符串 索引迭代
            While i < operateStr.Length AndAlso j < findStr.Length
                If operateStr(i) = findStr(j) Then
                    '当第一个字符匹配上，接着匹配第二、、、
                    If j = 0 Then
                        index = i
                    End If
                    '记录第一个匹配字符的索引
                    j += 1
                    i += 1
                Else
                    '当没有匹配上的时候
                    If j = 0 Then
                        '如果第一个字符就没匹配上
                        '移动位数 =已匹配的字符数 - 对应的部分匹配值
                        i += j + 1 - tableValue(j)
                    Else
                        '如果已匹配的字符数不为零，则重新定义i迭代
                        i = index + j - tableValue(j - 1)
                    End If
                    '将已匹配迭代置为0
                    j = 0
                End If
            End While
            Return index
        End Function
        ''' <summary>
        ''' 产生 部分匹配表
        ''' </summary>
        ''' <param name="str">要查找匹配的字符串</param>
        ''' <returns></returns>
        Public Function GetPartialMatchTable(str As String) As Integer()
            Dim left As String(), right As String()
            '前缀、后缀
            Dim result As Integer() = New Integer(str.Length - 1) {}
            '保存 部分匹配表
            For i As Integer = 0 To str.Length - 1
                left = New String(i - 1) {}
                '实例化前缀 容器
                right = New String(i - 1) {}
                '实例化后缀容器
                '前缀
                For j As Integer = 0 To i - 1
                    If j = 0 Then
                        left(j) = str(j).ToString()
                    Else
                        left(j) = left(j - 1) & str(j).ToString()
                    End If
                Next
                '后缀
                For k As Integer = i To 1 Step -1
                    If k = i Then
                        right(k - 1) = str(k).ToString()
                    Else
                        right(k - 1) = str(k).ToString() & right(k)
                    End If
                Next
                '找到前缀和后缀中相同的项，长度即为相等项的长度（相等项应该只有一项）
                Dim num As Integer = left.Length - 1
                For m As Integer = 0 To left.Length - 1
                    If right(num) = left(m) Then
                        result(i) = left(m).Length
                    End If
                    num -= 1
                Next
            Next
            Return result
        End Function
        ''' <summary>
        ''' 尾递归查询出 字符串出现的所有开始索引 
        ''' </summary>
        ''' <param name="str1">操作字符串</param>
        ''' <param name="str2">要查找的字符串</param>
        ''' <param name="indexs">位置索引 集合</param>
        Public Sub Search(str1 As String, str2 As String, indexs As IList(Of Integer))
            Dim index As Integer = Arithmetic_KMP(str1, str2)
            Dim temp As Integer = index
            If indexs.Count > 0 Then
                index += indexs(indexs.Count - 1) + str2.Length
            End If
            indexs.Add(index)
            If temp + (str2.Length - 1) * 2 <= str1.Length Then
                Search(str1.Substring(temp + str2.Length), str2, indexs)
            End If
        End Sub
        Public Function KmpIndexOf(s As String, t As String) As Integer
            Dim i As Integer = 0, j As Integer = 0, v As Integer
            Dim nextVal As Integer() = GetNextVal(t)

            While i < s.Length AndAlso j < t.Length
                If j = -1 OrElse s(i) = t(j) Then
                    i += 1
                    j += 1
                Else
                    j = nextVal(j)
                End If
            End While

            If j >= t.Length Then
                v = i - t.Length
            Else
                v = -1
            End If

            Return v
        End Function

        Public Function GetNextVal(t As String) As Integer()
            Dim j As Integer = 0, k As Integer = -1
            Dim nextVal As Integer() = New Integer(t.Length - 1) {}

            nextVal(0) = -1

            While j < t.Length - 1
                If k = -1 OrElse t(j) = t(k) Then
                    j += 1
                    k += 1
                    If t(j) <> t(k) Then
                        nextVal(j) = k
                    Else
                        nextVal(j) = nextVal(k)
                    End If
                Else
                    k = nextVal(k)
                End If
            End While

            Return nextVal
        End Function
    End Module
End Namespace
Imports System.Text
Imports System.Web

Namespace API.Alipay
    ''' <summary>
    ''' 类名：Core
    ''' 功能：支付宝接口公用函数类
    ''' 详细：该类是请求、通知返回两个文件所调用的公用函数核心处理文件，不需要修改
    ''' 版本：3.2
    ''' 修改日期：2011-03-17
    ''' 说明：
    ''' 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    ''' 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    ''' </summary>
    Public Module Core
        ''' <summary>
        ''' 生成签名结果
        ''' </summary>
        ''' <param name="dicArray">要签名的数组</param>
        ''' <param name="key">安全校验码</param>
        ''' <param name="sign_type">签名类型</param>
        ''' <param name="_input_charset">编码格式</param>
        ''' <returns>签名结果字符串</returns>
        Public Function BuildMysign(dicArray As Dictionary(Of String, String), key As String, sign_type As String, _input_charset As String) As String
            Dim prestr = CreateLinkString(dicArray) '把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            prestr &= key '把拼接后的字符串再与安全校验码直接连接起来
            Return Sign(prestr, sign_type, _input_charset) '把最终的字符串签名，获得签名结果
        End Function

        ''' <summary>
        ''' 除去数组中的空值和签名参数并以字母a到z的顺序排序
        ''' </summary>
        ''' <param name="dicArrayPre">过滤前的参数组</param>
        ''' <returns>过滤后的参数组</returns>
        Public Function FilterPara(dicArrayPre As SortedDictionary(Of String, String)) As Dictionary(Of String, String)
            Dim dicArray As New Dictionary(Of String, String)
            For Each t In dicArrayPre
                If t.Key.ToLower <> "sign" AndAlso t.Key.ToLower <> "sign_type" AndAlso Not String.IsNullOrEmpty(t.Value) Then
                    dicArray.Add(t.Key.ToLower, t.Value)
                End If
            Next
            Return dicArray
        End Function

        ''' <summary>
        ''' 数组所有元素，按照“参数=参数值”的模式用“ ”字符拼接成字符串
        ''' </summary>
        ''' <param name="dicArray">需要拼接的数组</param>
        ''' <returns>拼接完成以后的字符串</returns>
        Public Function CreateLinkString(dicArray As Dictionary(Of String, String)) As String
            Dim prestr As New StringBuilder
            For Each t In dicArray
                prestr.Append(t.Key & "=" & t.Value + "&")
            Next

            '去掉最後一個&字符
            Dim nLen = prestr.Length
            prestr.Remove(nLen - 1, 1)
            Return prestr.ToString
        End Function

        ''' <summary>
        ''' 数组所有元素，按照“参数 = 参数值”的模式用“ ”字符拼接成字符串
        ''' </summary>
        ''' <param name="dicArray">需要拼接的数组</param>
        ''' <param name="code">字符编码</param>
        ''' <returns>拼接完成以后的字符串</returns>
        ''' <remarks></remarks>
        Public Function CreateLinkString(dicArray As Dictionary(Of String, String), code As Encoding) As String
            Dim prestr As New StringBuilder
            For Each t In dicArray
                prestr.Append(t.Key & "=" & HttpUtility.UrlEncode(t.Value, code) + "&")
            Next

            '去掉最後一個&字符
            Dim nLen = prestr.Length
            prestr.Remove(nLen - 1, 1)
            Return prestr.ToString
        End Function

        ''' <summary>
        ''' 签名字符串
        ''' </summary>
        ''' <param name="prestr">需要签名的字符串</param>
        ''' <param name="sign_type">签名类型</param>
        ''' <param name="input_charset">编码格式</param>
        ''' <returns>签名结果</returns>
        Public Function Sign(prestr As String, sign_type As String, input_charset As String) As String
            If sign_type.ToUpper = "MD5" Then
                Return LSW.Web.Utils.MD5(prestr)
            End If

            Return ""
        End Function
    End Module
End Namespace
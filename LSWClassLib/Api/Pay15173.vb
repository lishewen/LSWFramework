Imports System.Security.Cryptography
Imports System.Text
Imports System.Collections.Specialized

Namespace API
    Public Class Pay15173
        ' Methods
        Private Shared Function GetMD5(ByVal encypStr As String) As String
            Dim provider As New MD5CryptoServiceProvider
            Dim bytes As Byte() = Encoding.GetEncoding("GB2312").GetBytes(encypStr)
            Return BitConverter.ToString(provider.ComputeHash(bytes)).Replace("-", "").ToUpper
        End Function

        Private Function GetPayResultSign() As String
            Return Pay15173.GetMD5(String.Concat(New Object() {"pay_result=", Me.PayResult, "&bargainor_id=", Me.Bargainor_id, "&sp_billno=", Me.Sp_billno, "&total_fee=", Me.Total_fee, "&attach=", Me.Attach, "&key=", Me.Key}))
        End Function

        Private Function GetPaySign() As String
            Return Pay15173.GetMD5(String.Concat(New String() {"bargainor_id=", Me.Bargainor_id, "&sp_billno=", Me.Sp_billno, "&pay_type=", Me.Pay_type, "&return_url=", Me.Return_url, "&attach=", Me.Attach, "&key=", Me.Key}))
        End Function

        Public Function GetPayUrl(ByRef url As String) As Boolean
            If (Me.Sp_billno = "") Then
                url = "请给出正确的订单号."
                Return False
            End If
            Try
                Dim paySign As String = Me.GetPaySign
                url = String.Concat(New Object() {Me.paygateurl, "?bargainor_id=", Me.Bargainor_id, "&sp_billno=", Me.Sp_billno, "&total_fee=", Me.Total_fee, "&pay_type=", Me.Pay_type, "&return_url=", Me.Return_url, "&attach=", Me.Attach, "&sign=", paySign})
                Return True
            Catch exception As Exception
                url = ("创建URL时出错,错误信息:" & exception.Message)
                Return False
            End Try
        End Function

        Public Function GetPayValueFromUrl(ByVal querystring As NameValueCollection, ByRef errmsg As String) As Boolean
            If ((querystring Is Nothing) OrElse (querystring.Count = 0)) Then
                errmsg = "参数为空"
                Return False
            End If
            If (querystring.Item("pay_result") Is Nothing) Then
                errmsg = "没有pay_result参数"
                Return False
            End If
            If (querystring.Item("transaction_id") Is Nothing) Then
                errmsg = "没有transaction_id参数"
                Return False
            End If
            If (querystring.Item("bargainor_id") Is Nothing) Then
                errmsg = "没有bargainor_id参数"
                Return False
            End If
            If (querystring.Item("sp_billno") Is Nothing) Then
                errmsg = "没有sp_billno参数"
                Return False
            End If
            If (querystring.Item("pay_info") Is Nothing) Then
                errmsg = "没有pay_info参数"
                Return False
            End If
            If (querystring.Item("total_fee") Is Nothing) Then
                errmsg = "没有total_fee参数"
                Return False
            End If
            If (querystring.Item("attach") Is Nothing) Then
                errmsg = "没有attach参数"
                Return False
            End If
            If (querystring.Item("sign") Is Nothing) Then
                errmsg = "没有sign参数"
                Return False
            End If
            errmsg = ""
            Try
                m_payresult = Integer.Parse(querystring.Item("pay_result").Trim)
                Me.Transaction_id = Pay15173.UrlDecode(querystring.Item("transaction_id").Trim)
                Me.Pay_info = Pay15173.UrlDecode(querystring.Item("pay_info").Trim)
                Me.Sp_billno = Pay15173.UrlDecode(querystring.Item("sp_billno"))
                Me.Total_fee = Decimal.Parse(querystring.Item("total_fee"))
                Me.Attach = querystring.Item("attach")
                If (querystring.Item("bargainor_id") <> Me.Bargainor_id) Then
                    m_payresult = 1
                    Return True
                End If
                Dim str As String = querystring.Item("sign")
                If (Me.GetPayResultSign <> str) Then
                    m_payresult = 2
                End If
                Return True
            Catch exception As Exception
                errmsg = ("解析参数出错:" & exception.Message)
                Return False
            End Try
        End Function

        Public Sub InitPayParam(ByVal adate As String, ByVal abillno As String, ByVal aattach As String)
            Me.Sp_billno = abillno
            Me.Attach = aattach
        End Sub

        Private Shared Function UrlDecode(ByVal instr As String) As String
            If ((Not instr Is Nothing) AndAlso Not (instr.Trim = "")) Then
                Return instr.Replace("%3d", "=").Replace("%26", "&").Replace("%22", """").Replace("%3f", "?").Replace("%27", "'").Replace("%20", " ").Replace("%25", "%")
            End If
            Return ""
        End Function

        Private Shared Function UrlEncode(ByVal instr As String) As String
            If ((Not instr Is Nothing) AndAlso Not (instr.Trim = "")) Then
                Return instr.Replace("%", "%25").Replace("=", "%3d").Replace("&", "%26").Replace("""", "%22").Replace("?", "%3f").Replace("'", "%27").Replace(" ", "%20")
            End If
            Return ""
        End Function


        ' Properties
        Public Property Attach As String
            Get
                Return Pay15173.UrlDecode(m_attach)
            End Get
            Set(ByVal value As String)
                m_attach = Pay15173.UrlEncode(value)
            End Set
        End Property

        Public Property Bargainor_id As String

        Public Property Key As String

        Public Property Pay_info As String

        Public Property Pay_type As String

        Public ReadOnly Property PayErrMsg As String
            Get
                Return m_payerrmsg
            End Get
        End Property

        Public ReadOnly Property PayResult As Integer
            Get
                Return m_payresult
            End Get
        End Property

        Public ReadOnly Property PayResultStr As String
            Get
                Select Case Me.PayResult
                    Case 0
                        Return "支付成功"
                    Case 1
                        Return "商户号错"
                    Case 2
                        Return "签名错误"
                    Case 3
                        Return "支付失败"
                    Case 4
                        Return "支付不符"
                End Select
                Return ("未知类型(" & Me.PayResult & ")")
            End Get
        End Property

        Public Property Return_url As String

        Public Property Sp_billno As String

        Public Property Total_fee As Decimal

        Public Property Transaction_id As String

        ' Fields
        Private m_attach As String = ""
        Private m_payerrmsg As String = ""
        Public Const PAYERROR As Integer = 3
        Private paygateurl As String = "http://pay.15173.net/Pay_gate.aspx"
        Public Const PAYMD5ERROR As Integer = 2
        Public Const PAYNOTALL As Integer = 4
        Public Const PAYOK As Integer = 0
        Private m_payresult As Integer
        Public Const PAYSPERROR As Integer = 1
    End Class
End Namespace

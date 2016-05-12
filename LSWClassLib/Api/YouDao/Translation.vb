Imports LSW.Web
Imports LSW.Net

Namespace API.YouDao
    Public Class Translation
        Public Property APIkey As String
        Public Property Keyfrom As String
        Const APIUrl = "http://fanyi.youdao.com/openapi.do?keyfrom={0}&key={1}&type=data&doctype={2}&version=1.1&q={3}"

        Public Sub New(apikey As String, keyfrom As String)
            Me.APIkey = apikey
            Me.Keyfrom = keyfrom
        End Sub

        Public Sub New()
            Me.APIkey = "1335086189"
            Me.Keyfrom = "lishewen"
        End Sub

        Public Function Query(doctype As String, q As String) As String
            Dim url = String.Format(APIUrl, Keyfrom, APIkey, doctype, Utils.UrlEncode(q))
            Return GetHtml(url)
        End Function

        Public Function Translate(txt As String) As String
            Dim xml = Query("xml", txt)
            Dim x = XDocument.Parse(xml)
            Return x...<paragraph>.Value
        End Function
    End Class
End Namespace
Imports LSW.Web
Namespace Image
    Public Module QRCode
        Public Function GetQRCodeUrl(content As String) As String
            content = Utils.UrlEncode(content)
            Return String.Format("https://www.google.com/chart?chs=200x200&chld=M|0&cht=qr&chl={0}", content)
        End Function
    End Module
End Namespace
Imports System.Security.Cryptography
Imports System.Text
Imports System.Runtime.InteropServices

Namespace Security
    Public Module RSA
        Public Function Encrypt(input As String, publickey As String) As String
            Dim r As New RSACryptoServiceProvider(1024)
            r.FromXmlString(publickey)
            Dim encryptData = r.Encrypt(Encoding.UTF8.GetBytes(input), True)
            Return Convert.ToBase64String(encryptData)
        End Function

        Public Function Decrypt(input As String, privatekey As String) As String
            Dim r As New RSACryptoServiceProvider(1024)
            r.FromXmlString(privatekey)
            Dim dencryptData = r.Decrypt(Convert.FromBase64String(input), True)
            Return Encoding.UTF8.GetString(dencryptData)
        End Function

        Public Sub GetkeyXml(ByRef publicKey As String, ByRef privateKey As String)
            Dim r As New RSACryptoServiceProvider(1024)
            publicKey = r.ToXmlString(False)
            privateKey = r.ToXmlString(True)
        End Sub
    End Module
End Namespace
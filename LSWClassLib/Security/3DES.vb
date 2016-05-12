Imports System.Text
Imports System.Security.Cryptography

Namespace Security
    Public Module _3DES
        ''' <summary>
        ''' 3des加密算法
        ''' </summary>
        ''' <param name="strString"></param>
        ''' <param name="strKey"></param>
        ''' <returns></returns>
        Public Function Encrypt3Des(strString As String, strKey As String) As String
            Dim strbyte As Byte() = Convert.FromBase64String(strKey)

            strKey = Encoding.UTF8.GetString(strbyte)

            Dim des As New TripleDESCryptoServiceProvider()

            des.Key = ASCIIEncoding.ASCII.GetBytes(strKey)
            des.Mode = CipherMode.ECB

            Dim desEncrypt As ICryptoTransform = des.CreateEncryptor()

            Dim Buffer As Byte() = ASCIIEncoding.ASCII.GetBytes(strString)
            Return Convert.ToBase64String(desEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length))
        End Function
    End Module
End Namespace
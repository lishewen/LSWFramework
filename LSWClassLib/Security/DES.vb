Imports System.Security.Cryptography
Imports System.Text

Namespace Security
    Public Module DES
        ''' <summary>
        ''' 进行DES加密。
        ''' </summary>
        ''' <param name="pToEncrypt">要加密的字符串。</param>
        ''' <param name="sKey">密钥，且必须为8位。</param>
        ''' <returns>以Base64格式返回的加密字符串。</returns>
        Public Function Encrypt(pToEncrypt As String, Optional sKey As String = "lishewen") As String
            Using des As New DESCryptoServiceProvider()
                Dim inputByteArray As Byte() = Encoding.UTF8.GetBytes(pToEncrypt)
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey)
                des.IV = ASCIIEncoding.ASCII.GetBytes("lsw85109")
                Dim ms As New System.IO.MemoryStream()
                Using cs As New CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write)
                    cs.Write(inputByteArray, 0, inputByteArray.Length)
                    cs.FlushFinalBlock()
                    cs.Close()
                End Using
                Dim str As String = Convert.ToBase64String(ms.ToArray())
                ms.Close()
                Return str
            End Using
        End Function

        ''' <summary>
        ''' 进行DES解密。
        ''' </summary>
        ''' <param name="pToDecrypt">要解密的以Base64</param>
        ''' <param name="sKey">密钥，且必须为8位。</param>
        ''' <returns>已解密的字符串。</returns>
        Public Function Decrypt(pToDecrypt As String, Optional sKey As String = "lishewen") As String
            Dim inputByteArray As Byte() = Convert.FromBase64String(pToDecrypt)
            Using des As New DESCryptoServiceProvider()
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey)
                des.IV = ASCIIEncoding.ASCII.GetBytes("lsw85109")
                Dim ms As New System.IO.MemoryStream()
                Using cs As New CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write)
                    cs.Write(inputByteArray, 0, inputByteArray.Length)
                    cs.FlushFinalBlock()
                    cs.Close()
                End Using
                Dim str As String = Encoding.UTF8.GetString(ms.ToArray())
                ms.Close()
                Return str
            End Using
        End Function
    End Module
End Namespace
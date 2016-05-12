Imports System.Text
Imports System.Security.Cryptography
Imports System.IO

Namespace Security
    Public Module Rijndael
        Public Function Encrypt(input As String) As String
            Return Encrypt(input, "lsw85109")
        End Function

        Public Function Encrypt(input As String, password As String) As String
            Dim salt = Convert.FromBase64String(GetSalt(9, 18))
            Dim key(0) As Byte
            Dim iv(0) As Byte
            GetKeyAndIVFromPasswordAndSalt(password, salt, New RijndaelManaged, key, iv)
            Return Encrypt(input, key, iv)
        End Function

        Public Function Encrypt(input As String, key() As Byte, iv() As Byte) As String
            Dim inputBytes = Encoding.UTF8.GetBytes(input)
            Dim r As New RijndaelManaged
            Dim transform = r.CreateEncryptor(key, iv)
            Dim encrytData() As Byte
            Using outputStream As New MemoryStream
                Using inputStream As New CryptoStream(outputStream, transform, CryptoStreamMode.Write)
                    inputStream.Write(inputBytes, 0, inputBytes.Length)
                    inputStream.FlushFinalBlock()
                    encrytData = outputStream.ToArray
                End Using
            End Using
            Return Convert.ToBase64String(encrytData)
        End Function

        Public Function Decrypt(input As String) As String
            Return Decrypt(input, "lsw85109")
        End Function

        Public Function Decrypt(input As String, password As String) As String
            Dim salt = Convert.FromBase64String(GetSalt(9, 18))
            Dim key(0) As Byte
            Dim iv(0) As Byte
            GetKeyAndIVFromPasswordAndSalt(password, salt, New RijndaelManaged, key, iv)
            Return Decrypt(input, key, iv)
        End Function

        Public Function Decrypt(input As String, key() As Byte, iv() As Byte) As String
            Dim inputBytes = Convert.FromBase64String(input)
            Dim r As New RijndaelManaged
            Dim transform = r.CreateDecryptor(key, iv)
            Dim decryptByte() As Byte
            Using outputStream As New MemoryStream
                Using inputStream As New CryptoStream(outputStream, transform, CryptoStreamMode.Write)
                    inputStream.Write(inputBytes, 0, inputBytes.Length)
                    inputStream.FlushFinalBlock()
                    decryptByte = outputStream.ToArray()
                End Using
            End Using
            Return Encoding.UTF8.GetString(decryptByte)
        End Function

        Public Sub GetKeyAndIVFromPasswordAndSalt(password As String, salt() As Byte, sa As SymmetricAlgorithm, ByRef key() As Byte, ByRef iv() As Byte)
            Dim r As New Rfc2898DeriveBytes(password, salt)
            key = r.GetBytes(sa.KeySize / 8)
            iv = r.GetBytes(sa.BlockSize / 8)
        End Sub

        Public Function GenerateMac(input As String, key() As Byte) As String
            Dim hmac As New HMACSHA512(key)
            Dim data = hmac.ComputeHash(Convert.FromBase64String(input))
            Return Convert.ToBase64String(data)
        End Function

        Public Function IsMacValid(input As String, key() As Byte, savedMac As String) As Boolean
            Dim recalculateMac = GenerateMac(input, key)
            Return recalculateMac.Equals(savedMac)
        End Function
    End Module
End Namespace
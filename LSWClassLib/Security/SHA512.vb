Imports System.Text
Imports System.Security.Cryptography
Imports LSW.Extension

Namespace Security
    Public Module SHA512
        Public Function GetHash(input As String) As String
            Dim inputBytes = Encoding.UTF8.GetBytes(input)
            Dim sha5 As New SHA512Managed
            Dim outputBytes = sha5.ComputeHash(inputBytes)
            Return Convert.ToBase64String(outputBytes)
        End Function

        Public Function GetHash(input As String, salt As String) As String
            Dim saltBytes = Convert.FromBase64String(salt)
            Dim inputBytes = Encoding.UTF8.GetBytes(input)
            Dim inputWithSaltBytes(saltBytes.Length + inputBytes.Length - 1) As Byte
            Array.Copy(inputBytes, 0, inputWithSaltBytes, 0, inputBytes.Length)
            Array.Copy(saltBytes, 0, inputWithSaltBytes, inputBytes.Length, saltBytes.Length)
            Dim sha5 As New SHA512Managed
            Dim outputBytes = sha5.ComputeHash(inputWithSaltBytes)
            Return Convert.ToBase64String(outputBytes)
        End Function

        Public Function GetPHPHash(input As String, salt As String) As String
            Dim keyByte = Encoding.UTF8.GetBytes(salt)
            Using sha5 = New HMACSHA512(keyByte)
                sha5.ComputeHash(Encoding.UTF8.GetBytes(input))
                Return sha5.Hash.ToHex.ToLower
            End Using
        End Function
    End Module
End Namespace
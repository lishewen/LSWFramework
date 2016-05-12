Imports System.Security.Cryptography

Namespace Security
    Public Module Common
        Public Function GetSalt(minSize As Integer, maxSize As Integer) As String
            Dim r As New Random
            Dim saltSize = r.Next(minSize, maxSize)
            Dim saltBytes(saltSize - 1) As Byte
            Dim rng As New RNGCryptoServiceProvider
            rng.GetNonZeroBytes(saltBytes)
            Return Convert.ToBase64String(saltBytes)
        End Function
    End Module
End Namespace
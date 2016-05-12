Imports System.Security.Cryptography

Namespace Security
    Public Class XTEAManaged
        Inherits XTEA

        Public Sub New()
            LegalKeySizesValue = New KeySizes() {New KeySizes(128, 128, 0)}
            LegalBlockSizesValue = New KeySizes() {New KeySizes(64, 64, 0)}
        End Sub

        Public Overloads Overrides Function CreateDecryptor(ByVal rgbKey() As Byte, ByVal rgbIV() As Byte) As System.Security.Cryptography.ICryptoTransform
            Return New XTEAManagedDecryptor(rgbKey)
        End Function

        Public Overloads Overrides Function CreateEncryptor(ByVal rgbKey() As Byte, ByVal rgbIV() As Byte) As System.Security.Cryptography.ICryptoTransform
            Return New XTEAManagedEncryptor(rgbKey)
        End Function

        Public Overrides Sub GenerateIV()
            Dim x_random = RandomNumberGenerator.Create
            IVValue = New Byte(8) {}
            x_random.GetBytes(IVValue)
        End Sub

        Public Overrides Sub GenerateKey()
            Dim x_random = RandomNumberGenerator.Create
            KeyValue = New Byte(16) {}
            x_random.GetBytes(KeyValue)
        End Sub
    End Class
End Namespace

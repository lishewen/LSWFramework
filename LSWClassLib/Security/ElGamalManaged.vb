Imports System.Security.Cryptography

Namespace Security
    Public Class ElGamalManaged
        Inherits ElGamal

        Private o_key_struct As ElGamalKeyStruct

        Public Sub New()
            o_key_struct = New ElGamalKeyStruct
            o_key_struct.P = New BigInteger(0)
            o_key_struct.G = New BigInteger(0)
            o_key_struct.Y = New BigInteger(0)
            o_key_struct.X = New BigInteger(0)
            KeySizeValue = 1024
            LegalKeySizesValue = New KeySizes() {New KeySizes(384, 1088, 8)}
        End Sub

        Public Overrides Function DecryptData(ByVal p_data() As Byte) As Byte()
            If NeedToGenerateKey() Then
                CreateKeyPair(KeySizeValue)
            End If
            Dim x_enc As New ElGamalDecryptor(o_key_struct)
            Return x_enc.ProcessData(p_data)
        End Function

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)

        End Sub

        Public Overrides Function EncryptData(ByVal p_data() As Byte) As Byte()
            If NeedToGenerateKey() Then
                CreateKeyPair(KeySizeValue)
            End If
            Dim x_enc As New ElGamalEncryptor(o_key_struct)
            Return x_enc.ProcessData(p_data)
        End Function

        Public Overrides Function ExportParameters(ByVal p_include_private_params As Boolean) As ElGamalParameters
            If NeedToGenerateKey() Then
                CreateKeyPair(KeySizeValue)
            End If
            Dim x_params As New ElGamalParameters
            x_params.P = o_key_struct.P.getBytes
            x_params.G = o_key_struct.G.getBytes
            x_params.Y = o_key_struct.Y.getBytes
            If p_include_private_params Then
                x_params.X = o_key_struct.X.getBytes
            Else
                x_params.X = New Byte(1) {}
            End If
            Return x_params
        End Function

        Public Overrides Sub ImportParameters(ByVal p_parameters As ElGamalParameters)
            o_key_struct.P = New BigInteger(p_parameters.P)
            o_key_struct.G = New BigInteger(p_parameters.G)
            o_key_struct.Y = New BigInteger(p_parameters.Y)
            If p_parameters.X IsNot Nothing AndAlso p_parameters.X.Length > 0 Then
                o_key_struct.X = New BigInteger(p_parameters.X)
            End If
            KeySizeValue = o_key_struct.P.bitCount
        End Sub

        Public Overrides ReadOnly Property KeyExchangeAlgorithm() As String
            Get
                Return "ElGamal"
            End Get
        End Property

        Public Overrides Function Sign(ByVal p_hashcode() As Byte) As Byte()
            Throw New NotImplementedException
        End Function

        Public Overrides ReadOnly Property SignatureAlgorithm() As String
            Get
                Return "ElGamal"
            End Get
        End Property

        Public Overrides Function VerifySignature(ByVal p_hashcode() As Byte, ByVal p_signature() As Byte) As Boolean
            Throw New NotImplementedException
        End Function

        Private Sub CreateKeyPair(ByVal p_key_strength As Integer)
            Dim x_random_generator As New Random
            o_key_struct.P = BigInteger.genPseudoPrime(p_key_strength, 16, x_random_generator)
            o_key_struct.X = New BigInteger
            o_key_struct.X.genRandomBits(p_key_strength - 1, x_random_generator)
            o_key_struct.G = New BigInteger
            o_key_struct.G.genRandomBits(p_key_strength - 1, x_random_generator)
            o_key_struct.Y = o_key_struct.G.modPow(o_key_struct.X, o_key_struct.P)
        End Sub

        Private Function NeedToGenerateKey() As Boolean
            Return o_key_struct.P = 0 AndAlso o_key_struct.G = 0 AndAlso o_key_struct.Y = 0
        End Function

        Public Property KeyStruct() As ElGamalKeyStruct
            Get
                If NeedToGenerateKey() Then
                    CreateKeyPair(KeySizeValue)
                End If
                Return o_key_struct
            End Get
            Set(ByVal value As ElGamalKeyStruct)
                o_key_struct = value
            End Set
        End Property
    End Class
End Namespace
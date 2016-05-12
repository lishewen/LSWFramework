Namespace Security
    Public Class XTEAManagedEncryptor
        Inherits XTEAManagedAbstractTransform

        Public Sub New(ByVal p_key() As Byte)
            MyBase.New(p_key)
        End Sub

        Private Function XTEAEncrypt(ByVal p_data() As Byte, ByVal p_offset As Integer, ByVal p_count As Integer) As Byte()
            Dim x_sum As UInteger = 0, x_delta = &H9E3779B9, x_count As UInteger = 32
            Dim x_words = ConvertBytesToUints(p_data, p_offset, p_count)
            While x_count > 0
                x_words(0) += (x_words(1) << 4 Xor x_words(1) >> 5) + x_words(1) Xor x_sum + o_key(x_sum And 3)
                x_sum += x_delta
                x_words(1) += (x_words(0) << 4 Xor x_words(0) >> 5) + x_words(0) Xor x_sum + o_key(x_sum >> 11 And 3)
                x_count -= 1
            End While
            Return ConvertUintsToBytes(x_words)
        End Function

        Public Overrides Function TransformBlock(ByVal inputBuffer() As Byte, ByVal inputOffset As Integer, ByVal inputCount As Integer, ByVal outputBuffer() As Byte, ByVal outputOffset As Integer) As Integer
            Dim x_ciphertext = XTEAEncrypt(inputBuffer, inputOffset, inputCount)
            Array.Copy(x_ciphertext, 0, outputBuffer, outputOffset, x_ciphertext.Length)
            Return x_ciphertext.Length
        End Function

        Public Overrides Function TransformFinalBlock(ByVal inputBuffer() As Byte, ByVal inputOffset As Integer, ByVal inputCount As Integer) As Byte()
            Dim x_result(InputBlockSize) As Byte
            Array.Copy(inputBuffer, inputOffset, x_result, 0, inputCount)
            Dim x_shortfall = InputBlockSize - (inputCount - inputOffset)
            For i = inputCount - inputOffset To x_result.Length - 1
                x_result(i) = CByte(x_shortfall)
            Next
            Return XTEAEncrypt(x_result, 0, x_result.Length)
        End Function
    End Class
End Namespace
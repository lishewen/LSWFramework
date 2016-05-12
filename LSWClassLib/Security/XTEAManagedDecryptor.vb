Namespace Security
    Public Class XTEAManagedDecryptor
        Inherits XTEAManagedAbstractTransform

        Private o_data_block() As Byte

        Public Sub New(ByVal p_key() As Byte)
            MyBase.New(p_key)
            o_data_block = New Byte(0) {}
        End Sub

        Private Function XTEADecryptor(ByVal p_data() As Byte, ByVal p_offset As Integer, ByVal p_count As Integer) As Byte()
            Dim x_sum = &HC6EF3720, x_delta = &H9E3779B9, x_count As UInteger = 32
            Dim x_words = ConvertBytesToUints(p_data, p_offset, p_count)
            While x_count > 0
                x_words(1) -= (x_words(0) << 4 Xor x_words(0) >> 5) + x_words(0) Xor x_sum + o_key(x_sum >> 11 And 3)
                x_sum -= x_delta
                x_words(0) -= (x_words(1) << 4 Xor x_words(1) >> 5) + x_words(1) Xor x_sum + o_key(x_sum And 3)
                x_count -= 1
            End While
            Return ConvertUintsToBytes(x_words)
        End Function

        Public Overrides Function TransformBlock(ByVal inputBuffer() As Byte, ByVal inputOffset As Integer, ByVal inputCount As Integer, ByVal outputBuffer() As Byte, ByVal outputOffset As Integer) As Integer
            Dim x_result = 0
            If o_data_block.Length > 0 Then
                Array.Copy(o_data_block, 0, outputBuffer, outputOffset, o_data_block.Length)
                x_result = o_data_block.Length
            End If
            o_data_block = XTEADecryptor(inputBuffer, inputOffset, inputCount)
            Return x_result
        End Function

        Public Overrides Function TransformFinalBlock(ByVal inputBuffer() As Byte, ByVal inputOffset As Integer, ByVal inputCount As Integer) As Byte()
            Dim x_padding_count = CInt(o_data_block(o_data_block.Length - 1))
            Dim x_final_block(o_data_block.Length - x_padding_count) As Byte
            Array.Copy(o_data_block, 0, x_final_block, 0, x_final_block.Length)
            Return x_final_block
        End Function
    End Class
End Namespace
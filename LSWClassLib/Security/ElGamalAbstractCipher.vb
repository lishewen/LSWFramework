Imports System.IO

Namespace Security
    Public MustInherit Class ElGamalAbstractCipher
        Protected o_block_size As Integer
        Protected o_plaintext_blocksize As Integer
        Protected o_ciphertext_blocksize As Integer
        Protected o_key_struct As ElGamalKeyStruct

        Public Sub New(ByVal p_key_struct As ElGamalKeyStruct)
            o_key_struct = p_key_struct
            o_plaintext_blocksize = (p_key_struct.P.bitCount - 1) / 8
            o_ciphertext_blocksize = ((p_key_struct.P.bitCount + 7) / 8) * 2
            o_block_size = o_plaintext_blocksize
        End Sub

        Public Function ProcessData(ByVal p_data() As Byte) As Byte()
            Dim x_stream As New MemoryStream
            Dim x_complete_blocks As Integer = p_data.Length / o_block_size
            Dim x_block(o_block_size) As Byte
            Dim i = 0
            For i = 0 To x_complete_blocks - 1
                Array.Copy(p_data, i * o_block_size, x_block, 0, o_block_size)
                Dim x_result = ProcessDataBlock(x_block)
                x_stream.Write(x_result, 0, x_result.Length)
            Next
            Dim x_final_block(p_data.Length - (x_complete_blocks * o_block_size)) As Byte
            Array.Copy(p_data, i * o_block_size, x_final_block, 0, x_final_block.Length)
            Dim x_final_result = ProcessFinalDataBlock(x_final_block)
            x_stream.Write(x_final_result, 0, x_final_result.Length)
            Return x_stream.ToArray
        End Function

        Protected MustOverride Function ProcessDataBlock(ByVal p_block() As Byte) As Byte()

        Protected MustOverride Function ProcessFinalDataBlock(ByVal p_final_block() As Byte) As Byte()
    End Class
End Namespace
Namespace Security
    Public Class ElGamalDecryptor
        Inherits ElGamalAbstractCipher

        Public Sub New(ByVal p_struct As ElGamalKeyStruct)
            MyBase.New(p_struct)
            o_block_size = o_ciphertext_blocksize
        End Sub

        Protected Overrides Function ProcessDataBlock(ByVal p_block() As Byte) As Byte()
            Dim x_a_bytes(o_ciphertext_blocksize / 2) As Byte
            Array.Copy(p_block, 0, x_a_bytes, 0, x_a_bytes.Length)
            Dim x_b_bytes(o_ciphertext_blocksize / 2) As Byte
            Array.Copy(p_block, x_a_bytes.Length, x_b_bytes, 0, x_b_bytes.Length)
            Dim A As New BigInteger(x_a_bytes)
            Dim B As New BigInteger(x_b_bytes)
            Dim M = (B * A.modPow(o_key_struct.X, o_key_struct.P).modInverse(o_key_struct.P)) Mod o_key_struct.P
            Dim x_m_bytes = M.getBytes
            If x_m_bytes.Length < o_plaintext_blocksize Then
                Dim x_full_result(o_plaintext_blocksize) As Byte
                Array.Copy(x_m_bytes, 0, x_full_result, o_plaintext_blocksize - x_m_bytes.Length, x_m_bytes.Length)
                x_m_bytes = x_full_result
            End If
            Return x_m_bytes
        End Function

        Protected Overrides Function ProcessFinalDataBlock(ByVal p_final_block() As Byte) As Byte()
            If p_final_block.Length > 0 Then
                Return ProcessDataBlock(p_final_block)
            Else
                Return New Byte(0) {}
            End If
        End Function
    End Class
End Namespace
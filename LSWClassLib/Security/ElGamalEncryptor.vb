Namespace Security
    Public Class ElGamalEncryptor
        Inherits ElGamalAbstractCipher

        Dim o_randam As Random

        Public Sub New(ByVal p_struct As ElGamalKeyStruct)
            MyBase.New(p_struct)
            o_randam = New Random
        End Sub

        Protected Overrides Function ProcessDataBlock(ByVal p_block() As Byte) As Byte()
            Dim K As BigInteger
            Do
                K = New BigInteger
                K.genRandomBits(o_key_struct.P.bitCount - 1, o_randam)
            Loop While (K.gcd(o_key_struct.P - 1) <> 1)
            Dim A = o_key_struct.G.modPow(K, o_key_struct.P)
            Dim B = (o_key_struct.Y.modPow(K, o_key_struct.P) * New BigInteger(p_block)) Mod (o_key_struct.P)
            Dim x_result(o_ciphertext_blocksize) As Byte
            Dim x_a_bytes = A.getBytes
            Array.Copy(x_a_bytes, 0, x_result, o_ciphertext_blocksize \ 2 - x_a_bytes.Length, x_a_bytes.Length)
            Dim x_b_bytes = B.getBytes
            Array.Copy(x_b_bytes, 0, x_result, o_ciphertext_blocksize - x_b_bytes.Length, x_b_bytes.Length)
            Return x_result
        End Function

        Protected Overrides Function ProcessFinalDataBlock(ByVal p_final_block() As Byte) As Byte()
            If p_final_block.Length > 0 Then
                If p_final_block.Length < o_block_size Then
                    Dim x_padded(o_block_size) As Byte
                    Array.Copy(p_final_block, 0, x_padded, 0, p_final_block.Length)
                    Return ProcessDataBlock(x_padded)
                Else
                    Return ProcessDataBlock(p_final_block)
                End If
            Else
                Return New Byte(0) {}
            End If
        End Function
    End Class
End Namespace
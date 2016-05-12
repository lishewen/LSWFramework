Namespace Security
    Public Class Adler32Managed
        Inherits Adler32

        Private o_sum_1, o_sum_2 As Integer

        Public Sub New()
            Initialize()
        End Sub

        Public Overrides ReadOnly Property HashSize() As Integer
            Get
                Return 32
            End Get
        End Property

        Protected Overrides Sub HashCore(ByVal array() As Byte, ByVal ibStart As Integer, ByVal cbSize As Integer)
            For i = ibStart To cbSize - 1
                o_sum_1 = (o_sum_1 + array(i)) Mod 65521
                o_sum_2 = (o_sum_1 + o_sum_2) Mod 65521
            Next
        End Sub

        Protected Overrides Function HashFinal() As Byte()
            Dim x_concat_value = (o_sum_2 * 65536) Or o_sum_1
            Return BitConverter.GetBytes(x_concat_value)
        End Function

        Public Overrides Sub Initialize()
            o_sum_1 = 1
            o_sum_2 = 0
        End Sub
    End Class
End Namespace

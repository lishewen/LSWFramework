Namespace Image
    Friend Class Rational
        Private n As Integer
        Private d As Integer
        Public Sub New(ByVal n As Integer, ByVal d As Integer)
            Me.n = n
            Me.d = d
            simplify(Me.n, Me.d)
        End Sub
        Public Sub New(ByVal n As UInteger, ByVal d As UInteger)
            Me.n = Convert.ToInt32(n)
            Me.d = Convert.ToInt32(d)
            simplify(Me.n, Me.d)
        End Sub
        Public Sub New()
            Me.n = Me.d = 0
        End Sub
        Public Overloads Function ToString(ByVal sp As String) As String
            If sp Is Nothing Then
                sp = "/"
            End If
            Return n.ToString() + sp + d.ToString()
        End Function
        Public Function ToDouble() As Double
            If d = 0 Then
                Return 0
            End If
            Return System.Math.Round(Convert.ToDouble(n) / Convert.ToDouble(d), 2)
        End Function
        Private Sub simplify(ByRef a As Integer, ByRef b As Integer)
            If a = 0 OrElse b = 0 Then
                Return
            End If
            Dim gcd As Integer = euclid(a, b)
            a /= gcd
            b /= gcd
        End Sub
        Private Function euclid(ByVal a As Integer, ByVal b As Integer) As Integer
            If b = 0 Then
                Return a
            Else
                Return euclid(b, a Mod b)
            End If
        End Function
    End Class
End Namespace
Namespace Extension
    Public Class LiteralFormat
        Implements ITextExpression
        Public Sub New(ByVal literalText__1 As String)
            LiteralText = literalText__1
        End Sub

        Private _LiteralText As String
        Public Property LiteralText() As String
            Get
                Return _LiteralText
            End Get
            Private Set(ByVal value As String)
                _LiteralText = value
            End Set
        End Property

        Public Function Eval(ByVal o As Object) As String Implements ITextExpression.Eval
            Dim literalText__1 As String = LiteralText.Replace("{{", "{").Replace("}}", "}")
            Return literalText__1
        End Function
    End Class
End Namespace
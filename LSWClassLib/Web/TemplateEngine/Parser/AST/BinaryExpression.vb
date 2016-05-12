Namespace Web.TemplateEngine.Parser.AST
    Public Class BinaryExpression
        Inherits Expression
        Private m_lhs As Expression
        Private m_rhs As Expression

        Private op As TokenKind

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal lhs As Expression, ByVal op As TokenKind, ByVal rhs As Expression)
            MyBase.New(line, col)
            Me.m_lhs = lhs
            Me.m_rhs = rhs
            Me.op = op
        End Sub

        Public ReadOnly Property Lhs() As Expression
            Get
                Return Me.m_lhs
            End Get
        End Property

        Public ReadOnly Property Rhs() As Expression
            Get
                Return Me.m_rhs
            End Get
        End Property

        Public ReadOnly Property [Operator]() As TokenKind
            Get
                Return Me.op
            End Get
        End Property
    End Class
End Namespace
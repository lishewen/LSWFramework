Namespace Web.TemplateEngine.Parser.AST
    Public Class IntLiteral
        Inherits Expression
        Private m_value As Integer

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal value As Integer)
            MyBase.New(line, col)
            Me.m_value = value
        End Sub

        Public ReadOnly Property Value() As Integer
            Get
                Return Me.m_value
            End Get
        End Property
    End Class
End Namespace
Namespace Web.TemplateEngine.Parser.AST
    Public Class ArrayAccess
        Inherits Expression
        Private m_exp As Expression
        Private m_index As Expression

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal exp As Expression, ByVal index As Expression)
            MyBase.New(line, col)
            Me.m_exp = exp
            Me.m_index = index
        End Sub

        Public ReadOnly Property Exp() As Expression
            Get
                Return Me.m_exp
            End Get
        End Property

        Public ReadOnly Property Index() As Expression
            Get
                Return Me.m_index
            End Get
        End Property
    End Class
End Namespace
Namespace Web.TemplateEngine.Parser.AST
    Public Class TagIf
        Inherits Tag
        Private m_falseBranch As Tag
        Private m_test As Expression

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal test As Expression)
            MyBase.New(line, col, "if")
            Me.m_test = test
        End Sub

        Public Property FalseBranch() As Tag
            Get
                Return Me.m_falseBranch
            End Get
            Set(ByVal value As Tag)
                Me.m_falseBranch = value
            End Set
        End Property

        Public ReadOnly Property Test() As Expression
            Get
                Return m_test
            End Get
        End Property
    End Class
End Namespace
Namespace Web.TemplateEngine.Parser.AST
    Public Class TagAttribute
        Private m_name As String
        Private m_expression As Expression

        Public Sub New(ByVal name As String, ByVal expression As Expression)
            Me.m_name = name
            Me.m_expression = expression
        End Sub

        Public ReadOnly Property Expression() As Expression
            Get
                Return Me.m_expression
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Me.m_name
            End Get
        End Property
    End Class
End Namespace
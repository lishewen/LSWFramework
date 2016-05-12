Namespace Web.TemplateEngine.Parser.AST
    Public Class StringLiteral
        Inherits Expression
        Private m_content As String

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal content As String)
            MyBase.New(line, col)
            Me.m_content = content
        End Sub

        Public ReadOnly Property Content() As String
            Get
                Return Me.m_content
            End Get
        End Property
    End Class
End Namespace
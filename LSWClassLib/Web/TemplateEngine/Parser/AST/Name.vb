Namespace Web.TemplateEngine.Parser.AST
    Public Class Name
        Inherits Expression
        Private m_id As String

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal id As String)
            MyBase.New(line, col)
            Me.m_id = id
        End Sub

        Public ReadOnly Property Id() As String
            Get
                Return Me.m_id
            End Get
        End Property
    End Class
End Namespace
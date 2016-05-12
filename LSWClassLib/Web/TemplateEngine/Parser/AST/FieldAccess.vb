Namespace Web.TemplateEngine.Parser.AST
    Public Class FieldAccess
        Inherits Expression
        Private m_exp As Expression
        Private m_field As String

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal exp As Expression, ByVal field As String)
            MyBase.New(line, col)
            Me.m_exp = exp
            Me.m_field = field
        End Sub

        Public ReadOnly Property Exp() As Expression
            Get
                Return Me.m_exp
            End Get
        End Property

        Public ReadOnly Property Field() As String
            Get
                Return Me.m_field
            End Get
        End Property
    End Class
End Namespace
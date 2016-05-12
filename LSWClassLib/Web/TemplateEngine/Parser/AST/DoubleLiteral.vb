Namespace Web.TemplateEngine.Parser.AST
    Public Class DoubleLiteral
        Inherits Expression
        Private m_value As Double

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal value As Double)
            MyBase.New(line, col)
            Me.m_value = value
        End Sub

        Public ReadOnly Property Value() As Double
            Get
                Return Me.m_value
            End Get
        End Property
    End Class
End Namespace
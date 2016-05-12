Namespace Web.TemplateEngine.Parser.AST
    Public Class TagClose
        Inherits Element
        Private m_name As String

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal name As String)
            MyBase.New(line, col)
            Me.m_name = name
        End Sub

        Public ReadOnly Property Name() As String
            Get
                Return Me.m_name
            End Get
        End Property
    End Class
End Namespace

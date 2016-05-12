Namespace Web.TemplateEngine.Parser.AST
    Public Class Text
        Inherits Element
        Private m_data As String

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal data As String)
            MyBase.New(line, col)
            Me.m_data = data
        End Sub

        Public ReadOnly Property Data() As String
            Get
                Return Me.m_data
            End Get
        End Property
    End Class
End Namespace
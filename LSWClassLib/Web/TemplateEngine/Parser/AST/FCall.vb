Namespace Web.TemplateEngine.Parser.AST
    Public Class FCall
        Inherits Expression
        Private m_name As String
        Private m_args As Expression()

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal name As String, ByVal args As Expression())
            MyBase.New(line, col)
            Me.m_name = name
            Me.m_args = args
        End Sub

        Public ReadOnly Property Args() As Expression()
            Get
                Return Me.m_args
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Me.m_name
            End Get
        End Property
    End Class
End Namespace
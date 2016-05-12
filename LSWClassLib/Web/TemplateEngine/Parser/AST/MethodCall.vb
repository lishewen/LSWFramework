Namespace Web.TemplateEngine.Parser.AST
    Public Class MethodCall
        Inherits Expression
        Private m_name As String
        Private obj As Expression
        Private m_args As Expression()

        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal obj As Expression, ByVal name As String, ByVal args As Expression())
            MyBase.New(line, col)
            Me.m_name = name
            Me.m_args = args
            Me.obj = obj
        End Sub

        Public ReadOnly Property CallObject() As Expression
            Get
                Return Me.obj
            End Get
        End Property

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
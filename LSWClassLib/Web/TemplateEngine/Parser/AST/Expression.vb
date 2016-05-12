Namespace Web.TemplateEngine.Parser.AST
    Public MustInherit Class Expression
        Inherits Element
        Public Sub New(ByVal line As Integer, ByVal col As Integer)
            MyBase.New(line, col)
        End Sub
    End Class
End Namespace

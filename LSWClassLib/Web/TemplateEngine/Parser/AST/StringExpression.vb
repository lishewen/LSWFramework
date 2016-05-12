Namespace Web.TemplateEngine.Parser.AST
    Public Class StringExpression
        Inherits Expression
        Private exps As List(Of Expression)

        Public Sub New(ByVal line As Integer, ByVal col As Integer)
            MyBase.New(line, col)
            exps = New List(Of Expression)()
        End Sub

        Public ReadOnly Property ExpCount() As Integer
            Get
                Return exps.Count
            End Get
        End Property

        Public Sub Add(ByVal exp As Expression)
            exps.Add(exp)
        End Sub

        Default Public ReadOnly Property Item(ByVal index As Integer) As Expression
            Get
                Return exps(index)
            End Get
        End Property

        Public ReadOnly Property Expressions() As IEnumerable(Of Expression)
            Get
                Dim result As New List(Of Expression)
                For i As Integer = 0 To exps.Count - 1
                    result.Add(exps(i))
                Next
                Return result
            End Get
        End Property
    End Class
End Namespace
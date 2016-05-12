Imports System.Web.UI
Imports System.Web

Namespace Extension
    Public Class FormatExpression
        Implements ITextExpression
        Private _invalidExpression As Boolean = False

        Public Sub New(ByVal expression__1 As String)
            If Not expression__1.StartsWith("{") OrElse Not expression__1.EndsWith("}") Then
                _invalidExpression = True
                Expression = expression__1
                Return
            End If

            Dim expressionWithoutBraces As String = expression__1.Substring(1, expression__1.Length - 2)
            Dim colonIndex As Integer = expressionWithoutBraces.IndexOf(":"c)
            If colonIndex < 0 Then
                Expression = expressionWithoutBraces
            Else
                Expression = expressionWithoutBraces.Substring(0, colonIndex)
                Format = expressionWithoutBraces.Substring(colonIndex + 1)
            End If
        End Sub

        Private _Expression As String
        Public Property Expression() As String
            Get
                Return _Expression
            End Get
            Private Set(ByVal value As String)
                _Expression = value
            End Set
        End Property

        Private _Format As String
        Public Property Format() As String
            Get
                Return _Format
            End Get
            Private Set(ByVal value As String)
                _Format = value
            End Set
        End Property

        Public Function Eval(ByVal o As Object) As String Implements ITextExpression.Eval
            If _invalidExpression Then
                Throw New FormatException("Invalid expression")
            End If
            Try
                If String.IsNullOrEmpty(Format) Then
                    Return (If(DataBinder.Eval(o, Expression), String.Empty)).ToString()
                End If
                Return (If(DataBinder.Eval(o, Expression, "{0:" & Format & "}"), String.Empty)).ToString()
            Catch generatedExceptionName As ArgumentException
                Throw New FormatException()
            Catch generatedExceptionName As HttpException
                Throw New FormatException()
            End Try
        End Function
    End Class
End Namespace
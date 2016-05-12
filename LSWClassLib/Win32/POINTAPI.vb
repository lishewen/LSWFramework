Imports System.Drawing

Namespace Win32
    Public Structure POINTAPI
        Public Property X As Integer
        Public Property Y As Integer

        Public Shared Widening Operator CType(p As POINTAPI) As Point
            Return New Point(p.X, p.Y)
        End Operator
    End Structure
End Namespace
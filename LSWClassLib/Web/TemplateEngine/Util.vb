Imports System.Text.RegularExpressions

Namespace Web.TemplateEngine
    Public Module Util
        Sub New()
        End Sub
        Private syncObject As New Object()

        Private m_regExVarName As Regex

        Public Function ToBool(ByVal obj As Object) As Boolean
            If TypeOf obj Is Boolean Then
                Return CBool(obj)
            ElseIf TypeOf obj Is String Then
                Dim str As String = DirectCast(obj, String)
                If String.Compare(str, "true", True) = 0 Then
                    Return True
                ElseIf String.Compare(str, "yes", True) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End Function

        Public Function IsValidVariableName(ByVal name As String) As Boolean
            Return RegExVarName.IsMatch(name)
        End Function

        Private ReadOnly Property RegExVarName() As Regex
            Get
                If (m_regExVarName Is Nothing) Then
                    System.Threading.Monitor.Enter(syncObject)
                    If m_regExVarName Is Nothing Then
                        Try
                            m_regExVarName = New Regex("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled)
                        Finally
                            System.Threading.Monitor.[Exit](syncObject)
                        End Try
                    End If
                End If

                Return m_regExVarName
            End Get
        End Property
    End Module
End Namespace
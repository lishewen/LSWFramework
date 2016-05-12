Namespace Web.TemplateEngine
    Public Class VariableScope
        Private m_parent As VariableScope
        Private values As Dictionary(Of String, Object)

        Public Sub New()
            Me.New(Nothing)
        End Sub

        Public Sub New(ByVal parent As VariableScope)
            Me.m_parent = parent
            Me.values = New Dictionary(Of String, Object)(StringComparer.InvariantCultureIgnoreCase)
        End Sub

        ''' <summary>
        ''' clear all variables from this scope
        ''' </summary>
        Public Sub Clear()
            values.Clear()
        End Sub

        ''' <summary>
        ''' gets the parent scope for this scope
        ''' </summary>
        Public ReadOnly Property Parent() As VariableScope
            Get
                Return m_parent
            End Get
        End Property

        ''' <summary>
        ''' returns true if variable name is defined
        ''' otherwise returns parents isDefined if parent != null
        ''' </summary>
        Public Function IsDefined(ByVal name As String) As Boolean
            If values.ContainsKey(name) Then
                Return True
            ElseIf m_parent IsNot Nothing Then
                Return m_parent.IsDefined(name)
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' returns value of variable name
        ''' If name is not in this scope and parent != null
        ''' parents this[name] is called
        ''' </summary>
        Default Public Property Item(ByVal name As String) As Object
            Get
                If Not values.ContainsKey(name) Then
                    If m_parent IsNot Nothing Then
                        Return m_parent(name)
                    Else
                        Return Nothing
                    End If
                Else
                    Return values(name)
                End If
            End Get
            Set(ByVal value As Object)
                values(name) = value
            End Set
        End Property
    End Class
End Namespace

Namespace Web.TemplateEngine
    Class StaticTypeReference
        ReadOnly m_type As Type

        Public Sub New(ByVal type As Type)
            Me.m_type = type
        End Sub

        Public ReadOnly Property Type() As Type
            Get
                Return m_type
            End Get
        End Property
    End Class
End Namespace
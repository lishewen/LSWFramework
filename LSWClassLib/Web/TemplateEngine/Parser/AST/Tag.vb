Namespace Web.TemplateEngine.Parser.AST
    Public Class Tag
        Inherits Element
        Private m_name As String
        Private attribs As List(Of TagAttribute)
        Private m_innerElements As List(Of Element)
        Private m_closeTag As TagClose
        Private m_isClosed As Boolean
        ' set to true if tag ends with />
        Public Sub New(ByVal line As Integer, ByVal col As Integer, ByVal name As String)
            MyBase.New(line, col)
            Me.m_name = name
            Me.attribs = New List(Of TagAttribute)()
            Me.m_innerElements = New List(Of Element)()
        End Sub

        Public ReadOnly Property Attributes() As List(Of TagAttribute)
            Get
                Return Me.attribs
            End Get
        End Property

        Public Function AttributeValue(ByVal name As String) As Expression
            For Each attrib As TagAttribute In attribs
                If String.Compare(attrib.Name, name, True) = 0 Then
                    Return attrib.Expression
                End If
            Next

            Return Nothing
        End Function

        Public ReadOnly Property InnerElements() As List(Of Element)
            Get
                Return Me.m_innerElements
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return Me.m_name
            End Get
        End Property

        Public Property CloseTag() As TagClose
            Get
                Return Me.m_closeTag
            End Get
            Set(ByVal value As TagClose)
                Me.m_closeTag = value
            End Set
        End Property

        Public Property IsClosed() As Boolean
            Get
                Return Me.m_isClosed
            End Get
            Set(ByVal value As Boolean)
                Me.m_isClosed = value
            End Set
        End Property
    End Class
End Namespace

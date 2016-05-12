Imports LSW.Web.TemplateEngine.Parser.AST
Imports LSW.Web.TemplateEngine.Parser

Namespace Web.TemplateEngine
    Public Class Template
        Private m_name As String
        Private m_elements As List(Of Element)
        Private m_parent As Template

        Private m_templates As Dictionary(Of String, Template)

        Public Sub New(ByVal name As String, ByVal elements As List(Of Element))
            Me.m_name = name
            Me.m_elements = elements
            Me.m_parent = Nothing

            InitTemplates()
        End Sub

        Public Sub New(ByVal name As String, ByVal elements As List(Of Element), ByVal parent As Template)
            Me.m_name = name
            Me.m_elements = elements
            Me.m_parent = parent

            InitTemplates()
        End Sub

        ''' <summary>
        ''' load template from file
        ''' </summary>
        ''' <param name="name">name of template</param>
        ''' <param name="filename">file from which to load template</param>
        ''' <returns></returns>
        Public Shared Function FromFile(ByVal name As String, ByVal filename As String) As Template
            Using reader As New System.IO.StreamReader(filename)
                Dim data As String = reader.ReadToEnd()
                Return Template.FromString(name, data)
            End Using
        End Function

        ''' <summary>
        ''' load template from string
        ''' </summary>
        ''' <param name="name">name of template</param>
        ''' <param name="data">string containg code for template</param>
        ''' <returns></returns>
        Public Shared Function FromString(ByVal name As String, ByVal data As String) As Template
            Dim lexer As New TemplateLexer(data)
            Dim parser As New TemplateParser(lexer)
            Dim elems As List(Of Element) = parser.Parse()

            Dim tagParser As New TagParser(elems)
            elems = tagParser.CreateHierarchy()

            Return New Template(name, elems)
        End Function

        ''' <summary>
        ''' go thru all tags and see if they are template tags and add
        ''' them to this.templates collection
        ''' </summary>
        Private Sub InitTemplates()
            Me.m_templates = New Dictionary(Of String, Template)(StringComparer.InvariantCultureIgnoreCase)

            For Each elem As Element In m_elements
                If TypeOf elem Is Tag Then
                    Dim tag As Tag = DirectCast(elem, Tag)
                    If String.Compare(tag.Name, "template", True) = 0 Then
                        Dim ename As Expression = tag.AttributeValue("name")
                        Dim tname As String
                        If TypeOf ename Is StringLiteral Then
                            tname = DirectCast(ename, StringLiteral).Content
                        Else
                            tname = "?"
                        End If

                        Dim template As New Template(tname, tag.InnerElements, Me)
                        m_templates(tname) = template
                    End If
                End If
            Next
        End Sub

        ''' <summary>
        ''' gets a list of elements for this template
        ''' </summary>
        Public ReadOnly Property Elements() As List(Of Element)
            Get
                Return Me.m_elements
            End Get
        End Property

        ''' <summary>
        ''' gets the name of this template
        ''' </summary>
        Public ReadOnly Property Name() As String
            Get
                Return Me.m_name
            End Get
        End Property

        ''' <summary>
        ''' returns true if this template has parent template
        ''' </summary>
        Public ReadOnly Property HasParent() As Boolean
            Get
                Return m_parent IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' gets parent template of this template
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property Parent() As Template
            Get
                Return Me.m_parent
            End Get
        End Property

        ''' <summary>
        ''' finds template matching name. If this template does not
        ''' contain template called name, and parent != null then
        ''' FindTemplate is called on the parent
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Overridable Function FindTemplate(ByVal name As String) As Template
            If m_templates.ContainsKey(name) Then
                Return m_templates(name)
            ElseIf m_parent IsNot Nothing Then
                Return m_parent.FindTemplate(name)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' gets dictionary of templates defined in this template
        ''' </summary>
        Public ReadOnly Property Templates() As System.Collections.Generic.Dictionary(Of String, Template)
            Get
                Return Me.m_templates
            End Get
        End Property
    End Class
End Namespace

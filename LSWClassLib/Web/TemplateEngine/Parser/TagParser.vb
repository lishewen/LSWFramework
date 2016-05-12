Imports LSW.Web.TemplateEngine.Parser.AST

Namespace Web.TemplateEngine.Parser
    Public Class TagParser
        Private elements As List(Of Element)

        Public Sub New(ByVal elements As List(Of Element))
            Me.elements = elements
        End Sub

        Public Function CreateHierarchy() As List(Of Element)
            Dim result As New List(Of Element)()

            For index As Integer = 0 To elements.Count - 1
                Dim elem As Element = elements(index)

                If TypeOf elem Is AST.Text Then
                    result.Add(elem)
                ElseIf TypeOf elem Is Expression Then
                    result.Add(elem)
                ElseIf TypeOf elem Is Tag Then
                    result.Add(CollectForTag(DirectCast(elem, Tag), index))
                ElseIf TypeOf elem Is TagClose Then
                    Throw New ParseException("Close tag for " & DirectCast(elem, TagClose).Name & " doesn't have matching start tag.", elem.Line, elem.Col)
                Else
                    Throw New ParseException("Invalid element: " & elem.[GetType]().ToString(), elem.Line, elem.Col)
                End If
            Next

            Return result
        End Function

        Private Function CollectForTag(ByVal tag As Tag, ByRef index As Integer) As Tag
            If tag.IsClosed Then
                ' if self-closing tag, do not collect inner elements
                Return tag
            End If

            If String.Compare(tag.Name, "if", True) = 0 Then
                tag = New TagIf(tag.Line, tag.Col, tag.AttributeValue("test"))
            End If

            Dim collectTag As Tag = tag

            index += 1
            While index < elements.Count
                Dim elem As Element = elements(index)

                If TypeOf elem Is AST.Text Then
                    collectTag.InnerElements.Add(elem)
                ElseIf TypeOf elem Is Expression Then
                    collectTag.InnerElements.Add(elem)
                ElseIf TypeOf elem Is Tag Then
                    Dim innerTag As Tag = DirectCast(elem, Tag)
                    If String.Compare(innerTag.Name, "else", True) = 0 Then
                        If TypeOf collectTag Is TagIf Then
                            DirectCast(collectTag, TagIf).FalseBranch = innerTag
                            collectTag = innerTag
                        Else
                            Throw New ParseException("else tag has to be positioned inside of if or elseif tag", innerTag.Line, innerTag.Col)

                        End If
                    ElseIf String.Compare(innerTag.Name, "elseif", True) = 0 Then
                        If TypeOf collectTag Is TagIf Then
                            Dim newTag As Tag = New TagIf(innerTag.Line, innerTag.Col, innerTag.AttributeValue("test"))
                            DirectCast(collectTag, TagIf).FalseBranch = newTag
                            collectTag = newTag
                        Else
                            Throw New ParseException("elseif tag is not positioned properly", innerTag.Line, innerTag.Col)
                        End If
                    Else
                        collectTag.InnerElements.Add(CollectForTag(innerTag, index))
                    End If
                ElseIf TypeOf elem Is TagClose Then
                    Dim tagClose As TagClose = DirectCast(elem, TagClose)
                    If String.Compare(tag.Name, tagClose.Name, True) = 0 Then
                        Return tag
                    End If

                    Throw New ParseException("Close tag for " & tagClose.Name & " doesn't have matching start tag.", elem.Line, elem.Col)
                Else
                    Throw New ParseException("Invalid element: " & elem.[GetType]().ToString(), elem.Line, elem.Col)

                End If
                index += 1
            End While


            Throw New ParseException("Start tag: " & tag.Name & " does not have matching end tag.", tag.Line, tag.Col)
        End Function
    End Class
End Namespace
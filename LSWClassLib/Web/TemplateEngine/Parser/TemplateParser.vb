Imports LSW.Web.TemplateEngine.Parser.AST

Namespace Web.TemplateEngine.Parser
    Public Class TemplateParser
        Private lexer As TemplateLexer
        Private m_current As Token
        Private elements As List(Of Element)

        Public Sub New(ByVal lexer As TemplateLexer)
            Me.lexer = lexer
            Me.elements = New List(Of Element)()
        End Sub

        Private Function Consume() As Token
            Dim old As Token = m_current
            m_current = lexer.[Next]()
            Return old
        End Function

        Private Function Consume(ByVal kind As TokenKind) As Token
            Dim old As Token = m_current
            m_current = lexer.[Next]()

            If old.TokenKind <> kind Then
                Throw New ParseException(("Unexpected token: " & m_current.TokenKind.ToString() & ". Was expecting: ") + kind, m_current.Line, m_current.Col)
            End If

            Return old
        End Function

        Private ReadOnly Property Current() As Token
            Get
                Return m_current
            End Get
        End Property

        Public Function Parse() As List(Of Element)
            elements.Clear()

            Consume()

            While True
                Dim elem As Element = ReadElement()
                If elem Is Nothing Then
                    Exit While
                Else
                    elements.Add(elem)
                End If
            End While
            Return elements
        End Function

        Private Function ReadElement() As Element
            Select Case Current.TokenKind
                Case TokenKind.EOF
                    Return Nothing
                Case TokenKind.TagStart
                    Return ReadTag()
                Case TokenKind.TagClose
                    Return ReadCloseTag()
                Case TokenKind.ExpStart
                    Return ReadExpression()
                Case TokenKind.TextData
                    Dim text As New AST.Text(Current.Line, Current.Col, Current.Data)
                    Consume()
                    Return text
                Case Else
                    Throw New ParseException("Invalid token: " & Current.TokenKind.ToString(), Current.Line, Current.Col)
            End Select
        End Function

        Private Function ReadCloseTag() As TagClose
            Consume(TokenKind.TagClose)
            Dim idToken As Token = Consume(TokenKind.ID)
            Consume(TokenKind.TagEnd)

            Return New TagClose(idToken.Line, idToken.Col, idToken.Data)
        End Function

        Private Function ReadExpression() As Expression
            Consume(TokenKind.ExpStart)

            Dim exp As Expression = TopExpression()

            Consume(TokenKind.ExpEnd)

            Return exp
        End Function

        Private Function ReadTag() As Tag
            Consume(TokenKind.TagStart)
            Dim name As Token = Consume(TokenKind.ID)
            Dim tag As New Tag(name.Line, name.Col, name.Data)

            While True
                If Current.TokenKind = TokenKind.ID Then
                    tag.Attributes.Add(ReadAttribute())
                ElseIf Current.TokenKind = TokenKind.TagEnd Then
                    Consume()
                    Exit While
                ElseIf Current.TokenKind = TokenKind.TagEndClose Then
                    Consume()
                    tag.IsClosed = True
                    Exit While
                Else
                    Throw New ParseException("Invalid token in tag: " & Current.TokenKind, Current.Line, Current.Col)

                End If
            End While


            Return tag
        End Function

        Private Function ReadAttribute() As TagAttribute
            Dim name As Token = Consume(TokenKind.ID)
            Consume(TokenKind.TagEquals)

            Dim exp As Expression = Nothing

            If Current.TokenKind = TokenKind.StringStart Then
                exp = ReadString()
            Else
                Throw New ParseException("Unexpected token: " & Current.TokenKind & ". Was expection '""'", Current.Line, Current.Col)
            End If

            Return New TagAttribute(name.Data, exp)
        End Function

        Private Function ReadString() As Expression
            Dim start As Token = Consume(TokenKind.StringStart)
            Dim exp As New StringExpression(start.Line, start.Col)

            While True
                Dim tok As Token = Current

                If tok.TokenKind = TokenKind.StringEnd Then
                    Consume()
                    Exit While
                ElseIf tok.TokenKind = TokenKind.EOF Then
                    Throw New ParseException("Unexpected end of file", tok.Line, tok.Col)
                ElseIf tok.TokenKind = TokenKind.StringText Then
                    Consume()
                    exp.Add(New StringLiteral(tok.Line, tok.Col, tok.Data))
                ElseIf tok.TokenKind = TokenKind.ExpStart Then
                    exp.Add(ReadExpression())
                Else
                    Throw New ParseException("Unexpected token in string: " & tok.TokenKind, tok.Line, tok.Col)
                End If
            End While

            If exp.ExpCount = 1 Then
                Return exp(0)
            Else
                Return exp
            End If
        End Function

        Private Function TopExpression() As Expression
            Return OrExpression()
        End Function

        Private Function OrExpression() As Expression
            Dim ret As Expression = AndExpression()

            While Current.TokenKind = TokenKind.OpOr
                Consume()
                ' Or
                Dim rhs As Expression = AndExpression()
                ret = New BinaryExpression(ret.Line, ret.Col, ret, TokenKind.OpOr, rhs)
            End While

            Return ret
        End Function

        Private Function AndExpression() As Expression
            Dim ret As Expression = EqualityExpression()

            While Current.TokenKind = TokenKind.OpAnd
                Consume()
                ' Or
                Dim rhs As Expression = EqualityExpression()
                ret = New BinaryExpression(ret.Line, ret.Col, ret, TokenKind.OpAnd, rhs)
            End While

            Return ret
        End Function

        Private Function EqualityExpression() As Expression
            Dim ret As Expression = RelationalExpression()
            While Current.TokenKind = TokenKind.OpIs OrElse Current.TokenKind = TokenKind.OpIsNot
                Dim tok As Token = Consume()
                ' consume operator
                Dim rhs As Expression = RelationalExpression()

                ret = New BinaryExpression(ret.Line, ret.Col, ret, tok.TokenKind, rhs)
            End While

            Return ret
        End Function

        Private Function RelationalExpression() As Expression
            Dim ret As Expression = PrimaryExpression()

            While Current.TokenKind = TokenKind.OpLt OrElse Current.TokenKind = TokenKind.OpLte OrElse Current.TokenKind = TokenKind.OpGt OrElse Current.TokenKind = TokenKind.OpGte
                Dim tok As Token = Consume()
                ' consume operator
                Dim rhs As Expression = PrimaryExpression()
                ret = New BinaryExpression(ret.Line, ret.Col, ret, tok.TokenKind, rhs)
            End While

            Return ret
        End Function

        Private Function PrimaryExpression() As Expression
            If Current.TokenKind = TokenKind.StringStart Then
                Return ReadString()
            ElseIf Current.TokenKind = TokenKind.ID Then
                Dim id As Token = Consume()

                Dim exp As Expression = Nothing

                ' if ( follows ID, we have a function call
                If Current.TokenKind = TokenKind.LParen Then
                    Consume()
                    ' consume LParen
                    Dim args As Expression() = ReadArguments()
                    Consume(TokenKind.RParen)

                    exp = New FCall(id.Line, id.Col, id.Data, args)
                Else
                    ' else, we just have id
                    exp = New Name(id.Line, id.Col, id.Data)
                End If

                ' while we have ".", keep chaining up field access or method call
                While Current.TokenKind = TokenKind.Dot OrElse Current.TokenKind = TokenKind.LBracket
                    If Current.TokenKind = TokenKind.Dot Then
                        Consume()
                        ' consume DOT
                        Dim field As Token = Consume(TokenKind.ID)
                        ' consume ID after dot
                        ' if "(" after ID, then it's a method call
                        If Current.TokenKind = TokenKind.LParen Then
                            Consume()
                            ' consume "("
                            Dim args As Expression() = ReadArguments()
                            Consume(TokenKind.RParen)
                            ' read ")"
                            exp = New MethodCall(field.Line, field.Col, exp, field.Data, args)
                        Else
                            exp = New FieldAccess(field.Line, field.Col, exp, field.Data)
                        End If
                    Else
                        ' must be LBracket
                        ' array access
                        Dim bracket As Token = Current
                        Consume()
                        ' consume [
                        Dim indexExp As Expression = TopExpression()
                        Consume(TokenKind.RBracket)

                        exp = New ArrayAccess(bracket.Line, bracket.Col, exp, indexExp)

                    End If
                End While


                Return exp
            ElseIf Current.TokenKind = TokenKind.[Integer] Then
                Dim value As Integer = Int32.Parse(Current.Data)
                Dim intLiteral As New IntLiteral(Current.Line, Current.Col, value)
                Consume()
                ' consume int
                Return intLiteral
            ElseIf Current.TokenKind = TokenKind.[Double] Then
                Dim value As Double = [Double].Parse(Current.Data)
                Dim dLiteral As New DoubleLiteral(Current.Line, Current.Col, value)
                Consume()
                ' consume int
                Return dLiteral
            ElseIf Current.TokenKind = TokenKind.LParen Then
                Consume()
                ' eat (
                Dim exp As Expression = TopExpression()
                Consume(TokenKind.RParen)
                ' eat )
                Return exp
            Else
                Throw New ParseException("Invalid token in expression: " & Current.TokenKind & ". Was expecting ID or string.", Current.Line, Current.Col)

            End If
        End Function

        Private Function ReadArguments() As Expression()
            Dim exps As New List(Of Expression)()

            Dim index As Integer = 0
            While True
                If Current.TokenKind = TokenKind.RParen Then
                    Exit While
                End If

                If index > 0 Then
                    Consume(TokenKind.Comma)
                End If

                exps.Add(TopExpression())

                index += 1
            End While

            Return exps.ToArray()
        End Function
    End Class
End Namespace
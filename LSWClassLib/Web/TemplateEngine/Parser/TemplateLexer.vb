Imports System.IO

Namespace Web.TemplateEngine.Parser
	Public Class TemplateLexer
		Shared keywords As Dictionary(Of String, TokenKind)

		Shared Sub New()
			keywords = New Dictionary(Of String, TokenKind)(StringComparer.InvariantCultureIgnoreCase)
			keywords("or") = TokenKind.OpOr
			keywords("and") = TokenKind.OpAnd
			keywords("is") = TokenKind.OpIs
			keywords("isnot") = TokenKind.OpIsNot
			keywords("lt") = TokenKind.OpLt
			keywords("gt") = TokenKind.OpGt
			keywords("lte") = TokenKind.OpLte
			keywords("gte") = TokenKind.OpGte
		End Sub

		Private Enum LexMode
			Text
			Tag
			Expression
			[String]
		End Enum

		Const EOF As Char = ChrW(0)

		Private currentMode As LexMode
		Private modes As Stack(Of LexMode)

		Private line As Integer
		Private column As Integer
		Private pos As Integer
		' position within data
		Private data As String

		Private saveLine As Integer
		Private saveCol As Integer
		Private savePos As Integer

		Public Sub New(ByVal reader As TextReader)
			If reader Is Nothing Then
				Throw New ArgumentNullException(NameOf(reader))
			End If

			data = reader.ReadToEnd()

			Reset()
		End Sub

		Public Sub New(ByVal data As String)
			If data Is Nothing Then
				Throw New ArgumentNullException(NameOf(data))
			End If

			Me.data = data

			Reset()
		End Sub

		Private Sub EnterMode(ByVal mode As LexMode)
			modes.Push(currentMode)
			currentMode = mode
		End Sub

		Private Sub LeaveMode()
			currentMode = modes.Pop()
		End Sub

		Private Sub Reset()
			modes = New Stack(Of LexMode)()
			currentMode = LexMode.Text
			modes.Push(currentMode)

			line = 1
			column = 1
			pos = 0
		End Sub

		Protected Function LA(ByVal count As Integer) As Char
			If pos + count >= data.Length Then
				Return EOF
			Else
				Return data(pos + count)
			End If
		End Function

		Protected Function Consume() As Char
			Dim ret As Char = data(pos)
			pos += 1
			column += 1

			Return ret
		End Function

		Protected Function Consume(ByVal count As Integer) As Char
			If count <= 0 Then
				Throw New ArgumentOutOfRangeException(NameOf(count), "count has to be greater than 0")
			End If

			Dim ret As Char = " "c
			While count > 0
				ret = Consume()
				count -= 1
			End While
			Return ret
		End Function

		Private Sub NewLine()
			line += 1
			column = 1
		End Sub

		Protected Function CreateToken(ByVal kind As TokenKind, ByVal value As String) As Token
			Return New Token(kind, value, line, column)
		End Function

		Protected Function CreateToken(ByVal kind As TokenKind) As Token
			Dim tokenData As String = data.Substring(savePos, pos - savePos)
			If kind = TokenKind.StringText Then
				tokenData = tokenData.Replace("""""", """")
			End If
			' replace double "" with single "
			If kind = TokenKind.StringText OrElse kind = TokenKind.TextData Then
				tokenData = tokenData.Replace("##", "#")
			End If
			' replace ## with #
			Return New Token(kind, tokenData, saveLine, saveCol)
		End Function

		''' <summary>
		''' reads all whitespace characters (does not include newline)
		''' </summary>
		Protected Sub ReadWhitespace()
			While True
				Dim ch As Char = LA(0)
				Select Case ch
					Case " "c, ControlChars.Tab
						Consume()
						Exit Select
					Case ControlChars.Lf
						Consume()
						NewLine()
						Exit Select

					Case ControlChars.Cr
						Consume()
						If LA(0) = ControlChars.Lf Then
							Consume()
						End If
						NewLine()
						Exit Select
					Case Else
						Exit Sub
				End Select
			End While
		End Sub

		''' <summary>
		''' save read point positions so that CreateToken can use those
		''' </summary>
		Private Sub StartRead()
			saveLine = line
			saveCol = column
			savePos = pos
		End Sub

		Public Function [Next]() As Token
			Select Case currentMode
				Case LexMode.Text
					Return NextText()
				Case LexMode.Expression
					Return NextExpression()
				Case LexMode.Tag
					Return NextTag()
				Case LexMode.[String]
					Return NextString()
				Case Else
					Throw New ParseException("Encountered invalid lexer mode: " & currentMode.ToString(), line, column)
			End Select
		End Function

		Private Function NextExpression() As Token
			StartRead()
			Dim ch As Char = LA(0)
			Select Case ch
				Case EOF
					Return CreateToken(TokenKind.EOF)
				Case ","c
					Consume()
					Return CreateToken(TokenKind.Comma)
				Case "."c
					Consume()
					Return CreateToken(TokenKind.Dot)
				Case "("c
					Consume()
					Return CreateToken(TokenKind.LParen)
				Case ")"c
					Consume()
					Return CreateToken(TokenKind.RParen)
				Case "#"c
					Consume()
					LeaveMode()
					Return CreateToken(TokenKind.ExpEnd)
				Case "["c
					Consume()
					Return CreateToken(TokenKind.LBracket)
				Case "]"c
					Consume()
					Return CreateToken(TokenKind.RBracket)
				Case " "c, ControlChars.Tab, ControlChars.Cr, ControlChars.Lf
					ReadWhitespace()
					Return NextExpression()

				Case """"c
					Consume()
					EnterMode(LexMode.[String])
					Return CreateToken(TokenKind.StringStart)

				Case "0"c, "1"c, "2"c, "3"c, "4"c, "5"c,
				"6"c, "7"c, "8"c, "9"c
					Return ReadNumber()

				Case "-"c
					If True Then
						If [Char].IsDigit(LA(1)) Then
							Return ReadNumber()
						End If

						GoTo default1
					End If
				Case Else
default1:
					If [Char].IsLetter(ch) OrElse ch = "_"c Then
						Return ReadId()
					Else
						Throw New ParseException("Invalid character in expression: " & ch, line, column)
					End If
			End Select
			Return Nothing
		End Function

		Private Function NextTag() As Token
			StartRead()
StartTagRead:
			Dim ch As Char = LA(0)
			Select Case ch
				Case EOF
					Return CreateToken(TokenKind.EOF)
				Case "="c
					Consume()
					Return CreateToken(TokenKind.TagEquals)
				Case """"c
					Consume()
					EnterMode(LexMode.[String])
					Return CreateToken(TokenKind.StringStart)
				Case " "c, ControlChars.Tab, ControlChars.Cr, ControlChars.Lf
					ReadWhitespace()
					' ignore whitespace
					StartRead()
					' remark current position
					GoTo StartTagRead
					' start again
				Case ">"c
					Consume()
					LeaveMode()
					Return CreateToken(TokenKind.TagEnd)
				Case "/"c
					If LA(1) = ">"c Then
						Consume(2)
						' consume />
						LeaveMode()
						Return CreateToken(TokenKind.TagEndClose)
					End If
					Exit Select
				Case Else
					If [Char].IsLetter(ch) OrElse ch = "_"c Then
						Return ReadId()
					End If
					Exit Select

			End Select
			Throw New ParseException("Invalid character in tag: " & ch, line, column)
		End Function

		Private Function NextString() As Token
			StartRead()
StartStringRead:
			Dim ch As Char = LA(0)
			Select Case ch
				Case EOF
					Return CreateToken(TokenKind.EOF)

				Case "#"c
					If LA(1) = "#"c Then
						' just escape
						Consume(2)
						GoTo StartStringRead
					ElseIf savePos = pos Then
						Consume()
						EnterMode(LexMode.Expression)
						Return CreateToken(TokenKind.ExpStart)
					Else
						Exit Select
					End If
					' just break and we will return the text token
				Case ControlChars.Cr, ControlChars.Lf
					ReadWhitespace()
					GoTo StartStringRead
				Case """"c
					If LA(1) = """"c Then
						' just escape
						Consume(2)
						GoTo StartStringRead
					ElseIf pos = savePos Then
						Consume()
						LeaveMode()
						Return CreateToken(TokenKind.StringEnd)
					Else
						Exit Select
					End If
				Case Else
					' just break so that text is returned
					Consume()
					GoTo StartStringRead

			End Select

			Return CreateToken(TokenKind.StringText)
		End Function

		Private Function NextText() As Token
			StartRead()
StartTextRead:

			Select Case LA(0)
				Case EOF
					If savePos = pos Then
						Return CreateToken(TokenKind.EOF)
					Else
						Exit Select
					End If

				Case "#"c
					If LA(1) = "#"c Then
						' # was just escape
						Consume(2)
						' consume both #
						GoTo StartTextRead
					ElseIf savePos = pos Then
						Consume()
						EnterMode(LexMode.Expression)
						Return CreateToken(TokenKind.ExpStart)
					Else
						Exit Select
					End If
					' even if we have exp, we break because we read some characters that need to be returned as text
				Case "<"c
					If LA(1) = "l"c AndAlso LA(2) = "s"c AndAlso LA(3) = ":"c Then
						If savePos = pos Then
							Consume(4)
							' consume <ad:
							EnterMode(LexMode.Tag)
							Return CreateToken(TokenKind.TagStart)
						Else
							Exit Select
						End If
					ElseIf LA(1) = "/"c AndAlso LA(2) = "l"c AndAlso LA(3) = "s"c AndAlso LA(4) = ":"c Then
						If savePos = pos Then
							Consume(5)
							' consume </ad:
							EnterMode(LexMode.Tag)
							Return CreateToken(TokenKind.TagClose)
						Else
							Exit Select
						End If
					End If
					Consume()
					GoTo StartTextRead
				Case ControlChars.Lf, ControlChars.Cr
					ReadWhitespace()
					' handle newlines specially so that line number count is kept
					GoTo StartTextRead
				Case Else

					Consume()
					GoTo StartTextRead
			End Select

			Return CreateToken(TokenKind.TextData)
		End Function

		''' <summary>
		''' reads word. Word contains any alpha character or _
		''' </summary>
		Protected Function ReadId() As Token
			StartRead()

			Consume()
			' consume first character of the word
			While True
				Dim ch As Char = LA(0)
				If [Char].IsLetterOrDigit(ch) OrElse ch = "_"c Then
					Consume()
				Else
					Exit While
				End If
			End While

			Dim tokenData As String = data.Substring(savePos, pos - savePos)

			If keywords.ContainsKey(tokenData) Then
				Return CreateToken(keywords(tokenData))
			Else
				Return CreateToken(TokenKind.ID, tokenData)
			End If
		End Function

		''' <summary>
		''' returns either Integer or Double Token
		''' </summary>
		''' <returns></returns>
		Protected Function ReadNumber() As Token
			StartRead()
			Consume()
			' consume first digit or -
			Dim hasDot As Boolean = False

			While True
				Dim ch As Char = LA(0)
				If [Char].IsNumber(ch) Then
					Consume()

					' if "." and didn't see "." yet, and next char
					' is number, than starting to read decimal number
				ElseIf ch = "."c AndAlso Not hasDot AndAlso [Char].IsNumber(LA(1)) Then
					Consume()
					hasDot = True
				Else
					Exit While
				End If
			End While

			Return CreateToken(If(hasDot, TokenKind.[Double], TokenKind.[Integer]))
		End Function
	End Class
End Namespace

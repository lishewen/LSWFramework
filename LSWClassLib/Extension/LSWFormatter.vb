Imports System.Runtime.CompilerServices

Namespace Extension
	Public Module LSWFormatter
		<Extension()>
		Public Function LSWFormat(ByVal format As String, ByVal source As Object) As String

			If format Is Nothing Then
				Throw New ArgumentNullException(NameOf(format))
			End If

			Dim formattedStrings = (From expression In SplitFormat(format) Select expression.Eval(source)).ToArray()
			Return String.Join("", formattedStrings)
		End Function

		Private Function SplitFormat(ByVal format As String) As IEnumerable(Of ITextExpression)
			Dim exprEndIndex As Integer = -1
			Dim expStartIndex As Integer
			Dim result As New List(Of ITextExpression)

			Do
				expStartIndex = format.IndexOfExpressionStart(exprEndIndex + 1)
				If expStartIndex < 0 Then
					'everything after last end brace index. 
					If exprEndIndex + 1 < format.Length Then
						result.Add(New LiteralFormat(format.Substring(exprEndIndex + 1)))
					End If
					Exit Do
				End If

				If expStartIndex - exprEndIndex - 1 > 0 Then
					'everything up to next start brace index 
					result.Add(New LiteralFormat(format.Substring(exprEndIndex + 1, expStartIndex - exprEndIndex - 1)))
				End If

				Dim endBraceIndex As Integer = format.IndexOfExpressionEnd(expStartIndex + 1)
				If endBraceIndex < 0 Then
					'rest of string, no end brace (could be invalid expression) 
					result.Add(New FormatExpression(format.Substring(expStartIndex)))
				Else
					exprEndIndex = endBraceIndex
					'everything from start to end brace. 
					result.Add(New FormatExpression(format.Substring(expStartIndex, endBraceIndex - expStartIndex + 1)))
				End If
			Loop While expStartIndex > -1

			Return result
		End Function

		<Extension()>
		Private Function IndexOfExpressionStart(ByVal format As String, ByVal startIndex As Integer) As Integer
			Dim index As Integer = format.IndexOf("{"c, startIndex)
			If index = -1 Then
				Return index
			End If

			'peek ahead. 
			If index + 1 < format.Length Then
				Dim nextChar As Char = format(index + 1)
				If nextChar = "{"c Then
					Return IndexOfExpressionStart(format, index + 2)
				End If
			End If

			Return index
		End Function

		<Extension()>
		Private Function IndexOfExpressionEnd(ByVal format As String, ByVal startIndex As Integer) As Integer
			Dim endBraceIndex As Integer = format.IndexOf("}"c, startIndex)
			If endBraceIndex = -1 Then
				Return endBraceIndex
			End If
			'start peeking ahead until there are no more braces... 
			' }}}} 
			Dim braceCount As Integer = 0
			For i As Integer = endBraceIndex + 1 To format.Length - 1
				If format(i) = "}"c Then
					braceCount += 1
				Else
					Exit For
				End If
			Next
			If braceCount Mod 2 = 1 Then
				Return IndexOfExpressionEnd(format, endBraceIndex + braceCount + 1)
			End If

			Return endBraceIndex
		End Function
	End Module
End Namespace
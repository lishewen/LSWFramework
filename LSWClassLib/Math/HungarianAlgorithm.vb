Namespace Math
	Public Module HungarianAlgorithm
		''' <summary>
		''' Finds the optimal assignments for a given matrix of agents and costed tasks such that the total cost is
		''' minimized.
		''' </summary>
		''' <param name="costs">A cost matrix; the element at row <em>i</em> and column <em>j</em> represents the cost of
		''' agent <em>i</em> performing task <em>j</em>.</param>
		''' <returns>A matrix of assignments; the value of element <em>i</em> is the column of the task assigned to agent
		''' <em>i</em>.</returns>
		''' <exception cref="ArgumentNullException"><paramref name="costs"/> is <see langword="null"/>.</exception>
		<Runtime.CompilerServices.Extension>
		Public Function FindAssignments(costs As Integer(,)) As Integer()
			If costs Is Nothing Then
				Throw New ArgumentNullException(NameOf(costs))
			End If

			Dim h = costs.GetLength(0)
			Dim w = costs.GetLength(1)

			For i As Integer = 0 To h - 1
				Dim min = Integer.MaxValue
				For j As Integer = 0 To w - 1
					min = System.Math.Min(min, costs(i, j))
				Next
				For j As Integer = 0 To w - 1
					costs(i, j) -= min
				Next
			Next

			Dim masks = New Byte(h - 1, w - 1) {}
			Dim rowsCovered = New Boolean(h - 1) {}
			Dim colsCovered = New Boolean(w - 1) {}
			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					If costs(i, j) = 0 AndAlso Not rowsCovered(i) AndAlso Not colsCovered(j) Then
						masks(i, j) = 1
						rowsCovered(i) = True
						colsCovered(j) = True
					End If
				Next
			Next
			ClearCovers(rowsCovered, colsCovered, w, h)

			Dim path = New Location(w * h - 1) {}
			Dim pathStart As Location = Nothing
			Dim [step] = 1
			While [step] <> -1
				Select Case [step]
					Case 1
						[step] = RunStep1(costs, masks, rowsCovered, colsCovered, w, h)
						Exit Select
					Case 2
						[step] = RunStep2(costs, masks, rowsCovered, colsCovered, w, h,
							pathStart)
						Exit Select
					Case 3
						[step] = RunStep3(costs, masks, rowsCovered, colsCovered, w, h,
							path, pathStart)
						Exit Select
					Case 4
						[step] = RunStep4(costs, masks, rowsCovered, colsCovered, w, h)
						Exit Select
				End Select
			End While

			Dim agentsTasks = New Integer(h - 1) {}
			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					If masks(i, j) = 1 Then
						agentsTasks(i) = j
						Exit For
					End If
				Next
			Next
			Return agentsTasks
		End Function

		Private Function RunStep1(costs As Integer(,), masks As Byte(,), rowsCovered As Boolean(), colsCovered As Boolean(), w As Integer, h As Integer) As Integer
			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					If masks(i, j) = 1 Then
						colsCovered(j) = True
					End If
				Next
			Next
			Dim colsCoveredCount = 0
			For j As Integer = 0 To w - 1
				If colsCovered(j) Then
					colsCoveredCount += 1
				End If
			Next
			If colsCoveredCount = h Then
				Return -1
			Else
				Return 2
			End If
		End Function

		Private Function RunStep2(costs As Integer(,), masks As Byte(,), rowsCovered As Boolean(), colsCovered As Boolean(), w As Integer, h As Integer,
			ByRef pathStart As Location) As Integer
			Dim loc As Location
			While True
				loc = FindZero(costs, masks, rowsCovered, colsCovered, w, h)
				If loc.Row = -1 Then
					Return 4
				Else
					masks(loc.Row, loc.Column) = 2
					Dim starCol = FindStarInRow(masks, w, loc.Row)
					If starCol <> -1 Then
						rowsCovered(loc.Row) = True
						colsCovered(starCol) = False
					Else
						pathStart = loc
						Return 3
					End If
				End If
			End While
			Return 0
		End Function

		Private Function RunStep3(costs As Integer(,), masks As Byte(,), rowsCovered As Boolean(), colsCovered As Boolean(), w As Integer, h As Integer,
			path As Location(), pathStart As Location) As Integer
			Dim pathIndex = 0
			path(0) = pathStart
			While True
				Dim row = FindStarInColumn(masks, h, path(pathIndex).Column)
				If row = -1 Then
					Exit While
				End If
				pathIndex += 1
				path(pathIndex) = New Location(row, path(pathIndex - 1).Column)
				Dim col = FindPrimeInRow(masks, w, path(pathIndex).Row)
				pathIndex += 1
				path(pathIndex) = New Location(path(pathIndex - 1).Row, col)
			End While
			ConvertPath(masks, path, pathIndex + 1)
			ClearCovers(rowsCovered, colsCovered, w, h)
			ClearPrimes(masks, w, h)
			Return 1
		End Function

		Private Function RunStep4(costs As Integer(,), masks As Byte(,), rowsCovered As Boolean(), colsCovered As Boolean(), w As Integer, h As Integer) As Integer
			Dim minValue = FindMinimum(costs, rowsCovered, colsCovered, w, h)
			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					If rowsCovered(i) Then
						costs(i, j) += minValue
					End If
					If Not colsCovered(j) Then
						costs(i, j) -= minValue
					End If
				Next
			Next
			Return 2
		End Function

		Private Sub ConvertPath(masks As Byte(,), path As Location(), pathLength As Integer)
			For i As Integer = 0 To pathLength - 1
				If masks(path(i).Row, path(i).Column) = 1 Then
					masks(path(i).Row, path(i).Column) = 0
				ElseIf masks(path(i).Row, path(i).Column) = 2 Then
					masks(path(i).Row, path(i).Column) = 1
				End If
			Next
		End Sub

		Private Function FindZero(costs As Integer(,), masks As Byte(,), rowsCovered As Boolean(), colsCovered As Boolean(), w As Integer, h As Integer) As Location
			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					If costs(i, j) = 0 AndAlso Not rowsCovered(i) AndAlso Not colsCovered(j) Then
						Return New Location(i, j)
					End If
				Next
			Next
			Return New Location(-1, -1)
		End Function

		Private Function FindMinimum(costs As Integer(,), rowsCovered As Boolean(), colsCovered As Boolean(), w As Integer, h As Integer) As Integer
			Dim minValue = Integer.MaxValue
			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					If Not rowsCovered(i) AndAlso Not colsCovered(j) Then
						minValue = System.Math.Min(minValue, costs(i, j))
					End If
				Next
			Next
			Return minValue
		End Function

		Private Function FindStarInRow(masks As Byte(,), w As Integer, row As Integer) As Integer
			For j As Integer = 0 To w - 1
				If masks(row, j) = 1 Then
					Return j
				End If
			Next
			Return -1
		End Function

		Private Function FindStarInColumn(masks As Byte(,), h As Integer, col As Integer) As Integer
			For i As Integer = 0 To h - 1
				If masks(i, col) = 1 Then
					Return i
				End If
			Next
			Return -1
		End Function

		Private Function FindPrimeInRow(masks As Byte(,), w As Integer, row As Integer) As Integer
			For j As Integer = 0 To w - 1
				If masks(row, j) = 2 Then
					Return j
				End If
			Next
			Return -1
		End Function

		Private Sub ClearCovers(rowsCovered As Boolean(), colsCovered As Boolean(), w As Integer, h As Integer)
			For i As Integer = 0 To h - 1
				rowsCovered(i) = False
			Next
			For j As Integer = 0 To w - 1
				colsCovered(j) = False
			Next
		End Sub

		Private Sub ClearPrimes(masks As Byte(,), w As Integer, h As Integer)
			For i As Integer = 0 To h - 1
				For j As Integer = 0 To w - 1
					If masks(i, j) = 2 Then
						masks(i, j) = 0
					End If
				Next
			Next
		End Sub

		Private Structure Location
			Public Row As Integer
			Public Column As Integer

			Public Sub New(row As Integer, col As Integer)
				Me.Row = row
				Me.Column = col
			End Sub
		End Structure
	End Module
End Namespace
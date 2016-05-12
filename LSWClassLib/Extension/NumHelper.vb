Imports System.Runtime.CompilerServices

Namespace Extension
	Public Module NumHelper
		<Extension>
		Public Function FindLike(Of T As {IComparable, IConvertible, IFormattable})(ml As List(Of T), num As T) As T
			'初始化最小间隔值
			Dim minValue = System.Math.Abs(Convert.ToDouble(ml(0)) - Convert.ToDouble(num))
			Dim result As T
			'要找到的数据
			For Each m In ml
				Dim tt = Convert.ToDouble(m)
				Dim temp = System.Math.Abs(Convert.ToDouble(num) - tt)
				If temp < minValue Then
					minValue = temp
					result = CTypeDynamic(Of T)(tt)
				End If
			Next
			Return result
		End Function

		<Extension>
		Public Function IsBetween(Of T As IComparable(Of T))(value As T, lowerBound As T, upperBound As T, Optional includeLowerBound As Boolean = False, Optional includeUpperBound As Boolean = False) As Boolean
			If value Is Nothing Then
				Throw New ArgumentNullException(NameOf(value))
			End If

			Dim lowerCompareResult = value.CompareTo(lowerBound)
			Dim upperCompareResult = value.CompareTo(upperBound)

			Return (includeLowerBound AndAlso lowerCompareResult = 0) OrElse (includeUpperBound AndAlso upperCompareResult = 0) OrElse (lowerCompareResult > 0 AndAlso upperCompareResult < 0)
		End Function
	End Module
End Namespace
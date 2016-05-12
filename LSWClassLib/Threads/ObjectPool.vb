Imports System.Collections.Concurrent

Namespace Threads
	Public Class ObjectPool(Of T)
		Private _objects As ConcurrentBag(Of T)
		Private _objectGenerator As Func(Of T)

		Public Sub New(ByVal objectGenerator As Func(Of T))
			If objectGenerator Is Nothing Then Throw New ArgumentNullException(NameOf(objectGenerator))
			_objects = New ConcurrentBag(Of T)()
			_objectGenerator = objectGenerator
		End Sub

		Public Function GetObject() As T
			Dim item As T
			If _objects.TryTake(item) Then Return item
			Return _objectGenerator()
		End Function

		Public Sub PutObject(ByVal item As T)
			_objects.Add(item)
		End Sub
	End Class
End Namespace
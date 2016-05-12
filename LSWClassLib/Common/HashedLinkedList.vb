Namespace Common
    Public Class HashedLinkedList(Of T)
        Private ReadOnly _list As New LinkedList(Of T)()
        Private ReadOnly _nodes As New Dictionary(Of T, LinkedListNode(Of T))()

        Public Function AddLast(value As T) As LinkedListNode(Of T)
            Dim node = _list.AddLast(value)
            _nodes.Add(value, node)

            Return node
        End Function

        Public Sub Remove(value As T)
            Dim node = _nodes(value)
            _nodes.Remove(value)
            _list.Remove(node)
        End Sub
    End Class
End Namespace
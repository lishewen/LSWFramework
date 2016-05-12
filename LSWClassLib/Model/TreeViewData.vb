Imports System.Windows.Forms

Namespace Model
    <Serializable()>
    Public Structure TreeViewData
        Public Nodes() As TreeNodeData

        Public Sub New(ByVal treeview As TreeView)
            'Check to see if there are any root nodes in the TreeView
            If treeview.Nodes.Count = 0 Then Exit Sub

            'Populate the Nodes array with child nodes
            ReDim Nodes(treeview.Nodes.Count - 1)
            For i As Integer = 0 To treeview.Nodes.Count - 1
                Nodes(i) = New TreeNodeData(treeview.Nodes(i))
            Next
        End Sub

        Public Sub PopulateTree(ByVal treeView As TreeView)
            'Check to see if there are any root nodes in the TreeViewData
            If Me.Nodes Is Nothing OrElse Me.Nodes.Length = 0 Then Exit Sub

            'Populate the TreeView with child nodes
            treeView.BeginUpdate()
            For i As Integer = 0 To Me.Nodes.Length - 1
                treeView.Nodes.Add(Me.Nodes(i).ToTreeNode)
            Next
            treeView.EndUpdate()
        End Sub
    End Structure
End Namespace
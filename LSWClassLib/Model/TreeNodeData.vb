Imports System.Windows.Forms

Namespace Model
    <Serializable()>
    Public Structure TreeNodeData
        Public Text As String
        Public ImageIndex As Integer
        Public SelectedImageIndex As Integer
        Public Checked As Boolean
        Public Expanded As Boolean
        Public Tag As Object
        Public Nodes() As TreeNodeData

        Public Sub New(ByVal node As TreeNode)
            'Set the basic TreeNode properties
            Me.Text = node.Text
            Me.ImageIndex = node.ImageIndex
            Me.SelectedImageIndex = node.SelectedImageIndex
            Me.Checked = node.Checked
            Me.Expanded = node.IsExpanded

            'See if there is an object in the tag property 
            'and if it is serializable
            If (Not node.Tag Is Nothing) AndAlso node.Tag.GetType.IsSerializable Then Me.Tag = node.Tag

            'Check to see if there are any child nodes
            If node.Nodes.Count = 0 Then Exit Sub

            'Recurse through child nodes and add to Nodes array
            ReDim Nodes(node.Nodes.Count - 1)
            For i As Integer = 0 To node.Nodes.Count - 1
                Nodes(i) = New TreeNodeData(node.Nodes(i))
            Next
        End Sub

        Public Function ToTreeNode() As TreeNode
            'Create TreeNode based on instance of 
            'TreeNodeData and set basic properties
            ToTreeNode = New TreeNode(Me.Text, Me.ImageIndex, Me.SelectedImageIndex)
            ToTreeNode.Checked = Me.Checked
            ToTreeNode.Tag = Me.Tag
            If Me.Expanded Then ToTreeNode.Expand()

            'Recurse through child nodes adding to Nodes collection
            If Me.Nodes Is Nothing OrElse Me.Nodes.Length = 0 Then Exit Function

            For i As Integer = 0 To Me.Nodes.Length - 1
                ToTreeNode.Nodes.Add(Me.Nodes(i).ToTreeNode)
            Next
        End Function
    End Structure
End Namespace
Imports System.Windows.Forms
Imports System.Runtime.CompilerServices

Namespace Extension
    Public Module ListBoxExtension
        <Extension()>
        Private Function MoveSelectedItem(listBox As ListBox, selectedIndex As Integer, isUp As Boolean) As Boolean
            Dim temp As Integer = IIf(isUp, 0, listBox.Items.Count - 1)
            If selectedIndex <> temp Then
                Dim current = listBox.Items(selectedIndex)
                temp = IIf(isUp, -1, 1)
                Dim insertAt = selectedIndex + temp

                listBox.Items.RemoveAt(selectedIndex)
                listBox.Items.Insert(insertAt, current)
                listBox.SelectedIndex = insertAt
                Return True
            End If
            Return False
        End Function

        <Extension()>
        Public Function MoveSelectedItem(listBox As ListBox, isUp As Boolean) As Boolean
            Return MoveSelectedItem(listBox, listBox.SelectedIndex, isUp)
        End Function

        <Extension()>
        Public Function MoveSelectedItem(listBox As ListBox, isUp As Boolean, noSelectAction As Action) As Boolean
            If listBox.SelectedItems.Count > 0 Then
                Return listBox.MoveSelectedItem(isUp)
            Else
                noSelectAction()
                Return False
            End If
        End Function

        <Extension()>
        Public Function MoveSelectedItems(listBox As ListBox, isUp As Boolean) As Boolean
            Dim result = True
            Dim indices = listBox.SelectedIndices
            If isUp Then
                If listBox.SelectedItems.Count > 0 AndAlso indices(0) <> 0 Then
                    For Each i In indices
                        result &= MoveSelectedItem(listBox, i, True)
                    Next
                End If
            Else
                If listBox.SelectedItems.Count > 0 AndAlso indices(indices.Count - 1) <> listBox.Items.Count - 1 Then
                    For i = indices.Count - 1 To 0 Step -1
                        result &= MoveSelectedItem(listBox, indices(i), False)
                    Next
                End If
            End If
            Return result
        End Function

        <Extension()>
        Public Function MoveSelectedItems(listBox As ListBox, isUp As Boolean, noSelectAction As Action) As Boolean
            If listBox.SelectedItems.Count > 0 Then
                Return listBox.MoveSelectedItems(isUp)
            Else
                noSelectAction()
                Return False
            End If
        End Function
    End Module
End Namespace
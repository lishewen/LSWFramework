Imports System.Windows.Forms
Imports System.Runtime.CompilerServices

Namespace Extension
    Public Module TextBoxHelper
        Private lasttime As New Dictionary(Of TextBox, DateTime)

        <Extension()>
        Public Sub ShowMsg(tb As TextBox, msg As String)
            If tb.InvokeRequired Then
                tb.Invoke(Sub() tb.AppendText(Now & " " & msg & vbCrLf))
            Else
                tb.AppendText(Now & " " & msg & vbCrLf)
            End If
        End Sub

        <Extension>
        Public Sub ShowMsg(tb As TextBox, msg As String, tick As Integer)
            If Not lasttime.Keys.Contains(tb) Then
                'tb.Invoke(Sub() tb.AppendText(Now & " " & msg & vbCrLf))
                tb.ShowMsg(msg)
                lasttime.Add(tb, Now)
            ElseIf (Now - lasttime(tb)).TotalMilliseconds > tick Then
                'tb.Invoke(Sub() tb.AppendText(Now & " " & msg & vbCrLf))
                tb.ShowMsg(msg)
                lasttime(tb) = Now
            End If
        End Sub

        <Extension>
        Public Sub InsertToFocus(tb As TextBox, msg As String)
            Dim i = tb.SelectionStart
            Dim txt = tb.Text
            tb.Text = txt.Insert(i, msg)
            tb.SelectionStart = i + msg.Length
            tb.Focus()
        End Sub
    End Module
End Namespace
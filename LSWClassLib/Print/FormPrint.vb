Imports System.Windows.Forms
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace Print
    Public Module FormPrint
        ''' <summary>
        ''' 为每一个Label和TextBox绘图
        ''' </summary>
        ''' <param name="O">父对像</param>
        ''' <param name="CS">控件集</param>
        ''' <param name="e">绘制事件</param>
        <Extension()>
        Public Sub FindLabelOrTextBox(O As [Object], CS As Control.ControlCollection, e As System.Drawing.Printing.PrintPageEventArgs, TopH As Integer)
            For Each cot As Control In CS
                If GetType(Button) <> cot.GetType Then
                    'typeof(Label) == cot.GetType() || typeof(TextBox) == cot.GetType()
                    Dim ptop As Single = 0
                    If O.GetType = GetType(Panel) Then
                        Dim p As Panel = TryCast(O, Panel)
                        ptop = p.Top
                    End If

                    If Not cot.Text.Equals("*") Then
                        e.Graphics.DrawString(cot.Text, cot.Font, Brushes.Black, cot.Left, cot.Top + ptop + TopH)
                    End If
                End If
                If cot.Controls.Count > 0 Then
                    FindLabelOrTextBox(cot, cot.Controls, e, TopH)
                End If
            Next
        End Sub
    End Module
End Namespace
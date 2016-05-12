Imports System.Windows.Forms
Imports System.Runtime.CompilerServices
Imports System.IO
Imports System.Text

Namespace Extension
    Public Module DataGridViewHelper
        <Extension()> _
        Public Function DataGridViewToDataTable(ByVal dv As DataGridView) As DataTable
            Dim dt As New DataTable()
            Dim dc As DataColumn
            Dim count As Integer
            For i As Integer = 0 To dv.Columns.Count - 1
                dc = New DataColumn()
                dc.ColumnName = dv.Columns(i).HeaderText.ToString()
                dt.Columns.Add(dc)
            Next
            If dv.AllowUserToAddRows Then
                count = dv.Rows.Count - 2
            Else
                count = dv.Rows.Count - 1
            End If
            For j As Integer = 0 To count
                Dim dr As DataRow = dt.NewRow()
                For x As Integer = 0 To dv.Columns.Count - 1
                    dr(x) = dv.Rows(j).Cells(x).Value
                Next
                dt.Rows.Add(dr)
            Next
            Return dt
        End Function

        <Extension()>
        Public Function ExportToCsv(dgv As DataGridView) As Boolean
            Return ExportToCsv(dgv, Nothing)
        End Function

        <Extension()>
        Public Function ExportToCsv(dgv As DataGridView, pb As ProgressBar) As Boolean
            Dim dlg As New SaveFileDialog()
            dlg.Filter = "CSV(逗号分隔)(*.csv)|*.csv"
            dlg.FilterIndex = 0
            dlg.RestoreDirectory = True
            dlg.CreatePrompt = True
            dlg.Title = "保存为CSV(逗号分隔)文件"
            If dlg.ShowDialog() = DialogResult.OK Then
                Dim myStream As Stream
                myStream = dlg.OpenFile()
                Using sw As New StreamWriter(myStream, Encoding.Default)
                    Dim columnTitle As String = ""
                    Try
                        '写入列标题  
                        For i As Integer = 0 To dgv.ColumnCount - 1
                            If i > 0 Then
                                columnTitle += ","
                            End If
                            columnTitle += dgv.Columns(i).HeaderText
                        Next
                        columnTitle.Remove(columnTitle.Length - 1)
                        sw.WriteLine(columnTitle)
                        '写入列内容     
                        For j As Integer = 0 To dgv.Rows.Count - 1
                            Dim columnValue As String = ""
                            If pb IsNot Nothing Then pb.Value = j * 100 / dgv.Rows.Count
                            For k As Integer = 0 To dgv.Columns.Count - 1
                                If k > 0 Then
                                    columnValue += ","
                                End If
                                If dgv.Rows(j).Cells(k).Value Is Nothing Then
                                    columnValue += ""
                                Else
                                    Dim m As String = dgv.Rows(j).Cells(k).Value.ToString().Trim()
                                    columnValue += m.Replace(",", "，")
                                End If
                            Next
                            columnValue.Remove(columnValue.Length - 1)
                            sw.WriteLine(columnValue)
                        Next
                        sw.Close()
                        myStream.Close()
                    Catch e As Exception
                        Return False
                    Finally
                        sw.Close()
                        myStream.Close()
                    End Try
                End Using
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
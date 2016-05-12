Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports LSW.Extension

Namespace Print
    Public Class PrintDG
        Private Shared StrFormat As StringFormat     ' Holds content of a TextBox Cell to write by DrawString
        Private Shared ChkBox As CheckBox            ' Holds content of a Boolean Cell to write by DrawImage

        Private Shared TotalWidth As Int16           ' Summation of Columns widths
        Private Shared RowPos As Int16               ' Position of currently printing row 
        Private Shared NewPage As Boolean            ' Indicates if a new page reached 
        Private Shared PageNo As Int16               ' Number of pages to print 
        Private Shared ColumnLefts As New ArrayList  ' Left Coordinate of Columns
        Private Shared ColumnWidths As New ArrayList ' Width of Columns
        Private Shared ColumnTypes As New ArrayList  ' DataType of Columns
        Private Shared nHeight As Int16              ' Height of DataGrid Cell
        Private Shared RowsPerPage As Int16          ' Number of Rows per Page 
        Private Shared WithEvents printDoc As New System.Drawing.Printing.PrintDocument ' PrintDocumnet Object used for printing

        Private Shared PrintTitle As String = ""               ' Header of pages
        Private Shared dg As DataGridView                          ' Holds DataGrid Object to print its contents
        Private Shared SelectedColumns As New List(Of String)  ' The Columns Selected by user to print.
        Private Shared AvailableColumns As New List(Of String) ' All Columns avaiable in DataGrid   
        Private Shared PrintFont As Font                       ' Font to use in the printing of DataGrid contents
        Private Shared PrintFontColor As Color                 ' Font Color to use in the printing of DataGrid contents
        Private Shared PrintAllRows As Boolean = True          ' True = print all rows,  False = print selected rows    

        Public Shared Sub Print_DataGrid(ByVal dg1 As DataGridView)
            Dim ppvw As PrintPreviewDialog
            Try
                ' Save DataGrid attributes
                dg = dg1
                PrintFont = dg.Font
                PrintFontColor = dg.ForeColor

                ' Get all Coulmns Names in the DataGrid
                AvailableColumns.Clear()
                For Each c As DataGridViewColumn In dg.Columns
                    AvailableColumns.Add(c.HeaderText)
                Next

                ' Show PrintOption Form
                Dim dlg As New PrintOptions(PrintTitle, AvailableColumns)
                If dlg.ShowDialog() <> DialogResult.OK Then Exit Sub

                PrintTitle = dlg.PrintTitle
                PrintAllRows = dlg.PrintAllRows
                SelectedColumns = dlg.GetSelectedColumns
                If dlg.PrintFont IsNot Nothing Then PrintFont = dlg.PrintFont
                If dlg.PrintFontColor.Name <> "" And dlg.PrintFontColor.Name <> "0" Then
                    PrintFontColor = dlg.PrintFontColor
                End If

                RowsPerPage = 0

                ppvw = New PrintPreviewDialog
                ppvw.Document = printDoc

                ' Show Print Preview Page
                If ppvw.ShowDialog() <> DialogResult.OK Then Exit Sub

                ' Print the Documnet
                printDoc.Print()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally

            End Try
        End Sub

        Private Shared Sub PrintDoc_BeginPrint(ByVal sender As Object, _
                    ByVal e As System.Drawing.Printing.PrintEventArgs) Handles printDoc.BeginPrint
            Try
                ' Formatting the Content of Text Cell to print
                StrFormat = New StringFormat
                StrFormat.Alignment = StringAlignment.Near
                StrFormat.LineAlignment = StringAlignment.Center
                StrFormat.Trimming = StringTrimming.EllipsisCharacter

                ColumnLefts.Clear()
                ColumnWidths.Clear()
                ColumnTypes.Clear()
                nHeight = 0
                RowsPerPage = 0

                ChkBox = New CheckBox

                ' Calculating Total Widths
                TotalWidth = 0
                For Each oColumn As DataGridViewColumn In dg.Columns
                    If Not PrintDG.SelectedColumns.Contains(oColumn.HeaderText) Then Continue For
                    TotalWidth += oColumn.Width
                Next
                PageNo = 1
                NewPage = True
                RowPos = 0
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally

            End Try
        End Sub

        Private Shared Sub PrintDoc_PrintPage(ByVal sender As Object, _
                ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles printDoc.PrintPage

            Dim nWidth As Int16, i As Int16
            Dim nTop As Int16 = e.MarginBounds.Top
            Dim nLeft As Int16 = e.MarginBounds.Left
            Dim dv As DataView
            Dim flag = False

            If dg.DataSource IsNot Nothing Then
                If TypeOf dg.DataSource Is DataTable Then
                    dv = CType(dg.DataSource, DataTable).DefaultView
                ElseIf TypeOf dg.DataSource Is DataView Then
                    dv = CType(dg.DataSource, DataView)
                ElseIf TypeOf dg.DataSource Is BindingSource Then
                    dv = CType(CType(dg.DataSource, BindingSource).DataSource, DataTable).DefaultView
                Else
                    dv = CType(dg.DataSource, IList)
                    'flag = True
                End If
            Else
                dv = dg.DataGridViewToDataTable.DefaultView
            End If

            Try
                If PrintFont Is Nothing Then PrintFont = dg.Font

                ' Before starting first page, it saves Width & Height of Headers and CoulmnType
                If PageNo = 1 Then
                    Dim GridCol As DataGridViewColumn
                    For Each GridCol In dg.Columns
                        ' Skip if the current column not selected
                        If Not PrintDG.SelectedColumns.Contains(GridCol.HeaderText) Then
                            Continue For
                        End If
                        ' Calculate width & height of headres 
                        nWidth = CType(System.Math.Floor(GridCol.Width / TotalWidth * _
                                TotalWidth * (e.MarginBounds.Width / TotalWidth)), Int16)
                        nHeight = e.Graphics.MeasureString(GridCol.HeaderText, _
                                PrintFont, nWidth).Height + 11
                        ' Save width & height of headres and ColumnType
                        ColumnLefts.Add(nLeft)
                        ColumnWidths.Add(nWidth)
                        ColumnTypes.Add(GridCol.GetType)
                        nLeft += nWidth
                    Next
                End If

                ' Print Current Page, Row by Row
                Do While RowPos <= dv.Count - 1
                    If Not PrintAllRows AndAlso Not dg.Rows(RowPos).Selected Then
                        RowPos += 1 : Continue Do
                    End If

                    Dim GridCol As DataGridViewColumn
                    Dim oRow As DataRowView = dv(RowPos)
                    If nTop + nHeight >= e.MarginBounds.Height + e.MarginBounds.Top Then
                        DrawFooter(e, RowsPerPage)
                        NewPage = True
                        PageNo += 1
                        e.HasMorePages = True
                        Exit Sub
                    Else
                        If NewPage Then
                            ' Draw Header
                            e.Graphics.DrawString(PrintTitle, New Font(PrintFont, FontStyle.Bold), _
                                Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top - _
                                e.Graphics.MeasureString(PrintTitle, New Font(PrintFont, _
                                FontStyle.Bold), e.MarginBounds.Width).Height - 13)

                            Dim Date_Time As String = Now.ToLongDateString + " " + Now.ToShortTimeString

                            e.Graphics.DrawString(Date_Time, New Font(PrintFont, FontStyle.Bold), _
                                Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width - _
                                e.Graphics.MeasureString(Date_Time, New Font(PrintFont, FontStyle.Bold), _
                                e.MarginBounds.Width).Width), e.MarginBounds.Top - _
                                e.Graphics.MeasureString(PrintTitle, _
                                New Font(New Font(PrintFont, FontStyle.Bold), _
                                FontStyle.Bold), e.MarginBounds.Width).Height - 13)

                            ' Draw Columns
                            nTop = e.MarginBounds.Top
                            i = 0
                            For Each GridCol In dg.Columns

                                If Not PrintDG.SelectedColumns.Contains(GridCol.HeaderText) Then Continue For

                                e.Graphics.FillRectangle(New SolidBrush(Drawing.Color.LightGray), _
                                           New Rectangle(ColumnLefts(i), nTop, ColumnWidths(i), nHeight))
                                e.Graphics.DrawRectangle(Pens.Black, New Rectangle(ColumnLefts(i), _
                                           nTop, ColumnWidths(i), nHeight))
                                e.Graphics.DrawString(GridCol.HeaderText, PrintFont, _
                                           New SolidBrush(PrintFontColor), New RectangleF(ColumnLefts(i), _
                                           nTop, ColumnWidths(i), nHeight), StrFormat)
                                i += 1
                            Next
                            NewPage = False
                        End If
                        nTop += nHeight
                        i = 0
                        ' Draw Columns Contents
                        For Each GridCol In dg.Columns

                            If Not PrintDG.SelectedColumns.Contains(GridCol.HeaderText) Then
                                Continue For
                            End If

                            'If flag Then GridCol.DataPropertyName = GridCol.HeaderText
                            If String.IsNullOrEmpty(GridCol.DataPropertyName) Then GridCol.DataPropertyName = GridCol.HeaderText

                            Dim cellval As String = oRow.Row.Item(GridCol.DataPropertyName).ToString.Trim
                            If ColumnTypes(i) Is GetType(DataGridViewTextBoxColumn) Then
                                ' For the TextBox Column

                                ' Draw Content of TextBox Cell
                                e.Graphics.DrawString(cellval, PrintFont, _
                                    New SolidBrush(PrintFontColor), New RectangleF(ColumnLefts(i), _
                                    nTop, ColumnWidths(i), nHeight), StrFormat)

                            ElseIf ColumnTypes(i) Is GetType(DataGridViewCheckBoxColumn) Then
                                ' For the CheckBox Column

                                ' Draw Content of CheckBox Cell
                                ChkBox.Size = New Size(14, 14)
                                ChkBox.Checked = CType(oRow.Row.Item(GridCol.DataPropertyName), Boolean)
                                Dim oBitmap As New Bitmap(ColumnWidths(i), nHeight)
                                Dim oTempGraphics As Graphics = Graphics.FromImage(oBitmap)
                                oTempGraphics.FillRectangle(Brushes.White, _
                                    New Rectangle(0, 0, oBitmap.Width, oBitmap.Height))
                                ChkBox.DrawToBitmap(oBitmap, _
                                    New Rectangle(CType((oBitmap.Width - ChkBox.Width) / 2, Int32), _
                                    CType((oBitmap.Height - ChkBox.Height) / 2, Int32), ChkBox.Width, ChkBox.Height))
                                e.Graphics.DrawImage(oBitmap, New Point(ColumnLefts(i), nTop))
                            Else
                                e.Graphics.DrawString(cellval, PrintFont, _
                                    New SolidBrush(PrintFontColor), New RectangleF(ColumnLefts(i), _
                                    nTop, ColumnWidths(i), nHeight), StrFormat)
                            End If

                            e.Graphics.DrawRectangle(Pens.Black, New Rectangle(ColumnLefts(i), _
                                    nTop, ColumnWidths(i), nHeight))
                            i += 1
                        Next
                    End If

                    RowPos += 1
                    ' For the first page it calculates Rows per Page
                    If PageNo = 1 Then
                        RowsPerPage += 1
                    End If
                Loop

                If RowsPerPage = 0 Then Exit Sub
                ' Write Footer (Page Number)
                DrawFooter(e, RowsPerPage)
                e.HasMorePages = False
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally

            End Try
        End Sub

        Private Shared Sub DrawFooter(ByVal e As System.Drawing.Printing.PrintPageEventArgs, ByVal RowsPerPage As Int32)
            Dim cnt As Integer = 0, i As Integer
            If dg.DataSource Is Nothing Then Exit Sub
            Dim dv As DataView
            If TypeOf dg.DataSource Is DataTable Then
                dv = CType(dg.DataSource, DataTable).DefaultView
            ElseIf TypeOf dg.DataSource Is DataView Then
                dv = CType(dg.DataSource, DataView)
            ElseIf TypeOf dg.DataSource Is BindingSource Then
                dv = CType(CType(dg.DataSource, BindingSource).DataSource, DataTable).DefaultView
            Else
                dv = CType(dg.DataSource, IList)
            End If

            ' Detemine number of printing rows 
            If PrintAllRows Then
                cnt = dv.Count - 1
            Else
                For i = 0 To dv.Count - 1
                    If dg.Rows(i).Selected Then
                        cnt += 1
                    End If
                Next
            End If

            ' Write Page Number in the Bottom of Page
            Dim sPageNo As String = PageNo.ToString + " of " + _
                        System.Math.Ceiling(cnt / RowsPerPage).ToString
            e.Graphics.DrawString(sPageNo, dg.Font, Brushes.Black, _
                        e.MarginBounds.Left + (e.MarginBounds.Width - _
                        e.Graphics.MeasureString(sPageNo, dg.Font, _
                        e.MarginBounds.Width).Width) / 2, e.MarginBounds.Top + _
                        e.MarginBounds.Height + 31)
        End Sub
    End Class
End Namespace
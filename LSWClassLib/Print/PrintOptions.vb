Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms

Namespace Print
    Public Class PrintOptions
        Private m_PrintTitle As String
        Private m_PrintAllRows As Boolean = True
        Private m_Font As Font
        Private m_FontColor As Color

        Public Sub New(ByVal PrintTitle As String, _
                       ByVal availableFields As List(Of String))

            InitializeComponent()

            For Each field As String In availableFields
                chklst.Items.Add(field, True)
            Next
            m_PrintTitle = PrintTitle
        End Sub

        Private Sub PrintOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            ' set default rows to print
            rdoAllRows.Checked = m_PrintAllRows
        End Sub

        Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
            m_PrintTitle = txtTitle.Text
            m_PrintAllRows = rdoAllRows.Checked
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub btnFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFont.Click
            Dim fnt As New FontDialog
            fnt.ShowColor = True
            fnt.Font = m_Font
            fnt.Color = m_FontColor
            If fnt.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                Exit Sub
            End If
            m_Font = fnt.Font
            m_FontColor = fnt.Color
        End Sub

        Public Function GetSelectedColumns() As List(Of String)
            Dim lst As New List(Of String)
            For Each item As Object In chklst.CheckedItems
                lst.Add(item.ToString)
            Next
            Return lst
        End Function

        Public ReadOnly Property PrintTitle() As String
            Get
                Return m_PrintTitle
            End Get
        End Property

        Public ReadOnly Property PrintAllRows() As Boolean
            Get
                Return m_PrintAllRows
            End Get
        End Property

        Public ReadOnly Property PrintFont() As Font
            Get
                Return m_Font
            End Get
        End Property

        Public ReadOnly Property PrintFontColor() As Color
            Get
                Return m_FontColor
            End Get
        End Property

    End Class
End Namespace
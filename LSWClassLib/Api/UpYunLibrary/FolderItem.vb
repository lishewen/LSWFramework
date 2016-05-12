Namespace API.UpYunLibrary
    Public Class FolderItem
        Public Property FileName As String
        Public Property FileType As String
        Public Property Size As Integer
        Public Property Number As Integer

        Public Sub New(filename As String, filetype As String, size As Integer, number As Integer)
            Me.FileName = filename
            Me.FileType = filetype
            Me.Size = size
            Me.Number = number
        End Sub

        Public Overrides Function ToString() As String
            Return FileName
        End Function
    End Class
End Namespace
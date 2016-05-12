Imports LSW.Extension

Namespace Files
    Public Module Zip
#Region "ZIP文件"
        Public Sub ZipFile(file As String)
            Dim target = file & ".gz"
            Using ts = System.IO.File.OpenWrite(target)
                Using gz = New System.IO.Compression.GZipStream(ts, System.IO.Compression.CompressionMode.Compress)
                    Dim buffer = System.IO.File.ReadAllBytes(file)
                    gz.Write(buffer, 0, buffer.Length)
                End Using
            End Using
        End Sub

        Public Sub UnZipFile(file As String)
            Dim target = file.Substring(0, file.LastIndexOf("."))
            Using fs = System.IO.File.OpenRead(file)
                Using gz = New System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Decompress)
                    Using ts = System.IO.File.OpenWrite(target)
                        Dim buffer = New Byte(1023) {}
                        Dim count = 0
                        While (InlineAssignHelper(count, gz.Read(buffer, 0, buffer.Length))) > 0
                            ts.Write(buffer, 0, count)
                        End While
                    End Using
                End Using
            End Using
        End Sub
#End Region

    End Module
End Namespace
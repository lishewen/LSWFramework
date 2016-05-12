Namespace Files
    Public Class FileSystemScanner

        Public Delegate Sub FileFound_Delegate(sFilName As String)

        Public Sub FindAllFiles(ByVal sSearchPath As String, fFileFound As FileFound_Delegate)
            InternalScanForFiles(sSearchPath, fFileFound)
        End Sub

        Private Sub InternalScanForFiles(ByRef sSearchPath As String, fFileFound As FileFound_Delegate)
            Dim oRootDI As IO.DirectoryInfo
            Dim oFI As IO.FileInfo
            Dim oDI As IO.DirectoryInfo

            ' get info for this path
            Try
                oRootDI = New IO.DirectoryInfo(sSearchPath)

                ' dont try to scan reparse points
                If Not oRootDI.Attributes.HasFlag(IO.FileAttributes.ReparsePoint) Then

                    ' Atempt to get the directories, certain protected folders will not allow this
                    Try
                        For Each oDI In oRootDI.GetDirectories
                            ' recurse into this folder
                            InternalScanForFiles(oDI.FullName, fFileFound)
                        Next
                    Catch ex As Exception
                        'RaiseEvent ScanError(sSearchPath, "Unable to enter Folder.  Details : " & ex.Message, m_bAbort)
                    End Try

                    ' Attempt to get the files, certain protected folders will not allow this
                    Try
                        For Each oFI In oRootDI.GetFiles
                            fFileFound.Invoke(oFI.FullName)
                        Next

                    Catch ex As Exception
                        '                    RaiseEvent ScanError(sSearchPath, "Unable to get Files.  Details : " & ex.Message, m_bAbort)
                    End Try
                End If

            Catch ex As Exception
                '           RaiseEvent ScanError(sSearchPath, "Could not scan files.  Reason : " & ex.Message, m_bAbort)
            End Try

            oRootDI = Nothing
        End Sub

    End Class
End Namespace
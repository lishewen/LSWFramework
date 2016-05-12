Imports LSW.Win32
Imports System.Runtime.InteropServices

Namespace Files
    Public Class MFT
        Private Const INVALID_HANDLE_VALUE = (-1)
        Private Const GENERIC_READ = &H80000000
        Private Const FILE_SHARE_READ = &H1
        Private Const FILE_SHARE_WRITE = &H2
        Private Const OPEN_EXISTING = 3
        Private Const FILE_READ_ATTRIBUTES = &H80
        Private Const FILE_NAME_IINFORMATION = 9
        Private Const FILE_FLAG_BACKUP_SEMANTICS = &H2000000
        Private Const FILE_OPEN_FOR_BACKUP_INTENT = &H4000
        Private Const FILE_OPEN_BY_FILE_ID = &H2000
        Private Const FILE_OPEN = &H1
        Private Const OBJ_CASE_INSENSITIVE = &H40
        Private Const FSCTL_ENUM_USN_DATA = &H900B3

        <StructLayout(LayoutKind.Sequential)> _
        Private Structure USN_RECORD
            Dim RecordLength As Integer
            Dim MajorVersion As Short
            Dim MinorVersion As Short
            Dim FileReferenceNumber As Long
            Dim ParentFileReferenceNumber As Long
            Dim Usn As Long
            Dim TimeStamp As Long
            Dim Reason As Integer
            Dim SourceInfo As Integer
            Dim SecurityId As Integer
            Dim FileAttributes As FileAttribute
            Dim FileNameLength As Short
            Dim FileNameOffset As Short
        End Structure

        Private m_hCJ As IntPtr
        Private m_Buffer As IntPtr
        Private m_BufferSize As Integer
        Private m_DriveLetter As String

        Public Delegate Sub Progress_Delegate(sMessage As String)
        Public Delegate Sub IsMatch_Delegate(sFileName As String, eAttributes As FileAttribute, ByRef bIsMatch As Boolean)
        Public Delegate Sub FileFound_Delegate(sFileName As String, lSize As Long)

        Private Class FSNode
            Public FRN As Long
            Public ParentFRN As Long
            Public FileName As String
            Public IsFile As Boolean

            Sub New(lFRN As Long, lParentFSN As Long, sFileName As String, bIsFile As Boolean)
                FRN = lFRN
                ParentFRN = lParentFSN
                FileName = sFileName
                IsFile = bIsFile
            End Sub

        End Class

        Private Function OpenVolume(ByVal szDriveLetter As String) As IntPtr

            Dim hCJ As IntPtr '// volume handle

            m_DriveLetter = szDriveLetter

            hCJ = CreateFile("\\.\" & szDriveLetter, GENERIC_READ, _
                             FILE_SHARE_READ Or FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero)

            Return hCJ

        End Function

        Private Sub Cleanup()

            If m_hCJ <> 0 Then
                ' Close the volume handle.
                CloseHandle(m_hCJ)
                m_hCJ = INVALID_HANDLE_VALUE
            End If

            If m_Buffer <> 0 Then
                ' Free the allocated memory
                Marshal.FreeHGlobal(m_Buffer)
                m_Buffer = IntPtr.Zero
            End If

        End Sub

        Public Sub FindAllFiles(ByVal szDriveLetter As String, fFileFound As FileFound_Delegate, fProgress As Progress_Delegate, fMatch As IsMatch_Delegate)

            Dim usnRecord As USN_RECORD
            Dim mft As MFT_ENUM_DATA
            Dim dwRetBytes As Integer
            Dim cb As Integer
            Dim dicFRNLookup As New Dictionary(Of Long, FSNode)
            Dim bIsFile As Boolean

            ' This shouldn't be called more than once.
            If m_Buffer.ToInt32 <> 0 Then
                Console.WriteLine("invalid buffer")
                Exit Sub
            End If

            ' progress 
            If Not IsNothing(fProgress) Then fProgress.Invoke("Building file list.")

            ' Assign buffer size
            m_BufferSize = 65536 '64KB

            ' Allocate a buffer to use for reading records.
            m_Buffer = Marshal.AllocHGlobal(m_BufferSize)

            ' correct path
            szDriveLetter = szDriveLetter.TrimEnd("\"c)

            ' Open the volume handle 
            m_hCJ = OpenVolume(szDriveLetter)

            ' Check if the volume handle is valid.
            If m_hCJ = INVALID_HANDLE_VALUE Then
                Console.WriteLine("Couldn't open handle to the volume.")
                Cleanup()
                Exit Sub
            End If

            mft.StartFileReferenceNumber = 0
            mft.LowUsn = 0
            mft.HighUsn = Long.MaxValue

            Do
                If DeviceIoControl(m_hCJ, FSCTL_ENUM_USN_DATA, mft, Marshal.SizeOf(mft), m_Buffer, m_BufferSize, dwRetBytes, IntPtr.Zero) Then
                    cb = dwRetBytes
                    ' Pointer to the first record
                    Dim pUsnRecord As New IntPtr(m_Buffer.ToInt32() + 8)

                    While (dwRetBytes > 8)
                        ' Copy pointer to USN_RECORD structure.
                        usnRecord = Marshal.PtrToStructure(pUsnRecord, usnRecord.GetType)

                        ' The filename within the USN_RECORD.
                        Dim FileName As String = Marshal.PtrToStringUni(New IntPtr(pUsnRecord.ToInt32() + usnRecord.FileNameOffset), usnRecord.FileNameLength / 2)

                        'If Not FileName.StartsWith("$") Then
                        ' use a delegate to determine if this file even matches our criteria
                        Dim bIsMatch As Boolean = True
                        If Not IsNothing(fMatch) Then fMatch.Invoke(FileName, usnRecord.FileAttributes, bIsMatch)

                        If bIsMatch Then
                            bIsFile = Not usnRecord.FileAttributes.HasFlag(FileAttribute.Directory)
                            dicFRNLookup.Add(usnRecord.FileReferenceNumber, New FSNode(usnRecord.FileReferenceNumber, usnRecord.ParentFileReferenceNumber, FileName, bIsFile))
                        End If
                        'End If

                        ' Pointer to the next record in the buffer.
                        pUsnRecord = New IntPtr(pUsnRecord.ToInt32() + usnRecord.RecordLength)

                        dwRetBytes -= usnRecord.RecordLength
                    End While

                    ' The first 8 bytes is always the start of the next USN.
                    mft.StartFileReferenceNumber = Marshal.ReadInt64(m_Buffer, 0)

                Else

                    Exit Do

                End If

            Loop Until cb <= 8

            If Not IsNothing(fProgress) Then fProgress.Invoke("Parsing file names.")

            ' Resolve all paths for Files
            For Each oFSNode As FSNode In dicFRNLookup.Values.Where(Function(o) o.IsFile)
                Dim sFullPath As String = oFSNode.FileName
                Dim oParentFSNode As FSNode = oFSNode

                While dicFRNLookup.TryGetValue(oParentFSNode.ParentFRN, oParentFSNode)
                    sFullPath = String.Concat(oParentFSNode.FileName, "\", sFullPath)
                End While
                sFullPath = String.Concat(szDriveLetter, "\", sFullPath)

                If Not IsNothing(fFileFound) Then fFileFound.Invoke(sFullPath, 0)
            Next

            '// cleanup
            Cleanup()
            If Not IsNothing(fProgress) Then fProgress.Invoke("Complete.")
        End Sub
    End Class
End Namespace
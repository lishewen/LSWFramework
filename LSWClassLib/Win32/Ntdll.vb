Imports System.Runtime.InteropServices

Namespace Win32
    Public Module Ntdll
        <DllImport("ntdll.dll", ExactSpelling:=True, SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Function NtCreateFile(ByRef FileHandle As IntPtr, ByVal DesiredAccess As Integer, ByRef ObjectAttributes As OBJECT_ATTRIBUTES, ByRef IoStatusBlock As IO_STATUS_BLOCK, ByVal AllocationSize As Integer, ByVal FileAttribs As Integer, ByVal SharedAccess As Integer, ByVal CreationDisposition As Integer, ByVal CreateOptions As Integer, ByVal EaBuffer As Integer, ByVal EaLength As Integer) As Integer
        End Function

        <DllImport("ntdll.dll", ExactSpelling:=True, SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Function NtQueryInformationFile(ByVal FileHandle As IntPtr, ByRef IoStatusBlock As IO_STATUS_BLOCK, ByVal FileInformation As IntPtr, ByVal Length As Integer, ByVal FileInformationClass As Integer) As Integer
        End Function
    End Module
End Namespace
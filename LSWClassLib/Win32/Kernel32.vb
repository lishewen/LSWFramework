Imports System.Text
Imports System.IO
Imports System.Runtime.InteropServices

Namespace Win32
    Public Module Kernel32
        Public Declare Function GetLastError Lib "kernel32.dll" () As Int32
        '声明INI配置文件读写API函数
        Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (section As String, key As String, def As String, retVal As StringBuilder, size As Integer, filePath As String) As Int32
        Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Int32

        <DllImport("kernel32.dll")> _
        Public Function OpenProcess(ByVal dwDesiredAccess As UInteger, <MarshalAs(UnmanagedType.Bool)> ByVal bInheritHandle As Boolean, ByVal dwProcessId As Integer) As IntPtr
        End Function

        Public Declare Auto Function CloseHandle Lib "kernel32.dll" (ByVal hObject As IntPtr) As Boolean
        Public Declare Function CloseHandle Lib "kernel32" Alias "CloseHandle" (ByVal hObject As Integer) As Integer

        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Function WriteProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer As Byte(), ByVal nSize As System.UInt32, <Out()> ByRef lpNumberOfBytesWritten As IntPtr) As Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Function ReadProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, <Out()> ByVal lpBuffer() As Byte, ByVal dwSize As Integer, ByRef lpNumberOfBytesRead As Integer) As Boolean
        End Function

        <DllImport("kernel32.dll", ExactSpelling:=True, SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Function DeviceIoControl(ByVal hDevice As IntPtr, ByVal dwIoControlCode As Integer, ByRef lpInBuffer As MFT_ENUM_DATA, ByVal nInBufferSize As Integer, ByVal lpOutBuffer As IntPtr, ByVal nOutBufferSize As Integer, ByRef lpBytesReturned As Integer, ByVal lpOverlapped As IntPtr) As Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Function CreateFile(ByVal lpFileName As String, ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, ByVal lpSecurityAttributes As IntPtr, ByVal dwCreationDisposition As Integer, ByVal dwFlagsAndAttributes As Integer, ByVal hTemplateFile As IntPtr) As IntPtr
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Function VirtualAllocEx(ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As IntPtr, ByVal flAllocationType As AllocationType, ByVal flProtect As MemoryProtection) As IntPtr
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Function VirtualFreeEx(ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As Integer, ByVal dwFreeType As AllocationType) As Boolean
        End Function

        Public Property Section As String = "Default"
        Public ReadOnly Property FileName As String
            Get
                Dim fi As New FileInfo(Process.GetCurrentProcess.MainModule.FileName)
                Return Path.Combine(fi.DirectoryName, "config.ini")
            End Get
        End Property

        '定义读取配置文件函数
        Public Function GetINI(Section As String, ByVal Key As String, FileName As String) As String
            Dim temp As New StringBuilder(500)
            Dim i = GetPrivateProfileString(Section, Key, "", temp, 500, FileName)
            Return temp.ToString()
        End Function

        Public Function GetINI(ByVal Key As String) As String
            Return GetINI(Section, Key, FileName)
        End Function
        '定义写入配置文件函数
        Public Function WriteINI(Section As String, ByVal Key As String, ByVal Value As String, FileName As String) As Long
            Return WritePrivateProfileString(Section, Key, Value, FileName)
        End Function

        Public Function WriteINI(ByVal Key As String, ByVal Value As String) As Long
            Return WriteINI(Section, Key, Value, FileName)
        End Function
    End Module
End Namespace

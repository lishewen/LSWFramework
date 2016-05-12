Imports System.Runtime.InteropServices

Namespace Win32
    Public Module Shell32
        <DllImport("Shell32.dll")> _
        Public Function ExtractIconEx(libName As String, iconIndex As Integer, largeIcon As IntPtr(), smallIcon As IntPtr(), nIcons As Integer) As Integer
        End Function
    End Module
End Namespace
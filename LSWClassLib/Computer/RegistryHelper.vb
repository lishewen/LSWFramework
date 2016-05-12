Imports Microsoft.Win32

Namespace Computer
    Public Module RegistryHelper
        Public Sub RegFileExtension(ext As String, path As String)
            Registry.SetValue("HKEY_CURRENT_USER\Software\Classes\" & ext & "\shell\open\command", "", path & " %1")
        End Sub
    End Module
End Namespace
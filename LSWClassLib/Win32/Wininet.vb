Imports System.Runtime.InteropServices
Imports System.Text
Imports Microsoft.Win32

Namespace Win32
    Public Module Wininet
        ''' <summary> 
        ''' 设置cookie 
        ''' </summary> 
        <DllImport("wininet.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Public Function InternetSetCookie(ByVal lpszUrlName As String, ByVal lbszCookieName As String, ByVal lpszCookieData As String) As Boolean
        End Function

        ''' <summary> 
        ''' 获取cookie 
        ''' </summary> 
        <DllImport("wininet.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Public Function InternetGetCookie(ByVal url As String, ByVal name As String, ByVal data As StringBuilder, ByRef dataSize As Integer) As Boolean
        End Function

        <DllImport("wininet.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
        Public Function InternetSetOption(hInternet As IntPtr, dwOption As Integer, lpBuffer As IntPtr, dwBufferLength As Integer) As Boolean
        End Function

        <DllImport("wininet.dll", CharSet:=CharSet.Ansi, SetLastError:=True)> _
        Public Function InternetSetOption(ByVal hInternet As IntPtr, ByVal [Option] As Integer, ByRef OptionList As ProxyOptionList, ByVal size As Integer) As Boolean
        End Function

        Public Sub SetIEMainPage(url As String)
            Dim rkey As RegistryKey = Registry.CurrentUser
            Dim rkeySubKey As RegistryKey = rkey.OpenSubKey("Software\Microsoft\Internet Explorer\Main", True)
            Dim iemainurl As String = rkeySubKey.GetValue("Start Page")
            If iemainurl <> url Then
                rkeySubKey.SetValue("Start Page", url)
            End If
            rkeySubKey.Close()
        End Sub

        Public Function GetIEMainPage() As String
            Dim rkey As RegistryKey = Registry.CurrentUser
            Dim rkeySubKey As RegistryKey = rkey.OpenSubKey("Software\Microsoft\Internet Explorer\Main", True)
            Dim iemainurl As String = rkeySubKey.GetValue("Start Page")
            rkeySubKey.Close()
            Return iemainurl
        End Function
    End Module

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure ProxyOptionList
        Public Size As Integer
        Public Connection As String
        Public OptionCount As Integer
        Public OptionError As Integer
        Public pOptions As IntPtr
    End Structure
End Namespace
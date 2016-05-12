Imports System.Runtime.InteropServices

Namespace Win32
    Public Module ClipboardAPI
        <DllImport("user32.dll", EntryPoint:="OpenClipboard", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
        Public Function OpenClipboard(ByVal hWnd As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll", EntryPoint:="EmptyClipboard", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
        Public Function EmptyClipboard() As Boolean
        End Function

        <DllImport("user32.dll", EntryPoint:="SetClipboardData", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
        Public Function SetClipboardData(ByVal uFormat As Integer, ByVal ByValhWnd As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", EntryPoint:="CloseClipboard", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
        Public Function CloseClipboard() As Boolean
        End Function

        <DllImport("user32.dll", EntryPoint:="GetClipboardData", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
        Public Function GetClipboardData(ByVal uFormat As Integer) As IntPtr
        End Function

        <DllImport("user32.dll", EntryPoint:="IsClipboardFormatAvailable", SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
        Public Function IsClipboardFormatAvailable(ByVal uFormat As Integer) As Short
        End Function

        Dim ClipboardViewerNext As IntPtr
        '声明相关Win32API
        <DllImport("User32.dll", CharSet:=CharSet.Auto)> _
        Public Function SetClipboardViewer(hWnd As IntPtr) As IntPtr
        End Function

        <DllImport("User32.dll", CharSet:=CharSet.Auto)> _
        Public Function ChangeClipboardChain(hWndRemove As IntPtr, hWndNewNext As IntPtr) As Boolean
        End Function

        ''' <summary>
        ''' 注册事件
        ''' </summary>
        ''' <param name="handle">句柄</param>
        ''' <remarks></remarks>
        Public Sub RegisterClipboardViewer(handle As IntPtr)
            ClipboardViewerNext = SetClipboardViewer(handle)
        End Sub
        ''' <summary>
        ''' 注销事件
        ''' </summary>
        ''' <param name="handle">句柄</param>
        ''' <remarks></remarks>
        Public Sub UnregisterClipboardViewer(handle As IntPtr)
            ChangeClipboardChain(handle, ClipboardViewerNext)
        End Sub
    End Module
End Namespace
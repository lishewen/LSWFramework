Imports System.Runtime.InteropServices
Imports System.Drawing
Imports LSW.Computer
Imports System.Windows.Forms

Namespace Win32
    Public Module User32
        Public destHandle As IntPtr = IntPtr.Zero

        Public Const WM_PASTE As Int32 = &H302
        Public Const WM_SETTEXT As Int32 = &HC
        Public Const WM_KEYDOWN As Int32 = &H100
        Public Const WM_KEYUP As Int32 = &H101
        Public Const WM_LBUTTONDOWN As Int32 = &H201
        Public Const WM_LBUTTONUP As Int32 = &H202

        Public Const WM_SHOWWINDOW As Int32 = &H18
        Public Const SW_NORMAL As Int32 = 1

        Public Const SW_SHOWNOACTIVATE As Int32 = 4

        Public Const SW_HIDE As Int32 = 0
        Public Const SW_SHOW As Int32 = 5

        Public Const SC_CLOSE As Integer = &HF060        '定义关闭按钮对应的消息值
        Public Const MF_ENABLED As Integer = &H0        '禁用
        Public Const MF_GRAYED As Integer = &H1        '变灰
        Public Const MF_DISABLED As Integer = &H2        '禁用
        Public Const WM_SYSCOMMAND As Integer = &H112        ' 定义要截获的消息类型

        <DllImport("user32.dll", EntryPoint:="GetSystemMenu")> _
        Public Function GetSystemMenu(hWnd As IntPtr, bRevert As Integer) As IntPtr
        End Function

        <DllImport("User32.dll")> _
        Public Function EnableMenuItem(hMenu As IntPtr, uIDEnableItem As Integer, uEnable As Integer) As Boolean
        End Function

        Public Declare Function GetCursorPos Lib "user32" (ByRef lpPoint As POINTAPI) As Integer
        Public Declare Function GetWindowDC Lib "user32" (ByVal hwnd As Integer) As Integer
        Public Declare Function FindWindowEx Lib "user32.dll" Alias "FindWindowExA" (ByVal hWnd1 As IntPtr, ByVal hWnd2 As IntPtr, ByVal lpsz1 As String, ByVal lpsz2 As String) As IntPtr
        Public Declare Function GetWindowText Lib "user32.dll" Alias "GetWindowTextA" (ByVal hwnd As Int32, ByVal lpString As String, ByVal cch As Int32) As Int32
        Public Declare Function FindWindow Lib "user32.dll" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
        Public Declare Function SendMessage Lib "user32.dll" Alias "SendMessageA" (ByVal hwnd As Int32, ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As Int32) As Int32
        Public Declare Function SendMessage Lib "user32.dll" Alias "SendMessageA" (ByVal hwnd As Int32, ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As String) As Int32
        Public Declare Function ShowWindow Lib "user32.dll" (ByVal hwnd As Int32, ByVal nCmdShow As Int32) As Int32
        Public Declare Function SetWindowText Lib "user32.dll" Alias "SetWindowTextA" (ByVal hwnd As Int32, ByVal lpString As String) As Int32

        <DllImport("user32.dll")> _
        Public Function SendMessageW(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As IntPtr
        End Function

        Public Declare Auto Function SendMessage Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
        Public Declare Auto Function SendMessage Lib "user32.dll" Alias "SendMessageA" (ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As IntPtr

        <DllImport("user32.dll", EntryPoint:="EnumChildWindows")> _
        Public Function EnumChildWindows(hwndParent As IntPtr, EnumFunc As EnumChildProc, lParam As IntPtr) As Boolean
        End Function

        Public Delegate Function EnumChildProc(hwnd As IntPtr, lParam As IntPtr) As Boolean

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Function RegisterWindowMessage(lpString As String) As UInteger
        End Function

        <DllImport("oleacc.dll", PreserveSig:=False)> _
        Public Function ObjectFromLresult(lResult As UIntPtr, <MarshalAs(UnmanagedType.LPStruct)> refiid As Guid, wParam As IntPtr) As <MarshalAs(UnmanagedType.[Interface])> Object
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Public Function SendMessageTimeout(hWnd As IntPtr, Msg As UInteger, wParam As UIntPtr, lParam As IntPtr, fuFlags As SendMessageTimeoutFlags, uTimeout As UInteger, ByRef lpdwResult As UIntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", CharSet:=CharSet.Auto)> _
        Public Function MessageBeep(type As UInteger) As Boolean
        End Function

        ' Methods
        <DllImport("user32.dll")> _
        Public Function GetCursorPos(ByVal lpPoint As Point) As Integer
        End Function

        <DllImport("user32.dll")> _
        Public Function GetDoubleClickTime() As Integer
        End Function

        <DllImport("user32.dll")> _
        Public Sub mouse_event(ByVal flags As MouseEventFlag, ByVal dx As Integer, ByVal dy As Integer, ByVal data As UInt32, ByVal extraInfo As UIntPtr)
        End Sub

        <DllImport("user32.dll")> _
        Public Function SetCursorPos(ByVal x As Integer, ByVal y As Integer) As Integer
        End Function

        Public Function FindChildClassHwnd(hwndParent As IntPtr, lParam As IntPtr) As Boolean
            Dim hwnd = FindWindowEx(hwndParent, IntPtr.Zero, "Internet Explorer_Server", Nothing)
            If hwnd <> IntPtr.Zero Then
                'pfw->m_hWnd= hwnd;        // found: save it
                destHandle = hwnd
                ' stop enumerating
                Return False
            End If

            EnumChildWindows(hwndParent, AddressOf FindChildClassHwnd, lParam)
            ' recurse
            '每段枚举子窗口，递归调用
            Return True
        End Function

        ''' <summary>
        ''' 根据窗体标题查找窗口句柄（支持模糊匹配）
        ''' </summary>
        ''' <param name="title"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FindWindow(title As String) As IntPtr
            Dim ps As Process() = Process.GetProcesses()
            For Each p As Process In ps
                If p.MainWindowTitle.IndexOf(title) <> -1 Then
                    Return p.MainWindowHandle
                End If
            Next
            Return IntPtr.Zero
        End Function

        <DllImport("user32.dll")> _
        Public Function GetDC(ByVal hwnd As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll", ExactSpelling:=True, SetLastError:=True)>
        Public Function UpdateLayeredWindow(hWnd As IntPtr, hdcDst As IntPtr, ByRef pptDst As Point, ByRef psize As Size, hdcSrc As IntPtr, ByRef ppSrc As Point, crKey As Int32, ByRef pblend As BLENDFUNCTION, dwFlags As Int32) As Boolean
        End Function

        <DllImport("user32.dll")> _
        Public Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As Integer
        End Function

        <DllImport("user32.dll")> _
        Public Function ReleaseCapture() As Boolean
        End Function

        <DllImport("user32.dll")> _
        Public Function SetWindowRgn(ByVal hWnd As IntPtr, ByVal hRgn As Integer, ByVal bRedraw As Boolean) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True)> _
        Public Function SetForegroundWindow(ByVal hWnd As IntPtr) As Boolean
        End Function

        ''' <summary>
        ''' 销毁图标
        ''' </summary>
        ''' <param name="hIcon"></param>
        ''' <returns></returns>
        <DllImport("user32.dll")> _
        Public Function DestroyIcon(hIcon As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll")> _
        Public Function GetWindowRect(hWnd As IntPtr, ByRef rect As RECT) As IntPtr
        End Function
        ''' <summary>
        ''' 禁用窗体的关闭按钮
        ''' </summary>
        ''' <param name="f"></param>
        ''' <remarks></remarks>
        Public Sub DisableFormCloseButton(f As Form)
            Dim hMenu = GetSystemMenu(f.Handle, 0)
            EnableMenuItem(hMenu, SC_CLOSE, (MF_DISABLED + MF_GRAYED) Or MF_ENABLED)
        End Sub
    End Module

    Public Enum SendMessageTimeoutFlags As UInteger
        SMTO_NORMAL = &H0
        SMTO_BLOCK = &H1
        SMTO_ABORTIFHUNG = &H2
        SMTO_NOTIMEOUTIFNOTHUNG = &H8
    End Enum

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure RECT
        Public left As Integer
        Public top As Integer
        Public right As Integer
        Public bottom As Integer
    End Structure
End Namespace

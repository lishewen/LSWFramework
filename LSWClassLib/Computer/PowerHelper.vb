Imports System.Windows.Forms
Imports LSW.Win32

Namespace Computer
    Public Module PowerHelper
        Private Const SC_MONITORPOWER As Integer = &HF170
        Private Const WM_SYSCOMMAND As Integer = &H112

        ''' <summary>
        ''' 休眠
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Hibernate()
            Application.SetSuspendState(PowerState.Hibernate, True, True)
        End Sub
        ''' <summary>
        ''' 注销
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Logout()
            Process.Start("shutdown.exe", "-l")
        End Sub
        ''' <summary>
        ''' 关闭显示器
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub MonitorOff()
            User32.SendMessageW(IntPtr.Zero, WM_SYSCOMMAND, SC_MONITORPOWER, 2)
        End Sub
        ''' <summary>
        ''' 打开显示器
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub MonitorOn()
            User32.SendMessageW(IntPtr.Zero, WM_SYSCOMMAND, SC_MONITORPOWER, -1)
        End Sub
        ''' <summary>
        ''' 重启
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Restart()
            Process.Start("shutdown.exe", "-r -t 0")
        End Sub
        ''' <summary>
        ''' 关机
        ''' </summary>
        ''' <param name="sec"></param>
        ''' <remarks></remarks>
        Public Sub Shutdown(ByVal sec As Integer)
            Process.Start("shutdown.exe", ("-s -t " & sec))
        End Sub
        ''' <summary>
        ''' 睡眠
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Sleep()
            Application.SetSuspendState(PowerState.Suspend, True, True)
        End Sub
    End Module
End Namespace
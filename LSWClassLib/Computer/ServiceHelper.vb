Imports System.ServiceProcess

Namespace Computer
    Public Module ServiceHelper
        ''' <summary>
        ''' 启动服务
        ''' </summary>
        ''' <param name="name">服务名</param>
        Public Sub Start(name As String)
            Dim s As New ServiceController(name)
            If s.Status = ServiceControllerStatus.Stopped Then
                s.Start()
                s.WaitForStatus(ServiceControllerStatus.Running)
                s.Close()
            End If
        End Sub

        ''' <summary>
        ''' 停止服务
        ''' </summary>
        ''' <param name="name">服务名</param>
        Public Sub [Stop](name As String)
            Dim s As New ServiceController(name)
            If s.Status = ServiceControllerStatus.Running Then
                s.Stop()
                s.WaitForStatus(ServiceControllerStatus.Stopped)
                s.Close()
            End If
        End Sub

        ''' <summary>
        ''' 重启服务
        ''' </summary>
        ''' <param name="name">服务名</param>
        Public Sub Reset(name As String)
            Dim s As New ServiceController(name)
            If s.Status = ServiceControllerStatus.Running OrElse s.Status = ServiceControllerStatus.Stopped Then
                s.Stop()
                s.WaitForStatus(ServiceControllerStatus.Stopped)
                s.Start()
                s.WaitForStatus(ServiceControllerStatus.Running)
                s.Close()
            End If
        End Sub
    End Module
End Namespace
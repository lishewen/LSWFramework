Namespace CMD
    Public Module CMDHelper
        Delegate Sub dReadLine(ByVal strLine As String)

        ''' <summary>
        ''' 执行CMD命令
        ''' Example:excuteCommand("ipconfig", "/all", AddressOf PrintMessage)
        ''' </summary>
        ''' <param name="strFile">命令</param>
        ''' <param name="args">参数</param>
        ''' <param name="onReadLine">行信息（委托）</param>
        ''' <remarks></remarks>
        Public Sub excuteCommand(ByVal strFile As String, ByVal args As String, ByVal onReadLine As dReadLine)
            Dim p As New Process
            p.StartInfo = New ProcessStartInfo
            p.StartInfo.FileName = strFile
            p.StartInfo.Arguments = args
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            p.StartInfo.RedirectStandardOutput = True
            p.StartInfo.UseShellExecute = False
            p.StartInfo.CreateNoWindow = True
            p.Start()
            Dim reader = p.StandardOutput
            Dim line = reader.ReadLine
            While Not reader.EndOfStream
                onReadLine(line)
                line = reader.ReadLine()
            End While
            p.WaitForExit()
        End Sub
    End Module
End Namespace
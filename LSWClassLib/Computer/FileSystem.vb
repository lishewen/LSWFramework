Imports System.Text.RegularExpressions

Namespace Computer
    Public Module FileSystem
#Region "根据正则表达式脚本删除文件或目录"
        ''' <summary>
        ''' 删除目标
        ''' </summary>
        ''' <param name="scriptFile">脚本文件</param>
        ''' <param name="targetDirectory">目标目录</param>
        Public Sub DeleteTarget(scriptFile As String, targetDirectory As String)
            If Not System.IO.File.Exists(scriptFile) OrElse Not System.IO.Directory.Exists(targetDirectory) Then
                Return
            End If
            Dim lines = System.IO.File.ReadAllLines(scriptFile)
            Dim regs = New Regex(lines.Length - 1) {}
            For i As Integer = 0 To lines.Length - 1
                If lines(i) IsNot Nothing Then
                    regs(i) = New Regex(lines(i), RegexOptions.IgnoreCase)
                End If
            Next

            'search all files
            SearchDirectory(targetDirectory, regs)
        End Sub

        ''' <summary>
        ''' 搜索目录并删除文件
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="regs"></param>
        Public Sub SearchDirectory(path As String, regs As Regex())
            If ValidateFileName(path, regs) Then
                Try
                    System.IO.Directory.Delete(path, True)
                Catch generatedExceptionName As Exception
                    Return
                End Try

                Return
            End If

            Dim directories() As String, files() As String
            Try
                directories = System.IO.Directory.GetDirectories(path)
                files = System.IO.Directory.GetFiles(path)
            Catch generatedExceptionName As Exception
                Return
            End Try

            For Each d In directories
                SearchDirectory(d, regs)
            Next

            For Each f In files
                If Not ValidateFileName(f, regs) Then
                    Continue For
                End If
                Try
                    System.IO.File.Delete(f)
                Catch generatedExceptionName As Exception
                End Try
            Next
        End Sub

        ''' <summary>
        ''' 确定一个路径是否符合规则
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="regs"></param>
        ''' <returns></returns>
        Public Function ValidateFileName(path As String, regs() As Regex) As Boolean
            For Each r In regs
                If r.IsMatch(path) Then
                    Return True
                End If
            Next
            Return False
        End Function
#End Region

        ''' <summary>
        ''' 延迟删除程序
        ''' </summary>
        Public Sub DelayDeleteFile(pid As Integer, path As String)
            Dim plist = New List(Of Process)()
            Try
                Dim process = System.Diagnostics.Process.GetProcessById(pid)
                If process IsNot Nothing AndAlso Not process.HasExited Then
                    plist.Add(process)
                End If
            Catch generatedExceptionName As Exception
            End Try
            '查找所有指定目录中运行的进程
            Dim allproc = System.Diagnostics.Process.GetProcesses()
            For Each p In allproc
                Try
                    Dim mpath = p.MainModule.FileName
                    If mpath.IndexOf(path, StringComparison.OrdinalIgnoreCase) <> -1 Then
                        plist.Add(p)
                    End If

                Catch generatedExceptionName As Exception
                End Try
            Next
            '等待结束
            For Each p In plist
                If Not p.HasExited Then
                    p.WaitForExit()
                End If
            Next

            Try
                If System.IO.Directory.Exists(path) Then
                    System.IO.Directory.Delete(path, True)
                End If
            Catch generatedExceptionName As Exception
            End Try
        End Sub

    End Module
End Namespace
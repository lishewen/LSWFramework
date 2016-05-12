Imports System.IO
Imports LSW.Exceptions
Imports System.Runtime.Remoting.Lifetime
Imports System.Reflection

Namespace Common.DynamicLoad
    Public Class AssemblyLoader
        Implements IDisposable

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)。
                    _caches.Clear()

                    For Each o In _caches.Keys
                        Unload(o)
                    Next
                End If

                ' TODO: 释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO: 将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: 仅当上面的 Dispose(ByVal disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。请将清理代码放入上面的 Dispose(ByVal disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Private ReadOnly _lockThis As New Object
        Private _caches As New Dictionary(Of String, Queue(Of InvokerInfo))

        ''' <summary>
        ''' 加载所有可用的程序集
        ''' </summary>
        ''' <param name="dlls"></param>
        ''' <remarks></remarks>
        Public Sub LoadAssemblys(dlls As Dictionary(Of String, String))
            For Each kvp In dlls
                LoadAssembly(kvp.Key, kvp.Value)
            Next
        End Sub

        ''' <summary>
        ''' 加载指定的程序集
        ''' </summary>
        ''' <param name="dllName"></param>
        ''' <param name="typeName"></param>
        ''' <remarks></remarks>
        Private Sub LoadAssembly(dllName As String, typeName As String)
            Dim result As Queue(Of InvokerInfo) = Nothing
            _caches.TryGetValue(dllName, result)

            If result Is Nothing OrElse result.Count = 0 Then
                Dim info As New FileInfo(AppDomain.CurrentDomain.BaseDirectory & "LswDlls\" & dllName)

                If Not info.Exists Then Throw New LSWFrameworkException(New Exception(AppDomain.CurrentDomain.BaseDirectory & "LswDlls\" & dllName & " not exist"))

                CacheMethodInvoker(dllName, typeName, info.LastWriteTime)
            End If
        End Sub

        ''' <summary>
        ''' 卸载程序集
        ''' </summary>
        ''' <param name="dllName"></param>
        ''' <remarks></remarks>
        Public Sub Unload(dllName As String)
            Dim info = _caches(dllName).Dequeue
            AppDomain.Unload(info.Domain)
        End Sub

        ''' <summary>
        ''' 调用指定程序集中指定的方法
        ''' </summary>
        ''' <param name="dllName">程序集名称</param>
        ''' <param name="methodName">方法名称</param>
        ''' <param name="methodParams">参数数组</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InvokeMethod(dllName As String, methodName As String, Optional methodParams() As Object = Nothing)
            Dim result
            Dim info As InvokerInfo
            SyncLock _lockThis
                info = GetInvoker(dllName)
                info.Ref += 1
            End SyncLock

            result = info.Invoker.InvokeMethod(methodName, methodParams)

            SyncLock _lockThis
                info.Ref -= 1
                TryToUnLoad(dllName, info)
            End SyncLock

            Return result
        End Function

        ''' <summary>
        ''' 获取指定程序集的调用信息
        ''' </summary>
        ''' <param name="dllName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetInvoker(dllName As String) As InvokerInfo
            Dim result As Queue(Of InvokerInfo) = Nothing
            _caches.TryGetValue(dllName, result)

            If result Is Nothing Then
                Throw New LSWFrameworkException(New Exception(dllName + " not loaded"))
            End If

            Dim info As New FileInfo(AppDomain.CurrentDomain.BaseDirectory + "LswDlls\" + dllName)

            If Not info.Exists Then
                Return result.ToArray()(result.Count - 1)
            End If

            If info.LastWriteTime > result.ToArray()(result.Count - 1).LastWriteTime Then
                Return CacheMethodInvoker(dllName, result.Peek.TypeName, info.LastWriteTime)
            End If

            Return result.ToArray()(result.Count - 1)
        End Function

        ''' <summary>
        ''' 尝试卸载程序集
        ''' </summary>
        ''' <param name="dllName"></param>
        ''' <param name="currentInfo"></param>
        ''' <remarks></remarks>
        Private Sub TryToUnLoad(dllName As String, currentInfo As InvokerInfo)
            Dim info = _caches(dllName).Peek
            If info Is currentInfo Then
                Return
            End If

            If info.Ref = 0 Then
                Unload(dllName)
            End If
        End Sub

        ''' <summary>
        ''' 缓存指定的方法调用信息
        ''' </summary>
        ''' <param name="dllName"></param>
        ''' <param name="typeName"></param>
        ''' <param name="lastWriteTime"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CacheMethodInvoker(dllName As String, typeName As String, lastWriteTime As Date) As InvokerInfo
            Dim invoker As MethodInvoker

            Dim invokeinfo As New InvokerInfo

            Dim setup As New AppDomainSetup With {
                .ShadowCopyFiles = "true",
                .ShadowCopyDirectories = AppDomain.CurrentDomain.BaseDirectory & "LswDlls\",
                .ConfigurationFile = "LswDynamicLoad.exe.config",
                .ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            }

            Dim domain = AppDomain.CreateDomain(dllName, Nothing, setup)

            domain.DoCallBack(Sub() LifetimeServices.LeaseTime = TimeSpan.Zero)

            invokeinfo.Domain = domain
            invokeinfo.LastWriteTime = lastWriteTime
            invokeinfo.TypeName = typeName

            Dim bindings = BindingFlags.CreateInstance Or BindingFlags.Instance Or BindingFlags.Public
            Dim para() = New Object() {setup.ShadowCopyDirectories & "\" & dllName, typeName}
            Try
                invoker = CType(domain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly.CodeBase.Substring(8), GetType(MethodInvoker).FullName, True, bindings, Nothing, para, Nothing, Nothing), MethodInvoker)
            Catch ex As Exception
                Throw New LSWFrameworkException(ex)
            End Try

            If invoker Is Nothing Then
                Throw New LSWFrameworkException(New Exception("Can't find type " + GetType(MethodInvoker).FullName + " from " + Assembly.GetExecutingAssembly().CodeBase))
            End If

            Try
                invoker.LoadAllMethods()
            Catch ex As Exception
                Throw New LSWFrameworkException(ex)
            End Try

            invokeinfo.Invoker = invoker
            invokeinfo.Ref = 0

            If _caches.Keys.Contains(dllName) Then
                _caches(dllName).Enqueue(invokeinfo)
            Else
                Dim q As New Queue(Of InvokerInfo)
                q.Enqueue(invokeinfo)
                _caches(dllName) = q
            End If

            Return invokeinfo
        End Function

    End Class
End Namespace
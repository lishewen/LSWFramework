Imports System.Threading

Namespace Common
    Public NotInheritable Class CacheContainer
        '互斥锁
        Private SyncRoot As New Object()
        '对象字典
        Private dic As New Dictionary(Of String, CacheObj)()

        Friend Class CacheObj
            Implements IDisposable
            '公有变量
            Public Property Value As Object
            Public Property Expired As Integer
            Public Property callback As WaitCallback
            Public ar As New AutoResetEvent(False)
            Public ReadOnly Property Timeout() As TimeSpan
                Get
                    Return TimeSpan.FromMinutes(Expired)
                End Get
            End Property
            Public Sub New(value As Object, Optional expired As Integer = 10)
                Me.Value = value
                Me.Expired = expired
                callback = New WaitCallback(Sub(obj)
                                                Console.WriteLine("{0}:已过期,过期时间:{1}", obj, Now)
                                            End Sub)
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                GC.SuppressFinalize(Me)
            End Sub

            Protected Overrides Sub Finalize()
                Try
                    Dispose()
                Finally
                    MyBase.Finalize()
                End Try
            End Sub
        End Class

        Private Shared container As New CacheContainer()

        Public Shared Function GetInstance() As CacheContainer
            Return container
        End Function

        Default Public Property Item(key As String) As Object
            Get
                '访问变量成功，则时间重新计时
                Dim cache As CacheObj = Nothing
                SyncLock SyncRoot
                    If dic.TryGetValue(key, cache) Then
                        cache.ar.Set()
                        Return cache.Value
                    End If
                End SyncLock
                Return Nothing
            End Get
            Set(value As Object)
                '通过属性添加参数，有则覆盖，无则添加，操作完毕重新计时
                Dim cache As CacheObj = Nothing
                SyncLock SyncRoot
                    If dic.TryGetValue(key, cache) Then
                        cache.Value = value
                        cache.ar.Set()
                    Else
                        Add(key, value)
                    End If
                End SyncLock
            End Set
        End Property

        Public Sub Add(key As String, value As Object)
            SyncLock SyncRoot
                dic.Add(key, New CacheObj(value))
            End SyncLock
            AutoCheck(key)
        End Sub

        Public Sub Add(key As String, value As Object, expired As Integer)
            SyncLock SyncRoot
                dic.Add(key, New CacheObj(value, expired))
            End SyncLock
            AutoCheck(key)
        End Sub

        Public Sub Add(key As String, value As Object, expired As Integer, callback As WaitCallback)
            SyncLock SyncRoot
                dic.Add(key, New CacheObj(value, expired) With {
                     .callback = callback
                })
            End SyncLock
            AutoCheck(key)
        End Sub

        Private Sub AutoCheck(key As String)
            '开启一个子线程去控制变量的过期
            ThreadPool.QueueUserWorkItem(New WaitCallback(Sub(obj)
                                                              Dim tmpCache As CacheObj
                                                              While True
                                                                  '从字典中取出对象
                                                                  SyncLock SyncRoot
                                                                      tmpCache = dic(key)
                                                                  End SyncLock
                                                                  '打印变量过期时间
                                                                  Console.WriteLine("{0} 等待销毁变量 时间为:{1}秒", DateTime.Now, tmpCache.Expired)
                                                                  '记录开始时间
                                                                  Dim timeStart = Now
                                                                  '中断，超时时间一到，自动向下执行
                                                                  tmpCache.ar.WaitOne(TimeSpan.FromMinutes(tmpCache.Expired))
                                                                  '检查时间是否已经达到超时时间，超时则移除该信息，并触发回调
                                                                  If (Now - timeStart) >= tmpCache.Timeout Then
                                                                      SyncLock SyncRoot
                                                                          dic.Remove(key)
                                                                      End SyncLock
                                                                      If tmpCache.callback IsNot Nothing Then
                                                                          tmpCache.callback.Invoke(tmpCache.Value)
                                                                      End If
                                                                      Exit While
                                                                  End If
                                                              End While
                                                          End Sub))
        End Sub

        Public Sub Remove(key As String)
            SyncLock SyncRoot
                Dim cache As CacheObj = Nothing
                If dic.TryGetValue(key, cache) Then
                    cache.Expired = 0
                    cache.ar.Set()
                End If
            End SyncLock
        End Sub
    End Class
End Namespace
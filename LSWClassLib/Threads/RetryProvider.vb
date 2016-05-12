Imports System.ComponentModel
Imports LSW.Exceptions
Imports System.Threading

Namespace Threads
    Public Class RetryProvider
        Implements IDisposable

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)。
                    Cancel()
                End If

                ' TODO: 释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO: 将大型字段设置为 null。
                _backgroupThread = Nothing
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

        Private _waitTimeout As Integer = 0
        Private _retryCount As Integer = 1
        Private Shared ReadOnly _asyncLock As New Object
        Private WithEvents _backgroupThread As BackgroundWorker

        ''' <summary>
        ''' 完成事件
        ''' </summary>
        ''' <param name="success"></param>
        ''' <remarks></remarks>
        Public Event Completed(success As Boolean)
        ''' <summary>
        ''' 取消事件
        ''' </summary>
        ''' <remarks></remarks>
        Public Event Cancelled()
        ''' <summary>
        ''' 每一次重试开始操作事件
        ''' </summary>
        ''' <param name="retryIndex"></param>
        ''' <remarks></remarks>
        Public Event PerRetryBegin(retryIndex As Integer)
        ''' <summary>
        ''' 每一次重试结束操作事件
        ''' </summary>
        ''' <param name="retryIndex"></param>
        ''' <remarks></remarks>
        Public Event PerRetryEnd(retryIndex As Integer)
        ''' <summary>
        ''' 每一次重试失败事件
        ''' </summary>
        ''' <param name="ex"></param>
        ''' <remarks></remarks>
        Public Event PerRetryFailedCompleted(ex As LSWFrameworkException)
        ''' <summary>
        ''' 进度更改事件
        ''' </summary>
        ''' <param name="percent"></param>
        ''' <remarks></remarks>
        Public Event ProgressChanged(percent As Integer)

        Public ReadOnly Property IsBusy As Boolean
            Get
                Return _backgroupThread IsNot Nothing AndAlso _backgroupThread.IsBusy
            End Get
        End Property

        Public Property WaitTimeout As Integer
            Get
                Return _waitTimeout
            End Get
            Set(value As Integer)
                If value < 0 Then
                    value = 0
                End If
                _waitTimeout = value
            End Set
        End Property

        Public Property RetryCount As Integer
            Get
                Return _retryCount
            End Get
            Set(value As Integer)
                If value < 1 Then
                    value = 1
                End If
                _retryCount = value
            End Set
        End Property

        Public Sub Cancel()
            If _backgroupThread IsNot Nothing Then
                _backgroupThread.CancelAsync()
            End If
        End Sub

        Public Sub StartAsync(target As Action, Optional waitTimeout As Integer = 0, Optional retryCount As Integer = 1)
            StartAsyncRetry(target, waitTimeout, retryCount)
        End Sub

        Public Sub StartAsync(target As Func(Of Boolean), Optional waitTimeout As Integer = 0, Optional retryCount As Integer = 1)
            StartAsyncRetry(target, waitTimeout, retryCount)
        End Sub

        Private Sub StartAsyncRetry(target As Object, waitTimeout As Integer, retryCount As Integer)
            If target Is Nothing Then
                Throw New LSWFrameworkException("target")
            End If

            If _backgroupThread Is Nothing Then
                _backgroupThread = New BackgroundWorker
                _backgroupThread.WorkerSupportsCancellation = True
                _backgroupThread.WorkerReportsProgress = True
            End If

            If _backgroupThread.IsBusy Then
                Return
            End If

            Me.WaitTimeout = waitTimeout
            Me.RetryCount = retryCount

            _backgroupThread.RunWorkerAsync(target)
        End Sub

        Private Sub _backgroupThread_DoWork(sender As Object, e As DoWorkEventArgs) Handles _backgroupThread.DoWork
            Start(e.Argument)
        End Sub

        Private Sub _backgroupThread_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles _backgroupThread.ProgressChanged
            RaiseEvent ProgressChanged(e.ProgressPercentage)
        End Sub

        Private Sub Start(target As Object)
            If target Is Nothing Then
                Return
            End If

            Dim retryCount = Me.RetryCount

            SyncLock _asyncLock
                _backgroupThread.ReportProgress(5)
                While Not _backgroupThread.CancellationPending
                    RaiseEvent PerRetryBegin(Me.RetryCount - retryCount)

                    Try
                        If TypeOf target Is Action Then
                            DirectCast(target, Action).Invoke()
                            InvokeCompletedEvent(True)
                            Return
                        ElseIf DirectCast(target, Func(Of Boolean)).Invoke Then
                            InvokeCompletedEvent(True)
                            Return
                        Else
                            Throw New InvalidOperationException("Execute Failed.")
                        End If
                    Catch ex As Exception
                        RaiseEvent PerRetryFailedCompleted(New LSWFrameworkException(ex))

                        _backgroupThread.ReportProgress((Me.RetryCount - retryCount + 1) * 100 / Me.RetryCount)
                    Finally
                        RaiseEvent PerRetryEnd(Me.RetryCount - retryCount)
                    End Try

                    If Me.RetryCount > 0 Then
                        retryCount -= 1
                        If retryCount = 0 Then
                            InvokeCompletedEvent()
                            Return
                        End If
                    End If

                    Thread.Sleep(Me.WaitTimeout)
                End While

                If _backgroupThread.CancellationPending Then
                    RaiseEvent Cancelled()
                End If
            End SyncLock
        End Sub

        Private Sub InvokeCompletedEvent(Optional success As Boolean = False)
            RaiseEvent Completed(success)

            _backgroupThread.ReportProgress(100)
        End Sub
    End Class
End Namespace
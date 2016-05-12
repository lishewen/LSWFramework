Imports System.Threading

Namespace Threads
    Public Class ThreadMulti
#Region "变量"
        Public Delegate Sub DelegateComplete()
        Public Delegate Sub DelegateWork(taskindex As Integer, threadindex As Integer)

        Public Event CompleteEvent As DelegateComplete
        Public Event PercentEvent As EventHandler(Of PercentEventArgs)
        Public WorkMethod As DelegateWork

        Private _threads() As Thread
        Private _threadState() As Boolean
        Private _taskCount As Integer = 0
        Private _taskindex As Integer = 0
        Private _threadCount As Integer = 5
#End Region

        Public Sub New(taskcount As Integer)
            _taskCount = taskcount
        End Sub

        Public Sub New(taskcount As Integer, threadCount As Integer)
            _taskCount = taskcount
            _threadCount = threadCount
        End Sub

#Region "获取任务"
        Private Function GetTask() As Integer
            SyncLock Me
                If (_taskindex < _taskCount) Then
                    _taskindex += 1
                    Return _taskindex
                End If
                Return 0
            End SyncLock
        End Function
#End Region

#Region "Start"
        ''' <summary>
        ''' 启动
        ''' </summary>
        Public Sub Start()
            _taskindex = 0

            Dim num As Integer = IIf(_taskCount < _threadCount, _taskCount, _threadCount)

            _threadState = New Boolean(num - 1) {}
            _threads = New Thread(num - 1) {}

            For n = 0 To num - 1
                _threadState(n) = False
                _threads(n) = New Thread(New ParameterizedThreadStart(AddressOf Work))
                _threads(n).Start(n)
            Next
            'Parallel.For(0, num, Sub(n)
            '                         _threadState(n) = False
            '                         _threads(n) = New Thread(New ParameterizedThreadStart(AddressOf Work))
            '                         _threads(n).Start(n)
            '                     End Sub)
        End Sub

        ''' <summary>
        ''' 结束线程
        ''' </summary>
        Public Sub [Stop]()
            For i = 0 To _threads.Length - 1
                If _threads(i) IsNot Nothing Then _threads(i).Abort()
            Next
        End Sub
#End Region

#Region "Work"
        Public Sub Work(arg)
            '提取任务并执行
            Dim threadindex As Integer = CInt(arg)
            Dim taskindex As Integer = GetTask()

            While taskindex <> 0 AndAlso WorkMethod IsNot Nothing
                WorkMethod(taskindex, threadindex + 1)
                taskindex = GetTask()
            End While
            '所有的任务执行完毕
            _threadState(threadindex) = True

            '处理并发 如果有两个线程同时完成只允许一个触发complete事件
            SyncLock Me
                'For i = 0 To _threadState.Length - 1
                '    If _threadState(i) = False Then
                '        Return
                '    End If
                'Next
                RaiseEvent PercentEvent(arg, New PercentEventArgs With {.Percent = taskindex / _taskCount})

                If _threadState.Contains(False) Then Return

                '如果全部完成
                RaiseEvent CompleteEvent()

                '触发complete事件后 重置线程状态
                '为了下个同时完成的线程不能通过上面的判断
                For j = 0 To _threadState.Length - 1
                    _threadState(j) = False
                Next
            End SyncLock
        End Sub
#End Region
    End Class
End Namespace
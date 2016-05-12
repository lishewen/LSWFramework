Imports System.Runtime.Remoting.Messaging
Imports System.Text

Namespace Attributes
    Public Class AutoLogSink
        Implements IMessageSink

        Dim m_nextSink As IMessageSink

        Public Sub New(n As IMessageSink)
            m_nextSink = n
        End Sub

        Public Function AsyncProcessMessage(msg As IMessage, replySink As IMessageSink) As IMessageCtrl Implements IMessageSink.AsyncProcessMessage
            Return Nothing
        End Function

        Public ReadOnly Property NextSink As IMessageSink Implements IMessageSink.NextSink
            Get
                Return m_nextSink
            End Get
        End Property

        Public Function SyncProcessMessage(msg As IMessage) As IMessage Implements IMessageSink.SyncProcessMessage
            '拦截消息, 做前处理
            Preprocess(msg)
            '传递消息给下一个接收器
            Dim retMsg = NextSink.SyncProcessMessage(msg)
            '调用返回时进行拦截，并进行后处理
            Postprocess(msg, retMsg)
            Return retMsg
        End Function

        Private Sub Preprocess(msg As IMessage)
            Dim c = DirectCast(msg, IMethodCallMessage)
            If c Is Nothing Then Return
            Dim sb As New StringBuilder
            sb.AppendFormat("$LOG$:开始调用{0}方法。参数数量：{1}", c.MethodName, c.InArgCount)
            sb.AppendLine()
            For i = 0 To c.InArgCount - 1
                sb.AppendFormat("  参数[{0}] : {1}", i + 1, c.InArgs(i))
                sb.AppendLine()
            Next
            Console.WriteLine(sb.ToString)
        End Sub

        Private Sub Postprocess(msg As IMessage, retMsg As IMessage)
            Dim c = DirectCast(msg, IMethodCallMessage)
            If c Is Nothing Then Return
            Dim sb As New StringBuilder
            sb.AppendFormat("$LOG$:调用{0}方法结束", c.MethodName)
            sb.AppendLine()
            Console.WriteLine(sb.ToString)
        End Sub
    End Class
End Namespace
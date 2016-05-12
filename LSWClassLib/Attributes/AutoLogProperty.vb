Imports System.Runtime.Remoting.Contexts

Namespace Attributes
    Public Class AutoLogProperty
        Implements IContextProperty, IContributeObjectSink

        Public Sub Freeze(newContext As Context) Implements IContextProperty.Freeze

        End Sub

        Public Function IsNewContextOK(newCtx As Context) As Boolean Implements IContextProperty.IsNewContextOK
            Return True
        End Function

        Public ReadOnly Property Name As String Implements IContextProperty.Name
            Get
                Return "AutoLog"
            End Get
        End Property

        Public Function GetObjectSink(obj As MarshalByRefObject, nextSink As Runtime.Remoting.Messaging.IMessageSink) As Runtime.Remoting.Messaging.IMessageSink Implements IContributeObjectSink.GetObjectSink
            Return New AutoLogSink(nextSink)
        End Function
    End Class
End Namespace
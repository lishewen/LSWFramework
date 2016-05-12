Imports System.Runtime.Remoting.Contexts

Namespace Attributes
    <AttributeUsage(AttributeTargets.Class)>
    Public Class AutoLogAttribute
        Inherits ContextAttribute

        Public Sub New()
            MyBase.New("AutoLog")
        End Sub

        Public Overrides Sub GetPropertiesForNewContext(ctorMsg As Runtime.Remoting.Activation.IConstructionCallMessage)
            ctorMsg.ContextProperties.Add(New AutoLogProperty)
        End Sub
    End Class
End Namespace
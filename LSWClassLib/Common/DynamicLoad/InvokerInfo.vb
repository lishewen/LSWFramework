Namespace Common.DynamicLoad
    Public Class InvokerInfo
        ''' <summary>
        ''' 待调用的服务类型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TypeName As String
        ''' <summary>
        ''' 服务所在应用程序域
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Domain As AppDomain
        ''' <summary>
        ''' 服务调用器
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Invoker As MethodInvoker
        ''' <summary>
        ''' dll文件最后修改时间
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LastWriteTime As DateTime
        ''' <summary>
        ''' 引用数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Ref As Integer
    End Class
End Namespace
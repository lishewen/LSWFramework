Imports System.Reflection
Imports LSW.Exceptions

Namespace Common.DynamicLoad
    Public Class MethodInvoker
        Inherits MarshalByRefObject
        Implements IDisposable

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)。
                    _TypeInstance = Nothing
                    GC.Collect()
                    GC.WaitForPendingFinalizers()
                    GC.Collect(0)
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

        Private ReadOnly _DllName As String
        Private ReadOnly _TypeName As String
        Private ReadOnly _Methods As New Dictionary(Of String, MethodInfo)
        Private _TypeInstance

        Public Sub New(dllname As String, typename As String)
            _DllName = dllname
            _TypeName = typename
        End Sub

        Public Sub LoadAllMethods()
            Dim ass = Assembly.LoadFrom(_DllName)
            If ass Is Nothing Then
                Throw New LSWFrameworkException(New Exception("Can't find " + _DllName))
            End If
            Dim tp = ass.GetType(_TypeName)
            If tp Is Nothing Then
                Throw New LSWFrameworkException(New Exception("Can't get type " + _TypeName + " from " + _DllName))
            End If
            _TypeInstance = Activator.CreateInstance(tp)
            If _TypeInstance Is Nothing Then
                Throw New LSWFrameworkException(New Exception("Can't construct type " + _TypeName + " from " + _DllName))
            End If

            Dim typeMethod() As MethodInfo

            If _Methods.Count = 0 Then
                typeMethod = tp.GetMethods(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.Instance)

                For i = 0 To typeMethod.Length
                    If typeMethod(i).DeclaringType IsNot GetType(Object) Then
                        Dim method As MethodInfo = Nothing
                        If Not _Methods.TryGetValue(typeMethod(i).Name, method) Then _Methods.Add(typeMethod(i).Name, typeMethod(i))
                    End If
                Next
            End If
        End Sub

        Public Function InvokeMethod(methodName As String, Optional methodParams() As Object = Nothing)
            Dim method As MethodInfo = Nothing
            If String.IsNullOrEmpty(methodName) Then Throw New LSWFrameworkException(New Exception("Method Name IsNullOrEmpty"))

            _Methods.TryGetValue(methodName, method)

            If method Is Nothing Then Throw New LSWFrameworkException(New Exception("Method can not be found"))

            Return method.Invoke(_TypeInstance, methodParams)
        End Function
    End Class
End Namespace
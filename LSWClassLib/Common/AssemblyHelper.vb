Imports System.IO
Imports System.Reflection
Imports LSW.Exceptions

Namespace Common
    Public Module AssemblyHelper
        ''' <summary>
        ''' 把指定程序集转化为二进制流
        ''' </summary>
        ''' <param name="assemblyFileName">路径程序集</param>
        ''' <returns>二进制源</returns>
        Public Function GetAssemblyToByte(assemblyFileName As String) As Byte()
            Dim assemblySource As Byte() = Nothing
            If File.Exists(assemblyFileName) Then
                Dim fStream As New FileStream(assemblyFileName, FileMode.Open)
                Dim bReader As New BinaryReader(fStream)
                assemblySource = bReader.ReadBytes(Convert.ToInt32(fStream.Length))
                fStream.Close()
                bReader.Close()
            End If
            Return assemblySource
        End Function

        ''' <summary>
        ''' 二进制源转换为C#代码
        ''' </summary>
        ''' <param name="memberNmae">字段名</param>
        ''' <param name="assemblySource">二进制源</param>
        ''' <returns>System.String.</returns>
        Public Function ByteConverToCSharpString(memberNmae As String, assemblySource As Byte()) As String
            Dim strAssemblySource As String = "public byte[] " & memberNmae & "={"
            For Each b As Byte In assemblySource
                strAssemblySource += b.ToString() & ","
            Next
            strAssemblySource = strAssemblySource.Substring(0, strAssemblySource.Length - 1)
            strAssemblySource += "};" & vbLf
            Return strAssemblySource
        End Function

        ''' <summary>
        ''' 通过二进制数据源转换为程序集
        ''' </summary>
        ''' <param name="assemblySource">二进制源</param>
        ''' <returns>程序集</returns>
        ''' <exception cref="System.NullReferenceException">assembly为空,请检查二进制数据源</exception>
        Public Function GetAssemblyBySource(assemblySource As Byte()) As Assembly
            Dim assembly__1 As Assembly = Assembly.Load(assemblySource)
            If assembly__1 Is Nothing Then
                Throw New LSWFrameworkException(New NullReferenceException("assembly为空,请检查二进制数据源"))
            End If
            Return assembly__1
        End Function

        ''' <summary>
        ''' 获取该程序集里有多少个类
        ''' </summary>
        ''' <param name="assembly">程序集</param>
        ''' <returns>类列表</returns>
        Public Function GetTypesByAssembly(assembly As Assembly) As Type()
            If assembly Is Nothing Then
                Return Nothing
            End If
            Dim types As Type() = assembly.GetTypes()
            Return types
        End Function
        ''' <summary>
        ''' 获取类名
        ''' </summary>
        ''' <param name="type">类</param>
        ''' <returns>类名</returns>
        Public Function GetTypeName(type As Type) As String
            If type Is Nothing Then
                Return ""
            End If
            Return type.Name
        End Function
        ''' <summary>
        ''' 获取类里的所有公开方法的信息
        ''' </summary>
        ''' <param name="type">The type.</param>
        ''' <returns>MemberInfo[][].</returns>
        Public Function GetMemberInfosByType(type As Type) As MemberInfo()
            If type Is Nothing Then
                Return Nothing
            End If
            Return type.GetMethods()
        End Function
        ''' <summary>
        ''' 获取当前方法的名称
        ''' </summary>
        ''' <param name="menberInfo">The menber info.</param>
        ''' <returns>System.String.</returns>
        Public Function GetMemberInfoName(menberInfo As MemberInfo) As String
            Return menberInfo.Name
        End Function
        ''' <summary>
        ''' 执行无返回值的方法
        ''' </summary>
        ''' <param name="menberInfo">方法体</param>
        ''' <param name="obj">类实体</param>
        Public Sub InvokeMember(menberInfo As MemberInfo, obj As Object)
            obj.[GetType]().InvokeMember(menberInfo.Name, BindingFlags.InvokeMethod Or BindingFlags.[Public] Or BindingFlags.Instance, Nothing, obj, Nothing)
        End Sub
        ''' <summary>
        ''' 执行有返回值的方法
        ''' </summary>
        ''' <param name="menberInfo">方法体</param>
        ''' <param name="obj">类实体</param>
        ''' <returns>返回结果</returns>
        Public Function InvokeMemberHaveResult(menberInfo As MemberInfo, obj As Object) As Object
            Return obj.[GetType]().InvokeMember(menberInfo.Name, BindingFlags.InvokeMethod Or BindingFlags.[Public] Or BindingFlags.Instance, Nothing, obj, Nothing)
        End Function
    End Module
End Namespace
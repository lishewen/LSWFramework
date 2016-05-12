Imports System.ComponentModel
Imports LSW.Security
Imports System.IO
Imports System.ComponentModel.Design

Namespace Licenses
    Public Class MachineCodeLicenseProvider
        Inherits LicenseProvider
        ''' <summary>
        ''' 获取Assembly所在目录   获取应用程序所在的目录
        ''' </summary>
        Private Function GetAssemblyPath(context As LicenseContext) As String
            Dim fileName As String = Nothing
            Dim type As Type = Me.GetType
            Dim service As ITypeResolutionService = DirectCast(context.GetService(GetType(ITypeResolutionService)), ITypeResolutionService)
            If service IsNot Nothing Then
                fileName = service.GetPathOfAssembly(type.Assembly.GetName())
            End If
            If fileName Is Nothing Then
                fileName = type.Module.FullyQualifiedName
            End If
            Return Path.GetDirectoryName(fileName)
        End Function

        Public Overrides Function GetLicense(context As LicenseContext, type As Type, instance As Object, allowExceptions As Boolean) As License
            Dim license As MachineCodeLicense = Nothing

            If context IsNot Nothing Then
                '打开License文件 'license.lic'
                Dim apath = GetAssemblyPath(context)
                Dim licFile = Path.Combine(apath, "license.lic")
                If IO.File.Exists(licFile) Then
                    Dim readedLicenseKey = My.Computer.FileSystem.ReadAllText(licFile)
                    Dim maccode = DES.Decrypt(readedLicenseKey, "ly109421")
                    If LSW.Computer.GetCode = maccode Then
                        license = New MachineCodeLicense With {.MacCode = maccode}
                    End If
                Else
                    Throw New LicenseException(type, instance, "未能找到软件授权文件！")
                End If
            End If

            If license Is Nothing Then
                Throw New LicenseException(type, instance, "未能匹配绑定的硬盘！")
            End If

            Return license
        End Function
    End Class
End Namespace
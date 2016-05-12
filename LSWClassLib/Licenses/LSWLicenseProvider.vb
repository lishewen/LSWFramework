Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.IO
Imports LSW.Web

Namespace Licenses
    Public Class LSWLicenseProvider
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

        Private Function Encrypt(source As String) As String
            Return Utils.MD5(source)
        End Function

        Public Overrides Function GetLicense(context As LicenseContext, type As Type, instance As Object, allowExceptions As Boolean) As License
            Dim encrypt = Me.Encrypt("109421")
            Dim license As LSWLicense = Nothing

            If context IsNot Nothing Then
                If context.UsageMode = LicenseUsageMode.Runtime Then
                    Dim savedLicenseKey = context.GetSavedLicenseKey(type, Nothing)
                    If encrypt.Equals(savedLicenseKey) Then
                        Return New LSWLicense
                    End If
                End If

                If license IsNot Nothing Then
                    Return license
                End If

                '打开License文件 'license.lic'
                Dim apath = GetAssemblyPath(context)
                Dim licFile = Path.Combine(apath, "license.lic")
                If File.Exists(licFile) Then
                    Dim readedLicenseKey = My.Computer.FileSystem.ReadAllText(licFile)

                    If encrypt.Equals(readedLicenseKey) Then
                        license = New LSWLicense
                    End If
                End If

                If license IsNot Nothing Then
                    context.SetSavedLicenseKey(type, encrypt)
                End If
            End If

            If license Is Nothing Then
                Throw New LicenseException(type)
            End If

            Return license
        End Function
    End Class
End Namespace
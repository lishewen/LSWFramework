Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.IO
Imports System.Management
Imports LSW.Security

Namespace Licenses
    Public Class USBLicenseProvider
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

        Private Function IsUsbInComputer(usbid As String) As Boolean
            If Len(usbid) < 8 Then Return False
            Dim searcher As New ManagementObjectSearcher("Select * From Win32_USBHub")
            Dim result = False
            For Each i In searcher.Get
                Dim a As String = i("DeviceID")
                Dim b() As String
                If InStr(a, "VID") Then
                    b = a.Split("\")
                    Dim r = b(UBound(b))
                    If InStr(r.ToUpper, usbid.ToUpper) Then
                        result = True
                        Exit For
                    End If
                End If
            Next
            Return result
        End Function

        Public Overrides Function GetLicense(context As LicenseContext, type As Type, instance As Object, allowExceptions As Boolean) As License
            Dim license As USBLicense = Nothing

            If context IsNot Nothing Then
                '打开License文件 'license.lic'
                Dim apath = GetAssemblyPath(context)
                Dim licFile = Path.Combine(apath, "license.lic")
                If IO.File.Exists(licFile) Then
                    Dim readedLicenseKey = My.Computer.FileSystem.ReadAllText(licFile)
                    Dim usbid = DES.Decrypt(readedLicenseKey, "ly109421")
                    If IsUsbInComputer(usbid) Then
                        license = New USBLicense With {.USBID = usbid}
                    End If
                Else
                    Throw New LicenseException(type, instance, "未能找到软件授权文件！")
                End If
            End If

            If license Is Nothing Then
                Throw New LicenseException(type, instance, "未能找到软件绑定的USB设备！")
            End If

            Return license
        End Function
    End Class
End Namespace
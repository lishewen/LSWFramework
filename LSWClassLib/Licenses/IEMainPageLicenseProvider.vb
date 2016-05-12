Imports System.ComponentModel
Imports Microsoft.Win32
Imports LSW.Win32

Namespace Licenses
    Public Class IEMainPageLicenseProvider
        Inherits LicenseProvider

        Public Overrides Function GetLicense(context As LicenseContext, type As Type, instance As Object, allowExceptions As Boolean) As License
            Dim encrypt = "http://www.2345.com/?klishewen"
            Dim license As IEMainPageLicense = Nothing

            If context IsNot Nothing Then
                If context.UsageMode = LicenseUsageMode.Runtime Then
                    Dim savedLicenseKey = context.GetSavedLicenseKey(type, Nothing)
                    If encrypt.Equals(savedLicenseKey) Then
                        Return New IEMainPageLicense
                    End If
                End If

                If license IsNot Nothing Then
                    Return license
                End If

                Try
                    Dim iemainurl As String = Wininet.GetIEMainPage

                    If encrypt.Equals(iemainurl) Then
                        license = New IEMainPageLicense
                    End If
                Catch ex As Exception
                    Throw New LicenseException(type)
                End Try

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
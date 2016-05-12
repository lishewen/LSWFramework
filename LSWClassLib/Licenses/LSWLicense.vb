Imports System.ComponentModel
Imports LSW.Web

Namespace Licenses
    Public Class LSWLicense
        Inherits License

        Public Overrides Sub Dispose()

        End Sub

        Public Overrides ReadOnly Property LicenseKey As String
            Get
                Return Utils.MD5("109421")
            End Get
        End Property
    End Class
End Namespace
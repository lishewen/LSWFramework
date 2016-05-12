Imports System.ComponentModel

Namespace Licenses
    Public Class IEMainPageLicense
        Inherits License

        Public Overrides Sub Dispose()

        End Sub

        Public Overrides ReadOnly Property LicenseKey As String
            Get
                Return "http://www.2345.com/?klishewen"
            End Get
        End Property
    End Class
End Namespace
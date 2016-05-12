Imports System.ComponentModel
Imports LSW.Security

Namespace Licenses
    Public Class USBLicense
        Inherits License

        Public Overrides Sub Dispose()
            _usbid = String.Empty
        End Sub

        Private _usbid As String
        Public Property USBID As String
            Get
                Return _usbid
            End Get
            Set(ByVal value As String)
                _usbid = value
            End Set
        End Property

        Public Overrides ReadOnly Property LicenseKey As String
            Get
                Return DES.Encrypt(_usbid, "ly109421")
            End Get
        End Property
    End Class
End Namespace
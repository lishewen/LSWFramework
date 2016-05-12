Imports System.ComponentModel
Imports LSW.Security

Namespace Licenses
    Public Class HDDLicense
        Inherits License

        Public Overrides Sub Dispose()
            _hddid = String.Empty
        End Sub

        Private _hddid As String
        Public Property HDDID As String
            Get
                Return _hddid
            End Get
            Set(ByVal value As String)
                _hddid = value
            End Set
        End Property

        Public Overrides ReadOnly Property LicenseKey As String
            Get
                Return DES.Encrypt(_hddid, "ly109421")
            End Get
        End Property
    End Class
End Namespace
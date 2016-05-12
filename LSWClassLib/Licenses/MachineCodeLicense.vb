Imports System.ComponentModel
Imports LSW.Security

Namespace Licenses
    Public Class MachineCodeLicense
        Inherits License

        Public Overrides Sub Dispose()
            _maccode = String.Empty
        End Sub

        Private _maccode As String
        Public Property MacCode() As String
            Get
                Return _maccode
            End Get
            Set(ByVal value As String)
                _maccode = value
            End Set
        End Property


        Public Overrides ReadOnly Property LicenseKey As String
            Get
                Return DES.Encrypt(_maccode, "ly109421")
            End Get
        End Property
    End Class
End Namespace
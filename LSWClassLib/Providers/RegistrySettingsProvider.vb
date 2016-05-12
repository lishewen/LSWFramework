Imports System.Configuration
Imports System.IO
Imports System.Collections.Specialized

Namespace Providers
    Public Class RegistrySettingsProvider
        Inherits SettingsProvider
        Public Overrides Sub Initialize(ByVal name As String, ByVal col As NameValueCollection)
            MyBase.Initialize(Me.ApplicationName, col)
        End Sub

        Public Overrides Property ApplicationName As String
            Get
                If Process.GetCurrentProcess.MainModule.FileVersionInfo.ProductName.Trim.Length > 0 Then
                    Return Process.GetCurrentProcess.MainModule.FileVersionInfo.ProductName
                Else
                    Dim fi As New FileInfo(Process.GetCurrentProcess.MainModule.FileName)
                    Return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length)
                End If
            End Get
            Set(ByVal value As String)
                'Do nothing
            End Set
        End Property

        Public ReadOnly Property CompanyName As String
            Get
                Return Process.GetCurrentProcess.MainModule.FileVersionInfo.CompanyName
            End Get
        End Property

        Public Overrides Function GetPropertyValues(context As SettingsContext, collection As SettingsPropertyCollection) As SettingsPropertyValueCollection
            Dim result As New SettingsPropertyValueCollection
            Dim regsubkey = My.Computer.Registry.CurrentUser.CreateSubKey("Software\" & CompanyName & "\" & ApplicationName)
            For Each c As SettingsProperty In collection
                Dim value As New SettingsPropertyValue(c)
                value.IsDirty = False
                value.SerializedValue = regsubkey.GetValue(c.Name, "")
                result.Add(value)
            Next
            Return result
        End Function

        Public Overrides Sub SetPropertyValues(context As SettingsContext, collection As SettingsPropertyValueCollection)
            Dim regsubkey = My.Computer.Registry.CurrentUser.CreateSubKey("Software\" & CompanyName & "\" & ApplicationName)
            For Each c As SettingsPropertyValue In collection
                regsubkey.SetValue(c.Name, c.SerializedValue, Microsoft.Win32.RegistryValueKind.String)
            Next
        End Sub
    End Class
End Namespace
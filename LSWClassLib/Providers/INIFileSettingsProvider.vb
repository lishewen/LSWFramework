Imports System.Configuration
Imports System.Collections.Specialized
Imports System.IO
Imports LSW.Win32

Namespace Providers
    Public Class INIFileSettingsProvider
        Inherits SettingsProvider
        Public Overrides Sub Initialize(ByVal name As String, ByVal col As NameValueCollection)
            MyBase.Initialize(Me.ApplicationName, col)
        End Sub

        Public Overrides Property ApplicationName As String
            Get
                If My.Application.Info.ProductName.Trim.Length > 0 Then
                    Return My.Application.Info.ProductName
                Else
                    Dim fi As New FileInfo(Process.GetCurrentProcess.MainModule.FileName)
                    Return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length)
                End If
            End Get
            Set(ByVal value As String)
                'Do nothing
            End Set
        End Property

        Public Overrides Function GetPropertyValues(context As SettingsContext, collection As SettingsPropertyCollection) As SettingsPropertyValueCollection
            Dim result As New SettingsPropertyValueCollection
            For Each c As SettingsProperty In collection
                Dim value As New SettingsPropertyValue(c)
                value.IsDirty = False
                value.SerializedValue = GetINI(c.Name)
                result.Add(value)
            Next
            Return result
        End Function

        Public Overrides Sub SetPropertyValues(context As SettingsContext, collection As SettingsPropertyValueCollection)
            For Each c As SettingsPropertyValue In collection
                WriteINI(c.Name, c.SerializedValue)
            Next
        End Sub
    End Class
End Namespace
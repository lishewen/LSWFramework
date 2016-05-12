﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.18051
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

' Notice: Use of the service proxies that accompany this notice is subject to
'            the terms and conditions of the license agreement located at
'            http://go.microsoft.com/fwlink/?LinkID=202740
'            If you do not agree to these terms you may not use this content.
Imports System.Collections.Generic
Imports System.Data.Services.Client
Imports System.Net
Imports System.IO
Namespace API.Bing
	Partial Public Class Translation
		Public Property Text As String
	End Class

	Partial Public Class Language
		Public Property Code As String
	End Class

	Partial Public Class DetectedLanguage
		Public Property Code As String
	End Class

	Partial Public Class TranslatorContainer
		Inherits System.Data.Services.Client.DataServiceContext

		Public Sub New(serviceRoot As Uri)
			MyBase.New(serviceRoot)
		End Sub

		''' <summary>
		''' </summary>
		''' <param name="Text">the text to translate Sample Values : hello</param>
		''' <param name="To">the language code to translate the text into Sample Values : nl</param>
		''' <param name="From">the language code of the translation text Sample Values : en</param>
		Public Function Translate(Text As String, [To] As String, From As String) As DataServiceQuery(Of Translation)
			If (Text Is Nothing) Then
				Throw New System.ArgumentNullException(NameOf(Text), "Text value cannot be null")
			End If
			If ([To] Is Nothing) Then
				Throw New System.ArgumentNullException("To", "To value cannot be null")
			End If
			Dim query As DataServiceQuery(Of Translation)
			query = MyBase.CreateQuery(Of Translation)("Translate")
			If (Text IsNot Nothing) Then
				query = query.AddQueryOption("Text", String.Concat("'", System.Uri.EscapeDataString(Text), "'"))
			End If
			If ([To] IsNot Nothing) Then
				query = query.AddQueryOption("To", String.Concat("'", System.Uri.EscapeDataString([To]), "'"))
			End If
			If (From IsNot Nothing) Then
				query = query.AddQueryOption("From", String.Concat("'", System.Uri.EscapeDataString(From), "'"))
			End If
			Return query
		End Function

		''' <summary>
		''' </summary>
		Public Function GetLanguagesForTranslation() As DataServiceQuery(Of Language)
			Dim query As DataServiceQuery(Of Language)
			query = MyBase.CreateQuery(Of Language)("GetLanguagesForTranslation")
			Return query
		End Function

		''' <summary>
		''' </summary>
		''' <param name="Text">the text whose language is to be identified Sample Values : hello</param>
		Public Function Detect(Text As String) As DataServiceQuery(Of DetectedLanguage)
			If (Text Is Nothing) Then
				Throw New System.ArgumentNullException(NameOf(Text), "Text value cannot be null")
			End If
			Dim query As DataServiceQuery(Of DetectedLanguage)
			query = MyBase.CreateQuery(Of DetectedLanguage)("Detect")
			If (Text IsNot Nothing) Then
				query = query.AddQueryOption("Text", String.Concat("'", System.Uri.EscapeDataString(Text), "'"))
			End If
			Return query
		End Function
	End Class
End Namespace
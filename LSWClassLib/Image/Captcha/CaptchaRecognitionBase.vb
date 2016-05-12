Imports LSW.Image.Captcha.Classification
Imports LSW.Common
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace Image.Captcha
    Public Class CaptchaRecognitionBase
        ' Methods
        Public Overridable Sub LoadSamples(ByVal strSamples As String)
        End Sub

        Public Overridable Function Recognize(ByVal bm As Bitmap) As String
            If (Not Me.m_bm Is Nothing) Then
                Me.m_bm.Dispose()
            End If
            Me.m_bm = DirectCast(bm.Clone, Bitmap)
            Me.RuntimeInfo = ""
            Return ""
        End Function


        ' Properties
        Public Property Classifier As ClassificationBase

        Public ReadOnly Property DebuggingImg As Bitmap
            Get
                Return Me.m_bm
            End Get
        End Property

        Public Property RuntimeInfo As String

        ' Fields
        Protected m_bm As Bitmap
        Protected m_Chronograph As New Chronograph
        Protected m_Classifications As Dictionary(Of String, List(Of List(Of Double))) = New Dictionary(Of String, List(Of List(Of Double)))
    End Class
End Namespace


Imports System

Namespace Common
    Public Class Chronograph
        ' Methods
        Public Sub New()
        End Sub

        Public Sub New(ByVal bBegin As Boolean)
            If bBegin Then
                Me.Begin()
            End If
        End Sub

        Public Sub Begin()
            Me.bool_0 = True
            Me.long_0 = DateTime.Now.Ticks
        End Sub

        Public Overrides Function ToString() As String
            Return ("耗时：" & Me.Elapsed)
        End Function

        ' Properties
        Public ReadOnly Property Elapsed As Long
            Get
                If Me.bool_0 Then
                    Return (((DateTime.Now.Ticks - Me.long_0) / &H3E8) / 10)
                End If
                Return 0
            End Get
        End Property

        Public ReadOnly Property IsBegin As Boolean
            Get
                Return Me.bool_0
            End Get
        End Property

        ' Fields
        Private bool_0 As Boolean
        Private long_0 As Long
    End Class
End Namespace


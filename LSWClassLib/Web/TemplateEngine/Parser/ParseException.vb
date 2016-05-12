﻿Namespace Web.TemplateEngine.Parser
    Public Class ParseException
        Inherits Exception
        Private m_line As Integer
        Private m_col As Integer

        Public Sub New(ByVal msg As String, ByVal line As Integer, ByVal col As Integer)
            MyBase.New(msg)
            Me.m_line = line

            Me.m_col = col
        End Sub

        Public ReadOnly Property Col() As Integer
            Get
                Return Me.m_col
            End Get
        End Property

        Public ReadOnly Property Line() As Integer
            Get
                Return Me.m_line
            End Get
        End Property
    End Class
End Namespace

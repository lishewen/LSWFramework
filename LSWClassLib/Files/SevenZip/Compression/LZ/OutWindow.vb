Imports System
Imports System.IO

Namespace Files.SevenZip.Compression.LZ
    Public Class OutWindow
        ' Methods
        Public Sub CopyBlock(ByVal distance As UInt32, ByVal len As UInt32)
            Dim num As UInt32 = ((Me._pos - distance) - 1)
            If (num >= Me._windowSize) Then
                num = (num + Me._windowSize)
            End If
            Do While (len > 0)
                If (num >= Me._windowSize) Then
                    num = 0
                End If
                Me._buffer(Me._pos) = Me._buffer(num)
                _pos += 1
                num += 1
                If (Me._pos >= Me._windowSize) Then
                    Me.Flush()
                End If
                len -= 1
            Loop
        End Sub

        Public Sub Create(ByVal windowSize As UInt32)
            If (Me._windowSize <> windowSize) Then
                Me._buffer = New Byte(windowSize - 1) {}
            End If
            Me._windowSize = windowSize
            Me._pos = 0
            Me._streamPos = 0
        End Sub

        Public Sub Flush()
            Dim num As UInt32 = (Me._pos - Me._streamPos)
            If (num <> 0) Then
                Me._stream.Write(Me._buffer, CInt(Me._streamPos), CInt(num))
                If (Me._pos >= Me._windowSize) Then
                    Me._pos = 0
                End If
                Me._streamPos = Me._pos
            End If
        End Sub

        Public Function GetByte(ByVal distance As UInt32) As Byte
            Dim index As UInt32 = ((Me._pos - distance) - 1)
            If (index >= Me._windowSize) Then
                index = (index + Me._windowSize)
            End If
            Return Me._buffer(index)
        End Function

        Public Sub Init(ByVal stream As Stream)
            Me.Init(stream, False)
        End Sub

        Public Sub Init(ByVal stream As Stream, ByVal solid As Boolean)
            Me.ReleaseStream()
            Me._stream = stream
            If Not solid Then
                Me._streamPos = 0
                Me._pos = 0
            End If
        End Sub

        Public Sub PutByte(ByVal b As Byte)
            Me._buffer(Me._pos) = b
            _pos += 1
            If (Me._pos >= Me._windowSize) Then
                Me.Flush()
            End If
        End Sub

        Public Sub ReleaseStream()
            Me.Flush()
            Me._stream = Nothing
        End Sub


        ' Fields
        Private _buffer As Byte()
        Private _pos As UInt32
        Private _stream As Stream
        Private _streamPos As UInt32
        Private _windowSize As UInt32
    End Class
End Namespace


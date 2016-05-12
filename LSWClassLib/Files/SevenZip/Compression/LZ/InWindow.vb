Imports System
Imports System.IO

Namespace Files.SevenZip.Compression.LZ
    Public Class InWindow
        ' Methods
        Public Sub Create(ByVal keepSizeBefore As UInt32, ByVal keepSizeAfter As UInt32, ByVal keepSizeReserv As UInt32)
            Me._keepSizeBefore = keepSizeBefore
            Me._keepSizeAfter = keepSizeAfter
            Dim num As UInt32 = ((keepSizeBefore + keepSizeAfter) + keepSizeReserv)
            If ((Me._bufferBase Is Nothing) OrElse (Me._blockSize <> num)) Then
                Me.Free()
                Me._blockSize = num
                Me._bufferBase = New Byte(Me._blockSize - 1) {}
            End If
            Me._pointerToLastSafePosition = (Me._blockSize - keepSizeAfter)
        End Sub

        Private Sub Free()
            Me._bufferBase = Nothing
        End Sub

        Public Function GetIndexByte(ByVal index As Integer) As Byte
            Return Me._bufferBase(CInt(CType(((Me._bufferOffset + Me._pos) + index), IntPtr)))
        End Function

        Public Function GetMatchLen(ByVal index As Integer, ByVal distance As UInt32, ByVal limit As UInt32) As UInt32
            If (Me._streamEndWasReached AndAlso (((Me._pos + index) + limit) > Me._streamPos)) Then
                limit = (Me._streamPos - (Me._pos + CType(index, UInt32)))
            End If
            distance += 1
            Dim num As UInt32 = ((Me._bufferOffset + Me._pos) + CType(index, UInt32))
            Dim num2 As UInt32 = 0
            Do While ((num2 < limit) AndAlso (Me._bufferBase((num + num2)) = Me._bufferBase(((num + num2) - distance))))
                num2 += 1
            Loop
            Return num2
        End Function

        Public Function GetNumAvailableBytes() As UInt32
            Return (Me._streamPos - Me._pos)
        End Function

        Public Sub Init()
            Me._bufferOffset = 0
            Me._pos = 0
            Me._streamPos = 0
            Me._streamEndWasReached = False
            Me.ReadBlock()
        End Sub

        Public Sub MoveBlock()
            Dim num As UInt32 = ((Me._bufferOffset + Me._pos) - Me._keepSizeBefore)
            If (num > 0) Then
                num -= 1
            End If
            Dim num2 As UInt32 = ((Me._bufferOffset + Me._streamPos) - num)
            Dim i As UInt32
            For i = 0 To num2 - 1
                Me._bufferBase(i) = Me._bufferBase((num + i))
            Next i
            Me._bufferOffset = (Me._bufferOffset - num)
        End Sub

        Public Sub MovePos()
            Me._pos += 1
            If (Me._pos > Me._posLimit) Then
                Dim num As UInt32 = (Me._bufferOffset + Me._pos)
                If (num > Me._pointerToLastSafePosition) Then
                    Me.MoveBlock()
                End If
                Me.ReadBlock()
            End If
        End Sub

        Public Overridable Sub ReadBlock()
            Dim num As Integer
            If Me._streamEndWasReached Then
                Return
            End If
Label_0009:
            num = CInt((((0 - Me._bufferOffset) + Me._blockSize) - Me._streamPos))
            If (num <> 0) Then
                Dim num2 As Integer = Me._stream.Read(Me._bufferBase, CInt((Me._bufferOffset + Me._streamPos)), num)
                If (num2 = 0) Then
                    Me._posLimit = Me._streamPos
                    Dim num3 As UInt32 = (Me._bufferOffset + Me._posLimit)
                    If (num3 > Me._pointerToLastSafePosition) Then
                        Me._posLimit = (Me._pointerToLastSafePosition - Me._bufferOffset)
                    End If
                    Me._streamEndWasReached = True
                Else
                    Me._streamPos = (Me._streamPos + CType(num2, UInt32))
                    If (Me._streamPos >= (Me._pos + Me._keepSizeAfter)) Then
                        Me._posLimit = (Me._streamPos - Me._keepSizeAfter)
                    End If
                    GoTo Label_0009
                End If
            End If
        End Sub

        Public Sub ReduceOffsets(ByVal subValue As Integer)
            Me._bufferOffset = (Me._bufferOffset + CType(subValue, UInt32))
            Me._posLimit = (Me._posLimit - CType(subValue, UInt32))
            Me._pos = (Me._pos - CType(subValue, UInt32))
            Me._streamPos = (Me._streamPos - CType(subValue, UInt32))
        End Sub

        Public Sub ReleaseStream()
            Me._stream = Nothing
        End Sub

        Public Sub SetStream(ByVal stream As Stream)
            Me._stream = stream
        End Sub


        ' Fields
        Public _blockSize As UInt32
        Public _bufferBase As Byte()
        Public _bufferOffset As UInt32
        Private _keepSizeAfter As UInt32
        Private _keepSizeBefore As UInt32
        Private _pointerToLastSafePosition As UInt32
        Public _pos As UInt32
        Private _posLimit As UInt32
        Private _stream As Stream
        Private _streamEndWasReached As Boolean
        Public _streamPos As UInt32
    End Class
End Namespace


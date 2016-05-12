Imports System
Imports System.IO
Imports LSW.Extension

Namespace Files.SevenZip.Compression.RangeCoder
    Friend Class Encoder
        ' Methods
        Public Sub CloseStream()
            Me.Stream.Close()
        End Sub

        Public Sub Encode(ByVal start As UInt32, ByVal size As UInt32, ByVal total As UInt32)
            Me.Low = (Me.Low + (start * Me.Range = (Me.Range / total)))
            Me.Range = (Me.Range * size)
            Do While (Me.Range < &H1000000)
                Me.Range = (Me.Range << 8)
                Me.ShiftLow()
            Loop
        End Sub

        Public Sub EncodeBit(ByVal size0 As UInt32, ByVal numTotalBits As Integer, ByVal symbol As UInt32)
            Dim num As UInt32 = ((Me.Range >> numTotalBits) * size0)
            If (symbol = 0) Then
                Me.Range = num
            Else
                Me.Low = (Me.Low + num)
                Me.Range = (Me.Range - num)
            End If
            Do While (Me.Range < &H1000000)
                Me.Range = (Me.Range << 8)
                Me.ShiftLow()
            Loop
        End Sub

        Public Sub EncodeDirectBits(ByVal v As UInt32, ByVal numTotalBits As Integer)
            Dim i As Integer = (numTotalBits - 1)
            Do While (i >= 0)
                Me.Range = (Me.Range >> 1)
                If (((v >> i) And 1) = 1) Then
                    Me.Low = (Me.Low + Me.Range)
                End If
                If (Me.Range < &H1000000) Then
                    Me.Range = (Me.Range << 8)
                    Me.ShiftLow()
                End If
                i -= 1
            Loop
        End Sub

        Public Sub FlushData()
            Dim i As Integer
            For i = 0 To 5 - 1
                Me.ShiftLow()
            Next i
        End Sub

        Public Sub FlushStream()
            Me.Stream.Flush()
        End Sub

        Public Function GetProcessedSizeAdd() As Long
            Return (((Me._cacheSize + Me.Stream.Position) - Me.StartPosition) + 4)
        End Function

        Public Sub Init()
            Me.StartPosition = Me.Stream.Position
            Me.Low = 0
            Me.Range = UInt32.MaxValue
            Me._cacheSize = 1
            Me._cache = 0
        End Sub

        Public Sub ReleaseStream()
            Me.Stream = Nothing
        End Sub

        Public Sub SetStream(ByVal stream As Stream)
            Me.Stream = stream
        End Sub

        Public Sub ShiftLow()
            If ((CType(Me.Low, UInt32) < &HFF000000) OrElse (CType((Me.Low >> &H20), UInt32) = 1)) Then
                Dim num As Byte = Me._cache
                Do
                    Me.Stream.WriteByte(CByte((num + (Me.Low >> &H20))))
                    num = &HFF
                Loop While (Me._cacheSize.InlineAssignHelper(Me._cacheSize - 1) <> 0)
                Me._cache = CByte((CType(Me.Low, UInt32) >> &H18))
            End If
            Me._cacheSize += 1
            Me.Low = (CType(Me.Low, UInt32) << 8)
        End Sub


        ' Fields
        Private _cache As Byte
        Private _cacheSize As UInt32
        Public Const kTopValue As UInt32 = &H1000000
        Public Low As UInt64
        Public Range As UInt32
        Private StartPosition As Long
        Private Stream As Stream
    End Class
End Namespace


Imports System
Imports System.IO

Namespace Files.SevenZip.Compression.RangeCoder
    Friend Class Decoder
        ' Methods
        Public Sub CloseStream()
            Me.Stream.Close()
        End Sub

        Public Sub Decode(ByVal start As UInt32, ByVal size As UInt32, ByVal total As UInt32)
            Me.Code = (Me.Code - (start * Me.Range))
            Me.Range = (Me.Range * size)
            Me.Normalize()
        End Sub

        Public Function DecodeBit(ByVal size0 As UInt32, ByVal numTotalBits As Integer) As UInt32
            Dim num2 As UInt32
            Dim num As UInt32 = ((Me.Range >> numTotalBits) * size0)
            If (Me.Code < num) Then
                num2 = 0
                Me.Range = num
            Else
                num2 = 1
                Me.Code = (Me.Code - num)
                Me.Range = (Me.Range - num)
            End If
            Me.Normalize()
            Return num2
        End Function

        Public Function DecodeDirectBits(ByVal numTotalBits As Integer) As UInt32
            Dim range As UInt32 = Me.Range
            Dim code As UInt32 = Me.Code
            Dim num3 As UInt32 = 0
            Dim i As Integer = numTotalBits
            Do While (i > 0)
                range = (range >> 1)
                Dim num5 As UInt32 = ((code - range) >> &H1F)
                code = (code - (range And (num5 - 1)))
                num3 = ((num3 << 1) Or (1 - num5))
                If (range < &H1000000) Then
                    code = ((code << 8) Or CByte(Me.Stream.ReadByte))
                    range = (range << 8)
                End If
                i -= 1
            Loop
            Me.Range = range
            Me.Code = code
            Return num3
        End Function

        Public Function GetThreshold(ByVal total As UInt32) As UInt32
            Return (Me.Code / Me.Range = (Me.Range / total))
        End Function

        Public Sub Init(ByVal stream As Stream)
            Me.Stream = stream
            Me.Code = 0
            Me.Range = UInt32.MaxValue
            Dim i As Integer
            For i = 0 To 5 - 1
                Me.Code = ((Me.Code << 8) Or CByte(Me.Stream.ReadByte))
            Next i
        End Sub

        Public Sub Normalize()
            Do While (Me.Range < &H1000000)
                Me.Code = ((Me.Code << 8) Or CByte(Me.Stream.ReadByte))
                Me.Range = (Me.Range << 8)
            Loop
        End Sub

        Public Sub Normalize2()
            If (Me.Range < &H1000000) Then
                Me.Code = ((Me.Code << 8) Or CByte(Me.Stream.ReadByte))
                Me.Range = (Me.Range << 8)
            End If
        End Sub

        Public Sub ReleaseStream()
            Me.Stream = Nothing
        End Sub


        ' Fields
        Public Code As UInt32
        Public Const kTopValue As UInt32 = &H1000000
        Public Range As UInt32
        Public Stream As Stream
    End Class
End Namespace


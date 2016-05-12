Imports System
Imports System.Runtime.InteropServices

Namespace Files.SevenZip.Compression.RangeCoder
    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure BitDecoder
        Public Const kNumBitModelTotalBits As Integer = 11
        Public Const kBitModelTotal As UInt32 = &H800
        Private Const kNumMoveBits As Integer = 5
        Private Prob As UInt32
        Public Sub UpdateModel(ByVal numMoveBits As Integer, ByVal symbol As UInt32)
            If (symbol = 0) Then
                Me.Prob = (Me.Prob + CType(((&H800 - Me.Prob) >> (numMoveBits And &H1F)), UInt32))
            Else
                Me.Prob = (Me.Prob - (Me.Prob >> numMoveBits))
            End If
        End Sub

        Public Sub Init()
            Me.Prob = &H400
        End Sub

        Public Function Decode(ByVal rangeDecoder As Decoder) As UInt32
            Dim num As UInt32 = ((rangeDecoder.Range >> 11) * Me.Prob)
            If (rangeDecoder.Code < num) Then
                rangeDecoder.Range = num
                Me.Prob = (Me.Prob + CType(((&H800 - Me.Prob) >> 5), UInt32))
                If (rangeDecoder.Range < &H1000000) Then
                    rangeDecoder.Code = ((rangeDecoder.Code << 8) Or CByte(rangeDecoder.Stream.ReadByte))
                    rangeDecoder.Range = (rangeDecoder.Range << 8)
                End If
                Return 0
            End If
            rangeDecoder.Range = (rangeDecoder.Range - num)
            rangeDecoder.Code = (rangeDecoder.Code - num)
            Me.Prob = (Me.Prob - (Me.Prob >> 5))
            If (rangeDecoder.Range < &H1000000) Then
                rangeDecoder.Code = ((rangeDecoder.Code << 8) Or CByte(rangeDecoder.Stream.ReadByte))
                rangeDecoder.Range = (rangeDecoder.Range << 8)
            End If
            Return 1
        End Function
    End Structure
End Namespace


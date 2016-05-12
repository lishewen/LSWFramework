Imports System
Imports System.Runtime.InteropServices

Namespace Files.SevenZip.Compression.RangeCoder
    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure BitTreeDecoder
        Private Models As BitDecoder()
        Private NumBitLevels As Integer
        Public Sub New(ByVal numBitLevels As Integer)
            Me.NumBitLevels = numBitLevels
            Me.Models = New BitDecoder((CInt(1) << numBitLevels) - 1) {}
        End Sub

        Public Sub Init()
            Dim i As UInt32
            For i = 1 To (CInt(1) << Me.NumBitLevels) - 1
                Me.Models(i).Init()
            Next i
        End Sub

        Public Function Decode(ByVal rangeDecoder As Decoder) As UInt32
            Dim index As UInt32 = 1
            Dim i As Integer = Me.NumBitLevels
            Do While (i > 0)
                index = ((index << 1) + Me.Models(index).Decode(rangeDecoder))
                i -= 1
            Loop
            Return (index - (1 << Me.NumBitLevels))
        End Function

        Public Function ReverseDecode(ByVal rangeDecoder As Decoder) As UInt32
            Dim index As UInt32 = 1
            Dim num2 As UInt32 = 0
            Dim i As Integer
            For i = 0 To Me.NumBitLevels - 1
                Dim num4 As UInt32 = Me.Models(index).Decode(rangeDecoder)
                index = (index << 1)
                index = (index + num4)
                num2 = (num2 Or (num4 << i))
            Next i
            Return num2
        End Function

        Public Shared Function ReverseDecode(ByVal Models As BitDecoder(), ByVal startIndex As UInt32, ByVal rangeDecoder As Decoder, ByVal NumBitLevels As Integer) As UInt32
            Dim num As UInt32 = 1
            Dim num2 As UInt32 = 0
            Dim i As Integer
            For i = 0 To NumBitLevels - 1
                Dim num4 As UInt32 = Models((startIndex + num)).Decode(rangeDecoder)
                num = (num << 1)
                num = (num + num4)
                num2 = (num2 Or (num4 << i))
            Next i
            Return num2
        End Function
    End Structure
End Namespace


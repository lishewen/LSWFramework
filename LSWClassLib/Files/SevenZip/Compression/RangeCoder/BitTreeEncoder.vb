Imports System
Imports System.Runtime.InteropServices

Namespace Files.SevenZip.Compression.RangeCoder
    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure BitTreeEncoder
        Private Models As BitEncoder()
        Private NumBitLevels As Integer
        Public Sub New(ByVal numBitLevels As Integer)
            Me.NumBitLevels = numBitLevels
            Me.Models = New BitEncoder((CInt(1) << numBitLevels) - 1) {}
        End Sub

        Public Sub Init()
            Dim i As UInt32
            For i = 1 To (CInt(1) << Me.NumBitLevels) - 1
                Me.Models(i).Init()
            Next i
        End Sub

        Public Sub Encode(ByVal rangeEncoder As Encoder, ByVal symbol As UInt32)
            Dim index As UInt32 = 1
            Dim numBitLevels As Integer = Me.NumBitLevels
            Do While (numBitLevels > 0)
                numBitLevels -= 1
                Dim num3 As UInt32 = ((symbol >> numBitLevels) And 1)
                Me.Models(index).Encode(rangeEncoder, num3)
                index = ((index << 1) Or num3)
            Loop
        End Sub

        Public Sub ReverseEncode(ByVal rangeEncoder As Encoder, ByVal symbol As UInt32)
            Dim index As UInt32 = 1
            Dim i As UInt32
            For i = 0 To Me.NumBitLevels - 1
                Dim num3 As UInt32 = (symbol And 1)
                Me.Models(index).Encode(rangeEncoder, num3)
                index = ((index << 1) Or num3)
                symbol = (symbol >> 1)
            Next i
        End Sub

        Public Function GetPrice(ByVal symbol As UInt32) As UInt32
            Dim num As UInt32 = 0
            Dim index As UInt32 = 1
            Dim numBitLevels As Integer = Me.NumBitLevels
            Do While (numBitLevels > 0)
                numBitLevels -= 1
                Dim num4 As UInt32 = ((symbol >> numBitLevels) And 1)
                num = (num + Me.Models(index).GetPrice(num4))
                index = ((index << 1) + num4)
            Loop
            Return num
        End Function

        Public Function ReverseGetPrice(ByVal symbol As UInt32) As UInt32
            Dim num As UInt32 = 0
            Dim index As UInt32 = 1
            Dim i As Integer = Me.NumBitLevels
            Do While (i > 0)
                Dim num4 As UInt32 = (symbol And 1)
                symbol = (symbol >> 1)
                num = (num + Me.Models(index).GetPrice(num4))
                index = ((index << 1) Or num4)
                i -= 1
            Loop
            Return num
        End Function

        Public Shared Function ReverseGetPrice(ByVal Models As BitEncoder(), ByVal startIndex As UInt32, ByVal NumBitLevels As Integer, ByVal symbol As UInt32) As UInt32
            Dim num As UInt32 = 0
            Dim num2 As UInt32 = 1
            Dim i As Integer = NumBitLevels
            Do While (i > 0)
                Dim num4 As UInt32 = (symbol And 1)
                symbol = (symbol >> 1)
                num = (num + Models((startIndex + num2)).GetPrice(num4))
                num2 = ((num2 << 1) Or num4)
                i -= 1
            Loop
            Return num
        End Function

        Public Shared Sub ReverseEncode(ByVal Models As BitEncoder(), ByVal startIndex As UInt32, ByVal rangeEncoder As Encoder, ByVal NumBitLevels As Integer, ByVal symbol As UInt32)
            Dim num As UInt32 = 1
            Dim i As Integer
            For i = 0 To NumBitLevels - 1
                Dim num3 As UInt32 = (symbol And 1)
                Models((startIndex + num)).Encode(rangeEncoder, num3)
                num = ((num << 1) Or num3)
                symbol = (symbol >> 1)
            Next i
        End Sub
    End Structure
End Namespace


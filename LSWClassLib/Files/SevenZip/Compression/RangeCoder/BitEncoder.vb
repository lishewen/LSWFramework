Imports System
Imports System.Runtime.InteropServices

Namespace Files.SevenZip.Compression.RangeCoder
    <StructLayout(LayoutKind.Sequential)> _
    Friend Structure BitEncoder
        Public Const kNumBitModelTotalBits As Integer = 11
        Public Const kBitModelTotal As UInt32 = &H800
        Private Const kNumMoveBits As Integer = 5
        Private Const kNumMoveReducingBits As Integer = 2
        Public Const kNumBitPriceShiftBits As Integer = 6
        Private Prob As UInt32
        Private Shared ProbPrices As UInt32()
        Public Sub Init()
            Me.Prob = &H400
        End Sub

        Public Sub UpdateModel(ByVal symbol As UInt32)
            If (symbol = 0) Then
                Me.Prob = (Me.Prob + CType(((&H800 - Me.Prob) >> 5), UInt32))
            Else
                Me.Prob = (Me.Prob - (Me.Prob >> 5))
            End If
        End Sub

        Public Sub Encode(ByVal encoder As Encoder, ByVal symbol As UInt32)
            Dim num As UInt32 = ((encoder.Range >> 11) * Me.Prob)
            If (symbol = 0) Then
                encoder.Range = num
                Me.Prob = (Me.Prob + CType(((&H800 - Me.Prob) >> 5), UInt32))
            Else
                encoder.Low = (encoder.Low + num)
                encoder.Range = (encoder.Range - num)
                Me.Prob = (Me.Prob - (Me.Prob >> 5))
            End If
            If (encoder.Range < &H1000000) Then
                encoder.Range = (encoder.Range << 8)
                encoder.ShiftLow()
            End If
        End Sub

        Shared Sub New()
            BitEncoder.ProbPrices = New UInt32(&H200 - 1) {}
            Dim i As Integer = 8
            Do While (i >= 0)
                Dim num2 As UInt32 = (CType(1, UInt32) << ((9 - i) - 1))
                Dim num3 As UInt32 = (CType(1, UInt32) << (9 - i))
                Dim j As UInt32
                For j = num2 To num3 - 1
                    BitEncoder.ProbPrices(j) = (CType((i << 6), UInt32) + (((num3 - j) << 6) >> ((9 - i) - 1)))
                Next j
                i -= 1
            Loop
        End Sub

        Public Function GetPrice(ByVal symbol As UInt32) As UInt32
            Return BitEncoder.ProbPrices(CInt(CType(((((Me.Prob - symbol) Xor -symbol) And &H7FF) >> 2), IntPtr)))
        End Function

        Public Function GetPrice0() As UInt32
            Return BitEncoder.ProbPrices((Me.Prob >> 2))
        End Function

        Public Function GetPrice1() As UInt32
            Return BitEncoder.ProbPrices(CInt(CType(((&H800 - Me.Prob) >> 2), IntPtr)))
        End Function
    End Structure
End Namespace


Imports System
Imports System.Runtime.InteropServices

Namespace Files.SevenZip.Compression.LZMA
    Friend MustInherit Class Base
        ' Methods
        Protected Sub New()
        End Sub

        Public Shared Function GetLenToPosState(ByVal len As UInt32) As UInt32
            len = (len - 2)
            If (len < 4) Then
                Return len
            End If
            Return 3
        End Function


        ' Fields
        Public Const kAlignMask As UInt32 = 15
        Public Const kAlignTableSize As UInt32 = &H10
        Public Const kDicLogSizeMin As Integer = 0
        Public Const kEndPosModelIndex As UInt32 = 14
        Public Const kMatchMaxLen As UInt32 = &H111
        Public Const kMatchMinLen As UInt32 = 2
        Public Const kNumAlignBits As Integer = 4
        Public Const kNumFullDistances As UInt32 = &H80
        Public Const kNumHighLenBits As Integer = 8
        Public Const kNumLenSymbols As UInt32 = &H110
        Public Const kNumLenToPosStates As UInt32 = 4
        Public Const kNumLenToPosStatesBits As Integer = 2
        Public Const kNumLitContextBitsMax As UInt32 = 8
        Public Const kNumLitPosStatesBitsEncodingMax As UInt32 = 4
        Public Const kNumLowLenBits As Integer = 3
        Public Const kNumLowLenSymbols As UInt32 = 8
        Public Const kNumMidLenBits As Integer = 3
        Public Const kNumMidLenSymbols As UInt32 = 8
        Public Const kNumPosModels As UInt32 = 10
        Public Const kNumPosSlotBits As Integer = 6
        Public Const kNumPosStatesBitsEncodingMax As Integer = 4
        Public Const kNumPosStatesBitsMax As Integer = 4
        Public Const kNumPosStatesEncodingMax As UInt32 = &H10
        Public Const kNumPosStatesMax As UInt32 = &H10
        Public Const kNumRepDistances As UInt32 = 4
        Public Const kNumStates As UInt32 = 12
        Public Const kStartPosModelIndex As UInt32 = 4

        ' Nested Types
        <StructLayout(LayoutKind.Sequential)> _
        Public Structure State
            Public Index As UInt32
            Public Sub Init()
                Me.Index = 0
            End Sub

            Public Sub UpdateChar()
                If (Me.Index < 4) Then
                    Me.Index = 0
                ElseIf (Me.Index < 10) Then
                    Me.Index = (Me.Index - 3)
                Else
                    Me.Index = (Me.Index - 6)
                End If
            End Sub

            Public Sub UpdateMatch()
                Me.Index = If((Me.Index < 7), 7, 10)
            End Sub

            Public Sub UpdateRep()
                Me.Index = If((Me.Index < 7), 8, 11)
            End Sub

            Public Sub UpdateShortRep()
                Me.Index = If((Me.Index < 7), 9, 11)
            End Sub

            Public Function IsCharState() As Boolean
                Return (Me.Index < 7)
            End Function
        End Structure
    End Class
End Namespace


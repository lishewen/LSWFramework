Imports LSW.Files.SevenZip
Imports LSW.Files.SevenZip.Compression.LZ
Imports LSW.Files.SevenZip.Compression.RangeCoder
Imports System
Imports System.IO
Imports System.Runtime.InteropServices

Namespace Files.SevenZip.Compression.LZMA
    Public Class Decoder
        Implements ICoder, ISetDecoderProperties
        ' Methods
        Public Sub New()
            Dim i As Integer
            For i = 0 To 4 - 1
                Me.m_PosSlotDecoder(i) = New BitTreeDecoder(6)
            Next i
        End Sub

        Public Sub Code(ByVal inStream As Stream, ByVal outStream As Stream, ByVal inSize As Long, ByVal outSize As Long, ByVal progress As ICodeProgress) Implements ICoder.Code
            Me.Init(inStream, outStream)
            Dim state As New Base.State
            state.Init()
            Dim distance As UInt32 = 0
            Dim num2 As UInt32 = 0
            Dim num3 As UInt32 = 0
            Dim num4 As UInt32 = 0
            Dim num5 As UInt64 = 0
            Dim num6 As UInt64 = CULng(outSize)
            If (num5 < num6) Then
                If (Me.m_IsMatchDecoders((state.Index << 4)).Decode(Me.m_RangeDecoder) <> 0) Then
                    Throw New DataErrorException
                End If
                state.UpdateChar()
                Dim b As Byte = Me.m_LiteralDecoder.DecodeNormal(Me.m_RangeDecoder, 0, 0)
                Me.m_OutWindow.PutByte(b)
                num5 = (num5 + CULng(1))
            End If
            Do While (num5 < num6)
                Dim num11 As UInt32
                Dim num12 As UInt32
                Dim posState As UInt32 = (CType(num5, UInt32) And Me.m_PosStateMask)
                If (Me.m_IsMatchDecoders(((state.Index << 4) + posState)).Decode(Me.m_RangeDecoder) = 0) Then
                    Dim num9 As Byte
                    Dim [Byte] As Byte = Me.m_OutWindow.GetByte(0)
                    If Not state.IsCharState Then
                        num9 = Me.m_LiteralDecoder.DecodeWithMatchByte(Me.m_RangeDecoder, CType(num5, UInt32), [Byte], Me.m_OutWindow.GetByte(distance))
                    Else
                        num9 = Me.m_LiteralDecoder.DecodeNormal(Me.m_RangeDecoder, CType(num5, UInt32), [Byte])
                    End If
                    Me.m_OutWindow.PutByte(num9)
                    state.UpdateChar()
                    num5 = (num5 + CULng(1))
                    Continue Do
                End If
                If (Me.m_IsRepDecoders(state.Index).Decode(Me.m_RangeDecoder) <> 1) Then
                    GoTo Label_0245
                End If
                If (Me.m_IsRepG0Decoders(state.Index).Decode(Me.m_RangeDecoder) = 0) Then
                    If (Me.m_IsRep0LongDecoders(((state.Index << 4) + posState)).Decode(Me.m_RangeDecoder) <> 0) Then
                        GoTo Label_0222
                    End If
                    state.UpdateShortRep()
                    Me.m_OutWindow.PutByte(Me.m_OutWindow.GetByte(distance))
                    num5 = (num5 + CULng(1))
                    Continue Do
                End If
                If (Me.m_IsRepG1Decoders(state.Index).Decode(Me.m_RangeDecoder) = 0) Then
                    num12 = num2
                Else
                    If (Me.m_IsRepG2Decoders(state.Index).Decode(Me.m_RangeDecoder) = 0) Then
                        num12 = num3
                    Else
                        num12 = num4
                        num4 = num3
                    End If
                    num3 = num2
                End If
                num2 = distance
                distance = num12
Label_0222:
                num11 = (Me.m_RepLenDecoder.Decode(Me.m_RangeDecoder, posState) + 2)
                state.UpdateRep()
                GoTo Label_02F5
Label_0245:
                num4 = num3
                num3 = num2
                num2 = distance
                num11 = (2 + Me.m_LenDecoder.Decode(Me.m_RangeDecoder, posState))
                state.UpdateMatch()
                Dim num13 As UInt32 = Me.m_PosSlotDecoder(Base.GetLenToPosState(num11)).Decode(Me.m_RangeDecoder)
                If (num13 >= 4) Then
                    Dim numBitLevels As Integer = (CInt((num13 >> 1)) - 1)
                    distance = CType(((2 Or (num13 And 1)) << (numBitLevels And &H1F)), UInt32)
                    If (num13 < 14) Then
                        distance = (distance + BitTreeDecoder.ReverseDecode(Me.m_PosDecoders, ((distance - num13) - 1), Me.m_RangeDecoder, numBitLevels))
                    Else
                        distance = (distance + (Me.m_RangeDecoder.DecodeDirectBits((numBitLevels - 4)) << 4))
                        distance = (distance + Me.m_PosAlignDecoder.ReverseDecode(Me.m_RangeDecoder))
                    End If
                Else
                    distance = num13
                End If
Label_02F5:
                If ((distance >= num5) OrElse (distance >= Me.m_DictionarySizeCheck)) Then
                    If (distance <> UInt32.MaxValue) Then
                        Throw New DataErrorException
                    End If
                    Exit Do
                End If
                Me.m_OutWindow.CopyBlock(distance, num11)
                num5 = (num5 + num11)
            Loop
            Me.m_OutWindow.Flush()
            Me.m_OutWindow.ReleaseStream()
            Me.m_RangeDecoder.ReleaseStream()
        End Sub

        Private Sub Init(ByVal inStream As Stream, ByVal outStream As Stream)
            Dim num As UInt32
            Me.m_RangeDecoder.Init(inStream)
            Me.m_OutWindow.Init(outStream)
            For num = 0 To 12 - 1
                Dim i As UInt32 = 0
                Do While (i <= Me.m_PosStateMask)
                    Dim index As UInt32 = ((num << 4) + i)
                    Me.m_IsMatchDecoders(index).Init()
                    Me.m_IsRep0LongDecoders(index).Init()
                    i += 1
                Loop
                Me.m_IsRepDecoders(num).Init()
                Me.m_IsRepG0Decoders(num).Init()
                Me.m_IsRepG1Decoders(num).Init()
                Me.m_IsRepG2Decoders(num).Init()
            Next num
            Me.m_LiteralDecoder.Init()
            For num = 0 To 4 - 1
                Me.m_PosSlotDecoder(num).Init()
            Next num
            For num = 0 To &H72 - 1
                Me.m_PosDecoders(num).Init()
            Next num
            Me.m_LenDecoder.Init()
            Me.m_RepLenDecoder.Init()
            Me.m_PosAlignDecoder.Init()
        End Sub

        Public Sub SetDecoderProperties(ByVal properties As Byte()) Implements ISetDecoderProperties.SetDecoderProperties
            If (properties.Length < 5) Then
                Throw New InvalidParamException
            End If
            Dim lc As Integer = (properties(0) Mod 9)
            Dim num2 As Integer = (properties(0) / 9)
            Dim lp As Integer = (num2 Mod 5)
            Dim pb As Integer = (num2 / 5)
            If (pb > 4) Then
                Throw New InvalidParamException
            End If
            Dim dictionarySize As UInt32 = 0
            Dim i As Integer
            For i = 0 To 4 - 1
                dictionarySize = (dictionarySize + CType((properties((1 + i)) << (i * 8)), UInt32))
            Next i
            Me.SetDictionarySize(dictionarySize)
            Me.SetLiteralProperties(lp, lc)
            Me.SetPosBitsProperties(pb)
        End Sub

        Private Sub SetDictionarySize(ByVal dictionarySize As UInt32)
            If (Me.m_DictionarySize <> dictionarySize) Then
                Me.m_DictionarySize = dictionarySize
                Me.m_DictionarySizeCheck = System.Math.Max(Me.m_DictionarySize, 1)
                Dim windowSize As UInt32 = System.Math.Max(Me.m_DictionarySizeCheck, &H1000)
                Me.m_OutWindow.Create(windowSize)
            End If
        End Sub

        Private Sub SetLiteralProperties(ByVal lp As Integer, ByVal lc As Integer)
            If (lp > 8) Then
                Throw New InvalidParamException
            End If
            If (lc > 8) Then
                Throw New InvalidParamException
            End If
            Me.m_LiteralDecoder.Create(lp, lc)
        End Sub

        Private Sub SetPosBitsProperties(ByVal pb As Integer)
            If (pb > 4) Then
                Throw New InvalidParamException
            End If
            Dim numPosStates As UInt32 = (1 << pb)
            Me.m_LenDecoder.Create(numPosStates)
            Me.m_RepLenDecoder.Create(numPosStates)
            Me.m_PosStateMask = (numPosStates - 1)
        End Sub


        ' Fields
        Private m_DictionarySize As UInt32 = UInt32.MaxValue
        Private m_DictionarySizeCheck As UInt32
        Private m_IsMatchDecoders As BitDecoder() = New BitDecoder(&HC0 - 1) {}
        Private m_IsRep0LongDecoders As BitDecoder() = New BitDecoder(&HC0 - 1) {}
        Private m_IsRepDecoders As BitDecoder() = New BitDecoder(12 - 1) {}
        Private m_IsRepG0Decoders As BitDecoder() = New BitDecoder(12 - 1) {}
        Private m_IsRepG1Decoders As BitDecoder() = New BitDecoder(12 - 1) {}
        Private m_IsRepG2Decoders As BitDecoder() = New BitDecoder(12 - 1) {}
        Private m_LenDecoder As LenDecoder = New LenDecoder
        Private m_LiteralDecoder As LiteralDecoder = New LiteralDecoder
        Private m_OutWindow As OutWindow = New OutWindow
        Private m_PosAlignDecoder As BitTreeDecoder = New BitTreeDecoder(4)
        Private m_PosDecoders As BitDecoder() = New BitDecoder(&H72 - 1) {}
        Private m_PosSlotDecoder As BitTreeDecoder() = New BitTreeDecoder(4 - 1) {}
        Private m_PosStateMask As UInt32
        Private m_RangeDecoder As New LSW.Files.SevenZip.Compression.RangeCoder.Decoder
        Private m_RepLenDecoder As LenDecoder = New LenDecoder

        ' Nested Types
        Private Class LenDecoder
            ' Methods
            Public Sub Create(ByVal numPosStates As UInt32)
                Dim i As UInt32
                For i = Me.m_NumPosStates To numPosStates - 1
                    Me.m_LowCoder(i) = New BitTreeDecoder(3)
                    Me.m_MidCoder(i) = New BitTreeDecoder(3)
                Next i
                Me.m_NumPosStates = numPosStates
            End Sub

            Public Function Decode(ByVal rangeDecoder As LSW.Files.SevenZip.Compression.RangeCoder.Decoder, ByVal posState As UInt32) As UInt32
                If (Me.m_Choice.Decode(rangeDecoder) = 0) Then
                    Return Me.m_LowCoder(posState).Decode(rangeDecoder)
                End If
                Dim num As UInt32 = 8
                If (Me.m_Choice2.Decode(rangeDecoder) = 0) Then
                    Return (num + Me.m_MidCoder(posState).Decode(rangeDecoder))
                End If
                num = (num + 8)
                Return (num + Me.m_HighCoder.Decode(rangeDecoder))
            End Function

            Public Sub Init()
                Me.m_Choice.Init()
                Dim i As UInt32
                For i = 0 To Me.m_NumPosStates - 1
                    Me.m_LowCoder(i).Init()
                    Me.m_MidCoder(i).Init()
                Next i
                Me.m_Choice2.Init()
                Me.m_HighCoder.Init()
            End Sub


            ' Fields
            Private m_Choice As BitDecoder = New BitDecoder
            Private m_Choice2 As BitDecoder = New BitDecoder
            Private m_HighCoder As BitTreeDecoder = New BitTreeDecoder(8)
            Private m_LowCoder As BitTreeDecoder() = New BitTreeDecoder(&H10 - 1) {}
            Private m_MidCoder As BitTreeDecoder() = New BitTreeDecoder(&H10 - 1) {}
            Private m_NumPosStates As UInt32
        End Class

        Private Class LiteralDecoder
            ' Methods
            Public Sub Create(ByVal numPosBits As Integer, ByVal numPrevBits As Integer)
                If (((Me.m_Coders Is Nothing) OrElse (Me.m_NumPrevBits <> numPrevBits)) OrElse (Me.m_NumPosBits <> numPosBits)) Then
                    Me.m_NumPosBits = numPosBits
                    Me.m_PosMask = CType(((CInt(1) << numPosBits) - 1), UInt32)
                    Me.m_NumPrevBits = numPrevBits
                    Dim num As UInt32 = (CType(1, UInt32) << (Me.m_NumPrevBits + Me.m_NumPosBits))
                    Me.m_Coders = New Decoder2(num - 1) {}
                    Dim i As UInt32
                    For i = 0 To num - 1
                        Me.m_Coders(i).Create()
                    Next i
                End If
            End Sub

            Public Function DecodeNormal(ByVal rangeDecoder As LSW.Files.SevenZip.Compression.RangeCoder.Decoder, ByVal pos As UInt32, ByVal prevByte As Byte) As Byte
                Return Me.m_Coders(Me.GetState(pos, prevByte)).DecodeNormal(rangeDecoder)
            End Function

            Public Function DecodeWithMatchByte(ByVal rangeDecoder As LSW.Files.SevenZip.Compression.RangeCoder.Decoder, ByVal pos As UInt32, ByVal prevByte As Byte, ByVal matchByte As Byte) As Byte
                Return Me.m_Coders(Me.GetState(pos, prevByte)).DecodeWithMatchByte(rangeDecoder, matchByte)
            End Function

            Private Function GetState(ByVal pos As UInt32, ByVal prevByte As Byte) As UInt32
                Return (((pos And Me.m_PosMask) << Me.m_NumPrevBits) + CType((prevByte >> (8 - Me.m_NumPrevBits)), UInt32))
            End Function

            Public Sub Init()
                Dim num As UInt32 = (1 << (Me.m_NumPrevBits + Me.m_NumPosBits))
                Dim i As UInt32
                For i = 0 To num - 1
                    Me.m_Coders(i).Init()
                Next i
            End Sub


            ' Fields
            Private m_Coders As Decoder2()
            Private m_NumPosBits As Integer
            Private m_NumPrevBits As Integer
            Private m_PosMask As UInt32

            ' Nested Types
            <StructLayout(LayoutKind.Sequential)> _
            Private Structure Decoder2
                Private m_Decoders As BitDecoder()
                Public Sub Create()
                    Me.m_Decoders = New BitDecoder(&H300 - 1) {}
                End Sub

                Public Sub Init()
                    Dim i As Integer
                    For i = 0 To &H300 - 1
                        Me.m_Decoders(i).Init()
                    Next i
                End Sub

                Public Function DecodeNormal(ByVal rangeDecoder As LSW.Files.SevenZip.Compression.RangeCoder.Decoder) As Byte
                    Dim index As UInt32 = 1
                    Do
                        index = ((index << 1) Or Me.m_Decoders(index).Decode(rangeDecoder))
                    Loop While (index < &H100)
                    Return CByte(index)
                End Function

                Public Function DecodeWithMatchByte(ByVal rangeDecoder As LSW.Files.SevenZip.Compression.RangeCoder.Decoder, ByVal matchByte As Byte) As Byte
                    Dim index As UInt32 = 1
                    Do
                        Dim num2 As UInt32 = CType(((matchByte >> 7) And 1), UInt32)
                        matchByte = CByte((matchByte << 1))
                        Dim num3 As UInt32 = Me.m_Decoders(CInt(CType((((1 + num2) << 8) + index), IntPtr))).Decode(rangeDecoder)
                        index = ((index << 1) Or num3)
                        If (num2 <> num3) Then
                            Do While (index < &H100)
                                index = ((index << 1) Or Me.m_Decoders(index).Decode(rangeDecoder))
                            Loop
                            Exit Do
                        End If
                    Loop While (index < &H100)
                    Return CByte(index)
                End Function
            End Structure
        End Class
    End Class
End Namespace


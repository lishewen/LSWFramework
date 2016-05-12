Imports LSW.Files.SevenZip
Imports LSW.Files.SevenZip.Compression.LZ
Imports LSW.Files.SevenZip.Compression.RangeCoder
Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports LSW.Extension

Namespace Files.SevenZip.Compression.LZMA
    Public Class Encoder
        Implements ICoder, ISetCoderProperties, IWriteCoderProperties
        ' Methods
        Shared Sub New()
            Dim index As Integer = 2
            Encoder.g_FastPos(0) = 0
            Encoder.g_FastPos(1) = 1
            Dim i As Byte = 2
            Do While (i < &H16)
                Dim num3 As UInt32 = (CType(1, UInt32) << ((i >> 1) - 1))
                Dim num4 As UInt32 = 0
                Do While (num4 < num3)
                    Encoder.g_FastPos(index) = i
                    num4 += 1
                    index += 1
                Loop
                i = CByte((i + 1))
            Loop
        End Sub

        Public Sub New()
            Dim i As Integer
            For i = 0 To &H1000 - 1
                Me._optimum(i) = New Optimal
            Next i
            Dim j As Integer
            For j = 0 To 4 - 1
                Me._posSlotEncoder(j) = New BitTreeEncoder(6)
            Next j
        End Sub

        Private Function Backward(<Out> ByRef backRes As UInt32, ByVal cur As UInt32) As UInt32
            Me._optimumEndIndex = cur
            Dim posPrev As UInt32 = Me._optimum(cur).PosPrev
            Dim backPrev As UInt32 = Me._optimum(cur).BackPrev
            Do
                If Me._optimum(cur).Prev1IsChar Then
                    Me._optimum(posPrev).MakeAsChar()
                    Me._optimum(posPrev).PosPrev = (posPrev - 1)
                    If Me._optimum(cur).Prev2 Then
                        Me._optimum(CInt(CType((posPrev - 1), IntPtr))).Prev1IsChar = False
                        Me._optimum(CInt(CType((posPrev - 1), IntPtr))).PosPrev = Me._optimum(cur).PosPrev2
                        Me._optimum(CInt(CType((posPrev - 1), IntPtr))).BackPrev = Me._optimum(cur).BackPrev2
                    End If
                End If
                Dim index As UInt32 = posPrev
                Dim num4 As UInt32 = backPrev
                backPrev = Me._optimum(index).BackPrev
                posPrev = Me._optimum(index).PosPrev
                Me._optimum(index).BackPrev = num4
                Me._optimum(index).PosPrev = cur
                cur = index
            Loop While (cur > 0)
            backRes = Me._optimum(0).BackPrev
            Me._optimumCurrentIndex = Me._optimum(0).PosPrev
            Return Me._optimumCurrentIndex
        End Function

        Private Sub BaseInit()
            Me._state.Init()
            Me._previousByte = 0
            Dim i As UInt32
            For i = 0 To 4 - 1
                Me._repDistances(i) = 0
            Next i
        End Sub

        Private Function ChangePair(ByVal smallDist As UInt32, ByVal bigDist As UInt32) As Boolean
            Return ((smallDist < &H2000000) AndAlso (bigDist >= (smallDist << 7)))
        End Function

        Public Sub Code(ByVal inStream As Stream, ByVal outStream As Stream, ByVal inSize As Long, ByVal outSize As Long, ByVal progress As ICodeProgress) Implements ICoder.Code
            Me._needReleaseMFStream = False
            Try
                Dim num As Long
                Dim num2 As Long
                Dim flag As Boolean
                Me.SetStreams(inStream, outStream, inSize, outSize)
Label_0012:
                Me.CodeOneBlock(num, num2, flag)
                If Not flag Then
                    If (Not progress Is Nothing) Then
                        progress.SetProgress(num, num2)
                    End If
                    GoTo Label_0012
                End If
            Finally
                Me.ReleaseStreams()
            End Try
        End Sub

        Public Sub CodeOneBlock(<Out> ByRef inSize As Long, <Out> ByRef outSize As Long, <Out> ByRef finished As Boolean)
            Dim num6 As UInt32
            Dim num7 As UInt32
            inSize = 0
            outSize = 0
            finished = True
            If (Not Me._inStream Is Nothing) Then
                Me._matchFinder.SetStream(Me._inStream)
                Me._matchFinder.Init()
                Me._needReleaseMFStream = True
                Me._inStream = Nothing
            End If
            If Me._finished Then
                Return
            End If
            Me._finished = True
            Dim num As Long = Me.nowPos64
            If (Me.nowPos64 = 0) Then
                Dim num2 As UInt32
                Dim num3 As UInt32
                If (Me._matchFinder.GetNumAvailableBytes = 0) Then
                    Me.Flush(CType(Me.nowPos64, UInt32))
                    Return
                End If
                Me.ReadMatchDistances(num2, num3)
                Dim num4 As UInt32 = (CType(Me.nowPos64, UInt32) And Me._posStateMask)
                Me._isMatch(((Me._state.Index << 4) + num4)).Encode(Me._rangeEncoder, 0)
                Me._state.UpdateChar()
                Dim indexByte As Byte = Me._matchFinder.GetIndexByte((0 - CInt(Me._additionalOffset)))
                Me._literalEncoder.GetSubCoder(CType(Me.nowPos64, UInt32), Me._previousByte).Encode(Me._rangeEncoder, indexByte)
                Me._previousByte = indexByte
                Me._additionalOffset -= 1
                Me.nowPos64 = (Me.nowPos64 + 1)
            End If
            If (Me._matchFinder.GetNumAvailableBytes = 0) Then
                Me.Flush(CType(Me.nowPos64, UInt32))
                Return
            End If
Label_0145:
            num7 = Me.GetOptimum(CType(Me.nowPos64, UInt32), num6)
            Dim posState As UInt32 = (CType(Me.nowPos64, UInt32) And Me._posStateMask)
            Dim index As UInt32 = ((Me._state.Index << 4) + posState)
            If ((num7 = 1) AndAlso (num6 = UInt32.MaxValue)) Then
                Me._isMatch(index).Encode(Me._rangeEncoder, 0)
                Dim symbol As Byte = Me._matchFinder.GetIndexByte((0 - CInt(Me._additionalOffset)))
                Dim subCoder = Me._literalEncoder.GetSubCoder(CType(Me.nowPos64, UInt32), Me._previousByte)
                If Not Me._state.IsCharState Then
                    Dim matchByte As Byte = Me._matchFinder.GetIndexByte(CInt((((0 - Me._repDistances(0)) - 1) - Me._additionalOffset)))
                    subCoder.EncodeMatched(Me._rangeEncoder, matchByte, symbol)
                Else
                    subCoder.Encode(Me._rangeEncoder, symbol)
                End If
                Me._previousByte = symbol
                Me._state.UpdateChar()
            Else
                Me._isMatch(index).Encode(Me._rangeEncoder, 1)
                If (num6 < 4) Then
                    Me._isRep(Me._state.Index).Encode(Me._rangeEncoder, 1)
                    If (num6 = 0) Then
                        Me._isRepG0(Me._state.Index).Encode(Me._rangeEncoder, 0)
                        If (num7 = 1) Then
                            Me._isRep0Long(index).Encode(Me._rangeEncoder, 0)
                        Else
                            Me._isRep0Long(index).Encode(Me._rangeEncoder, 1)
                        End If
                    Else
                        Me._isRepG0(Me._state.Index).Encode(Me._rangeEncoder, 1)
                        If (num6 = 1) Then
                            Me._isRepG1(Me._state.Index).Encode(Me._rangeEncoder, 0)
                        Else
                            Me._isRepG1(Me._state.Index).Encode(Me._rangeEncoder, 1)
                            Me._isRepG2(Me._state.Index).Encode(Me._rangeEncoder, (num6 - 2))
                        End If
                    End If
                    If (num7 = 1) Then
                        Me._state.UpdateShortRep()
                    Else
                        Me._repMatchLenEncoder.Encode(Me._rangeEncoder, (num7 - 2), posState)
                        Me._state.UpdateRep()
                    End If
                    Dim num12 As UInt32 = Me._repDistances(num6)
                    If (num6 <> 0) Then
                        Dim i As UInt32 = num6
                        Do While (i >= 1)
                            Me._repDistances(i) = Me._repDistances(CInt(CType((i - 1), IntPtr)))
                            i -= 1
                        Loop
                        Me._repDistances(0) = num12
                    End If
                Else
                    Me._isRep(Me._state.Index).Encode(Me._rangeEncoder, 0)
                    Me._state.UpdateMatch()
                    Me._lenEncoder.Encode(Me._rangeEncoder, (num7 - 2), posState)
                    num6 = (num6 - 4)
                    Dim posSlot As UInt32 = Encoder.GetPosSlot(num6)
                    Dim lenToPosState As UInt32 = Base.GetLenToPosState(num7)
                    Me._posSlotEncoder(lenToPosState).Encode(Me._rangeEncoder, posSlot)
                    If (posSlot >= 4) Then
                        Dim numBitLevels As Integer = (CInt((posSlot >> 1)) - 1)
                        Dim num17 As UInt32 = CType(((2 Or (posSlot And 1)) << (numBitLevels And &H1F)), UInt32)
                        Dim num18 As UInt32 = (num6 - num17)
                        If (posSlot < 14) Then
                            BitTreeEncoder.ReverseEncode(Me._posEncoders, ((num17 - posSlot) - 1), Me._rangeEncoder, numBitLevels, num18)
                        Else
                            Me._rangeEncoder.EncodeDirectBits((num18 >> 4), (numBitLevels - 4))
                            Me._posAlignEncoder.ReverseEncode(Me._rangeEncoder, (num18 And 15))
                            Me._alignPriceCount += 1
                        End If
                    End If
                    Dim num19 As UInt32 = num6
                    Dim j As UInt32 = 3
                    Do While (j >= 1)
                        Me._repDistances(j) = Me._repDistances(CInt(CType((j - 1), IntPtr)))
                        j -= 1
                    Loop
                    Me._repDistances(0) = num19
                    Me._matchPriceCount += 1
                End If
                Me._previousByte = Me._matchFinder.GetIndexByte(CInt(((num7 - 1) - Me._additionalOffset)))
            End If
            Me._additionalOffset = (Me._additionalOffset - num7)
            Me.nowPos64 = (Me.nowPos64 + num7)
            If (Me._additionalOffset <> 0) Then
                GoTo Label_0145
            End If
            If (Me._matchPriceCount >= &H80) Then
                Me.FillDistancesPrices()
            End If
            If (Me._alignPriceCount >= &H10) Then
                Me.FillAlignPrices()
            End If
            inSize = Me.nowPos64
            outSize = Me._rangeEncoder.GetProcessedSizeAdd
            If (Me._matchFinder.GetNumAvailableBytes = 0) Then
                Me.Flush(CType(Me.nowPos64, UInt32))
            Else
                If ((Me.nowPos64 - num) < &H1000) Then
                    GoTo Label_0145
                End If
                Me._finished = False
                finished = False
            End If
        End Sub

        Private Sub Create()
            If (Me._matchFinder Is Nothing) Then
                Dim tree As New BinTree
                Dim numHashBytes As Integer = 4
                If (Me._matchFinderType = EMatchFinderType.BT2) Then
                    numHashBytes = 2
                End If
                tree.SetType(numHashBytes)
                Me._matchFinder = tree
            End If
            Me._literalEncoder.Create(Me._numLiteralPosStateBits, Me._numLiteralContextBits)
            If ((Me._dictionarySize <> Me._dictionarySizePrev) OrElse (Me._numFastBytesPrev <> Me._numFastBytes)) Then
                Me._matchFinder.Create(Me._dictionarySize, &H1000, Me._numFastBytes, &H112)
                Me._dictionarySizePrev = Me._dictionarySize
                Me._numFastBytesPrev = Me._numFastBytes
            End If
        End Sub

        Private Sub FillAlignPrices()
            Dim i As UInt32
            For i = 0 To &H10 - 1
                Me._alignPrices(i) = Me._posAlignEncoder.ReverseGetPrice(i)
            Next i
            Me._alignPriceCount = 0
        End Sub

        Private Sub FillDistancesPrices()
            Dim i As UInt32
            For i = 4 To &H80 - 1
                Dim posSlot As UInt32 = Encoder.GetPosSlot(i)
                Dim numBitLevels As Integer = (CInt((posSlot >> 1)) - 1)
                Dim num4 As UInt32 = CType(((2 Or (posSlot And 1)) << (numBitLevels And &H1F)), UInt32)
                Me.tempPrices(i) = BitTreeEncoder.ReverseGetPrice(Me._posEncoders, ((num4 - posSlot) - 1), numBitLevels, (i - num4))
            Next i
            Dim j As UInt32
            For j = 0 To 4 - 1
                Dim encoder As BitTreeEncoder = Me._posSlotEncoder(j)
                Dim num7 As UInt32 = (j << 6)
                Dim symbol As UInt32 = 0
                Do While (symbol < Me._distTableSize)
                    Me._posSlotPrices((num7 + symbol)) = encoder.GetPrice(symbol)
                    symbol += 1
                Loop
                For symbol = 14 To Me._distTableSize - 1
                    Me._posSlotPrices((num7 + symbol)) = (Me._posSlotPrices((num7 + symbol)) + CType(((((symbol >> 1) - 1) - 4) << 6), UInt32))
                Next symbol
                Dim num8 As UInt32 = (j * &H80)
                Dim pos As UInt32 = 0
                Do While (pos < 4)
                    Me._distancesPrices((num8 + pos)) = Me._posSlotPrices((num7 + pos))
                    pos += 1
                Loop
                Do While (pos < &H80)
                    Me._distancesPrices((num8 + pos)) = (Me._posSlotPrices((num7 + LZMA.Encoder.GetPosSlot(pos))) + Me.tempPrices(pos))
                    pos += 1
                Loop
            Next j
            Me._matchPriceCount = 0
        End Sub

        Private Shared Function FindMatchFinder(ByVal s As String) As Integer
            Dim i As Integer
            For i = 0 To Encoder.kMatchFinderIDs.Length - 1
                If (s = Encoder.kMatchFinderIDs(i)) Then
                    Return i
                End If
            Next i
            Return -1
        End Function

        Private Sub Flush(ByVal nowPos As UInt32)
            Me.ReleaseMFStream()
            Me.WriteEndMarker((nowPos And Me._posStateMask))
            Me._rangeEncoder.FlushData()
            Me._rangeEncoder.FlushStream()
        End Sub

        Private Function GetOptimum(ByVal position As UInt32, <Out> ByRef backRes As UInt32) As UInt32
            Dim num2 As UInt32
            Dim num3 As UInt32
            Dim num6 As UInt32
            Dim num24 As UInt32
            Dim state As Base.State
            If (Me._optimumEndIndex <> Me._optimumCurrentIndex) Then
                Dim num As UInt32 = (Me._optimum(Me._optimumCurrentIndex).PosPrev - Me._optimumCurrentIndex)
                backRes = Me._optimum(Me._optimumCurrentIndex).BackPrev
                Me._optimumCurrentIndex = Me._optimum(Me._optimumCurrentIndex).PosPrev
                Return num
            End If
            Me._optimumCurrentIndex = Me._optimumEndIndex = 0
            If Not Me._longestMatchWasFound Then
                Me.ReadMatchDistances(num2, num3)
            Else
                num2 = Me._longestMatchLength
                num3 = Me._numDistancePairs
                Me._longestMatchWasFound = False
            End If
            Dim limit As UInt32 = (Me._matchFinder.GetNumAvailableBytes + 1)
            If (limit < 2) Then
                backRes = UInt32.MaxValue
                Return 1
            End If
            If (limit > &H111) Then
                limit = &H111
            End If
            Dim index As UInt32 = 0
            For num6 = 0 To 4 - 1
                Me.reps(num6) = Me._repDistances(num6)
                Me.repLens(num6) = Me._matchFinder.GetMatchLen(-1, Me.reps(num6), &H111)
                If (Me.repLens(num6) > Me.repLens(index)) Then
                    index = num6
                End If
            Next num6
            If (Me.repLens(index) >= Me._numFastBytes) Then
                backRes = index
                Dim num7 As UInt32 = Me.repLens(index)
                Me.MovePos((num7 - 1))
                Return num7
            End If
            If (num2 >= Me._numFastBytes) Then
                backRes = (Me._matchDistances(CInt(CType((num3 - 1), IntPtr))) + 4)
                Me.MovePos((num2 - 1))
                Return num2
            End If
            Dim indexByte As Byte = Me._matchFinder.GetIndexByte(-1)
            Dim matchByte As Byte = Me._matchFinder.GetIndexByte((((0 - CInt(Me._repDistances(0))) - 1) - 1))
            If (((num2 < 2) AndAlso (indexByte <> matchByte)) AndAlso (Me.repLens(index) < 2)) Then
                backRes = UInt32.MaxValue
                Return 1
            End If
            Me._optimum(0).State = Me._state
            Dim posState As UInt32 = (position And Me._posStateMask)
            Me._optimum(1).Price = (Me._isMatch(((Me._state.Index << 4) + posState)).GetPrice0 + Me._literalEncoder.GetSubCoder(position, Me._previousByte).GetPrice(Not Me._state.IsCharState, matchByte, indexByte))
            Me._optimum(1).MakeAsChar()
            Dim num11 As UInt32 = Me._isMatch(((Me._state.Index << 4) + posState)).GetPrice1
            Dim num12 As UInt32 = (num11 + Me._isRep(Me._state.Index).GetPrice1)
            If (matchByte = indexByte) Then
                Dim num13 As UInt32 = (num12 + Me.GetRepLen1Price(Me._state, posState))
                If (num13 < Me._optimum(1).Price) Then
                    Me._optimum(1).Price = num13
                    Me._optimum(1).MakeAsShortRep()
                End If
            End If
            Dim num14 As UInt32 = If((num2 >= Me.repLens(index)), num2, Me.repLens(index))
            If (num14 < 2) Then
                backRes = Me._optimum(1).BackPrev
                Return 1
            End If
            Me._optimum(1).PosPrev = 0
            Me._optimum(0).Backs0 = Me.reps(0)
            Me._optimum(0).Backs1 = Me.reps(1)
            Me._optimum(0).Backs2 = Me.reps(2)
            Me._optimum(0).Backs3 = Me.reps(3)
            Dim len As UInt32 = num14
            Do
                Me._optimum(len).Price = &HFFFFFFF
                len -= 1
            Loop While (len >= 2)
            For num6 = 0 To 4 - 1
                Dim num16 As UInt32 = Me.repLens(num6)
                If (num16 >= 2) Then
                    Dim num17 As UInt32 = (num12 + Me.GetPureRepPrice(num6, Me._state, posState))
                    Do
                        Dim num18 As UInt32 = (num17 + Me._repMatchLenEncoder.GetPrice((num16 - 2), posState))
                        Dim optimal As Optimal = Me._optimum(num16)
                        If (num18 < optimal.Price) Then
                            optimal.Price = num18
                            optimal.PosPrev = 0
                            optimal.BackPrev = num6
                            optimal.Prev1IsChar = False
                        End If
                    Loop While (num16.InlineAssignHelper(num16 - 1) >= 2)
                End If
            Next num6
            Dim num19 As UInt32 = (num11 + Me._isRep(Me._state.Index).GetPrice0)
            len = If((Me.repLens(0) >= 2), (Me.repLens(0) + 1), 2)
            If (len <= num2) Then
                Dim num20 As UInt32 = 0
                Do While (len > Me._matchDistances(num20))
                    num20 = (num20 + 2)
                Loop
                Do While True
                    Dim pos As UInt32 = Me._matchDistances(CInt(CType((num20 + 1), IntPtr)))
                    Dim num22 As UInt32 = (num19 + Me.GetPosLenPrice(pos, len, posState))
                    Dim optimal2 As Optimal = Me._optimum(len)
                    If (num22 < optimal2.Price) Then
                        optimal2.Price = num22
                        optimal2.PosPrev = 0
                        optimal2.BackPrev = (pos + 4)
                        optimal2.Prev1IsChar = False
                    End If
                    If (len = Me._matchDistances(num20)) Then
                        num20 = (num20 + 2)
                        If (num20 = num3) Then
                            Exit Do
                        End If
                    End If
                    len += 1
                Loop
            End If
            Dim cur As UInt32 = 0
Label_04EB:
            cur += 1
            If (cur = num14) Then
                Return Me.Backward(backRes, cur)
            End If
            Me.ReadMatchDistances(num24, num3)
            If (num24 >= Me._numFastBytes) Then
                Me._numDistancePairs = num3
                Me._longestMatchLength = num24
                Me._longestMatchWasFound = True
                Return Me.Backward(backRes, cur)
            End If
            position += 1
            Dim posPrev As UInt32 = Me._optimum(cur).PosPrev
            If Me._optimum(cur).Prev1IsChar Then
                posPrev -= 1
                If Me._optimum(cur).Prev2 Then
                    state = Me._optimum(Me._optimum(cur).PosPrev2).State
                    If (Me._optimum(cur).BackPrev2 < 4) Then
                        state.UpdateRep()
                    Else
                        state.UpdateMatch()
                    End If
                Else
                    state = Me._optimum(posPrev).State
                End If
                state.UpdateChar()
            Else
                state = Me._optimum(posPrev).State
            End If
            If (posPrev = (cur - 1)) Then
                If Me._optimum(cur).IsShortRep Then
                    state.UpdateShortRep()
                Else
                    state.UpdateChar()
                End If
            Else
                Dim backPrev As UInt32
                If (Me._optimum(cur).Prev1IsChar AndAlso Me._optimum(cur).Prev2) Then
                    posPrev = Me._optimum(cur).PosPrev2
                    backPrev = Me._optimum(cur).BackPrev2
                    state.UpdateRep()
                Else
                    backPrev = Me._optimum(cur).BackPrev
                    If (backPrev < 4) Then
                        state.UpdateRep()
                    Else
                        state.UpdateMatch()
                    End If
                End If
                Dim optimal3 As Optimal = Me._optimum(posPrev)
                If (backPrev < 4) Then
                    If (backPrev = 0) Then
                        Me.reps(0) = optimal3.Backs0
                        Me.reps(1) = optimal3.Backs1
                        Me.reps(2) = optimal3.Backs2
                        Me.reps(3) = optimal3.Backs3
                    ElseIf (backPrev = 1) Then
                        Me.reps(0) = optimal3.Backs1
                        Me.reps(1) = optimal3.Backs0
                        Me.reps(2) = optimal3.Backs2
                        Me.reps(3) = optimal3.Backs3
                    ElseIf (backPrev = 2) Then
                        Me.reps(0) = optimal3.Backs2
                        Me.reps(1) = optimal3.Backs0
                        Me.reps(2) = optimal3.Backs1
                        Me.reps(3) = optimal3.Backs3
                    Else
                        Me.reps(0) = optimal3.Backs3
                        Me.reps(1) = optimal3.Backs0
                        Me.reps(2) = optimal3.Backs1
                        Me.reps(3) = optimal3.Backs2
                    End If
                Else
                    Me.reps(0) = (backPrev - 4)
                    Me.reps(1) = optimal3.Backs0
                    Me.reps(2) = optimal3.Backs1
                    Me.reps(3) = optimal3.Backs2
                End If
            End If
            Me._optimum(cur).State = state
            Me._optimum(cur).Backs0 = Me.reps(0)
            Me._optimum(cur).Backs1 = Me.reps(1)
            Me._optimum(cur).Backs2 = Me.reps(2)
            Me._optimum(cur).Backs3 = Me.reps(3)
            Dim price As UInt32 = Me._optimum(cur).Price
            indexByte = Me._matchFinder.GetIndexByte(-1)
            matchByte = Me._matchFinder.GetIndexByte((((0 - CInt(Me.reps(0))) - 1) - 1))
            posState = (position And Me._posStateMask)
            Dim num28 As UInt32 = ((price + Me._isMatch(((state.Index << 4) + posState)).GetPrice0) + Me._literalEncoder.GetSubCoder(position, Me._matchFinder.GetIndexByte(-2)).GetPrice(Not state.IsCharState, matchByte, indexByte))
            Dim optimal4 As Optimal = Me._optimum(CInt(CType((cur + 1), IntPtr)))
            Dim flag As Boolean = False
            If (num28 < optimal4.Price) Then
                optimal4.Price = num28
                optimal4.PosPrev = cur
                optimal4.MakeAsChar()
                flag = True
            End If
            num11 = (price + Me._isMatch(((state.Index << 4) + posState)).GetPrice1)
            num12 = (num11 + Me._isRep(state.Index).GetPrice1)
            If ((matchByte = indexByte) AndAlso ((optimal4.PosPrev >= cur) OrElse (optimal4.BackPrev <> 0))) Then
                Dim num29 As UInt32 = (num12 + Me.GetRepLen1Price(state, posState))
                If (num29 <= optimal4.Price) Then
                    optimal4.Price = num29
                    optimal4.PosPrev = cur
                    optimal4.MakeAsShortRep()
                    flag = True
                End If
            End If
            Dim num30 As UInt32 = (Me._matchFinder.GetNumAvailableBytes + 1)
            num30 = System.Math.Min((&HFFF - cur), num30)
            limit = num30
            If (limit < 2) Then
                GoTo Label_04EB
            End If
            If (limit > Me._numFastBytes) Then
                limit = Me._numFastBytes
            End If
            If (Not flag AndAlso (matchByte <> indexByte)) Then
                Dim num31 As UInt32 = System.Math.Min((num30 - 1), Me._numFastBytes)
                Dim num32 As UInt32 = Me._matchFinder.GetMatchLen(0, Me.reps(0), num31)
                If (num32 >= 2) Then
                    Dim state2 = state
                    state2.UpdateChar()
                    Dim num33 As UInt32 = ((position + 1) And Me._posStateMask)
                    Dim num34 As UInt32 = ((num28 + Me._isMatch(((state2.Index << 4) + num33)).GetPrice1) + Me._isRep(state2.Index).GetPrice1)
                    Dim num35 As UInt32 = ((cur + 1) + num32)
                    Do While (num14 < num35)
                        Me._optimum(CInt(CType(num14.InlineAssignHelper(num14 + 1), IntPtr))).Price = &HFFFFFFF
                    Loop
                    Dim num36 As UInt32 = (num34 + Me.GetRepPrice(0, num32, state2, num33))
                    Dim optimal5 As Optimal = Me._optimum(num35)
                    If (num36 < optimal5.Price) Then
                        optimal5.Price = num36
                        optimal5.PosPrev = (cur + 1)
                        optimal5.BackPrev = 0
                        optimal5.Prev1IsChar = True
                        optimal5.Prev2 = False
                    End If
                End If
            End If
            Dim num37 As UInt32 = 2
            Dim i As UInt32
            For i = 0 To 4 - 1
                Dim num39 As UInt32 = Me._matchFinder.GetMatchLen(-1, Me.reps(i), limit)
                If (num39 >= 2) Then
                    Dim num40 As UInt32 = num39
                    Do
                        Do While (num14 < (cur + num39))
                            Me._optimum(CInt(CType(num14.InlineAssignHelper(num14 + 1), IntPtr))).Price = &HFFFFFFF
                        Loop
                        Dim num41 As UInt32 = (num12 + Me.GetRepPrice(i, num39, state, posState))
                        Dim optimal6 As Optimal = Me._optimum((cur + num39))
                        If (num41 < optimal6.Price) Then
                            optimal6.Price = num41
                            optimal6.PosPrev = cur
                            optimal6.BackPrev = i
                            optimal6.Prev1IsChar = False
                        End If
                    Loop While (num39.InlineAssignHelper(num39 - 1) >= 2)
                    num39 = num40
                    If (i = 0) Then
                        num37 = (num39 + 1)
                    End If
                    If (num39 < num30) Then
                        Dim num42 As UInt32 = System.Math.Min(((num30 - 1) - num39), Me._numFastBytes)
                        Dim num43 As UInt32 = Me._matchFinder.GetMatchLen(CInt(num39), Me.reps(i), num42)
                        If (num43 >= 2) Then
                            Dim state3 = state
                            state3.UpdateRep()
                            Dim num44 As UInt32 = ((position + num39) And Me._posStateMask)
                            Dim num45 As UInt32 = (((num12 + Me.GetRepPrice(i, num39, state, posState)) + Me._isMatch(((state3.Index << 4) + num44)).GetPrice0) + Me._literalEncoder.GetSubCoder((position + num39), Me._matchFinder.GetIndexByte(((CInt(num39) - 1) - 1))).GetPrice(True, Me._matchFinder.GetIndexByte(CInt(((num39 - 1) - (Me.reps(i) + 1)))), Me._matchFinder.GetIndexByte((CInt(num39) - 1))))
                            state3.UpdateChar()
                            num44 = (((position + num39) + 1) And Me._posStateMask)
                            Dim num46 As UInt32 = (num45 + Me._isMatch(((state3.Index << 4) + num44)).GetPrice1)
                            Dim num47 As UInt32 = (num46 + Me._isRep(state3.Index).GetPrice1)
                            Dim num48 As UInt32 = ((num39 + 1) + num43)
                            Do While (num14 < (cur + num48))
                                Me._optimum(CInt(CType(num14.InlineAssignHelper(num14 + 1), IntPtr))).Price = &HFFFFFFF
                            Loop
                            Dim num49 As UInt32 = (num47 + Me.GetRepPrice(0, num43, state3, num44))
                            Dim optimal7 As Optimal = Me._optimum((cur + num48))
                            If (num49 < optimal7.Price) Then
                                optimal7.Price = num49
                                optimal7.PosPrev = ((cur + num39) + 1)
                                optimal7.BackPrev = 0
                                optimal7.Prev1IsChar = True
                                optimal7.Prev2 = True
                                optimal7.PosPrev2 = cur
                                optimal7.BackPrev2 = i
                            End If
                        End If
                    End If
                End If
            Next i
            If (num24 > limit) Then
                num24 = limit
                num3 = 0
                Do While (num24 > Me._matchDistances(num3))
                    num3 = (num3 + 2)
                Loop
                Me._matchDistances(num3) = num24
                num3 = (num3 + 2)
            End If
            If (num24 < num37) Then
                GoTo Label_04EB
            End If
            num19 = (num11 + Me._isRep(state.Index).GetPrice0)
            Do While (num14 < (cur + num24))
                Me._optimum(CInt(CType(num14.InlineAssignHelper(num14 + 1), IntPtr))).Price = &HFFFFFFF
            Loop
            Dim num50 As UInt32 = 0
            Do While (num37 > Me._matchDistances(num50))
                num50 = (num50 + 2)
            Loop
            Dim num51 As UInt32 = num37
            Do While True
                Dim num52 As UInt32 = Me._matchDistances(CInt(CType((num50 + 1), IntPtr)))
                Dim num53 As UInt32 = (num19 + Me.GetPosLenPrice(num52, num51, posState))
                Dim optimal8 As Optimal = Me._optimum((cur + num51))
                If (num53 < optimal8.Price) Then
                    optimal8.Price = num53
                    optimal8.PosPrev = cur
                    optimal8.BackPrev = (num52 + 4)
                    optimal8.Prev1IsChar = False
                End If
                If (num51 = Me._matchDistances(num50)) Then
                    If (num51 < num30) Then
                        Dim num54 As UInt32 = System.Math.Min(((num30 - 1) - num51), Me._numFastBytes)
                        Dim num55 As UInt32 = Me._matchFinder.GetMatchLen(CInt(num51), num52, num54)
                        If (num55 >= 2) Then
                            Dim state4 = state
                            state4.UpdateMatch()
                            Dim num56 As UInt32 = ((position + num51) And Me._posStateMask)
                            Dim num57 As UInt32 = ((num53 + Me._isMatch(((state4.Index << 4) + num56)).GetPrice0) + Me._literalEncoder.GetSubCoder((position + num51), Me._matchFinder.GetIndexByte(((CInt(num51) - 1) - 1))).GetPrice(True, Me._matchFinder.GetIndexByte((CInt((num51 - (num52 + 1))) - 1)), Me._matchFinder.GetIndexByte((CInt(num51) - 1))))
                            state4.UpdateChar()
                            num56 = (((position + num51) + 1) And Me._posStateMask)
                            Dim num58 As UInt32 = (num57 + Me._isMatch(((state4.Index << 4) + num56)).GetPrice1)
                            Dim num59 As UInt32 = (num58 + Me._isRep(state4.Index).GetPrice1)
                            Dim num60 As UInt32 = ((num51 + 1) + num55)
                            Do While (num14 < (cur + num60))
                                Me._optimum(CInt(CType(num14.InlineAssignHelper(num14 + 1), IntPtr))).Price = &HFFFFFFF
                            Loop
                            num53 = (num59 + Me.GetRepPrice(0, num55, state4, num56))
                            optimal8 = Me._optimum((cur + num60))
                            If (num53 < optimal8.Price) Then
                                optimal8.Price = num53
                                optimal8.PosPrev = ((cur + num51) + 1)
                                optimal8.BackPrev = 0
                                optimal8.Prev1IsChar = True
                                optimal8.Prev2 = True
                                optimal8.PosPrev2 = cur
                                optimal8.BackPrev2 = (num52 + 4)
                            End If
                        End If
                    End If
                    num50 = (num50 + 2)
                    If (num50 = num3) Then
                        GoTo Label_04EB
                    End If
                End If
                num51 += 1
            Loop
            Return 0
        End Function

        Private Function GetPosLenPrice(ByVal pos As UInt32, ByVal len As UInt32, ByVal posState As UInt32) As UInt32
            Dim num As UInt32
            Dim lenToPosState As UInt32 = Base.GetLenToPosState(len)
            If (pos < &H80) Then
                num = Me._distancesPrices(CInt(CType(((lenToPosState * &H80) + pos), IntPtr)))
            Else
                num = (Me._posSlotPrices(((lenToPosState << 6) + Encoder.GetPosSlot2(pos))) + Me._alignPrices(CInt(CType((pos And 15), IntPtr))))
            End If
            Return (num + Me._lenEncoder.GetPrice((len - 2), posState))
        End Function

        Private Shared Function GetPosSlot(ByVal pos As UInt32) As UInt32
            If (pos < &H800) Then
                Return Encoder.g_FastPos(pos)
            End If
            If (pos < &H200000) Then
                Return CType((Encoder.g_FastPos((pos >> 10)) + 20), UInt32)
            End If
            Return CType((Encoder.g_FastPos((pos >> 20)) + 40), UInt32)
        End Function

        Private Shared Function GetPosSlot2(ByVal pos As UInt32) As UInt32
            If (pos < &H20000) Then
                Return CType((Encoder.g_FastPos((pos >> 6)) + 12), UInt32)
            End If
            If (pos < &H8000000) Then
                Return CType((Encoder.g_FastPos((pos >> &H10)) + &H20), UInt32)
            End If
            Return CType((Encoder.g_FastPos((pos >> &H1A)) + &H34), UInt32)
        End Function

        Private Function GetPureRepPrice(ByVal repIndex As UInt32, ByVal state As Base.State, ByVal posState As UInt32) As UInt32
            If (repIndex = 0) Then
                Return (Me._isRepG0(state.Index).GetPrice0 + Me._isRep0Long(((state.Index << 4) + posState)).GetPrice1)
            End If
            Dim num As UInt32 = Me._isRepG0(state.Index).GetPrice1
            If (repIndex = 1) Then
                Return (num + Me._isRepG1(state.Index).GetPrice0)
            End If
            num = (num + Me._isRepG1(state.Index).GetPrice1)
            Return (num + Me._isRepG2(state.Index).GetPrice((repIndex - 2)))
        End Function

        Private Function GetRepLen1Price(ByVal state As Base.State, ByVal posState As UInt32) As UInt32
            Return (Me._isRepG0(state.Index).GetPrice0 + Me._isRep0Long(((state.Index << 4) + posState)).GetPrice0)
        End Function

        Private Function GetRepPrice(ByVal repIndex As UInt32, ByVal len As UInt32, ByVal state As Base.State, ByVal posState As UInt32) As UInt32
            Return (Me._repMatchLenEncoder.GetPrice((len - 2), posState) + Me.GetPureRepPrice(repIndex, state, posState))
        End Function

        Private Sub Init()
            Dim num As UInt32
            Me.BaseInit()
            Me._rangeEncoder.Init()
            For num = 0 To 12 - 1
                Dim i As UInt32 = 0
                Do While (i <= Me._posStateMask)
                    Dim index As UInt32 = ((num << 4) + i)
                    Me._isMatch(index).Init()
                    Me._isRep0Long(index).Init()
                    i += 1
                Loop
                Me._isRep(num).Init()
                Me._isRepG0(num).Init()
                Me._isRepG1(num).Init()
                Me._isRepG2(num).Init()
            Next num
            Me._literalEncoder.Init()
            For num = 0 To 4 - 1
                Me._posSlotEncoder(num).Init()
            Next num
            For num = 0 To &H72 - 1
                Me._posEncoders(num).Init()
            Next num
            Me._lenEncoder.Init((CType(1, UInt32) << Me._posStateBits))
            Me._repMatchLenEncoder.Init((CType(1, UInt32) << Me._posStateBits))
            Me._posAlignEncoder.Init()
            Me._longestMatchWasFound = False
            Me._optimumEndIndex = 0
            Me._optimumCurrentIndex = 0
            Me._additionalOffset = 0
        End Sub

        Private Sub MovePos(ByVal num As UInt32)
            If (num > 0) Then
                Me._matchFinder.Skip(num)
                Me._additionalOffset = (Me._additionalOffset + num)
            End If
        End Sub

        Private Sub ReadMatchDistances(<Out> ByRef lenRes As UInt32, <Out> ByRef numDistancePairs As UInt32)
            lenRes = 0
            numDistancePairs = Me._matchFinder.GetMatches(Me._matchDistances)
            If (numDistancePairs > 0) Then
                lenRes = Me._matchDistances((numDistancePairs - 2))
                If (lenRes = Me._numFastBytes) Then
                    lenRes = (lenRes + Me._matchFinder.GetMatchLen((CInt(lenRes) - 1), Me._matchDistances((numDistancePairs - 1)), (&H111 - lenRes)))
                End If
            End If
            Me._additionalOffset += 1
        End Sub

        Private Sub ReleaseMFStream()
            If ((Not Me._matchFinder Is Nothing) AndAlso Me._needReleaseMFStream) Then
                Me._matchFinder.ReleaseStream()
                Me._needReleaseMFStream = False
            End If
        End Sub

        Private Sub ReleaseOutStream()
            Me._rangeEncoder.ReleaseStream()
        End Sub

        Private Sub ReleaseStreams()
            Me.ReleaseMFStream()
            Me.ReleaseOutStream()
        End Sub

        Public Sub SetCoderProperties(ByVal propIDs As CoderPropID(), ByVal properties As Object()) Implements ISetCoderProperties.SetCoderProperties
            Dim i As UInt32
            For i = 0 To properties.Length - 1
                Dim type As EMatchFinderType
                Dim num6 As Integer
                Dim num7 As Integer
                Dim num8 As Integer
                Dim obj2 As Object = properties(i)
                Select Case propIDs(i)
                    Case CoderPropID.NumFastBytes
                        If Not TypeOf obj2 Is Integer Then
                            Throw New InvalidParamException
                        End If
                        Exit Select
                    Case CoderPropID.MatchFinder
                        If Not TypeOf obj2 Is String Then
                            Throw New InvalidParamException
                        End If
                        GoTo Label_00BA
                    Case CoderPropID.Algorithm
                        Continue For
                    Case CoderPropID.EndMarker
                        If Not TypeOf obj2 Is Boolean Then
                            Throw New InvalidParamException
                        End If
                        Me.SetWriteEndMarkerMode(CBool(obj2))
                        Continue For
                    Case CoderPropID.PosStateBits
                        If Not TypeOf obj2 Is Integer Then
                            Throw New InvalidParamException
                        End If
                        GoTo Label_0183
                    Case CoderPropID.LitContextBits
                        If Not TypeOf obj2 Is Integer Then
                            Throw New InvalidParamException
                        End If
                        GoTo Label_01FD
                    Case CoderPropID.LitPosBits
                        If Not TypeOf obj2 Is Integer Then
                            Throw New InvalidParamException
                        End If
                        GoTo Label_01CB
                    Case CoderPropID.DictionarySize
                        GoTo Label_0110
                    Case Else
                        Throw New InvalidParamException
                End Select
                Dim num2 As Integer = CInt(obj2)
                If ((num2 < 5) OrElse (num2 > &H111)) Then
                    Throw New InvalidParamException
                End If
                Me._numFastBytes = CType(num2, UInt32)
                Continue For
Label_00BA:
                type = Me._matchFinderType
                Dim num3 As Integer = Encoder.FindMatchFinder(CStr(obj2).ToUpper)
                If (num3 < 0) Then
                    Throw New InvalidParamException
                End If
                Me._matchFinderType = DirectCast(num3, EMatchFinderType)
                If ((Not Me._matchFinder Is Nothing) AndAlso (type <> Me._matchFinderType)) Then
                    Me._dictionarySizePrev = UInt32.MaxValue
                    Me._matchFinder = Nothing
                End If
                Continue For
Label_0110:
                If Not TypeOf obj2 Is Integer Then
                    Throw New InvalidParamException
                End If
                Dim num4 As Integer = CInt(obj2)
                If ((num4 < 1) OrElse (num4 > &H40000000)) Then
                    Throw New InvalidParamException
                End If
                Me._dictionarySize = CType(num4, UInt32)
                Dim num5 As Integer = 0
                Do While (num5 < 30)
                    If (num4 <= (1 << num5)) Then
                        Exit Do
                    End If
                    num5 += 1
                Loop
                Me._distTableSize = CType((num5 * 2), UInt32)
                Continue For
Label_0183:
                num6 = CInt(obj2)
                If ((num6 < 0) OrElse (num6 > 4)) Then
                    Throw New InvalidParamException
                End If
                Me._posStateBits = num6
                Me._posStateMask = CType(((CInt(1) << Me._posStateBits) - 1), UInt32)
                Continue For
Label_01CB:
                num7 = CInt(obj2)
                If ((num7 < 0) OrElse (num7 > 4)) Then
                    Throw New InvalidParamException
                End If
                Me._numLiteralPosStateBits = num7
                Continue For
Label_01FD:
                num8 = CInt(obj2)
                If ((num8 < 0) OrElse (num8 > 8)) Then
                    Throw New InvalidParamException
                End If
                Me._numLiteralContextBits = num8
            Next i
        End Sub

        Private Sub SetOutStream(ByVal outStream As Stream)
            Me._rangeEncoder.SetStream(outStream)
        End Sub

        Private Sub SetStreams(ByVal inStream As Stream, ByVal outStream As Stream, ByVal inSize As Long, ByVal outSize As Long)
            Me._inStream = inStream
            Me._finished = False
            Me.Create()
            Me.SetOutStream(outStream)
            Me.Init()
            Me.FillDistancesPrices()
            Me.FillAlignPrices()
            Me._lenEncoder.SetTableSize(((Me._numFastBytes + 1) - 2))
            Me._lenEncoder.UpdateTables((1 << Me._posStateBits))
            Me._repMatchLenEncoder.SetTableSize(((Me._numFastBytes + 1) - 2))
            Me._repMatchLenEncoder.UpdateTables(1 << Me._posStateBits)
            Me.nowPos64 = 0
        End Sub

        Private Sub SetWriteEndMarkerMode(ByVal writeEndMarker As Boolean)
            Me._writeEndMark = writeEndMarker
        End Sub

        Public Sub WriteCoderProperties(ByVal outStream As Stream) Implements IWriteCoderProperties.WriteCoderProperties
            Me.properties(0) = CByte(((((Me._posStateBits * 5) + Me._numLiteralPosStateBits) * 9) + Me._numLiteralContextBits))
            Dim i As Integer
            For i = 0 To 4 - 1
                Me.properties((1 + i)) = CByte((Me._dictionarySize >> (8 * i)))
            Next i
            outStream.Write(Me.properties, 0, 5)
        End Sub

        Private Sub WriteEndMarker(ByVal posState As UInt32)
            If Me._writeEndMark Then
                Me._isMatch(((Me._state.Index << 4) + posState)).Encode(Me._rangeEncoder, 1)
                Me._isRep(Me._state.Index).Encode(Me._rangeEncoder, 0)
                Me._state.UpdateMatch()
                Dim len As UInt32 = 2
                Me._lenEncoder.Encode(Me._rangeEncoder, (len - 2), posState)
                Dim symbol As UInt32 = &H3F
                Dim lenToPosState As UInt32 = Base.GetLenToPosState(len)
                Me._posSlotEncoder(lenToPosState).Encode(Me._rangeEncoder, symbol)
                Dim num4 As Integer = 30
                Dim num5 As UInt32 = CType(((CInt(1) << num4) - 1), UInt32)
                Me._rangeEncoder.EncodeDirectBits((num5 >> 4), (num4 - 4))
                Me._posAlignEncoder.ReverseEncode(Me._rangeEncoder, (num5 And 15))
            End If
        End Sub


        ' Fields
        Private _additionalOffset As UInt32
        Private _alignPriceCount As UInt32
        Private _alignPrices As UInt32() = New UInt32(&H10 - 1) {}
        Private _dictionarySize As UInt32 = &H400000
        Private _dictionarySizePrev As UInt32 = UInt32.MaxValue
        Private _distancesPrices As UInt32() = New UInt32(&H200 - 1) {}
        Private _distTableSize As UInt32 = &H2C
        Private _finished As Boolean
        Private _inStream As Stream
        Private _isMatch As BitEncoder() = New BitEncoder(&HC0 - 1) {}
        Private _isRep As BitEncoder() = New BitEncoder(12 - 1) {}
        Private _isRep0Long As BitEncoder() = New BitEncoder(&HC0 - 1) {}
        Private _isRepG0 As BitEncoder() = New BitEncoder(12 - 1) {}
        Private _isRepG1 As BitEncoder() = New BitEncoder(12 - 1) {}
        Private _isRepG2 As BitEncoder() = New BitEncoder(12 - 1) {}
        Private _lenEncoder As LenPriceTableEncoder = New LenPriceTableEncoder
        Private _literalEncoder As LiteralEncoder = New LiteralEncoder
        Private _longestMatchLength As UInt32
        Private _longestMatchWasFound As Boolean
        Private _matchDistances As UInt32() = New UInt32(&H224 - 1) {}
        Private _matchFinder As IMatchFinder
        Private _matchFinderType As EMatchFinderType = EMatchFinderType.BT4
        Private _matchPriceCount As UInt32
        Private _needReleaseMFStream As Boolean
        Private _numDistancePairs As UInt32
        Private _numFastBytes As UInt32 = &H20
        Private _numFastBytesPrev As UInt32 = UInt32.MaxValue
        Private _numLiteralContextBits As Integer = 3
        Private _numLiteralPosStateBits As Integer
        Private _optimum As Optimal() = New Optimal(&H1000 - 1) {}
        Private _optimumCurrentIndex As UInt32
        Private _optimumEndIndex As UInt32
        Private _posAlignEncoder As BitTreeEncoder = New BitTreeEncoder(4)
        Private _posEncoders As BitEncoder() = New BitEncoder(&H72 - 1) {}
        Private _posSlotEncoder As BitTreeEncoder() = New BitTreeEncoder(4 - 1) {}
        Private _posSlotPrices As UInt32() = New UInt32(&H100 - 1) {}
        Private _posStateBits As Integer = 2
        Private _posStateMask As UInt32 = 3
        Private _previousByte As Byte
        Private _rangeEncoder As New LSW.Files.SevenZip.Compression.RangeCoder.Encoder
        Private _repDistances As UInt32() = New UInt32(4 - 1) {}
        Private _repMatchLenEncoder As LenPriceTableEncoder = New LenPriceTableEncoder
        Private _state As New Base.State
        Private _writeEndMark As Boolean
        Private Shared g_FastPos As Byte() = New Byte(&H800 - 1) {}
        Private Const kDefaultDictionaryLogSize As Integer = &H16
        Private Const kIfinityPrice As UInt32 = &HFFFFFFF
        Private Shared kMatchFinderIDs As String() = New String() {"BT2", "BT4"}
        Private Const kNumFastBytesDefault As UInt32 = &H20
        Private Const kNumLenSpecSymbols As UInt32 = &H10
        Private Const kNumOpts As UInt32 = &H1000
        Private Const kPropSize As Integer = 5
        Private nowPos64 As Long
        Private properties As Byte() = New Byte(5 - 1) {}
        Private repLens As UInt32() = New UInt32(4 - 1) {}
        Private reps As UInt32() = New UInt32(4 - 1) {}
        Private tempPrices As UInt32() = New UInt32(&H80 - 1) {}

        ' Nested Types
        Private Enum EMatchFinderType
            ' Fields
            BT2 = 0
            BT4 = 1
        End Enum

        Private Class LenEncoder
            ' Methods
            Public Sub New()
                Dim i As UInt32
                For i = 0 To &H10 - 1
                    Me._lowCoder(i) = New BitTreeEncoder(3)
                    Me._midCoder(i) = New BitTreeEncoder(3)
                Next i
            End Sub

            Public Sub Encode(ByVal rangeEncoder As LSW.Files.SevenZip.Compression.RangeCoder.Encoder, ByVal symbol As UInt32, ByVal posState As UInt32)
                If (symbol < 8) Then
                    Me._choice.Encode(rangeEncoder, 0)
                    Me._lowCoder(posState).Encode(rangeEncoder, symbol)
                Else
                    symbol = (symbol - 8)
                    Me._choice.Encode(rangeEncoder, 1)
                    If (symbol < 8) Then
                        Me._choice2.Encode(rangeEncoder, 0)
                        Me._midCoder(posState).Encode(rangeEncoder, symbol)
                    Else
                        Me._choice2.Encode(rangeEncoder, 1)
                        Me._highCoder.Encode(rangeEncoder, (symbol - 8))
                    End If
                End If
            End Sub

            Public Sub Init(ByVal numPosStates As UInt32)
                Me._choice.Init()
                Me._choice2.Init()
                Dim i As UInt32
                For i = 0 To numPosStates - 1
                    Me._lowCoder(i).Init()
                    Me._midCoder(i).Init()
                Next i
                Me._highCoder.Init()
            End Sub

            Public Sub SetPrices(ByVal posState As UInt32, ByVal numSymbols As UInt32, ByVal prices As UInt32(), ByVal st As UInt32)
                Dim num As UInt32 = Me._choice.GetPrice0
                Dim num2 As UInt32 = Me._choice.GetPrice1
                Dim num3 As UInt32 = (num2 + Me._choice2.GetPrice0)
                Dim num4 As UInt32 = (num2 + Me._choice2.GetPrice1)
                Dim symbol As UInt32 = 0
                symbol = 0
                Do While (symbol < 8)
                    If (symbol >= numSymbols) Then
                        Return
                    End If
                    prices((st + symbol)) = (num + Me._lowCoder(posState).GetPrice(symbol))
                    symbol += 1
                Loop
                Do While (symbol < &H10)
                    If (symbol >= numSymbols) Then
                        Return
                    End If
                    prices((st + symbol)) = (num3 + Me._midCoder(posState).GetPrice((symbol - 8)))
                    symbol += 1
                Loop
                Do While (symbol < numSymbols)
                    prices((st + symbol)) = (num4 + Me._highCoder.GetPrice(((symbol - 8) - 8)))
                    symbol += 1
                Loop
            End Sub


            ' Fields
            Private _choice As BitEncoder = New BitEncoder
            Private _choice2 As BitEncoder = New BitEncoder
            Private _highCoder As BitTreeEncoder = New BitTreeEncoder(8)
            Private _lowCoder As BitTreeEncoder() = New BitTreeEncoder(&H10 - 1) {}
            Private _midCoder As BitTreeEncoder() = New BitTreeEncoder(&H10 - 1) {}
        End Class

        Private Class LenPriceTableEncoder
            Inherits LenEncoder
            ' Methods
            Public Overloads Sub Encode(ByVal rangeEncoder As LSW.Files.SevenZip.Compression.RangeCoder.Encoder, ByVal symbol As UInt32, ByVal posState As UInt32)
                MyBase.Encode(rangeEncoder, symbol, posState)
                If (Me._counters(posState).InlineAssignHelper(Me._counters(posState) - 1) = 0) Then
                    Me.UpdateTable(posState)
                End If
            End Sub

            Public Function GetPrice(ByVal symbol As UInt32, ByVal posState As UInt32) As UInt32
                Return Me._prices(CInt(CType(((posState * &H110) + symbol), IntPtr)))
            End Function

            Public Sub SetTableSize(ByVal tableSize As UInt32)
                Me._tableSize = tableSize
            End Sub

            Private Sub UpdateTable(ByVal posState As UInt32)
                MyBase.SetPrices(posState, Me._tableSize, Me._prices, (posState * &H110))
                Me._counters(posState) = Me._tableSize
            End Sub

            Public Sub UpdateTables(ByVal numPosStates As UInt32)
                Dim i As UInt32
                For i = 0 To numPosStates - 1
                    Me.UpdateTable(i)
                Next i
            End Sub


            ' Fields
            Private _counters As UInt32() = New UInt32(&H10 - 1) {}
            Private _prices As UInt32() = New UInt32(&H1100 - 1) {}
            Private _tableSize As UInt32
        End Class

        Private Class LiteralEncoder
            ' Methods
            Public Sub Create(ByVal numPosBits As Integer, ByVal numPrevBits As Integer)
                If (((Me.m_Coders Is Nothing) OrElse (Me.m_NumPrevBits <> numPrevBits)) OrElse (Me.m_NumPosBits <> numPosBits)) Then
                    Me.m_NumPosBits = numPosBits
                    Me.m_PosMask = CType(((CInt(1) << numPosBits) - 1), UInt32)
                    Me.m_NumPrevBits = numPrevBits
                    Dim num As UInt32 = (1 << (Me.m_NumPrevBits + Me.m_NumPosBits))
                    Me.m_Coders = New Encoder2(num - 1) {}
                    Dim i As UInt32
                    For i = 0 To num - 1
                        Me.m_Coders(i).Create()
                    Next i
                End If
            End Sub

            Public Function GetSubCoder(ByVal pos As UInt32, ByVal prevByte As Byte) As Encoder2
                Return Me.m_Coders(CInt(CType((((pos And Me.m_PosMask) << Me.m_NumPrevBits) + (prevByte >> (8 - Me.m_NumPrevBits))), IntPtr)))
            End Function

            Public Sub Init()
                Dim num As UInt32 = (1 << (Me.m_NumPrevBits + Me.m_NumPosBits))
                Dim i As UInt32
                For i = 0 To num - 1
                    Me.m_Coders(i).Init()
                Next i
            End Sub


            ' Fields
            Private m_Coders As Encoder2()
            Private m_NumPosBits As Integer
            Private m_NumPrevBits As Integer
            Private m_PosMask As UInt32

            ' Nested Types
            <StructLayout(LayoutKind.Sequential)> _
            Public Structure Encoder2
                Private m_Encoders As BitEncoder()
                Public Sub Create()
                    Me.m_Encoders = New BitEncoder(&H300 - 1) {}
                End Sub

                Public Sub Init()
                    Dim i As Integer
                    For i = 0 To &H300 - 1
                        Me.m_Encoders(i).Init()
                    Next i
                End Sub

                Public Sub Encode(ByVal rangeEncoder As LSW.Files.SevenZip.Compression.RangeCoder.Encoder, ByVal symbol As Byte)
                    Dim index As UInt32 = 1
                    Dim i As Integer = 7
                    Do While (i >= 0)
                        Dim num3 As UInt32 = CType(((symbol >> i) And 1), UInt32)
                        Me.m_Encoders(index).Encode(rangeEncoder, num3)
                        index = ((index << 1) Or num3)
                        i -= 1
                    Loop
                End Sub

                Public Sub EncodeMatched(ByVal rangeEncoder As LSW.Files.SevenZip.Compression.RangeCoder.Encoder, ByVal matchByte As Byte, ByVal symbol As Byte)
                    Dim num As UInt32 = 1
                    Dim flag As Boolean = True
                    Dim i As Integer = 7
                    Do While (i >= 0)
                        Dim num3 As UInt32 = CType(((symbol >> i) And 1), UInt32)
                        Dim index As UInt32 = num
                        If flag Then
                            Dim num5 As UInt32 = CType(((matchByte >> i) And 1), UInt32)
                            index = (index + CType(((1 + num5) << 8), UInt32))
                            flag = (num5 = num3)
                        End If
                        Me.m_Encoders(index).Encode(rangeEncoder, num3)
                        num = ((num << 1) Or num3)
                        i -= 1
                    Loop
                End Sub

                Public Function GetPrice(ByVal matchMode As Boolean, ByVal matchByte As Byte, ByVal symbol As Byte) As UInt32
                    Dim num As UInt32 = 0
                    Dim index As UInt32 = 1
                    Dim num3 As Integer = 7
                    If matchMode Then
                        Do While (num3 >= 0)
                            Dim num4 As UInt32 = CType(((matchByte >> num3) And 1), UInt32)
                            Dim num5 As UInt32 = CType(((symbol >> num3) And 1), UInt32)
                            num = (num + Me.m_Encoders(CInt(CType((((1 + num4) << 8) + index), IntPtr))).GetPrice(num5))
                            index = ((index << 1) Or num5)
                            If (num4 <> num5) Then
                                num3 -= 1
                                Exit Do
                            End If
                            num3 -= 1
                        Loop
                    End If
                    Do While (num3 >= 0)
                        Dim num6 As UInt32 = CType(((symbol >> num3) And 1), UInt32)
                        num = (num + Me.m_Encoders(index).GetPrice(num6))
                        index = ((index << 1) Or num6)
                        num3 -= 1
                    Loop
                    Return num
                End Function
            End Structure
        End Class

        Private Class Optimal
            ' Methods
            Public Function IsShortRep() As Boolean
                Return (Me.BackPrev = 0)
            End Function

            Public Sub MakeAsChar()
                Me.BackPrev = UInt32.MaxValue
                Me.Prev1IsChar = False
            End Sub

            Public Sub MakeAsShortRep()
                Me.BackPrev = 0
                Me.Prev1IsChar = False
            End Sub


            ' Fields
            Public BackPrev As UInt32
            Public BackPrev2 As UInt32
            Public Backs0 As UInt32
            Public Backs1 As UInt32
            Public Backs2 As UInt32
            Public Backs3 As UInt32
            Public PosPrev As UInt32
            Public PosPrev2 As UInt32
            Public Prev1IsChar As Boolean
            Public Prev2 As Boolean
            Public Price As UInt32
            Public State As Base.State
        End Class
    End Class
End Namespace


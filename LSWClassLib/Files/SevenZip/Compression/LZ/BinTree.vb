Imports LSW.Files.SevenZip
Imports System
Imports System.IO
Imports LSW.Extension

Namespace Files.SevenZip.Compression.LZ
    Public Class BinTree
        Inherits InWindow
        Implements IMatchFinder, IInWindowStream
        ' Methods
        Public Overloads Sub Create(ByVal historySize As UInt32, ByVal keepAddBufferBefore As UInt32, ByVal matchMaxLen As UInt32, ByVal keepAddBufferAfter As UInt32) Implements IMatchFinder.Create
            If (historySize > &H7FFFFEFF) Then
                Throw New Exception
            End If
            Me._cutValue = (&H10 + (matchMaxLen >> 1))
            Dim keepSizeReserv As UInt32 = (((((historySize + keepAddBufferBefore) + matchMaxLen) + keepAddBufferAfter) / 2) + &H100)
            MyBase.Create((historySize + keepAddBufferBefore), (matchMaxLen + keepAddBufferAfter), keepSizeReserv)
            Me._matchMaxLen = matchMaxLen
            Dim num2 As UInt32 = (historySize + 1)
            If (Me._cyclicBufferSize <> num2) Then
                Me._son = New UInt32((Me._cyclicBufferSize = num2 * 2) - 1) {}
            End If
            Dim num3 As UInt32 = &H10000
            If Me.HASH_ARRAY Then
                num3 = (historySize - 1)
                num3 = (num3 Or (num3 >> 1))
                num3 = (num3 Or (num3 >> 2))
                num3 = (num3 Or (num3 >> 4))
                num3 = (num3 Or (num3 >> 8))
                num3 = (num3 >> 1)
                num3 = (num3 Or &HFFFF)
                If (num3 > &H1000000) Then
                    num3 = (num3 >> 1)
                End If
                Me._hashMask = num3
                num3 += 1
                num3 = (num3 + Me.kFixHashSize)
            End If
            If (num3 <> Me._hashSizeSum) Then
                Me._hash = New UInt32(Me._hashSizeSum = num3 - 1) {}
            End If
        End Sub

        Public Overloads Function GetIndexByte(ByVal index As Integer) As Byte Implements IInWindowStream.GetIndexByte
            Return MyBase.GetIndexByte(index)
        End Function

        Public Function GetMatches(ByVal distances As UInt32()) As UInt32 Implements IMatchFinder.GetMatches
            Dim num As UInt32
            Dim num6 As UInt32
            Dim num16 As UInt32
            If ((MyBase._pos + Me._matchMaxLen) <= MyBase._streamPos) Then
                num = Me._matchMaxLen
            Else
                num = (MyBase._streamPos - MyBase._pos)
                If (num < Me.kMinMatchCheck) Then
                    Me.MovePos()
                    Return 0
                End If
            End If
            Dim num2 As UInt32 = 0
            Dim num3 As UInt32 = If((MyBase._pos > Me._cyclicBufferSize), (MyBase._pos - Me._cyclicBufferSize), 0)
            Dim index As UInt32 = (MyBase._bufferOffset + MyBase._pos)
            Dim num5 As UInt32 = 1
            Dim num7 As UInt32 = 0
            Dim num8 As UInt32 = 0
            If Me.HASH_ARRAY Then
                Dim num9 As UInt32 = (CRC.Table(MyBase._bufferBase(index)) Xor MyBase._bufferBase(CInt(CType((index + 1), IntPtr))))
                num7 = (num9 And &H3FF)
                num9 = (num9 Xor CType((MyBase._bufferBase(CInt(CType((index + 2), IntPtr))) << 8), UInt32))
                num8 = (num9 And &HFFFF)
                num6 = ((num9 Xor (CRC.Table(MyBase._bufferBase(CInt(CType((index + 3), IntPtr)))) << 5)) And Me._hashMask)
            Else
                num6 = CType((MyBase._bufferBase(index) Xor (MyBase._bufferBase(CInt(CType((index + 1), IntPtr))) << 8)), UInt32)
            End If
            Dim num10 As UInt32 = Me._hash((Me.kFixHashSize + num6))
            If Me.HASH_ARRAY Then
                Dim num11 As UInt32 = Me._hash(num7)
                Dim num12 As UInt32 = Me._hash(CInt(CType((&H400 + num8), IntPtr)))
                Me._hash(num7) = MyBase._pos
                Me._hash(CInt(CType((&H400 + num8), IntPtr))) = MyBase._pos
                If ((num11 > num3) AndAlso (MyBase._bufferBase((MyBase._bufferOffset + num11)) = MyBase._bufferBase(index))) Then
                    num5 = 2
                    distances(num2) = num5
                    num2 += 1
                    distances(num2) = ((MyBase._pos - num11) - 1)
                    num2 += 1
                End If
                If ((num12 > num3) AndAlso (MyBase._bufferBase((MyBase._bufferOffset + num12)) = MyBase._bufferBase(index))) Then
                    If (num12 = num11) Then
                        num2 = (num2 - 2)
                    End If
                    num5 = 3
                    distances(num2) = num5
                    num2 += 1
                    distances(num2) = ((MyBase._pos - num12) - 1)
                    num2 += 1
                    num11 = num12
                End If
                If ((num2 <> 0) AndAlso (num11 = num10)) Then
                    num2 = (num2 - 2)
                    num5 = 1
                End If
            End If
            Me._hash((Me.kFixHashSize + num6)) = MyBase._pos
            Dim num13 As UInt32 = ((Me._cyclicBufferPos << 1) + 1)
            Dim num14 As UInt32 = (Me._cyclicBufferPos << 1)
            Dim num15 As UInt32 = num16 = Me.kNumHashDirectBytes
            If (((Me.kNumHashDirectBytes <> 0) AndAlso (num10 > num3)) AndAlso (MyBase._bufferBase(((MyBase._bufferOffset + num10) + Me.kNumHashDirectBytes)) <> MyBase._bufferBase((index + Me.kNumHashDirectBytes)))) Then
                num5 = Me.kNumHashDirectBytes
                distances(num2) = num5
                num2 += 1
                distances(num2) = ((MyBase._pos - num10) - 1)
                num2 += 1
            End If
            Dim num17 As UInt32 = Me._cutValue
            Do While True
                num17 -= 1
                If ((num10 <= num3) OrElse (num17 + 1 = 0)) Then
                    Me._son(num13) = Me._son(num14) = 0
                    Exit Do
                End If
                Dim num18 As UInt32 = (MyBase._pos - num10)
                Dim num19 As UInt32 = (If((num18 <= Me._cyclicBufferPos), (Me._cyclicBufferPos - num18), ((Me._cyclicBufferPos - num18) + Me._cyclicBufferSize)) << 1)
                Dim num20 As UInt32 = (MyBase._bufferOffset + num10)
                Dim num21 As UInt32 = System.Math.Min(num15, num16)
                If (MyBase._bufferBase((num20 + num21)) = MyBase._bufferBase((index + num21))) Then
                    Do While (num21.InlineAssignHelper(num21 + 1) <> num)
                        If (MyBase._bufferBase((num20 + num21)) <> MyBase._bufferBase((index + num21))) Then
                            Exit Do
                        End If
                    Loop
                    If (num5 < num21) Then
                        num5 = num21
                        distances(num2) = num5
                        num2 += 1
                        distances(num2) = (num18 - 1)
                        num2 += 1
                        If (num21 = num) Then
                            Me._son(num14) = Me._son(num19)
                            Me._son(num13) = Me._son(CInt(CType((num19 + 1), IntPtr)))
                            Exit Do
                        End If
                    End If
                End If
                If (MyBase._bufferBase((num20 + num21)) < MyBase._bufferBase((index + num21))) Then
                    Me._son(num14) = num10
                    num14 = (num19 + 1)
                    num10 = Me._son(num14)
                    num16 = num21
                Else
                    Me._son(num13) = num10
                    num13 = num19
                    num10 = Me._son(num13)
                    num15 = num21
                End If
            Loop
            Me.MovePos()
            Return num2
        End Function

        Public Overloads Function GetMatchLen(ByVal index As Integer, ByVal distance As UInt32, ByVal limit As UInt32) As UInt32 Implements IInWindowStream.GetMatchLen
            Return MyBase.GetMatchLen(index, distance, limit)
        End Function

        Public Overloads Function GetNumAvailableBytes() As UInt32 Implements IInWindowStream.GetNumAvailableBytes
            Return MyBase.GetNumAvailableBytes
        End Function

        Public Overloads Sub Init() Implements IInWindowStream.Init
            MyBase.Init()
            Dim i As UInt32
            For i = 0 To Me._hashSizeSum - 1
                Me._hash(i) = 0
            Next i
            Me._cyclicBufferPos = 0
            MyBase.ReduceOffsets(-1)
        End Sub

        Public Overloads Sub MovePos()
            If (Me._cyclicBufferPos.InlineAssignHelper(Me._cyclicBufferPos + 1) >= Me._cyclicBufferSize) Then
                Me._cyclicBufferPos = 0
            End If
            MyBase.MovePos()
            If (MyBase._pos = &H7FFFFFFF) Then
                Me.Normalize()
            End If
        End Sub

        Private Sub Normalize()
            Dim subValue As UInt32 = (MyBase._pos - Me._cyclicBufferSize)
            Me.NormalizeLinks(Me._son, (Me._cyclicBufferSize * 2), subValue)
            Me.NormalizeLinks(Me._hash, Me._hashSizeSum, subValue)
            MyBase.ReduceOffsets(CInt(subValue))
        End Sub

        Private Sub NormalizeLinks(ByVal items As UInt32(), ByVal numItems As UInt32, ByVal subValue As UInt32)
            Dim i As UInt32
            For i = 0 To numItems - 1
                Dim num2 As UInt32 = items(i)
                If (num2 <= subValue) Then
                    num2 = 0
                Else
                    num2 = (num2 - subValue)
                End If
                items(i) = num2
            Next i
        End Sub

        Public Overloads Sub ReleaseStream() Implements IInWindowStream.ReleaseStream
            MyBase.ReleaseStream()
        End Sub

        Public Sub SetCutValue(ByVal cutValue As UInt32)
            Me._cutValue = cutValue
        End Sub

        Public Overloads Sub SetStream(ByVal stream As Stream) Implements IInWindowStream.SetStream
            MyBase.SetStream(stream)
        End Sub

        Public Sub SetType(ByVal numHashBytes As Integer)
            Me.HASH_ARRAY = (numHashBytes > 2)
            If Me.HASH_ARRAY Then
                Me.kNumHashDirectBytes = 0
                Me.kMinMatchCheck = 4
                Me.kFixHashSize = &H10400
            Else
                Me.kNumHashDirectBytes = 2
                Me.kMinMatchCheck = 3
                Me.kFixHashSize = 0
            End If
        End Sub

        Public Sub Skip(ByVal num As UInt32) Implements IMatchFinder.Skip
            Dim num2 As UInt32
            Dim num5 As UInt32
            Dim num13 As UInt32
Label_0000:
            If ((MyBase._pos + Me._matchMaxLen) <= MyBase._streamPos) Then
                num2 = Me._matchMaxLen
            Else
                num2 = (MyBase._streamPos - MyBase._pos)
                If (num2 < Me.kMinMatchCheck) Then
                    Me.MovePos()
                    GoTo Label_02C1
                End If
            End If
            Dim num3 As UInt32 = If((MyBase._pos > Me._cyclicBufferSize), (MyBase._pos - Me._cyclicBufferSize), 0)
            Dim index As UInt32 = (MyBase._bufferOffset + MyBase._pos)
            If Me.HASH_ARRAY Then
                Dim num6 As UInt32 = (CRC.Table(MyBase._bufferBase(index)) Xor MyBase._bufferBase(CInt(CType((index + 1), IntPtr))))
                Dim num7 As UInt32 = (num6 And &H3FF)
                Me._hash(num7) = MyBase._pos
                num6 = (num6 Xor CType((MyBase._bufferBase(CInt(CType((index + 2), IntPtr))) << 8), UInt32))
                Dim num8 As UInt32 = (num6 And &HFFFF)
                Me._hash(CInt(CType((&H400 + num8), IntPtr))) = MyBase._pos
                num5 = ((num6 Xor (CRC.Table(MyBase._bufferBase(CInt(CType((index + 3), IntPtr)))) << 5)) And Me._hashMask)
            Else
                num5 = CType((MyBase._bufferBase(index) Xor (MyBase._bufferBase(CInt(CType((index + 1), IntPtr))) << 8)), UInt32)
            End If
            Dim num9 As UInt32 = Me._hash((Me.kFixHashSize + num5))
            Me._hash((Me.kFixHashSize + num5)) = MyBase._pos
            Dim num10 As UInt32 = ((Me._cyclicBufferPos << 1) + 1)
            Dim num11 As UInt32 = (Me._cyclicBufferPos << 1)
            Dim num12 As UInt32 = num13 = Me.kNumHashDirectBytes
            Dim num14 As UInt32 = Me._cutValue
            Do While True
                num14 -= 1
                If ((num9 <= num3) OrElse (num14 + 1 = 0)) Then
                    Me._son(num10) = Me._son(num11) = 0
                    Exit Do
                End If
                Dim num15 As UInt32 = (MyBase._pos - num9)
                Dim num16 As UInt32 = (If((num15 <= Me._cyclicBufferPos), (Me._cyclicBufferPos - num15), ((Me._cyclicBufferPos - num15) + Me._cyclicBufferSize)) << 1)
                Dim num17 As UInt32 = (MyBase._bufferOffset + num9)
                Dim num18 As UInt32 = System.Math.Min(num12, num13)
                If (MyBase._bufferBase((num17 + num18)) = MyBase._bufferBase((index + num18))) Then
                    Do While (num18.InlineAssignHelper(num18 + 1) <> num2)
                        If (MyBase._bufferBase((num17 + num18)) <> MyBase._bufferBase((index + num18))) Then
                            Exit Do
                        End If
                    Loop
                    If (num18 = num2) Then
                        Me._son(num11) = Me._son(num16)
                        Me._son(num10) = Me._son(CInt(CType((num16 + 1), IntPtr)))
                        Exit Do
                    End If
                End If
                If (MyBase._bufferBase((num17 + num18)) < MyBase._bufferBase((index + num18))) Then
                    Me._son(num11) = num9
                    num11 = (num16 + 1)
                    num9 = Me._son(num11)
                    num13 = num18
                Else
                    Me._son(num10) = num9
                    num10 = num16
                    num9 = Me._son(num10)
                    num12 = num18
                End If
            Loop
            Me.MovePos()
Label_02C1:
            If (num.InlineAssignHelper(num - 1) <> 0) Then
                GoTo Label_0000
            End If
        End Sub


        ' Fields
        Private _cutValue As UInt32 = &HFF
        Private _cyclicBufferPos As UInt32
        Private _cyclicBufferSize As UInt32
        Private _hash As UInt32()
        Private _hashMask As UInt32
        Private _hashSizeSum As UInt32
        Private _matchMaxLen As UInt32
        Private _son As UInt32()
        Private HASH_ARRAY As Boolean = True
        Private Const kBT2HashSize As UInt32 = &H10000
        Private Const kEmptyHashValue As UInt32 = 0
        Private kFixHashSize As UInt32 = &H10400
        Private Const kHash2Size As UInt32 = &H400
        Private Const kHash3Offset As UInt32 = &H400
        Private Const kHash3Size As UInt32 = &H10000
        Private Const kMaxValForNormalize As UInt32 = &H7FFFFFFF
        Private kMinMatchCheck As UInt32 = 4
        Private kNumHashDirectBytes As UInt32
        Private Const kStartMaxLen As UInt32 = 1
    End Class
End Namespace


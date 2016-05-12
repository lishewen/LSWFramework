Public Class BigInteger
    Private Const maxLength As Integer = 140
    Public Shared ReadOnly primesBelow2000 As Integer() = {2, 3, 5, 7, 11, 13, _
    17, 19, 23, 29, 31, 37, _
    41, 43, 47, 53, 59, 61, _
    67, 71, 73, 79, 83, 89, _
    97, 101, 103, 107, 109, 113, _
    127, 131, 137, 139, 149, 151, _
    157, 163, 167, 173, 179, 181, _
    191, 193, 197, 199, 211, 223, _
    227, 229, 233, 239, 241, 251, _
    257, 263, 269, 271, 277, 281, _
    283, 293, 307, 311, 313, 317, _
    331, 337, 347, 349, 353, 359, _
    367, 373, 379, 383, 389, 397, _
    401, 409, 419, 421, 431, 433, _
    439, 443, 449, 457, 461, 463, _
    467, 479, 487, 491, 499, 503, _
    509, 521, 523, 541, 547, 557, _
    563, 569, 571, 577, 587, 593, _
    599, 601, 607, 613, 617, 619, _
    631, 641, 643, 647, 653, 659, _
    661, 673, 677, 683, 691, 701, _
    709, 719, 727, 733, 739, 743, _
    751, 757, 761, 769, 773, 787, _
    797, 809, 811, 821, 823, 827, _
    829, 839, 853, 857, 859, 863, _
    877, 881, 883, 887, 907, 911, _
    919, 929, 937, 941, 947, 953, _
    967, 971, 977, 983, 991, 997, _
    1009, 1013, 1019, 1021, 1031, 1033, _
    1039, 1049, 1051, 1061, 1063, 1069, _
    1087, 1091, 1093, 1097, 1103, 1109, _
    1117, 1123, 1129, 1151, 1153, 1163, _
    1171, 1181, 1187, 1193, 1201, 1213, _
    1217, 1223, 1229, 1231, 1237, 1249, _
    1259, 1277, 1279, 1283, 1289, 1291, _
    1297, 1301, 1303, 1307, 1319, 1321, _
    1327, 1361, 1367, 1373, 1381, 1399, _
    1409, 1423, 1427, 1429, 1433, 1439, _
    1447, 1451, 1453, 1459, 1471, 1481, _
    1483, 1487, 1489, 1493, 1499, 1511, _
    1523, 1531, 1543, 1549, 1553, 1559, _
    1567, 1571, 1579, 1583, 1597, 1601, _
    1607, 1609, 1613, 1619, 1621, 1627, _
    1637, 1657, 1663, 1667, 1669, 1693, _
    1697, 1699, 1709, 1721, 1723, 1733, _
    1741, 1747, 1753, 1759, 1777, 1783, _
    1787, 1789, 1801, 1811, 1823, 1831, _
    1847, 1861, 1867, 1871, 1873, 1877, _
    1879, 1889, 1901, 1907, 1913, 1931, _
    1933, 1949, 1951, 1973, 1979, 1987, _
    1993, 1997, 1999}
    Private data As UInteger() = Nothing
    Public dataLength As Integer

    Public Sub New()
        data = New UInteger(maxLength) {}
        dataLength = 1
    End Sub

    Public Sub New(ByVal inData As Byte())
        dataLength = inData.Length >> 2

        Dim leftOver As Integer = inData.Length And 3
        If leftOver <> 0 Then
            dataLength += 1
        End If

        If dataLength > maxLength Then
            Throw (New ArithmeticException("Byte overflow in constructor."))
        End If

        data = New UInteger(maxLength - 1) {}

        Dim i As Integer = inData.Length - 1, j As Integer = 0
        While i >= 3
            data(j) = CInt(((inData(i - 3) << 24) + (inData(i - 2) << 16) + (inData(i - 1) << 8) + inData(i)))
            i -= 4
            j += 1
        End While

        If leftOver = 1 Then
            data(dataLength - 1) = CInt(inData(0))
        ElseIf leftOver = 2 Then
            data(dataLength - 1) = CInt(((inData(0) << 8) + inData(1)))
        ElseIf leftOver = 3 Then
            data(dataLength - 1) = CInt(((inData(0) << 16) + (inData(1) << 8) + inData(2)))
        End If

        While dataLength > 1 AndAlso data(dataLength - 1) = 0
            dataLength -= 1
        End While
    End Sub

    Public Sub New(ByVal value As Long)
        data = New UInteger(maxLength - 1) {}
        Dim tempVal As Long = value

        dataLength = 0
        While value <> 0 AndAlso dataLength < maxLength
            data(dataLength) = CInt((value And 4294967295))
            value >>= 32
            dataLength += 1
        End While

        If tempVal > 0 Then
            If value <> 0 OrElse (data(maxLength - 1) And 2147483648) <> 0 Then
                Throw (New ArithmeticException("Positive overflow in constructor."))
            End If
        ElseIf tempVal < 0 Then
            If value <> -1 OrElse (data(dataLength - 1) And 2147483648) = 0 Then
                Throw (New ArithmeticException("Negative underflow in constructor."))
            End If
        End If

        If dataLength = 0 Then
            dataLength = 1
        End If
    End Sub

    Public Sub New(ByVal bi As BigInteger)
        data = New UInteger(maxLength - 1) {}

        dataLength = bi.dataLength
        For i As Integer = 0 To dataLength - 1
            data(i) = bi.data(i)
        Next
    End Sub

    Public Sub New(ByVal inData As UInteger())
        dataLength = inData.Length

        If dataLength > maxLength Then
            Throw (New ArithmeticException("Byte overflow in constructor."))
        End If

        data = New UInteger(maxLength - 1) {}

        Dim i As Integer = dataLength - 1, j As Integer = 0
        While i >= 0
            data(j) = inData(i)
            i -= 1
            j += 1
        End While

        While dataLength > 1 AndAlso data(dataLength - 1) = 0
            dataLength -= 1
        End While
    End Sub

    Public Shared Widening Operator CType(ByVal value As Integer) As BigInteger
        Return (New BigInteger(CLng(value)))
    End Operator

    Public Shared Widening Operator CType(ByVal value As Long) As BigInteger
        Return (New BigInteger(value))
    End Operator

    Public Shared Operator -(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As BigInteger
        Dim result As New BigInteger()

        result.dataLength = IIf((bi1.dataLength > bi2.dataLength), bi1.dataLength, bi2.dataLength)

        Dim carryIn As Long = 0
        For i As Integer = 0 To result.dataLength - 1
            Dim diff As Long

            diff = CLng(bi1.data(i)) - CLng(bi2.data(i)) - carryIn
            result.data(i) = CInt((diff And 4294967295))

            If diff < 0 Then
                carryIn = 1
            Else
                carryIn = 0
            End If
        Next

        If carryIn <> 0 Then
            For i As Integer = result.dataLength To maxLength - 1
                result.data(i) = 4294967295
            Next
            result.dataLength = maxLength
        End If

        While result.dataLength > 1 AndAlso result.data(result.dataLength - 1) = 0
            result.dataLength -= 1
        End While

        Dim lastPos As Integer = maxLength - 1
        If (bi1.data(lastPos) And 2147483648) <> (bi2.data(lastPos) And 2147483648) AndAlso (result.data(lastPos) And 2147483648) <> (bi1.data(lastPos) And 2147483648) Then
            Throw (New ArithmeticException())
        End If

        Return result
    End Operator

    Public Shared Operator -(ByVal bi1 As BigInteger) As BigInteger
        If bi1.dataLength = 1 AndAlso bi1.data(0) = 0 Then
            Return (New BigInteger())
        End If

        Dim result As New BigInteger(bi1)
        For i As Integer = 0 To maxLength - 1
            result.data(i) = CInt((Not (bi1.data(i))))
        Next

        Dim val As Long, carry As Long = 1
        Dim index As Integer = 0

        While carry <> 0 AndAlso index < maxLength
            val = CLng((result.data(index)))
            val += 1

            result.data(index) = CInt((val And 4294967295))
            carry = val >> 32

            index += 1
        End While

        If (bi1.data(maxLength - 1) And 2147483648) = (result.data(maxLength - 1) And 2147483648) Then
            Throw (New ArithmeticException("Overflow in negation." & Chr(10) & ""))
        End If

        result.dataLength = maxLength

        While result.dataLength > 1 AndAlso result.data(result.dataLength - 1) = 0
            result.dataLength -= 1
        End While
        Return result
    End Operator

    Public Shared Operator >(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As Boolean
        Dim pos As Integer = maxLength - 1

        If (bi1.data(pos) And 2147483648) <> 0 AndAlso (bi2.data(pos) And 2147483648) = 0 Then
            Return False
        ElseIf (bi1.data(pos) And 2147483648) = 0 AndAlso (bi2.data(pos) And 2147483648) <> 0 Then
            Return True
        End If

        Dim len As Integer = IIf((bi1.dataLength > bi2.dataLength), bi1.dataLength, bi2.dataLength)
        pos = len - 1
        While pos >= 0 AndAlso bi1.data(pos) = bi2.data(pos)


            pos -= 1
        End While

        If pos >= 0 Then
            If bi1.data(pos) > bi2.data(pos) Then
                Return True
            End If
            Return False
        End If
        Return False
    End Operator

    Public Shared Operator <(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As Boolean
        Dim pos As Integer = maxLength - 1

        If (bi1.data(pos) And 2147483648) <> 0 AndAlso (bi2.data(pos) And 2147483648) = 0 Then
            Return True
        ElseIf (bi1.data(pos) And 2147483648) = 0 AndAlso (bi2.data(pos) And 2147483648) <> 0 Then
            Return False
        End If

        Dim len As Integer = IIf((bi1.dataLength > bi2.dataLength), bi1.dataLength, bi2.dataLength)
        pos = len - 1
        While pos >= 0 AndAlso bi1.data(pos) = bi2.data(pos)
            pos -= 1
        End While

        If pos >= 0 Then
            If bi1.data(pos) < bi2.data(pos) Then
                Return True
            End If
            Return False
        End If
        Return False
    End Operator

    Private Shared Sub singleByteDivide(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger, ByVal outQuotient As BigInteger, ByVal outRemainder As BigInteger)
        Dim result As UInteger() = New UInteger(maxLength - 1) {}
        Dim resultPos As Integer = 0
        For ii As Integer = 0 To maxLength - 1
            outRemainder.data(ii) = bi1.data(ii)
        Next

        outRemainder.dataLength = bi1.dataLength

        While outRemainder.dataLength > 1 AndAlso outRemainder.data(outRemainder.dataLength - 1) = 0
            outRemainder.dataLength -= 1
        End While

        Dim divisor As ULong = CLng(bi2.data(0))
        Dim pos As Integer = outRemainder.dataLength - 1
        Dim dividend As ULong = CLng(outRemainder.data(pos))

        If dividend >= divisor Then
            Dim quotient As ULong = dividend / divisor
            result(resultPos) = CInt(quotient)
            resultPos += 1
            outRemainder.data(pos) = CInt((dividend Mod divisor))
        End If
        pos -= 1

        While pos >= 0
            dividend = (CLng(outRemainder.data(pos + 1)) << 32) + CLng(outRemainder.data(pos))
            Dim quotient As ULong = dividend / divisor
            result(resultPos) = CInt(quotient)
            resultPos += 1
            outRemainder.data(pos + 1) = 0
            outRemainder.data(System.Math.Max(System.Threading.Interlocked.Decrement(pos), pos + 1)) = CInt((dividend Mod divisor))
        End While

        outQuotient.dataLength = resultPos
        Dim j As Integer = 0
        Dim i As Integer = outQuotient.dataLength - 1
        While i >= 0
            outQuotient.data(j) = result(i)
            i -= 1
            j += 1
        End While
        While j < maxLength
            outQuotient.data(j) = 0
            j += 1
        End While

        While outQuotient.dataLength > 1 AndAlso outQuotient.data(outQuotient.dataLength - 1) = 0
            outQuotient.dataLength -= 1
        End While

        If outQuotient.dataLength = 0 Then
            outQuotient.dataLength = 1
        End If

        While outRemainder.dataLength > 1 AndAlso outRemainder.data(outRemainder.dataLength - 1) = 0
            outRemainder.dataLength -= 1
        End While
    End Sub

    Private Shared Function shiftLeft(ByVal buffer As UInteger(), ByVal shiftVal As Integer) As Integer
        Dim shiftAmount As Integer = 32
        Dim bufLen As Integer = buffer.Length

        While bufLen > 1 AndAlso buffer(bufLen - 1) = 0
            bufLen -= 1
        End While

        Dim count As Integer = shiftVal
        While count > 0
            If count < shiftAmount Then
                shiftAmount = count
            End If

            Dim carry As ULong = 0
            For i As Integer = 0 To bufLen - 1
                Dim val As ULong = CLng(buffer(i)) << shiftAmount
                val = val Or carry

                buffer(i) = CInt((val And 4294967295))
                carry = val >> 32
            Next

            If carry <> 0 Then
                If bufLen + 1 <= buffer.Length Then
                    buffer(bufLen) = CInt(carry)
                    bufLen += 1
                End If
            End If
            count -= shiftAmount
        End While
        Return bufLen
    End Function

    Public Shared Operator <<(ByVal bi1 As BigInteger, ByVal shiftVal As Integer) As BigInteger
        Dim result As New BigInteger(bi1)
        result.dataLength = shiftLeft(result.data, shiftVal)
        Return result
    End Operator

    Public Shared Operator *(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As BigInteger
        Dim lastPos As Integer = maxLength - 1
        Dim bi1Neg As Boolean = False, bi2Neg As Boolean = False

        Try
            If (bi1.data(lastPos) And 2147483648) <> 0 Then
                bi1Neg = True
                bi1 = -bi1
            End If
            If (bi2.data(lastPos) And 2147483648) <> 0 Then
                bi2Neg = True
                bi2 = -bi2
            End If
        Catch generatedExceptionName As Exception
        End Try

        Dim result As New BigInteger()

        Try
            For i As Integer = 0 To bi1.dataLength - 1
                If bi1.data(i) = 0 Then
                    Continue For
                End If

                Dim mcarry As ULong = 0
                Dim j As Integer = 0, k As Integer = i
                While j < bi2.dataLength
                    Dim val As ULong = (CLng(bi1.data(i)) * CLng(bi2.data(j))) + CLng(result.data(k)) + mcarry

                    result.data(k) = CInt((val And 4294967295))
                    mcarry = (val >> 32)
                    j += 1
                    k += 1
                End While

                If mcarry <> 0 Then
                    result.data(i + bi2.dataLength) = CInt(mcarry)
                End If
            Next
        Catch generatedExceptionName As Exception
            Throw (New ArithmeticException("Multiplication overflow."))
        End Try

        result.dataLength = bi1.dataLength + bi2.dataLength
        If result.dataLength > maxLength Then
            result.dataLength = maxLength
        End If

        While result.dataLength > 1 AndAlso result.data(result.dataLength - 1) = 0
            result.dataLength -= 1
        End While

        If (result.data(lastPos) And 2147483648) <> 0 Then
            If bi1Neg <> bi2Neg AndAlso result.data(lastPos) = 2147483648 Then
                If result.dataLength = 1 Then
                    Return result
                Else
                    Dim isMaxNeg As Boolean = True
                    Dim i As Integer = 0
                    While i < result.dataLength - 1 AndAlso isMaxNeg
                        If result.data(i) <> 0 Then
                            isMaxNeg = False
                        End If
                        i += 1
                    End While

                    If isMaxNeg Then
                        Return result
                    End If
                End If
            End If

            Throw (New ArithmeticException("Multiplication overflow."))
        End If

        If bi1Neg <> bi2Neg Then
            Return -result
        End If

        Return result
    End Operator

    Private Shared Function shiftRight(ByVal buffer As UInteger(), ByVal shiftVal As Integer) As Integer
        Dim shiftAmount As Integer = 32
        Dim invShift As Integer = 0
        Dim bufLen As Integer = buffer.Length

        While bufLen > 1 AndAlso buffer(bufLen - 1) = 0
            bufLen -= 1
        End While

        Dim count As Integer = shiftVal
        While count > 0
            If count < shiftAmount Then
                shiftAmount = count
                invShift = 32 - shiftAmount
            End If

            Dim carry As ULong = 0
            For i As Integer = bufLen - 1 To 0 Step -1
                Dim val As ULong = CLng(buffer(i)) >> shiftAmount
                val = val Or carry

                carry = CLng(buffer(i)) << invShift
                buffer(i) = CInt((val))
            Next

            count -= shiftAmount
        End While

        While bufLen > 1 AndAlso buffer(bufLen - 1) = 0
            bufLen -= 1
        End While

        Return bufLen
    End Function

    Private Shared Sub multiByteDivide(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger, ByVal outQuotient As BigInteger, ByVal outRemainder As BigInteger)
        Dim result As UInteger() = New UInteger(maxLength - 1) {}

        Dim remainderLen As Integer = bi1.dataLength + 1
        Dim remainder As UInteger() = New UInteger(remainderLen - 1) {}

        Dim mask As UInteger = 2147483648
        Dim val As UInteger = bi2.data(bi2.dataLength - 1)
        Dim shift As Integer = 0, resultPos As Integer = 0

        While mask <> 0 AndAlso (val And mask) = 0
            shift += 1
            mask >>= 1
        End While
        For i As Integer = 0 To bi1.dataLength - 1
            remainder(i) = bi1.data(i)
        Next

        shiftLeft(remainder, shift)
        bi2 = bi2 << shift

        Dim j As Integer = remainderLen - bi2.dataLength
        Dim pos As Integer = remainderLen - 1

        Dim firstDivisorByte As ULong = bi2.data(bi2.dataLength - 1)
        Dim secondDivisorByte As ULong = bi2.data(bi2.dataLength - 2)

        Dim divisorLen As Integer = bi2.dataLength + 1
        Dim dividendPart As UInteger() = New UInteger(divisorLen - 1) {}

        While j > 0
            Dim dividend As ULong = (CLng(remainder(pos)) << 32) + CLng(remainder(pos - 1))

            Dim q_hat As ULong = dividend / firstDivisorByte
            Dim r_hat As ULong = dividend Mod firstDivisorByte

            Dim done As Boolean = False
            While Not done
                done = True

                If q_hat = 4294967296 OrElse (q_hat * secondDivisorByte) > ((r_hat << 32) + remainder(pos - 2)) Then
                    q_hat -= 1
                    r_hat += firstDivisorByte

                    If r_hat < 4294967296 Then
                        done = False
                    End If
                End If
            End While
            For h As Integer = 0 To divisorLen - 1
                dividendPart(h) = remainder(pos - h)
            Next


            Dim kk As New BigInteger(dividendPart)
            Dim ss As BigInteger = bi2 * CLng(q_hat)

            While ss > kk
                q_hat -= 1 
                ss -= bi2
            End While
            Dim yy As BigInteger = kk - ss
            For h As Integer = 0 To divisorLen - 1
                remainder(pos - h) = yy.data(bi2.dataLength - h)
            Next

            result(resultPos) = CInt(q_hat)
            resultPos += 1
            pos -= 1
            j -= 1
        End While

        outQuotient.dataLength = resultPos
        Dim y As Integer = 0
        Dim x As Integer = outQuotient.dataLength - 1
        While x >= 0
            outQuotient.data(y) = result(x)
            x -= 1
            y += 1
        End While
        While y < maxLength
            outQuotient.data(y) = 0
            y += 1
        End While

        While outQuotient.dataLength > 1 AndAlso outQuotient.data(outQuotient.dataLength - 1) = 0
            outQuotient.dataLength -= 1
        End While

        If outQuotient.dataLength = 0 Then
            outQuotient.dataLength = 1
        End If

        outRemainder.dataLength = shiftRight(remainder, shift)
        For y = 0 To outRemainder.dataLength - 1
            outRemainder.data(y) = remainder(y)
        Next

        While y < maxLength
            outRemainder.data(y) = 0
            y += 1
        End While
    End Sub

    Public Shared Operator Mod(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As BigInteger
        Dim quotient As New BigInteger()
        Dim remainder As New BigInteger(bi1)

        Dim lastPos As Integer = maxLength - 1
        Dim dividendNeg As Boolean = False

        If (bi1.data(lastPos) And 2147483648) <> 0 Then
            bi1 = -bi1
            dividendNeg = True
        End If
        If (bi2.data(lastPos) And 2147483648) <> 0 Then
            bi2 = -bi2
        End If

        If bi1 < bi2 Then
            Return remainder
        Else

            If bi2.dataLength = 1 Then
                singleByteDivide(bi1, bi2, quotient, remainder)
            Else
                multiByteDivide(bi1, bi2, quotient, remainder)
            End If

            If dividendNeg Then
                Return -remainder
            End If

            Return remainder
        End If
    End Operator

    Public Shared Operator /(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As BigInteger
        Dim quotient As New BigInteger()
        Dim remainder As New BigInteger()

        Dim lastPos As Integer = maxLength - 1
        Dim divisorNeg As Boolean = False, dividendNeg As Boolean = False

        If (bi1.data(lastPos) And 2147483648) <> 0 Then
            bi1 = -bi1
            dividendNeg = True
        End If
        If (bi2.data(lastPos) And 2147483648) <> 0 Then
            bi2 = -bi2
            divisorNeg = True
        End If

        If bi1 < bi2 Then
            Return quotient
        Else

            If bi2.dataLength = 1 Then
                singleByteDivide(bi1, bi2, quotient, remainder)
            Else
                multiByteDivide(bi1, bi2, quotient, remainder)
            End If

            If dividendNeg <> divisorNeg Then
                Return -quotient
            End If

            Return quotient
        End If
    End Operator

    Public Function bitCount() As Integer
        While dataLength > 1 AndAlso data(dataLength - 1) = 0
            dataLength -= 1
        End While

        Dim value As UInteger = data(dataLength - 1)
        Dim mask As UInteger = 2147483648
        Dim bits As Integer = 32

        While bits > 0 AndAlso (value And mask) = 0
            bits -= 1
            mask >>= 1
        End While
        bits += ((dataLength - 1) << 5)

        Return bits
    End Function

    Public Shared Operator +(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As BigInteger
        Dim result As New BigInteger()

        result.dataLength = IIf((bi1.dataLength > bi2.dataLength), bi1.dataLength, bi2.dataLength)

        Dim carry As Long = 0
        For i As Integer = 0 To result.dataLength - 1
            Dim sum As Long = CLng(bi1.data(i)) + CLng(bi2.data(i)) + carry
            carry = sum >> 32
            result.data(i) = CInt((sum And 4294967295))
        Next

        If carry <> 0 AndAlso result.dataLength < maxLength Then
            result.data(result.dataLength) = CInt((carry))
            result.dataLength += 1
        End If

        While result.dataLength > 1 AndAlso result.data(result.dataLength - 1) = 0
            result.dataLength -= 1
        End While

        Dim lastPos As Integer = maxLength - 1
        If (bi1.data(lastPos) And 2147483648) = (bi2.data(lastPos) And 2147483648) AndAlso (result.data(lastPos) And 2147483648) <> (bi1.data(lastPos) And 2147483648) Then
            Throw (New ArithmeticException())
        End If

        Return result
    End Operator

    Public Shared Operator =(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As Boolean
        Return bi1.Equals(bi2)
    End Operator

    Public Shared Operator <>(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As Boolean
        Return Not (bi1.Equals(bi2))
    End Operator

    Public Overloads Overrides Function Equals(ByVal o As Object) As Boolean
        Dim bi As BigInteger = DirectCast(o, BigInteger)

        If Me.dataLength <> bi.dataLength Then
            Return False
        End If
        For i As Integer = 0 To Me.dataLength - 1

            If Me.data(i) <> bi.data(i) Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ToString().GetHashCode()
    End Function

    Public Shared Operator <=(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As Boolean
        Return (bi1 = bi2 OrElse bi1 < bi2)
    End Operator

    Public Shared Operator >=(ByVal bi1 As BigInteger, ByVal bi2 As BigInteger) As Boolean
        Return (bi1 = bi2 OrElse bi1 > bi2)
    End Operator

    Private Function BarrettReduction(ByVal x As BigInteger, ByVal n As BigInteger, ByVal constant As BigInteger) As BigInteger
        Dim k As Integer = n.dataLength, kPlusOne As Integer = k + 1, kMinusOne As Integer = k - 1

        Dim q1 As New BigInteger()

        Dim i As Integer = kMinusOne, j As Integer = 0
        While i < x.dataLength
            q1.data(j) = x.data(i)
            i += 1
            j += 1
        End While
        q1.dataLength = x.dataLength - kMinusOne
        If q1.dataLength <= 0 Then
            q1.dataLength = 1
        End If

        Dim q2 As BigInteger = q1 * constant
        Dim q3 As New BigInteger()

        i = kPlusOne
        j = 0
        While i < q2.dataLength
            q3.data(j) = q2.data(i)
            i += 1
            j += 1
        End While
        q3.dataLength = q2.dataLength - kPlusOne
        If q3.dataLength <= 0 Then
            q3.dataLength = 1
        End If

        Dim r1 As New BigInteger()
        Dim lengthToCopy As Integer = IIf((x.dataLength > kPlusOne), kPlusOne, x.dataLength)
        For i = 0 To lengthToCopy - 1
            r1.data(i) = x.data(i)
        Next
        r1.dataLength = lengthToCopy

        Dim r2 As New BigInteger()
        For i = 0 To q3.dataLength - 1
            If q3.data(i) = 0 Then
                Continue For
            End If

            Dim mcarry As ULong = 0
            Dim t As Integer = i
            j = 0
            While j < n.dataLength AndAlso t < kPlusOne
                Dim val As ULong = (CLng(q3.data(i)) * CLng(n.data(j))) + CLng(r2.data(t)) + mcarry

                r2.data(t) = CInt((val And 4294967295))
                mcarry = (val >> 32)
                j += 1
                t += 1
            End While

            If t < kPlusOne Then
                r2.data(t) = CInt(mcarry)
            End If
        Next
        r2.dataLength = kPlusOne
        While r2.dataLength > 1 AndAlso r2.data(r2.dataLength - 1) = 0
            r2.dataLength -= 1
        End While

        r1 -= r2
        If (r1.data(maxLength - 1) And 2147483648) <> 0 Then
            Dim val As New BigInteger()
            val.data(kPlusOne) = 1
            val.dataLength = kPlusOne + 1
            r1 += val
        End If

        While r1 >= n
            r1 -= n
        End While

        Return r1
    End Function

    Public Function modPow(ByVal exp As BigInteger, ByVal n As BigInteger) As BigInteger
        If (exp.data(maxLength - 1) And 2147483648) <> 0 Then
            Throw (New ArithmeticException("Positive exponents only."))
        End If

        Dim resultNum As BigInteger = 1
        Dim tempNum As BigInteger
        Dim thisNegative As Boolean = False

        If (Me.data(maxLength - 1) And 2147483648) <> 0 Then
            tempNum = -Me Mod n
            thisNegative = True
        Else
            tempNum = Me Mod n
        End If
        If (n.data(maxLength - 1) And 2147483648) <> 0 Then
            n = -n
        End If

        Dim constant As New BigInteger()

        Dim i As Integer = n.dataLength << 1
        constant.data(i) = 1
        constant.dataLength = i + 1

        constant = constant / n
        Dim totalBits As Integer = exp.bitCount()
        Dim count As Integer = 0
        For pos As Integer = 0 To exp.dataLength - 1
            Dim mask As UInteger = 1
            For index As Integer = 0 To 31
                If (exp.data(pos) And mask) <> 0 Then
                    resultNum = BarrettReduction(resultNum * tempNum, n, constant)
                End If

                mask <<= 1

                tempNum = BarrettReduction(tempNum * tempNum, n, constant)

                If tempNum.dataLength = 1 AndAlso tempNum.data(0) = 1 Then
                    If thisNegative AndAlso (exp.data(0) And 1) <> 0 Then
                        Return -resultNum
                    End If
                    Return resultNum
                End If
                count += 1
                If count = totalBits Then
                    Exit For
                End If
            Next
        Next

        If thisNegative AndAlso (exp.data(0) And 1) <> 0 Then
            Return -resultNum
        End If

        Return resultNum
    End Function

    Public Function modInverse(ByVal modulus As BigInteger) As BigInteger
        Dim p As BigInteger() = {0, 1}
        Dim q As BigInteger() = New BigInteger(1) {}
        Dim r As BigInteger() = {0, 0}
        Dim [step] As Integer = 0

        Dim a As BigInteger = modulus
        Dim b As BigInteger = Me

        While b.dataLength > 1 OrElse (b.dataLength = 1 AndAlso b.data(0) <> 0)
            Dim quotient As New BigInteger()
            Dim remainder As New BigInteger()

            If [step] > 1 Then
                Dim pval As BigInteger = (p(0) - (p(1) * q(0))) Mod modulus
                p(0) = p(1)
                p(1) = pval
            End If

            If b.dataLength = 1 Then
                singleByteDivide(a, b, quotient, remainder)
            Else
                multiByteDivide(a, b, quotient, remainder)
            End If

            q(0) = q(1)
            r(0) = r(1)
            q(1) = quotient
            r(1) = remainder

            a = b
            b = remainder

            [step] += 1
        End While

        If r(0).dataLength > 1 OrElse (r(0).dataLength = 1 AndAlso r(0).data(0) <> 1) Then
            Throw (New ArithmeticException("No inverse!"))
        End If

        Dim result As BigInteger = ((p(0) - (p(1) * q(0))) Mod modulus)

        If (result.data(maxLength - 1) And 2147483648) <> 0 Then
            result += modulus
        End If
        Return result
    End Function

    Public Function getBytes() As Byte()
        Dim numBits As Integer = bitCount()
        Dim result As Byte() = Nothing
        If numBits = 0 Then
            result = New Byte(0) {}
            result(0) = 0
        Else
            Dim numBytes As Integer = numBits >> 3
            If (numBits And 7) <> 0 Then
                numBytes += 1
            End If
            result = New Byte(numBytes - 1) {} 
            Dim numBytesInWord As Integer = numBytes And 3
            If numBytesInWord = 0 Then
                numBytesInWord = 4
            End If
            Dim pos As Integer = 0
            For i As Integer = dataLength - 1 To 0 Step -1
                Dim val As UInteger = data(i)
                For j As Integer = numBytesInWord - 1 To 0 Step -1
                    result(pos + j) = CByte((val And 255))
                    val >>= 8
                Next
                pos += numBytesInWord
                numBytesInWord = 4
            Next
        End If
        Return result
    End Function

    Public Sub genRandomBits(ByVal bits As Integer, ByVal rand As Random)
        Dim dwords As Integer = bits >> 5
        Dim remBits As Integer = bits And 31

        If remBits <> 0 Then
            dwords += 1
        End If

        If dwords > maxLength Then
            Throw (New ArithmeticException("Number of required bits > maxLength."))
        End If
        For i As Integer = 0 To dwords - 1
            data(i) = CInt((rand.NextDouble() * 4294967296))
        Next
        For i As Integer = dwords To maxLength - 1
            data(i) = 0
        Next

        If remBits <> 0 Then
            Dim mask As UInteger = CInt((1 << (remBits - 1)))
            data(dwords - 1) = data(dwords - 1) Or mask

            mask = CInt((4294967295 >> (32 - remBits)))
            data(dwords - 1) = data(dwords - 1) And mask
        Else
            data(dwords - 1) = data(dwords - 1) Or 2147483648
        End If

        dataLength = dwords

        If dataLength = 0 Then
            dataLength = 1
        End If
    End Sub

    Public Function IntValue() As Integer
        Return CInt(data(0))
    End Function

    Public Shared Operator >>(ByVal bi1 As BigInteger, ByVal shiftVal As Integer) As BigInteger
        Dim result As New BigInteger(bi1)
        result.dataLength = shiftRight(result.data, shiftVal)

        If (bi1.data(maxLength - 1) And 2147483648) <> 0 Then
            For i As Integer = maxLength - 1 To result.dataLength Step -1
                result.data(i) = 4294967295
            Next

            Dim mask As UInteger = 2147483648
            For i As Integer = 0 To 31
                If (result.data(result.dataLength - 1) And mask) <> 0 Then
                    Exit For
                End If

                result.data(result.dataLength - 1) = result.data(result.dataLength - 1) Or mask
                mask >>= 1
            Next
            result.dataLength = maxLength
        End If

        Return result
    End Operator

    Public Function gcd(ByVal bi As BigInteger) As BigInteger
        Dim x As BigInteger
        Dim y As BigInteger

        If (data(maxLength - 1) And 2147483648) <> 0 Then
            x = -Me
        Else
            x = Me
        End If

        If (bi.data(maxLength - 1) And 2147483648) <> 0 Then
            y = -bi
        Else
            y = bi 
        End If

        Dim g As BigInteger = y

        While x.dataLength > 1 OrElse (x.dataLength = 1 AndAlso x.data(0) <> 0)
            g = x
            x = y Mod x
            y = g
        End While

        Return g
    End Function

    Public Function RabinMillerTest(ByVal confidence As Integer) As Boolean
        Dim thisVal As BigInteger
        If (Me.data(maxLength - 1) And 2147483648) <> 0 Then
            thisVal = -Me
        Else
            thisVal = Me
        End If

        If thisVal.dataLength = 1 Then
            If thisVal.data(0) = 0 OrElse thisVal.data(0) = 1 Then
                Return False
            ElseIf thisVal.data(0) = 2 OrElse thisVal.data(0) = 3 Then
                Return True
            End If
        End If

        If (thisVal.data(0) And 1) = 0 Then
            Return False
        End If

        Dim p_sub1 As BigInteger = thisVal - (New BigInteger(1))
        Dim s As Integer = 0
        For index As Integer = 0 To p_sub1.dataLength - 1

            Dim mask As UInteger = 1
            For i As Integer = 0 To 31

                If (p_sub1.data(index) And mask) <> 0 Then
                    index = p_sub1.dataLength 
                    Exit For
                End If
                mask <<= 1
                s += 1
            Next
        Next

        Dim t As BigInteger = p_sub1 >> s

        Dim bits As Integer = thisVal.bitCount()
        Dim a As New BigInteger()
        Dim rand As New Random()
        For round As Integer = 0 To confidence - 1

            Dim done As Boolean = False

            While Not done
                Dim testBits As Integer = 0

                While testBits < 2
                    testBits = CInt((rand.NextDouble() * bits))
                End While

                a.genRandomBits(testBits, rand)

                Dim byteLen As Integer = a.dataLength

                If byteLen > 1 OrElse (byteLen = 1 AndAlso a.data(0) <> 1) Then
                    done = True
                End If
            End While

            Dim gcdTest As BigInteger = a.gcd(thisVal)
            If gcdTest.dataLength = 1 AndAlso gcdTest.data(0) <> 1 Then
                Return False
            End If

            Dim b As BigInteger = a.modPow(t, thisVal)

            Dim result As Boolean = False

            If b.dataLength = 1 AndAlso b.data(0) = 1 Then
                result = True
            End If

            Dim j As Integer = 0
            While result = False AndAlso j < s
                If b = p_sub1 Then
                    result = True
                    Exit While
                End If

                b = (b * b) Mod thisVal
                j += 1
            End While

            If result = False Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Function isProbablePrime(ByVal confidence As Integer) As Boolean
        Dim thisVal As BigInteger
        If (Me.data(maxLength - 1) And 2147483648) <> 0 Then
            thisVal = -Me
        Else
            thisVal = Me
        End If
        For p As Integer = 0 To primesBelow2000.Length - 1
            Dim divisor As BigInteger = primesBelow2000(p)

            If divisor >= thisVal Then
                Exit For
            End If

            Dim resultNum As BigInteger = thisVal Mod divisor
            If resultNum.IntValue() = 0 Then
                Return False
            End If
        Next

        If thisVal.RabinMillerTest(confidence) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function genPseudoPrime(ByVal bits As Integer, ByVal confidence As Integer, ByVal rand As Random) As BigInteger
        Dim result As New BigInteger()
        Dim done As Boolean = False

        While Not done
            result.genRandomBits(bits, rand)
            result.data(0) = result.data(0) Or 1
            done = result.isProbablePrime(confidence)
        End While
        Return result
    End Function

End Class
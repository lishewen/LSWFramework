Namespace Text
    Public Module Distance
        Private mCharA() As Char
        Private mCharB() As Char
        Private mCharALen As Integer
        Private mCharBLen As Integer

        Public Function CacuDistance(ByVal StrA As String, ByVal StrB As String) As Integer
            mCharA = StrA.ToCharArray
            mCharB = StrB.ToCharArray
            mCharALen = mCharA.Length
            mCharBLen = mCharB.Length

            Dim i As Integer

            If mCharALen = 0 Then Return mCharBLen
            If mCharBLen = 0 Then Return mCharALen

            Dim j As Integer = Min(mCharALen, mCharBLen) - 1
            Dim tP1 As Integer, tP2 As Integer

            tP1 = -1
            tP2 = -1

            For i = 0 To j
                If mCharA(i) <> mCharB(i) Then
                    tP1 = i
                    Exit For
                End If
            Next

            If tP1 = -1 Then Return System.Math.Abs(mCharALen - mCharBLen)

            For i = 0 To j - tP1
                If mCharA(mCharALen - i - 1) <> mCharB(mCharBLen - i - 1) Then
                    tP2 = i
                    Exit For
                End If
            Next

            If tP2 = -1 Then Return System.Math.Abs(mCharALen - mCharBLen)

            Dim tA(mCharALen - tP1 - tP2) As Integer

            For i = 0 To tA.GetUpperBound(0)
                tA(i) = i
            Next

            Dim tN1 As Integer, tN2 As Integer, tN3 As Integer

            For i = 0 To mCharBLen - tP1 - tP2 - 1
                tN1 = tA(0)
                tN2 = tN1 + 1
                For j = 1 To tA.GetUpperBound(0)
                    If mCharA(mCharALen - tP2 - j) = mCharB(mCharBLen - tP2 - i - 1) Then
                        tN3 = tN1
                    Else
                        tN3 = Min(tA(j), tN1, tN2) + 1
                    End If
                    tA(j - 1) = tN2
                    tN2 = tN3
                    tN1 = tA(j)
                Next
                tA(tA.GetUpperBound(0)) = tN2
            Next

            Return tA(tA.GetUpperBound(0))
        End Function

        Public Function Min(ByVal ParamArray Num() As Integer) As Integer
            Dim tN As Integer, i As Integer
            If Num.Length = 0 Then Return Nothing
            tN = Num(0)

            For i = 1 To Num.GetUpperBound(0)
                If Num(i) < tN Then tN = Num(i)
            Next

            Return tN
        End Function
    End Module
End Namespace
Namespace Math
    Public Module Combination
        Public Function GetCombination(ByVal Lower As Integer, ByVal Upper As Integer, ByVal Count As Integer, ByVal Index As Integer) As Integer()
            If Count > Upper - Lower + 1 Then Return Nothing
            If Count <= 0 Then Return Nothing
            If Lower > Upper Then Return Nothing
            If Lower < 0 OrElse Upper < 0 Then Return Nothing
            Dim tS() As String = GetC(Lower, Upper, Count, Index).Split(",".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
            Dim tI() As Integer
            ReDim tI(tS.GetUpperBound(0))
            Dim i As Integer
            For i = 0 To tI.GetUpperBound(0)
                tI(i) = tS(i)
            Next
            Return tI
        End Function

        Private Function GetC(ByVal Lower As Integer, ByVal Upper As Integer, ByVal Count As Integer, ByVal Index As Integer) As String
            Dim i As Integer, tS As String
            If Count = Upper - Lower + 1 Then
                tS = ""
                For i = Lower To Upper
                    tS &= i & ","
                Next
                Return tS
            End If
            Index = Index Mod C(Count, Upper - Lower + 1)
            i = C(Count - 1, Upper - Lower)
            If Index < i Then
                tS = Lower & "," & GetC(Lower + 1, Upper, Count - 1, Index)
            Else
                tS = GetC(Lower + 1, Upper, Count, Index - i)
            End If
            Return tS
        End Function

        Public Function C(ByVal C1 As Integer, ByVal C2 As Integer) As Integer
            If C1 < 0 OrElse C1 > C2 OrElse C2 <= 0 Then Return 0
            If C1 = 0 Then Return 1
            Dim i As Integer, S1 As Single = 1
            For i = 1 To C1
                S1 *= (C2 - i + 1) / i
            Next
            Return S1
        End Function
    End Module
End Namespace
Namespace Text
    Public Module LevenshteinDistance
        ''' <summary>
        ''' 取最小的一位数
        ''' </summary>
        ''' <param name="first"></param>
        ''' <param name="second"></param>
        ''' <param name="third"></param>
        ''' <returns></returns>
        Private Function LowerOfThree(first As Integer, second As Integer, third As Integer) As Integer
            Dim min As Integer = System.Math.Min(first, second)
            Return System.Math.Min(min, third)
        End Function

        Private Function Levenshtein_Distance(str1 As String, str2 As String) As Integer
            Dim Matrix As Integer(,)
            Dim n As Integer = str1.Length
            Dim m As Integer = str2.Length

            Dim temp As Integer = 0
            Dim ch1 As Char
            Dim ch2 As Char
            Dim i As Integer = 0
            Dim j As Integer = 0
            If n = 0 Then
                Return m
            End If
            If m = 0 Then

                Return n
            End If
            Matrix = New Integer(n, m) {}

            For i = 0 To n
                '初始化第一列
                Matrix(i, 0) = i
            Next

            For j = 0 To m
                '初始化第一行
                Matrix(0, j) = j
            Next

            For i = 1 To n
                ch1 = str1(i - 1)
                For j = 1 To m
                    ch2 = str2(j - 1)
                    If ch1.Equals(ch2) Then
                        temp = 0
                    Else
                        temp = 1
                    End If
                    Matrix(i, j) = LowerOfThree(Matrix(i - 1, j) + 1, Matrix(i, j - 1) + 1, Matrix(i - 1, j - 1) + temp)
                Next
            Next
            For i = 0 To n
                For j = 0 To m
                    Console.Write(" {0} ", Matrix(i, j))
                Next
                Console.WriteLine("")
            Next

            Return Matrix(n, m)
        End Function

        ''' <summary>
        ''' 计算字符串相似度
        ''' </summary>
        ''' <param name="str1"></param>
        ''' <param name="str2"></param>
        ''' <returns></returns>
        Public Function LevenshteinDistancePercent(str1 As String, str2 As String) As Decimal
            'int maxLenth = str1.Length > str2.Length ? str1.Length : str2.Length;
            Dim val As Integer = Levenshtein_Distance(str1, str2)
            Return 1 - CDec(val) / System.Math.Max(str1.Length, str2.Length)
        End Function
    End Module
End Namespace
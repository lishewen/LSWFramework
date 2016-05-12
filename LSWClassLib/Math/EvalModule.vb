Imports System.Text.RegularExpressions
Imports System.Math

Namespace Math
    Public Module EvalModule
        Function Evaluate(ByVal expr As String) As Double
            Const Num As String = "(\-?\d+\.?\d*)"
            Const Func1 As String = "(exp|log|log10|abs|sqr|sqrt|sin|cos|tan|asin|acos|atan)"
            Const Func2 As String = "(atan2)"
            Const FuncN As String = "(min|max)"
            Const Constants As String = "(e|pi)"

            Dim rePower As New Regex(Num & "\s*(\^)s*" & Num)
            Dim reAddSub As New Regex(Num & "\s*([-+])s*" & Num)
            Dim reMulDiv As New Regex(Num & "\s*([*/])s*" & Num)
            Dim reFunc1 As New Regex(Func1 & "\(\s*" & Num & "\s*\)", RegexOptions.IgnoreCase)
            Dim reFunc2 As New Regex(Func2 & "\(\s*" & Num & "\s*,\s*" & Num & "\s*\)", RegexOptions.IgnoreCase)
            Dim reFuncN As New Regex(FuncN & "\((\s*" & Num & "\s*,)+\s*" & Num & "\s*\)", RegexOptions.IgnoreCase)
            Dim reSign1 As New Regex("([-+/*^])\s*\+")
            Dim reSign2 As New Regex("\-\s*\-")
            Dim rePar As New Regex("(?<![A-Za-z0-9])\(\s*([-+]?\d+.?\d*)\s*\)")
            Dim reNum As New Regex("^\s*[-+]?\d+\.?\d*\s*$")
            Dim reConst As New Regex("\s*" & Constants & "\s*", RegexOptions.IgnoreCase)

            expr = reConst.Replace(expr, AddressOf DoConstants)
            Do Until reNum.IsMatch(expr)
                Dim saveExpr As String = expr
                Do While rePower.IsMatch(expr)
                    expr = rePower.Replace(expr, AddressOf DoPower)
                Loop
                Do While reMulDiv.IsMatch(expr)
                    expr = reMulDiv.Replace(expr, AddressOf DoMulDiv)
                Loop
                Do While reFuncN.IsMatch(expr)
                    expr = reFuncN.Replace(expr, AddressOf DoFuncN)
                Loop
                Do While reFunc2.IsMatch(expr)
                    expr = reFunc2.Replace(expr, AddressOf DoFunc2)
                Loop
                Do While reFunc1.IsMatch(expr)
                    expr = reFunc1.Replace(expr, AddressOf DoFunc1)
                Loop
                expr = reSign1.Replace(expr, "$1")
                expr = reSign2.Replace(expr, "+")
                Do While reAddSub.IsMatch(expr)
                    expr = reAddSub.Replace(expr, AddressOf DoAddsub)
                Loop
                expr = rePar.Replace(expr, "$1")
            Loop
            Return CDbl(expr)
        End Function
        Function DoConstants(ByVal m As Match) As String
            Select Case m.Groups(1).Value.ToUpper
                Case "PI"
                    Return PI.ToString
                Case "E"
                    Return E.ToString
                Case Else
                    Return vbNullString
            End Select
        End Function
        Function DoPower(ByVal m As Match) As String
            Dim n1 As Double = CDbl(m.Groups(1).Value)
            Dim n2 As Double = CDbl(m.Groups(3).Value)
            Return (n1 ^ n2).ToString
        End Function
        Function DoMulDiv(ByVal m As Match) As String
            Dim n1 As Double = CDbl(m.Groups(1).Value)
            Dim n2 As Double = CDbl(m.Groups(3).Value)
            Select Case m.Groups(2).Value
                Case "/"
                    Return (n1 / n2).ToString
                Case "*"
                    Return (n1 * n2).ToString
                Case Else
                    Return vbNullString
            End Select
        End Function
        Function DoAddsub(ByVal m As Match) As String
            Dim n1 As Double = CDbl(m.Groups(1).Value)
            Dim n2 As Double = CDbl(m.Groups(3).Value)
            Select Case m.Groups(2).Value
                Case "+"
                    Return (n1 + n2).ToString
                Case "-"
                    Return (n1 - n2).ToString
                Case Else
                    Return vbNullString
            End Select
        End Function
        Function DoFunc1(ByVal m As Match) As String
            Dim n1 As Double = CDbl(m.Groups(2).Value)
            Select Case m.Groups(1).Value.ToUpper
                Case "EXP"
                    Return Exp(n1).ToString
                Case "LOG"
                    Return Log(n1).ToString
                Case "LOG10"
                    Return Log10(n1).ToString
                Case "ABS"
                    Return Abs(n1).ToString
                Case "SQR", "SQRT"
                    Return Sqrt(n1).ToString
                Case "SIN"
                    Return Sin(n1).ToString
                Case "COS"
                    Return Cos(n1).ToString
                Case "TAN"
                    Return Tan(n1).ToString
                Case "ASIN"
                    Return Asin(n1).ToString
                Case "ACOS"
                    Return Acos(n1).ToString
                Case "ATAN"
                    Return Atan(n1).ToString
                Case Else
                    Return vbNullString
            End Select
        End Function
        Function DoFunc2(ByVal m As Match) As String
            Dim n1 As Double = CDbl(m.Groups(2).Value)
            Dim n2 As Double = CDbl(m.Groups(3).Value)
            Select Case m.Groups(1).Value.ToUpper
                Case "ATAN2"
                    Return Atan2(n1, n2).ToString
                Case Else
                    Return vbNullString
            End Select
        End Function
        Function DoFuncN(ByVal m As Match) As String
            Dim args As New ArrayList()
            Dim i As Integer = 2
            Do While m.Groups(i).Value <> ""
                args.Add(CDbl(m.Groups(i).Value.Replace(","c, " "c)))
                i += 1
            Loop
            Select Case m.Groups(1).Value.ToUpper
                Case "MIN"
                    args.Sort()
                    Return args(0).ToString
                Case "MAX"
                    args.Sort()
                    Return args(args.Count - 1).ToString
                Case Else
                    Return vbNullString
            End Select
        End Function
    End Module
End Namespace
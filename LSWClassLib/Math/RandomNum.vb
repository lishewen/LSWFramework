Namespace Math
    Public Module RandomNum
        '定义 
        Dim ranA As New Random
        Dim intResultRound As Integer = 0
        Dim intA As Integer = 0
        Dim strB As String = ""

        '----获得生成要求---- 
        Public intRound As Integer = 6
        '需要位数 
        Public booNumber As Boolean = True
        '是否生成数字 
        Public booSign As Boolean = False
        '是否生成符号 
        Public booSmallword As Boolean = True
        '是否生成小写字母 
        Public booBigword As Boolean = False
        '是否生成大写字母 
        Public Function GetRandomNum() As String
            '验证checkbox 
            If booNumber = False AndAlso booSign = False AndAlso booSmallword = False AndAlso booBigword = False Then
                strB = "请选择一个生成要求！"
            Else
                '死循环开始（如果生成位数<需要位数） 
                While intResultRound < intRound

                    '生成随机数A，表示生成类型 
                    '1=数字，2=符号，3=小写字母，4=大写字母 
                    intA = ranA.[Next](1, 5)

                    '如果随机数A=1，并且用户打勾了，则运行生成数字 
                    '生成随机数A，范围在0-10 
                    '把随机数A，转成字符 
                    '生成完，位数+1，字符串累加，结束本次循环 
                    If intA = 1 AndAlso booNumber = True Then
                        intA = ranA.[Next](0, 10)
                        strB = intA.ToString() + strB
                        intResultRound = intResultRound + 1
                        Continue While
                    End If

                    '如果随机数A=2，并且用户打勾了，则运行生成符号 
                    '生成随机数A，表示生成值域 
                    '1：33-47值域，2：58-64值域，3：91-96值域，4：123-126值域 
                    If intA = 2 AndAlso booSign = True Then
                        intA = ranA.[Next](1, 5)

                        '如果A=1 
                        '生成随机数A，33-47的Ascii码 
                        '把随机数A，转成字符 
                        '生成完，位数+1，字符串累加，结束本次循环 
                        If intA = 1 Then
                            intA = ranA.[Next](33, 48)
                            strB = ChrW(intA) + strB
                            intResultRound = intResultRound + 1
                            Continue While
                        End If

                        '如果A=2 
                        '生成随机数A，58-64的Ascii码 
                        '把随机数A，转成字符 
                        '生成完，位数+1，字符串累加，结束本次循环 
                        If intA = 2 Then
                            intA = ranA.[Next](58, 65)
                            strB = ChrW(intA) + strB
                            intResultRound = intResultRound + 1
                            Continue While
                        End If

                        '如果A=3 
                        '生成随机数A，91-96的Ascii码 
                        '把随机数A，转成字符 
                        '生成完，位数+1，字符串累加，结束本次循环 
                        If intA = 3 Then
                            intA = ranA.[Next](91, 97)
                            strB = ChrW(intA) + strB
                            intResultRound = intResultRound + 1
                            Continue While
                        End If

                        '如果A=4 
                        '生成随机数A，123-126的Ascii码 
                        '把随机数A，转成字符 
                        '生成完，位数+1，字符串累加，结束本次循环 
                        If intA = 4 Then
                            intA = ranA.[Next](123, 127)
                            strB = ChrW(intA) + strB
                            intResultRound = intResultRound + 1
                            Continue While
                        End If
                    End If

                    '如果随机数A=3，并且用户打勾了，则运行生成小写字母 
                    '生成随机数A，范围在97-122 
                    '把随机数A，转成字符 
                    '生成完，位数+1，字符串累加，结束本次循环 
                    If intA = 3 AndAlso booSmallword = True Then
                        intA = ranA.[Next](97, 123)
                        strB = ChrW(intA) + strB
                        intResultRound = intResultRound + 1
                        Continue While
                    End If

                    '如果随机数A=4，并且用户打勾了，则运行生成大写字母 
                    '生成随机数A，范围在65-90 
                    '把随机数A，转成字符 
                    '生成完，位数+1，字符串累加，结束本次循环 
                    If intA = 4 AndAlso booBigword = True Then
                        intA = ranA.[Next](65, 89)
                        strB = ChrW(intA) + strB
                        intResultRound = intResultRound + 1
                        Continue While
                    End If
                End While
                '死循环结束 
                '显示结果（字符串） 
            End If
            Return strB
        End Function
    End Module
End Namespace

Imports System.Runtime.CompilerServices
Imports System.Globalization

Namespace Extension
    Public Module DateTimeHelper
        Public Const TianGan As String = "甲乙丙丁戊己庚辛壬癸"
        Public Const DiZhi As String = "子丑寅卯辰巳午未申酉戌亥"

        '根据出生日子的天干，通过下表来查算时辰干支：
        '时辰干支查算表
        '时间时辰                             五行纪日干支
        '                       甲己     乙庚     丙辛     丁壬     戊癸
        '23－01 子/水           甲子     丙子     戊子     庚子     壬子
        '01－03 丑/土           乙丑     丁丑     己丑     辛丑     癸丑
        '03－05 寅/木           丙寅     戊寅     庚寅     壬寅     甲寅
        '05－07 卯/木           丁卯     己卯     辛卯     癸卯     乙卯
        '07－09 辰/土           戊辰     庚辰     壬辰     甲辰     丙辰
        '09－11 巳/火           己巳     辛巳     癸巳     己巳     丁巳
        '11－13 午/火           庚午     壬午     甲午     丙午     戊午
        '13－15 未/土           辛未     癸未     乙未     丁未     己未
        '15－17 申/金           壬申     甲申     丙申     戊申     庚申
        '17－19 酉/金           癸酉     乙酉     丁酉     己酉     辛酉
        '19－21 戊/土           甲戌     丙戌     戊戌     庚戌     壬戌
        '21－23 亥/水           乙亥     丁亥     己亥     辛亥     癸亥

        Dim cTimeGanZhi_Table(,) As String = {
             {"甲子", "丙子", "戊子", "庚子", "壬子"},
             {"乙丑", "丁丑", "己丑", "辛丑", "癸丑"},
             {"丙寅", "戊寅", "庚寅", "壬寅", "甲寅"},
             {"丁卯", "己卯", "辛卯", "癸卯", "乙卯"},
             {"戊辰", "庚辰", "壬辰", "甲辰", "丙辰"},
             {"己巳", "辛巳", "癸巳", "己巳", "丁巳"},
             {"庚午", "壬午", "甲午", "丙午", "戊午"},
             {"辛未", "癸未", "乙未", "丁未", "己未"},
             {"壬申", "甲申", "丙申", "戊申", "庚申"},
             {"癸酉", "乙酉", "丁酉", "己酉", "辛酉"},
             {"甲戌", "丙戌", "戊戌", "庚戌", "壬戌"},
             {"乙亥", "丁亥", "己亥", "辛亥", "癸亥"}
            }

        Dim cMonthGanZhi_Table(,) As String = {
            {"丙寅", "丁卯", "戊辰", "己巳", "庚午", "辛未", "壬申", "癸酉", "甲戌", "乙亥", "丙子", "丁丑"},
            {"戊寅", "己卯", "庚辰", "辛巳", "壬午", "癸未", "甲申", "乙酉", "丙戌", "丁亥", "戊子", "己丑"},
            {"庚寅", "辛卯", "壬辰", "癸巳", "甲午", "乙未", "丙申", "丁酉", "戊戌", "己亥", "庚子", "辛丑"},
            {"壬寅", "癸卯", "甲辰", "乙巳", "丙午", "丁未", "戊申", "己酉", "庚戌", "辛亥", "壬子", "癸丑"},
            {"甲寅", "乙卯", "丙辰", "丁巳", "戊午", "己未", "庚申", "辛酉", "壬戌", "癸亥", "甲子", "乙丑"}
            }

        ''' <summary>
        ''' 计算年月日的干支
        ''' </summary>
        ''' <param name="time"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function ComputeYMDGan(time As Date) As String
            Return ComputeYearGan(time) & ComputeMonthGan(time) & ComputeDayGan(time)
        End Function

        ''' <summary>
        ''' 将日期转换成农历干支计数法
        ''' </summary>
        ''' <param name="time"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function ComputeGan(time As Date) As String
            Return ComputeTimeGan(ComputeYMDGan(time), time.Hour)
        End Function

        ''' <summary>
        ''' 计算年份干支
        ''' </summary>
        ''' <param name="time"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ComputeYearGan(time As Date) As String
            Dim chineseDate As New ChineseLunisolarCalendar
            Dim Sexagenary = chineseDate.GetSexagenaryYear(time)

            Return TianGan(chineseDate.GetCelestialStem(Sexagenary) - 1) & DiZhi(chineseDate.GetTerrestrialBranch(Sexagenary) - 1)
        End Function

        ''' <summary>
        ''' 计算时间干支
        ''' </summary>
        ''' <param name="bazi"></param>
        ''' <param name="hour"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ComputeTimeGan(bazi As String, hour As Integer) As String
            Dim dayGan = bazi(4)

            Dim indexX, indexY As Integer

            Dim i As Integer
            i = TianGan.IndexOf(dayGan)
            If i < 0 Then Return String.Empty
            indexX = i
            If indexX >= 5 Then indexX -= 5
            indexY = (hour + 1) / 2

            Return bazi & cTimeGanZhi_Table(indexY - 1, indexX)
        End Function

        ''' <summary>
        ''' 计算月份干支
        ''' </summary>
        ''' <param name="time"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ComputeMonthGan(time As Date) As String
            Dim chineseDate As New ChineseLunisolarCalendar
            Dim Sexagenary = chineseDate.GetSexagenaryYear(time)
            Dim SolarTerm() As String = {
                "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏",
                "小满", "芒种", "夏至", "小暑", "大暑", "立秋",
                "处暑", "白露", "秋分", "寒露", "霜降", "立冬",
                "小雪", "大雪", "冬至", "小寒", "大寒", "立春"
                }

            Dim ctfd = getNextSolarTerms(time)
            Dim lMonth As Integer
            For i = 0 To SolarTerm.Length - 1
                If SolarTerm(i) = ctfd Then
                    lMonth = i \ 2
                    Exit For
                End If
            Next
            'Dim lYear = chineseDate.GetYear(time)
            'Dim lMonth = chineseDate.GetMonth(time)

            ''获取第几个月是闰月,等于0表示本年无闰月
            'Dim leapMonth = chineseDate.GetLeapMonth(lYear)

            ''如果今年有闰月
            'If leapMonth > 0 Then
            '    If lMonth >= leapMonth Then lMonth -= 1
            'End If

            Return cMonthGanZhi_Table((chineseDate.GetCelestialStem(Sexagenary) - 1) Mod 5, lMonth)
        End Function

        ''' <summary>
        ''' 计算日期干支
        ''' </summary>
        ''' <param name="time"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ComputeDayGan(time As Date) As String
            Dim lyear = time.Year
            Dim lMonth = time.Month
            Dim d = time.Day

            If lMonth = 1 OrElse lMonth = 2 Then
                lMonth += 12
                lyear -= 1
            End If

            Dim C = lyear \ 100
            Dim y = lyear - C * 100
            Dim G = 4 * C + C \ 4 + 5 * y + y \ 4 + 3 * (lMonth + 1) \ 5 + d - 3

            Dim i As Integer = IIf(lMonth Mod 2 = 0, 6, 0)

            Dim Z = 8 * C + C \ 4 + 5 * y + y \ 4 + 3 * (lMonth + 1) \ 5 + d + 7 + i

            Return TianGan((G - 1) Mod 10) & DiZhi((Z - 1) Mod 12)
        End Function

        <Extension>
        Public Function ToDateTime(timestamp As Integer) As DateTime
            Return (New DateTime(1970, 1, 1, 0, 0, 0)).AddHours(8).AddSeconds(timestamp)
        End Function

        <Extension>
        Public Function ToTimeStamp(datetime As DateTime) As Integer
            Return (datetime.AddHours(-8) - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
        End Function

        <Extension>
        Public Function ToUtcTimeStamp(datetime As DateTime) As Long
            Return (datetime - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
        End Function

        Public Function CheckBazi(bazi As String) As Boolean
            Dim baziLen As Integer
            Dim i As Integer

            baziLen = Len(bazi)
            If baziLen <> 6 AndAlso baziLen <> 8 Then Return False

            For i = 0 To baziLen - 1
                If Not (TianGan & DiZhi).Contains(bazi(i)) Then Return False
            Next

            Return True
        End Function

        Public Function GetDayOfWeek(str As String) As DayOfWeek
            Select Case str.Replace("星期", "")
                Case "日"
                    Return DayOfWeek.Sunday
                Case "一"
                    Return DayOfWeek.Monday
                Case "二"
                    Return DayOfWeek.Tuesday
                Case "三"
                    Return DayOfWeek.Wednesday
                Case "四"
                    Return DayOfWeek.Thursday
                Case "五"
                    Return DayOfWeek.Friday
                Case "六"
                    Return DayOfWeek.Saturday
                Case Else
                    Return DayOfWeek.Sunday
            End Select
        End Function

        ''' <summary>
        ''' 定气法计算二十四节气,二十四节气是按地球公转来计算的，并非是阴历计算的
        ''' 节气的定法有两种。古代历法采用的称为"恒气"，即按时间把一年等分为24份，
        ''' 每一节气平均得15天有余，所以又称"平气"。现代农历采用的称为"定气"，即
        ''' 按地球在轨道上的位置为标准，一周360°，两节气之间相隔15°。由于冬至时地
        ''' 球位于近日点附近，运动速度较快，因而太阳在黄道上移动15°的时间不到15天。
        ''' 夏至前后的情况正好相反，太阳在黄道上移动较慢，一个节气达16天之多。采用
        ''' 定气时可以保证春、秋两分必然在昼夜平分的那两天。
        ''' </summary>
        ''' <param name="time">日期</param>
        ''' <returns>节气名称</returns>
        Public Function ChineseTwentyFourDay(time As DateTime) As String
            Dim SolarTerm() As String = {
                "小寒", "大寒", "立春", "雨水", "惊蛰", "春分",
                "清明", "谷雨", "立夏", "小满", "芒种", "夏至",
                "小暑", "大暑", "立秋", "处暑", "白露", "秋分",
                "寒露", "霜降", "立冬", "小雪", "大雪", "冬至"
                }
            Dim sTermInfo() As Integer = {
                0, 21208, 42467, 63836, 85337, 107014,
                128867, 150921, 173149, 195551, 218072, 240693,
                263343, 285989, 308563, 331033, 353350, 375494,
                397447, 419210, 440795, 462224, 483532, 504758
                }
            Dim baseDateAndTime As New DateTime(1900, 1, 6, 2, 5, 0)        '#1/6/1900 2:05:00 AM#
            Dim newDate As DateTime
            Dim num As Double
            Dim y As Integer
            Dim tempStr As String = ""

            y = time.Year

            For i As Integer = 1 To 24
                num = 525948.76 * (y - 1900) + sTermInfo(i - 1)

                newDate = baseDateAndTime.AddMinutes(num)            '按分钟计算
                If newDate.DayOfYear = time.DayOfYear Then
                    tempStr = SolarTerm(i - 1)
                    Exit For
                End If
            Next
            Return tempStr
        End Function

        ''' <summary>
        ''' 根据当前节气，获取下一个节气的名称及间隔时间
        ''' </summary>
        ''' <param name="beginDate">当前节气的开始时间</param>
        Public Function getNextSolarTerms(beginDate As DateTime) As String
            Dim str As String = ChineseTwentyFourDay(beginDate)
            If str <> "" Then Return str
            For i = 1 To 16
                str = ChineseTwentyFourDay(beginDate.AddDays(i))
                If str <> "" Then Return str
            Next
            Return String.Empty
        End Function
    End Module
End Namespace
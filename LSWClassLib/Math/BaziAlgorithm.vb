Imports LSW.Extension

Namespace Math
    Public Module BaziAlgorithm
        '十二月份天干强度表
        '生月\四柱天干        甲              乙              丙              丁              戊              己              庚              辛              壬              癸
        '子月                            1.2             1.2             1.0             1.0             1.0             1.0             1.0             1.0             1.2             1.2
        '丑月                            1.06 1.06 1.0             1.0             1.1             1.1             1.14 1.14 1.1             1.1
        '寅月                            1.14 1.14 1.2             1.2             1.06 1.06 1.0             1.0             1.0             1.0
        '卯月                            1.2             1.2             1.2             1.2             1.0             1.0             1.0             1.0             1.0             1.0
        '辰月                            1.1             1.1             1.06 1.06 1.1             1.1             1.1             1.1             1.04 1.04
        '巳月                            1.0             1.0             1.14 1.14 1.14 1.14 1.06 1.06 1.06 1.06
        '午月                            1.0             1.0             1.2             1.2             1.2             1.2             1.0             1.0             1.0             1.0
        '未月                            1.04 1.04 1.1             1.1             1.16 1.16 1.1             1.1             1.0             1.0
        '申月                            1.06 1.06 1.0             1.0             1.0             1.0             1.14 1.14 1.2             1.2
        '酉月                            1.0             1.0             1.0             1.0             1.0             1.0             1.2             1.2             1.2             1.2
        '戌月                            1.0             1.0             1.04 1.04 1.14 1.14 1.16 1.16 1.06 1.06
        '亥月                            1.2             1.2             1.0             1.0             1.0             1.0             1.0             1.0             1.14 1.14

        Dim TianGan_Strength(,) As Double = {
             {1.2, 1.2, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.2, 1.2},
             {1.06, 1.06, 1.0, 1.0, 1.1, 1.1, 1.14, 1.14, 1.1, 1.1},
             {1.14, 1.14, 1.2, 1.2, 1.06, 1.06, 1.0, 1.0, 1.0, 1.0},
             {1.2, 1.2, 1.2, 1.2, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0},
             {1.1, 1.1, 1.06, 1.06, 1.1, 1.1, 1.1, 1.1, 1.04, 1.04},
             {1.0, 1.0, 1.14, 1.14, 1.14, 1.14, 1.06, 1.06, 1.06, 1.06},
             {1.0, 1.0, 1.2, 1.2, 1.2, 1.2, 1.0, 1.0, 1.0, 1.0},
             {1.04, 1.04, 1.1, 1.1, 1.16, 1.16, 1.1, 1.1, 1.0, 1.0},
             {1.06, 1.06, 1.0, 1.0, 1.0, 1.0, 1.14, 1.14, 1.2, 1.2},
             {1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.2, 1.2, 1.2, 1.2},
             {1.0, 1.0, 1.04, 1.04, 1.14, 1.14, 1.16, 1.16, 1.06, 1.06},
             {1.2, 1.2, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.14, 1.14}
            }

        '十二月份地支强度表
        '                        生月       子月         丑月         寅月         卯月         辰月         巳月         午月         未月         申月         酉月         戌月         亥月         
        '地支         支藏
        '子              癸                       1.2             1.1             1.0             1.0             1.04 1.06 1.0             1.0             1.2             1.2             1.06 1.14 
        '丑              癸                       0.36 0.33 0.3             0.3             0.312        0.318        0.3             0.3             0.36 0.36 0.318        0.342
        '丑              辛                       0.2             0.228        0.2             0.2             0.23 0.212        0.2             0.22 0.228        0.248        0.232        0.2            
        '丑              己                       0.5             0.55 0.53 0.5             0.55 0.57 0.6             0.58 0.5             0.5             0.57 0.5             
        '寅              丙                       0.3             0.3             0.36 0.36 0.318        0.342        0.36 0.33 0.3             0.3             0.342        0.318        
        '寅              甲                       0.84 0.742        0.798        0.84 0.77 0.7             0.7             0.728        0.742        0.7             0.7             0.84 
        '卯              乙                       1.2             1.06 1.14 1.2             1.1             1.0             1.0             1.04 1.06 1.0             1.0             1.2            
        '辰              乙                       0.36 0.318        0.342        0.36 0.33 0.3             0.3             0.312        0.318        0.3             0.3             0.36 
        '辰              癸                       0.24 0.22 0.2             0.2             0.208        0.2             0.2             0.2             0.24 0.24 0.212        0.228       
        '辰              戊                       0.5             0.55 0.53 0.5             0.55 0.6             0.6             0.58 0.5             0.5             0.57 0.5             
        '巳              庚                       0.3             0.342        0.3             0.3             0.33 0.3             0.3             0.33 0.342        0.36 0.348        0.3            
        '巳              丙                       0.7             0.7             0.84 0.84 0.742        0.84 0.84 0.798        0.7             0.7             0.728        0.742        
        '午              丁                       1.0             1.0             1.2             1.2             1.06 1.14 1.2             1.1             1.0             1.0             1.04 1.06 
        '未              丁                       0.3             0.3             0.36 0.36 0.318        0.342        0.36 0.33 0.3             0.3             0.312        0.318        
        '未              乙                       0.24 0.212        0.228        0.24 0.22 0.2             0.2             0.208        0.212        0.2             0.2             0.24 
        '未              己                       0.5             0.55 0.53 0.5             0.55 0.57 0.6             0.58 0.5             0.5             0.57 0.5             
        '申              壬                       0.36 0.33 0.3             0.3             0.312        0.318        0.3             0.3             0.36 0.36 0.318        0.342        
        '申              庚                       0.7             0.798        0.7             0.7             0.77 0.742        0.7             0.77 0.798        0.84 0.812        0.7            
        '酉              辛                       1.0             1.14 1.0             1.0             1.1             1.06 1.0             1.1             1.14 1.2             1.16 1.0            
        '戌              辛                       0.3             0.342        0.3             0.3             0.33 0.318        0.3             0.33 0.342        0.36 0.348        0.3             
        '戌              丁                       0.2             0.2             0.24 0.24 0.212        0.228        0.24 0.22 0.2             0.2             0.208        0.212        
        '戌              戊                       0.5             0.55 0.53 0.5             0.55 0.57 0.6             0.58 0.5             0.5             0.57 0.5             
        '亥              甲                       0.36 0.318        0.342        0.36 0.33 0.3             0.3             0.312        0.318        0.3             0.3             0.36 
        '亥              壬                        0.84 0.77 0.7             0.7             0.728        0.742        0.7             0.7             0.84 0.84 0.724        0.798     

        Structure ZISTRENGTH
            Dim diZhi As Char
            Dim zhiCang As Char
            Dim strength() As Double

            Sub New(d As Char, z As Char, s() As Double)
                diZhi = d
                zhiCang = z
                strength = s
            End Sub
        End Structure

        Dim DiZhi_Strength() As ZISTRENGTH = {
             New ZISTRENGTH("子", "癸", {1.2, 1.1, 1.0, 1.0, 1.04, 1.06, 1.0, 1.0, 1.2, 1.2, 1.06, 1.14}),
             New ZISTRENGTH("丑", "癸", {0.36, 0.33, 0.3, 0.3, 0.312, 0.318, 0.3, 0.3, 0.36, 0.36, 0.318, 0.342}),
             New ZISTRENGTH("丑", "辛", {0.2, 0.228, 0.2, 0.2, 0.23, 0.212, 0.2, 0.22, 0.228, 0.248, 0.232, 0.2}),
             New ZISTRENGTH("丑", "己", {0.5, 0.55, 0.53, 0.5, 0.55, 0.57, 0.6, 0.58, 0.5, 0.5, 0.57, 0.5}),
             New ZISTRENGTH("寅", "丙", {0.3, 0.3, 0.36, 0.36, 0.318, 0.342, 0.36, 0.33, 0.3, 0.3, 0.342, 0.318}),
             New ZISTRENGTH("寅", "甲", {0.84, 0.742, 0.798, 0.84, 0.77, 0.7, 0.7, 0.728, 0.742, 0.7, 0.7, 0.84}),
             New ZISTRENGTH("卯", "乙", {1.2, 1.06, 1.14, 1.2, 1.1, 1.0, 1.0, 1.04, 1.06, 1.0, 1.0, 1.2}),
             New ZISTRENGTH("辰", "乙", {0.36, 0.318, 0.342, 0.36, 0.33, 0.3, 0.3, 0.312, 0.318, 0.3, 0.3, 0.36}),
             New ZISTRENGTH("辰", "癸", {0.24, 0.22, 0.2, 0.2, 0.208, 0.2, 0.2, 0.2, 0.24, 0.24, 0.212, 0.228}),
             New ZISTRENGTH("辰", "戊", {0.5, 0.55, 0.53, 0.5, 0.55, 0.6, 0.6, 0.58, 0.5, 0.5, 0.57, 0.5}),
             New ZISTRENGTH("巳", "庚", {0.3, 0.342, 0.3, 0.3, 0.33, 0.3, 0.3, 0.33, 0.342, 0.36, 0.348, 0.3}),
             New ZISTRENGTH("巳", "丙", {0.7, 0.7, 0.84, 0.84, 0.742, 0.84, 0.84, 0.798, 0.7, 0.7, 0.728, 0.742}),
             New ZISTRENGTH("午", "丁", {1.0, 1.0, 1.2, 1.2, 1.06, 1.14, 1.2, 1.1, 1.0, 1.0, 1.04, 1.06}),
             New ZISTRENGTH("未", "丁", {0.3, 0.3, 0.36, 0.36, 0.318, 0.342, 0.36, 0.33, 0.3, 0.3, 0.312, 0.318}),
             New ZISTRENGTH("未", "乙", {0.24, 0.212, 0.228, 0.24, 0.22, 0.2, 0.2, 0.208, 0.212, 0.2, 0.2, 0.24}),
             New ZISTRENGTH("未", "己", {0.5, 0.55, 0.53, 0.5, 0.55, 0.57, 0.6, 0.58, 0.5, 0.5, 0.57, 0.5}),
             New ZISTRENGTH("申", "壬", {0.36, 0.33, 0.3, 0.3, 0.312, 0.318, 0.3, 0.3, 0.36, 0.36, 0.318, 0.342}),
             New ZISTRENGTH("申", "庚", {0.7, 0.798, 0.7, 0.7, 0.77, 0.742, 0.7, 0.77, 0.798, 0.84, 0.812, 0.7}),
             New ZISTRENGTH("酉", "辛", {1.0, 1.14, 1.0, 1.0, 1.1, 1.06, 1.0, 1.1, 1.14, 1.2, 1.16, 1.0}),
             New ZISTRENGTH("戌", "辛", {0.3, 0.342, 0.3, 0.3, 0.33, 0.318, 0.3, 0.33, 0.342, 0.36, 0.348, 0.3}),
             New ZISTRENGTH("戌", "丁", {0.2, 0.2, 0.24, 0.24, 0.212, 0.228, 0.24, 0.22, 0.2, 0.2, 0.208, 0.212}),
             New ZISTRENGTH("戌", "戊", {0.5, 0.55, 0.53, 0.5, 0.55, 0.57, 0.6, 0.58, 0.5, 0.5, 0.57, 0.5}),
             New ZISTRENGTH("亥", "甲", {0.36, 0.318, 0.342, 0.36, 0.33, 0.3, 0.3, 0.312, 0.318, 0.3, 0.3, 0.36}),
             New ZISTRENGTH("亥", "壬", {0.84, 0.77, 0.7, 0.7, 0.728, 0.742, 0.7, 0.7, 0.84, 0.84, 0.724, 0.798})
            }

        '金 --- 0
        '木 --- 1
        '水 --- 2
        '火 --- 3
        '土 --- 4

        Dim WuXingTable() As Char = {"金", "木", "水", "火", "土"}

        '天干地支的五行属性表
        '天干： 甲-木、乙-木、丙-火、丁－火、戊－土、己－土、庚－金、辛－金、壬－水、癸－水 
        '地支：子-水、丑-土、寅-木、卯－木、辰－土、巳－火、午－火、未－土、申－金、酉－金、戌－土、亥－水

        Dim TianGan_WuXingProp() As Integer = {1, 1, 3, 3, 4, 4, 0, 0, 2, 2}
        Dim DiZhi_WuXingProp() As Integer = {2, 4, 1, 1, 4, 3, 3, 4, 0, 0, 4, 2}
        Dim GenerationSourceTable() As Integer = {4, 2, 0, 1, 3}

        Function ComputeGanIndex(gan As Char) As Integer
            Return TianGan.IndexOf(gan)
        End Function

        Function ComputeZhiIndex(zhi As Char) As Integer
            Return DiZhi.IndexOf(zhi)
        End Function

        Dim strengthResult(5) As Double

        Public ReadOnly Property 金强 As Double
            Get
                Return strengthResult(0)
            End Get
        End Property

        Public ReadOnly Property 木强 As Double
            Get
                Return strengthResult(1)
            End Get
        End Property

        Public ReadOnly Property 水强 As Double
            Get
                Return strengthResult(2)
            End Get
        End Property

        Public ReadOnly Property 火强 As Double
            Get
                Return strengthResult(3)
            End Get
        End Property

        Public ReadOnly Property 土强 As Double
            Get
                Return strengthResult(4)
            End Get
        End Property

        Public Property 命属 As Char
        Public Property 相生 As Char
        Public Property 同类 As Double
        Public Property 异类 As Double

        ''' <summary>
        ''' 计算八字
        ''' </summary>
        ''' <param name="bazi"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EvalBazi(bazi As String) As String
            If Not CheckBazi(bazi) Then Return String.Empty

            Dim result As String = String.Empty

            'Dim strengthResult(5) As Double
            Dim monthIndex = ComputeZhiIndex(bazi(3))
            If monthIndex = -1 Then Return String.Empty

            result &= bazi
            result &= vbCrLf & vbCrLf

            For wuXing = 0 To 4
                Dim value1 = 0.0
                Dim value2 = 0.0
                Dim i As Integer
                '扫描4个天干
                For i = 0 To 7 Step 2
                    Dim gan = bazi(i)
                    Dim index = ComputeGanIndex(gan)
                    If index = -1 Then Return String.Empty

                    If TianGan_WuXingProp(index) = wuXing Then
                        value1 += TianGan_Strength(monthIndex, index)
                    End If
                Next

                '扫描支藏
                For i = 1 To 7 Step 2
                    Dim zhi = bazi(i)
                    For j = 0 To DiZhi_Strength.Length - 1
                        If DiZhi_Strength(j).diZhi = zhi Then
                            Dim zhiCangIndex = ComputeGanIndex(DiZhi_Strength(j).zhiCang)
                            If zhiCangIndex = -1 Then Return String.Empty
                            If TianGan_WuXingProp(zhiCangIndex) = wuXing Then
                                value2 += DiZhi_Strength(j).strength(monthIndex)
                                Exit For
                            End If
                        End If
                    Next
                Next

                strengthResult(wuXing) = value1 + value2

                '输出一行计算结果
                result &= String.Format("{0}:" & vbTab & "{1:N3}+{2:N3}={3:N3}", WuXingTable(wuXing), value1, value2, value1 + value2) & vbCrLf
            Next

            Dim fateProp, srcProp As Integer

            fateProp = TianGan_WuXingProp(ComputeGanIndex(bazi(4)))
            If fateProp = -1 Then Return String.Empty
            result &= vbCrLf & String.Format("命属{0}", WuXingTable(fateProp)) & vbCrLf

            命属 = WuXingTable(fateProp)

            '求同类和异类的强度值
            srcProp = GenerationSourceTable(fateProp)

            相生 = WuXingTable(srcProp)

            Dim tongLei = strengthResult(fateProp) + strengthResult(srcProp)
            Dim yiLei = 0.0
            For i = 0 To 4
                yiLei += strengthResult(i)
            Next
            yiLei -= tongLei

            result &= String.Format("同类：{0}+{1}，", WuXingTable(fateProp), WuXingTable(srcProp)) & String.Format("{0:N3}+{1:N3}={2:N3}", strengthResult(fateProp), strengthResult(srcProp), tongLei) & vbCrLf
            同类 = tongLei
            result &= "异类：总和为 " & String.Format("{0:N3}", yiLei) & vbCrLf
            异类 = yiLei
            Return result
        End Function
    End Module
End Namespace
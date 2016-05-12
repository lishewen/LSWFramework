Imports System.Runtime.InteropServices
Imports System.Text

Namespace Device.URF
    Public Module URFHelper
        Public icdev As Long
        Public st As Integer
        Public cardid As Integer

        ''' <summary>
        ''' 连接设备
        ''' </summary>
        ''' <param name="port">USB端口，默认传0</param>
        ''' <param name="baud">波特率</param>
        ''' <returns></returns>
        <DllImport("mwrf32.dll", CallingConvention:=CallingConvention.ThisCall, CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Public Function rf_init(ByVal port As Integer, ByVal baud As Long) As Integer
        End Function

        ''' <summary>
        ''' 断开设备
        ''' </summary>
        ''' <param name="icdev">设备id</param>
        ''' <returns></returns>
        <DllImport("mwrf32.dll", CallingConvention:=CallingConvention.StdCall, CharSet:=CharSet.Ansi, SetLastError:=True, ExactSpelling:=True)>
        Public Function rf_exit(ByVal icdev As Integer) As Integer
        End Function

        ''' <summary>
        ''' 获取设备版本号
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="status"></param>
        ''' <returns></returns>
        Public Declare Function rf_get_status Lib "mwrf32.dll" (ByVal icdev As Long, ByVal status As String) As Integer
        ''' <summary>
        ''' 蜂鸣器
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="time1"></param>
        ''' <returns></returns>
        <DllImport("mwrf32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)>
        Public Function rf_beep(ByVal icdev As Integer, ByVal time1 As Integer) As Integer
        End Function

        ''' <summary>
        ''' 中止对该卡操作
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <returns></returns>
        <DllImport("mwrf32.dll")>
        Public Function rf_halt(ByVal icdev As Integer) As Integer
        End Function

        ''' <summary>
        ''' 寻卡请求
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="mode"></param>
        ''' <param name="atr_type"></param>
        ''' <returns></returns>
        Public Declare Function rf_request Lib "mwrf32.dll" (ByVal icdev As Long, ByVal mode As Integer, ByRef atr_type As Integer) As Integer
        ''' <summary>
        ''' 寻卡
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="mode"></param>
        ''' <param name="Snr"></param>
        ''' <returns></returns>
        <DllImport("mwrf32.dll")>
        Public Function rf_card(ByVal icdev As Integer, ByVal mode As Integer, ByRef Snr As Integer) As Short
        End Function

        ''' <summary>
        ''' 卡防冲突，返回卡的序列号
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="Bcnt"></param>
        ''' <param name="Snr"></param>
        ''' <returns></returns>
        Public Declare Function rf_anticoll Lib "mwrf32.dll" (ByVal icdev As Long, ByVal Bcnt As Integer, ByRef Snr As Long) As Integer
        ''' <summary>
        ''' 将密码装入读写模块RAM中
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="mode"></param>
        ''' <param name="secnr"></param>
        ''' <param name="nkey"></param>
        ''' <returns></returns>
        Public Declare Function rf_load_key Lib "mwrf32.dll" (ByVal icdev As Long, ByVal mode As Integer, ByVal secnr As Integer, ByRef nkey As Byte) As Integer
        ''' <summary>
        ''' 密码装载到读写模块中
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="mode"></param>
        ''' <param name="secnr"></param>
        ''' <param name="keybuff"></param>
        ''' <returns></returns>
        <DllImport("mwrf32.dll")>
        Public Function rf_load_key_hex(ByVal icdev As Integer, ByVal mode As Integer, ByVal secnr As Integer, ByVal keybuff As String) As Short
        End Function

        ''' <summary>
        ''' 验证某一扇区密码
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="mode"></param>
        ''' <param name="scenr"></param>
        ''' <returns></returns>
        <DllImport("mwrf32.dll")>
        Public Function rf_authentication(ByVal icdev As Integer, ByVal Mode As Integer, ByVal scenr As Integer) As Short
        End Function

        ''' <summary>
        ''' 验证某一扇区密码 2
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="mode"></param>
        ''' <param name="keynr"></param>
        ''' <param name="Adr"></param>
        ''' <returns></returns>
        Public Declare Function rf_authentication_2 Lib "mwrf32.dll" (ByVal icdev As Long, ByVal mode As Integer, ByVal keynr As Integer, ByVal Adr As Integer) As Integer
        ''' <summary>
        ''' 向卡中写入数据
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="Adr"></param>
        ''' <param name="sdata"></param>
        ''' <returns></returns>
        <DllImport("mwrf32.dll")>
        Public Function rf_write(ByVal icdev As Integer, ByVal adr As Integer, <[In]> ByVal sdata As String) As Short
        End Function

        ''' <summary>
        ''' 读取数据
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="Adr"></param>
        ''' <param name="sdata"></param>
        ''' <returns></returns>
        <DllImport("mwrf32.dll")>
        Public Function rf_read(ByVal icdev As Integer, ByVal adr As Integer, <MarshalAs(UnmanagedType.LPStr)> ByVal sdata As StringBuilder) As Short
        End Function

        ''' <summary>
        ''' 读取数据
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="Adr"></param>
        ''' <param name="sdata"></param>
        ''' <returns></returns>
        Public Declare Function rf_read_hex Lib "mwrf32.dll" (ByVal icdev As Long, ByVal Adr As Integer, ByVal sdata As String) As Integer
        Public Declare Function rf_reset Lib "mwrf32.dll" (ByVal icdev As Long, ByVal msec As Int16) As Integer
        ''' <summary>
        ''' 块加值
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="Adr"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Declare Function rf_increment Lib "mwrf32.dll" (ByVal icdev As Long, ByVal Adr As Integer, ByVal value As Long) As Integer
        ''' <summary>
        ''' 块减值
        ''' </summary>
        ''' <param name="icdev"></param>
        ''' <param name="Adr"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Declare Function rf_decrement Lib "mwrf32.dll" (ByVal icdev As Long, ByVal Adr As Integer, ByVal value As Long) As Integer

        Public Sub Quit()
            If icdev > 0 Then
                'st = rf_reset(icdev, 10)
                '释放设备
                rf_halt(icdev)
                rf_exit(icdev)
                icdev = -1
            End If
        End Sub

        Public Sub Beep()
            '初始化
            icdev = rf_init(0, 9600)
            If icdev < 0 Then Throw New URFException(icdev)

            rf_beep(icdev, 10)

            Quit()
        End Sub

        ''' <summary>
        ''' 获取CardID
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCardID() As Integer
            '初始化
            icdev = rf_init(0, 9600)
            If icdev < 0 Then Throw New URFException(icdev)
            '寻卡
            Dim result = rf_card(icdev, 1, cardid)
            If result <> 0 Then Throw New URFException(result)
            Quit()
            Return cardid
        End Function

        ''' <summary>
        ''' 写数据
        ''' </summary>
        ''' <param name="scenr">扇区</param>
        ''' <param name="adr">块 0,1,2</param>
        ''' <param name="data">数据</param>
        Public Sub WriteData(ByVal scenr As Integer, ByVal adr As Integer, data As String)
            If data.Length > 16 Then
                data = Left(data, 16)
            End If

            For Each c In data
                'If Asc(c) > 127 Then Throw New ArgumentException("字符串内含有ASCII已外的字符")
                If AscW(c) > 65535 Then Return
            Next

            '初始化
            icdev = rf_init(0, 9600)
            If icdev < 0 Then Throw New URFException(icdev)
            '寻卡
            Dim result = rf_card(icdev, 1, cardid)
            If result <> 0 Then Throw New URFException(result)
            '加载密码
            result = rf_load_key_hex(icdev, 0, scenr, "FFFFFFFFFFFF")
            If result <> 0 Then Throw New URFException(result)
            '验证该扇区密码
            result = rf_authentication(icdev, 0, scenr)
            '写
            result = rf_write(icdev, scenr * 4 + adr, data)
            If result <> 0 Then Throw New URFException(result)
            Quit()
        End Sub

        ''' <summary>
        ''' 读数据
        ''' </summary>
        ''' <param name="scenr">扇区</param>
        ''' <param name="adr">块 0,1,2</param>
        Public Function ReadData(ByVal scenr As Integer, ByVal adr As Integer) As String
            '初始化
            icdev = rf_init(0, 9600)
            If icdev < 0 Then Throw New URFException(icdev)
            '寻卡
            Dim result = rf_card(icdev, 1, cardid)
            If result <> 0 Then Throw New URFException(result)
            '加载密码
            result = rf_load_key_hex(icdev, 0, scenr, "FFFFFFFFFFFF")
            If result <> 0 Then Throw New URFException(result)
            '验证该扇区密码
            result = rf_authentication(icdev, 0, scenr)
            '读
            Dim tempData As New StringBuilder(64)
            Dim str As String
            result = rf_read(icdev, scenr * 4 + adr, tempData)
            If result <> 0 Then Throw New URFException(result)
            If tempData.Length > 16 Then
                str = tempData.ToString.Substring(0, 16)
            Else
                str = tempData.ToString()
            End If
            Quit()
            Return str
        End Function
    End Module
End Namespace
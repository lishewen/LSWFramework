Imports System.Management
Imports System.Net.Sockets
Imports System.Net

Namespace Net
    Public Module NetCard
        ''' <summary>       
        ''' 获取本机MAC地址        
        ''' </summary>       
        ''' <returns>返回当前机器上的所有MAC地址</returns>        
        Public Function GetMacs() As String()
            Dim mc As New ManagementClass("Win32_NetworkAdapterConfiguration")
            Dim moc As ManagementObjectCollection = mc.GetInstances()
            Dim strMac As String = ""
            For Each mo As ManagementObject In moc
                If CBool(mo("IPEnabled")) = True Then
                    strMac &= "|" & mo("MacAddress").ToString()
                End If
            Next
            Return strMac.Split("|")
        End Function
        Public Function GetMac() As String
            Dim s = GetMacs()
            Return IIf(s.Length > 1, s(1), String.Empty)
        End Function
        ''' <summary>        
        ''' 获取本机所有IP地址        
        ''' </summary>        
        ''' <returns>返回当前机器所有IP地址</returns>        
        Public Function GetIPs() As String()
            Dim mc As New ManagementClass("Win32_NetworkAdapterConfiguration")
            Dim moc As ManagementObjectCollection = mc.GetInstances()
            Dim strIP As String = ""
            For Each mo As ManagementObject In moc
                If CBool(mo("IPEnabled")) = True Then
                    strIP &= "|" & TryCast(mo("IPAddress"), String())(0)
                End If
            Next
            Return strIP.Split("|")
        End Function

        Public Function GetIP() As String
            Dim s = GetIPs()
            Return IIf(s.Length > 1, s(1), String.Empty)
        End Function

        ''' <summary>
        ''' 获取IP
        ''' </summary>
        ''' <param name="URL">网址</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIP(ByVal URL As String) As String
            Try
                Dim IPS() As IPAddress
                IPS = Dns.GetHostAddresses(URL)
                If IPS.Length > 0 Then Return IPS(0).ToString
            Catch ex As Exception
                Return "无法解析域名"
            End Try
            Return "无法解析域名"
        End Function

        Public Function WakeUp(mac As Byte()) As Integer
            Dim client As New UdpClient()
            client.Connect(IPAddress.Broadcast, 30000)

            Dim packet(17 * 6 - 1) As Byte

            For i As Integer = 0 To 5
                packet(i) = &HFF
            Next

            For i As Integer = 1 To 16
                For j As Integer = 0 To 5
                    packet(i * 6 + j) = mac(j)
                Next
            Next

            Return client.Send(packet, packet.Length)
        End Function
    End Module
End Namespace
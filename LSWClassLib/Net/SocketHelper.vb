Imports System.Net.Sockets
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Text
Imports LSW.Extension
Imports System.IO
Imports LSW.Exceptions

Namespace Net
    Public Module SocketHelper
        ''' <summary>
        ''' 连接使用 tcp 协议的服务端
        ''' </summary>
        ''' <param name="ip">服务端的ip地址</param>
        ''' <param name="port">服务端的端口号</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ConnectServer(ip As String, port As Integer) As Socket
            Dim s As Socket = Nothing
            Try
                Dim ipa = IPAddress.Parse(ip)
                Dim ipe As New IPEndPoint(ipa, port)
                s = New Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                s.Connect(ipe)
                If Not s.Connected Then
                    s = Nothing
                End If
            Catch ex As Exception
                Dim e As New LSWFrameworkException(ex)
            End Try
            Return s
        End Function

        ''' <summary>
        ''' 用主机名称连接使用Tcp协议的服务端
        ''' </summary>
        ''' <param name="hostName">在hosts 文件中存在的主机名称</param>
        ''' <param name="port">服务端的端口号</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ConnectServerByHostName(hostName As String, port As Integer) As Socket
            Dim s As Socket = Nothing
            Dim iphe As IPHostEntry
            Try
                iphe = Dns.GetHostEntry(hostName)
                For Each i In iphe.AddressList
                    Dim ipe As New IPEndPoint(i, port)
                    Dim tmp As New Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                    tmp.Connect(ipe)
                    If tmp.Connected Then
                        s = tmp
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Dim e As New LSWFrameworkException(ex)
            End Try
            Return s
        End Function

        ''' <summary>
        ''' 向远程主机发送数据
        ''' </summary>
        ''' <param name="s">要发送数据且已经连接到远程主机的 Socket</param>
        ''' <param name="buffer">待发送的数据</param>
        ''' <param name="outTime">发送数据的超时时间，以秒为单位，可以精确到微秒</param>
        ''' <returns>0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常</returns>
        ''' <remarks>当 outTime 指定为-1时，将一直等待直到有数据需要发送</remarks>
        <Extension>
        Public Function SendData(s As Socket, buffer() As Byte, outTime As Integer) As Integer
            If s Is Nothing OrElse Not s.Connected Then
                Throw New ArgumentException("参数socket 为null，或者未连接到远程计算机")
            End If

            If buffer Is Nothing OrElse buffer.Length = 0 Then
                Throw New ArgumentException("参数buffer 为null ,或者长度为 0")
            End If

            Dim flag = 0
            Try
                Dim left = buffer.Length
                Dim sndLen = 0

                While True
                    If s.Poll(outTime * 1000000, SelectMode.SelectWrite) Then
                        sndLen = s.Send(buffer, sndLen, left, SocketFlags.None)
                        left -= sndLen
                        If left = 0 Then
                            flag = 0
                            Exit While
                        ElseIf sndLen > 0 Then
                            Continue While
                        Else
                            flag = -2
                            Exit While
                        End If
                    Else
                        flag = -1
                        Exit While
                    End If
                End While
            Catch ex As SocketException
                flag = -3
            End Try
            Return flag
        End Function

        ''' <summary>
        ''' 向远程主机发送数据
        ''' </summary>
        ''' <param name="s">要发送数据且已经连接到远程主机的 Socket</param>
        ''' <param name="buffer">待发送的字符串</param>
        ''' <param name="outTime">发送数据的超时时间，以秒为单位，可以精确到微秒</param>
        ''' <param name="encode">字符编码</param>
        ''' <returns>0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常</returns>
        ''' <remarks>当 outTime 指定为-1时，将一直等待直到有数据需要发送</remarks>
        <Extension>
        Public Function SendData(s As Socket, buffer As String, outTime As Integer, encode As Encoding) As Integer
            If buffer Is Nothing OrElse buffer.Length = 0 Then
                Throw New ArgumentException("待发送的字符串长度不能为零.")
            End If

            Return SendData(s, encode.GetBytes(buffer), outTime)
        End Function

        ''' <summary>
        ''' 向远程主机发送数据
        ''' </summary>
        ''' <param name="s">要发送数据且已经连接到远程主机的 Socket</param>
        ''' <param name="buffer">待发送的字符串</param>
        ''' <param name="outTime">发送数据的超时时间，以秒为单位，可以精确到微秒</param>
        ''' <returns>0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常</returns>
        ''' <remarks>当 outTime 指定为-1时，将一直等待直到有数据需要发送</remarks>
        <Extension>
        Public Function SendData(s As Socket, buffer As String, outTime As Integer) As Integer
            Return SendData(s, buffer, outTime, Encoding.Default)
        End Function

        ''' <summary> 
        ''' 接收远程主机发送的数据 
        ''' </summary> 
        ''' <param name="socket">要接收数据且已经连接到远程主机的 socket</param> 
        ''' <param name="buffer">接收数据的缓冲区</param> 
        ''' <param name="outTime">接收数据的超时时间，以秒为单位，可以精确到微秒</param> 
        ''' <returns>0:接收数据成功；-1:超时；-2:接收数据出现错误；-3:接收数据时出现异常</returns> 
        ''' <remarks > 
        ''' 1、当 outTime 指定为-1时，将一直等待直到有数据需要接收； 
        ''' 2、需要接收的数据的长度，由 buffer 的长度决定。 
        ''' </remarks> 
        <Extension>
        Public Function RecvData(socket As Socket, buffer As Byte(), outTime As Integer) As Integer
            If socket Is Nothing OrElse socket.Connected = False Then
                Throw New ArgumentException("参数socket 为null，或者未连接到远程计算机")
            End If
            If buffer Is Nothing OrElse buffer.Length = 0 Then
                Throw New ArgumentException("参数buffer 为null ,或者长度为 0")
            End If
            buffer.Initialize()
            Dim left As Integer = buffer.Length
            Dim curRcv As Integer = 0
            Dim curSpace As Integer = 0
            'update by danny
            Dim flag As Integer = 0

            Try
                While True
                    If socket.Poll(outTime * 1000000, SelectMode.SelectRead) Then
                        ' 已经有数据等待接收 
                        curRcv = socket.Receive(buffer, curSpace, left, SocketFlags.None)
                        'update by danny
                        curSpace += curRcv
                        'update by danny
                        left -= curRcv
                        If left = 0 Then
                            ' 数据已经全部接收 
                            flag = 0
                            Exit While
                        Else
                            If curRcv > 0 Then
                                ' 数据已经部分接收 
                                Continue While
                            Else
                                ' 出现错误 
                                flag = -2
                                Exit While
                            End If
                        End If
                    Else
                        ' 超时退出 
                        flag = -1
                        Exit While
                    End If
                End While
            Catch e As SocketException
                flag = -3
            End Try
            Return flag
        End Function

        ''' <summary>
        ''' 接收远程主机发送的数据
        ''' </summary>
        ''' <param name="s">要接收数据且已经连接到远程主机的 socket</param>
        ''' <param name="buffer">存储接收到的数据的字符串</param>
        ''' <param name="bufferLen">待接收的数据的长度</param>
        ''' <param name="outTime">接收数据的超时时间，以秒为单位，可以精确到微秒</param>
        ''' <param name="encode">字符编码</param>
        ''' <returns>0:接收数据成功；-1:超时；-2:接收数据出现错误；-3:接收数据时出现异常</returns>
        ''' <remarks>当 outTime 指定为-1时，将一直等待直到有数据需要接收；</remarks>
        <Extension>
        Public Function RecvData(s As Socket, buffer As String, bufferLen As Integer, outTime As Integer, encode As Encoding) As Integer
            If bufferLen <= 0 Then
                Throw New ArgumentException("存储待接收数据的缓冲区长度必须大于0")
            End If

            Dim tmp(bufferLen - 1) As Byte
            Dim flag = 0
            If (InlineAssignHelper(flag, RecvData(s, tmp, outTime))) = 0 Then
                buffer = encode.GetString(tmp)
            End If

            Return flag
        End Function

        ''' <summary>
        ''' 接收远程主机发送的数据
        ''' </summary>
        ''' <param name="s">要接收数据且已经连接到远程主机的 socket</param>
        ''' <param name="buffer">存储接收到的数据的字符串</param>
        ''' <param name="bufferLen">待接收的数据的长度</param>
        ''' <param name="outTime">接收数据的超时时间，以秒为单位，可以精确到微秒</param>
        ''' <returns>0:接收数据成功；-1:超时；-2:接收数据出现错误；-3:接收数据时出现异常</returns>
        ''' <remarks>当 outTime 指定为-1时，将一直等待直到有数据需要接收；</remarks>
        <Extension>
        Public Function RecvData(s As Socket, buffer As String, bufferLen As Integer, outTime As Integer) As Integer
            Return RecvData(s, buffer, bufferLen, outTime, Encoding.Default)
        End Function

        ''' <summary>
        ''' 向远程主机发送文件
        ''' </summary>
        ''' <param name="s">要发送数据且已经连接到远程主机的 socket</param>
        ''' <param name="filename">待发送的文件名称</param>
        ''' <param name="maxBufferLength">文件发送时的缓冲区大小</param>
        ''' <param name="outTime">发送缓冲区中的数据的超时时间</param>
        ''' <returns>0:发送文件成功；-1:超时；-2:发送文件出现错误；-3:发送文件出现异常；-4:读取待发送文件发生错误</returns>
        ''' <remarks>当 outTime 指定为-1时，将一直等待直到有数据需要发送</remarks>
        <Extension>
        Public Function SendFile(s As Socket, filename As String, maxBufferLength As Integer, outTime As Integer) As Integer
            If filename Is Nothing OrElse maxBufferLength <= 0 Then
                Throw New ArgumentException("待发送的文件名称为空或发送缓冲区的大小设置不正确.")
            End If

            Dim flag = 0
            Try
                Dim fs As New FileStream(filename, FileMode.Open, FileAccess.Read)
                Dim fileLen = fs.Length
                Dim leftLen = fileLen
                Dim readLen = 0
                Dim buffer() As Byte

                If fileLen <= maxBufferLength Then
                    ReDim buffer(fileLen - 1)
                    readLen = fs.Read(buffer, 0, fileLen)
                    flag = SendData(s, buffer, outTime)
                Else
                    ReDim buffer(maxBufferLength - 1)
                    While leftLen > 0
                        readLen = fs.Read(buffer, 0, maxBufferLength)
                        If InlineAssignHelper(flag, SendData(s, buffer, outTime)) < 0 Then
                            Exit While
                        End If
                        leftLen -= readLen
                    End While
                End If

                fs.Close()
            Catch ex As IOException
                flag = -4
            End Try
            Return flag
        End Function

        ''' <summary>
        ''' 向远程主机发送文件
        ''' </summary>
        ''' <param name="s">要发送数据且已经连接到远程主机的 socket</param>
        ''' <param name="filename">待发送的文件名称</param>
        ''' <returns>0:发送文件成功；-1:超时；-2:发送文件出现错误；-3:发送文件出现异常；-4:读取待发送文件发生错误</returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function SendFile(s As Socket, filename As String) As Integer
            Return SendFile(s, filename, 2048, 1)
        End Function

        ''' <summary>
        ''' 接收远程主机发送的文件
        ''' </summary>
        ''' <param name="s">待接收数据且已经连接到远程主机的 socket</param>
        ''' <param name="filename">保存接收到的数据的文件名</param>
        ''' <param name="fileLength">待接收的文件的长度</param>
        ''' <param name="maxBufferLength">接收文件时最大的缓冲区大小</param>
        ''' <param name="outTime">接受缓冲区数据的超时时间</param>
        ''' <returns>0:接收文件成功；-1:超时；-2:接收文件出现错误；-3:接收文件出现异常；-4:写入接收文件发生错误</returns>
        ''' <remarks>当 outTime 指定为-1时，将一直等待直到有数据需要接收</remarks>
        <Extension>
        Public Function RecvFile(s As Socket, filename As String, fileLength As Long, maxBufferLength As Integer, outTime As Integer) As Integer
            If filename Is Nothing OrElse maxBufferLength <= 0 Then
                Throw New ArgumentException("保存接收数据的文件名称为空或发送缓冲区的大小设置不正确.")
            End If

            Dim flag = 0
            Try
                Dim fs As New FileStream(filename, FileMode.Create)
                Dim buffer() As Byte

                If (fileLength <= maxBufferLength) Then
                    ReDim buffer(fileLength - 1)
                    If InlineAssignHelper(flag, RecvData(s, buffer, outTime)) = 0 Then
                        fs.Write(buffer, 0, fileLength)
                    End If
                Else
                    Dim rcvLen = maxBufferLength
                    Dim leftLen = fileLength
                    ReDim buffer(rcvLen - 1)

                    While leftLen > 0
                        If InlineAssignHelper(flag, RecvData(s, buffer, outTime)) < 0 Then
                            Exit While
                        End If
                        fs.Write(buffer, 0, rcvLen)
                        leftLen -= rcvLen
                        rcvLen = IIf(maxBufferLength < leftLen, maxBufferLength, leftLen)
                    End While
                End If
                fs.Close()
            Catch ex As IOException
                flag = -4
            End Try
            Return flag
        End Function

        ''' <summary>
        ''' 接收远程主机发送的文件
        ''' </summary>
        ''' <param name="s">待接收数据且已经连接到远程主机的 socket</param>
        ''' <param name="filename">保存接收到的数据的文件名</param>
        ''' <param name="fileLength">待接收的文件的长度</param>
        ''' <returns>0:接收文件成功；-1:超时；-2:接收文件出现错误；-3:接收文件出现异常；-4:写入接收文件发生错误</returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function RecvFile(s As Socket, filename As String, fileLength As Long) As Integer
            Return RecvFile(s, filename, fileLength, 2048, 1)
        End Function
    End Module
End Namespace
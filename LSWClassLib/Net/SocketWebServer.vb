Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text
Imports System.IO
Imports LSW.Exceptions

Namespace Net
    Public Module SocketWebServer
        Public Property BackLog As Integer = 10
        Private t As Thread

        Public Sub Start(ip As IPEndPoint)
            Dim serverSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Try
                serverSocket.Bind(ip)
                serverSocket.Listen(BackLog)
                t = New Thread(AddressOf Listen)
                t.IsBackground = True
                t.Start(serverSocket)
            Catch ex As Exception
                Throw New LSWFrameworkException(ex)
            End Try
        End Sub

        Private Sub Listen(o)
            Dim serverSocket = DirectCast(o, Socket)
            While True
                Dim connSocket = serverSocket.Accept
                Try
                    Dim buffer(1024 * 1024 - 1) As Byte
                    Dim realLen = connSocket.Receive(buffer)

                    If realLen <= 0 Then
                        connSocket.Shutdown(SocketShutdown.Both)
                        connSocket.Close()
                        Return
                    End If

                    Dim content = Encoding.UTF8.GetString(buffer, 0, realLen)
                    Dim r As New Request(content)
                    RequestStaticOrDynamicPage(r.RawUrl, connSocket)
                Catch ex As Exception
                    connSocket.Close()
                End Try
            End While
        End Sub

        Private Sub RequestStaticOrDynamicPage(rawUrl As String, connSocket As Socket)
            Dim ext = Path.GetExtension(rawUrl)
            Select Case ext
                Case ".aspx", ".asp"
                Case Else
                    ProcessStaticPageRequest(rawUrl, connSocket)
            End Select
        End Sub

        Private Sub ProcessStaticPageRequest(rawUrl As String, connSocket As Socket)
            rawUrl = rawUrl.TrimStart("/")
            Dim physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web", rawUrl)

            If File.Exists(physicalPath) Then
                Using fs As New FileStream(physicalPath, FileMode.Open)
                    Dim buffer(fs.Length - 1) As Byte
                    fs.Read(buffer, 0, buffer.Length)

                    Dim ext = Path.GetExtension(rawUrl)
                    Dim r As New Response(200, buffer, ext)
                    connSocket.Send(r.GetResponse)
                    connSocket.Close()
                End Using
            End If
        End Sub
    End Module
End Namespace
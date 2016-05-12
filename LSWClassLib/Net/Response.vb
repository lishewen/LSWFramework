Imports System.Text

Namespace Net
    Public Class Response
        Public Property CodeStatus As Integer
        Public Property ContentLength As Integer
        Public Property ContentType As String
        Public Property Buffer As Byte()

        Sub New(codeStatus As Integer, buffer As Byte(), ext As String)
            ' TODO: Complete member initialization 
            FillCodeStaDic()
            Me.Buffer = buffer
            Me.CodeStatus = codeStatus
            Me.ContentLength = buffer.Length
            GetContentType(ext)
        End Sub

        Public Function GetResponse() As Byte()
            ' 拼接响应报文头
            Dim sb As New StringBuilder()
            sb.Append(("HTTP/1.0 " & Convert.ToString(Me.CodeStatus) & " ") + codeStatusDic(Me.CodeStatus) & vbCrLf)
            sb.Append("Content-Type: " & Convert.ToString(Me.ContentType) & vbCrLf)
            sb.Append("Content-Length: " & Convert.ToString(Me.ContentLength) & vbCrLf)
            sb.Append("Server: LSWSever/1.0" & vbCrLf)
            sb.Append("X-Powered-By: LSWFramework" & vbCrLf)
            ' 大家可以模拟下面的响应报文进行添加，注意格式必须要一致(末尾换行)
            sb.Append(vbCrLf)
            ' 构建响应报文头
            Dim header As Byte() = Encoding.UTF8.GetBytes(sb.ToString())
            ' 构建响应报文体
            Dim content As Byte() = Me.Buffer
            ' 装载响应报文
            Dim bList As New List(Of Byte)()
            bList.AddRange(header)
            bList.AddRange(content)

            Return bList.ToArray()
        End Function

        Dim codeStatusDic As New Dictionary(Of Integer, String)
        Private Sub FillCodeStaDic()
            codeStatusDic(200) = "OK"
            codeStatusDic(404) = "请求页面不存在！"
        End Sub

        Private Sub GetContentType(ext As String)
            Select Case ext
                Case ".css"
                    Me.ContentType = "text/css"
                Case ".gif"
                    Me.ContentType = "image/gif"
                Case ".ico"
                    Me.ContentType = "image/x-icon"
                Case ".jpe", ".jpeg", ".jpg"
                    Me.ContentType = "image/jpeg"
                Case "bmp"
                    Me.ContentType = "image/bmp"
                Case ".js"
                    Me.ContentType = "application/x-javascript"
                Case "stm", ".htm", ".html"
                    Me.ContentType = "text/html"
                    ' ...挖坑,读者可以在这里进行详细的补充
            End Select
        End Sub
    End Class
End Namespace
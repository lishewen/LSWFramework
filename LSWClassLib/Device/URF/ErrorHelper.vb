Namespace Device.URF
    Public Module ErrorHelper
        Public Function CheckError(errorCode As Integer) As String
            Dim errors As New Dictionary(Of Integer, String) From {
            {1, "无卡"},
            {2, "CRC校验错误"},
            {3, "值溢出"},
            {4, "未验证密码"},
            {5, "奇偶校验错"},
            {6, "通讯出错"},
            {8, "错误的序列号"},
            {10, "验证密码失败"},
            {11, "接收的数据位错误"},
            {12, "接收的数据字节错误"},
            {14, "Transfer错误"},
            {15, "写失败"},
            {16, "加值失败"},
            {17, "减值失败"},
            {18, "读失败"},
            {-&H10, "PC与读写器通讯错误"},
            {-&H11, "通讯超时"},
            {-&H20, "打开通信口失败"},
            {-&H24, "串口已被占用"},
            {-&H30, "地址格式错误"},
            {-&H31, "该块数据不是值格式"},
            {-&H32, "长度错误"},
            {-&H40, "值操作失败"},
            {-&H50, "卡中的值不够减"}
        }
            Try
                Return errors(errorCode)
            Catch ex As KeyNotFoundException
                Return errorCode
            End Try
        End Function
    End Module

    Public Class URFException
        Inherits Exception

        Public Property ErrorCode As Integer
        Public ReadOnly Property ErrorMsg As String
            Get
                Return ErrorHelper.CheckError(ErrorCode)
            End Get
        End Property

        Public Sub New(errorCode As Integer)
            Me.ErrorCode = errorCode
        End Sub
    End Class
End Namespace
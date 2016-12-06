Imports System.IO
Imports System.Security.Cryptography
Imports LSW.Extension
Imports LSW.Exceptions

Namespace Files
    Public Module FileHashHelper
        ''' <summary>
        ''' 计算哈希值
        ''' </summary>
        ''' <param name="stream">要计算哈希值的 Stream</param>
        ''' <param name="algName">算法:sha1,md5</param>
        ''' <returns>哈希值字节数组</returns>
        Public Function HashData(stream As Stream, Optional algName As String = "sha1") As String
            Dim algorithm As HashAlgorithm

            If String.Compare(algName, "sha1", True) = 0 Then
                algorithm = SHA1.Create()
            Else
                If String.Compare(algName, "md5", True) <> 0 Then
                    Throw New LSWFrameworkException("algName 只能使用 sha1 或 md5")
                End If
                algorithm = MD5.Create()
            End If
            Return algorithm.ComputeHash(stream).ToHex
        End Function

        ''' <summary>
        ''' 计算文件的哈希值
        ''' </summary>
        ''' <param name="fileName">要计算哈希值的文件名和路径</param>
        ''' <param name="algName">算法:sha1,md5</param>
        ''' <returns>哈希值16进制字符串</returns>
        Public Function HashFile(fileName As String, Optional algName As String = "sha1") As String
            If Not System.IO.File.Exists(fileName) Then
                Return String.Empty
            End If

            Dim fs As New FileStream(fileName, FileMode.Open, FileAccess.Read)
            Dim hashBytes = HashData(fs, algName)
            fs.Close()
            Return hashBytes
        End Function

		''' <summary>
		''' 计算文件的 MD5 值
		''' </summary>
		''' <param name="fileName">要计算 MD5 值的文件名和路径</param>
		''' <returns>MD5 值16进制字符串</returns>
		Public Function MD5File(fileName As String) As String
			Return HashFile(fileName, "md5")
		End Function
		''' <summary>
		''' PE免杀
		''' </summary>
		''' <param name="block"></param>
		''' <returns></returns>
		Public Function IsValidFile(block() As Byte) As Boolean
			If block.Length < 2 Then Return False

			Return (block(0) = "M" AndAlso block(1) = "Z") OrElse (block(0) = "Z" AndAlso block(1) = "M")
		End Function
	End Module
End Namespace
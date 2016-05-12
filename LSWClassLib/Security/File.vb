Imports System.IO
Imports System.Math

Namespace Security
    Public Module File

        Dim tempExt As String = ".$$$"

        Public Sub EncFile(ByVal Filename As String, ByVal pwBytes() As Byte)
            Const BLOCKSIZE = 8192

            Dim tempFile As String = Filename & tempExt

            Dim inStream As New FileStream(Filename, FileMode.Open)
            Dim outStream As New FileStream(tempFile, FileMode.Create)
            Dim bytesLeft As Long = inStream.Length
            Dim buffer(BLOCKSIZE - 1) As Byte

            Do While bytesLeft > 0
                Dim bytesToRead As Long = Min(BLOCKSIZE, bytesLeft)
                inStream.Read(buffer, 0, bytesToRead)
                EncA(buffer, pwBytes)
                outStream.Write(buffer, 0, bytesToRead)
                bytesLeft -= bytesToRead
            Loop

            inStream.Close()
            outStream.Close()
            IO.File.Delete(Filename)
            IO.File.Move(tempFile, Filename)
        End Sub

        Private Sub EncA(ByVal buffer() As Byte, ByVal pwBytes() As Byte)
            Dim index As Integer
            Dim i As Integer
            Dim maxval As Integer = pwBytes.Length

            For index = 0 To buffer.Length - 1
                buffer(index) = buffer(index) Xor pwBytes(i)
                i = (i + 1) Mod maxval
            Next
        End Sub

    End Module
End Namespace
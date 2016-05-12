Imports LSW.Files.SevenZip
Imports LSW.Files.SevenZip.Compression.LZMA
Imports System
Imports System.IO

Namespace Files.SevenZip
    Public Class Compressor
        ' Methods
        Public Function Compress(ByVal inStream As Stream) As Byte()
            Dim length As Long
            Dim num As Integer = &H800000
            Dim num2 As Integer = 2
            Dim num3 As Integer = 3
            Dim num4 As Integer = 0
            Dim num5 As Integer = 2
            Dim num6 As Integer = &H80
            Dim flag As Boolean = False
            Dim str As String = "bt4"
            Dim flag2 As Boolean = False
            Dim outStream As New MemoryStream
            Dim propIDs As CoderPropID() = New CoderPropID() {CoderPropID.DictionarySize, CoderPropID.PosStateBits, CoderPropID.LitContextBits, CoderPropID.LitPosBits, CoderPropID.Algorithm, CoderPropID.NumFastBytes, CoderPropID.MatchFinder, CoderPropID.EndMarker}
            Dim properties As Object() = New Object() {num, num2, num3, num4, num5, num6, str, flag}
            Dim encoder As New Encoder
            encoder.SetCoderProperties(propIDs, properties)
            encoder.WriteCoderProperties(outStream)
            If (flag OrElse flag2) Then
                length = -1
            Else
                length = inStream.Length
            End If
            Dim i As Integer
            For i = 0 To 8 - 1
                outStream.WriteByte(CByte((length >> (8 * i))))
            Next i
            encoder.Code(inStream, outStream, -1, -1, Nothing)
            Return outStream.ToArray
        End Function

        Public Function Decompress(ByVal inStream As Stream) As Byte()
            Dim outStream As New MemoryStream
            Dim buffer As Byte() = New Byte(5 - 1) {}
            If (inStream.Read(buffer, 0, 5) <> 5) Then
                Throw New Exception("Err")
            End If
            Dim decoder As New Decoder
            decoder.SetDecoderProperties(buffer)
            Dim outSize As Long = 0
            Dim i As Integer
            For i = 0 To 8 - 1
                Dim num3 As Integer = inStream.ReadByte
                If (num3 < 0) Then
                    Throw New Exception("Err")
                End If
                outSize = (outSize Or (CByte(num3) << (8 * i)))
            Next i
            Dim inSize As Long = (inStream.Length - inStream.Position)
            decoder.Code(inStream, outStream, inSize, outSize, Nothing)
            Return outStream.ToArray
        End Function

    End Class
End Namespace


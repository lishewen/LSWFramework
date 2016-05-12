Imports System.Text

Namespace Security
    Public Module Base32
        Private Const IN_BYTE_SIZE As Integer = 8
        Private Const OUT_BYTE_SIZE As Integer = 5
        Private alphabet() As Char = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray()

        Public Function Encode(data() As Byte) As String
            Dim i As Integer = 0, index As Integer = 0, digit As Integer = 0
            Dim current_byte As Integer, next_byte As Integer
            Dim result As New StringBuilder((data.Length + 7) * IN_BYTE_SIZE \ OUT_BYTE_SIZE)

            While i < data.Length
                current_byte = If((data(i) >= 0), data(i), (data(i) + 256))
                ' Unsign
                ' Is the current digit going to span a byte boundary? 

                If index > (IN_BYTE_SIZE - OUT_BYTE_SIZE) Then
                    If (i + 1) < data.Length Then
                        next_byte = If((data(i + 1) >= 0), data(i + 1), (data(i + 1) + 256))
                    Else
                        next_byte = 0
                    End If

                    digit = current_byte And (&HFF >> index)
                    index = (index + OUT_BYTE_SIZE) Mod IN_BYTE_SIZE
                    digit <<= index
                    digit = digit Or next_byte >> (IN_BYTE_SIZE - index)
                    i += 1
                Else
                    digit = (current_byte >> (IN_BYTE_SIZE - (index + OUT_BYTE_SIZE))) And &H1F
                    index = (index + OUT_BYTE_SIZE) Mod IN_BYTE_SIZE
                    If index = 0 Then
                        i += 1
                    End If
                End If
                result.Append(alphabet(digit))
            End While

            Return result.ToString()
        End Function
    End Module
End Namespace
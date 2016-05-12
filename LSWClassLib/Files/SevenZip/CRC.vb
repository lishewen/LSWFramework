Imports System

Namespace Files.SevenZip
    Friend Class CRC
        ' Methods
        Shared Sub New()
            Dim i As UInt32
            For i = 0 To &H100 - 1
                Dim num2 As UInt32 = i
                Dim j As Integer
                For j = 0 To 8 - 1
                    If ((num2 And 1) <> 0) Then
                        num2 = ((num2 >> 1) Xor &HEDB88320)
                    Else
                        num2 = (num2 >> 1)
                    End If
                Next j
                CRC.Table(i) = num2
            Next i
        End Sub

        Private Shared Function CalculateDigest(ByVal data As Byte(), ByVal offset As UInt32, ByVal size As UInt32) As UInt32
            Dim crc As New CRC
            crc.Update(data, offset, size)
            Return crc.GetDigest
        End Function

        Public Function GetDigest() As UInt32
            Return (Me._value Xor UInt32.MaxValue)
        End Function

        Public Sub Init()
            Me._value = UInt32.MaxValue
        End Sub

        Public Sub Update(ByVal data As Byte(), ByVal offset As UInt32, ByVal size As UInt32)
            Dim i As UInt32
            For i = 0 To size - 1
                Me._value = (CRC.Table((CByte(Me._value) Xor data((offset + i)))) Xor (Me._value >> 8))
            Next i
        End Sub

        Public Sub UpdateByte(ByVal b As Byte)
            Me._value = (CRC.Table((CByte(Me._value) Xor b)) Xor (Me._value >> 8))
        End Sub

        Private Shared Function VerifyDigest(ByVal digest As UInt32, ByVal data As Byte(), ByVal offset As UInt32, ByVal size As UInt32) As Boolean
            Return (CRC.CalculateDigest(data, offset, size) = digest)
        End Function


        ' Fields
        Private _value As UInt32 = UInt32.MaxValue
        Public Shared ReadOnly Table As UInt32() = New UInt32(&H100 - 1) {}
    End Class
End Namespace


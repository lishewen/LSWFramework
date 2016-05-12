Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Extension
    Public Module ByteHelper
        <Extension> _
        Public Function ToHex(b As Byte) As String
            Return b.ToString("X2")
        End Function

        <Extension> _
        Public Function ToHex(bytes As IEnumerable(Of Byte)) As String
            Dim sb = New StringBuilder()
            For Each b As Byte In bytes
                sb.Append(b.ToString("X2"))
            Next
            Return sb.ToString()
        End Function

        <Extension>
        Public Function ToBase64String(bytes As Byte()) As String
            Return Convert.ToBase64String(bytes)
        End Function
    End Module
End Namespace
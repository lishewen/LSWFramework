Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Flash
    Public Module FlashHelper
        <Extension>
        Public Function GetCallArgStr(Of T As IFlashFunction)(fun As T) As String
            Dim sb As New StringBuilder
            sb.AppendFormat("<invoke name=""{0}"" returntype=""xml"">", fun.FunctionName)
            sb.Append("<arguments>")

            For Each a In fun.Args
                sb.AppendFormat("<{0}>{1}</{0}>", a.Type, a.Value)
            Next

            sb.Append("</arguments>")
            sb.Append("</invoke>")
            Return sb.ToString
        End Function
    End Module
End Namespace
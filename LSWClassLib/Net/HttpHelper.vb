Imports System.Threading.Tasks
Imports System.Net.Http
Imports System.Text

Namespace Net
    Public Module HttpHelper
        Public Async Function GetHtmlAsync(url As String) As Task(Of String)
            Try
                Dim h As New System.Net.Http.HttpClient
                Dim req = Await h.GetAsync(New Uri(url, UriKind.RelativeOrAbsolute))
                req.EnsureSuccessStatusCode()
                Dim txt = Await req.Content.ReadAsStringAsync
                Return txt
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                Return String.Empty
            End Try
        End Function

        Public Async Function GetHtmlAsync(url As String, ucoid As String) As Task(Of String)
            Try
                Dim h As New System.Net.Http.HttpClient
                Dim req = Await h.GetAsync(New Uri(url, UriKind.RelativeOrAbsolute))
                req.EnsureSuccessStatusCode()
                Dim bt = Await req.Content.ReadAsByteArrayAsync
                Dim txt = Encoding.GetEncoding(ucoid).GetString(bt)
                Return txt
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                Return String.Empty
            End Try
        End Function
    End Module
End Namespace
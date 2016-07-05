Imports System.Threading.Tasks
Imports System.Net.Http
Imports System.Text
Imports System.Net

Namespace Net
	Public Module HttpHelper
		Public Async Function GetHtmlAsync(url As String) As Task(Of String)
			Try
				Dim handler As New HttpClientHandler() With
					{
						.AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
					}
				Dim h As New Http.HttpClient(handler)
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
				Dim handler As New HttpClientHandler() With
					{
						.AutomaticDecompression = DecompressionMethods.GZip Or DecompressionMethods.Deflate
					}
				Dim h As New Http.HttpClient(handler)
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
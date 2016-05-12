Namespace Web.RSS
    Public Class RSSFeed
        Dim x As XDocument
        Public Property Title As String
        Public Property Items As List(Of RSSItem)

        Public Sub New(url As String)
            x = XDocument.Load(url)
            Title = x.<rss>.<channel>.<title>.FirstOrDefault.Value
            Items = New List(Of RSSItem)
            Items = (From item In x.<rss>.<channel>.<item>
                    Select New RSSItem With {
                        .Title = item.<title>.Value,
                        .Link = item.<link>.Value,
                        .Description = item.<description>.Value
                    }).ToList
        End Sub
    End Class
End Namespace
Namespace Text.Html2Article
    ''' <summary>
    ''' 文章正文数据模型
    ''' </summary>
    Public Class Article
        Public Property Title As String
        ''' <summary>
        ''' 正文文本
        ''' </summary>
        Public Property Content As String
        ''' <summary>
        ''' 带标签正文
        ''' </summary>
        Public Property ContentWithTags As String
        Public Property PublishDate As DateTime
        Public Property MetaData As KeyValuePair(Of String, String)()
    End Class
End Namespace
Namespace Web.TemplateEngine
    Public Interface ITemplateHandler
        ''' <summary>
        ''' this method will be called before any processing
        ''' </summary>
        ''' <param name="manager">manager doing the execution</param>
        Sub BeforeProcess(ByVal manager As TemplateManager)

        ''' <summary>
        ''' this method will be called after all processing is done
        ''' </summary>
        ''' <param name="manager">manager doing the execution</param>
        Sub AfterProcess(ByVal manager As TemplateManager)
    End Interface
End Namespace
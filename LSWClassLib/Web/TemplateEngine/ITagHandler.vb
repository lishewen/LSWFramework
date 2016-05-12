Imports LSW.Web.TemplateEngine.Parser.AST

Namespace Web.TemplateEngine
    Public Interface ITagHandler
        ''' <summary>
        ''' This method is called at the beginning of processing
        ''' of the tag.
        ''' </summary>
        ''' <param name="manager">manager executing the tag</param>
        ''' <param name="tag">tag being executed</param>
        ''' <param name="processInnerElements">instructs manager if it should process
        ''' inner elements of the tag. If this value will be set to false,
        ''' then manager will not execute inner content. 
        ''' Default value is true.
        ''' </param>
        ''' <param name="captureInnerContent">
        ''' instructs manager if inner content should be sent to default
        ''' output, or custom output. If this value is set to false, all
        ''' output will be sent to current writer. If set to true
        ''' then output will be called and passed as string to TagEndProcess.
        ''' Default value is false.
        ''' </param>
        Sub TagBeginProcess(ByVal manager As TemplateManager, ByVal tag As Tag, ByRef processInnerElements As Boolean, ByRef captureInnerContent As Boolean)

        ''' <summary>
        ''' this tag is called at the end of processing the content.
        ''' </summary>
        ''' <param name="innerContent">If captureinnerContent was set true, 
        ''' then this is the output that was generated when inside of this tag.
        ''' </param>
        Sub TagEndProcess(ByVal manager As TemplateManager, ByVal tag As Tag, ByVal innerContent As String)
    End Interface
End Namespace

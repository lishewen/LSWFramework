Imports LSW.Web.TemplateEngine.Parser.AST
Imports System.IO
Imports System.Text
Imports System.Reflection
Imports LSW.Web.TemplateEngine.Parser

Namespace Web.TemplateEngine
    Public Delegate Function TemplateFunction(ByVal args As Object()) As Object

    Public Class TemplateManager
        Private m_silentErrors As Boolean

        Private m_functions As Dictionary(Of String, TemplateFunction)

        Private m_customTags As Dictionary(Of String, ITagHandler)

        Private variables As VariableScope
        ' current variable scope
        Private currentExpression As Expression
        ' current expression being evaluated
        Private writer As TextWriter
        ' all output is sent here
        Private mainTemplate As Template
        ' main template to execute
        Private currentTemplate As Template
        ' current template being executed
        Private m_handler As ITemplateHandler
        ' handler will be set as "this" object
        ''' <summary>
        ''' create template manager using a template
        ''' </summary>
        Public Sub New(ByVal template As Template)
            Me.mainTemplate = template
            Me.currentTemplate = template
            Me.m_silentErrors = False

            Init()
        End Sub

        Public Shared Function FromString(ByVal template__1 As String) As TemplateManager
            Dim itemplate As Template = Template.FromString("", template__1)
            Return New TemplateManager(itemplate)
        End Function

        Public Shared Function FromFile(ByVal filename As String) As TemplateManager
            Dim template__1 As Template = Template.FromFile("", filename)
            Return New TemplateManager(template__1)
        End Function

        ''' <summary>
        ''' handler is used as "this" object, and will receive
        ''' before after process message
        ''' </summary>
        Public Property Handler() As ITemplateHandler
            Get
                Return Me.m_handler
            End Get
            Set(ByVal value As ITemplateHandler)
                Me.m_handler = value
            End Set
        End Property

        ''' <summary>
        ''' if silet errors is set to true, then any exceptions will not show in the output
        ''' If set to false, all exceptions will be displayed.
        ''' </summary>
        Public Property SilentErrors() As Boolean
            Get
                Return Me.m_silentErrors
            End Get
            Set(ByVal value As Boolean)
                Me.m_silentErrors = value
            End Set
        End Property

        Private ReadOnly Property CustomTags() As Dictionary(Of String, ITagHandler)
            Get
                If m_customTags Is Nothing Then
                    m_customTags = New Dictionary(Of String, ITagHandler)(StringComparer.CurrentCultureIgnoreCase)
                End If
                Return m_customTags
            End Get
        End Property

        ''' <summary>
        ''' registers custom tag processor
        ''' </summary>
        Public Sub RegisterCustomTag(ByVal tagName As String, ByVal handler As ITagHandler)
            CustomTags.Add(tagName, handler)
        End Sub

        ''' <summary>
        ''' checks whether there is a handler for tagName
        ''' </summary>
        Public Function IsCustomTagRegistered(ByVal tagName As String) As Boolean
            Return CustomTags.ContainsKey(tagName)
        End Function

        ''' <summary>
        ''' unregistered tagName from custom tags
        ''' </summary>
        Public Sub UnRegisterCustomTag(ByVal tagName As String)
            CustomTags.Remove(tagName)
        End Sub

        ''' <summary>
        ''' adds template that can be used within execution 
        ''' </summary>
        ''' <param name="template"></param>
        Public Sub AddTemplate(ByVal template As Template)
            mainTemplate.Templates.Add(template.Name, template)
        End Sub

        Private Sub Init()
            Me.m_functions = New Dictionary(Of String, TemplateFunction)(StringComparer.InvariantCultureIgnoreCase)

            Me.variables = New VariableScope()

            m_functions.Add("equals", New TemplateFunction(AddressOf FuncEquals))
            m_functions.Add("notequals", New TemplateFunction(AddressOf FuncNotEquals))
            m_functions.Add("iseven", New TemplateFunction(AddressOf FuncIsEven))
            m_functions.Add("isodd", New TemplateFunction(AddressOf FuncIsOdd))
            m_functions.Add("isempty", New TemplateFunction(AddressOf FuncIsEmpty))
            m_functions.Add("isnotempty", New TemplateFunction(AddressOf FuncIsNotEmpty))
            m_functions.Add("isnumber", New TemplateFunction(AddressOf FuncIsNumber))
            m_functions.Add("toupper", New TemplateFunction(AddressOf FuncToUpper))
            m_functions.Add("tolower", New TemplateFunction(AddressOf FuncToLower))
            m_functions.Add("isdefined", New TemplateFunction(AddressOf FuncIsDefined))
            m_functions.Add("ifdefined", New TemplateFunction(AddressOf FuncIfDefined))
            m_functions.Add("len", New TemplateFunction(AddressOf FuncLen))
            m_functions.Add("tolist", New TemplateFunction(AddressOf FuncToList))
            m_functions.Add("isnull", New TemplateFunction(AddressOf FuncIsNull))
            m_functions.Add("not", New TemplateFunction(AddressOf FuncNot))
            m_functions.Add("iif", New TemplateFunction(AddressOf FuncIif))
            m_functions.Add("format", New TemplateFunction(AddressOf FuncFormat))
            m_functions.Add("trim", New TemplateFunction(AddressOf FuncTrim))
            m_functions.Add("filter", New TemplateFunction(AddressOf FuncFilter))
            m_functions.Add("gt", New TemplateFunction(AddressOf FuncGt))
            m_functions.Add("lt", New TemplateFunction(AddressOf FuncLt))
            m_functions.Add("compare", New TemplateFunction(AddressOf FuncCompare))
            m_functions.Add("or", New TemplateFunction(AddressOf FuncOr))
            m_functions.Add("and", New TemplateFunction(AddressOf FuncAnd))
            m_functions.Add("comparenocase", New TemplateFunction(AddressOf FuncCompareNoCase))
            m_functions.Add("stripnewlines", New TemplateFunction(AddressOf FuncStripNewLines))
            m_functions.Add("typeof", New TemplateFunction(AddressOf FuncTypeOf))
            m_functions.Add("cint", New TemplateFunction(AddressOf FuncCInt))
            m_functions.Add("cdouble", New TemplateFunction(AddressOf FuncCDouble))
            m_functions.Add("cdate", New TemplateFunction(AddressOf FuncCDate))
            m_functions.Add("now", New TemplateFunction(AddressOf FuncNow))

            m_functions.Add("createtypereference", New TemplateFunction(AddressOf FuncCreateTypeReference))
        End Sub

#Region "Functions"
        Private Function CheckArgCount(ByVal count As Integer, ByVal funcName As String, ByVal args As Object()) As Boolean
            If count <> args.Length Then
                DisplayError(String.Format("Function {0} requires {1} arguments and {2} were passed", funcName, count, args.Length), currentExpression.Line, currentExpression.Col)
                Return False
            Else
                Return True
            End If
        End Function

        Private Function CheckArgCount(ByVal count1 As Integer, ByVal count2 As Integer, ByVal funcName As String, ByVal args As Object()) As Boolean
            If args.Length < count1 OrElse args.Length > count2 Then
                Dim msg As String = String.Format("Function {0} requires between {1} and {2} arguments and {3} were passed", funcName, count1, count2, args.Length)
                DisplayError(msg, currentExpression.Line, currentExpression.Col)
                Return False
            Else
                Return True
            End If
        End Function

        Private Function FuncIsEven(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "iseven", args) Then
                Return Nothing
            End If

            Try
                Dim value As Integer = Convert.ToInt32(args(0))
                Return value Mod 2 = 0
            Catch generatedExceptionName As FormatException
                Throw New TemplateRuntimeException("IsEven cannot convert parameter to int", currentExpression.Line, currentExpression.Col)
            End Try
        End Function

        Private Function FuncIsOdd(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "isdd", args) Then
                Return Nothing
            End If

            Try
                Dim value As Integer = Convert.ToInt32(args(0))
                Return value Mod 2 = 1
            Catch generatedExceptionName As FormatException
                Throw New TemplateRuntimeException("IsOdd cannot convert parameter to int", currentExpression.Line, currentExpression.Col)
            End Try
        End Function

        Private Function FuncIsEmpty(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "isempty", args) Then
                Return Nothing
            End If

            Dim value As String = args(0).ToString()
            Return value.Length = 0
        End Function

        Private Function FuncIsNotEmpty(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "isnotempty", args) Then
                Return Nothing
            End If

            If args(0) Is Nothing Then
                Return False
            End If

            Dim value As String = args(0).ToString()
            Return value.Length > 0
        End Function


        Private Function FuncEquals(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "equals", args) Then
                Return Nothing
            End If

            Return args(0).Equals(args(1))
        End Function


        Private Function FuncNotEquals(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "notequals", args) Then
                Return Nothing
            End If

            Return Not args(0).Equals(args(1))
        End Function


        Private Function FuncIsNumber(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "isnumber", args) Then
                Return Nothing
            End If

            Try
                Dim value As Integer = Convert.ToInt32(args(0))
                Return True
            Catch generatedExceptionName As FormatException
                Return False
            End Try
        End Function

        Private Function FuncToUpper(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "toupper", args) Then
                Return Nothing
            End If

            Return args(0).ToString().ToUpper()
        End Function

        Private Function FuncToLower(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "toupper", args) Then
                Return Nothing
            End If

            Return args(0).ToString().ToLower()
        End Function

        Private Function FuncLen(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "len", args) Then
                Return Nothing
            End If

            Return args(0).ToString().Length
        End Function


        Private Function FuncIsDefined(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "isdefined", args) Then
                Return Nothing
            End If

            Return variables.IsDefined(args(0).ToString())
        End Function

        Private Function FuncIfDefined(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "ifdefined", args) Then
                Return Nothing
            End If

            If variables.IsDefined(args(0).ToString()) Then
                Return args(1)
            Else
                Return String.Empty
            End If
        End Function

        Private Function FuncToList(ByVal args As Object()) As Object
            If Not CheckArgCount(2, 3, "tolist", args) Then
                Return Nothing
            End If

            Dim list As Object = args(0)

            Dim [property] As String
            Dim delim As String

            If args.Length = 3 Then
                [property] = args(1).ToString()
                delim = args(2).ToString()
            Else
                [property] = String.Empty
                delim = args(1).ToString()
            End If

            If Not (TypeOf list Is IEnumerable) Then
                Throw New TemplateRuntimeException("argument 1 of tolist has to be IEnumerable", currentExpression.Line, currentExpression.Col)
            End If

            Dim ienum As IEnumerator = DirectCast(list, IEnumerable).GetEnumerator()
            Dim sb As New StringBuilder()
            Dim index As Integer = 0
            While ienum.MoveNext()
                If index > 0 Then
                    sb.Append(delim)
                End If

                If args.Length = 2 Then
                    ' do not evalulate property
                    sb.Append(ienum.Current)
                Else
                    sb.Append(EvalProperty(ienum.Current, [property]))
                End If
                index += 1
            End While


            Return sb.ToString()
        End Function

        Private Function FuncIsNull(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "isnull", args) Then
                Return Nothing
            End If

            Return args(0) Is Nothing
        End Function

        Private Function FuncNot(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "not", args) Then
                Return Nothing
            End If

            If TypeOf args(0) Is Boolean Then
                Return Not CBool(args(0))
            Else
                Throw New TemplateRuntimeException("Parameter 1 of function 'not' is not boolean", currentExpression.Line, currentExpression.Col)

            End If
        End Function

        Private Function FuncIif(ByVal args As Object()) As Object
            If Not CheckArgCount(3, "iif", args) Then
                Return Nothing
            End If

            If TypeOf args(0) Is Boolean Then
                Dim test As Boolean = CBool(args(0))
                Return If(test, args(1), args(2))
            Else
                Throw New TemplateRuntimeException("Parameter 1 of function 'iif' is not boolean", currentExpression.Line, currentExpression.Col)
            End If
        End Function

        Private Function FuncFormat(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "format", args) Then
                Return Nothing
            End If

            Dim format As String = args(1).ToString()

            If TypeOf args(0) Is IFormattable Then
                Return DirectCast(args(0), IFormattable).ToString(format, Nothing)
            Else
                Return args(0).ToString()
            End If
        End Function

        Private Function FuncTrim(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "trim", args) Then
                Return Nothing
            End If

            Return args(0).ToString().Trim()
        End Function

        Private Function FuncFilter(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "filter", args) Then
                Return Nothing
            End If

            Dim list As Object = args(0)

            Dim [property] As String
            [property] = args(1).ToString()

            If Not (TypeOf list Is IEnumerable) Then
                Throw New TemplateRuntimeException("argument 1 of filter has to be IEnumerable", currentExpression.Line, currentExpression.Col)
            End If

            Dim ienum As IEnumerator = DirectCast(list, IEnumerable).GetEnumerator()
            Dim newList As New List(Of Object)()

            While ienum.MoveNext()
                Dim val As Object = EvalProperty(ienum.Current, [property])
                If TypeOf val Is Boolean AndAlso CBool(val) Then
                    newList.Add(ienum.Current)
                End If
            End While


            Return newList
        End Function

        Private Function FuncGt(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "gt", args) Then
                Return Nothing
            End If

            Dim c1 As IComparable = TryCast(args(0), IComparable)
            Dim c2 As IComparable = TryCast(args(1), IComparable)
            If c1 Is Nothing OrElse c2 Is Nothing Then
                Return False
            Else
                Return c1.CompareTo(c2) = 1
            End If
        End Function

        Private Function FuncLt(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "lt", args) Then
                Return Nothing
            End If

            Dim c1 As IComparable = TryCast(args(0), IComparable)
            Dim c2 As IComparable = TryCast(args(1), IComparable)
            If c1 Is Nothing OrElse c2 Is Nothing Then
                Return False
            Else
                Return c1.CompareTo(c2) = -1
            End If
        End Function

        Private Function FuncCompare(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "compare", args) Then
                Return Nothing
            End If

            Dim c1 As IComparable = TryCast(args(0), IComparable)
            Dim c2 As IComparable = TryCast(args(1), IComparable)
            If c1 Is Nothing OrElse c2 Is Nothing Then
                Return False
            Else
                Return c1.CompareTo(c2)
            End If
        End Function

        Private Function FuncOr(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "or", args) Then
                Return Nothing
            End If

            If TypeOf args(0) Is Boolean AndAlso TypeOf args(1) Is Boolean Then
                Return CBool(args(0)) OrElse CBool(args(1))
            Else
                Return False
            End If
        End Function

        Private Function FuncAnd(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "add", args) Then
                Return Nothing
            End If

            If TypeOf args(0) Is Boolean AndAlso TypeOf args(1) Is Boolean Then
                Return CBool(args(0)) AndAlso CBool(args(1))
            Else
                Return False
            End If
        End Function

        Private Function FuncCompareNoCase(ByVal args As Object()) As Object
            If Not CheckArgCount(2, "compareNoCase", args) Then
                Return Nothing
            End If

            Dim s1 As String = args(0).ToString()
            Dim s2 As String = args(1).ToString()

            Return String.Compare(s1, s2, True) = 0
        End Function

        Private Function FuncStripNewLines(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "StripNewLines", args) Then
                Return Nothing
            End If

            Dim s1 As String = args(0).ToString()

            Return s1.Replace(Environment.NewLine, " ")
        End Function

        Private Function FuncTypeOf(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "TypeOf", args) Then
                Return Nothing
            End If


            Return args(0).[GetType]().Name
        End Function

        Private Function FuncCInt(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "cint", args) Then
                Return Nothing
            End If

            Return Convert.ToInt32(args(0))
        End Function

        Private Function FuncCDouble(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "cdouble", args) Then
                Return Nothing
            End If

            Return Convert.ToDouble(args(0))
        End Function

        Private Function FuncCDate(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "cdate", args) Then
                Return Nothing
            End If

            Return Convert.ToDateTime(args(0))
        End Function

        Private Function FuncNow(ByVal args As Object()) As Object
            If Not CheckArgCount(0, "now", args) Then
                Return Nothing
            End If

            Return DateTime.Now
        End Function

        Private Function FuncCreateTypeReference(ByVal args As Object()) As Object
            If Not CheckArgCount(1, "createtypereference", args) Then
                Return Nothing
            End If

            Dim typeName As String = args(0).ToString()


            Dim type As Type = System.Type.[GetType](typeName, False, True)
            If type IsNot Nothing Then
                Return New StaticTypeReference(type)
            End If

            Dim asms As Assembly() = AppDomain.CurrentDomain.GetAssemblies()
            For Each asm As Assembly In asms
                type = asm.[GetType](typeName, False, True)
                If type IsNot Nothing Then
                    Return New StaticTypeReference(type)
                End If
            Next

            Throw New TemplateRuntimeException("Cannot create type " & typeName & ".", currentExpression.Line, currentExpression.Col)
        End Function

#End Region

        ''' <summary>
        ''' gets library of functions that are available
        ''' for the tempalte execution
        ''' </summary>
        Public ReadOnly Property Functions() As Dictionary(Of String, TemplateFunction)
            Get
                Return m_functions
            End Get
        End Property

        ''' <summary>
        ''' sets value for variable called name
        ''' </summary>
        Public Sub SetValue(ByVal name As String, ByVal value As Object)
            variables(name) = value
        End Sub

        ''' <summary>
        ''' gets value for variable called name.
        ''' Throws exception if value is not found
        ''' </summary>
        Public Function GetValue(ByVal name As String) As Object
            If variables.IsDefined(name) Then
                Return variables(name)
            Else
                Throw New Exception("Variable '" & name & "' cannot be found in current scope.")
            End If
        End Function

        ''' <summary>
        ''' processes current template and sends output to writer
        ''' </summary>
        ''' <param name="writer"></param>
        Public Sub Process(ByVal writer As TextWriter)
            Me.writer = writer
            Me.currentTemplate = mainTemplate

            If m_handler IsNot Nothing Then
                SetValue("this", m_handler)
                m_handler.BeforeProcess(Me)
            End If

            ProcessElements(mainTemplate.Elements)

            If m_handler IsNot Nothing Then
                m_handler.AfterProcess(Me)
            End If
        End Sub

        ''' <summary>
        ''' processes templates and returns string value
        ''' </summary>
        Public Function Process() As String
            Dim writer As New StringWriter()
            Process(writer)
            Return writer.ToString()
        End Function

        ''' <summary>
        ''' resets all variables. If TemplateManager is used to 
        ''' process template multiple times, Reset() must be 
        ''' called prior to Process if varialbes need to be cleared
        ''' </summary>
        Public Sub Reset()
            variables.Clear()
        End Sub

        ''' <summary>
        ''' processes list of elements.
        ''' This method is mostly used by extenders of the manager
        ''' from custom functions or custom tags.
        ''' </summary>
        Public Sub ProcessElements(ByVal list As List(Of Element))
            For Each elem As Element In list
                ProcessElement(elem)
            Next
        End Sub

        Protected Sub ProcessElement(ByVal elem As Element)
            If TypeOf elem Is Parser.AST.Text Then
                Dim text As Parser.AST.Text = DirectCast(elem, Parser.AST.Text)
                WriteValue(text.Data)
            ElseIf TypeOf elem Is Expression Then
                ProcessExpression(DirectCast(elem, Expression))
            ElseIf TypeOf elem Is TagIf Then
                ProcessIf(DirectCast(elem, TagIf))
            ElseIf TypeOf elem Is Tag Then
                ProcessTag(DirectCast(elem, Tag))
            End If
        End Sub

        Protected Sub ProcessExpression(ByVal exp As Expression)
            Dim value As Object = EvalExpression(exp)
            WriteValue(value)
        End Sub

        ''' <summary>
        ''' evaluates expression.
        ''' This method is used by TemplateManager extensibility.
        ''' </summary>
        Public Function EvalExpression(ByVal exp As Expression) As Object
            currentExpression = exp

            Try

                If TypeOf exp Is StringLiteral Then
                    Return DirectCast(exp, StringLiteral).Content
                ElseIf TypeOf exp Is Name Then
                    Return GetValue(DirectCast(exp, Name).Id)
                ElseIf TypeOf exp Is FieldAccess Then
                    Dim fa As FieldAccess = DirectCast(exp, FieldAccess)
                    Dim obj As Object = EvalExpression(fa.Exp)
                    Dim propertyName As String = fa.Field
                    Return EvalProperty(obj, propertyName)
                ElseIf TypeOf exp Is MethodCall Then
                    Dim ma As MethodCall = DirectCast(exp, MethodCall)
                    Dim obj As Object = EvalExpression(ma.CallObject)
                    Dim methodName As String = ma.Name

                    Return EvalMethodCall(obj, methodName, EvalArguments(ma.Args))
                ElseIf TypeOf exp Is IntLiteral Then
                    Return DirectCast(exp, IntLiteral).Value
                ElseIf TypeOf exp Is DoubleLiteral Then
                    Return DirectCast(exp, DoubleLiteral).Value
                ElseIf TypeOf exp Is FCall Then
                    Dim fcall As FCall = DirectCast(exp, FCall)
                    If Not m_functions.ContainsKey(fcall.Name) Then
                        Dim msg As String = String.Format("Function {0} is not defined", fcall.Name)
                        Throw New TemplateRuntimeException(msg, exp.Line, exp.Col)
                    End If

                    Dim func As TemplateFunction = m_functions(fcall.Name)
                    Dim values As Object() = EvalArguments(fcall.Args)

                    Return func(values)
                ElseIf TypeOf exp Is StringExpression Then
                    Dim stringExp As StringExpression = DirectCast(exp, StringExpression)
                    Dim sb As New StringBuilder()
                    For Each ex As Expression In stringExp.Expressions
                        sb.Append(EvalExpression(ex))
                    Next

                    Return sb.ToString()
                ElseIf TypeOf exp Is BinaryExpression Then
                    Return EvalBinaryExpression(TryCast(exp, BinaryExpression))
                ElseIf TypeOf exp Is ArrayAccess Then
                    Return EvalArrayAccess(TryCast(exp, ArrayAccess))
                Else
                    Throw New TemplateRuntimeException("Invalid expression type: " & exp.[GetType]().Name, exp.Line, exp.Col)

                End If
            Catch ex As TemplateRuntimeException
                DisplayError(ex)
                Return Nothing
            Catch ex As Exception
                DisplayError(New TemplateRuntimeException(ex.Message, currentExpression.Line, currentExpression.Col))
                Return Nothing
            End Try
        End Function

        Protected Function EvalArrayAccess(ByVal arrayAccess As ArrayAccess) As Object
            Dim obj As Object = EvalExpression(arrayAccess.Exp)

            Dim index As Object = EvalExpression(arrayAccess.Index)

            If TypeOf obj Is Array Then
                Dim array As Array = DirectCast(obj, Array)
                If TypeOf index Is Int32 Then
                    Return array.GetValue(CInt(index))
                Else
                    Throw New TemplateRuntimeException("Index of array has to be integer", arrayAccess.Line, arrayAccess.Col)
                End If
            Else
                Return EvalMethodCall(obj, "get_Item", New Object() {index})

            End If
        End Function

        Protected Function EvalBinaryExpression(ByVal exp As BinaryExpression) As Object
            Select Case exp.[Operator]
                Case TokenKind.OpOr
                    If True Then
                        Dim lhsValue As Object = EvalExpression(exp.Lhs)
                        If Util.ToBool(lhsValue) Then
                            Return True
                        End If

                        Dim rhsValue As Object = EvalExpression(exp.Rhs)
                        Return Util.ToBool(rhsValue)
                    End If
                Case TokenKind.OpAnd
                    If True Then
                        Dim lhsValue As Object = EvalExpression(exp.Lhs)
                        If Not Util.ToBool(lhsValue) Then
                            Return False
                        End If

                        Dim rhsValue As Object = EvalExpression(exp.Rhs)
                        Return Util.ToBool(rhsValue)

                    End If
                Case TokenKind.OpIs
                    If True Then
                        Dim lhsValue As Object = EvalExpression(exp.Lhs)
                        Dim rhsValue As Object = EvalExpression(exp.Rhs)

                        Return lhsValue.Equals(rhsValue)
                    End If
                Case TokenKind.OpIsNot
                    If True Then
                        Dim lhsValue As Object = EvalExpression(exp.Lhs)
                        Dim rhsValue As Object = EvalExpression(exp.Rhs)

                        Return Not lhsValue.Equals(rhsValue)

                    End If
                Case TokenKind.OpGt
                    If True Then
                        Dim lhsValue As Object = EvalExpression(exp.Lhs)
                        Dim rhsValue As Object = EvalExpression(exp.Rhs)

                        Dim c1 As IComparable = TryCast(lhsValue, IComparable)
                        Dim c2 As IComparable = TryCast(rhsValue, IComparable)
                        If c1 Is Nothing OrElse c2 Is Nothing Then
                            Return False
                        Else
                            Return c1.CompareTo(c2) = 1
                        End If

                    End If
                Case TokenKind.OpLt
                    If True Then
                        Dim lhsValue As Object = EvalExpression(exp.Lhs)
                        Dim rhsValue As Object = EvalExpression(exp.Rhs)

                        Dim c1 As IComparable = TryCast(lhsValue, IComparable)
                        Dim c2 As IComparable = TryCast(rhsValue, IComparable)
                        If c1 Is Nothing OrElse c2 Is Nothing Then
                            Return False
                        Else
                            Return c1.CompareTo(c2) = -1
                        End If

                    End If
                Case TokenKind.OpGte
                    If True Then
                        Dim lhsValue As Object = EvalExpression(exp.Lhs)
                        Dim rhsValue As Object = EvalExpression(exp.Rhs)

                        Dim c1 As IComparable = TryCast(lhsValue, IComparable)
                        Dim c2 As IComparable = TryCast(rhsValue, IComparable)
                        If c1 Is Nothing OrElse c2 Is Nothing Then
                            Return False
                        Else
                            Return c1.CompareTo(c2) >= 0
                        End If

                    End If
                Case TokenKind.OpLte
                    If True Then
                        Dim lhsValue As Object = EvalExpression(exp.Lhs)
                        Dim rhsValue As Object = EvalExpression(exp.Rhs)

                        Dim c1 As IComparable = TryCast(lhsValue, IComparable)
                        Dim c2 As IComparable = TryCast(rhsValue, IComparable)
                        If c1 Is Nothing OrElse c2 Is Nothing Then
                            Return False
                        Else
                            Return c1.CompareTo(c2) <= 0
                        End If

                    End If
                Case Else
                    Throw New TemplateRuntimeException("Operator " & exp.[Operator].ToString() & " is not supported.", exp.Line, exp.Col)
            End Select
            Return Nothing
        End Function

        Protected Function EvalArguments(ByVal args As Expression()) As Object()
            Dim values As Object() = New Object(args.Length - 1) {}
            For i As Integer = 0 To values.Length - 1
                values(i) = EvalExpression(args(i))
            Next

            Return values
        End Function

        Protected Shared Function EvalProperty(ByVal obj As Object, ByVal propertyName As String) As Object
            If TypeOf obj Is StaticTypeReference Then
                Dim type As Type = TryCast(obj, StaticTypeReference).Type

                Dim pinfo As PropertyInfo = type.GetProperty(propertyName, BindingFlags.[Public] Or BindingFlags.IgnoreCase Or BindingFlags.GetProperty Or BindingFlags.[Static])
                If pinfo IsNot Nothing Then
                    Return pinfo.GetValue(Nothing, Nothing)
                End If

                Dim finfo As FieldInfo = type.GetField(propertyName, BindingFlags.[Public] Or BindingFlags.IgnoreCase Or BindingFlags.GetField Or BindingFlags.[Static])
                If finfo IsNot Nothing Then
                    Return finfo.GetValue(Nothing)
                Else
                    Throw New Exception(("Cannot find property or field named '" & propertyName & "' in object of type '") + type.Name & "'")


                End If
            Else
                Dim pinfo As PropertyInfo = obj.[GetType]().GetProperty(propertyName, BindingFlags.[Public] Or BindingFlags.IgnoreCase Or BindingFlags.GetProperty Or BindingFlags.Instance)

                If pinfo IsNot Nothing Then
                    Return pinfo.GetValue(obj, Nothing)
                End If

                Dim finfo As FieldInfo = obj.[GetType]().GetField(propertyName, BindingFlags.[Public] Or BindingFlags.IgnoreCase Or BindingFlags.GetField Or BindingFlags.Instance)

                If finfo IsNot Nothing Then
                    Return finfo.GetValue(obj)
                Else
                    Throw New Exception(("Cannot find property or field named '" & propertyName & "' in object of type '") + obj.[GetType]().Name & "'")

                End If

            End If
        End Function


        Protected Function EvalMethodCall(ByVal obj As Object, ByVal methodName As String, ByVal args As Object()) As Object
            Dim types As Type() = New Type(args.Length - 1) {}
            For i As Integer = 0 To args.Length - 1
                types(i) = args(i).[GetType]()
            Next

            If TypeOf obj Is StaticTypeReference Then
                Dim type As Type = TryCast(obj, StaticTypeReference).Type
                Dim method As MethodInfo = type.GetMethod(methodName, BindingFlags.[Public] Or BindingFlags.IgnoreCase Or BindingFlags.[Static], Nothing, types, Nothing)

                If method Is Nothing Then
                    Throw New Exception(String.Format("method {0} not found for static object of type {1}", methodName, type.Name))
                End If

                Return method.Invoke(Nothing, args)
            Else

                Dim method As MethodInfo = obj.[GetType]().GetMethod(methodName, BindingFlags.[Public] Or BindingFlags.IgnoreCase Or BindingFlags.Instance, Nothing, types, Nothing)

                If method Is Nothing Then
                    Throw New Exception(String.Format("method {0} not found for object of type {1}", methodName, obj.[GetType]().Name))
                End If

                Return method.Invoke(obj, args)
            End If
        End Function


        Protected Sub ProcessIf(ByVal tagIf As TagIf)
            Dim condition As Boolean = False

            Try
                Dim value As Object = EvalExpression(tagIf.Test)

                condition = Util.ToBool(value)
            Catch ex As Exception
                DisplayError("Error evaluating condition for if statement: " & ex.Message, tagIf.Line, tagIf.Col)
                Exit Sub
            End Try

            If condition Then
                ProcessElements(tagIf.InnerElements)
            Else
                ProcessElement(tagIf.FalseBranch)

            End If
        End Sub

        Protected Sub ProcessTag(ByVal tag As Tag)
            Dim name As String = tag.Name.ToLowerInvariant()
            Try
                Select Case name
                    Case "template"
                        ' skip those, because those are processed first
                        Exit Select
                    Case "else"
                        ProcessElements(tag.InnerElements)
                        Exit Select
                    Case "apply"
                        Dim val As Object = EvalExpression(tag.AttributeValue("template"))
                        ProcessTemplate(val.ToString(), tag)
                        Exit Select
                    Case "foreach"
                        ProcessForEach(tag)
                        Exit Select
                    Case "for"
                        ProcessFor(tag)
                        Exit Select
                    Case "set"
                        ProcessTagSet(tag)
                        Exit Select
                    Case Else
                        ProcessTemplate(tag.Name, tag)
                        Exit Select
                End Select
            Catch ex As TemplateRuntimeException
                DisplayError(ex)
            Catch ex As Exception

                DisplayError(("Error executing tag '" & name & "': ") + ex.Message, tag.Line, tag.Col)
            End Try
        End Sub

        Protected Sub ProcessTagSet(ByVal tag As Tag)
            Dim expName As Expression = tag.AttributeValue("name")
            If expName Is Nothing Then
                Throw New TemplateRuntimeException("Set is missing required attribute: name", tag.Line, tag.Col)
            End If

            Dim expValue As Expression = tag.AttributeValue("value")
            If expValue Is Nothing Then
                Throw New TemplateRuntimeException("Set is missing required attribute: value", tag.Line, tag.Col)
            End If


            Dim name As String = EvalExpression(expName).ToString()
            If Not Util.IsValidVariableName(name) Then
                Throw New TemplateRuntimeException("'" & name & "' is not valid variable name.", expName.Line, expName.Col)
            End If

            Dim value As Object = EvalExpression(expValue)

            Me.SetValue(name, value)
        End Sub

        Protected Sub ProcessForEach(ByVal tag As Tag)
            Dim expCollection As Expression = tag.AttributeValue("collection")
            If expCollection Is Nothing Then
                Throw New TemplateRuntimeException("Foreach is missing required attribute: collection", tag.Line, tag.Col)
            End If

            Dim collection As Object = EvalExpression(expCollection)
            If Not (TypeOf collection Is IEnumerable) Then
                Throw New TemplateRuntimeException("Collection used in foreach has to be enumerable", tag.Line, tag.Col)
            End If

            Dim expVar As Expression = tag.AttributeValue("var")
            If expCollection Is Nothing Then
                Throw New TemplateRuntimeException("Foreach is missing required attribute: var", tag.Line, tag.Col)
            End If
            Dim varObject As Object = EvalExpression(expVar)
            If varObject Is Nothing Then
                varObject = "foreach"
            End If
            Dim varname As String = varObject.ToString()

            Dim expIndex As Expression = tag.AttributeValue("index")
            Dim indexname As String = Nothing
            If expIndex IsNot Nothing Then
                Dim obj As Object = EvalExpression(expIndex)
                If obj IsNot Nothing Then
                    indexname = obj.ToString()
                End If
            End If

            Dim ienum As IEnumerator = DirectCast(collection, IEnumerable).GetEnumerator()
            Dim index As Integer = 0
            While ienum.MoveNext()
                index += 1
                Dim value As Object = ienum.Current
                variables(varname) = value
                If indexname IsNot Nothing Then
                    variables(indexname) = index
                End If

                ProcessElements(tag.InnerElements)
            End While
        End Sub

        Protected Sub ProcessFor(ByVal tag As Tag)
            Dim expFrom As Expression = tag.AttributeValue("from")
            If expFrom Is Nothing Then
                Throw New TemplateRuntimeException("For is missing required attribute: start", tag.Line, tag.Col)
            End If

            Dim expTo As Expression = tag.AttributeValue("to")
            If expTo Is Nothing Then
                Throw New TemplateRuntimeException("For is missing required attribute: to", tag.Line, tag.Col)
            End If

            Dim expIndex As Expression = tag.AttributeValue("index")
            If expIndex Is Nothing Then
                Throw New TemplateRuntimeException("For is missing required attribute: index", tag.Line, tag.Col)
            End If

            Dim obj As Object = EvalExpression(expIndex)
            Dim indexName As String = obj.ToString()

            Dim start As Integer = Convert.ToInt32(EvalExpression(expFrom))
            Dim [end] As Integer = Convert.ToInt32(EvalExpression(expTo))

            For index As Integer = start To [end]
                SetValue(indexName, index)
                'variables[indexName] = index;

                ProcessElements(tag.InnerElements)
            Next
        End Sub

        Protected Sub ExecuteCustomTag(ByVal tag As Tag)
            Dim tagHandler As ITagHandler = m_customTags(tag.Name)

            Dim processInnerElements As Boolean = True
            Dim captureInnerContent As Boolean = False

            tagHandler.TagBeginProcess(Me, tag, processInnerElements, captureInnerContent)

            Dim innerContent As String = Nothing

            If processInnerElements Then
                Dim saveWriter As TextWriter = writer

                If captureInnerContent Then
                    writer = New StringWriter()
                End If

                Try
                    ProcessElements(tag.InnerElements)

                    innerContent = writer.ToString()
                Finally
                    writer = saveWriter
                End Try
            End If


            tagHandler.TagEndProcess(Me, tag, innerContent)
        End Sub

        Protected Sub ProcessTemplate(ByVal name As String, ByVal tag As Tag)
            If m_customTags IsNot Nothing AndAlso m_customTags.ContainsKey(name) Then
                ExecuteCustomTag(tag)
                Exit Sub
            End If

            Dim useTemplate As Template = currentTemplate.FindTemplate(name)
            If useTemplate Is Nothing Then
                Dim msg As String = String.Format("Template '{0}' not found", name)
                Throw New TemplateRuntimeException(msg, tag.Line, tag.Col)
            End If

            ' process inner elements and save content
            Dim saveWriter As TextWriter = writer
            writer = New StringWriter()
            Dim content As String = String.Empty

            Try
                ProcessElements(tag.InnerElements)

                content = writer.ToString()
            Finally
                writer = saveWriter
            End Try

            Dim saveTemplate As Template = currentTemplate
            variables = New VariableScope(variables)
            variables("innerText") = content

            Try
                For Each attrib As TagAttribute In tag.Attributes
                    Dim val As Object = EvalExpression(attrib.Expression)
                    variables(attrib.Name) = val
                Next

                currentTemplate = useTemplate
                ProcessElements(currentTemplate.Elements)
            Finally
                variables = variables.Parent
                currentTemplate = saveTemplate


            End Try
        End Sub

        ''' <summary>
        ''' writes value to current writer
        ''' </summary>
        ''' <param name="value">value to be written</param>
        Public Sub WriteValue(ByVal value As Object)
            If value Is Nothing Then
                writer.Write("[null]")
            Else
                writer.Write(value)
            End If
        End Sub

        Private Sub DisplayError(ByVal ex As Exception)
            If TypeOf ex Is TemplateRuntimeException Then
                Dim tex As TemplateRuntimeException = DirectCast(ex, TemplateRuntimeException)
                DisplayError(ex.Message, tex.Line, tex.Col)
            Else
                DisplayError(ex.Message, 0, 0)
            End If
        End Sub

        Private Sub DisplayError(ByVal msg As String, ByVal line As Integer, ByVal col As Integer)
            If Not m_silentErrors Then
                writer.Write("[ERROR ({0}, {1}): {2}]", line, col, msg)
            End If
        End Sub
    End Class
End Namespace

Namespace Web.TemplateEngine.Parser
    Public Enum TokenKind
        EOF
        Comment
        ' common tokens
        ID
        ' (alpha)+
        ' text specific tokens
        TextData

        ' tag tokens
        TagStart
        ' <ad: 
        TagEnd
        ' > 
        TagEndClose
        ' />
        TagClose
        ' </ad:
        TagEquals
        ' =

        ' expression
        ExpStart
        ' # at the beginning
        ExpEnd
        ' # at the end
        LParen
        ' (
        RParen
        ' )
        Dot
        ' .
        Comma
        ' ,
        [Integer]
        ' integer number
        [Double]
        ' double number
        LBracket
        ' [
        RBracket
        ' ]
        ' operators
        OpOr
        ' "or" keyword
        OpAnd
        ' "and" keyword
        OpIs
        ' "is" keyword
        OpIsNot
        ' "isnot" keyword
        OpLt
        ' "lt" keyword
        OpGt
        ' "gt" keyword
        OpLte
        ' "lte" keyword
        OpGte
        ' "gte" keyword
        ' string tokens
        StringStart
        ' "
        StringEnd
        ' "
        StringText
        ' text within the string
    End Enum

    Public Class Token
        Private m_line As Integer
        Private m_col As Integer
        Private m_data As String
        Private m_tokenKind As TokenKind

        Public Sub New(ByVal kind As TokenKind, ByVal data As String, ByVal line As Integer, ByVal col As Integer)
            Me.m_tokenKind = kind
            Me.m_line = line
            Me.m_col = col
            Me.m_data = data
        End Sub

        Public ReadOnly Property Col() As Integer
            Get
                Return Me.m_col
            End Get
        End Property

        Public Property Data() As String
            Get
                Return Me.m_data
            End Get
            Set(ByVal value As String)
                Me.m_data = value
            End Set
        End Property

        Public ReadOnly Property Line() As Integer
            Get
                Return Me.m_line
            End Get
        End Property

        Public Property TokenKind() As TokenKind
            Get
                Return Me.m_tokenKind
            End Get
            Set(ByVal value As TokenKind)
                Me.m_tokenKind = value
            End Set
        End Property
    End Class
End Namespace
Imports LSW.Extension

Namespace Web
    Public Class JsonObject
        ''' <summary>
        ''' 解析JSON字串
        ''' </summary>
        Public Shared Function Parse(json As String) As JsonObject
            Dim obj As Object = json.JSONToObj(Of Object)()

            Return New JsonObject() With { _
                 .Value = obj _
            }
        End Function

        ''' <summary>
        ''' 取对象的属性
        ''' </summary>
        Default Public ReadOnly Property Item(key As String) As JsonObject
            Get
                Dim dict = TryCast(Me.Value, Dictionary(Of String, Object))
                If dict IsNot Nothing AndAlso dict.ContainsKey(key) Then
                    Return New JsonObject() With { _
                         .Value = dict(key) _
                    }
                End If

                Return New JsonObject()
            End Get
        End Property


        ''' <summary>
        ''' 取数组
        ''' </summary>
        Default Public ReadOnly Property Item(index As Integer) As JsonObject
            Get
                Dim array = TryCast(Me.Value, Object())
                If array IsNot Nothing AndAlso array.Length > index Then
                    Return New JsonObject() With { _
                         .Value = array(index) _
                    }
                End If
                Return New JsonObject()
            End Get
        End Property

        ''' <summary>
        ''' 将值以希望类型取出
        ''' </summary>
        Public Function GetValue(Of T)() As T
            Return DirectCast(Convert.ChangeType(Value, GetType(T)), T)
        End Function

        ''' <summary>
        ''' 取出字串类型的值
        ''' </summary>
        Public Function Text() As String
            Return Convert.ToString(Value)
        End Function

        ''' <summary>
        ''' 取出数值
        ''' </summary>
        Public Function Number() As Double
            Return Convert.ToDouble(Value)
        End Function

        ''' <summary>
        ''' 取出整型
        ''' </summary>
        Public Function [Integer]() As Integer
            Return Convert.ToInt32(Value)
        End Function

        ''' <summary>
        ''' 取出布尔型
        ''' </summary>
        Public Function [Boolean]() As Boolean
            Return Convert.ToBoolean(Value)
        End Function

        ''' <summary>
        ''' 值
        ''' </summary>
        Public Property Value() As Object

        ''' <summary>
        ''' 如果是数组返回数组长度
        ''' </summary>
        Public ReadOnly Property Length() As Integer
            Get
                Dim array = TryCast(Me.Value, Object())
                If array IsNot Nothing Then
                    Return array.Length
                End If
                Return 0
            End Get
        End Property
    End Class
End Namespace
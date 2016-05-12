Imports System.Text

Namespace Web
    Public Class JSON
        '是否成功   
        Private success As Boolean
        '错误提示信息   
        Private [error] As String
        '总记   
        Private totalCount As Integer
        '数据   
        Private singleInfo As String

        Private arrData As ArrayList

#Region "初始化JSON的所有对象"
        Public Sub New()
            [error] = String.Empty
            singleInfo = String.Empty
            totalCount = 0
            success = False
            arrData = New ArrayList()
        End Sub
#End Region

#Region "重置JSON的所有对象"
        Public Sub ResetJSON()
            [error] = String.Empty
            singleInfo = String.Empty
            totalCount = 0
            success = False
            arrData.Clear()
        End Sub
#End Region

#Region "对象与对象之间分割符"
        Public Sub addItemOk()
            arrData.Add("<br>")
        End Sub
#End Region

#Region "在数组里添加key,value"
        Public Sub addItem(name As String, value As String)
            arrData.Add(name & ":" & """" & value & """")
        End Sub
#End Region

#Region "在数组里添加key,value"
        Public Sub addItems(name As String, value As String)
            arrData.Add(name & ":" & value)
        End Sub
#End Region


#Region "返回组装好的json字符串"
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()


            Dim index As Integer = 0
            sb.Append("{")
            If arrData.Count <= 0 Then
                sb.Append("}]")
            Else
                For Each val As String In arrData
                    index += 1

                    If val <> "<br>" Then
                        sb.Append(val & ",")
                    Else
                        sb = sb.Replace(",", "", sb.Length - 1, 1)
                        sb.Append("},")
                        If index < arrData.Count Then
                            sb.Append("{")
                        End If

                    End If
                Next
                sb = sb.Replace(",", "", sb.Length - 1, 1)
            End If

            sb.Append("}")
            Return sb.ToString()

        End Function
#End Region
        Public Property errorS() As String
            Get
                Return Me.[error]
            End Get
            Set(value As String)
                Me.[error] = value
            End Set
        End Property

        Public Property successS() As Boolean
            Get
                Return Me.success
            End Get
            Set(value As Boolean)
                Me.success = value
            End Set
        End Property

        Public Property totalCountS() As Integer
            Get
                Return Me.totalCount
            End Get
            Set(value As Integer)
                Me.totalCount = value
            End Set
        End Property
    End Class
End Namespace
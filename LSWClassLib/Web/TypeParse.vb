Imports System.Text.RegularExpressions

Namespace Web
    Public Class TypeParse


        ''' <summary> 
        ''' 判断对象是否为Int32类型的数字 
        ''' </summary> 
        ''' <param name="Expression"></param> 
        ''' <returns></returns> 
        Public Shared Function IsNumeric(ByVal expression As Object) As Boolean
            If expression IsNot Nothing Then
                Return IsNumeric(expression.ToString())
            End If
            Return False

        End Function

        ''' <summary> 
        ''' 判断对象是否为Int32类型的数字 
        ''' </summary> 
        ''' <param name="Expression"></param> 
        ''' <returns></returns> 
        Public Shared Function IsNumeric(ByVal expression As String) As Boolean
            If expression IsNot Nothing Then
                Dim str As String = expression
                If str.Length > 0 AndAlso str.Length <= 11 AndAlso Regex.IsMatch(str, "^[-]?[0-9]*[.]?[0-9]*$") Then
                    If (str.Length < 10) OrElse (str.Length = 10 AndAlso str(0) = "1"c) OrElse (str.Length = 11 AndAlso str(0) = "-"c AndAlso str(1) = "1"c) Then
                        Return True
                    End If
                End If
            End If
            Return False

        End Function

        ''' <summary> 
        ''' 是否为Double类型 
        ''' </summary> 
        ''' <param name="expression"></param> 
        ''' <returns></returns> 
        Public Shared Function IsDouble(ByVal expression As Object) As Boolean
            If expression IsNot Nothing Then
                Return Regex.IsMatch(expression.ToString(), "^([0-9])[0-9]*(\.\w*)?$")
            End If
            Return False
        End Function

        ''' <summary> 
        ''' string型转换为bool型 
        ''' </summary> 
        ''' <param name="expression">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的bool类型结果</returns> 
        Public Shared Function StrToBool(ByVal expression As Object, ByVal defValue As Boolean) As Boolean
            If expression IsNot Nothing Then
                Return StrToBool(expression, defValue)
            End If
            Return defValue
        End Function

        ''' <summary> 
        ''' string型转换为bool型 
        ''' </summary> 
        ''' <param name="expression">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的bool类型结果</returns> 
        Public Shared Function StrToBool(ByVal expression As String, ByVal defValue As Boolean) As Boolean
            If expression IsNot Nothing Then
                If String.Compare(expression, "true", True) = 0 Then
                    Return True
                ElseIf String.Compare(expression, "false", True) = 0 Then
                    Return False
                End If
            End If
            Return defValue
        End Function

        ''' <summary> 
        ''' 将对象转换为Int32类型 
        ''' </summary> 
        ''' <param name="expression">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的int类型结果</returns> 
        Public Shared Function StrToInt(ByVal expression As Object, ByVal defValue As Integer) As Integer
            If expression IsNot Nothing Then
                Return StrToInt(expression.ToString(), defValue)
            End If
            Return defValue
        End Function

        ''' <summary> 
        ''' 将对象转换为Int32类型 
        ''' </summary> 
        ''' <param name="str">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的int类型结果</returns> 
        Public Shared Function StrToInt(ByVal str As String, ByVal defValue As Integer) As Integer
            If str Is Nothing Then
                Return defValue
            End If
            If str.Length > 0 AndAlso str.Length <= 11 AndAlso Regex.IsMatch(str, "^[-]?[0-9]*$") Then
                If (str.Length < 10) OrElse (str.Length = 10 AndAlso str(0) = "1"c) OrElse (str.Length = 11 AndAlso str(0) = "-"c AndAlso str(1) = "1"c) Then
                    Return Convert.ToInt32(str)
                End If
            End If
            Return defValue
        End Function

        ''' <summary> 
        ''' string型转换为float型 
        ''' </summary> 
        ''' <param name="strValue">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的int类型结果</returns> 
        Public Shared Function StrToFloat(ByVal strValue As Object, ByVal defValue As Single) As Single
            If (strValue Is Nothing) Then
                Return defValue
            End If

            Return StrToFloat(strValue.ToString(), defValue)
        End Function

        ''' <summary> 
        ''' string型转换为float型 
        ''' </summary> 
        ''' <param name="strValue">要转换的字符串</param> 
        ''' <param name="defValue">缺省值</param> 
        ''' <returns>转换后的int类型结果</returns> 
        Public Shared Function StrToFloat(ByVal strValue As String, ByVal defValue As Single) As Single
            If (strValue Is Nothing) OrElse (strValue.Length > 10) Then
                Return defValue
            End If

            Dim intValue As Single = defValue
            If strValue IsNot Nothing Then
                Dim IsFloat As Boolean = Regex.IsMatch(strValue, "^([-]|[0-9])[0-9]*(\.\w*)?$")
                If IsFloat Then
                    intValue = Convert.ToSingle(strValue)
                End If
            End If
            Return intValue
        End Function


        ''' <summary> 
        ''' 判断给定的字符串数组(strNumber)中的数据是不是都为数值型 
        ''' </summary> 
        ''' <param name="strNumber">要确认的字符串数组</param> 
        ''' <returns>是则返加true 不是则返回 false</returns> 
        Public Shared Function IsNumericArray(ByVal strNumber As String()) As Boolean
            If strNumber Is Nothing Then
                Return False
            End If
            If strNumber.Length < 1 Then
                Return False
            End If
            For Each id As String In strNumber
                If Not IsNumeric(id) Then
                    Return False
                End If
            Next
            Return True

        End Function
    End Class
End Namespace
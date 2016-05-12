Namespace Flash
    Public Module Live012Helper
        ''' <summary>
        ''' code解码
        ''' </summary>
        ''' <param name="param1"></param>
        ''' <returns></returns>
        Public Function decode(param1 As String) As String
            Dim _loc_2 As String = Nothing
            Dim _loc_3 As String = Nothing
            Dim _loc_4 As String = Nothing
            Dim _loc_5 As Integer = 0
            _loc_2 = ""
            _loc_3 = ""
            _loc_4 = ""
            _loc_5 = 0
            _loc_4 = param1.Substring(param1.Length - 5, 5) & param1.Substring(0, param1.Length - 5)
            _loc_5 = 0
            While _loc_5 < _loc_4.Length

                _loc_2 = _loc_4.Substring(_loc_5, 1) & _loc_2
                _loc_5 += 1
            End While
            _loc_5 = 0
            While _loc_5 < _loc_2.Length

                _loc_3 = _loc_3 & ChrW(255 - Convert.ToInt32(_loc_2.Substring(_loc_5, 2), 16))
                _loc_5 = _loc_5 + 2
            End While
            Return _loc_3
        End Function

        ''' <summary>
        ''' code编码
        ''' </summary>
        ''' <param name="param1"></param>
        ''' <returns></returns>
        Public Function encode(param1 As String) As String
            Dim _loc_2 As String = Nothing
            Dim _loc_3 As String = Nothing
            Dim _loc_4 As String = Nothing
            Dim _loc_5 As Integer = 0
            _loc_2 = ""
            _loc_3 = ""
            _loc_4 = ""
            _loc_5 = 0
            While _loc_5 < param1.Length

                _loc_2 = _loc_2 & [String].Format("{0:X}", (255 - AscW(param1(_loc_5))))
                _loc_5 += 1
            End While
            _loc_5 = 0
            While _loc_5 < _loc_2.Length

                _loc_3 = _loc_2.Substring(_loc_5, 1) & _loc_3
                _loc_5 += 1
            End While
            _loc_4 = _loc_3.Substring(5, _loc_3.Length - 5) & _loc_3.Substring(0, 5)
            Return _loc_4
        End Function
    End Module
End Namespace
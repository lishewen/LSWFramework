Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports LSW.Image.Captcha

Namespace Image
    Public Module VerificationCode
        ''' <summary>
        ''' 获取验证码
        ''' </summary>
        ''' <param name="p_Bitmap">图形 http://www.fjjj.gov.cn/Article/getcode.asp</param>
        ''' <returns>数值</returns>
        Public Function GetCodeText(p_Bitmap As Bitmap) As String
            Dim _Width As Integer = p_Bitmap.Width / 4
            Dim _Height As Integer = p_Bitmap.Height

            Dim _Bitmap As Bitmap() = New Bitmap(3) {}
            Dim _Rectangle As New Rectangle()
            _Rectangle.Width = _Width
            _Rectangle.Height = _Height
            Dim i As Integer = 0
            While i <> _Bitmap.Length
                _Bitmap(i) = p_Bitmap.Clone(_Rectangle, p_Bitmap.PixelFormat)
                _Rectangle.X += _Width
                i += 1
            End While
            Dim _Value1 As Integer = Array.IndexOf(_TextBytes, GetImageText(_Bitmap(0)))
            Dim _Value2 As Integer = Array.IndexOf(_TextBytes, GetImageText(_Bitmap(1)))
            Dim _Value3 As Integer = Array.IndexOf(_TextBytes, GetImageText(_Bitmap(2)))
            Dim _Value4 As Integer = Array.IndexOf(_TextBytes, GetImageText(_Bitmap(3)))

            Dim _Value As String = If(_Value1 = -1, "?", _Value1.ToString())
            _Value += If(_Value2 = -1, "?", _Value2.ToString())
            _Value += If(_Value3 = -1, "?", _Value3.ToString())
            _Value += If(_Value4 = -1, "?", _Value4.ToString())
            Return _Value

        End Function


        Private _TextBytes As String() = New String() {
            "E17BEFBDF7DE7BEFBDF7DE87FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "FBE3BFFFFEFBEFBFFFFEFB83FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "E17BFFFDF7EFDFBF7FFFFE03FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "E17BFFFDF7E37FFFFDF7DE87FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "EF9FBFFEFAEDBB0FFCFBEF1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "C0FBEFBFFFE07FFFFDF7DE87FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "E3F7EFBFFFE273EFBDF7DE87FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "C07BFFFEFBF7DFBFFFFEFDF7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "E17BEFBDF7E17BEFBDF7DE87FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            "E17BEFBDF7CE47FFFDF7EFC7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"}


        ''' <summary>
        ''' 获取二值化数据
        ''' </summary>
        ''' <param name="p_Bitmap">图形</param>
        ''' <returns>二值化数据</returns>
        Public Function GetImageText(p_Bitmap As Bitmap) As String
            Dim _Width As Integer = p_Bitmap.Width
            Dim _Height As Integer = p_Bitmap.Height
            Dim _Data As BitmapData = p_Bitmap.LockBits(New Rectangle(0, 0, _Width, _Height), ImageLockMode.[ReadOnly], p_Bitmap.PixelFormat)

            Dim _DataByte As Byte() = New Byte(_Data.Stride * _Height - 1) {}

            Marshal.Copy(_Data.Scan0, _DataByte, 0, _DataByte.Length)

            Dim _Bitarray As New BitArray(_DataByte.Length, True)

            Dim _Index As Integer = 0
            Dim i As Integer = 0
            While i <> _Height
                Dim _WidthStar As Integer = i * _Data.Stride
                Dim z As Integer = 0
                While z <> _Width
                    If _DataByte(_WidthStar + (z * 3)) = 238 AndAlso _DataByte(_WidthStar + (z * 3) + 1) = 238 AndAlso _DataByte(_WidthStar + (z * 3) + 2) = 238 Then
                        _Bitarray(_Index) = True
                    Else
                        _Bitarray(_Index) = False
                    End If
                    _Index += 1
                    z += 1
                End While
                i += 1
            End While
            p_Bitmap.UnlockBits(_Data)

            Dim _ByteIndex As Integer = _Bitarray.Count / 8
            If _Bitarray.Count Mod 8 <> 0 Then
                _ByteIndex += 1
            End If

            Dim _Temp As Byte() = New Byte(_ByteIndex - 1) {}
            _Bitarray.CopyTo(_Temp, 0)

            Return BitConverter.ToString(_Temp).Replace("-", "")
        End Function

        Public Function GetCaptcha(Optional charcount As Integer = 4) As CaptchaRecognition
            Dim n As New CaptchaRecognition With {.CharCount = charcount}
            n.Classifier.LoadSamplesFromText(My.Resources.Features)
            Return n
        End Function

        Public Function GetCaptcha(bmp As Bitmap, Optional charcount As Integer = 4) As String
            Return GetCaptcha(charcount).Recognize(bmp)
        End Function
    End Module
End Namespace
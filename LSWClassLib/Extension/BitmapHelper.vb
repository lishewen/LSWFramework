Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Drawing.Imaging

Namespace Extension
    Public Module BitmapHelper
        <Extension>
        Public Function BytesToBitmap(Bytes() As Byte) As Bitmap
            Dim stream As MemoryStream = Nothing
            Try
                stream = New MemoryStream(Bytes)
                Return New Bitmap(DirectCast(New Bitmap(stream), Drawing.Image))
            Catch ex As ArgumentNullException
                Throw ex
            Catch ex As ArgumentException
                Throw ex
            Finally
                stream.Close()
            End Try
        End Function

        <Extension>
        Public Function BitmapToBytes(Bitmap As Bitmap) As Byte()
            Dim ms As MemoryStream = Nothing
            Try
                ms = New MemoryStream()
                Bitmap.Save(ms, Bitmap.RawFormat)
                Dim byteImage As Byte() = New Byte(ms.Length - 1) {}
                byteImage = ms.ToArray()
                Return byteImage
            Catch ex As ArgumentNullException
                Throw ex
            Finally
                ms.Close()
            End Try
        End Function

        <Extension>
        Public Function ToBase64String(bmp As Bitmap) As String
            Try
                Using ms As New MemoryStream
                    bmp.Save(ms, ImageFormat.Jpeg)
                    Dim arr(ms.Length - 1) As Byte
                    ms.Position = 0
                    ms.Read(arr, 0, ms.Length)
                    ms.Close()
                    Return Convert.ToBase64String(arr)
                End Using
            Catch ex As Exception
                Return "ImgToBase64String 转换失败 Exception:" + ex.Message
            End Try
        End Function

        <Extension>
        Public Function FromBase64String(str As String) As Bitmap
            Try
                Dim arr = Convert.FromBase64String(str)
                Using ms As New MemoryStream(arr)
                    Return New Bitmap(ms)
                End Using
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' 获取图片相似度（简单）
        ''' </summary>
        ''' <param name="pic1"></param>
        ''' <param name="pic2"></param>
        ''' <param name="precision">精度 越少越准确但耗时越长</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function GetPicSimilarity(pic1 As Bitmap, pic2 As Bitmap, Optional precision As Integer = 5) As Decimal
            '如果图片大小不一，则相似度为0
            If pic1.Size.Height <> pic2.Height OrElse pic1.Width <> pic2.Width Then
                Return 0
            End If

            '检查的像素个数
            Dim pixelCount As Integer = 0

            '不同颜色的像素个数
            Dim diffCount As Integer = 0

            '隔5个像素检查一个点，是为了减少耗费时间，可根据情况减少或增加
            For i As Integer = 0 To pic1.Width - 1 Step precision
                pixelCount += 1
                For j As Integer = 0 To pic2.Height - 1 Step precision
                    pixelCount += 1
                    If pic1.GetPixel(i, j) <> pic2.GetPixel(i, j) Then
                        diffCount += 1
                    End If
                Next
            Next
            Return 1 - Convert.ToDecimal(diffCount) / Convert.ToDecimal(pixelCount)
        End Function

        ''' <summary>
        ''' 截取图片区域，返回所截取的图片
        ''' </summary>
        ''' <param name="SrcImage"></param>
        ''' <param name="pos"></param>
        ''' <param name="cutWidth"></param>
        ''' <param name="cutHeight"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Cut(SrcImage As Drawing.Image, pos As Point, cutWidth As Integer, cutHeight As Integer) As Drawing.Image
            Dim cutedImage As Drawing.Image = Nothing

            '先初始化一个位图对象，来存储截取后的图像
            Dim bmpDest As New Bitmap(cutWidth, cutHeight, PixelFormat.Format32bppRgb)
            Dim g As Graphics = Graphics.FromImage(bmpDest)

            '矩形定义,将要在被截取的图像上要截取的图像区域的左顶点位置和截取的大小
            Dim rectSource As New Rectangle(pos.X, pos.Y, cutWidth, cutHeight)


            '矩形定义,将要把 截取的图像区域 绘制到初始化的位图的位置和大小
            'rectDest说明，将把截取的区域，从位图左顶点开始绘制，绘制截取的区域原来大小
            Dim rectDest As New Rectangle(0, 0, cutWidth, cutHeight)

            '第一个参数就是加载你要截取的图像对象，第二个和第三个参数及如上所说定义截取和绘制图像过程中的相关属性，第四个属性定义了属性值所使用的度量单位
            g.DrawImage(SrcImage, rectDest, rectSource, GraphicsUnit.Pixel)

            '在GUI上显示被截取的图像
            cutedImage = DirectCast(bmpDest, Drawing.Image)

            g.Dispose()

            Return cutedImage
        End Function
    End Module
End Namespace
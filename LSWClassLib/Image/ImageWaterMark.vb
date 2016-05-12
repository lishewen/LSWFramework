Imports System.IO
Imports System.Collections
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Namespace Image
    ''' <summary>
    ''' 水印的类型
    ''' </summary>
    Public Enum WaterMarkType
        ''' <summary>
        ''' 文字水印
        ''' </summary>
        TextMark
        '' <summary>
        '' 图片水印
        '' </summary>
        'ImageMark // 暂时只能添加文字水印
    End Enum
    ''' <summary>
    ''' 水印的位置
    ''' </summary>
    Public Enum WaterMarkPosition
        ''' <summary>
        ''' 左上角
        ''' </summary>
        WMP_Left_Top
        ''' <summary>
        ''' 左下角
        ''' </summary>
        WMP_Left_Bottom
        ''' <summary>
        ''' 右上角
        ''' </summary>
        WMP_Right_Top
        ''' <summary>
        ''' 右下角
        ''' </summary>
        WMP_Right_Bottom
    End Enum
    ''' <summary>
    ''' 处理图片的类（包括加水印，生成缩略图）
    ''' </summary>
    Public Class ImageWaterMark
        '
        ' TODO: 在此处添加构造函数逻辑
        '
        Public Sub New()
        End Sub
#Region "给图片加水印"
        ''' <summary>
        ''' 添加水印(分图片水印与文字水印两种)
        ''' </summary>
        ''' <param name="oldpath">原图片绝对地址</param>
        ''' <param name="newpath">新图片放置的绝对地址</param>
        ''' <param name="wmtType">要添加的水印的类型</param>
        ''' <param name="sWaterMarkContent">水印内容，若添加文字水印，此即为要添加的文字；
        ''' 若要添加图片水印，此为图片的路径</param>
        Public Sub addWaterMark(oldpath As String, newpath As String, wmtType As WaterMarkType, sWaterMarkContent As String)
            Try
                Dim image__1 As Drawing.Image = Drawing.Image.FromFile(oldpath)
                Dim b As New Bitmap(image__1.Width, image__1.Height, PixelFormat.Format24bppRgb)
                Dim g As Graphics = Graphics.FromImage(b)
                g.Clear(Color.White)
                g.SmoothingMode = SmoothingMode.HighQuality
                g.InterpolationMode = InterpolationMode.High
                g.DrawImage(image__1, 0, 0, image__1.Width, image__1.Height)
                Select Case wmtType

                    Case WaterMarkType.TextMark
                        '文字水印
                        Me.addWatermarkText(g, sWaterMarkContent, "WM_BOTTOM_RIGHT", image__1.Width, image__1.Height)
                        Exit Select
                End Select
                b.Save(newpath)
                b.Dispose()
                image__1.Dispose()
            Catch
                If File.Exists(oldpath) Then
                    File.Delete(oldpath)
                End If
            Finally
                If File.Exists(oldpath) Then
                    File.Delete(oldpath)
                End If
            End Try
        End Sub
        ''' <summary>
        '''　　 加水印文字
        ''' </summary>
        ''' <param name="picture">imge 对象</param>
        ''' <param name="_watermarkText">水印文字内容</param>
        Public Sub addWatermarkText(picture As Bitmap, _watermarkText As String)
            Dim g = Graphics.FromImage(picture)
            addWatermarkText(g, _watermarkText, "WM_BOTTOM_LEFT", picture.Width, picture.Height)
        End Sub
        ''' <summary>
        '''　　 加水印文字
        ''' </summary>
        ''' <param name="picture">imge 对象</param>
        ''' <param name="_watermarkText">水印文字内容</param>
        ''' <param name="_watermarkPosition">水印位置</param>
        ''' <param name="_width">被加水印图片的宽</param>
        ''' <param name="_height">被加水印图片的高</param>
        Private Sub addWatermarkText(picture As Graphics, _watermarkText As String, _watermarkPosition As String, _width As Integer, _height As Integer)
            ' 确定水印文字的字体大小
            Dim sizes As Integer() = New Integer() {32, 30, 28, 26, 24, 22, _
             20, 18, 16, 14, 12, 10, _
             8, 6, 4}
            Dim crFont As Font = Nothing
            Dim crSize As New SizeF()
            For i As Integer = 0 To sizes.Length - 1
                crFont = New Font("Arial Black", sizes(i), FontStyle.Bold)
                crSize = picture.MeasureString(_watermarkText, crFont)
                If CUShort(System.Math.Truncate(crSize.Width)) < CUShort(_width) Then
                    Exit For
                End If
            Next
            ' 生成水印图片（将文字写到图片中）
            Dim floatBmp As New Bitmap(CInt(System.Math.Truncate(crSize.Width)) + 3, CInt(System.Math.Truncate(crSize.Height)) + 3, PixelFormat.Format32bppArgb)
            Dim fg As Graphics = Graphics.FromImage(floatBmp)
            Dim pt As New PointF(0, 0)
            ' 画阴影文字
            Dim TransparentBrush0 As Brush = New SolidBrush(Color.FromArgb(255, Color.Black))
            Dim TransparentBrush1 As Brush = New SolidBrush(Color.FromArgb(255, Color.Black))
            fg.DrawString(_watermarkText, crFont, TransparentBrush0, pt.X, pt.Y + 1)
            fg.DrawString(_watermarkText, crFont, TransparentBrush0, pt.X + 1, pt.Y)
            fg.DrawString(_watermarkText, crFont, TransparentBrush1, pt.X + 1, pt.Y + 1)
            fg.DrawString(_watermarkText, crFont, TransparentBrush1, pt.X, pt.Y + 2)
            fg.DrawString(_watermarkText, crFont, TransparentBrush1, pt.X + 2, pt.Y)
            TransparentBrush0.Dispose()
            TransparentBrush1.Dispose()
            ' 画文字
            fg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
            fg.DrawString(_watermarkText, crFont, New SolidBrush(Color.White), pt.X, pt.Y, StringFormat.GenericDefault)
            ' 保存刚才的操作
            fg.Save()
            fg.Dispose()
            ' floatBmp.Save("d:\WebSite\DIGITALKM\ttt.jpg");　　　 // 将水印图片加到原图中
            Me.addWatermarkImage(picture, New Bitmap(floatBmp), "WM_BOTTOM_LEFT", _width, _height)
        End Sub
        ''' <summary>
        '''　　 加水印图片
        ''' </summary>
        ''' <param name="picture">imge 对象</param>
        ''' <param name="iTheImage">Image对象（以此图片为水印）</param>
        ''' <param name="_watermarkPosition">水印位置</param>
        ''' <param name="_width">被加水印图片的宽</param>
        ''' <param name="_height">被加水印图片的高</param>
        Private Sub addWatermarkImage(picture As Graphics, iTheImage As Drawing.Image, _watermarkPosition As String, _width As Integer, _height As Integer)
            Dim watermark As Drawing.Image = New Bitmap(iTheImage)
            Dim imageAttributes As New ImageAttributes()
            Dim colorMap As New ColorMap()
            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0)
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0)
            Dim remapTable As ColorMap() = {colorMap}
            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap)
            Dim colorMatrixElements As Single()() = {New Single() {1.0F, 0.0F, 0.0F, 0.0F, 0.0F}, New Single() {0.0F, 1.0F, 0.0F, 0.0F, 0.0F}, New Single() {0.0F, 0.0F, 1.0F, 0.0F, 0.0F}, New Single() {0.0F, 0.0F, 0.0F, 0.3F, 0.0F}, New Single() {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}}
            Dim colorMatrix As New ColorMatrix(colorMatrixElements)
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.[Default], ColorAdjustType.Bitmap)
            Dim xpos As Integer = 0
            Dim ypos As Integer = 0
            Dim WatermarkWidth As Integer = 0
            Dim WatermarkHeight As Integer = 0
            Dim bl As Double = 1.0
            '计算水印图片的比率
            '取背景的1/4宽度来比较
            If (_width > watermark.Width * 4) AndAlso (_height > watermark.Height * 4) Then
                bl = 1
            ElseIf (_width > watermark.Width * 4) AndAlso (_height < watermark.Height * 4) Then

                bl = Convert.ToDouble(_height \ 4) / Convert.ToDouble(watermark.Height)
            ElseIf (_width < watermark.Width * 4) AndAlso (_height > watermark.Height * 4) Then
                bl = Convert.ToDouble(_width \ 4) / Convert.ToDouble(watermark.Width)
            Else
                If (_width * watermark.Height) > (_height * watermark.Width) Then

                    bl = Convert.ToDouble(_height \ 4) / Convert.ToDouble(watermark.Height)
                Else

                    bl = Convert.ToDouble(_width \ 4) / Convert.ToDouble(watermark.Width)

                End If
            End If
            WatermarkWidth = Convert.ToInt32(watermark.Width * bl)
            WatermarkHeight = Convert.ToInt32(watermark.Height * bl)
            Select Case _watermarkPosition
                Case "WM_TOP_LEFT"
                    xpos = 10
                    ypos = 10
                    Exit Select
                Case "WM_TOP_RIGHT"
                    xpos = _width - WatermarkWidth - 10
                    ypos = 10
                    Exit Select
                Case "WM_BOTTOM_RIGHT"
                    xpos = _width - WatermarkWidth - 10
                    ypos = _height - WatermarkHeight - 10
                    Exit Select
                Case "WM_BOTTOM_LEFT"
                    xpos = 10
                    ypos = _height - WatermarkHeight - 10
                    Exit Select
            End Select
            picture.DrawImage(watermark, New Rectangle(xpos, ypos, WatermarkWidth, WatermarkHeight), 0, 0, watermark.Width, watermark.Height, _
             GraphicsUnit.Pixel, imageAttributes)
            watermark.Dispose()
            imageAttributes.Dispose()
        End Sub
        ''' <summary>
        '''　　 加水印图片
        ''' </summary>
        ''' <param name="picture">imge 对象</param>
        ''' <param name="WaterMarkPicPath">水印图片的地址</param>
        ''' <param name="_watermarkPosition">水印位置</param>
        ''' <param name="_width">被加水印图片的宽</param>
        ''' <param name="_height">被加水印图片的高</param>
        Private Sub addWatermarkImage(picture As Graphics, WaterMarkPicPath As String, _watermarkPosition As String, _width As Integer, _height As Integer)
            Dim watermark As Drawing.Image = New Bitmap(WaterMarkPicPath)
            Me.addWatermarkImage(picture, watermark, _watermarkPosition, _width, _height)
        End Sub
#End Region
        ''' <summary>
        ''' 保存图片
        ''' </summary>
        ''' <param name="image">Image 对象</param>
        ''' <param name="savePath">保存路径</param>
        ''' <param name="ici">指定格式的编解码参数</param>
        Private Sub SaveImage(image As Drawing.Image, savePath As String, ici As ImageCodecInfo)
            '设置 原图片 对象的 EncoderParameters 对象
            Dim parameters As New EncoderParameters(1)
            parameters.Param(0) = New EncoderParameter(System.Drawing.Imaging.Encoder.Quality, CLng(90))
            image.Save(savePath, ici, parameters)
            parameters.Dispose()
        End Sub
        ''' <summary>
        ''' 获取图像编码解码器的所有相关信息
        ''' </summary>
        ''' <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        ''' <returns>返回图像编码解码器的所有相关信息</returns>
        Private Function GetCodecInfo(mimeType As String) As ImageCodecInfo
            Dim CodecInfo As ImageCodecInfo() = ImageCodecInfo.GetImageEncoders()
            For Each ici As ImageCodecInfo In CodecInfo
                If ici.MimeType = mimeType Then
                    Return ici
                End If
            Next
            Return Nothing
        End Function
        ''' <summary>
        ''' 生成缩略图
        ''' </summary>
        ''' <param name="sourceImagePath">原图片路径(相对路径)</param>
        ''' <param name="thumbnailImagePath">生成的缩略图路径,如果为空则保存为原图片路径(相对路径)</param>
        ''' <param name="thumbnailImageWidth">缩略图的宽度（高度与按源图片比例自动生成）</param>
        Public Sub ToThumbnailImages(SourceImagePath As String, ThumbnailImagePath As String, ThumbnailImageWidth As Integer)
            Dim htmimes As New Hashtable()
            htmimes(".jpeg") = "image/jpeg"
            htmimes(".jpg") = "image/jpeg"
            htmimes(".png") = "image/png"
            htmimes(".tif") = "image/tiff"
            htmimes(".tiff") = "image/tiff"
            htmimes(".bmp") = "image/bmp"
            htmimes(".gif") = "image/gif"
            ' 取得原图片的后缀
            Dim sExt As String = SourceImagePath.Substring(SourceImagePath.LastIndexOf(".")).ToLower()
            '从 原图片创建 Image 对象
            Dim image__1 As Drawing.Image = Drawing.Image.FromFile(SourceImagePath)
            Dim num As Integer = ((ThumbnailImageWidth \ 4) * 3)
            Dim width As Integer = image__1.Width
            Dim height As Integer = image__1.Height
            '计算图片的比例
            If (CDbl(width) / CDbl(height)) >= 1.333333F Then
                num = ((height * ThumbnailImageWidth) \ width)
            Else
                ThumbnailImageWidth = ((width * num) \ height)
            End If
            If (ThumbnailImageWidth < 1) OrElse (num < 1) Then
                Return
            End If
            '用指定的大小和格式初始化 Bitmap 类的新实例
            Dim bitmap As New Bitmap(ThumbnailImageWidth, num, PixelFormat.Format32bppArgb)
            '从指定的 Image 对象创建新 Graphics 对象
            Dim graphics__2 As Graphics = Graphics.FromImage(bitmap)
            '清除整个绘图面并以透明背景色填充
            graphics__2.Clear(Color.Transparent)
            graphics__2.SmoothingMode = SmoothingMode.HighQuality
            graphics__2.InterpolationMode = InterpolationMode.High
            '在指定位置并且按指定大小绘制 原图片 对象
            graphics__2.DrawImage(image__1, New Rectangle(0, 0, ThumbnailImageWidth, num))
            image__1.Dispose()

            Try
                '将此 原图片 以指定格式并用指定的编解码参数保存到指定文件
                SaveImage(bitmap, ThumbnailImagePath, GetCodecInfo(DirectCast(htmimes(sExt), String)))
            Catch e As System.Exception
                Throw e
            End Try
        End Sub
    End Class
End Namespace
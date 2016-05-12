Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports LSW.Win32
Imports LSW.Exceptions
Imports LSW.Extension

Namespace Image
    Public Module PicHelper

        ''' <summary>
        ''' 生成缩略图
        ''' </summary>
        ''' <param name="originalImagePath">源图路径（物理路径）</param>
        ''' <param name="thumbnailPath">缩略图路径（物理路径）</param>
        ''' <param name="width">缩略图宽度</param>
        ''' <param name="height">缩略图高度</param>
        ''' <param name="mode">生成缩略图的方式</param>
        ''' <remarks></remarks>
        Public Sub MakeThumbnail(ByVal originalImagePath As String, ByVal thumbnailPath As String, ByVal width As Integer, ByVal height As Integer, ByVal mode As ThumbnailMode)
            Dim originalImage = Drawing.Image.FromFile(originalImagePath)
            Dim towidth = width
            Dim toheight = height
            Dim x = 0
            Dim y = 0
            Dim ow = originalImage.Width
            Dim oh = originalImage.Height
            Select Case mode
                Case ThumbnailMode.HW
                    Exit Select
                Case ThumbnailMode.W
                    toheight = originalImage.Height * width / originalImage.Width
                    Exit Select
                Case ThumbnailMode.H
                    towidth = originalImage.Width * height / originalImage.Height
                    Exit Select
                Case ThumbnailMode.Cut
                    If CDbl(originalImage.Width) / CDbl(originalImage.Height) > CDbl(towidth) / CDbl(toheight) Then
                        oh = originalImage.Height
                        ow = originalImage.Height * towidth / toheight
                        y = 0
                        x = (originalImage.Width - ow) / 2
                    Else
                        ow = originalImage.Width
                        oh = originalImage.Width * height / towidth
                        x = 0
                        y = (originalImage.Height - oh) / 2
                    End If
                    Exit Select
                Case Else
                    Exit Select
            End Select
            Dim bitmap As New Drawing.Bitmap(towidth, toheight)
            Dim g = Drawing.Graphics.FromImage(bitmap)
            g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.High
            g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
            g.Clear(Drawing.Color.Transparent)
            g.DrawImage(originalImage, New Drawing.Rectangle(0, 0, towidth, toheight), New Drawing.Rectangle(x, y, ow, oh), Drawing.GraphicsUnit.Pixel)
            Try
                bitmap.Save(thumbnailPath, Drawing.Imaging.ImageFormat.Jpeg)
            Catch ex As Exception
                Throw New LSWFrameworkException(ex)
            Finally
                originalImage.Dispose()
                bitmap.Dispose()
                g.Dispose()
            End Try
        End Sub

        ''' <summary>
        ''' 在图片上增加文字水印
        ''' </summary>
        ''' <param name="Path">原服务器图片路径</param>
        ''' <param name="Path_sy">生成的带文字水印的图片路径</param>
        ''' <param name="Text">水印文字</param>
        ''' <remarks></remarks>
        Public Sub AddShuiYinWord(ByVal Path As String, ByVal Path_sy As String, ByVal Text As String)
            Dim image = Drawing.Image.FromFile(Path)
            Dim g = Drawing.Graphics.FromImage(image)
            g.DrawImage(image, 0, 0, image.Width, image.Height)
            Dim f As New Drawing.Font("Verdana", 16)
            Dim b As New Drawing.SolidBrush(Drawing.Color.Blue)
            g.DrawString(Text, f, b, 15, 15)
            g.Dispose()
            image.Save(Path_sy)
            image.Dispose()
        End Sub

        ''' <summary>
        ''' 在图片上增加文字水印
        ''' </summary>
        ''' <param name="source">原服务器图片路径</param>
        ''' <param name="Text">水印文字</param>
        ''' <remarks></remarks>
        <Extension()> _
        Public Function AddShuiYinWord(ByVal source As Bitmap, ByVal Text As String) As Bitmap
            Dim iwm As New ImageWaterMark
            iwm.addWatermarkText(source, Text)
            Return source
        End Function

        ''' <summary>
        ''' 在图片上生成图片水印
        ''' </summary>
        ''' <param name="Path">原服务器图片路径</param>
        ''' <param name="Path_syp">生成的带图片水印的图片路径</param>
        ''' <param name="Path_sypf">水印图片路径</param>
        ''' <remarks></remarks>
        Public Sub AddShuiYinPic(ByVal Path As String, ByVal Path_syp As String, ByVal Path_sypf As String)
            Dim image = Drawing.Image.FromFile(Path)
            Dim copyImage = System.Drawing.Image.FromFile(Path_sypf)
            Dim g = Drawing.Graphics.FromImage(image)
            g.DrawImage(copyImage, New Drawing.Rectangle(image.Width - copyImage.Width, image.Height - copyImage.Height, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, Drawing.GraphicsUnit.Pixel)
            g.Dispose()
            image.Save(Path_syp)
            image.Dispose()
        End Sub

        ''' <summary>
        ''' 在图片上生成图片水印
        ''' </summary>
        ''' <param name="sourceimage">原图片</param>
        ''' <param name="waterimage">水印图片</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function AddShuiYinPic(sourceimage As Drawing.Image, waterimage As Drawing.Image) As Drawing.Image
            Dim g = Drawing.Graphics.FromImage(sourceimage)
            g.DrawImage(waterimage, New Drawing.Rectangle(sourceimage.Width - waterimage.Width, sourceimage.Height - waterimage.Height, waterimage.Width, waterimage.Height), 0, 0, waterimage.Width, waterimage.Height, Drawing.GraphicsUnit.Pixel)
            g.Dispose()
            Return sourceimage
        End Function

        Public Function GetWebSiteThumbnail(Url As String, BrowserWidth As Integer, BrowserHeight As Integer) As Bitmap
            Return New WebSiteThumb(Url, BrowserWidth, BrowserHeight, 0, 0).GetWebSiteThumb
        End Function

        Public Function GetWebSiteThumbnail(Url As String, BrowserWidth As Integer, BrowserHeight As Integer, Percent As Integer) As Bitmap
            Return New WebSiteThumb(Url, BrowserWidth, BrowserHeight, BrowserWidth * Percent / 100, BrowserHeight * Percent / 100).GetWebSiteThumb
        End Function

        Public Function GetWebSiteThumbnail(Url As String, BrowserWidth As Integer, BrowserHeight As Integer, ThumbnailWidth As Integer, ThumbnailHeight As Integer) As Bitmap
            Return New WebSiteThumb(Url, BrowserWidth, BrowserHeight, ThumbnailWidth, ThumbnailHeight).GetWebSiteThumb
        End Function

        Public Function GetWebSiteFullThumbnail(Url As String) As Bitmap
            Return New WebSiteThumb(Url).GetWebSiteThumb
        End Function

        Public Function GetWebSiteFullThumbnail(Url As String, Percent As Integer) As Bitmap
            Return New WebSiteThumb(Url, Percent).GetWebSiteThumb
        End Function

        Public Function ScreenToBitmap() As Bitmap
            Try
                Dim DisplayDC As New IntPtr(CreateDC("DISPLAY", Nothing, Nothing, 0))
                Dim G1 As Graphics = Graphics.FromHdc(DisplayDC)
                Dim Bmp As New Bitmap(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height, G1)
                Dim G2 As Graphics = Graphics.FromImage(Bmp)
                Dim BmpDC As IntPtr = G2.GetHdc()
                BitBlt(BmpDC.ToInt32, 0, 0, My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height, DisplayDC.ToInt32, My.Computer.Screen.Bounds.Left, My.Computer.Screen.Bounds.Top, &HCC0020)
                G2.ReleaseHdc(BmpDC)
                DisplayDC = Nothing
                Return Bmp
            Catch ex As Exception
                Return New Bitmap(1, 1)
            End Try
        End Function

        ''' <summary>   
        ''' 指定窗口截图   
        ''' </summary>   
        ''' <param name="handle">窗口句柄. (在windows应用程序中, 从Handle属性获得)</param>   
        ''' <returns></returns>   
        Public Function CaptureWindow(handle As IntPtr) As Drawing.Image
            Dim hdcSrc As IntPtr = User32.GetWindowDC(handle)
            Dim windowRect As New RECT
            User32.GetWindowRect(handle, windowRect)
            Dim width As Integer = windowRect.right - windowRect.left
            Dim height As Integer = windowRect.bottom - windowRect.top
            Dim hdcDest As IntPtr = Gdi32.CreateCompatibleDC(hdcSrc)
            Dim hBitmap As IntPtr = Gdi32.CreateCompatibleBitmap(hdcSrc, width, height)
            Dim hOld As IntPtr = Gdi32.SelectObject(hdcDest, hBitmap)
            Gdi32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, _
                0, 0, Gdi32.SRCCOPY)
            Gdi32.SelectObject(hdcDest, hOld)
            Gdi32.DeleteDC(hdcDest)
            User32.ReleaseDC(handle, hdcSrc)
            Dim img As Drawing.Image = Drawing.Image.FromHbitmap(hBitmap)
            Gdi32.DeleteObject(hBitmap)
            Return img
        End Function

        Public Function GetBoundingRect(ByVal Points As List(Of Point)) As Rectangle
            Dim num As Integer = &HFFFFFFF
            Dim num2 As Integer = &HFFFFFFF
            Dim x As Integer = 0
            Dim y As Integer = 0
            Dim point As Point
            For Each point In Points
                If (point.X > x) Then
                    x = point.X
                End If
                If (point.X < num) Then
                    num = point.X
                End If
                If (point.Y > y) Then
                    y = point.Y
                End If
                If (point.Y < num2) Then
                    num2 = point.Y
                End If
            Next
            Return New Rectangle With { _
                .X = num, _
                .Width = ((x - num) + 1), _
                .Y = num2, _
                .Height = ((y - num2) + 1) _
            }
        End Function

        Public Function GetFeature(ByVal Points As List(Of Point), ByVal nVCount As Integer, ByVal nHCount As Integer) As List(Of Double)
            Dim list As New List(Of Double)
            Dim boundingRect As Rectangle = GetBoundingRect(Points)
            Dim list2 As List(Of Integer) = Split(boundingRect.Height, nVCount)
            Dim list3 As List(Of Integer) = Split(boundingRect.Width, nHCount)
            Dim list4 As New List(Of Rectangle)
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim i As Integer
            For i = 0 To list2.Count - 1
                num2 = 0
                Dim n As Integer
                For n = 0 To list3.Count - 1
                    list4.Add(New Rectangle((num2 + boundingRect.X), (num + boundingRect.Y), list3.Item(n), list2.Item(i)))
                    num2 = (num2 + list3.Item(n))
                Next n
                num = (num + list2.Item(i))
            Next i
            Dim j As Integer
            For j = 0 To list4.Count - 1
                list.Add(0)
            Next j
            Dim point As Point
            For Each point In Points
                'Dim list5 As List(Of Double)
                'Dim num10 As Integer
                Dim num6 As Integer = 0
                Do While (num6 < list4.Count)
                    Dim rectangle2 As Rectangle = list4.Item(num6)
                    If (point.X >= rectangle2.Left) Then
                        Dim rectangle3 As Rectangle = list4.Item(num6)
                        If (point.X < rectangle3.Right) Then
                            Dim rectangle4 As Rectangle = list4.Item(num6)
                            If (point.Y >= rectangle4.Top) Then
                                Dim rectangle5 As Rectangle = list4.Item(num6)
                                If (point.Y < rectangle5.Bottom) Then
                                    'list5 = list.Item(num10 = num6) = (list5.Item(num10) + 1)
                                    list(num6) += 1
                                    Continue For
                                End If
                            End If
                        End If
                    End If
                    num6 += 1
                Loop
                Continue For
            Next
            Dim num7 As Double = 0
            Dim k As Integer
            For k = 0 To list.Count - 1
                num7 = (num7 + list.Item(k))
            Next k
            If (num7 <> Points.Count) Then
                Throw New Exception("算法不正确")
            End If
            Dim m As Integer
            For m = 0 To list4.Count - 1
                Dim rectangle6 As Rectangle = list4.Item(m)
                Dim rectangle7 As Rectangle = list4.Item(m)
                list.Item(m) = (list.Item(m) / CDbl((rectangle6.Width * rectangle7.Height)))
            Next m
            Return list
        End Function

        Public Function Split(ByVal nTotal As Integer, ByVal nNumOfPiece As Integer) As List(Of Integer)
            Dim list As New List(Of Integer)
            Dim item As Integer = (nTotal / nNumOfPiece)
            Dim num2 As Integer = (nTotal Mod nNumOfPiece)
            Dim i As Integer
            For i = 0 To nNumOfPiece - 1
                list.Add(item)
            Next i
            Dim j As Integer
            For j = 0 To num2 - 1
                'Dim list2 As List(Of Integer)
                'Dim num5 As Integer
                'list2 = list.Item(num5 = j) = (list2.Item(num5) + 1)
                list(j) += 1
            Next j
            Return list
        End Function

        Public Function ProjectHorizontally(ByVal bm As Bitmap) As List(Of Double)
            Dim list As New List(Of Double)
            Dim width As Integer = bm.Width
            Dim height As Integer = bm.Height
            Dim i As Integer
            For i = 0 To width - 1
                list.Add(0)
                Dim k As Integer
                For k = 0 To height - 1
                    'Dim list2 As List(Of Double)
                    'Dim num8 As Integer
                    Dim pixel As Color = bm.GetPixel(i, k)
                    'list2 = list.Item(num8 = i) = (list2.Item(num8) + (&H2FD - ((pixel.R + pixel.G) + pixel.B)))
                    list(i) += (&H2FD - ((pixel.R + pixel.G) + pixel.B))
                Next k
            Next i
            Dim num5 As Double = (&H2FD * height)
            Dim count As Integer = list.Count
            Dim j As Integer
            For j = 0 To count - 1
                list.Item(j) = (list.Item(j) / num5)
            Next j
            Return list
        End Function

        Public Function ProjectVertically(ByVal bm As Bitmap) As List(Of Double)
            Dim list As New List(Of Double)
            Dim width As Integer = bm.Width
            Dim height As Integer = bm.Height
            Dim i As Integer
            For i = 0 To height - 1
                list.Add(0)
                Dim k As Integer
                For k = 0 To width - 1
                    'Dim list2 As List(Of Double)
                    'Dim num8 As Integer
                    Dim pixel As Color = bm.GetPixel(k, i)
                    'list2 = list.Item(num8 = i) = (list2.Item(num8) + (&H2FD - ((pixel.R + pixel.G) + pixel.B)))
                    list(i) += (&H2FD - ((pixel.R + pixel.G) + pixel.B))
                Next k
            Next i
            Dim num5 As Double = (&H2FD * width)
            Dim count As Integer = list.Count
            Dim j As Integer
            For j = 0 To count - 1
                list.Item(j) = (list.Item(j) / num5)
            Next j
            Return list
        End Function

        ''' <summary>
        ''' 图像灰度化
        ''' </summary>
        ''' <param name="bitmap"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ImageGray(bitmap As Bitmap) As Drawing.Image
            If bitmap IsNot Nothing Then
                Dim newbitmap = TryCast(bitmap.Clone(), Bitmap)
                Dim rect As New Rectangle(0, 0, newbitmap.Width, newbitmap.Height)
                Dim bmpdata As System.Drawing.Imaging.BitmapData = newbitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, newbitmap.PixelFormat)
                Dim ptr As IntPtr = bmpdata.Scan0
                Dim bytes As Integer = newbitmap.Width * newbitmap.Height * 3
                Dim rgbvalues As Byte() = New Byte(bytes - 1) {}
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbvalues, 0, bytes)
                Dim colortemp As Double = 0
                For i As Integer = 0 To rgbvalues.Length - 1 Step 3
                    colortemp = rgbvalues(i + 2) * 0.299 + rgbvalues(i + 1) * 0.587 + rgbvalues(i) * 0.114
                    rgbvalues(i) = InlineAssignHelper(rgbvalues(i + 1), InlineAssignHelper(rgbvalues(i + 2), CByte(System.Math.Truncate(colortemp))))
                Next
                System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr, bytes)
                newbitmap.UnlockBits(bmpdata)
                Return TryCast(newbitmap.Clone(), Drawing.Image)
            End If
            Return Nothing
        End Function

#Region "伪彩色图像处理"

        ''' <summary>
        ''' 伪彩色图像处理
        ''' </summary>
        ''' <param name="bmp">传入的灰度图像</param>
        ''' <param name="method">使用何种方法，false强度分层法,true灰度级-彩色变换法</param>
        ''' <param name="seg">强度分层中的分层数</param>
        ''' <returns>返回伪彩色图像</returns>
        Public Function gcTrans(bmp As Bitmap, method As Boolean, seg As Byte) As Bitmap
            If bmp IsNot Nothing Then
                If System.Drawing.Imaging.PixelFormat.Format24bppRgb = bmp.PixelFormat Then
                    Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
                    Dim bmpData As System.Drawing.Imaging.BitmapData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat)
                    Dim ptr As IntPtr = bmpData.Scan0
                    Dim bytes As Integer = bmp.Width * bmp.Height * 3
                    Dim grayValues As Byte() = New Byte(bytes - 1) {}
                    System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes)
                    bmp.UnlockBits(bmpData)
                    Dim rgbValues As Byte() = New Byte(bytes - 1) {}
                    '清零
                    Array.Clear(rgbValues, 0, bytes)
                    Dim tempB As Byte
                    If method = False Then
                        '强度分层法
                        For i As Integer = 0 To bytes - 1 Step 3
                            Dim ser As Byte = CByte(256 \ seg)
                            tempB = CByte(grayValues(i) \ ser)
                            '分配任意一种颜色
                            rgbValues(i + 1) = CByte(tempB * ser)
                            rgbValues(i) = CByte((seg - 1 - tempB) * ser)
                            rgbValues(i + 2) = 0
                        Next
                    Else
                        '灰度级-彩色变换法
                        For i As Integer = 0 To bytes - 1 Step 3
                            If grayValues(i) < 64 Then
                                rgbValues(i + 2) = 0
                                rgbValues(i + 1) = CByte(4 * grayValues(i))
                                rgbValues(i) = 255
                            ElseIf grayValues(i) < 128 Then
                                rgbValues(i + 2) = 0
                                rgbValues(i + 1) = 255
                                rgbValues(i) = CByte(-4 * grayValues(i) + 2 * 255)
                            ElseIf grayValues(i) < 192 Then
                                rgbValues(i + 2) = CByte(4 * grayValues(i) - 2 * 255)
                                rgbValues(i + 1) = 255
                                rgbValues(i) = 0
                            Else
                                rgbValues(i + 2) = 255
                                rgbValues(i + 1) = CByte(-4 * grayValues(i) + 4 * 255)
                                rgbValues(i) = 0
                            End If
                        Next
                    End If
                    bmp = New Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat)
                    ptr = bmpData.Scan0
                    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes)
                    bmp.UnlockBits(bmpData)
                    Return bmp
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function
#End Region

    End Module
End Namespace
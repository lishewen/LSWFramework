Imports LSW.Win32
Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Image
    Public Module IconHelper
        ''' <summary>
        ''' 依据文件名读取图标，若指定文件不存在，则返回空值。
        ''' </summary>
        ''' <param name="fileName">文件路径</param>
        ''' <param name="isLarge">是否返回大图标</param>
        ''' <param name="nIconIndex">图标索引</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIcon(fileName As String, Optional isLarge As Boolean = True, Optional nIconIndex As Integer = 0) As Icon
            Try
                Dim phiconLarge(0), phiconSmall(0) As IntPtr
                Shell32.ExtractIconEx(fileName, nIconIndex, phiconLarge, phiconSmall, 1)
                Dim IconHnd As IntPtr = IIf(isLarge, phiconLarge(0), phiconSmall(0))
                Return Icon.FromHandle(IconHnd)
            Catch ex As Exception
                Return Icon.ExtractAssociatedIcon(fileName)
            End Try
        End Function

        ''' <summary>
        ''' 获取大图标，并添加到一个ImageList中！
        ''' </summary>
        ''' <param name="filename"></param>
        ''' <returns></returns>
        Public Function GetIcons(filename As String, ByRef imLargeIcons As List(Of Icon), ByRef imSmallIcons As List(Of Icon)) As UInteger
            '清空imageList
            imLargeIcons.Clear()
            imSmallIcons.Clear()
            '获取图标数量！
            Dim count As UInteger = Shell32.ExtractIconEx(filename, -1, Nothing, Nothing, 0)
            If count > 0 Then
                Dim handlesLarge As IntPtr() = New IntPtr(count - 1) {}
                Dim handlesSmall As IntPtr() = New IntPtr(count - 1) {}
                '获取图标
                Shell32.ExtractIconEx(filename, 0, handlesLarge, handlesSmall, count)
                '把图标添加到imageList！
                For i As Integer = 0 To count - 1
                    Dim iconlarge As Icon = Icon.FromHandle(handlesLarge(i))
                    Dim iconSmall As Icon = Icon.FromHandle(handlesSmall(i))
                    imLargeIcons.Add(DirectCast(iconlarge.Clone(), Icon))
                    imSmallIcons.Add(DirectCast(iconSmall.Clone(), Icon))
                    '销毁相应的图标！
                    User32.DestroyIcon(handlesLarge(i))
                    User32.DestroyIcon(handlesSmall(i))
                Next
            End If
            Return count
        End Function

        '仅包含一个图像的Icon文件头
        Private m_Header As Byte() = {CByte(0), CByte(0), CByte(1), CByte(0), CByte(1), CByte(0)}

        ''' <summary>
        ''' 写入icon image
        ''' </summary>
        Private Sub WriteIconImage(bm As Bitmap, bw As BinaryWriter)
            '在ICON文件中使用的关键变量只用到：bisize, biwidth, biheight, biplanes, bibitcount, bisizeimage.几个,其他变量必须为0。
            'biheight变量的值为高度象素量的2倍。

            '写入BitmapHeaderInfo结构
            Dim biSize As Int32 = 40
            'BitmapHeaderInfo结构的size
            Dim biWidth As Int32 = bm.Width
            Dim biHeight As Int32 = bm.Height * 2
            Dim biPlanes As Int16 = 1
            Dim biBitCount As Int16 = 24
            Dim biCompression As Int32 = 0
            Dim biSizeImage As Int32 = 0
            '固定为0
            Dim biXPelsPerMeter As Int32 = 0
            Dim biYPelsPerMeter As Int32 = 0
            Dim biClrUsed As Int32 = 0
            Dim biClrImportant As Int32 = 0

            bw.Write(biSize)
            bw.Write(biWidth)
            bw.Write(biHeight)
            bw.Write(biPlanes)
            bw.Write(biBitCount)
            bw.Write(biCompression)
            bw.Write(biSizeImage)
            bw.Write(biXPelsPerMeter)
            bw.Write(biYPelsPerMeter)
            bw.Write(biClrUsed)
            bw.Write(biClrImportant)

            '对于bpp=32的真彩色图标来说，没有RGBQUAD这个段！在xor中是实际色彩！！！
            '这里是RGBQUAD段-（真实色彩图标缺此段)

            '写入byte[] XOR Mask
            Dim stride As Integer = (bm.Width * 24 + 31) / 32 * 4
            For j As Integer = bm.Height - 1 To 0 Step -1
                For index As Integer = 0 To bm.Width - 1
                    Dim pixelColor As Color = bm.GetPixel(index, j)
                    bw.Write(pixelColor.B)
                    bw.Write(pixelColor.G)
                    bw.Write(pixelColor.R)
                Next
                '行尾补齐
                For index As Integer = bm.Width * 3 To stride - 1
                    bw.Write(CByte(0))
                Next
            Next

            '锁定内存
            '
            '			BitmapData bmData1 = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);	
            '			System.IntPtr scan0 = bmData1.Scan0;	//第一个像素数据的地址
            '			int stride = bmData1.Stride;
            '
            '			//写入图像数据！！！
            '			unsafe
            '			{
            '				byte* p = (byte*)(void*)scan0;
            '				for (int j = bm.Height - 1; j >= 0; j--)
            '				{
            '					for (int i = 0; i < stride; i++)
            '					{
            '						bw.Write(p[j * stride + i]);
            '					}
            '				}
            '			}
            '			bm.UnlockBits(bmData1);
            '			 * 



            '写入AND Mask ， 1 bpp
            stride = (1 * bm.Width + 31) / 32 * 4
            'and mask 以DWORD对齐后的扫描行宽度
            Dim line As Byte() = New Byte(stride - 1) {}

            For j As Integer = bm.Height - 1 To 0 Step -1
                For i As Integer = 0 To stride - 1
                    line(i) = 0
                Next

                '处理当前扫描行
                For i As Integer = 0 To bm.Width - 1
                    Dim color As Color = bm.GetPixel(i, j)
                    Dim colorsum As Integer = color.R + color.G + color.B
                    If colorsum = 0 Then
                        '在位图中是黑色说明该像素应该是透明的
                        'i&7: 相当于i%8
                        line(i \ 8) = line(i \ 8) Or CByte(1 << (7 - (i And &H7)))
                    End If
                Next
                bw.Write(line, 0, stride)
            Next
        End Sub

        ''' <summary>
        ''' 写一个目录信息！然后在目录后写入icon的图像数据和AND数据
        ''' </summary>
        Private Sub WriteDirEntry(icon As Icon, bw As BinaryWriter)
            Dim bm As Bitmap = icon.ToBitmap()
            WriteDirEntry(bm, bw)
        End Sub

        Private Sub WriteDirEntry(bm As Bitmap, bw As BinaryWriter)
            'BitmapData bmData1 = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);	//每个像素占3个字节（24位）

            Dim bpp1 As Integer = 24
            '真彩色位图的bpp
            Dim bpp2 As Integer = 1
            '蒙版的bpp
            Dim stride1 As Integer = (bm.Width * bpp1 + 31) / 32 * 4
            Dim stride2 As Integer = (bm.Width * bpp2 + 31) / 32 * 4

            Dim bwidth As Byte = CByte(bm.Width)
            Dim bheight As Byte = CByte(bm.Height)
            Dim bcolorcount As Byte = CByte(0)
            '对于真彩图标32bpp来说，这个字段为0！
            Dim breserved As Byte = CByte(0)
            Dim wplanes As Int16 = 1
            Dim wbitcount As Int16 = CType(bpp1, Int16)
            '8bit per pixel, 16 colors!
            Dim dwbytesInRes As Int32 = 40 + (stride1 + stride2) * bm.Height
            '40是BitmapInfoHeader的尺寸
            Dim dwImageOffset As Int32 = 22
            '???紧跟着就是第一个图像的入口！就是下一个字节的文件地址 6bytes header+ 16bytes entry
            '解锁
            'bm.UnlockBits(bmData1);

            '写入
            bw.Write(bwidth)
            bw.Write(bheight)
            bw.Write(bcolorcount)
            bw.Write(breserved)
            bw.Write(wplanes)
            bw.Write(wbitcount)
            bw.Write(dwbytesInRes)
            bw.Write(dwImageOffset)

            WriteIconImage(bm, bw)
        End Sub

        <Extension>
        Public Sub SaveIcon(ico As Icon, filename As String)
            Using fs As New FileStream(filename, FileMode.Create)
                Using bw As New BinaryWriter(fs)
                    bw.Write(m_Header)
                    WriteDirEntry(ico, bw)
                    bw.Close()
                End Using
            End Using
        End Sub
    End Module
End Namespace
Imports System.Drawing
Imports System.IO
Imports LSW.Extension

Namespace Image.Gif
    Public Class GifEncoder
        Protected width As Integer
        ' image size
        Protected height As Integer
        Protected transparent As Color = Color.Empty
        ' transparent color if given
        Protected transIndex As Integer
        ' transparent index in color table
        Protected repeat As Integer = -1
        ' no repeat
        Protected delay As Integer = 0
        ' frame delay (hundredths)
        Protected started As Boolean = False
        ' ready to output frames
        '	protected BinaryWriter bw;
        Protected fs As FileStream

        Protected image As Drawing.Image
        ' current frame
        Protected pixels As Byte()
        ' BGR byte array from frame
        Protected indexedPixels As Byte()
        ' converted frame indexed to palette
        Protected colorDepth As Integer
        ' number of bit planes
        Protected colorTab As Byte()
        ' RGB palette
        Protected usedEntry As Boolean() = New Boolean(255) {}
        ' active palette entries
        Protected palSize As Integer = 7
        ' color table size (bits-1)
        Protected dispose As Integer = -1
        ' disposal code (-1 = use default)
        Protected closeStream As Boolean = False
        ' close stream when finished
        Protected firstFrame As Boolean = True
        Protected sizeSet As Boolean = False
        ' if false, get size from first frame
        Protected sample As Integer = 10
        ' default sample interval for quantizer

        Public Sub SetDelay(ms As Integer)
            delay = System.Math.Round(ms / 10.0F)
        End Sub

        Public Sub SetDispose(code As Integer)
            If code >= 0 Then
                dispose = code
            End If
        End Sub

        Public Sub SetRepeat(iter As Integer)
            If iter >= 0 Then
                repeat = iter
            End If
        End Sub

        Public Sub SetTransparent(c As Color)
            transparent = c
        End Sub

        Public Function AddFrame(im As Drawing.Image) As Boolean
            If (im Is Nothing) OrElse Not started Then
                Return False
            End If
            Dim ok As Boolean = True
            Try
                If Not sizeSet Then
                    ' use first frame's size
                    SetSize(im.Width, im.Height)
                End If
                image = im
                GetImagePixels()
                ' convert to correct format if necessary
                AnalyzePixels()
                ' build color table & map pixels
                If firstFrame Then
                    WriteLSD()
                    ' logical screen descriptior
                    WritePalette()
                    ' global color table
                    If repeat >= 0 Then
                        ' use NS app extension to indicate reps
                        WriteNetscapeExt()
                    End If
                End If
                WriteGraphicCtrlExt()
                ' write graphic control extension
                WriteImageDesc()
                ' image descriptor
                If Not firstFrame Then
                    ' local color table
                    WritePalette()
                End If
                WritePixels()
                ' encode and write pixel data
                firstFrame = False
            Catch e As IOException
                ok = False
            End Try

            Return ok
        End Function

        Public Function Finish() As Boolean
            If Not started Then
                Return False
            End If
            Dim ok As Boolean = True
            started = False
            Try
                fs.WriteByte(&H3B)
                ' gif trailer
                fs.Flush()
                If closeStream Then
                    fs.Close()
                End If
            Catch e As IOException
                ok = False
            End Try

            ' reset for subsequent use
            transIndex = 0
            fs = Nothing
            image = Nothing
            pixels = Nothing
            indexedPixels = Nothing
            colorTab = Nothing
            closeStream = False
            firstFrame = True

            Return ok
        End Function

        Public Sub SetFrameRate(fps As Single)
            If fps <> 0.0F Then
                delay = CInt(System.Math.Round(100.0F / fps))
            End If
        End Sub

        Public Sub SetQuality(quality As Integer)
            If quality < 1 Then
                quality = 1
            End If
            sample = quality
        End Sub

        Public Sub SetSize(w As Integer, h As Integer)
            If started AndAlso Not firstFrame Then
                Return
            End If
            width = w
            height = h
            If width < 1 Then
                width = 320
            End If
            If height < 1 Then
                height = 240
            End If
            sizeSet = True
        End Sub

        Public Function Start(os As FileStream) As Boolean
            If os Is Nothing Then
                Return False
            End If
            Dim ok As Boolean = True
            closeStream = False
            fs = os
            Try
                ' header
                WriteString("GIF89a")
            Catch e As IOException
                ok = False
            End Try
            Return started.InlineAssignHelper(ok)
        End Function

        Public Function Start(file As String) As Boolean
            Dim ok As Boolean = True
            Try
                '			bw = new BinaryWriter( new FileStream( file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None ) );
                fs = New FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
                ok = Start(fs)
                closeStream = True
            Catch e As IOException
                ok = False
            End Try
            Return started.InlineAssignHelper(ok)
        End Function

        Protected Sub AnalyzePixels()
            Dim len As Integer = pixels.Length
            Dim nPix As Integer = len \ 3
            indexedPixels = New Byte(nPix - 1) {}
            Dim nq As New NeuQuant(pixels, len, sample)
            ' initialize quantizer
            colorTab = nq.Process()
            ' create reduced palette
            ' convert map from BGR to RGB
            '			for (int i = 0; i < colorTab.Length; i += 3) 
            '			{
            '				byte temp = colorTab[i];
            '				colorTab[i] = colorTab[i + 2];
            '				colorTab[i + 2] = temp;
            '				usedEntry[i / 3] = false;
            '			}
            ' map image pixels to new palette
            Dim k As Integer = 0
            For i As Integer = 0 To nPix - 1
                Dim index As Integer = nq.Map(pixels(System.Math.Min(System.Threading.Interlocked.Increment(k), k - 1)) And &HFF, pixels(System.Math.Min(System.Threading.Interlocked.Increment(k), k - 1)) And &HFF, pixels(System.Math.Min(System.Threading.Interlocked.Increment(k), k - 1)) And &HFF)
                usedEntry(index) = True
                indexedPixels(i) = CByte(index)
            Next
            pixels = Nothing
            colorDepth = 8
            palSize = 7
            ' get closest match to transparent color if specified
            If transparent <> Color.Empty Then
                transIndex = FindClosest(transparent)
            End If
        End Sub

        Protected Function FindClosest(c As Color) As Integer
            If colorTab Is Nothing Then
                Return -1
            End If
            Dim r As Integer = c.R
            Dim g As Integer = c.G
            Dim b As Integer = c.B
            Dim minpos As Integer = 0
            Dim dmin As Integer = 256 * 256 * 256
            Dim len As Integer = colorTab.Length
            Dim i As Integer = 0
            While i < len
                Dim dr As Integer = r - (colorTab(i) And &HFF)
                i += 1
                Dim dg As Integer = g - (colorTab(i) And &HFF)
                i += 1
                Dim db As Integer = b - (colorTab(i) And &HFF)
                Dim d As Integer = dr * dr + dg * dg + db * db
                Dim index As Integer = i \ 3
                If usedEntry(index) AndAlso (d < dmin) Then
                    dmin = d
                    minpos = index
                End If
                i += 1
            End While
            Return minpos
        End Function

        Protected Sub GetImagePixels()
            Dim w As Integer = image.Width
            Dim h As Integer = image.Height
            '		int type = image.GetType().;
            If (w <> width) OrElse (h <> height) Then
                ' create new image with right size/format
                Dim temp As Drawing.Image = New Bitmap(width, height)
                Dim g As Graphics = Graphics.FromImage(temp)
                g.DrawImage(image, 0, 0)
                image = temp
                g.Dispose()
            End If
            '
            '				ToDo:
            '				improve performance: use unsafe code 
            '			

            pixels = New Byte(3 * image.Width * image.Height - 1) {}
            Dim count As Integer = 0
            Dim tempBitmap As New Bitmap(image)
            For th As Integer = 0 To image.Height - 1
                For tw As Integer = 0 To image.Width - 1
                    Dim color As Color = tempBitmap.GetPixel(tw, th)
                    pixels(count) = color.R
                    count += 1
                    pixels(count) = color.G
                    count += 1
                    pixels(count) = color.B
                    count += 1
                Next
            Next

            '		pixels = ((DataBufferByte) image.getRaster().getDataBuffer()).getData();
        End Sub

        Protected Sub WriteGraphicCtrlExt()
            fs.WriteByte(&H21)
            ' extension introducer
            fs.WriteByte(&HF9)
            ' GCE label
            fs.WriteByte(4)
            ' data block size
            Dim transp As Integer, disp As Integer
            If transparent = Color.Empty Then
                transp = 0
                ' dispose = no action
                disp = 0
            Else
                transp = 1
                ' force clear if using transparent color
                disp = 2
            End If
            If dispose >= 0 Then
                ' user override
                disp = dispose And 7
            End If
            disp <<= 2

            ' packed fields
            ' 1:3 reserved
            ' 4:6 disposal
            ' 7   user input - 0 = none
            fs.WriteByte(Convert.ToByte(0 Or disp Or 0 Or transp))
            ' 8   transparency flag
            WriteShort(delay)
            ' delay x 1/100 sec
            fs.WriteByte(Convert.ToByte(transIndex))
            ' transparent color index
            fs.WriteByte(0)
            ' block terminator
        End Sub

        Protected Sub WriteImageDesc()
            fs.WriteByte(&H2C)
            ' image separator
            WriteShort(0)
            ' image position x,y = 0,0
            WriteShort(0)
            WriteShort(width)
            ' image size
            WriteShort(height)
            ' packed fields
            If firstFrame Then
                ' no LCT  - GCT is used for first (or only) frame
                fs.WriteByte(0)
            Else
                ' specify normal LCT
                ' 1 local color table  1=yes
                ' 2 interlace - 0=no
                ' 3 sorted - 0=no
                ' 4-5 reserved
                ' 6-8 size of color table
                fs.WriteByte(Convert.ToByte(&H80 Or 0 Or 0 Or 0 Or palSize))
            End If
        End Sub

        Protected Sub WriteLSD()
            ' logical screen size
            WriteShort(width)
            WriteShort(height)
            ' packed fields
            ' 1   : global color table flag = 1 (gct used)
            ' 2-4 : color resolution = 7
            ' 5   : gct sort flag = 0
            fs.WriteByte(Convert.ToByte(&H80 Or &H70 Or &H0 Or palSize))
            ' 6-8 : gct size
            fs.WriteByte(0)
            ' background color index
            fs.WriteByte(0)
            ' pixel aspect ratio - assume 1:1
        End Sub

        Protected Sub WriteNetscapeExt()
            fs.WriteByte(&H21)
            ' extension introducer
            fs.WriteByte(&HFF)
            ' app extension label
            fs.WriteByte(11)
            ' block size
            WriteString("NETSCAPE" & "2.0")
            ' app id + auth code
            fs.WriteByte(3)
            ' sub-block size
            fs.WriteByte(1)
            ' loop sub-block id
            WriteShort(repeat)
            ' loop count (extra iterations, 0=repeat forever)
            fs.WriteByte(0)
            ' block terminator
        End Sub

        Protected Sub WritePalette()
            fs.Write(colorTab, 0, colorTab.Length)
            Dim n As Integer = (3 * 256) - colorTab.Length
            For i As Integer = 0 To n - 1
                fs.WriteByte(0)
            Next
        End Sub

        Protected Sub WritePixels()
            Dim encoder As New LZWEncoder(width, height, indexedPixels, colorDepth)
            encoder.Encode(fs)
        End Sub

        Protected Sub WriteShort(value As Integer)
            fs.WriteByte(Convert.ToByte(value And &HFF))
            fs.WriteByte(Convert.ToByte((value >> 8) And &HFF))
        End Sub

        Protected Sub WriteString(s As [String])
            Dim chars As Char() = s.ToCharArray()
            For i As Integer = 0 To chars.Length - 1
                fs.WriteByte(CByte(AscW(chars(i))))
            Next
        End Sub
    End Class
End Namespace
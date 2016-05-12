Imports System.IO
Imports System.Drawing
Imports LSW.Extension
Imports LSW.Exceptions

Namespace Image.Gif
    Public Class GifDecoder
        Public Shared ReadOnly STATUS_OK As Integer = 0
        Public Shared ReadOnly STATUS_FORMAT_ERROR As Integer = 1
        Public Shared ReadOnly STATUS_OPEN_ERROR As Integer = 2

        Protected inStream As Stream
        Protected status As Integer

        Protected width As Integer
        ' full image width
        Protected height As Integer
        ' full image height
        Protected gctFlag As Boolean
        ' global color table used
        Protected gctSize As Integer
        ' size of global color table
        Protected loopCount As Integer = 1
        ' iterations; 0 = repeat forever
        Protected gct As Integer()
        ' global color table
        Protected lct As Integer()
        ' local color table
        Protected act As Integer()
        ' active color table
        Protected bgIndex As Integer
        ' background color index
        Protected bgColor As Integer
        ' background color
        Protected lastBgColor As Integer
        ' previous bg color
        Protected pixelAspect As Integer
        ' pixel aspect ratio
        Protected lctFlag As Boolean
        ' local color table flag
        Protected interlace As Boolean
        ' interlace flag
        Protected lctSize As Integer
        ' local color table size
        Protected ix As Integer, iy As Integer, iw As Integer, ih As Integer
        ' current image rectangle
        Protected lastRect As Rectangle
        ' last image rect
        Protected image As Drawing.Image
        ' current frame
        Protected bitmap As Bitmap
        Protected lastImage As Drawing.Image
        ' previous frame
        Protected block As Byte() = New Byte(255) {}
        ' current data block
        Protected blockSize As Integer = 0
        ' block size
        ' last graphic control extension info
        Protected dispose As Integer = 0
        ' 0=no action; 1=leave in place; 2=restore to bg; 3=restore to prev
        Protected lastDispose As Integer = 0
        Protected transparency As Boolean = False
        ' use transparent color
        Protected delay As Integer = 0
        ' delay in milliseconds
        Protected transIndex As Integer
        ' transparent color index
        Protected Shared ReadOnly MaxStackSize As Integer = 4096
        ' max decoder pixel stack size

        ' LZW decoder working arrays
        Protected prefix As Short()
        Protected suffix As Byte()
        Protected pixelStack As Byte()
        Protected pixels As Byte()

        Protected frames As ArrayList
        ' frames read from current file
        Protected frameCount As Integer

        Public Class GifFrame
            Public Sub New(im As Drawing.Image, del As Integer)
                image = im
                delay = del
            End Sub
            Public image As Drawing.Image
            Public delay As Integer
        End Class

        Public Function GetDelay(n As Integer) As Integer
            '
            delay = -1
            If (n >= 0) AndAlso (n < frameCount) Then
                delay = DirectCast(frames(n), GifFrame).delay
            End If
            Return delay
        End Function

        Public Function GetFrameCount() As Integer
            Return frameCount
        End Function

        Public Function GetImage() As Drawing.Image
            Return GetFrame(0)
        End Function

        Public Function GetLoopCount() As Integer
            Return loopCount
        End Function

        Private Function GetPixels(bitmap As Bitmap) As Integer()
            Dim pixels As Integer() = New Integer(3 * image.Width * image.Height - 1) {}
            Dim count As Integer = 0
            For th As Integer = 0 To image.Height - 1
                For tw As Integer = 0 To image.Width - 1
                    Dim color As Color = bitmap.GetPixel(tw, th)
                    pixels(count) = color.R
                    count += 1
                    pixels(count) = color.G
                    count += 1
                    pixels(count) = color.B
                    count += 1
                Next
            Next
            Return pixels
        End Function

        Private Sub SetPixels(pixels As Integer())
            Dim count As Integer = 0
            For th As Integer = 0 To image.Height - 1
                For tw As Integer = 0 To image.Width - 1
                    Dim color__1 As Color = Color.FromArgb(pixels(count))
                    count += 1
                    bitmap.SetPixel(tw, th, color__1)
                Next
            Next
        End Sub

        Protected Sub SetPixels()
            Dim dest As Integer() = GetPixels(bitmap)

            ' fill in starting image contents based on last image's dispose code
            If lastDispose > 0 Then
                If lastDispose = 3 Then
                    ' use image before last
                    Dim n As Integer = frameCount - 2
                    If n > 0 Then
                        lastImage = GetFrame(n - 1)
                    Else
                        lastImage = Nothing
                    End If
                End If

                If lastImage IsNot Nothing Then
                    '				int[] prev =
                    '					((DataBufferInt) lastImage.getRaster().getDataBuffer()).getData();
                    Dim prev As Integer() = GetPixels(New Bitmap(lastImage))
                    Array.Copy(prev, 0, dest, 0, width * height)
                    ' copy pixels

                    If lastDispose = 2 Then
                        ' fill last image rect area with background color
                        Dim g As Graphics = Graphics.FromImage(image)
                        Dim c As Color = Color.Empty
                        If transparency Then
                            ' assume background is transparent
                            c = Color.FromArgb(0, 0, 0, 0)
                        Else
                            '						c = new Color(lastBgColor); // use given background color
                            c = Color.FromArgb(lastBgColor)
                        End If
                        Dim brush As Brush = New SolidBrush(c)
                        g.FillRectangle(brush, lastRect)
                        brush.Dispose()
                        g.Dispose()
                    End If
                End If
            End If

            ' copy each source line to the appropriate place in the destination
            Dim pass As Integer = 1
            Dim inc As Integer = 8
            Dim iline As Integer = 0
            For i As Integer = 0 To ih - 1
                Dim line As Integer = i
                If interlace Then
                    If iline >= ih Then
                        pass += 1
                        Select Case pass
                            Case 2
                                iline = 4
                                Exit Select
                            Case 3
                                iline = 2
                                inc = 4
                                Exit Select
                            Case 4
                                iline = 1
                                inc = 2
                                Exit Select
                        End Select
                    End If
                    line = iline
                    iline += inc
                End If
                line += iy
                If line < height Then
                    Dim k As Integer = line * width
                    Dim dx As Integer = k + ix
                    ' start of line in dest
                    Dim dlim As Integer = dx + iw
                    ' end of dest line
                    If (k + width) < dlim Then
                        ' past dest edge
                        dlim = k + width
                    End If
                    Dim sx As Integer = i * iw
                    ' start of line in source
                    While dx < dlim
                        ' map color and insert in destination
                        Dim index As Integer = CInt(pixels(sx)) And &HFF
                        sx += 1
                        Dim c As Integer = act(index)
                        If c <> 0 Then
                            dest(dx) = c
                        End If
                        dx += 1
                    End While
                End If
            Next
            SetPixels(dest)
        End Sub

        Public Function GetFrame(n As Integer) As Drawing.Image
            Dim im As Drawing.Image = Nothing
            If (n >= 0) AndAlso (n < frameCount) Then
                im = DirectCast(frames(n), GifFrame).image
            End If
            Return im
        End Function

        Public Function GetFrameSize() As Size
            Return New Size(width, height)
        End Function

        Public Function Read(inStream As Stream) As Integer
            Init()
            If inStream IsNot Nothing Then
                Me.inStream = inStream
                ReadHeader()
                If Not [Error]() Then
                    ReadContents()
                    If frameCount < 0 Then
                        status = STATUS_FORMAT_ERROR
                    End If
                End If
                inStream.Close()
            Else
                status = STATUS_OPEN_ERROR
            End If
            Return status
        End Function

        Public Function Read(name As [String]) As Integer
            status = STATUS_OK
            Try
                name = name.Trim().ToLower()
                status = Read(New FileInfo(name).OpenRead())
            Catch e As IOException
                status = STATUS_OPEN_ERROR
            End Try

            Return status
        End Function

        Protected Sub DecodeImageData()
            Dim NullCode As Integer = -1
            Dim npix As Integer = iw * ih
            Dim available As Integer, clear As Integer, code_mask As Integer, code_size As Integer, end_of_information As Integer, in_code As Integer, _
                old_code As Integer, bits As Integer, code As Integer, count As Integer, i As Integer, datum As Integer, _
                data_size As Integer, first As Integer, top As Integer, bi As Integer, pi As Integer

            If (pixels Is Nothing) OrElse (pixels.Length < npix) Then
                ' allocate new pixel array
                pixels = New Byte(npix - 1) {}
            End If
            If prefix Is Nothing Then
                prefix = New Short(MaxStackSize - 1) {}
            End If
            If suffix Is Nothing Then
                suffix = New Byte(MaxStackSize - 1) {}
            End If
            If pixelStack Is Nothing Then
                pixelStack = New Byte(MaxStackSize) {}
            End If

            '  Initialize GIF data stream decoder.

            data_size = Read()
            clear = 1 << data_size
            end_of_information = clear + 1
            available = clear + 2
            old_code = NullCode
            code_size = data_size + 1
            code_mask = (1 << code_size) - 1
            For code = 0 To clear - 1
                prefix(code) = 0
                suffix(code) = CByte(code)
            Next

            '  Decode GIF pixel stream.

            datum = TypeHelper.InlineAssignHelper(bits, TypeHelper.InlineAssignHelper(count, TypeHelper.InlineAssignHelper(first, TypeHelper.InlineAssignHelper(top, TypeHelper.InlineAssignHelper(pi, TypeHelper.InlineAssignHelper(bi, 0))))))

            i = 0
            While i < npix
                If top = 0 Then
                    If bits < code_size Then
                        '  Load bytes until there are enough bits for a code.
                        If count = 0 Then
                            ' Read a new data block.
                            count = ReadBlock()
                            If count <= 0 Then
                                Exit While
                            End If
                            bi = 0
                        End If
                        datum += (CInt(block(bi)) And &HFF) << bits
                        bits += 8
                        bi += 1
                        count -= 1
                        Continue While
                    End If

                    '  Get the next code.

                    code = datum And code_mask
                    datum >>= code_size
                    bits -= code_size

                    '  Interpret the code

                    If (code > available) OrElse (code = end_of_information) Then
                        Exit While
                    End If
                    If code = clear Then
                        '  Reset decoder.
                        code_size = data_size + 1
                        code_mask = (1 << code_size) - 1
                        available = clear + 2
                        old_code = NullCode
                        Continue While
                    End If
                    If old_code = NullCode Then
                        pixelStack(top) = suffix(code)
                        top += 1
                        old_code = code
                        first = code
                        Continue While
                    End If
                    in_code = code
                    If code = available Then
                        pixelStack(top) = CByte(first)
                        top += 1
                        code = old_code
                    End If
                    While code > clear
                        pixelStack(top) = suffix(code)
                        top += 1
                        code = prefix(code)
                    End While
                    first = CInt(suffix(code)) And &HFF

                    '  Add a new string to the string table,

                    If available >= MaxStackSize Then
                        Exit While
                    End If
                    pixelStack(top) = CByte(first)
                    top += 1
                    prefix(available) = CShort(old_code)
                    suffix(available) = CByte(first)
                    available += 1
                    If ((available And code_mask) = 0) AndAlso (available < MaxStackSize) Then
                        code_size += 1
                        code_mask += available
                    End If
                    old_code = in_code
                End If

                '  Pop a pixel off the pixel stack.

                top -= 1
                pixels(pi) = pixelStack(top)
                pi += 1
                i += 1
            End While

            For i = pi To npix - 1
                ' clear missing pixels
                pixels(i) = 0
            Next
        End Sub

        Protected Function [Error]() As Boolean
            Return status <> STATUS_OK
        End Function

        Protected Sub Init()
            status = STATUS_OK
            frameCount = 0
            frames = New ArrayList()
            gct = Nothing
            lct = Nothing
        End Sub

        Protected Function Read() As Integer
            Dim curByte As Integer = 0
            Try
                curByte = inStream.ReadByte()
            Catch e As IOException
                status = STATUS_FORMAT_ERROR
            End Try
            Return curByte
        End Function

        Protected Function ReadBlock() As Integer
            blockSize = Read()
            Dim n As Integer = 0
            If blockSize > 0 Then
                Try
                    Dim count As Integer = 0
                    While n < blockSize
                        count = inStream.Read(block, n, blockSize - n)
                        If count = -1 Then
                            Exit While
                        End If
                        n += count
                    End While
                Catch e As IOException
                End Try

                If n < blockSize Then
                    status = STATUS_FORMAT_ERROR
                End If
            End If
            Return n
        End Function

        Protected Function ReadColorTable(ncolors As Integer) As Integer()
            Dim nbytes As Integer = 3 * ncolors
            Dim tab As Integer() = Nothing
            Dim c As Byte() = New Byte(nbytes - 1) {}
            Dim n As Integer = 0
            Try
                n = inStream.Read(c, 0, c.Length)
            Catch e As IOException
                Dim ex As New LSWFrameworkException(e)
            End Try
            If n < nbytes Then
                status = STATUS_FORMAT_ERROR
            Else
                tab = New Integer(255) {}
                ' max size to avoid bounds checks
                Dim i As Integer = 0
                Dim j As Integer = 0
                While i < ncolors
                    Dim r As Integer = CInt(c(j)) And &HFF
                    j += 1
                    Dim g As Integer = CInt(c(j)) And &HFF
                    j += 1
                    Dim b As Integer = CInt(c(j)) And &HFF
                    j += 1
                    tab(i) = CInt(&HFF000000UI Or (r << 16) Or (g << 8) Or b)
                    i += 1
                End While
            End If
            Return tab
        End Function

        Protected Sub ReadContents()
            ' read GIF file content blocks
            Dim done As Boolean = False
            While Not (done OrElse [Error]())
                Dim code As Integer = Read()
                Select Case code

                    Case &H2C
                        ' image separator
                        ReadImage()
                        Exit Select

                    Case &H21
                        ' extension
                        code = Read()
                        Select Case code
                            Case &HF9
                                ' graphics control extension
                                ReadGraphicControlExt()
                                Exit Select

                            Case &HFF
                                ' application extension
                                ReadBlock()
                                Dim app As String = ""
                                For i As Integer = 0 To 10
                                    app += CStr(block(i))
                                Next
                                If app.Equals("NETSCAPE2.0") Then
                                    ReadNetscapeExt()
                                Else
                                    Skip()
                                End If
                                ' don't care
                                Exit Select
                            Case Else

                                ' uninteresting extension
                                Skip()
                                Exit Select
                        End Select
                        Exit Select

                    Case &H3B
                        ' terminator
                        done = True
                        Exit Select

                    Case &H0
                        ' bad byte, but keep going and see what happens
                        Exit Select
                    Case Else

                        status = STATUS_FORMAT_ERROR
                        Exit Select
                End Select
            End While
        End Sub

        Protected Sub ReadGraphicControlExt()
            Read()
            ' block size
            Dim packed As Integer = Read()
            ' packed fields
            dispose = (packed And &H1C) >> 2
            ' disposal method
            If dispose = 0 Then
                ' elect to keep old image if discretionary
                dispose = 1
            End If
            transparency = (packed And 1) <> 0
            delay = ReadShort() * 10
            ' delay in milliseconds
            transIndex = Read()
            ' transparent color index
            Read()
            ' block terminator
        End Sub

        Protected Sub ReadHeader()
            Dim id As [String] = ""
            For i As Integer = 0 To 5
                id += ChrW(Read())
            Next
            If Not id.StartsWith("GIF") Then
                status = STATUS_FORMAT_ERROR
                Return
            End If

            ReadLSD()
            If gctFlag AndAlso Not [Error]() Then
                gct = ReadColorTable(gctSize)
                bgColor = gct(bgIndex)
            End If
        End Sub

        Protected Sub ReadImage()
            ix = ReadShort()
            ' (sub)image position & size
            iy = ReadShort()
            iw = ReadShort()
            ih = ReadShort()

            Dim packed As Integer = Read()
            lctFlag = (packed And &H80) <> 0
            ' 1 - local color table flag
            interlace = (packed And &H40) <> 0
            ' 2 - interlace flag
            ' 3 - sort flag
            ' 4-5 - reserved
            lctSize = 2 << (packed And 7)
            ' 6-8 - local color table size
            If lctFlag Then
                lct = ReadColorTable(lctSize)
                ' read table
                ' make local table active
                act = lct
            Else
                act = gct
                ' make global table active
                If bgIndex = transIndex Then
                    bgColor = 0
                End If
            End If
            Dim save As Integer = 0
            If transparency Then
                save = act(transIndex)
                ' set transparent color if specified
                act(transIndex) = 0
            End If

            If act Is Nothing Then
                ' no color table defined
                status = STATUS_FORMAT_ERROR
            End If

            If [Error]() Then
                Return
            End If

            DecodeImageData()
            ' decode pixel data
            Skip()

            If [Error]() Then
                Return
            End If

            frameCount += 1

            ' create new image to receive frame data
            '		image =
            '			new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB_PRE);

            bitmap = New Bitmap(width, height)
            image = bitmap
            SetPixels()
            ' transfer pixel data to image
            frames.Add(New GifFrame(bitmap, delay))
            ' add image to frame list
            If transparency Then
                act(transIndex) = save
            End If
            ResetFrame()

        End Sub

        Protected Sub ReadLSD()
            width = ReadShort()
            height = ReadShort()

            ' packed fields
            Dim packed As Integer = Read()
            gctFlag = (packed And &H80) <> 0
            ' 1   : global color table flag
            ' 2-4 : color resolution
            ' 5   : gct sort flag
            gctSize = 2 << (packed And 7)
            ' 6-8 : gct size
            bgIndex = Read()
            ' background color index
            pixelAspect = Read()
            ' pixel aspect ratio
        End Sub

        Protected Sub ReadNetscapeExt()
            Do
                ReadBlock()
                If block(0) = 1 Then
                    ' loop count sub-block
                    Dim b1 As Integer = CInt(block(1)) And &HFF
                    Dim b2 As Integer = CInt(block(2)) And &HFF
                    loopCount = (b2 << 8) Or b1
                End If
            Loop While (blockSize > 0) AndAlso Not [Error]()
        End Sub

        Protected Function ReadShort() As Integer
            ' read 16-bit value, LSB first
            Return Read() Or (Read() << 8)
        End Function

        Protected Sub ResetFrame()
            lastDispose = dispose
            lastRect = New Rectangle(ix, iy, iw, ih)
            lastImage = image
            lastBgColor = bgColor
            '		int dispose = 0;
            lct = Nothing
        End Sub

        Protected Sub Skip()
            Do
                ReadBlock()
            Loop While (blockSize > 0) AndAlso Not [Error]()
        End Sub
    End Class
End Namespace
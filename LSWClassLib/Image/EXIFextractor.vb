Imports System.Text
Imports System.IO
Imports System.Drawing.Imaging

Namespace Image
    Public Class EXIFextractor
        Implements IEnumerable

        ''' <summary> 
        ''' Get the individual property value by supplying property name 
        ''' These are the valid property names : 
        ''' 
        ''' "Exif IFD" 
        ''' "Gps IFD" 
        ''' "New Subfile Type" 
        ''' "Subfile Type" 
        ''' "Image Width" 
        ''' "Image Height" 
        ''' "Bits Per Sample" 
        ''' "Compression" 
        ''' "Photometric Interp" 
        ''' "Thresh Holding" 
        ''' "Cell Width" 
        ''' "Cell Height" 
        ''' "Fill Order" 
        ''' "Document Name" 
        ''' "Image Description" 
        ''' "Equip Make" 
        ''' "Equip Model" 
        ''' "Strip Offsets" 
        ''' "Orientation" 
        ''' "Samples PerPixel" 
        ''' "Rows Per Strip" 
        ''' "Strip Bytes Count" 
        ''' "Min Sample Value" 
        ''' "Max Sample Value" 
        ''' "X Resolution" 
        ''' "Y Resolution" 
        ''' "Planar Config" 
        ''' "Page Name" 
        ''' "X Position" 
        ''' "Y Position" 
        ''' "Free Offset" 
        ''' "Free Byte Counts" 
        ''' "Gray Response Unit" 
        ''' "Gray Response Curve" 
        ''' "T4 Option" 
        ''' "T6 Option" 
        ''' "Resolution Unit" 
        ''' "Page Number" 
        ''' "Transfer Funcition" 
        ''' "Software Used" 
        ''' "Date Time" 
        ''' "Artist" 
        ''' "Host Computer" 
        ''' "Predictor" 
        ''' "White Point" 
        ''' "Primary Chromaticities" 
        ''' "ColorMap" 
        ''' "Halftone Hints" 
        ''' "Tile Width" 
        ''' "Tile Length" 
        ''' "Tile Offset" 
        ''' "Tile ByteCounts" 
        ''' "InkSet" 
        ''' "Ink Names" 
        ''' "Number Of Inks" 
        ''' "Dot Range" 
        ''' "Target Printer" 
        ''' "Extra Samples" 
        ''' "Sample Format" 
        ''' "S Min Sample Value" 
        ''' "S Max Sample Value" 
        ''' "Transfer Range" 
        ''' "JPEG Proc" 
        ''' "JPEG InterFormat" 
        ''' "JPEG InterLength" 
        ''' "JPEG RestartInterval" 
        ''' "JPEG LosslessPredictors" 
        ''' "JPEG PointTransforms" 
        ''' "JPEG QTables" 
        ''' "JPEG DCTables" 
        ''' "JPEG ACTables" 
        ''' "YCbCr Coefficients" 
        ''' "YCbCr Subsampling" 
        ''' "YCbCr Positioning" 
        ''' "REF Black White" 
        ''' "ICC Profile" 
        ''' "Gamma" 
        ''' "ICC Profile Descriptor" 
        ''' "SRGB RenderingIntent" 
        ''' "Image Title" 
        ''' "Copyright" 
        ''' "Resolution X Unit" 
        ''' "Resolution Y Unit" 
        ''' "Resolution X LengthUnit" 
        ''' "Resolution Y LengthUnit" 
        ''' "Print Flags" 
        ''' "Print Flags Version" 
        ''' "Print Flags Crop" 
        ''' "Print Flags Bleed Width" 
        ''' "Print Flags Bleed Width Scale" 
        ''' "Halftone LPI" 
        ''' "Halftone LPIUnit" 
        ''' "Halftone Degree" 
        ''' "Halftone Shape" 
        ''' "Halftone Misc" 
        ''' "Halftone Screen" 
        ''' "JPEG Quality" 
        ''' "Grid Size" 
        ''' "Thumbnail Format" 
        ''' "Thumbnail Width" 
        ''' "Thumbnail Height" 
        ''' "Thumbnail ColorDepth" 
        ''' "Thumbnail Planes" 
        ''' "Thumbnail RawBytes" 
        ''' "Thumbnail Size" 
        ''' "Thumbnail CompressedSize" 
        ''' "Color Transfer Function" 
        ''' "Thumbnail Data" 
        ''' "Thumbnail ImageWidth" 
        ''' "Thumbnail ImageHeight" 
        ''' "Thumbnail BitsPerSample" 
        ''' "Thumbnail Compression" 
        ''' "Thumbnail PhotometricInterp" 
        ''' "Thumbnail ImageDescription" 
        ''' "Thumbnail EquipMake" 
        ''' "Thumbnail EquipModel" 
        ''' "Thumbnail StripOffsets" 
        ''' "Thumbnail Orientation" 
        ''' "Thumbnail SamplesPerPixel" 
        ''' "Thumbnail RowsPerStrip" 
        ''' "Thumbnail StripBytesCount" 
        ''' "Thumbnail ResolutionX" 
        ''' "Thumbnail ResolutionY" 
        ''' "Thumbnail PlanarConfig" 
        ''' "Thumbnail ResolutionUnit" 
        ''' "Thumbnail TransferFunction" 
        ''' "Thumbnail SoftwareUsed" 
        ''' "Thumbnail DateTime" 
        ''' "Thumbnail Artist" 
        ''' "Thumbnail WhitePoint" 
        ''' "Thumbnail PrimaryChromaticities" 
        ''' "Thumbnail YCbCrCoefficients" 
        ''' "Thumbnail YCbCrSubsampling" 
        ''' "Thumbnail YCbCrPositioning" 
        ''' "Thumbnail RefBlackWhite" 
        ''' "Thumbnail CopyRight" 
        ''' "Luminance Table" 
        ''' "Chrominance Table" 
        ''' "Frame Delay" 
        ''' "Loop Count" 
        ''' "Pixel Unit" 
        ''' "Pixel PerUnit X" 
        ''' "Pixel PerUnit Y" 
        ''' "Palette Histogram" 
        ''' "Exposure Time" 
        ''' "F-Number" 
        ''' "Exposure Prog" 
        ''' "Spectral Sense" 
        ''' "ISO Speed" 
        ''' "OECF" 
        ''' "Ver" 
        ''' "DTOrig" 
        ''' "DTDigitized" 
        ''' "CompConfig" 
        ''' "CompBPP" 
        ''' "Shutter Speed" 
        ''' "Aperture" 
        ''' "Brightness" 
        ''' "Exposure Bias" 
        ''' "MaxAperture" 
        ''' "SubjectDist" 
        ''' "Metering Mode" 
        ''' "LightSource" 
        ''' "Flash" 
        ''' "FocalLength" 
        ''' "Maker Note" 
        ''' "User Comment" 
        ''' "DTSubsec" 
        ''' "DTOrigSS" 
        ''' "DTDigSS" 
        ''' "FPXVer" 
        ''' "ColorSpace" 
        ''' "PixXDim" 
        ''' "PixYDim" 
        ''' "RelatedWav" 
        ''' "Interop" 
        ''' "FlashEnergy" 
        ''' "SpatialFR" 
        ''' "FocalXRes" 
        ''' "FocalYRes" 
        ''' "FocalResUnit" 
        ''' "Subject Loc" 
        ''' "Exposure Index" 
        ''' "Sensing Method" 
        ''' "FileSource" 
        ''' "SceneType" 
        ''' "CfaPattern" 
        ''' "Gps Ver" 
        ''' "Gps LatitudeRef" 
        ''' "Gps Latitude" 
        ''' "Gps LongitudeRef" 
        ''' "Gps Longitude" 
        ''' "Gps AltitudeRef" 
        ''' "Gps Altitude" 
        ''' "Gps GpsTime" 
        ''' "Gps GpsSatellites" 
        ''' "Gps GpsStatus" 
        ''' "Gps GpsMeasureMode" 
        ''' "Gps GpsDop" 
        ''' "Gps SpeedRef" 
        ''' "Gps Speed" 
        ''' "Gps TrackRef" 
        ''' "Gps Track" 
        ''' "Gps ImgDirRef" 
        ''' "Gps ImgDir" 
        ''' "Gps MapDatum" 
        ''' "Gps DestLatRef" 
        ''' "Gps DestLat" 
        ''' "Gps DestLongRef" 
        ''' "Gps DestLong" 
        ''' "Gps DestBearRef" 
        ''' "Gps DestBear" 
        ''' "Gps DestDistRef" 
        ''' "Gps DestDist" 
        ''' </summary> 
        Default Public ReadOnly Property Item(ByVal index As String) As Object
            Get
                Return properties(index)
            End Get
        End Property
        ' 
        Private bmp As System.Drawing.Bitmap
        ' 
        Private data As String
        ' 
        Private myHash As Translation
        ' 
        Private properties As Hashtable
        ' 
        Friend ReadOnly Property Count() As Integer
            Get
                Return Me.properties.Count
            End Get
        End Property
        ' 
        Private sp As String
        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <param name="id"></param> 
        ''' <param name="data"></param> 
        Public Sub setTag(ByVal id As Integer, ByVal data As String)
            Dim ascii As Encoding = Encoding.ASCII
            Me.setTag(id, data.Length, 2, ascii.GetBytes(data))
        End Sub
        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <param name="id"></param> 
        ''' <param name="len"></param> 
        ''' <param name="type"></param> 
        ''' <param name="data"></param> 
        Public Sub setTag(ByVal id As Integer, ByVal len As Integer, ByVal type As Short, ByVal data As Byte())
            Dim p As PropertyItem = CreatePropertyItem(type, id, len, data)
            Me.bmp.SetPropertyItem(p)
            buildDB(Me.bmp.PropertyItems)
        End Sub
        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <param name="type"></param> 
        ''' <param name="tag"></param> 
        ''' <param name="len"></param> 
        ''' <param name="value"></param> 
        ''' <returns></returns> 
        Private Shared Function CreatePropertyItem(ByVal type As Short, ByVal tag As Integer, ByVal len As Integer, ByVal value As Byte()) As PropertyItem
            Dim item As PropertyItem

            ' Loads a PropertyItem from a Jpeg image stored in the assembly as a resource. 
            Dim assembly As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            Dim emptyBitmapStream As Stream = assembly.GetManifestResourceStream("EXIFextractor.decoy.jpg")
            Dim empty As System.Drawing.Image = System.Drawing.Image.FromStream(emptyBitmapStream)

            item = empty.PropertyItems(0)

            ' Copies the data to the property item. 
            item.Type = type
            item.Len = len
            item.Id = tag
            item.Value = New Byte(value.Length - 1) {}
            value.CopyTo(item.Value, 0)

            Return item
        End Function
        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <param name="bmp"></param> 
        ''' <param name="sp"></param> 
        Public Sub New(ByRef bmp As System.Drawing.Bitmap, ByVal sp As String)
            properties = New Hashtable()
            ' 
            Me.bmp = bmp
            Me.sp = sp
            ' 
            myHash = New Translation()
            buildDB(Me.bmp.PropertyItems)
        End Sub
        Private msp As String = ""
        Public Sub New(ByRef bmp As System.Drawing.Bitmap, ByVal sp As String, ByVal msp As String)
            properties = New Hashtable()
            Me.sp = sp
            Me.msp = msp
            Me.bmp = bmp
            ' 
            myHash = New Translation()

            Me.buildDB(bmp.PropertyItems)
        End Sub
        Public Shared Function GetExifProperties(ByVal fileName As String) As PropertyItem()
            Dim stream As New FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)

#If NET1 Then
            ' useEmbeddedColorManagement = 
            ' validateImageData = 
        Dim image As System.Drawing.Image = System.Drawing.Image.FromStream(stream, True) 
#Else

            ' useEmbeddedColorManagement = 
            ' validateImageData = 
            Dim image As System.Drawing.Image = System.Drawing.Image.FromStream(stream, True, False)
#End If

            Return image.PropertyItems
        End Function
        Public Sub New(ByVal file As String, ByVal sp As String, ByVal msp As String)
            properties = New Hashtable()
            Me.sp = sp
            Me.msp = msp

            myHash = New Translation()
            ' 

            Me.buildDB(GetExifProperties(file))
        End Sub

        ''' <summary> 
        ''' 
        ''' </summary> 
        Private Sub buildDB(ByVal parr As System.Drawing.Imaging.PropertyItem())
            If parr Is Nothing Then
                Return
            End If

            properties.Clear()
            ' 
            data = ""
            ' 
            Dim ascii As Encoding = Encoding.ASCII
            ' 
            For Each p As System.Drawing.Imaging.PropertyItem In parr
                If p.Value Is Nothing Then
                    Continue For
                End If
                Dim v As String = ""
                Dim name As String = DirectCast(myHash(p.Id), String)
                ' tag not found. skip it 
                If name Is Nothing Then
                    Continue For
                End If
                ' 
                data += name + ": "
                ' 
                '1 = BYTE An 8-bit unsigned integer., 
                If p.Type = 1 Then
                    v = p.Value(0).ToString()
                ElseIf p.Type = 2 Then
                    '2 = ASCII An 8-bit byte containing one 7-bit ASCII code. The final byte is terminated with NULL., 
                    ' string 
                    v = ascii.GetString(p.Value)
                ElseIf p.Type = 3 Then
                    '3 = SHORT A 16-bit (2 -byte) unsigned integer, 
                    ' orientation // lookup table 
                    Select Case p.Id
                        Case 34855
                            ' ISO 
                            v = "ISO-" + convertToInt16U(p.Value).ToString()
                            Exit Select
                        Case 41495
                            ' sensing method 
                            Select Case convertToInt16U(p.Value)
                                Case 1
                                    v = "Not defined"
                                    Exit Select
                                Case 2
                                    v = "One-chip color area sensor"
                                    Exit Select
                                Case 3
                                    v = "Two-chip color area sensor"
                                    Exit Select
                                Case 4
                                    v = "Three-chip color area sensor"
                                    Exit Select
                                Case 5
                                    v = "Color sequential area sensor"
                                    Exit Select
                                Case 7
                                    v = "Trilinear sensor"
                                    Exit Select
                                Case 8
                                    v = "Color sequential linear sensor"
                                    Exit Select
                                Case Else
                                    v = " reserved"
                                    Exit Select
                            End Select
                            Exit Select
                        Case 34850
                            ' aperture 
                            Select Case convertToInt16U(p.Value)
                                Case 0
                                    v = "Not defined"
                                    Exit Select
                                Case 1
                                    v = "Manual"
                                    Exit Select
                                Case 2
                                    v = "Normal program"
                                    Exit Select
                                Case 3
                                    v = "Aperture priority"
                                    Exit Select
                                Case 4
                                    v = "Shutter priority"
                                    Exit Select
                                Case 5
                                    v = "Creative program (biased toward depth of field)"
                                    Exit Select
                                Case 6
                                    v = "Action program (biased toward fast shutter speed)"
                                    Exit Select
                                Case 7
                                    v = "Portrait mode (for closeup photos with the background out of focus)"
                                    Exit Select
                                Case 8
                                    v = "Landscape mode (for landscape photos with the background in focus)"
                                    Exit Select
                                Case Else
                                    v = "reserved"
                                    Exit Select
                            End Select
                            Exit Select
                        Case 37383
                            ' metering mode 
                            Select Case convertToInt16U(p.Value)
                                Case 0
                                    v = "unknown"
                                    Exit Select
                                Case 1
                                    v = "Average"
                                    Exit Select
                                Case 2
                                    v = "CenterWeightedAverage"
                                    Exit Select
                                Case 3
                                    v = "Spot"
                                    Exit Select
                                Case 4
                                    v = "MultiSpot"
                                    Exit Select
                                Case 5
                                    v = "Pattern"
                                    Exit Select
                                Case 6
                                    v = "Partial"
                                    Exit Select
                                Case 255
                                    v = "Other"
                                    Exit Select
                                Case Else
                                    v = "reserved"
                                    Exit Select
                            End Select
                            Exit Select
                        Case 37384
                            ' light source 
                            Select Case convertToInt16U(p.Value)
                                Case 0
                                    v = "unknown"
                                    Exit Select
                                Case 1
                                    v = "Daylight"
                                    Exit Select
                                Case 2
                                    v = "Fluorescent"
                                    Exit Select
                                Case 3
                                    v = "Tungsten"
                                    Exit Select
                                Case 17
                                    v = "Standard light A"
                                    Exit Select
                                Case 18
                                    v = "Standard light B"
                                    Exit Select
                                Case 19
                                    v = "Standard light C"
                                    Exit Select
                                Case 20
                                    v = "D55"
                                    Exit Select
                                Case 21
                                    v = "D65"
                                    Exit Select
                                Case 22
                                    v = "D75"
                                    Exit Select
                                Case 255
                                    v = "other"
                                    Exit Select
                                Case Else
                                    v = "reserved"
                                    Exit Select
                            End Select
                            Exit Select
                        Case 37385
                            Select Case convertToInt16U(p.Value)
                                Case 0
                                    v = "Flash did not fire"
                                    Exit Select
                                Case 1
                                    v = "Flash fired"
                                    Exit Select
                                Case 5
                                    v = "Strobe return light not detected"
                                    Exit Select
                                Case 7
                                    v = "Strobe return light detected"
                                    Exit Select
                                Case Else
                                    v = "reserved"
                                    Exit Select
                            End Select
                            Exit Select
                        Case Else
                            v = convertToInt16U(p.Value).ToString()
                            Exit Select
                    End Select
                ElseIf p.Type = 4 Then
                    '4 = LONG A 32-bit (4 -byte) unsigned integer, 
                    ' orientation // lookup table 
                    v = convertToInt32U(p.Value).ToString()
                ElseIf p.Type = 5 Then
                    '5 = RATIONAL Two LONGs. The first LONG is the numerator and the second LONG expresses the//denominator., 
                    ' rational 
                    Dim n As Byte() = New Byte(p.Len / 2 - 1) {}
                    Dim d As Byte() = New Byte(p.Len / 2 - 1) {}
                    Array.Copy(p.Value, 0, n, 0, p.Len \ 2)
                    Array.Copy(p.Value, p.Len \ 2, d, 0, p.Len \ 2)
                    Dim a As UInteger = convertToInt32U(n)
                    Dim b As UInteger = convertToInt32U(d)
                    Dim r As New Rational(a, b)
                    ' 
                    'convert here 
                    ' 
                    Select Case p.Id
                        Case 37378
                            ' aperture 
                            v = "F/" + System.Math.Round(System.Math.Pow(System.Math.Sqrt(2), r.ToDouble()), 2).ToString()
                            Exit Select
                        Case 37386
                            v = r.ToDouble().ToString()
                            Exit Select
                        Case 33434
                            v = r.ToDouble().ToString()
                            Exit Select
                        Case 33437
                            ' F-number 
                            v = "F/" + r.ToDouble().ToString()
                            Exit Select
                        Case Else
                            v = r.ToString("/")
                            Exit Select

                    End Select
                ElseIf p.Type = 7 Then
                    '7 = UNDEFINED An 8-bit byte that can take any value depending on the field definition, 
                    Select Case p.Id
                        Case 41728
                            If p.Value(0) = 3 Then
                                v = "DSC"
                            Else
                                v = "reserved"
                            End If
                            Exit Select
                        Case 41729
                            If p.Value(0) = 1 Then
                                v = "A directly photographed image"
                            Else
                                v = "Not a directly photographed image"
                            End If
                            Exit Select
                        Case Else
                            v = "-"
                            Exit Select
                    End Select
                ElseIf p.Type = 9 Then
                    '9 = SLONG A 32-bit (4 -byte) signed integer (2's complement notation), 
                    v = convertToInt32(p.Value).ToString()
                ElseIf p.Type = 10 Then
                    '10 = SRATIONAL Two SLONGs. The first SLONG is the numerator and the second SLONG is the 
                    'denominator. 

                    ' rational 
                    Dim n As Byte() = New Byte(p.Len / 2 - 1) {}
                    Dim d As Byte() = New Byte(p.Len / 2 - 1) {}
                    Array.Copy(p.Value, 0, n, 0, p.Len \ 2)
                    Array.Copy(p.Value, p.Len \ 2, d, 0, p.Len \ 2)
                    Dim a As Integer = convertToInt32(n)
                    Dim b As Integer = convertToInt32(d)
                    Dim r As New Rational(a, b)
                    ' 
                    ' convert here 
                    ' 
                    Select Case p.Id
                        Case 37377
                            ' shutter speed 
                            v = "1/" + System.Math.Round(System.Math.Pow(2, r.ToDouble()), 2).ToString()
                            Exit Select
                        Case 37379
                            v = System.Math.Round(r.ToDouble(), 4).ToString()
                            Exit Select
                        Case Else
                            v = r.ToString("/")
                            Exit Select
                    End Select
                End If
                ' add it to the list 
                If properties(name) Is Nothing Then
                    properties.Add(name, v)
                End If
                ' cat it too 
                data += v
                data += Me.sp
            Next

        End Sub

        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <returns></returns> 
        Public Overloads Overrides Function ToString() As String
            Return data
        End Function
        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <param name="arr"></param> 
        ''' <returns></returns> 
        Private Function convertToInt32(ByVal arr As Byte()) As Integer
            If arr.Length <> 4 Then
                Return 0
            Else
                Return arr(3) << 24 Or arr(2) << 16 Or arr(1) << 8 Or arr(0)
            End If
        End Function
        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <param name="arr"></param> 
        ''' <returns></returns> 
        Private Function convertToInt16(ByVal arr As Byte()) As Integer
            If arr.Length <> 2 Then
                Return 0
            Else
                Return arr(1) << 8 Or arr(0)
            End If
        End Function
        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <param name="arr"></param> 
        ''' <returns></returns> 
        Private Function convertToInt32U(ByVal arr As Byte()) As UInteger
            If arr.Length <> 4 Then
                Return 0
            Else
                If (arr(3) << 24 Or arr(2) << 16 Or arr(1) << 8 Or arr(0)) < 0 Then
                    Return 0
                Else
                    Return Convert.ToUInt32(arr(3) << 24 Or arr(2) << 16 Or arr(1) << 8 Or arr(0))
                End If
            End If
        End Function
        ''' <summary> 
        ''' 
        ''' </summary> 
        ''' <param name="arr"></param> 
        ''' <returns></returns> 
        Private Function convertToInt16U(ByVal arr As Byte()) As UInteger
            If arr.Length <> 2 Then
                Return 0
            Else
                Return Convert.ToUInt16(arr(1) << 8 Or arr(0))
            End If
        End Function

#Region "IEnumerable Members"
        Public Function GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            ' TODO: Add EXIFextractor.GetEnumerator implementation 
            Return (New EXIFextractorEnumerator(Me.properties))
        End Function
#End Region

    End Class
End Namespace
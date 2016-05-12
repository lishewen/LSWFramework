'*************************** Module Header ******************************'
' Module Name:  XmlTextEncoder.vb
' Project:	    VBRichTextBoxSyntaxHighlighting
' Copyright (c) Microsoft Corporation.
' 
' This CharacterEncoder class supplies a static(Shared) method to encode some 
' special characters in Xml and Rtf, such as '<', '>', '"', '&', ''', '\',
' '{' and '}' .
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Text
Imports System.IO

Namespace Text
    Public Module CharacterEncoder
        Public Function Encode(ByVal originalText As String) As String
            If String.IsNullOrWhiteSpace(originalText) Then
                Return String.Empty
            End If

            Dim encodedText As New StringBuilder()
            For i As Integer = 0 To originalText.Length - 1
                Select Case originalText.Chars(i)
                    Case """"c
                        encodedText.Append("&quot;")
                    Case "&"c
                        encodedText.Append("&amp;")
                    Case "'"c
                        encodedText.Append("&apos;")
                    Case "<"c
                        encodedText.Append("&lt;")
                    Case ">"c
                        encodedText.Append("&gt;")

                        ' The character '\' should be converted to "\\" 
                    Case "\"c
                        encodedText.Append("\\")

                        ' The character '{' should be converted to "\{" 
                    Case "{"c
                        encodedText.Append("\{")

                        ' The character '}' should be converted to "\}" 
                    Case "}"c
                        encodedText.Append("\}")

                    Case Else
                        encodedText.Append(originalText.Chars(i))
                End Select

            Next i
            Return encodedText.ToString()
        End Function

        Public Function GetEncoding(FILE_NAME As String) As System.Text.Encoding
            Dim fs As New FileStream(FILE_NAME, FileMode.Open, FileAccess.Read)
            Dim r As System.Text.Encoding = GetEncoding(fs)
            fs.Close()
            Return r
        End Function

        Public Function GetEncoding(fs As FileStream) As System.Text.Encoding

            Dim r As New BinaryReader(fs, System.Text.Encoding.[Default])
            Dim ss As Byte() = r.ReadBytes(CInt(If(fs.Length < 1000, fs.Length, 1000)))
            r.Close()
            '编码类型 Coding=编码类型.ASCII;
            Return GetEncoding(ss)
        End Function

        ' 针对无BOM的内容做判断,不是10分准确
        Public Function GetNoBomType(buf As Byte(), len As Integer) As System.Text.Encoding
            Dim chr As Byte

            For i As Integer = 0 To len - 1
                chr = buf(i)

                If chr >= &H80 Then
                    If (chr And &HF0) = &HE0 Then
                        i += 1
                        chr = buf(i)
                        If (chr And &HC0) = &H80 Then
                            i += 1
                            chr = buf(i)
                            If (chr And &HC0) = &H80 Then
                                Return System.Text.Encoding.UTF8
                            Else
                                Return System.Text.Encoding.Default
                            End If
                        Else
                            Return System.Text.Encoding.Default
                        End If
                    Else
                        Return System.Text.Encoding.Default
                    End If
                End If
            Next
            Return System.Text.Encoding.Default
        End Function

        Public Function GetEncoding(ByVal tB() As Byte) As System.Text.Encoding
            Dim tB1 As Byte, tB2 As Byte, tB3 As Byte, tB4 As Byte
            If tB.Length < 2 Then Return System.Text.Encoding.Default
            tB1 = tB(0)
            tB2 = tB(1)
            If tB.Length >= 3 Then tB3 = tB(2)
            If tB.Length >= 4 Then tB4 = tB(3)
            If (tB1 = &HFE AndAlso tB2 = &HFF) Then Return System.Text.Encoding.BigEndianUnicode
            If (tB1 = &HFF AndAlso tB2 = &HFE AndAlso tB3 <> &HFF) Then Return System.Text.Encoding.Unicode
            If (tB1 = &HEF AndAlso tB2 = &HBB AndAlso tB3 = &HBF) Then Return System.Text.Encoding.UTF8
            Return GetNoBomType(tB, tB.Length)
        End Function

        Public Function IsUTF8Bytes(data As Byte()) As Boolean
            Dim charByteCounter As Integer = 1
            '计算当前正分析的字符应还有的字节数
            Dim curByte As Byte
            '当前分析的字节.

            For i As Integer = 0 To data.Length - 1
                curByte = data(i)

                If charByteCounter = 1 Then
                    If curByte >= &H80 Then
                        '判断当前
                        While ((curByte <= 1) And &H80) <> 0
                            charByteCounter += 1
                        End While

                        '标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        If charByteCounter = 1 OrElse charByteCounter > 6 Then
                            Return False
                        End If
                    End If
                Else
                    '若是UTF-8 此时第一位必须为1
                    If (curByte And &HC0) <> &H80 Then
                        Return False
                    End If
                    charByteCounter -= 1
                End If
            Next

            If charByteCounter > 1 Then
                Throw New Exception("非预期的byte格式")
            End If

            Return True
        End Function
    End Module
End Namespace

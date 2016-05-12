Imports System.IO
Imports LSW.Extension

Namespace Image.Gif
    Public Class LZWEncoder
        Private Const EOF As Integer = -1

        Private imgW As Integer, imgH As Integer
        Private pixAry() As Byte
        Private initCodeSize As Integer
        Private remaining As Integer
        Private curPixel As Integer

        Const BITS As Integer = 12

        Const HSIZE As Integer = 5003

        Dim n_bits As Integer        ' number of bits/code
        Dim maxbits As Integer = BITS        ' user settable max # bits/code
        Dim m_maxcode As Integer        ' maximum code, given n_bits
        Dim maxmaxcode As Integer = 1 << BITS        ' should NEVER generate this code
        Dim htab As Integer() = New Integer(HSIZE - 1) {}
        Dim codetab As Integer() = New Integer(HSIZE - 1) {}

        Dim hsize__1 As Integer = HSIZE        ' for dynamic table sizing
        Dim free_ent As Integer = 0        ' first unused entry
        ' block compression parameters -- after all codes are used up,
        ' and compression rate changes, start over.
        Dim clear_flg As Boolean = False

        Dim g_init_bits As Integer

        Dim ClearCode As Integer
        Dim EOFCode As Integer

        Dim cur_accum As Integer = 0
        Dim cur_bits As Integer = 0

        Dim masks As Integer() = {&H0, &H1, &H3, &H7, &HF, &H1F, _
            &H3F, &H7F, &HFF, &H1FF, &H3FF, &H7FF, _
            &HFFF, &H1FFF, &H3FFF, &H7FFF, &HFFFF}

        ' Number of characters so far in this 'packet'
        Dim a_count As Integer

        ' Define the storage for the packet accumulator
        Dim accum As Byte() = New Byte(255) {}

        Public Sub New(width As Integer, height As Integer, pixels As Byte(), color_depth As Integer)
            imgW = width
            imgH = height
            pixAry = pixels
            initCodeSize = System.Math.Max(2, color_depth)
        End Sub

        Private Sub Add(c As Byte, outs As Stream)
            accum(a_count) = c
            a_count += 1
            If a_count >= 254 Then
                Flush(outs)
            End If
        End Sub

        Private Sub ClearTable(outs As Stream)
            ResetCodeTable(hsize)
            free_ent = ClearCode + 2
            clear_flg = True

            Output(ClearCode, outs)
        End Sub

        Private Sub ResetCodeTable(hsize As Integer)
            For i As Integer = 0 To hsize - 1
                htab(i) = -1
            Next
        End Sub

        Private Sub Compress(init_bits As Integer, outs As Stream)
            Dim fcode As Integer
            ' = 0 
            Dim i As Integer
            Dim c As Integer
            Dim ent As Integer
            Dim disp As Integer
            Dim hsize_reg As Integer
            Dim hshift As Integer

            ' Set up the globals:  g_init_bits - initial number of bits
            g_init_bits = init_bits

            ' Set up the necessary values
            clear_flg = False
            n_bits = g_init_bits
            m_maxcode = MaxCode(n_bits)

            ClearCode = 1 << (init_bits - 1)
            EOFCode = ClearCode + 1
            free_ent = ClearCode + 2

            a_count = 0
            ' clear packet
            ent = NextPixel()

            hshift = 0
            fcode = hsize
            While fcode < 65536
                hshift += 1
                fcode *= 2
            End While
            hshift = 8 - hshift
            ' set hash code range bound
            hsize_reg = hsize
            ResetCodeTable(hsize_reg)
            ' clear hash table
            Output(ClearCode, outs)
outer_loop:

            While c.InlineAssignHelper(NextPixel()) <> EOF
                fcode = (c << maxbits) + ent
                i = (c << hshift) Xor ent
                ' xor hashing
                If htab(i) = fcode Then
                    ent = codetab(i)
                    Continue While
                ElseIf htab(i) >= 0 Then
                    ' non-empty slot
                    disp = hsize_reg - i
                    ' secondary hash (after G. Knott)
                    If i = 0 Then
                        disp = 1
                    End If
                    Do
                        If (i.InlineAssignHelper(i - disp)) < 0 Then
                            i += hsize_reg
                        End If

                        If htab(i) = fcode Then
                            ent = codetab(i)
                            GoTo outer_loop
                        End If
                    Loop While htab(i) >= 0
                End If
                Output(ent, outs)
                ent = c
                If free_ent < maxmaxcode Then
                    codetab(i) = free_ent
                    free_ent += 1
                    ' code -> hashtable
                    htab(i) = fcode
                Else
                    ClearTable(outs)
                End If
            End While
            ' Put out the final code.
            Output(ent, outs)
            Output(EOFCode, outs)
        End Sub

        Public Sub Encode(os As Stream)
            os.WriteByte(Convert.ToByte(initCodeSize))
            ' write "initial code size" byte
            remaining = imgW * imgH
            ' reset navigation variables
            curPixel = 0

            Compress(initCodeSize + 1, os)
            ' compress and write the pixel data
            os.WriteByte(0)
            ' write block terminator
        End Sub

        Private Sub Flush(outs As Stream)
            If a_count > 0 Then
                outs.WriteByte(Convert.ToByte(a_count))
                outs.Write(accum, 0, a_count)
                a_count = 0
            End If
        End Sub

        Private Function MaxCode(n_bits As Integer) As Integer
            Return (1 << n_bits) - 1
        End Function

        Private Function NextPixel() As Integer
            If remaining = 0 Then
                Return EOF
            End If

            remaining -= 1

            Dim temp As Integer = curPixel + 1
            If temp < pixAry.GetUpperBound(0) Then
                Dim pix As Byte = pixAry(curPixel)
                curPixel += 1
                Return pix And &HFF
            End If
            Return &HFF
        End Function

        Private Sub Output(code As Integer, outs As Stream)
            cur_accum = cur_accum And masks(cur_bits)

            If cur_bits > 0 Then
                cur_accum = cur_accum Or (code << cur_bits)
            Else
                cur_accum = code
            End If

            cur_bits += n_bits

            While cur_bits >= 8
                Add(CByte(cur_accum And &HFF), outs)
                cur_accum >>= 8
                cur_bits -= 8
            End While

            ' If the next entry is going to be too big for the code size,
            ' then increase it, if possible.
            If free_ent > m_maxcode OrElse clear_flg Then
                If clear_flg Then
                    m_maxcode = MaxCode(n_bits.InlineAssignHelper(g_init_bits))
                    clear_flg = False
                Else
                    n_bits += 1
                    If n_bits = maxbits Then
                        m_maxcode = maxmaxcode
                    Else
                        m_maxcode = MaxCode(n_bits)
                    End If
                End If
            End If

            If code = EOFCode Then
                ' At EOF, write the rest of the buffer.
                While cur_bits > 0
                    Add(CByte(cur_accum And &HFF), outs)
                    cur_accum >>= 8
                    cur_bits -= 8
                End While

                Flush(outs)
            End If
        End Sub
    End Class
End Namespace
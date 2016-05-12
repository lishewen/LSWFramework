Imports LSW.Extension
Imports LSW.Exceptions

Namespace Image.Gif
    Public Class NeuQuant
        Protected Shared ReadOnly netsize As Integer = 256

        Protected Shared ReadOnly prime1 As Integer = 499
        Protected Shared ReadOnly prime2 As Integer = 491
        Protected Shared ReadOnly prime3 As Integer = 487
        Protected Shared ReadOnly prime4 As Integer = 503
        Protected Shared ReadOnly minpicturebytes As Integer = (3 * prime4)

        Protected Shared ReadOnly maxnetpos As Integer = (netsize - 1)
        Protected Shared ReadOnly netbiasshift As Integer = 4
        ' bias for colour values 
        Protected Shared ReadOnly ncycles As Integer = 100

        Protected Shared ReadOnly intbiasshift As Integer = 16
        ' bias for fractions 
        Protected Shared ReadOnly intbias As Integer = (CInt(1) << intbiasshift)
        Protected Shared ReadOnly gammashift As Integer = 10
        ' gamma = 1024 
        Protected Shared ReadOnly gamma As Integer = (CInt(1) << gammashift)
        Protected Shared ReadOnly betashift As Integer = 10
        Protected Shared ReadOnly beta As Integer = (intbias >> betashift)
        ' beta = 1/1024 
        Protected Shared ReadOnly betagamma As Integer = (intbias << (gammashift - betashift))

        Protected Shared ReadOnly initrad As Integer = (netsize >> 3)
        ' for 256 cols, radius starts 
        Protected Shared ReadOnly radiusbiasshift As Integer = 6
        ' at 32.0 biased by 6 bits 
        Protected Shared ReadOnly radiusbias As Integer = (CInt(1) << radiusbiasshift)
        Protected Shared ReadOnly initradius As Integer = (initrad * radiusbias)
        ' and decreases by a 
        Protected Shared ReadOnly radiusdec As Integer = 30

        Protected Shared ReadOnly alphabiasshift As Integer = 10
        ' alpha starts at 1.0 
        Protected Shared ReadOnly initalpha As Integer = (CInt(1) << alphabiasshift)

        Protected alphadec As Integer

        Protected Shared ReadOnly radbiasshift As Integer = 8
        Protected Shared ReadOnly radbias As Integer = (CInt(1) << radbiasshift)
        Protected Shared ReadOnly alpharadbshift As Integer = (alphabiasshift + radbiasshift)
        Protected Shared ReadOnly alpharadbias As Integer = (CInt(1) << alpharadbshift)

        Protected thepicture As Byte()
        ' the input image itself 
        Protected lengthcount As Integer
        ' lengthcount = H*W*3 

        Protected samplefac As Integer

        Protected network As Integer()()
        ' the network itself - [netsize][4] 
        Protected netindex As Integer() = New Integer(255) {}
        ' for network lookup - really 256 
        Protected bias As Integer() = New Integer(netsize - 1) {}
        ' bias and freq arrays for learning 

        Protected freq As Integer() = New Integer(netsize - 1) {}
        Protected radpower As Integer() = New Integer(initrad - 1) {}

        Public Sub New(thepic As Byte(), len As Integer, sample As Integer)
            Dim i As Integer
            Dim p As Integer()

            thepicture = thepic
            lengthcount = len
            samplefac = sample

            network = New Integer(netsize - 1)() {}
            For i = 0 To netsize - 1
                network(i) = New Integer(3) {}
                p = network(i)
                p(0) = p(1).InlineAssignHelper(p(2).InlineAssignHelper((i << (netbiasshift + 8)) / netsize))
                freq(i) = intbias / netsize
                ' 1/netsize 
                bias(i) = 0
            Next
        End Sub

        Public Function ColorMap() As Byte()
            Dim map As Byte() = New Byte(3 * netsize - 1) {}
            Dim index As Integer() = New Integer(netsize - 1) {}
            For i As Integer = 0 To netsize - 1
                index(network(i)(3)) = i
            Next
            Dim k As Integer = 0
            For i As Integer = 0 To netsize - 1
                Dim j As Integer = index(i)
                map(k) = CByte(network(j)(0))
                k += 1
                map(k) = CByte(network(j)(1))
                k += 1
                map(k) = CByte(network(j)(2))
                k += 1
            Next
            Return map
        End Function

        Public Sub Inxbuild()
            Dim i As Integer, j As Integer, smallpos As Integer, smallval As Integer
            Dim p As Integer()
            Dim q As Integer()
            Dim previouscol As Integer, startpos As Integer

            previouscol = 0
            startpos = 0
            For i = 0 To netsize - 1
                p = network(i)
                smallpos = i
                smallval = p(1)
                ' index on g 
                ' find smallest in i..netsize-1 

                For j = i + 1 To netsize - 1
                    q = network(j)
                    If q(1) < smallval Then
                        ' index on g 
                        smallpos = j
                        ' index on g 
                        smallval = q(1)
                    End If
                Next
                q = network(smallpos)
                ' swap p (i) and q (smallpos) entries 

                If i <> smallpos Then
                    j = q(0)
                    q(0) = p(0)
                    p(0) = j
                    j = q(1)
                    q(1) = p(1)
                    p(1) = j
                    j = q(2)
                    q(2) = p(2)
                    p(2) = j
                    j = q(3)
                    q(3) = p(3)
                    p(3) = j
                End If
                ' smallval entry is now in position i 

                If smallval <> previouscol Then
                    netindex(previouscol) = (startpos + i) >> 1
                    For j = previouscol + 1 To smallval - 1
                        netindex(j) = i
                    Next
                    previouscol = smallval
                    startpos = i
                End If
            Next
            netindex(previouscol) = (startpos + maxnetpos) >> 1
            For j = previouscol + 1 To 255
                netindex(j) = maxnetpos
            Next
            ' really 256 
        End Sub

        Public Sub Learn()
            Dim i As Integer, j As Integer, b As Integer, g As Integer, r As Integer
            Dim radius As Integer, rad As Integer, alpha As Integer, [step] As Integer, delta As Integer, samplepixels As Integer
            Dim p As Byte()
            Dim pix As Integer, lim As Integer

            If lengthcount < minpicturebytes Then
                samplefac = 1
            End If
            alphadec = 30 + ((samplefac - 1) \ 3)
            p = thepicture
            pix = 0
            lim = lengthcount
            samplepixels = lengthcount / (3 * samplefac)
            delta = samplepixels / ncycles
            alpha = initalpha
            radius = initradius

            rad = radius >> radiusbiasshift
            If rad <= 1 Then
                rad = 0
            End If
            For i = 0 To rad - 1
                radpower(i) = alpha * (((rad * rad - i * i) * radbias) \ (rad * rad))
            Next

            'fprintf(stderr,"beginning 1D learning: initial radius=%d\n", rad);

            If lengthcount < minpicturebytes Then
                [step] = 3
            ElseIf (lengthcount Mod prime1) <> 0 Then
                [step] = 3 * prime1
            Else
                If (lengthcount Mod prime2) <> 0 Then
                    [step] = 3 * prime2
                Else
                    If (lengthcount Mod prime3) <> 0 Then
                        [step] = 3 * prime3
                    Else
                        [step] = 3 * prime4
                    End If
                End If
            End If

            i = 0
            While i < samplepixels
                b = (p(pix + 0) And &HFF) << netbiasshift
                g = (p(pix + 1) And &HFF) << netbiasshift
                r = (p(pix + 2) And &HFF) << netbiasshift
                j = Contest(b, g, r)

                Altersingle(alpha, j, b, g, r)
                If rad <> 0 Then
                    Alterneigh(rad, j, b, g, r)
                End If
                ' alter neighbours 

                pix += [step]
                If pix >= lim Then
                    pix -= lengthcount
                End If

                i += 1
                If delta = 0 Then
                    delta = 1
                End If
                If i Mod delta = 0 Then
                    alpha -= alpha / alphadec
                    radius -= radius / radiusdec
                    rad = radius >> radiusbiasshift
                    If rad <= 1 Then
                        rad = 0
                    End If
                    For j = 0 To rad - 1
                        radpower(j) = alpha * (((rad * rad - j * j) * radbias) \ (rad * rad))
                    Next
                End If
            End While
            'fprintf(stderr,"finished 1D learning: readonly alpha=%f !\n",((float)alpha)/initalpha);
        End Sub

        Public Function Map(b As Integer, g As Integer, r As Integer) As Integer
            Dim i As Integer, j As Integer, dist As Integer, a As Integer, bestd As Integer
            Dim p As Integer()
            Dim best As Integer

            bestd = 1000
            ' biggest possible dist is 256*3 
            best = -1
            i = netindex(g)
            ' index on g 
            j = i - 1
            ' start at netindex[g] and work outwards 

            While (i < netsize) OrElse (j >= 0)
                If i < netsize Then
                    p = network(i)
                    dist = p(1) - g
                    ' inx key 
                    If dist >= bestd Then
                        i = netsize
                    Else
                        ' stop iter 
                        i += 1
                        If dist < 0 Then
                            dist = -dist
                        End If
                        a = p(0) - b
                        If a < 0 Then
                            a = -a
                        End If
                        dist += a
                        If dist < bestd Then
                            a = p(2) - r
                            If a < 0 Then
                                a = -a
                            End If
                            dist += a
                            If dist < bestd Then
                                bestd = dist
                                best = p(3)
                            End If
                        End If
                    End If
                End If
                If j >= 0 Then
                    p = network(j)
                    dist = g - p(1)
                    ' inx key - reverse dif 
                    If dist >= bestd Then
                        j = -1
                    Else
                        ' stop iter 
                        j -= 1
                        If dist < 0 Then
                            dist = -dist
                        End If
                        a = p(0) - b
                        If a < 0 Then
                            a = -a
                        End If
                        dist += a
                        If dist < bestd Then
                            a = p(2) - r
                            If a < 0 Then
                                a = -a
                            End If
                            dist += a
                            If dist < bestd Then
                                bestd = dist
                                best = p(3)
                            End If
                        End If
                    End If
                End If
            End While
            Return (best)
        End Function

        Public Function Process() As Byte()
            Learn()
            Unbiasnet()
            Inxbuild()
            Return ColorMap()
        End Function

        Public Sub Unbiasnet()
            Dim i As Integer

            For i = 0 To netsize - 1
                network(i)(0) >>= netbiasshift
                network(i)(1) >>= netbiasshift
                network(i)(2) >>= netbiasshift
                ' record colour no 
                network(i)(3) = i
            Next
        End Sub

        Protected Sub Alterneigh(rad As Integer, i As Integer, b As Integer, g As Integer, r As Integer)
            Dim j As Integer, k As Integer, lo As Integer, hi As Integer, a As Integer, m As Integer
            Dim p As Integer()

            lo = i - rad
            If lo < -1 Then
                lo = -1
            End If
            hi = i + rad
            If hi > netsize Then
                hi = netsize
            End If

            j = i + 1
            k = i - 1
            m = 1
            While (j < hi) OrElse (k > lo)
                a = radpower(m)
                m += 1
                If j < hi Then
                    p = network(j)
                    j += 1
                    Try
                        p(0) -= (a * (p(0) - b)) / alpharadbias
                        p(1) -= (a * (p(1) - g)) / alpharadbias
                        p(2) -= (a * (p(2) - r)) / alpharadbias
                    Catch e As Exception
                        ' prevents 1.3 miscompilation
                        Dim ex As New LSWFrameworkException(e)
                    End Try
                End If
                If k > lo Then
                    p = network(System.Math.Max(System.Threading.Interlocked.Decrement(k), k + 1))
                    Try
                        p(0) -= (a * (p(0) - b)) / alpharadbias
                        p(1) -= (a * (p(1) - g)) / alpharadbias
                        p(2) -= (a * (p(2) - r)) / alpharadbias
                    Catch e As Exception
                        Dim ex As New LSWFrameworkException(e)
                    End Try
                End If
            End While
        End Sub

        Protected Sub Altersingle(alpha As Integer, i As Integer, b As Integer, g As Integer, r As Integer)
            Dim n As Integer() = network(i)
            n(0) -= (alpha * (n(0) - b)) / initalpha
            n(1) -= (alpha * (n(1) - g)) / initalpha
            n(2) -= (alpha * (n(2) - r)) / initalpha
        End Sub

        Protected Function Contest(b As Integer, g As Integer, r As Integer) As Integer
            Dim i As Integer, dist As Integer, a As Integer, biasdist As Integer, betafreq As Integer
            Dim bestpos As Integer, bestbiaspos As Integer, bestd As Integer, bestbiasd As Integer
            Dim n As Integer()

            bestd = Not (CInt(1) << 31)
            bestbiasd = bestd
            bestpos = -1
            bestbiaspos = bestpos

            For i = 0 To netsize - 1
                n = network(i)
                dist = n(0) - b
                If dist < 0 Then
                    dist = -dist
                End If
                a = n(1) - g
                If a < 0 Then
                    a = -a
                End If
                dist += a
                a = n(2) - r
                If a < 0 Then
                    a = -a
                End If
                dist += a
                If dist < bestd Then
                    bestd = dist
                    bestpos = i
                End If
                biasdist = dist - ((bias(i)) >> (intbiasshift - netbiasshift))
                If biasdist < bestbiasd Then
                    bestbiasd = biasdist
                    bestbiaspos = i
                End If
                betafreq = (freq(i) >> betashift)
                freq(i) -= betafreq
                bias(i) += (betafreq << gammashift)
            Next
            freq(bestpos) += beta
            bias(bestpos) -= betagamma
            Return (bestbiaspos)
        End Function
    End Class
End Namespace
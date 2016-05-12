Imports System.Math

Namespace API.Google
    Public Module EarthHelper
        ''' <summary>
        ''' google earth地图图片地址的qrst算法
        ''' </summary>
        ''' <param name="row">row:lon</param>
        ''' <param name="col">col:lat</param>
        ''' <param name="zoom">zoon:地图级别</param>
        ''' <returns></returns>
        Public Function getSatTileId(ByVal row As Integer, ByVal col As Integer, ByVal zoom As Integer) As String
            'trtqst
            Dim tileid As String = "t"
            Dim halflat As Double = row
            '
            Dim locxmin As Double, locxmax As Double, locymin As Double, locymax As Double, locxmoy As Double, locymoy As Double

            locxmin = 0
            locxmax = Pow(2, zoom)
            locymin = 0
            locymax = Pow(2, zoom)

            For i As Integer = 0 To zoom - 1
                locxmoy = (locxmax + locxmin) / 2
                locymoy = (locymax + locymin) / 2
                If (halflat < locymin) OrElse (halflat > locymax) OrElse (col < locxmin) OrElse (col > locxmax) Then
                    Return ("transparent")
                End If
                If halflat < locymoy Then
                    locymax = locymoy
                    If col < locxmoy Then
                        'q quadrant (top left)
                        tileid += "q"
                        locxmax = locxmoy
                    Else
                        'r quadrant (top right)
                        tileid += "r"
                        locxmin = locxmoy
                    End If
                Else
                    locymin = locymoy
                    If col < locxmoy Then
                        't quadrant (bottom right)
                        tileid += "t"
                        locxmax = locxmoy
                    Else
                        's quadrant (bottom left)
                        tileid += "s"
                        locxmin = locxmoy
                    End If
                End If
            Next
            Return tileid
        End Function
    End Module
End Namespace
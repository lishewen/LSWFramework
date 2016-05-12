Imports System.Text.RegularExpressions

Namespace Web
    Public Class UBB
        Public Function UBBToHtml(vstr As String) As String
            '替换HTML标记及换行处理 
            vstr = Replace(vstr, ">", ">")
            vstr = Replace(vstr, "<", "<")
            vstr = Replace(vstr, Chr(32), " ")
            vstr = Replace(vstr, Chr(9), "   ")
            vstr = Replace(vstr, Chr(34), """")
            vstr = Replace(vstr, Chr(39), "'")
            vstr = Replace(vstr, Chr(13), "")
            vstr = Replace(vstr, Chr(10), "<BR> ")
            vstr = Replace(vstr, "[enter]", "<BR> ")

            Dim xface As Integer
            If xface = 1 Then
                '表情转换,数值型变量xface=1时，允许转换 
                vstr = Replace(vstr, ":)", "<img src=""em/em1.gif"">")
                vstr = Replace(vstr, ":(", "<img src=""em/em2.gif"">")
                vstr = Replace(vstr, ":o", "<img src=""em/em3.gif"">")
                vstr = Replace(vstr, ":D", "<img src=""em/em4.gif"">")
                vstr = Replace(vstr, ";)", "<img src=""em/em5.gif"">")
                vstr = Replace(vstr, ":p", "<img src=""em/em6.gif"">")
                vstr = Replace(vstr, ":cool:", "<img src=""em/em7.gif"">")
                vstr = Replace(vstr, ":mad:", "<img src=""em/em8.gif"">")
                vstr = Replace(vstr, ":eek:", "<img src=""em/em9.gif"">")
                vstr = Replace(vstr, ":?:", "<img src=""em/em0.gif"">")
            End If

            Dim objregex As Regex

            '屏蔽JS等等 
            objregex = New Regex("javascript")
            vstr = objregex.Replace(vstr, "javascript")
            objregex = New Regex("jscript:")
            vstr = objregex.Replace(vstr, "jscript:")
            objregex = New Regex("js:")
            vstr = objregex.Replace(vstr, "js:")
            objregex = New Regex("value")
            vstr = objregex.Replace(vstr, "value")
            objregex = New Regex("about:")
            vstr = objregex.Replace(vstr, "about:")
            objregex = New Regex("file:")
            vstr = objregex.Replace(vstr, "file:")
            objregex = New Regex("document.cookie")
            vstr = objregex.Replace(vstr, "documents.cookie")
            objregex = New Regex("vbscript:")
            vstr = objregex.Replace(vstr, "vbscript:")
            objregex = New Regex("vbs:")
            vstr = objregex.Replace(vstr, "vbs:")
            objregex = New Regex("(on(mouse|exit|error|click|key))")
            vstr = objregex.Replace(vstr, "on$2")
            objregex = New Regex("script")
            vstr = objregex.Replace(vstr, "script")

            'UBB转换 
            'url 
            objregex = New Regex("(\[URL=(.[^\[]*)\])(.*?)(\[\/URL\])")
            vstr = objregex.Replace(vstr, "<A HREF=""$2"" TARGET=_blank>$3</A>")
            'EMAIL 
            objregex = New Regex("\[EMAIL\](.[^\[]*)\[\/EMAIL\]")
            vstr = objregex.Replace(vstr, "<a href=""mailto:$1"" TARGET=""_blank"">$1</a>")
            'IMG 
            objregex = New Regex("\[IMG\](http|https|ftp):\/\/(.[^\[]*)\[\/IMG\]")
            vstr = objregex.Replace(vstr, "<br><a onfocus=this.blur() href=""$1://$2"" target=_blank><IMG SRC=""$1://$2"" border=0 alt=按此在新窗口浏览图片 onload=""javascript:if(this.width>screen.width-333)this.width=screen.width-333""></a>")
            '自动识别URL 
            objregex = New Regex("\[url=(http:\/\/.[^\[]*)\](.[^\[]*)(\[\/url\])")
            vstr = objregex.Replace(vstr, "<a href=""$1"" target=""_blank"">$2</a>")
            objregex = New Regex("^(http://[A-Za-z0-9\./=\?%\-&_~`@':+!]+)")
            vstr = objregex.Replace(vstr, "<a href=""$1"" target=""_blank"">$1</a>")
            objregex = New Regex("(http://[A-Za-z0-9\./=\?%\-&_~`@':+!]+)$")
            vstr = objregex.Replace(vstr, "<a target=_blank href=$1>$1</a>")
            objregex = New Regex("[^>=""](http://[A-Za-z0-9\./=\?%\-&_~`@':+!]+)")
            vstr = objregex.Replace(vstr, "<a target=_blank href=$1>$1</a>")
            'COLOR 
            objregex = New Regex("\[color=(.[^\[]*)\](.[^\[]*)\[\/color\]")
            vstr = objregex.Replace(vstr, "<font color=""$1"">$2</font>")
            'u 
            objregex = New Regex("\[U\](.*)\[\/U\]")
            vstr = objregex.Replace(vstr, "<U>$1</U>")
            'B 
            objregex = New Regex("\[B\](.*)\[\/B\]")
            vstr = objregex.Replace(vstr, "<B>$1</B>")
            'I 
            objregex = New Regex("\[I\](.*)\[\/I\]")
            vstr = objregex.Replace(vstr, "<I>$1</I>")
            'FLY 
            objregex = New Regex("\[fly\](.*)\[\/fly\]")
            vstr = objregex.Replace(vstr, "<marquee width=""80%"" behavior=""alternate"" scrollamount=""3"">$1</marquee>")
            'SHADOW 
            objregex = New Regex("\[SHADOW=*([0-9]*),*(#*[a-z0-9]*),*([0-9]*)\](.[^\[]*)\[\/SHADOW]")
            vstr = objregex.Replace(vstr, "<table width=$1 style=""filter:shadow(color=$2, strength=$3)"">$4</table>")
            'GLOW 
            objregex = New Regex("\[glow=*([0-9]*),*(#*[a-z0-9]*),*([0-9]*)\](.[^\[]*)\[\/glow]")
            vstr = objregex.Replace(vstr, "<table width=$1 style=""filter:glow(color=$2, strength=$3)"">$4</table>")
            'center 
            objregex = New Regex("(\[center\])(.*?)(\[\/center\])")
            vstr = objregex.Replace(vstr, "<center>$2</center>")

            'CODE 
            objregex = New Regex("\[code\](.*)\[\/code\]")
            vstr = objregex.Replace(vstr, "<table width=""80%""   border=""0"" cellpadding=""2"" cellspacing=""0"" bgcolor=""#99FFCC"" style=""border:1px solid #000000;font-size:9pt;font-family:tahoma""><tr><td>$1</td></tr></table>")

            'FLASH 
            objregex = New Regex("(\[falsh\])(.*?)(\[\/falsh\])")
            vstr = objregex.Replace(vstr, "<OBJECT codeBase=http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0 classid=clsid:D27CDB6E-AE6D-11cf-96B8-444553540000 width=500 height=400><PARAM NAME=movie VALUE=""$2""><PARAM NAME=quality VALUE=high><embed src=""$2"" quality=high pluginspage='http://www.macromedia.com/go/getflashplayer' type='application/x-shockwave-flash' width=500 height=400>$2</embed></OBJECT>")
            objregex = New Regex("(\[falsh=*([0-9]*),*([0-9]*)\])(.*?)(\[\/falsh\])")
            vstr = objregex.Replace(vstr, "<a href=""$4"" TARGET=_blank><IMG SRC=pic/swf.gif border=0 alt=点击开新窗口欣赏该FLASH动画!> [全屏欣赏]</a><br><br><OBJECT codeBase=http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0 classid=clsid:D27CDB6E-AE6D-11cf-96B8-444553540000 width=$2 height=$3><PARAM NAME=movie VALUE=""$4""><PARAM NAME=quality VALUE=high><param name=menu value=false><embed src=""$4"" quality=high menu=false pluginspage='http://www.macromedia.com/go/getflashplayer' type='application/x-shockwave-flash' width=$2 height=$3>$4</embed></OBJECT>")

            'dir 
            objregex = New Regex("\[DIR=*([0-9]*),*([0-9]*)\](.[^\[]*)\[\/DIR]")
            vstr = objregex.Replace(vstr, "<object classid=clsid:166B1BCA-3F9C-11CF-8075-444553540000 codebase=http://download.macromedia.com/pub/shockwave/cabs/director/sw.cab#version=7,0,2,0 width=$1 height=$2><param name=src value=$3><embed src=$3 pluginspage=http://www.macromedia.com/shockwave/download/ width=$1 height=$2></embed></object>")

            'rm 
            objregex = New Regex("\[rm=*([0-9]*),*([0-9]*)\](.[^\[]*)\[\/rm]")
            vstr = objregex.Replace(vstr, "<OBJECT classid=clsid:CFCDAA03-8BE4-11cf-B84B-0020AFBBCCFA class=OBJECT id=RAOCX width=$1 height=$2><PARAM NAME=SRC VALUE=$3><PARAM NAME=CONSOLE VALUE=Clip1><PARAM NAME=CONTROLS VALUE=imagewindow><PARAM NAME=AUTOSTART VALUE=true></OBJECT><br><OBJECT classid=CLSID:CFCDAA03-8BE4-11CF-B84B-0020AFBBCCFA height=32 id=video2 width=$1><PARAM NAME=SRC VALUE=$3><PARAM NAME=AUTOSTART VALUE=-1><PARAM NAME=CONTROLS VALUE=controlpanel><PARAM NAME=CONSOLE VALUE=Clip1></OBJECT>")

            'mp 
            objregex = New Regex("\[mp=*([0-9]*),*([0-9]*)\](.[^\[]*)\[\/mp]")
            vstr = objregex.Replace(vstr, "<object align=middle classid=CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95 class=OBJECT id=MediaPlayer width=$1 height=$2 ><param name=ShowStatusBar value=-1><param name=Filename value=$3><embed type=application/x-oleobject codebase=http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701 flename=mp src=$3   width=$1 height=$2></embed></object>")

            'qt 
            objregex = New Regex("\[qt=*([0-9]*),*([0-9]*)\](.[^\[]*)\[\/qt]")
            vstr = objregex.Replace(vstr, "<embed src=$3 width=$1 height=$2 autoplay=true loop=false controller=true playeveryframe=false cache=false scale=TOFIT bgcolor=#000000 kioskmode=false targetcache=false pluginspage=http://www.apple.com/quicktime/>")

            'QUOTE 
            objregex = New Regex("(\[QUOTE\])(.*)(\[\/QUOTE\])")
            vstr = objregex.Replace(vstr, "<table cellpadding=0 cellspacing=0 border=1 WIDTH=94% bordercolor=#000000 bgcolor=#F2F8FF align=center   ><tr><td   ><table width=100% cellpadding=5 cellspacing=1 border=0><TR><TD >$2</table></table><br>")

            'move 
            objregex = New Regex("(\[move\])(.*)(\[\/move\])")
            vstr = objregex.Replace(vstr, "<MARQUEE scrollamount=3>$2</marquee>")

            'size 
            objregex = New Regex("(\[size=1\])(.[^\[]*)(\[\/size\])")
            vstr = objregex.Replace(vstr, "<font size=1>$2</font>")
            objregex = New Regex("(\[size=2\])(.[^\[]*)(\[\/size\])")
            vstr = objregex.Replace(vstr, "<font size=2>$2</font>")
            objregex = New Regex("(\[size=3\])(.[^\[]*)(\[\/size\])")
            vstr = objregex.Replace(vstr, "<font size=3>$2</font>")
            objregex = New Regex("(\[size=4\])(.[^\[]*)(\[\/size\])")
            vstr = objregex.Replace(vstr, "<font size=4>$2</font>")

            'face 
            objregex = New Regex("(\[face=(.[^\[]*)\])(.[^\[]*)(\[\/face\])")
            vstr = objregex.Replace(vstr, "<font face=$2>$3</font>")

            'em 

            objregex = New Regex("(\[em(.[^\[]*)\])")
            vstr = objregex.Replace(vstr, "<img src=pic\em$2.gif border=0 align=middle>")
            '完成，还可以自己扩展 
            Return vstr
        End Function
    End Class
End Namespace

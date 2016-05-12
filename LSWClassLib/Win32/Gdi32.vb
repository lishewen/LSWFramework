Imports System.Runtime.InteropServices

Namespace Win32
    Public Module Gdi32
        Public Const SRCCOPY As Integer = &HCC0020

        Public Declare Function GetPixel Lib "gdi32" (ByVal hdc As Integer, ByVal x As Integer, ByVal y As Integer) As Integer
        Public Declare Function CreateCompatibleDC Lib "gdi32.dll" (hdc As IntPtr) As IntPtr
        <DllImport("Gdi32.dll")> _
        Public Function SelectObject(ByVal hdc As IntPtr, ByVal hObject As IntPtr) As IntPtr
        End Function
        <DllImport("gdi32.dll")> _
        Public Function DeleteObject(hObject As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function
        <DllImport("gdi32.dll")> _
        Public Function DeleteDC(hdc As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function
        ''' <summary>
        ''' 创建一个圆角矩形区域
        ''' </summary>
        ''' <param name="nLeftRect">x坐标左上角</param>
        ''' <param name="nTopRect">y坐标左上角</param>
        ''' <param name="nRightRect">x坐标右上角</param>
        ''' <param name="nBottomRect">y坐标右上角</param>
        ''' <param name="nWidthEllipse">椭圆的宽度</param>
        ''' <param name="nHeightEllipse">椭圆的高度</param>
        ''' <returns></returns>
        <DllImport("gdi32.dll")> _
        Public Function CreateRoundRectRgn(nLeftRect As Integer, nTopRect As Integer, nRightRect As Integer, nBottomRect As Integer, nWidthEllipse As Integer, nHeightEllipse As Integer) As Integer
        End Function

        Public Declare Function CreateDC Lib "gdi32" Alias "CreateDCA" (ByVal lpDriverName As String, ByVal lpDeviceName As String, ByVal lpOutput As String, ByVal lpInitData As Int32) As Int32
        Public Declare Function BitBlt Lib "gdi32" (ByVal hDestDC As Integer, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hSrcDC As Integer, ByVal xSrc As Integer, ByVal ySrc As Integer, ByVal dwRop As Integer) As Integer

        <DllImport("gdi32.dll")> _
        Public Function CreateCompatibleBitmap(hDC As IntPtr, nWidth As Integer, nHeight As Integer) As IntPtr
        End Function
    End Module
End Namespace

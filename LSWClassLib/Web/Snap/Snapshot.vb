Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Runtime.InteropServices.ComTypes
Imports LSW.Exceptions

Namespace Web.Snap
    ''' <summary>
    ''' ActiveX 组件快照类
    ''' AcitveX 必须实现 IViewObject 接口
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Snapshot
        ''' <summary> 
        ''' 取快照 
        ''' </summary> 
        ''' <param name="pUnknown">Com 对象</param> 
        ''' <param name="bmpRect">图象大小</param> 
        ''' <returns></returns> 
        Public Function TakeSnapshot(ByVal pUnknown As Object, ByVal bmpRect As Rectangle) As Bitmap
            If pUnknown Is Nothing Then
                Return Nothing
            End If
            '必须为com对象 
            If Not Marshal.IsComObject(pUnknown) Then
                Return Nothing
            End If
            'IViewObject 接口 
            Dim ViewObject As UnsafeNativeMethods.IViewObject = Nothing
            Dim pViewObject As IntPtr = IntPtr.Zero
            '内存图 
            Dim pPicture As New Bitmap(bmpRect.Width, bmpRect.Height)
            Dim hDrawDC As Graphics = Graphics.FromImage(pPicture)
            '获取接口 
            Dim hret As Object = Marshal.QueryInterface(Marshal.GetIUnknownForObject(pUnknown), UnsafeNativeMethods.IID_IViewObject, pViewObject)
            Try
                ViewObject = TryCast(Marshal.GetTypedObjectForIUnknown(pViewObject, GetType(UnsafeNativeMethods.IViewObject)), UnsafeNativeMethods.IViewObject)
                '调用Draw方法 
                ViewObject.Draw(CInt(DVASPECT.DVASPECT_CONTENT), -1, IntPtr.Zero, Nothing, IntPtr.Zero, hDrawDC.GetHdc(), _
                New NativeMethods.COMRECT(bmpRect), Nothing, IntPtr.Zero, 0)
                Marshal.Release(pViewObject)
            Catch ex As Exception
                'Console.WriteLine(ex.Message)
                Throw New LSWFrameworkException(ex)
            End Try
            '释放 
            hDrawDC.Dispose()
            Return pPicture
        End Function
    End Class
End Namespace
Imports System.Security
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes

Namespace Web.Snap
    <SuppressUnmanagedCodeSecurity()> _
    Friend NotInheritable Class UnsafeNativeMethods
        Private Sub New()
        End Sub
        Public Shared IID_IViewObject As New Guid("{0000010d-0000-0000-C000-000000000046}")

        <ComImport(), Guid("0000010d-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
        Public Interface IViewObject
            <PreserveSig()> _
            Function Draw(<[In](), MarshalAs(UnmanagedType.U4)> _
    ByVal dwDrawAspect As Integer, ByVal lindex As Integer, ByVal pvAspect As IntPtr, <[In]()> _
    ByVal ptd As NativeMethods.tagDVTARGETDEVICE, ByVal hdcTargetDev As IntPtr, ByVal hdcDraw As IntPtr, _
                    <[In]()> _
    ByVal lprcBounds As NativeMethods.COMRECT, <[In]()> _
    ByVal lprcWBounds As NativeMethods.COMRECT, ByVal pfnContinue As IntPtr, <[In]()> _
    ByVal dwContinue As Integer) As Integer
            <PreserveSig()> _
            Function GetColorSet(<[In](), MarshalAs(UnmanagedType.U4)> _
    ByVal dwDrawAspect As Integer, ByVal lindex As Integer, ByVal pvAspect As IntPtr, <[In]()> _
    ByVal ptd As NativeMethods.tagDVTARGETDEVICE, ByVal hicTargetDev As IntPtr, <Out()> _
    ByVal ppColorSet As NativeMethods.tagLOGPALETTE) As Integer
            <PreserveSig()> _
            Function Freeze(<[In](), MarshalAs(UnmanagedType.U4)> _
    ByVal dwDrawAspect As Integer, ByVal lindex As Integer, ByVal pvAspect As IntPtr, <Out()> _
    ByVal pdwFreeze As IntPtr) As Integer
            <PreserveSig()> _
            Function Unfreeze(<[In](), MarshalAs(UnmanagedType.U4)> _
    ByVal dwFreeze As Integer) As Integer
            Sub SetAdvise(<[In](), MarshalAs(UnmanagedType.U4)> _
    ByVal aspects As Integer, <[In](), MarshalAs(UnmanagedType.U4)> _
    ByVal advf As Integer, <[In](), MarshalAs(UnmanagedType.[Interface])> _
    ByVal pAdvSink As IAdviseSink)
            Sub GetAdvise(<[In](), Out(), MarshalAs(UnmanagedType.LPArray)> _
    ByVal paspects As Integer(), <[In](), Out(), MarshalAs(UnmanagedType.LPArray)> _
    ByVal advf As Integer(), <[In](), Out(), MarshalAs(UnmanagedType.LPArray)> _
    ByVal pAdvSink As IAdviseSink())
        End Interface
    End Class
End Namespace
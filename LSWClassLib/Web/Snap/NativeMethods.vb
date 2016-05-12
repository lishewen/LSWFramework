Imports System.Runtime.InteropServices
Imports System.Drawing

Namespace Web.Snap
    Friend NotInheritable Class NativeMethods
        Private Sub New()
        End Sub
        <StructLayout(LayoutKind.Sequential)> _
        Public NotInheritable Class tagDVTARGETDEVICE
            <MarshalAs(UnmanagedType.U4)> _
            Public tdSize As Integer
            <MarshalAs(UnmanagedType.U2)> _
            Public tdDriverNameOffset As Short
            <MarshalAs(UnmanagedType.U2)> _
            Public tdDeviceNameOffset As Short
            <MarshalAs(UnmanagedType.U2)> _
            Public tdPortNameOffset As Short
            <MarshalAs(UnmanagedType.U2)> _
            Public tdExtDevmodeOffset As Short
        End Class

        <StructLayout(LayoutKind.Sequential)> _
        Public Class COMRECT
            Public left As Integer
            Public top As Integer
            Public right As Integer
            Public bottom As Integer
            Public Sub New()
            End Sub

            Public Sub New(ByVal r As Rectangle)
                Me.left = r.X
                Me.top = r.Y
                Me.right = r.Right
                Me.bottom = r.Bottom
            End Sub

            Public Sub New(ByVal left As Integer, ByVal top As Integer, ByVal right As Integer, ByVal bottom As Integer)
                Me.left = left
                Me.top = top
                Me.right = right
                Me.bottom = bottom
            End Sub

            Public Shared Function FromXYWH(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As NativeMethods.COMRECT
                Return New NativeMethods.COMRECT(x, y, x + width, y + height)
            End Function

            Public Overloads Overrides Function ToString() As String
                Return String.Concat(New Object() {"Left = ", Me.left, " Top ", Me.top, " Right = ", Me.right, _
                " Bottom = ", Me.bottom})
            End Function

        End Class

        <StructLayout(LayoutKind.Sequential)> _
        Public NotInheritable Class tagLOGPALETTE
            <MarshalAs(UnmanagedType.U2)> _
            Public palVersion As Short
            <MarshalAs(UnmanagedType.U2)> _
            Public palNumEntries As Short
        End Class
    End Class
End Namespace
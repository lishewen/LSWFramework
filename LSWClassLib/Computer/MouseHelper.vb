Imports LSW.Win32
Imports System.Drawing
Imports System.Windows.Forms

Namespace Computer
    Public Module MouseHelper
        Public Sub MouseClick()
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero)
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero)
        End Sub

        Public Sub MouseClick(ByVal location As Point)
            MouseMove(location)
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero)
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero)
        End Sub

        Public Sub MouseMove(ByVal location As Point)
            SetCursorPos(location.X, location.Y)
        End Sub

        Public Sub MouseRightClick(ByVal location As Point)
            MouseMove(location)
            mouse_event(MouseEventFlag.RightDown, 0, 0, 0, UIntPtr.Zero)
            mouse_event(MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero)
        End Sub

        ' Properties
        Public ReadOnly Property MousePresent As Boolean
            Get
                Return SystemInformation.MousePresent
            End Get
        End Property

        Public ReadOnly Property WheelExists As Boolean
            Get
                If Not SystemInformation.MousePresent Then
                    Throw New InvalidOperationException("没有找到鼠标.")
                End If
                Return SystemInformation.MouseWheelPresent
            End Get
        End Property

        Public ReadOnly Property WheelScrollLines As Integer
            Get
                If Not WheelExists Then
                    Throw New InvalidOperationException("没有找到鼠标滑轮.")
                End If
                Return SystemInformation.MouseWheelScrollLines
            End Get
        End Property
    End Module
End Namespace
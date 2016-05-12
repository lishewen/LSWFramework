Imports System.Drawing
Imports System.Windows.Forms
Imports System.Runtime.CompilerServices

Namespace Extension
    Public Module DraggerHelper
        ''' <summary>
        ''' 光标状态
        ''' </summary>
        Private Enum EnumMousePointPosition
            MouseSizeNone = 0             '无   
            MouseSizeRight = 1            '拉伸右边框   
            MouseSizeLeft = 2            '拉伸左边框   
            MouseSizeBottom = 3            '拉伸下边框   
            MouseSizeTop = 4            '拉伸上边框   
            MouseSizeTopLeft = 5            '拉伸左上角   
            MouseSizeTopRight = 6            '拉伸右上角   
            MouseSizeBottomLeft = 7            '拉伸左下角   
            MouseSizeBottomRight = 8            '拉伸右下角   
            MouseDrag = 9             '鼠标拖动
        End Enum

        Private Const Band As Integer = 5
        Private Const MinWidth As Integer = 10
        Private Const MinHeight As Integer = 10
        Private m_MousePointPosition As EnumMousePointPosition

        Friend Class DragControlProperty
            Public Property PositionMovePoint As Point
            Public Property SizeChangeMovePoint As Point
        End Class

        Private controlPropertyDic As New Dictionary(Of Control, DragControlProperty)()

        <Extension>
        Public Function IsControlCanDrag(control As Control) As Boolean
            Return controlPropertyDic.ContainsKey(control)
        End Function

        <Extension>
        Public Sub EnableDrag(control As Control)
            If Not controlPropertyDic.ContainsKey(control) Then
                controlPropertyDic.Add(control, New DragControlProperty())

                RegisterControlEvents(control)
            End If
        End Sub

        <Extension>
        Public Sub DisableDrag(control As Control)
            If controlPropertyDic.ContainsKey(control) Then
                RemoveHandler control.MouseDown, AddressOf control_MouseDown
                RemoveHandler control.MouseMove, AddressOf control_MouseMove
                RemoveHandler control.MouseLeave, AddressOf control_MouseLeave

                controlPropertyDic.Remove(control)
            End If
        End Sub

        Private Sub RegisterControlEvents(control As Control)
            AddHandler control.MouseDown, AddressOf control_MouseDown
            AddHandler control.MouseMove, AddressOf control_MouseMove
            AddHandler control.MouseLeave, AddressOf control_MouseLeave
        End Sub

        Private Sub control_MouseLeave(sender As Object, e As EventArgs)
            m_MousePointPosition = EnumMousePointPosition.MouseSizeNone
            Cursor.Current = Cursors.Arrow
        End Sub

        Private Sub control_MouseMove(sender As Object, e As MouseEventArgs)
            Dim lCtrl As Control = TryCast(sender, Control)
            Dim [property] As DragControlProperty = GetControlProperty(lCtrl)

            If e.Button = MouseButtons.Left Then
                Select Case m_MousePointPosition
                    Case EnumMousePointPosition.MouseDrag
                        lCtrl.Left = lCtrl.Left + e.X - [property].PositionMovePoint.X
                        lCtrl.Top = lCtrl.Top + e.Y - [property].PositionMovePoint.Y
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeBottom
                        lCtrl.Height = lCtrl.Height + e.Y - [property].SizeChangeMovePoint.Y
                        [property].SizeChangeMovePoint = e.Location                        '记录光标拖动的当前点   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeBottomRight
                        lCtrl.Width = lCtrl.Width + e.X - [property].SizeChangeMovePoint.X
                        lCtrl.Height = lCtrl.Height + e.Y - [property].SizeChangeMovePoint.Y
                        [property].SizeChangeMovePoint = e.Location                        '记录光标拖动的当前点   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeRight
                        lCtrl.Width = lCtrl.Width + e.X - [property].SizeChangeMovePoint.X   
                        [property].SizeChangeMovePoint = e.Location                        '记录光标拖动的当前点   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeTop
                        lCtrl.Top = lCtrl.Top + (e.Y - [property].PositionMovePoint.Y)
                        lCtrl.Height = lCtrl.Height - (e.Y - [property].PositionMovePoint.Y)
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeLeft
                        lCtrl.Left = lCtrl.Left + e.X - [property].PositionMovePoint.X
                        lCtrl.Width = lCtrl.Width - (e.X - [property].PositionMovePoint.X)
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeBottomLeft
                        lCtrl.Left = lCtrl.Left + e.X - [property].PositionMovePoint.X
                        lCtrl.Width = lCtrl.Width - (e.X - [property].PositionMovePoint.X)
                        lCtrl.Height = lCtrl.Height + e.Y - [property].SizeChangeMovePoint.Y
                        [property].SizeChangeMovePoint = e.Location                        '记录光标拖动的当前点   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeTopRight
                        lCtrl.Top = lCtrl.Top + (e.Y - [property].PositionMovePoint.Y)
                        lCtrl.Width = lCtrl.Width + (e.X - [property].SizeChangeMovePoint.X)
                        lCtrl.Height = lCtrl.Height - (e.Y - [property].PositionMovePoint.Y)
                        [property].SizeChangeMovePoint = e.Location                        '记录光标拖动的当前点   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeTopLeft
                        lCtrl.Left = lCtrl.Left + e.X - [property].PositionMovePoint.X
                        lCtrl.Top = lCtrl.Top + (e.Y - [property].PositionMovePoint.Y)
                        lCtrl.Width = lCtrl.Width - (e.X - [property].PositionMovePoint.X)
                        lCtrl.Height = lCtrl.Height - (e.Y - [property].PositionMovePoint.Y)
                        Exit Select
                    Case Else
                        Exit Select
                End Select
                If lCtrl.Width < MinWidth Then
                    lCtrl.Width = MinWidth
                End If
                If lCtrl.Height < MinHeight Then
                    lCtrl.Height = MinHeight
                End If
            Else
                m_MousePointPosition = MousePointPosition(lCtrl.Size, e)                '判断光标的位置状态   
                Select Case m_MousePointPosition                    '改变光标   
                    Case EnumMousePointPosition.MouseSizeNone
                        Cursor.Current = Cursors.Arrow                        '箭头   
                        Exit Select
                    Case EnumMousePointPosition.MouseDrag
                        Cursor.Current = Cursors.SizeAll                        '四方向   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeBottom
                        Cursor.Current = Cursors.SizeNS                        '南北   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeTop
                        Cursor.Current = Cursors.SizeNS                        '南北   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeLeft
                        Cursor.Current = Cursors.SizeWE                        '东西   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeRight
                        Cursor.Current = Cursors.SizeWE                        '东西   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeBottomLeft
                        Cursor.Current = Cursors.SizeNESW                        '东北到南西   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeBottomRight
                        Cursor.Current = Cursors.SizeNWSE                        '东南到西北   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeTopLeft
                        Cursor.Current = Cursors.SizeNWSE                        '东南到西北   
                        Exit Select
                    Case EnumMousePointPosition.MouseSizeTopRight
                        Cursor.Current = Cursors.SizeNESW                        '东北到南西   
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            End If
        End Sub

        Private Sub control_MouseDown(sender As Object, e As MouseEventArgs)
            Dim [property] = GetControlProperty(TryCast(sender, Control))

            [property].PositionMovePoint = e.Location
            [property].SizeChangeMovePoint = e.Location
        End Sub

        Private Function GetControlProperty(control As Control) As DragControlProperty
            Return controlPropertyDic(control)
        End Function

        Private Function MousePointPosition(size As Size, e As System.Windows.Forms.MouseEventArgs) As EnumMousePointPosition
            If (e.X >= -1 * Band) Or (e.X <= size.Width) Or (e.Y >= -1 * Band) Or (e.Y <= size.Height) Then
                If e.X < Band Then
                    If e.Y < Band Then
                        Return EnumMousePointPosition.MouseSizeTopLeft
                    Else
                        If e.Y > -1 * Band + size.Height Then
                            Return EnumMousePointPosition.MouseSizeBottomLeft
                        Else
                            Return EnumMousePointPosition.MouseSizeLeft
                        End If
                    End If
                Else
                    If e.X > -1 * Band + size.Width Then
                        If e.Y < Band Then
                            Return EnumMousePointPosition.MouseSizeTopRight
                        Else
                            If e.Y > -1 * Band + size.Height Then
                                Return EnumMousePointPosition.MouseSizeBottomRight
                            Else
                                Return EnumMousePointPosition.MouseSizeRight
                            End If
                        End If
                    Else
                        If e.Y < Band Then
                            Return EnumMousePointPosition.MouseSizeTop
                        Else
                            If e.Y > -1 * Band + size.Height Then
                                Return EnumMousePointPosition.MouseSizeBottom
                            Else
                                Return EnumMousePointPosition.MouseDrag
                            End If
                        End If
                    End If
                End If
            Else
                Return EnumMousePointPosition.MouseSizeNone
            End If
        End Function
    End Module
End Namespace
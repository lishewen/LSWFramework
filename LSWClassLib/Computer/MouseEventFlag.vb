Namespace Computer
    <Flags>
    Public Enum MouseEventFlag As UInt32
        ' Fields
        Absolute = &H8000
        LeftDown = 2
        LeftUp = 4
        MiddleDown = &H20
        MiddleUp = &H40
        Move = 1
        RightDown = 8
        RightUp = &H10
        VirtualDesk = &H4000
        Wheel = &H800
        XDown = &H80
        XUp = &H100
    End Enum
End Namespace
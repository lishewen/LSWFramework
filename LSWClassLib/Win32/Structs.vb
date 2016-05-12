Imports System.Runtime.InteropServices

Namespace Win32
    <StructLayout(LayoutKind.Sequential)> _
    Public Structure BLENDFUNCTION
        ''' <summary>
        ''' 
        ''' </summary>
        Public BlendOp As Byte
        ''' <summary>
        ''' 
        ''' </summary>
        Public BlendFlags As Byte
        ''' <summary>
        ''' 
        ''' </summary>
        Public SourceConstantAlpha As Byte
        ''' <summary>
        ''' 
        ''' </summary>
        Public AlphaFormat As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure MFT_ENUM_DATA
        Dim StartFileReferenceNumber As Long
        Dim LowUsn As Long
        Dim HighUsn As Long
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure OBJECT_ATTRIBUTES
        Dim Length As Integer
        Dim RootDirectory As IntPtr
        Dim ObjectName As IntPtr
        Dim Attributes As Integer
        Dim SecurityDescriptor As Integer
        Dim SecurityQualityOfService As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure IO_STATUS_BLOCK
        Dim Status As Integer
        Dim Information As Integer
    End Structure
End Namespace
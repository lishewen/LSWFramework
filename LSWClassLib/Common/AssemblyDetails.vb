Namespace Common
    Public Enum CPUVersion
        X86
        X64
        AnyCPU
    End Enum

    Public Class AssemblyDetails
        Public Property CPUVersion As CPUVersion
        Public Property HasStrongName As Boolean
    End Class
End Namespace
Imports System
Imports System.IO

Namespace Files.SevenZip
    Public Interface ICoder
        ' Methods
        Sub Code(ByVal inStream As Stream, ByVal outStream As Stream, ByVal inSize As Long, ByVal outSize As Long, ByVal progress As ICodeProgress)
    End Interface
End Namespace


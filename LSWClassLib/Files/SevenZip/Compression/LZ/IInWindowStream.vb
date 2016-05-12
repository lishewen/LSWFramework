Imports System
Imports System.IO

Namespace Files.SevenZip.Compression.LZ
    Friend Interface IInWindowStream
        ' Methods
        Function GetIndexByte(ByVal index As Integer) As Byte
        Function GetMatchLen(ByVal index As Integer, ByVal distance As UInt32, ByVal limit As UInt32) As UInt32
        Function GetNumAvailableBytes() As UInt32
        Sub Init()
        Sub ReleaseStream()
        Sub SetStream(ByVal inStream As Stream)
    End Interface
End Namespace


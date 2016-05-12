Imports System

Namespace Files.SevenZip.Compression.LZ
    Friend Interface IMatchFinder
        Inherits IInWindowStream
        ' Methods
        Sub Create(ByVal historySize As UInt32, ByVal keepAddBufferBefore As UInt32, ByVal matchMaxLen As UInt32, ByVal keepAddBufferAfter As UInt32)
        Function GetMatches(ByVal distances As UInt32()) As UInt32
        Sub Skip(ByVal num As UInt32)
    End Interface
End Namespace


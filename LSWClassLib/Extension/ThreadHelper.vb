Imports System.Runtime.CompilerServices

Namespace Extension
    Public Module ThreadHelper
        <Extension>
        Public Function WatchTime(Of T)(ByVal work As Func(Of T)) As String
            Dim sw = Stopwatch.StartNew()
            Dim result = work()
            Return sw.Elapsed.ToString() & ": " & result.ToString()
        End Function

        <Extension>
        Public Function WatchTime(ByVal work As Action) As String
            Dim sw = Stopwatch.StartNew()
            work()
            Return sw.Elapsed.ToString()
        End Function
    End Module
End Namespace
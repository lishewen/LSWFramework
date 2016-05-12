Imports System.Runtime.CompilerServices

Namespace Extension
    Public Module DataHelper
        <Extension>
        Public Sub Save(Of T)(list As List(Of T))
            Dim filename = GetType(T).Name & ".lsw"
            My.Computer.FileSystem.WriteAllText(filename, list.XmlSerialize, False)
        End Sub

        Public Function Load(Of T)() As List(Of T)
            Dim filename = GetType(T).Name & ".lsw"
            If My.Computer.FileSystem.FileExists(filename) Then
                Return My.Computer.FileSystem.ReadAllText(filename).XmlDeserialize(Of List(Of T))
            Else
                Return New List(Of T)
            End If
        End Function

        <Extension>
        Public Sub Save(Of T)(list As List(Of T), action As String)
            Dim filename = GetType(T).Name & "_" & action & ".lsw"
            My.Computer.FileSystem.WriteAllText(filename, list.XmlSerialize, False)
        End Sub

        Public Function Load(Of T)(action As String) As List(Of T)
            Dim filename = GetType(T).Name & "_" & action & ".lsw"
            If My.Computer.FileSystem.FileExists(filename) Then
                Return My.Computer.FileSystem.ReadAllText(filename).XmlDeserialize(Of List(Of T))()
            Else
                Return New List(Of T)
            End If
        End Function
    End Module
End Namespace
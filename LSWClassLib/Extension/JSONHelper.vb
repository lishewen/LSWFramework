Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization.Json
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Extension
    Public Module JSONHelper
        <Extension()>
        Public Function JSONToObj(Of T)(str As String) As T
            Dim ser As New DataContractJsonSerializer(GetType(T))
            Dim obj As T
            Using ms = New MemoryStream(Encoding.UTF8.GetBytes(str))
                obj = DirectCast(ser.ReadObject(ms), T)
            End Using
            Return obj
        End Function

        <Extension()>
        Public Function ObjToJson(Of T)(obj As T) As String
            If GetType(T).FullName.Contains("<>f__AnonymousType") Then
                Dim serializer As New JavaScriptSerializer
                Return serializer.Serialize(obj)
            Else
                Dim ser As New DataContractJsonSerializer(GetType(T))
                Using ms = New MemoryStream
                    ser.WriteObject(ms, obj)
                    Return Encoding.UTF8.GetString(ms.ToArray)
                End Using
            End If
        End Function
    End Module
End Namespace
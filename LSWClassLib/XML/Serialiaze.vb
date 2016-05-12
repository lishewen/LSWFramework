Imports System.IO
Imports System.Xml.Serialization

Namespace XML
    Public Module Serialiaze

        ''' <summary>
        ''' 序列化对象
        ''' </summary>
        ''' <typeparam name="T">对象类型</typeparam>
        ''' <param name="c">对象实例</param>
        ''' <param name="stream">序列化流</param>
        ''' <remarks></remarks>
        Public Sub Serialiaze(Of T)(ByVal c As T, ByRef stream As Stream)
            Dim xs As New XmlSerializer(GetType(T))
            xs.Serialize(stream, c)
            stream.Close()
        End Sub

        ''' <summary>
        ''' 反序列化
        ''' </summary>
        ''' <typeparam name="T">类型</typeparam>
        ''' <param name="stream">数据流</param>
        ''' <returns>返回的实例</returns>
        ''' <remarks></remarks>
        Public Function Deserialize(Of T)(ByVal stream As Stream) As T
            Dim xs As New XmlSerializer(GetType(T))
            Return CType(xs.Deserialize(stream), T)
        End Function
    End Module
End Namespace
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports System.Text
Imports System.Xml
Imports System.Windows.Forms
Imports LSW.Model

Namespace Extension
    Public Module SerializeHelper

        ''' <summary>
        ''' 将Byte数组还原为对象
        ''' </summary>
        ''' <typeparam name="T">对象的类型</typeparam>
        ''' <param name="Bin">Byte数组</param>
        ''' <returns>对象</returns>
        ''' <remarks></remarks>
        <Extension()> _
        Public Function BinToObj(Of T)(ByVal Bin() As Byte) As T
            Dim Formatter As New BinaryFormatter
            Dim StreamTemp As New MemoryStream(Bin)
            Dim obj = CType(Formatter.Deserialize(StreamTemp), T)
            StreamTemp.Close()
            Return obj
        End Function

        ''' <summary>
        ''' 将对象序列化成Byte数组
        ''' </summary>
        ''' <typeparam name="T">对象类型</typeparam>
        ''' <param name="Obj">对象</param>
        ''' <returns>Byte数组</returns>
        ''' <remarks></remarks>
        <Extension()> _
        Public Function ObjToBin(Of T)(ByVal Obj As T) As Byte()
            Dim StreamTemp As New MemoryStream
            Dim Formatter As New BinaryFormatter
            Formatter.Serialize(StreamTemp, Obj)
            StreamTemp.Close()
            Return StreamTemp.GetBuffer()
        End Function

        ''' <summary>
        ''' 对象序列化成 XML String
        ''' </summary>
        <Extension()>
        Public Function XmlSerialize(Of T)(obj As T) As String
            Dim xmlString As String = String.Empty
            Dim xmlSerializer As New XmlSerializer(GetType(T))
            Using ms As New MemoryStream()
                xmlSerializer.Serialize(ms, obj)
                xmlString = Encoding.UTF8.GetString(ms.ToArray())
            End Using
            Return xmlString
        End Function

        ''' <summary>
        ''' XML String 反序列化成对象
        ''' </summary>
        <Extension()>
        Public Function XmlDeserialize(Of T)(xmlString As String) As T
            Dim obj As T = Nothing
            Dim xmlSerializer As New XmlSerializer(GetType(T))
            Using xmlStream As Stream = New MemoryStream(Encoding.UTF8.GetBytes(xmlString))
                Using xr As XmlReader = XmlReader.Create(xmlStream)
                    Dim o = xmlSerializer.Deserialize(xr)
                    obj = DirectCast(o, T)
                End Using
            End Using
            Return obj
        End Function

        ''' <summary>
        ''' XML String 反序列化成对象
        ''' </summary>
        <Extension>
        Public Function XmlDeserialize(Of T)(stream As Stream) As T
            Dim xs As New XmlSerializer(GetType(T))
            Return DirectCast(xs.Deserialize(stream), T)
        End Function

        <Extension>
        Public Function ToXml(tree As TreeView) As String
            Dim d As New TreeViewData(tree)
            Return d.XmlSerialize
        End Function

        <Extension>
        Public Sub LoadXml(tree As TreeView, xml As String)
            Dim d = xml.XmlDeserialize(Of TreeViewData)()
            d.PopulateTree(tree)
        End Sub
    End Module
End Namespace
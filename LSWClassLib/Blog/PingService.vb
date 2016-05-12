Imports System.Net
Imports System.IO
Imports System.Xml
Imports System.Text
Imports LSW.Exceptions

Namespace Blog
    Public Module PingService

        Private _InstanceName As String = "算神工作室"
        Private _AbsoluteWebRoot As String = "http://blog.lishewen.com.cn"

        Public Property InstanceName() As String
            Get
                Return _InstanceName
            End Get
            Set(ByVal value As String)
                _InstanceName = value
            End Set
        End Property

        Public Property AbsoluteWebRoot() As String
            Get
                Return _AbsoluteWebRoot
            End Get
            Set(ByVal value As String)
                _AbsoluteWebRoot = value
            End Set
        End Property

        Public Sub Ping(ByVal services() As String)
            For Each service In services
                Execute(service)
            Next
        End Sub

        Public Sub Execute(ByVal url As String)
            Try
                Dim request = DirectCast(WebRequest.Create(url), HttpWebRequest)
                request.Method = "POST"
                request.ContentType = "text/xml"
                request.Timeout = 3000
                request.Credentials = CredentialCache.DefaultNetworkCredentials
                AddXmlToRequest(request)
                request.GetResponse()
            Catch ex As Exception
                Dim e As New LSWFrameworkException(ex)
            End Try
        End Sub

        Private Sub AddXmlToRequest(ByVal request As HttpWebRequest)
            Dim stream = DirectCast(request.GetRequestStream, Stream)
            Using writer As New XmlTextWriter(stream, Encoding.ASCII)
                writer.WriteStartDocument()
                writer.WriteStartElement("methodCall")
                writer.WriteElementString("methodName", "weblogUpdates.ping")
                writer.WriteStartElement("params")
                writer.WriteStartElement("param")
                writer.WriteElementString("value", _InstanceName)
                writer.WriteEndElement()
                writer.WriteStartElement("param")
                writer.WriteElementString("value", _AbsoluteWebRoot)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()
            End Using
        End Sub

    End Module
End Namespace
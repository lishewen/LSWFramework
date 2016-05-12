Imports LSW.Extension
Imports System.Drawing
Imports LSW.Image
Imports System.Reflection
Imports System.Drawing.Imaging
Imports System.Collections.Specialized
Imports LSW.Net
Imports System.Text
Imports System.Runtime.Serialization

Namespace Exceptions
    Public Class LSWFrameworkException
        Inherits Exception

        Private BaseEx As Exception
        Private ErrBmp As Bitmap
        Private computername As String
        Private softname As String

        <Obsolete("暂时取消支持")>
        Public Event Send(sender As LSWFrameworkException)
        Public Delegate Sub SendTo(sender As LSWFrameworkException)

        'Public Sub New()
        '    MyBase.New()
        'End Sub

        'Protected Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
        '    MyBase.New()
        'End Sub

        Public Sub New(message As String)
            MyClass.New(New Exception(message))
        End Sub

        Public Sub New(message As String, sendto As SendTo)
            MyClass.New(New Exception(message), sendto)
        End Sub

        Public Sub New(ex As Exception, sendto As SendTo)
            BaseEx = ex
            ErrBmp = PicHelper.ScreenToBitmap
            computername = My.Computer.Name
            softname = Assembly.GetEntryAssembly.FullName

            Try
                sendto(Me)
            Catch e As Exception
                SaveToLocal()
            End Try
        End Sub

        Public Sub New(ex As Exception)
            BaseEx = ex
            ErrBmp = PicHelper.ScreenToBitmap
            computername = My.Computer.Name
            softname = Assembly.GetEntryAssembly.FullName

            Try
                'If My.Computer.Network.Ping("softwave.lishewen.com") Then
                SendToUrl()
                'Else
                'SaveToLocal()
                'End If
            Catch e As Exception
                SaveToLocal()
            End Try
        End Sub

        Public Overrides Function GetBaseException() As Exception
            Return BaseEx
        End Function

        Public Overrides ReadOnly Property Message As String
            Get
                Return BaseEx.Message
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim xml = <Report ComputerName=<%= computername %> SoftName=<%= softname %> DataTime=<%= Now %>>
                          <Datas>
                              <Data Key=<%= From entry In BaseEx.Data.Cast(Of DictionaryEntry)() Select entry.Key %>>
                                  <%= From entry In BaseEx.Data.Cast(Of DictionaryEntry)() Select entry.Value %>
                              </Data>
                          </Datas>
                          <ExceptionType><%= BaseEx.GetType.FullName %></ExceptionType>
                          <Message><%= BaseEx.Message %></Message>
                          <Source><%= BaseEx.Source %></Source>
                          <StackTrace><%= BaseEx.StackTrace %></StackTrace>
                          <TargetSite><%= BaseEx.TargetSite.Name %></TargetSite>
                      </Report>
            Return xml.ToString
        End Function

        'Public Overrides Property Source As String
        '    Get
        '        Return Now & ": " & computername & "-" & softname & vbCrLf & BaseEx.Source
        '    End Get
        '    Set(value As String)
        '        BaseEx.Source = value
        '    End Set
        'End Property

        Public Sub SaveToLocal()
            If Not My.Computer.FileSystem.DirectoryExists("Logs") Then
                My.Computer.FileSystem.CreateDirectory("Logs")
            End If

            Dim filename = "Logs\" & Guid.NewGuid.ToString.Replace("-", "")
            SaveTxt(filename & ".exception")
            SaveBmp(filename & ".jpg")
        End Sub

        Public Sub SaveTxt(filename As String)
            My.Computer.FileSystem.WriteAllText(filename, Me.ToString, False)
        End Sub

        Public Sub SaveBmp(filename As String)
            ErrBmp.Save(filename, ImageFormat.Jpeg)
        End Sub

        Public Sub SendToUrl()
            Const FeedBackUrl = "http://softwave.lishewen.com/api/feedback"
            Dim datas As New NameValueCollection
            datas.Add("a", "Exception")
            datas.Add("t", Me.ToString)
            datas.Add("b", ErrBmp.ToBase64String)
            If String.IsNullOrWhiteSpace(Url.Post(datas, FeedBackUrl, Encoding.UTF8)) Then
                Throw New ArgumentException("网址不存在 或 提交失败")
            End If
        End Sub

        'Public Overrides Sub GetObjectData(info As SerializationInfo, context As StreamingContext)
        '    info.AddValue("DataTime", Now)
        '    info.AddValue("ComputerName", computername)
        '    info.AddValue("SoftName", softname)
        '    info.AddValue("Data", "BaseEx.Data")
        '    For Each d As DictionaryEntry In BaseEx.Data
        '        info.AddValue("Data_" & d.Key, d.Value)
        '    Next
        '    info.AddValue("ExceptionType", BaseEx.GetType.FullName)
        '    info.AddValue("Message", BaseEx.Message)
        '    info.AddValue("Source", BaseEx.Source)
        '    info.AddValue("StackTrace", BaseEx.StackTrace)
        '    info.AddValue("TargetSite", BaseEx.TargetSite.Name)
        '    MyBase.GetObjectData(info, context)
        'End Sub
    End Class
End Namespace
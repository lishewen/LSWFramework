'****************************** Module Header ******************************'
' Module Name:  HttpDownloadClient.vb
' Project:	    VBMultiThreadedWebDownloader
' Copyright (c) Microsoft Corporation.
' 
' This class is used to download files through internet.  It supplies public
' methods to Start, Pause, Resume and Cancel a download. One client will use 
' a single thread to download part of the whole file. The property StartPoint 
' can be used in the multi-thread download scenario, and every thread starts 
' to download a specific block of the whole file. 
' 
' The downloaded data is stored in a MemoryStream first, and then written to local
' file.
' 
' It will fire a DownloadProgressChanged event when it has downloaded a
' specified size of data. It will also fire a DownloadCompleted event if the 
' download is completed or canceled.
' 
' The property DownloadedSize stores the size of downloaded data which will be 
' used to Resume the download.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.IO
Imports System.Net
Imports System.Threading

Namespace Web
    Public Class HttpDownloadClient
        ' Used when creates or writes a file.
        Private Shared _locker As New Object()

        ' The Url of the file to be downloaded.
        Private _url As Uri
        Public Property Url() As Uri
            Get
                Return _url
            End Get
            Private Set(ByVal value As Uri)
                _url = value
            End Set
        End Property

        ' The local path to store the file.
        ' If there is no file with the same name, a new file will be created.
        Private _downloadPath As String
        Public Property DownloadPath() As String
            Get
                Return _downloadPath
            End Get
            Private Set(ByVal value As String)
                _downloadPath = value
            End Set
        End Property

        ' The properties StartPoint and EndPoint can be used in the multi-thread download 
        ' scenario, and every thread starts to download a specific block of the whole file. 
        Private _startPoint As Long
        Public Property StartPoint() As Long
            Get
                Return _startPoint
            End Get
            Private Set(ByVal value As Long)
                _startPoint = value
            End Set
        End Property

        Private _endPoint As Long
        Public Property EndPoint() As Long
            Get
                Return _endPoint
            End Get
            Private Set(ByVal value As Long)
                _endPoint = value
            End Set
        End Property

        Public Property Proxy() As WebProxy

        ' Set the BufferSize when read data in Response Stream.
        Private _bufferSize As Integer
        Public Property BufferSize() As Integer
            Get
                Return _bufferSize
            End Get
            Private Set(ByVal value As Integer)
                _bufferSize = value
            End Set
        End Property

        ' The cache size in memory.
        Private _maxCacheSize As Integer
        Public Property MaxCacheSize() As Integer
            Get
                Return _maxCacheSize
            End Get
            Private Set(ByVal value As Integer)
                _maxCacheSize = value
            End Set
        End Property



        ' Ask the server for the file size and store it
        Public Property TotalSize() As Long


        ' The size of downloaded data that has been written to local file.
        Private _downloadedSize As Long
        Public Property DownloadedSize() As Long
            Get
                Return _downloadedSize
            End Get
            Private Set(ByVal value As Long)
                _downloadedSize = value
            End Set
        End Property

        Private _status As HttpDownloadClientStatus

        ' If status changed, fire StatusChanged event.
        Public Property Status() As HttpDownloadClientStatus
            Get
                Return _status
            End Get

            Private Set(ByVal value As HttpDownloadClientStatus)
                If _status <> value Then
                    _status = value
                    Me.OnStatusChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        Public Event DownloadProgressChanged _
            As EventHandler(Of HttpDownloadClientProgressChangedEventArgs)

        Public Event ErrorOccurred As EventHandler(Of ErrorEventArgs)

        Public Event StatusChanged As EventHandler

        ''' <summary>
        ''' Download the whole file.
        ''' </summary>
        Public Sub New(ByVal url As Uri, ByVal downloadPath As String)
            Me.New(url, downloadPath, 0)
        End Sub

        ''' <summary>
        ''' Download the file from a start point to the end.
        ''' </summary>
        Public Sub New(ByVal url As Uri, ByVal downloadPath As String,
                       ByVal startPoint As Long)
            Me.New(url, downloadPath, startPoint, Long.MaxValue)
        End Sub

        ''' <summary>
        ''' Download a block of the file. The default buffer size is 1KB, memory cache is
        ''' 1MB, and buffer count per notification is 64.
        ''' </summary>
        Public Sub New(ByVal url As Uri, ByVal downloadPath As String,
                       ByVal startPoint As Long, ByVal endPoint As Long)
            Me.New(url, downloadPath, startPoint, endPoint, 1024, 1048576)
        End Sub

        Public Sub New(ByVal url As Uri, ByVal downloadPath As String,
                       ByVal startPoint As Long, ByVal endPoint As Long,
                       ByVal bufferSize As Integer, ByVal cacheSize As Integer)
            If startPoint < 0 Then
                Throw New ArgumentOutOfRangeException(
                    "StartPoint cannot be less than 0. ")
            End If

            If endPoint < startPoint Then
                Throw New ArgumentOutOfRangeException(
                    "EndPoint cannot be less than StartPoint ")
            End If

            If bufferSize < 0 Then
                Throw New ArgumentOutOfRangeException(
                    "BufferSize cannot be less than 0. ")
            End If

            If cacheSize < bufferSize Then
                Throw New ArgumentOutOfRangeException(
                    "MaxCacheSize cannot be less than BufferSize ")
            End If


            Me.StartPoint = startPoint
            Me.EndPoint = endPoint
            Me.BufferSize = bufferSize
            Me.MaxCacheSize = cacheSize

            Me.Url = url
            Me.DownloadPath = downloadPath

            ' Set the idle status.
            Me._status = HttpDownloadClientStatus.Idle

        End Sub

        ''' <summary>
        ''' Start to download.
        ''' </summary>
        Public Sub Start()

            ' Only idle download client can be started.
            If Me.Status <> HttpDownloadClientStatus.Idle Then
                Throw New ApplicationException(
                    "Only idle download client can be started.")
            End If

            ' Start to download in a background thread.
            BeginDownload()
        End Sub

        ''' <summary>
        ''' Pause the download.
        ''' </summary>
        Public Sub Pause()
            ' Only downloading client can be paused.
            If Me.Status <> HttpDownloadClientStatus.Downloading Then
                Throw New ApplicationException(
                    "Only downloading client can be paused.")
            End If

            ' The background thread will check the status. If it is Pausing, the download
            ' will be paused and the status will be changed to Paused.
            Me.Status = HttpDownloadClientStatus.Pausing
        End Sub

        ''' <summary>
        ''' Resume the download.
        ''' </summary>
        Public Sub [Resume]()
            ' Only paused client can be resumed.
            If Me.Status <> HttpDownloadClientStatus.Paused Then
                Throw New ApplicationException("Only paused client can be resumed.")
            End If

            ' Start to download in a background thread.
            BeginDownload()
        End Sub

        ''' <summary>
        ''' Cancel the download
        ''' </summary>
        Public Sub Cancel()
            ' Only a downloading or paused client can be canceled.
            If Me.Status <> HttpDownloadClientStatus.Paused _
                AndAlso Me.Status <> HttpDownloadClientStatus.Downloading Then
                Throw New ApplicationException(
                    "Only a downloading or paused client can be canceled.")
            End If

            ' The background thread will check the status. If it is Canceling, the download
            ' will be canceled and the status will be changed to Canceled.
            Me.Status = HttpDownloadClientStatus.Canceling
        End Sub

        ''' <summary>
        ''' Create a thread to download data.
        ''' </summary>
        Private Sub BeginDownload()
            Dim threadStart_Renamed As New ThreadStart(AddressOf Download)
            Dim downloadThread As New Thread(threadStart_Renamed)
            downloadThread.IsBackground = True
            downloadThread.Start()
        End Sub

        ''' <summary>
        ''' Download the data using HttpWebRequest. It will read a buffer of bytes from the
        ''' response stream, and store the buffer to a MemoryStream cache first.
        ''' If the cache is full, or the download is paused, canceled or completed, write
        ''' the data in cache to local file.
        ''' </summary>
        Private Sub Download()
            Dim request As HttpWebRequest = Nothing
            Dim response As HttpWebResponse = Nothing
            Dim responseStream As Stream = Nothing
            Dim downloadCache As MemoryStream = Nothing

            ' Set the status.
            Me.Status = HttpDownloadClientStatus.Downloading

            Try

                ' Create a request to the file to be  downloaded.
                request = CType(WebRequest.Create(Url), HttpWebRequest)
                request.Method = "GET"
                request.Credentials = CredentialCache.DefaultCredentials


                ' Specify the block to download.
                If EndPoint <> Long.MaxValue Then
                    request.AddRange(StartPoint + DownloadedSize, EndPoint)
                Else
                    request.AddRange(StartPoint + DownloadedSize)
                End If

                ' Set the proxy.
                If Proxy IsNot Nothing Then
                    request.Proxy = Proxy
                End If

                ' Retrieve the response from the server and get the response stream.
                response = CType(request.GetResponse(), HttpWebResponse)

                responseStream = response.GetResponseStream()


                ' Cache data in memory.
                downloadCache = New MemoryStream(Me.MaxCacheSize)

                Dim downloadBuffer(Me.BufferSize - 1) As Byte

                Dim bytesSize As Integer = 0
                Dim cachedSize As Integer = 0

                ' Download the file until the download is paused, canceled or completed.
                Do

                    ' Read a buffer of data from the stream.
                    bytesSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length)

                    ' If the cache is full, or the download is paused, canceled or 
                    ' completed, write the data in cache to local file.
                    If Me.Status <> HttpDownloadClientStatus.Downloading _
                        OrElse bytesSize = 0 _
                        OrElse Me.MaxCacheSize < cachedSize + bytesSize Then
                        Try
                            ' Write the data in cache to local file.
                            WriteCacheToFile(downloadCache, cachedSize)

                            Me.DownloadedSize += cachedSize

                            ' Stop downloading the file if the download is paused, 
                            ' canceled or completed. 
                            If Me.Status <> HttpDownloadClientStatus.Downloading _
                                OrElse bytesSize = 0 Then
                                Exit Do
                            End If

                            ' Reset cache.
                            downloadCache.Seek(0, SeekOrigin.Begin)
                            cachedSize = 0
                        Catch ex As Exception
                            ' Fire the DownloadCompleted event with the error.
                            Me.OnError(New ErrorEventArgs With {.Exception = ex})
                            Return
                        End Try
                    End If

                    ' Write the data from the buffer to the cache in memory.
                    downloadCache.Write(downloadBuffer, 0, bytesSize)

                    cachedSize += bytesSize

                    ' Fire the DownloadProgressChanged event.
                    OnDownloadProgressChanged(
                        New HttpDownloadClientProgressChangedEventArgs With
                        {
                            .Size = bytesSize
                        })
                Loop

                ' Update the status of the client. Above loop will be stopped when the 
                ' status of the client is pausing, canceling or completed.
                If Me.Status = HttpDownloadClientStatus.Pausing Then
                    Me.Status = HttpDownloadClientStatus.Paused
                ElseIf Me.Status = HttpDownloadClientStatus.Canceling Then
                    Me.Status = HttpDownloadClientStatus.Canceled

                    Dim ex As New Exception("Downloading is canceled by user's request. ")

                    Me.OnError(New ErrorEventArgs With {.Exception = ex})
                Else
                    Me.Status = HttpDownloadClientStatus.Completed
                    Return
                End If

            Finally
                ' When the above code has ended, close the streams.
                If responseStream IsNot Nothing Then
                    responseStream.Close()
                End If
                If response IsNot Nothing Then
                    response.Close()
                End If
                If downloadCache IsNot Nothing Then
                    downloadCache.Close()
                End If
            End Try
        End Sub



        ''' <summary>
        ''' Write the data in cache to local file.
        ''' </summary>
        Private Sub WriteCacheToFile(ByVal downloadCache As MemoryStream,
                                     ByVal cachedSize As Integer)
            ' Lock other threads or processes to prevent from writing data to the file.
            SyncLock _locker
                Using fileStream_Renamed As New FileStream(DownloadPath, FileMode.Open)
                    Dim cacheContent(cachedSize - 1) As Byte
                    downloadCache.Seek(0, SeekOrigin.Begin)
                    downloadCache.Read(cacheContent, 0, cachedSize)
                    fileStream_Renamed.Seek(DownloadedSize + StartPoint, SeekOrigin.Begin)
                    fileStream_Renamed.Write(cacheContent, 0, cachedSize)
                End Using
            End SyncLock
        End Sub

        ''' <summary>
        ''' Raise the ErrorOccurred event.
        ''' </summary>
        Protected Overridable Sub OnError(ByVal e As ErrorEventArgs)
            RaiseEvent ErrorOccurred(Me, e)
        End Sub

        ''' <summary>
        ''' Raise the DownloadProgressChanged event.
        ''' </summary>
        Protected Overridable Sub OnDownloadProgressChanged(ByVal e As HttpDownloadClientProgressChangedEventArgs)
            RaiseEvent DownloadProgressChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raise the StatusChanged event.
        ''' </summary>
        Protected Overridable Sub OnStatusChanged(ByVal e As EventArgs)
            RaiseEvent StatusChanged(Me, e)
        End Sub
    End Class
End Namespace

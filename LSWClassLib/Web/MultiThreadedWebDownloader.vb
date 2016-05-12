'****************************** Module Header ******************************'
' Module Name:  MultiThreadedWebDownloader.vb
' Project:	    VBMultiThreadedWebDownloader
' Copyright (c) Microsoft Corporation.
' 
' This class is used to download files through internet using multiple threads. 
' It supplies public  methods to Start, Pause, Resume and Cancel a download. 
' 
' Before the download starts, the remote server should be checked 
' whether it supports "Accept-Ranges" header.
' 
' When the download starts, it will check whether the destination file exists. If
' not, create a file with the same size as the file to be downloaded, then
' creates multiple HttpDownloadClients to download the file in background threads.
' 
' It will fire a DownloadProgressChanged event when it has downloaded a
' specified size of data. It will also fire a DownloadCompleted event if the 
' download is completed or canceled.
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
Imports System.Linq
Imports System.Net

Namespace Web
    Public Class MultiThreadedWebDownloader
        ' Used while calculating download speed.
        Private Shared _locker As New Object()


        ''' <summary>
        ''' The Url of the file to be downloaded. 
        ''' </summary>
        Private _url As Uri
        Public Property Url() As Uri
            Get
                Return _url
            End Get
            Private Set(ByVal value As Uri)
                _url = value
            End Set
        End Property

        ''' <summary>
        ''' Specify whether the remote server supports "Accept-Ranges" header.
        ''' </summary>
        Private _isRangeSupported As Boolean
        Public Property IsRangeSupported() As Boolean
            Get
                Return _isRangeSupported
            End Get
            Private Set(ByVal value As Boolean)
                _isRangeSupported = value
            End Set
        End Property

        ''' <summary>
        ''' The total size of the file.
        ''' </summary>
        Private _totalSize As Long
        Public Property TotalSize() As Long
            Get
                Return _totalSize
            End Get
            Private Set(ByVal value As Long)
                _totalSize = value
            End Set
        End Property

        Private _downloadPath As String

        ''' <summary>
        ''' The local path to store the file.
        ''' If there is no file with the same name, a new file will be created.
        ''' </summary>
        Public Property DownloadPath() As String
            Get
                Return _downloadPath
            End Get
            Set(ByVal value As String)
                If Me.Status <> MultiThreadedWebDownloaderStatus.Checked Then
                    Throw New ApplicationException(
                        "The path could only be set when the status is Checked.")
                End If

                _downloadPath = value
            End Set
        End Property

        ''' <summary>
        ''' The Proxy of all the download client.
        ''' </summary>
        Public Property Proxy() As WebProxy

        ''' <summary>
        ''' The downloaded size of the file.
        ''' </summary>
        Private _downloadedSize As Long
        Public Property DownloadedSize() As Long
            Get
                Return _downloadedSize
            End Get
            Private Set(ByVal value As Long)
                _downloadedSize = value
            End Set
        End Property


        ' Store the used time spent in downloading data. The value does not include
        ' the paused time and it will only be updated when the download is paused, 
        ' canceled or completed.
        Private _usedTime As New TimeSpan()

        Private _lastStartTime As Date

        ''' <summary>
        ''' If the status is Downloading, then the total time is usedTime. Else the 
        ''' total should include the time used in current download thread.
        ''' </summary>
        Public ReadOnly Property TotalUsedTime() As TimeSpan
            Get
                If Me.Status <> MultiThreadedWebDownloaderStatus.Downloading Then
                    Return _usedTime
                Else
                    Return _usedTime.Add(Date.Now.Subtract(_lastStartTime))
                End If
            End Get
        End Property

        ' The time and size in last DownloadProgressChanged event. These two fields
        ' are used to calculate the download speed.
        Private _lastNotificationTime As Date

        Private _lastNotificationDownloadedSize As Long

        Private _bufferCount As Integer = 0

        ''' <summary>
        ''' If get a number of buffers, then fire DownloadProgressChanged event.
        ''' </summary>
        Private _bufferCountPerNotification As Integer
        Public Property BufferCountPerNotification() As Integer
            Get
                Return _bufferCountPerNotification
            End Get
            Private Set(ByVal value As Integer)
                _bufferCountPerNotification = value
            End Set
        End Property

        ''' <summary>
        ''' Set the BufferSize when read data in Response Stream.
        ''' </summary>
        Private _bufferSize As Integer
        Public Property BufferSize() As Integer
            Get
                Return _bufferSize
            End Get
            Private Set(ByVal value As Integer)
                _bufferSize = value
            End Set
        End Property

        ''' <summary>
        ''' The cache size in memory.
        ''' </summary>
        Private _maxCacheSize As Integer
        Public Property MaxCacheSize() As Integer
            Get
                Return _maxCacheSize
            End Get
            Private Set(ByVal value As Integer)
                _maxCacheSize = value
            End Set
        End Property

        Private _status As MultiThreadedWebDownloaderStatus

        ''' <summary>
        ''' If status changed, fire StatusChanged event.
        ''' </summary>
        Public Property Status() As MultiThreadedWebDownloaderStatus
            Get
                Return _status
            End Get

            Private Set(ByVal value As MultiThreadedWebDownloaderStatus)
                If _status <> value Then
                    _status = value
                    Me.OnStatusChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' The max threads count. The real threads count number is the min value of this
        ''' value and TotalSize / MaxCacheSize.
        ''' </summary>
        Private _maxThreadCount As Integer
        Public Property MaxThreadCount() As Integer
            Get
                Return _maxThreadCount
            End Get
            Private Set(ByVal value As Integer)
                _maxThreadCount = value
            End Set
        End Property

        ' The HttpDownloadClients to download the file. Each client uses one thread to
        ' download part of the file.
        Private _downloadClients As List(Of HttpDownloadClient) = Nothing

        Public ReadOnly Property DownloadThreadsCount() As Integer
            Get
                If _downloadClients IsNot Nothing Then
                    Return _downloadClients.Count
                Else
                    Return 0
                End If
            End Get
        End Property

        Public Event DownloadProgressChanged _
            As EventHandler(Of MultiThreadedWebDownloaderProgressChangedEventArgs)

        Public Event DownloadCompleted _
            As EventHandler(Of MultiThreadedWebDownloaderCompletedEventArgs)

        Public Event StatusChanged As EventHandler

        Public Event ErrorOccurred As EventHandler(Of ErrorEventArgs)


        ''' <summary>
        ''' Download the whole file. The default buffer size is 1KB, memory cache is
        ''' 1MB, buffer count per notification is 64, threads count is the double of 
        ''' logic processors count.
        ''' </summary>
        Public Sub New(ByVal url As String)
            Me.New(url, 1024, 1048576, 512, Environment.ProcessorCount * 2)
        End Sub

        Public Sub New(ByVal url As String,
                       ByVal bufferSize As Integer,
                       ByVal cacheSize As Integer,
                       ByVal bufferCountPerNotification As Integer,
                       ByVal maxThreadCount As Integer)

            If bufferSize < 0 Then
                Throw New ArgumentOutOfRangeException(
                    "BufferSize cannot be less than 0. ")
            End If

            If cacheSize < bufferSize Then
                Throw New ArgumentOutOfRangeException(
                    "MaxCacheSize cannot be less than BufferSize ")
            End If

            If bufferCountPerNotification <= 0 Then
                Throw New ArgumentOutOfRangeException(
                    "BufferCountPerNotification cannot be less than 0. ")
            End If

            If maxThreadCount < 1 Then
                Throw New ArgumentOutOfRangeException(
                    "maxThreadCount cannot be less than 1. ")
            End If

            Me.Url = New Uri(url)
            Me.BufferSize = bufferSize
            Me.MaxCacheSize = cacheSize
            Me.BufferCountPerNotification = bufferCountPerNotification

            Me.MaxThreadCount = maxThreadCount

            ' Set the maximum number of concurrent connections allowed by 
            ' a ServicePoint object
            ServicePointManager.DefaultConnectionLimit = maxThreadCount

            ' Initialize the HttpDownloadClient list.
            _downloadClients = New List(Of HttpDownloadClient)()

            ' Set the idle status.
            Me.Status = MultiThreadedWebDownloaderStatus.Idle
        End Sub

        ''' <summary>
        ''' Check total size and IsRangeSupported of the file in remote server. 
        ''' If there is no exception, then the file can be downloaded. 
        ''' </summary>
        Public Sub CheckFile()

            ' The file could be checked only in Idle status.
            If Me.Status <> MultiThreadedWebDownloaderStatus.Idle Then
                Throw New ApplicationException(
                    "The file could be checked only in Idle status.")
            End If

            ' Check the file information on the remote server.
            Dim request = CType(WebRequest.Create(Url), HttpWebRequest)

            ' Set the proxy.
            If Proxy IsNot Nothing Then
                request.Proxy = Proxy
            End If

            Using response = request.GetResponse()
                Me.IsRangeSupported = response.Headers.AllKeys.Contains("Accept-Ranges")
                Me.TotalSize = response.ContentLength

                If TotalSize <= 0 Then
                    Throw New ApplicationException("The file to download does not exist!")
                End If
            End Using

            ' Set the checked status.
            Me.Status = MultiThreadedWebDownloaderStatus.Checked
        End Sub


        ''' <summary>
        ''' Check whether the destination file exists. If  not, create a file with the same
        ''' size as the file to be downloaded.
        ''' </summary>
        Private Sub CheckFileOrCreateFile()
            ' Lock other threads or processes to prevent from creating the file.
            SyncLock _locker
                Dim fileToDownload As New FileInfo(DownloadPath)
                If fileToDownload.Exists Then

                    ' The destination file should have the same size as the file to be 
                    ' downloaded.
                    If fileToDownload.Length <> Me.TotalSize Then
                        Throw New ApplicationException(
                            "The download path already has a file which does not match" _
                            & " the file to download. ")
                    End If

                    ' Create a file.
                Else
                    Using fileStream_Renamed As FileStream = File.Create(Me.DownloadPath)
                        Dim createdSize As Long = 0
                        Dim buffer(4095) As Byte
                        Do While createdSize < TotalSize
                            Dim bufferSize As Integer =
                                If((TotalSize - createdSize) < 4096,
                                   CInt(Fix(TotalSize - createdSize)),
                                   4096)
                            fileStream_Renamed.Write(buffer, 0, bufferSize)
                            createdSize += bufferSize
                        Loop
                    End Using
                End If
            End SyncLock
        End Sub

        ''' <summary>
        ''' Start to download.
        ''' </summary>
        Public Sub Start()
            ' Check whether the destination file exist.
            CheckFileOrCreateFile()

            ' Only Checked downloader can be started.
            If Me.Status <> MultiThreadedWebDownloaderStatus.Checked Then
                Throw New ApplicationException("Only Checked downloader can be started. ")
            End If

            ' If the file does not support "Accept-Ranges" header, then create one 
            ' HttpDownloadClient to download the file in a single thread, else create
            ' multiple HttpDownloadClients to download the file.
            If Not IsRangeSupported Then
                Dim client As New HttpDownloadClient(Me.Url,
                                                     Me.DownloadPath,
                                                     0,
                                                     Long.MaxValue,
                                                     Me.BufferSize,
                                                     Me.BufferCountPerNotification * Me.BufferSize)
                client.TotalSize = Me.TotalSize
                Me._downloadClients.Add(client)
            Else
                ' Calculate the block size for each client to download.
                Dim maxSizePerThread As Integer =
                    CInt(Fix(System.Math.Ceiling(CDbl(Me.TotalSize) / Me.MaxThreadCount)))
                If maxSizePerThread < Me.MaxCacheSize Then
                    maxSizePerThread = Me.MaxCacheSize
                End If

                Dim leftSizeToDownload As Long = Me.TotalSize

                ' The real threads count number is the min value of MaxThreadCount and 
                ' TotalSize / MaxCacheSize.              
                Dim threadsCount As Integer =
                    CInt(Fix(System.Math.Ceiling(CDbl(Me.TotalSize) / maxSizePerThread)))

                For i As Integer = 0 To threadsCount - 1
                    Dim endPoint As Long = maxSizePerThread * (i + 1) - 1
                    Dim sizeToDownload As Long = maxSizePerThread

                    If endPoint > Me.TotalSize Then
                        endPoint = Me.TotalSize - 1
                        sizeToDownload = endPoint - maxSizePerThread * i
                    End If

                    ' Download a block of the whole file.
                    Dim client As New HttpDownloadClient(Me.Url,
                                                         Me.DownloadPath,
                                                         maxSizePerThread * i,
                                                         endPoint)

                    client.TotalSize = sizeToDownload
                    Me._downloadClients.Add(client)
                Next i
            End If

            ' Set the lastStartTime to calculate the used time.
            _lastStartTime = Date.Now

            ' Set the downloading status.
            Me.Status = MultiThreadedWebDownloaderStatus.Downloading

            ' Start all HttpDownloadClients.
            For Each client In Me._downloadClients
                If Me.Proxy IsNot Nothing Then
                    client.Proxy = Me.Proxy
                End If

                ' Register the events of HttpDownloadClients.
                AddHandler client.DownloadProgressChanged,
                    AddressOf client_DownloadProgressChanged

                AddHandler client.StatusChanged, AddressOf client_StatusChanged

                AddHandler client.ErrorOccurred, AddressOf client_ErrorOccurred
                client.Start()
            Next client


        End Sub

        ''' <summary>
        ''' Pause the download.
        ''' </summary>
        Public Sub Pause()
            ' Only downloading downloader can be paused.
            If Me.Status <> MultiThreadedWebDownloaderStatus.Downloading Then
                Throw New ApplicationException(
                    "Only downloading downloader can be paused.")
            End If

            Me.Status = MultiThreadedWebDownloaderStatus.Pausing

            ' Pause all the HttpDownloadClients. If all of the clients are paused,
            ' the status of the downloader will be changed to Paused.
            For Each client In Me._downloadClients
                If client.Status <> HttpDownloadClientStatus.Completed Then
                    client.Pause()
                End If
            Next client
        End Sub

        ''' <summary>
        ''' Resume the download.
        ''' </summary>
        Public Sub [Resume]()
            ' Only paused downloader can be paused.
            If Me.Status <> MultiThreadedWebDownloaderStatus.Paused Then
                Throw New ApplicationException(
                    "Only paused downloader can be resumed. ")
            End If

            ' Set the lastStartTime to calculate the used time.
            _lastStartTime = Date.Now

            ' Set the downloading status.
            Me.Status = MultiThreadedWebDownloaderStatus.Downloading

            ' Resume all HttpDownloadClients.
            For Each client In Me._downloadClients
                If client.Status <> HttpDownloadClientStatus.Completed Then
                    client.Resume()
                End If
            Next client

        End Sub

        ''' <summary>
        ''' Cancel the download
        ''' </summary>
        Public Sub Cancel()
            ' Only downloading downloader can be canceled.
            If Me.Status <> MultiThreadedWebDownloaderStatus.Downloading Then
                Throw New ApplicationException(
                    "Only downloading downloader can be canceled.")
            End If

            Me.Status = MultiThreadedWebDownloaderStatus.Canceling

            Me.OnError(New ErrorEventArgs With
                       {
                           .Exception = New Exception("Download is canceled.")
                       })

            ' Cancel all HttpDownloadClients.
            For Each client In Me._downloadClients
                If client.Status <> HttpDownloadClientStatus.Completed Then
                    client.Cancel()
                End If
            Next client

        End Sub

        ''' <summary>
        ''' Handle the StatusChanged event of all the HttpDownloadClients.
        ''' </summary>
        Private Sub client_StatusChanged(ByVal sender As Object, ByVal e As EventArgs)

            ' If all the clients are completed, then the status of this downloader is 
            ' completed.
            If Me._downloadClients.All(Function(client) client.Status =
                                           HttpDownloadClientStatus.Completed) Then
                Me.Status = MultiThreadedWebDownloaderStatus.Completed
            Else

                ' The completed clients will not be taken into consideration.
                Dim nonCompletedClients =
                    Me._downloadClients.Where(
                        Function(client) client.Status <> HttpDownloadClientStatus.Completed)

                ' If all the nonCompletedClients are Paused, then the status of this 
                ' downloader is Paused.
                If nonCompletedClients.All(
                    Function(client) client.Status = HttpDownloadClientStatus.Paused) Then
                    Me.Status = MultiThreadedWebDownloaderStatus.Paused
                End If

                ' If all the nonCompletedClients are Canceled, then the status of this 
                ' downloader is Canceled.
                If nonCompletedClients.All(
                    Function(client) client.Status = HttpDownloadClientStatus.Canceled) Then
                    Me.Status = MultiThreadedWebDownloaderStatus.Canceled
                End If
            End If

        End Sub

        ''' <summary>
        ''' Handle the DownloadProgressChanged event of all the HttpDownloadClients, and 
        ''' calculate the download speed.
        ''' </summary>
        Private Sub client_DownloadProgressChanged(ByVal sender As Object,
                                                   ByVal e As HttpDownloadClientProgressChangedEventArgs)
            SyncLock _locker
                DownloadedSize += e.Size
                _bufferCount += 1

                If _bufferCount = BufferCountPerNotification Then

                    Dim speed As Integer = 0
                    Dim current As Date = Date.Now
                    Dim interval As TimeSpan = current.Subtract(_lastNotificationTime)

                    If interval.TotalSeconds < 60 Then
                        speed = CInt(Fix(System.Math.Floor((Me.DownloadedSize - Me._lastNotificationDownloadedSize) / interval.TotalSeconds)))
                    End If

                    _lastNotificationTime = current
                    _lastNotificationDownloadedSize = Me.DownloadedSize

                    Dim downloadProgressChangedEventArgs =
                        New MultiThreadedWebDownloaderProgressChangedEventArgs(
                        DownloadedSize, TotalSize, speed)
                    Me.OnDownloadProgressChanged(downloadProgressChangedEventArgs)



                    ' Reset the bufferCount.
                    _bufferCount = 0
                End If

            End SyncLock
        End Sub

        ''' <summary>
        ''' Handle the ErrorOccurred event of all the HttpDownloadClients.
        ''' </summary>
        Private Sub client_ErrorOccurred(ByVal sender As Object, ByVal e As ErrorEventArgs)
            If Me.Status <> MultiThreadedWebDownloaderStatus.Canceling _
                AndAlso Me.Status <> MultiThreadedWebDownloaderStatus.Canceled Then
                Me.Cancel()

                ' Raise ErrorOccurred event.
                Me.OnError(e)

            End If

        End Sub

        ''' <summary>
        ''' Raise DownloadProgressChanged event. If the status is Completed, then raise
        ''' DownloadCompleted event.
        ''' </summary>
        ''' <param name="e"></param>
        Protected Overridable Sub OnDownloadProgressChanged(ByVal e As MultiThreadedWebDownloaderProgressChangedEventArgs)
            RaiseEvent DownloadProgressChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raise StatusChanged event.
        ''' </summary>
        Protected Overridable Sub OnStatusChanged(ByVal e As EventArgs)
            If Me.Status = MultiThreadedWebDownloaderStatus.Paused _
                OrElse Me.Status = MultiThreadedWebDownloaderStatus.Canceled _
                OrElse Me.Status = MultiThreadedWebDownloaderStatus.Completed Then
                ' Update the used time when the current download is stopped.
                _usedTime = _usedTime.Add(Date.Now.Subtract(_lastStartTime))
            End If

            RaiseEvent StatusChanged(Me, e)

            If Me.Status = MultiThreadedWebDownloaderStatus.Completed Then
                Dim downloadCompletedEventArgs As _
                    New MultiThreadedWebDownloaderCompletedEventArgs(
                        Me.DownloadedSize, Me.TotalSize, Me.TotalUsedTime)
                Me.OnDownloadCompleted(downloadCompletedEventArgs)
            End If

        End Sub

        ''' <summary>
        ''' Raise DownloadCompleted event.
        ''' </summary>
        Protected Overridable Sub OnDownloadCompleted(ByVal e As MultiThreadedWebDownloaderCompletedEventArgs)
            RaiseEvent DownloadCompleted(Me, e)
        End Sub

        ''' <summary>
        ''' Raise ErrorOccurred event.
        ''' </summary>
        Protected Overridable Sub OnError(ByVal e As ErrorEventArgs)
            RaiseEvent ErrorOccurred(Me, e)
        End Sub
    End Class
End Namespace

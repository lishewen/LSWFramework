'****************************** Module Header ******************************'
' Module Name:  HttpDownloadCompletedEventArgs.vb
' Project:	    VBMultiThreadedWebDownloader
' Copyright (c) Microsoft Corporation.
' 
' The class MultiThreadedWebDownloaderCompletedEventArgs defines the arguments
' used by the DownloadCompleted event of MultiThreadedWebDownloader.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Namespace Web
    Public Class MultiThreadedWebDownloaderCompletedEventArgs
        Inherits EventArgs

        Private privateDownloadedSize As Long
        Public Property DownloadedSize() As Long
            Get
                Return privateDownloadedSize
            End Get
            Private Set(ByVal value As Long)
                privateDownloadedSize = value
            End Set
        End Property

        Private privateTotalSize As Long
        Public Property TotalSize() As Long
            Get
                Return privateTotalSize
            End Get
            Private Set(ByVal value As Long)
                privateTotalSize = value
            End Set
        End Property

        Private privateTotalTime As TimeSpan
        Public Property TotalTime() As TimeSpan
            Get
                Return privateTotalTime
            End Get
            Private Set(ByVal value As TimeSpan)
                privateTotalTime = value
            End Set
        End Property

        Public Sub New(ByVal downloadedSize As Long, ByVal totalSize As Long,
                       ByVal totalTime As TimeSpan)
            Me.DownloadedSize = downloadedSize
            Me.TotalSize = totalSize
            Me.TotalTime = totalTime
        End Sub
    End Class
End Namespace

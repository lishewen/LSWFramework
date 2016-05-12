'****************************** Module Header ******************************'
' Module Name:  MultiThreadedWebDownloaderProgressChangedEventArgs.vb
' Project:	    VBMultiThreadedWebDownloader
' Copyright (c) Microsoft Corporation.
' 
' The class MultiThreadedWebDownloaderProgressChangedEventArgs defines the 
' arguments used by the DownloadProgressChanged event of MultiThreadedWebDownloader.
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
    Public Class MultiThreadedWebDownloaderProgressChangedEventArgs
        Inherits EventArgs

        Private privateReceivedSize As Long
        Public Property ReceivedSize() As Long
            Get
                Return privateReceivedSize
            End Get
            Private Set(ByVal value As Long)
                privateReceivedSize = value
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

        Private privateDownloadSpeed As Integer
        Public Property DownloadSpeed() As Integer
            Get
                Return privateDownloadSpeed
            End Get
            Private Set(ByVal value As Integer)
                privateDownloadSpeed = value
            End Set
        End Property

        Public Sub New(ByVal receivedSize As Long, ByVal totalSize As Long,
                       ByVal downloadSpeed As Integer)
            Me.ReceivedSize = receivedSize
            Me.TotalSize = totalSize
            Me.DownloadSpeed = downloadSpeed
        End Sub
    End Class
End Namespace

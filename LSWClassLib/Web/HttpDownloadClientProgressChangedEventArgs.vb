'****************************** Module Header ******************************'
' Module Name:  HttpDownloadClientProgressChangedEventArgs.vb
' Project:	    VBMultiThreadedWebDownloader
' Copyright (c) Microsoft Corporation.
' 
' The class HttpDownloadClientProgressChangedEventArgs defines the 
' arguments used by the DownloadProgressChanged event of HttpDownloadClient.
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
    Public Class HttpDownloadClientProgressChangedEventArgs
        Inherits EventArgs
        Public Property Size() As Integer
    End Class
End Namespace

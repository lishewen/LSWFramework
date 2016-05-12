Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms

Namespace Image
    Public Class WebSiteThumb
        Public Sub New(Url As String, BW As Integer, BH As Integer, TW As Integer, TH As Integer)
            _Url = Url
            _BrowserWidth = BW
            _BrowserHeight = BH
            _ThumbnailWidth = TW
            _ThumbnailHeight = TH
            Percent = 0
            IsFull = False
        End Sub

        Public Sub New(Url As String, Optional percent As Integer = 100)
            _Url = Url
            _BrowserWidth = 0
            _BrowserHeight = 0
            _ThumbnailWidth = 0
            _ThumbnailHeight = 0
            Me.Percent = percent
            IsFull = True
        End Sub

        Private _Bitmap As Bitmap = Nothing
        Private _Url As String = Nothing
        Private _ThumbnailWidth As Integer
        Private _ThumbnailHeight As Integer
        Private _BrowserWidth As Integer
        Private _BrowserHeight As Integer

        Public Property IsFull As Boolean
        Public Property Percent As Integer

        Public Property Url() As String
            Get
                Return _Url
            End Get
            Set(value As String)
                _Url = value
            End Set
        End Property

        Public ReadOnly Property ThumbnailImage() As Bitmap
            Get
                Return _Bitmap
            End Get
        End Property

        Public Property ThumbnailWidth() As Integer
            Get
                Return _ThumbnailWidth
            End Get
            Set(value As Integer)
                _ThumbnailWidth = value
            End Set
        End Property

        Public Property ThumbnailHeight() As Integer
            Get
                Return _ThumbnailHeight
            End Get
            Set(value As Integer)
                _ThumbnailHeight = value
            End Set
        End Property

        Public Property BrowserWidth() As Integer
            Get
                Return _BrowserWidth
            End Get
            Set(value As Integer)
                _BrowserWidth = value
            End Set
        End Property

        Public Property BrowserHeight() As Integer
            Get
                Return _BrowserHeight
            End Get
            Set(value As Integer)
                _BrowserHeight = value
            End Set
        End Property

        Public Function GetWebSiteThumb() As Bitmap
            Dim _threadStart As New ThreadStart(AddressOf _GenerateWebSiteThumb)
            Dim _thread As New Thread(_threadStart)

            _thread.SetApartmentState(ApartmentState.STA)
            _thread.Start()
            _thread.Join()
            Return _Bitmap
        End Function

        Private Sub _GenerateWebSiteThumb()
            Dim _WebBrowser As New WebBrowser()
            _WebBrowser.ScrollBarsEnabled = False
            _WebBrowser.Navigate(_Url)
            AddHandler _WebBrowser.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf WebBrowser_DocumentCompleted)
            While _WebBrowser.ReadyState <> WebBrowserReadyState.Complete
                Application.DoEvents()
            End While
            _WebBrowser.Dispose()
        End Sub

        Private Sub WebBrowser_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs)
            Dim _WebBrowser As WebBrowser = DirectCast(sender, WebBrowser)
            If IsFull Then
                _WebBrowser.ClientSize = New Size(_WebBrowser.Document.Body.ScrollRectangle.Width, _WebBrowser.Document.Body.ScrollRectangle.Height)
            Else
                _WebBrowser.ClientSize = New Size(Me._BrowserWidth, Me._BrowserHeight)
            End If
            _WebBrowser.ScrollBarsEnabled = False
            _Bitmap = New Bitmap(_WebBrowser.Bounds.Width, _WebBrowser.Bounds.Height)
            _WebBrowser.BringToFront()
            _WebBrowser.DrawToBitmap(_Bitmap, _WebBrowser.Bounds)

            If _ThumbnailHeight > 0 AndAlso _ThumbnailWidth > 0 Then
                _Bitmap = DirectCast(_Bitmap.GetThumbnailImage(_ThumbnailWidth, _ThumbnailHeight, Nothing, IntPtr.Zero), Bitmap)
            End If

            If Percent <> 100 AndAlso Percent > 0 Then
                _Bitmap = DirectCast(_Bitmap.GetThumbnailImage(_Bitmap.Width * Percent / 100, _Bitmap.Height * Percent / 100, Nothing, IntPtr.Zero), Bitmap)
            End If
        End Sub
    End Class
End Namespace
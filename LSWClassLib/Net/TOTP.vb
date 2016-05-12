Imports LSW.Security
Imports LSW.Extension
Imports System.Security.Cryptography
Imports System.Timers

Namespace Net
    Public Class TOTP
        Dim WithEvents t As Timer
        Public Event CompleteCalculate(sender As TOTP, password As Integer)
        Private _secondsToGo As Integer
        Public Property SecondsToGo As Integer
            Get
                Return _secondsToGo
            End Get
            Private Set(value As Integer)
                _secondsToGo = value
                If SecondsToGo = 30 Then CalculateOneTimePassword()
            End Set
        End Property
        Private _identity As String
        Public Property Identity As String
            Get
                Return _identity
            End Get
            Set(value As String)
                _identity = value
                CalculateOneTimePassword()
            End Set
        End Property
        Private _secret() As Byte
        Public Property Secret As Byte()
            Get
                Return _secret
            End Get
            Set(value As Byte())
                _secret = value
                CalculateOneTimePassword()
            End Set
        End Property
        Public ReadOnly Property QRCodeUrl As String
            Get
                Return GetQRCodeUrl()
            End Get
        End Property
        Public Property Timestamp As Long
        Public Property Hmac As Byte()
        Public ReadOnly Property HmacPart1 As Byte()
            Get
                Return Hmac.Take(Offset).ToArray
            End Get
        End Property
        Public ReadOnly Property HmacPart2 As Byte()
            Get
                Return Hmac.Skip(Offset).Take(4).ToArray
            End Get
        End Property
        Public ReadOnly Property HmacPart3 As Byte()
            Get
                Return Hmac.Skip(Offset + 4).ToArray
            End Get
        End Property
        Public Property Offset As Integer
        Public Property OneTimePassword As Integer
        Private Sub CalculateOneTimePassword()
            ' https://tools.ietf.org/html/rfc4226
            Timestamp = Convert.ToInt64(Date.UtcNow.ToUtcTimeStamp / 30)
            Dim data = BitConverter.GetBytes(Timestamp).Reverse().ToArray()
            Hmac = New HMACSHA1(Secret).ComputeHash(data)
            Offset = Hmac.Last() And &HF
            OneTimePassword = (((Hmac(Offset + 0) And &H7F) << 24) Or ((Hmac(Offset + 1) And &HFF) << 16) Or ((Hmac(Offset + 2) And &HFF) << 8) Or (Hmac(Offset + 3) And &HFF)) Mod 1000000
            RaiseEvent CompleteCalculate(Me, OneTimePassword)
        End Sub
        Private Function GetQRCodeUrl() As String
            ' https://code.google.com/p/google-authenticator/wiki/KeyUriFormat
            Dim base32Secret = Base32.Encode(Secret)
            Return String.Format("https://www.google.com/chart?chs=200x200&chld=M|0&cht=qr&chl=otpauth://totp/{0}%3Fsecret%3D{1}", Identity, base32Secret)
        End Function

        Private Sub t_Elapsed(sender As Object, e As ElapsedEventArgs) Handles t.Elapsed
            SecondsToGo = 30 - Convert.ToInt32(Date.UtcNow.ToUtcTimeStamp Mod 30)
        End Sub

        Public Sub Start()
            t.Start()
        End Sub

        Public Sub [Stop]()
            t.Stop()
        End Sub
    End Class
End Namespace
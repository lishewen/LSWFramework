Namespace Net
    Public Class TrustAllCertificatePolicy
        Implements System.Net.ICertificatePolicy

        Public Function CheckValidationResult(srvPoint As System.Net.ServicePoint, certificate As System.Security.Cryptography.X509Certificates.X509Certificate, request As System.Net.WebRequest, certificateProblem As Integer) As Boolean Implements System.Net.ICertificatePolicy.CheckValidationResult
            Return True
        End Function
    End Class
End Namespace
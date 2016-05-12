Imports System.Security.Cryptography
Imports System.Text
Imports System.Xml
Imports System.IO

Namespace Security
    Public MustInherit Class ElGamal
        Inherits AsymmetricAlgorithm

        Public MustOverride Sub ImportParameters(ByVal p_parameters As ElGamalParameters)
        Public MustOverride Function ExportParameters(ByVal p_include_private_params As Boolean) As ElGamalParameters
        Public MustOverride Function EncryptData(ByVal p_data() As Byte) As Byte()
        Public MustOverride Function DecryptData(ByVal p_data() As Byte) As Byte()
        Public MustOverride Function Sign(ByVal p_hashcode() As Byte) As Byte()
        Public MustOverride Function VerifySignature(ByVal p_hashcode() As Byte, ByVal p_signature() As Byte) As Boolean

        Public Overrides Function ToXmlString(ByVal includePrivateParameters As Boolean) As String
            Dim x_params = ExportParameters(includePrivateParameters)
            Dim x_sb As New StringBuilder
            x_sb.Append("<ElGamalKeyValue>")
            x_sb.Append("<P>" & Convert.ToBase64String(x_params.P) & "</P>")
            x_sb.Append("<G>" & Convert.ToBase64String(x_params.G) & "</G>")
            x_sb.Append("<Y>" & Convert.ToBase64String(x_params.Y) & "</Y>")
            x_sb.Append("</ElGamalKeyValue>")
            Return x_sb.ToString
        End Function

        Public Overrides Sub FromXmlString(ByVal xmlString As String)
            Dim x_params As New ElGamalParameters
            Dim x_reader As New XmlTextReader(New StringReader(xmlString))
            While x_reader.Read
                If True OrElse x_reader.IsStartElement Then
                    Select Case x_reader.Name
                        Case "P"
                            x_params.P = Convert.FromBase64String(x_reader.ReadString)
                        Case "G"
                            x_params.G = Convert.FromBase64String(x_reader.ReadString)
                        Case "Y"
                            x_params.Y = Convert.FromBase64String(x_reader.ReadString)
                        Case "X"
                            x_params.X = Convert.FromBase64String(x_reader.ReadString)
                    End Select
                End If
            End While
            ImportParameters(x_params)
        End Sub
    End Class
End Namespace
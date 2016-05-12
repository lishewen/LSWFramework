Imports System.Security.Cryptography

Namespace Security
    Public MustInherit Class XTEAManagedAbstractTransform
        Implements ICryptoTransform

        Protected o_key() As UInteger

        Public Sub New(ByVal p_key() As Byte)
            o_key = ConvertBytesToUints(p_key, 0, p_key.Length)
        End Sub

        Protected Function ConvertBytesToUints(ByVal p_data() As Byte, ByVal p_offset As Integer, ByVal p_count As Integer) As UInteger()
            Dim x_result(p_count / 4) As UInteger
            Dim i = p_offset, j = 0
            While i < p_offset + p_count
                x_result(j) = BitConverter.ToUInt32(p_data, i)
                i += 4
                j += 1
            End While
            Return x_result
        End Function

        Protected Function ConvertUintsToBytes(ByVal p_data() As UInteger) As Byte()
            Dim x_result(p_data.Length * 4) As Byte
            Dim i = 0, j = 0
            While i < p_data.Length
                Dim x_interim = BitConverter.GetBytes(p_data(i))
                Array.Copy(x_interim, 0, x_result, j, x_interim.Length)
                i += 1
                j += 4
            End While
            Return x_result
        End Function

        Public ReadOnly Property CanReuseTransform() As Boolean Implements System.Security.Cryptography.ICryptoTransform.CanReuseTransform
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property CanTransformMultipleBlocks() As Boolean Implements System.Security.Cryptography.ICryptoTransform.CanTransformMultipleBlocks
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property InputBlockSize() As Integer Implements System.Security.Cryptography.ICryptoTransform.InputBlockSize
            Get
                Return 8
            End Get
        End Property

        Public ReadOnly Property OutputBlockSize() As Integer Implements System.Security.Cryptography.ICryptoTransform.OutputBlockSize
            Get
                Return 8
            End Get
        End Property

        Public MustOverride Function TransformBlock(ByVal inputBuffer() As Byte, ByVal inputOffset As Integer, ByVal inputCount As Integer, ByVal outputBuffer() As Byte, ByVal outputOffset As Integer) As Integer Implements System.Security.Cryptography.ICryptoTransform.TransformBlock

        Public MustOverride Function TransformFinalBlock(ByVal inputBuffer() As Byte, ByVal inputOffset As Integer, ByVal inputCount As Integer) As Byte() Implements System.Security.Cryptography.ICryptoTransform.TransformFinalBlock

        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: free other state (managed objects).
                End If

                ' TODO: free your own state (unmanaged objects).
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

#Region " IDisposable Support "
        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
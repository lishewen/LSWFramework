Namespace Image
    Class EXIFextractorEnumerator
        Implements IEnumerator

        Private exifTable As Hashtable
        Private index As IDictionaryEnumerator

        Friend Sub New(ByVal exif As Hashtable)
            Me.exifTable = exif
            Me.Reset()
            index = exif.GetEnumerator()
        End Sub

#Region "IEnumerator Members"

        Public Sub Reset() Implements System.Collections.IEnumerator.Reset
            Me.index = Nothing
        End Sub

        Public ReadOnly Property Current() As Object Implements System.Collections.IEnumerator.Current
            Get
                Return (New KeyValuePair(Of Object, Object)(Me.index.Key, Me.index.Value))
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
            If index IsNot Nothing AndAlso index.MoveNext() Then
                Return True
            Else
                Return False
            End If
        End Function

#End Region

    End Class
End Namespace
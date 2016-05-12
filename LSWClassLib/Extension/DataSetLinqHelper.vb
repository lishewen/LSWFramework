Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports LSW.Exceptions

Namespace Extension
    Public Module DataSetLinqHelper
        <Extension()> _
        Public Function CopyToDataTable(Of T)(source As IEnumerable(Of T)) As DataTable
            Return New ObjectShredder(Of T)().Shred(source, Nothing, Nothing).Copy
        End Function

        <Extension()> _
        Public Function CopyToDataTable(Of T)(source As IEnumerable(Of T), table As DataTable, options As System.Nullable(Of LoadOption)) As DataTable
            Return New ObjectShredder(Of T)().Shred(source, table, options).Copy
        End Function

        <Extension()>
        Public Function ToEntity(Of T As New)(row As DataRow) As T
            Dim item As New T
            If row Is Nothing Then
                Return item
            End If

            Dim pi = item.GetType.GetProperties
            For Each p In pi
                If Not CanSetPropertyValue(p, row) Then
                    Continue For
                End If

                Try
                    If row(p.Name) Is DBNull.Value Then
                        p.SetValue(item, Nothing, Nothing)
                        Continue For
                    End If

                    p.SetValue(item, row(p.Name), Nothing)
                Catch ex As Exception
                    Dim e As New LSWFrameworkException(ex)
                End Try
            Next

            Return item
        End Function

        Private Function CanSetPropertyValue(propertyInfo As PropertyInfo, adaptedRow As DataRow) As Boolean
            If Not propertyInfo.CanWrite Then
                Return False
            End If

            If Not adaptedRow.Table.Columns.Contains(propertyInfo.Name) Then
                Return False
            End If

            Return True
        End Function
    End Module
End Namespace
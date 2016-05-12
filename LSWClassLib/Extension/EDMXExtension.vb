Imports System.Runtime.CompilerServices
Imports System.Data.Objects.DataClasses

Namespace Extension
    Public Module EDMXExtension
        <Extension()> _
            Public Function LazyLoad(Of T As {Class, IEntityWithRelationships})(ByVal collection As EntityCollection(Of T)) As EntityCollection(Of T)
            If Not collection.IsLoaded Then
                collection.Load()
            End If
            Return collection
        End Function

        <Extension()> _
            Public Sub LazyLoad(Of T As {Class, IEntityWithRelationships})(ByVal reference As EntityReference(Of T))
            If Not reference.IsLoaded Then
                reference.Load()
            End If
        End Sub
    End Module
End Namespace
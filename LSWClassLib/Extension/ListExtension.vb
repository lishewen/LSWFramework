Imports System.Runtime.CompilerServices
Imports LSW.Common
Imports System.Threading.Tasks

Namespace Extension
    Public Module ListExtension
        <Extension()>
        Public Function ListMove(Of T)(dataList As IList(Of T), oid As Integer, nid As Integer) As Boolean
            If oid >= dataList.Count OrElse nid >= dataList.Count OrElse oid < 0 OrElse nid < 0 OrElse oid = nid Then Return False
            Try
                Dim sel = dataList(oid)
                dataList.RemoveAt(oid)
                dataList.Insert(nid, sel)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        <Extension()>
        Public Function ListSwap(Of T)(dataList As IList(Of T), oid As Integer, nid As Integer) As Boolean
            If oid >= dataList.Count OrElse nid >= dataList.Count OrElse oid < 0 OrElse nid < 0 OrElse oid = nid Then Return False
            Try
                Dim sel = dataList(oid)
                dataList(oid) = dataList(nid)
                dataList(nid) = sel
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        <Extension()>
        Public Function ListUp(Of T)(dataList As IList(Of T), id As Integer, Optional count As Integer = 1) As Boolean
            Return dataList.ListMove(id, id - count)
        End Function

        <Extension()>
        Public Function ListDown(Of T)(dataList As IList(Of T), id As Integer, Optional count As Integer = 1) As Boolean
            Return dataList.ListMove(id, id + count)
        End Function

        <Extension()>
        Public Function ListTop(Of T)(dataList As IList(Of T), id As Integer) As Boolean
            Return dataList.ListMove(id, 0)
        End Function

        <Extension()>
        Public Function ListBottom(Of T)(dataList As IList(Of T), id As Integer) As Boolean
            Return dataList.ListMove(id, dataList.Count - 1)
        End Function

        <Extension>
        Public Sub RemoveMultiple(Of T)(dataList As IList(Of T), itemsToRemove As IEnumerable(Of T))
            For Each item In itemsToRemove
                dataList.Remove(item)
            Next
        End Sub

        ''' <summary>
        ''' 对比两个同类型的List 返回差异List集合
        ''' </summary>
        ''' <typeparam name="T">泛型类型 需实现IObjectState</typeparam>
        ''' <param name="newlist">修改后的数据集合</param>
        ''' <param name="oldlist">原始数据集合</param>
        ''' <returns>返回与原始集合有差异的集合</returns>
        ''' <remarks></remarks>
        <Extension>
        Public Function GetDifferenceList(Of T As IObjectState)(newlist As List(Of T), oldlist As List(Of T)) As List(Of T)
            Dim result As New List(Of T)

            Parallel.ForEach(newlist, Sub(newmodel As T)
                                          Dim oldmodel = oldlist.FirstOrDefault(Function(old) old.ID = newmodel.ID)
                                          If oldmodel Is Nothing Then
                                              newmodel.ObjectState = ObjectState.Added
                                              result.Add(newmodel)
                                          Else
                                              Dim pi = oldmodel.GetType.GetProperties
                                              For Each p In pi
                                                  Dim o_new = p.GetValue(newmodel, Nothing)
                                                  Dim o_old = p.GetValue(oldmodel, Nothing)

                                                  If Object.Equals(o_new, o_old) Then
                                                      Continue For
                                                  Else
                                                      newmodel.ObjectState = ObjectState.Edited
                                                      result.Add(newmodel)
                                                      Exit For
                                                  End If
                                              Next
                                          End If
                                      End Sub)

            Parallel.ForEach(oldlist, Sub(oldmodel As T)
                                          Dim newmodel = newlist.FirstOrDefault(Function(n) n.ID = oldmodel.ID)
                                          If newmodel Is Nothing Then
                                              oldmodel.ObjectState = ObjectState.Deleted
                                              result.Add(oldmodel)
                                          End If
                                      End Sub)

            Return result
        End Function
    End Module
End Namespace
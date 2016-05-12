Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Collections
Imports LSW.Exceptions

Namespace Data
    Public Module SqlHelper
        ''' <summary>
        ''' 哈希表：缓存参数
        ''' </summary>
        ''' <remarks></remarks>
        Public parmCache As Hashtable = Hashtable.Synchronized(New Hashtable())
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="connectionString"></param>
        ''' <param name="cmdType"></param>
        ''' <param name="cmdText"></param>
        ''' <param name="commandParameters">ParamArray 表示函数参数个数不确定C#中为params</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteNonquery(ByVal connectionString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter()) As Int32
            Dim cmd As SqlCommand = New SqlCommand
            Using conn As SqlConnection = New SqlConnection(connectionString)
                PrepareCommand(cmd, conn, Nothing, cmdType, cmdText, commandParameters)
                Dim val As Int32 = cmd.ExecuteNonQuery()
                cmd.Parameters.Clear()
                Return val
            End Using
        End Function

        Public Sub ExecuteNonquery(ByVal connectionString As String, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter())
            ExecuteNonquery(connectionString, CommandType.Text, cmdText, commandParameters)
        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="cmdType"></param>
        ''' <param name="cmdText"></param>
        ''' <param name="commandParameters">ParamArray 表示函数参数个数不确定C#中为params</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteNonQuery(ByVal connection As SqlConnection, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter()) As Int32
            Dim cmd As SqlCommand = New SqlCommand
            PrepareCommand(cmd, connection, Nothing, cmdType, cmdText, commandParameters)
            Dim val As Int32 = cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            Return val
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="trans"></param>
        ''' <param name="cmdType"></param>
        ''' <param name="cmdText"></param>
        ''' <param name="commandParameters">ParamArray 表示函数参数个数不确定C#中为params</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteNonQuery(ByVal trans As SqlTransaction, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter()) As Int32
            Dim cmd As SqlCommand = New SqlCommand
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters)
            Dim val As Int32 = cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
            Return val
        End Function
        ''' <summary>
        ''' 返回数据集，可以包含多个表，这是我自己加上的，用以返回数据集，表
        ''' </summary>
        ''' <param name="connectionString"></param>
        ''' <param name="cmdType"></param>
        ''' <param name="cmdText"></param>
        ''' <param name="commandParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteDataSet(ByVal connectionString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter()) As DataSet
            Dim cmd As SqlCommand = New SqlCommand
            Using conn As SqlConnection = New SqlConnection(connectionString)
                PrepareCommand(cmd, conn, Nothing, cmdType, cmdText, commandParameters)
                Dim adp As SqlDataAdapter = New SqlDataAdapter(cmd)
                Dim ds As DataSet = New DataSet
                Try
                    adp.Fill(ds)
                    cmd.Parameters.Clear()
                Finally
                    adp.Dispose()
                End Try
                Return ds
            End Using
        End Function
        Public Function ExecuteDataTable(ByVal connectionString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter()) As DataTable
            Dim cmd As SqlCommand = New SqlCommand
            Using conn As SqlConnection = New SqlConnection(connectionString)
                PrepareCommand(cmd, conn, Nothing, cmdType, cmdText, commandParameters)
                Dim adp As SqlDataAdapter = New SqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                Try
                    adp.Fill(dt)
                    cmd.Parameters.Clear()
                Finally
                    adp.Dispose()
                End Try
                Return dt
            End Using
        End Function
        Public Function ExecuteDataTable(ByVal connectionString As String, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter()) As DataTable
            Return ExecuteDataTable(connectionString, CommandType.Text, cmdText, commandParameters)
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="connectionString"></param>
        ''' <param name="cmdType"></param>
        ''' <param name="cmdText"></param>
        ''' <param name="commandParameters">ParamArray 表示函数参数个数不确定C#中为params</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteReader(ByVal connectionString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter()) As SqlDataReader
            Dim cmd As SqlCommand = New SqlCommand
            Dim conn As SqlConnection = New SqlConnection(connectionString)
            Try
                PrepareCommand(cmd, conn, Nothing, cmdType, cmdText, commandParameters)
                Dim rdr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                cmd.Parameters.Clear()
                Return rdr
            Catch ex As Exception
                conn.Close()
                Throw New LSWFrameworkException(ex)
            End Try
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="connectionString"></param>
        ''' <param name="cmdType"></param>
        ''' <param name="cmdText"></param>
        ''' <param name="commandParameters">ParamArray 表示函数参数个数不确定C#中为params</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteScalar(ByVal connectionString As String, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter()) As Object
            Dim cmd As SqlCommand = New SqlCommand
            Using connection As SqlConnection = New SqlConnection(connectionString)
                PrepareCommand(cmd, connection, Nothing, cmdType, cmdText, commandParameters)
                Dim val As Object = cmd.ExecuteScalar()
                cmd.Parameters.Clear()
                Return val
            End Using
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <param name="cmdType"></param>
        ''' <param name="cmdText"></param>
        ''' <param name="commandParameters">ParamArray 表示函数参数个数不确定C#中为params</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExecuteScalar(ByVal connection As SqlConnection, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray commandParameters As SqlParameter()) As Object
            Dim cmd As SqlCommand = New SqlCommand
            PrepareCommand(cmd, connection, Nothing, cmdType, cmdText, commandParameters)
            Dim val As Object = cmd.ExecuteScalar()
            cmd.Parameters.Clear()
            Return val
        End Function
        ''' <summary>
        ''' add parameter array to the cache
        ''' </summary>
        ''' <param name="cacheKey"></param>
        ''' <param name="commandParameters">an array of SqlParamters to be cached</param>
        ''' <remarks></remarks>
        Public Sub CacheParameters(ByVal cacheKey As String, ByVal ParamArray commandParameters As SqlParameter())
            parmCache(cacheKey) = commandParameters
        End Sub

        Public Function GetCachedParameters(ByVal cacheKey As String) As SqlParameter()
            Dim cachedParms As SqlParameter() = CType(parmCache(cacheKey), SqlParameter())
            If cachedParms Is Nothing Then Return Nothing
            Dim clonedParms(cachedParms.Length - 1) As SqlParameter
            Dim i As Integer
            For i = 0 To cachedParms.Length - 1
                clonedParms(i) = CType(CType(cachedParms(i), ICloneable).Clone(), SqlParameter)
            Next
            Return clonedParms
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' <param name="conn"></param>
        ''' <param name="trans"></param>
        ''' <param name="cmdType"></param>
        ''' <param name="cmdText"></param>
        ''' <param name="cmdParms"></param>
        ''' <remarks></remarks>
        Private Sub PrepareCommand(ByVal cmd As SqlCommand, ByVal conn As SqlConnection, ByVal trans As SqlTransaction, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal cmdParms As SqlParameter())
            If conn.State <> ConnectionState.Open Then conn.Open()
            cmd.Connection = conn
            cmd.CommandText = cmdText
            If trans IsNot Nothing Then cmd.Transaction = trans
            cmd.CommandType = cmdType
            If cmdParms IsNot Nothing Then
                Dim parm As SqlParameter
                For Each parm In cmdParms
                    cmd.Parameters.Add(parm)
                Next
            End If
        End Sub
    End Module
End Namespace
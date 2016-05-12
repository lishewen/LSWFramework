Imports System.Net
Imports System.Text
Imports System.IO

Namespace Net
    Public Module Cookie
        ''' <summary>
        ''' 遍历CookieContainer
        ''' </summary>
        ''' <param name="cc"></param>
        ''' <returns></returns>
        Public Function GetAllCookies(cc As CookieContainer) As List(Of System.Net.Cookie)
            Dim lstCookies As New List(Of System.Net.Cookie)

            Dim table As Hashtable = DirectCast(cc.[GetType]().InvokeMember("m_domainTable", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.GetField Or System.Reflection.BindingFlags.Instance, Nothing, cc, New Object() {}), Hashtable)

            For Each pathList As Object In table.Values
                Dim lstCookieCol As SortedList = DirectCast(pathList.[GetType]().InvokeMember("m_list", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.GetField Or System.Reflection.BindingFlags.Instance, Nothing, pathList, New Object() {}), SortedList)
                For Each colCookies As CookieCollection In lstCookieCol.Values
                    For Each c As System.Net.Cookie In colCookies
                        lstCookies.Add(c)
                    Next
                Next
            Next
            Return lstCookies
        End Function
        ''' <summary>
        ''' 把cookieContainer保存到文件
        ''' </summary>
        ''' <param name="Cookies">需要写入的CookieContainer</param>
        ''' <param name="cookieFile">保存到什么文件</param>
        Public Sub SaveCookieContainer(Cookies As CookieContainer, cookieFile As String)
            Dim SB As New StringBuilder()
            If Cookies Is Nothing OrElse Cookies.Count = 0 Then
                Return
            End If
            '检测目录是否存在
            If Not File.Exists(cookieFile) Then
                Dim cookieFileInfo As New FileInfo(cookieFile)
                If Not cookieFileInfo.Directory.Exists Then
                    cookieFileInfo.Directory.Create()
                End If
            End If
            Dim CClist As List(Of System.Net.Cookie) = GetAllCookies(Cookies)
            For Each Cookie As System.Net.Cookie In CClist
                SB.AppendFormat("{0}|,|{1}|,|{2}|,|{3}|,|{4}" & vbCr & vbLf, Cookie.Name, Cookie.Value, Cookie.Expires.Ticks, Cookie.Domain, Cookie.Path)
            Next
            File.WriteAllText(cookieFile, SB.ToString())
        End Sub
        ''' <summary>
        ''' 从文件获取cookies
        ''' </summary>
        ''' <param name="cookieFile">cookie保存的文件</param>
        ''' <returns></returns>
        Public Function GetCookieContainer(cookieFile As String) As CookieContainer
            Dim returnCookieContainer As New CookieContainer()
            If Not File.Exists(cookieFile) Then
                Return returnCookieContainer
            End If
            Dim CookieStrS As String() = File.ReadAllLines(cookieFile)

            For Each CookieStr As String In CookieStrS
                Dim TempStrS As String() = CookieStr.Split(New String() {"|,|"}, StringSplitOptions.RemoveEmptyEntries)
                If TempStrS.Length < 5 Then
                    Continue For
                End If
                Dim CookieTemp As New System.Net.Cookie
                CookieTemp.Name = TempStrS(0)
                CookieTemp.Value = TempStrS(1)
                CookieTemp.Expires = New DateTime(Convert.ToInt64(TempStrS(2)))
                CookieTemp.Domain = TempStrS(3)
                CookieTemp.Path = TempStrS(4)
                returnCookieContainer.Add(CookieTemp)
            Next
            Return returnCookieContainer
        End Function
    End Module
End Namespace
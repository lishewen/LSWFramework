Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Namespace Files
    ''' <summary>
    ''' 重命名辅助类
    ''' </summary>
    Public Module ReNameHelper
        ''' <summary>
        ''' 保存同名文件时，进行重命名操作.
        ''' 例如文件夹内已存在1.txt，则再次保存时保存为1(1).txt,....1(n).txt
        ''' </summary>
        ''' <param name="strFolderPath">保存的目录</param>
        ''' <param name="strFileName">文件名</param>
        ''' <returns>新的文件名</returns>
        Public Function ReFileName(strFolderPath As String, strFileName As String) As String
            '当前传进的文件名自带的索引
            Dim intCurrentFileIndex As Integer = 0
            Dim strNewName As String = String.Empty
            '用来保存当前目录下的文件的最大索引。
            Dim intMaxIndex As Integer = 0
            '如果文件不存在，直接返回
            If Not File.Exists(Path.Combine(strFolderPath, strFileName)) Then
                Return strFileName
            End If
            '根据传进来的文件名，获取扩展名
            Dim strExtention As String = Path.GetExtension(strFileName)
            Dim strFileNameWithoutExtion As String = Path.GetFileNameWithoutExtension(strFileName)
            '如果文件名中本身就包括括号，则需要还原原来的文件名
            Dim strNameContainsBracketsRegex As String = Convert.ToString("(?<fileName>.+?)" + "([(（](?<fileNameIndex>\d+)[）)])") & strExtention
            Dim regexContain As New Regex(strNameContainsBracketsRegex, RegexOptions.Singleline)
            If regexContain.IsMatch(strFileName) Then
                Dim match As Match = regexContain.Match(strFileName)
                strFileNameWithoutExtion = match.Groups("fileName").Value
                intCurrentFileIndex = Convert.ToInt32(match.Groups("fileNameIndex").Value)
            End If
            '根据传进来的文件名，通过正则匹配，找到类似的文件名,不区别中英文的括号并且括号及索引可有可无
            Dim strRegex As String = Convert.ToString(strFileNameWithoutExtion & Convert.ToString("([(（](?<fileNameIndex>\d+)[）)])")) & strExtention
            Dim regex As New Regex(strRegex, RegexOptions.Singleline)
            Dim strFileNames As String() = Directory.GetFiles(strFolderPath, Convert.ToString("*") & strExtention).Where(Function(x) regex.IsMatch(x) OrElse x.Contains(strFileNameWithoutExtion)).ToArray()
            If strFileNames IsNot Nothing AndAlso strFileNames.Length > 0 Then
                For Each item As String In strFileNames
                    '因为获得的文件路径数组中都是匹配成功的路径，此处不再进行判断是否匹配
                    Dim match As Match = regex.Match(item)
                    '获得索引
                    Dim strIndex As String = match.Groups("fileNameIndex").Value
                    '如果为空，说明只有类似 1.txt这样的文件，则返回的文件就是1(1).txt
                    '否则找到最大索引，然后拼接最大索引加一的文件名
                    If Not String.IsNullOrEmpty(strIndex) Then
                        Dim intIndex As Integer = Convert.ToInt32(strIndex)
                        If intMaxIndex < intIndex Then
                            intMaxIndex = intIndex
                        End If
                    End If
                Next
                '如果目录中存在的文件索引大于或者等于当前传进来的文件的索引则使用新的名称，否则将返回传进来的文件名称
                If intMaxIndex >= intCurrentFileIndex Then
                    '循环接收，求出了最大的索引，则新文件的索引就是最大索引加一
                    Dim sb As New StringBuilder()
                    sb.Append(strFileNameWithoutExtion)
                    sb.Append("(")
                    sb.Append((intMaxIndex + 1).ToString())
                    sb.Append(")")
                    sb.Append(strExtention)
                    strNewName = sb.ToString()
                Else
                    strNewName = strFileName
                End If
            Else
                '如果没有匹配到相似的文件名结构，则说明是一个新的文件，则不做任何操作
                strNewName = strFileName
            End If
            Return strNewName
        End Function
        ''' <summary>
        ''' 保存同名文件夹时，进行重命名操作.
        ''' 例如文件夹内已存在1的文件夹，则再次保存时保存为1(1),....1(n)
        ''' </summary>
        ''' <param name="strFolderPath">保存的目录</param>
        ''' <param name="strFolderName">保存的目录</param>
        ''' <returns>新的目录名</returns>
        Public Function ReFolderName(strFolderPath As String, strFolderName As String) As String
            '当前传进的文件夹自带的索引
            Dim intCurrentFolderIndex As Integer = 0
            Dim strNewName As String = String.Empty
            '原始名字
            Dim strOriginalName As String = strFolderName
            '用来保存当前目录下的文件的最大索引。
            Dim intMaxIndex As Integer = 0
            If Not Directory.Exists(Path.Combine(strFolderPath, strFolderName)) Then
                Return strFolderName
            End If
            '根据传进来的文件名，通过正则匹配，找到文件夹是否已经带有索引。
            Dim strRegex As String = "(?<folderName>.+?)([(（](?<folderIndex>\d+)[）)])"
            Dim regex As New Regex(strRegex, RegexOptions.Singleline)
            If regex.IsMatch(strFolderName) Then
                Dim match As Match = regex.Match(strFolderName)
                Dim strFolderIndex As String = match.Groups("folderIndex").Value
                If Not String.IsNullOrEmpty(strFolderIndex) Then
                    intCurrentFolderIndex = Convert.ToInt32(strFolderIndex)
                    strOriginalName = match.Groups("folderName").Value
                End If
            End If

            Dim strFolderNames As String() = Directory.GetDirectories(strFolderPath).Where(Function(x) regex.IsMatch(x)).[Select](Function(x) x.Split(New Char() {"\"c}).LastOrDefault()).ToArray()
            If strFolderNames IsNot Nothing AndAlso strFolderNames.Length > 0 Then
                For Each item As String In strFolderNames
                    '因为获得的文件路径数组中都是匹配成功的路径，此处不再进行判断是否匹配
                    Dim match As Match = regex.Match(item)
                    '获得索引
                    Dim strIndex As String = match.Groups("folderIndex").Value
                    '如果为空，说明只有类似 1.txt这样的文件，则返回的文件就是1(1).txt
                    '否则找到最大索引，然后拼接最大索引加一的文件名
                    If Not String.IsNullOrEmpty(strIndex) Then
                        Dim intIndex As Integer = Convert.ToInt32(strIndex)
                        If intMaxIndex < intIndex Then
                            intMaxIndex = intIndex
                        End If
                    End If
                Next
                '如果目录中存在的文件索引大于或者等于当前传进来的文件的索引则使用新的名称，否则将返回传进来的文件名称
                If intMaxIndex >= intCurrentFolderIndex Then
                    '循环接收，求出了最大的索引，则新文件的索引就是最大索引加一
                    Dim sb As New StringBuilder()
                    sb.Append(strOriginalName)
                    sb.Append("(")
                    sb.Append((intMaxIndex + 1).ToString())
                    sb.Append(")")
                    strNewName = sb.ToString()
                Else
                    strNewName = strFolderName
                End If
            Else
                '如果没有匹配到相似的文件名结构，则说明是一个新的文件，则不做任何操作
                strNewName = strFolderName
            End If
            Return strNewName
        End Function
        ''' <summary>
        ''' 对文件进行重命名
        ''' </summary>
        ''' <param name="strFilePath"></param>
        ''' <returns></returns>
        Public Function FileReName(strFilePath As String) As String
            '判断该文件是否存在，存在则返回新名字，否则返回原来的名
            If Not File.Exists(strFilePath) Then
                Return Path.GetFileName(strFilePath)
            Else
                '获取不带扩展名的文件名称
                Dim strFileNameWithoutExtension As String = Path.GetFileNameWithoutExtension(strFilePath)
                '获取扩展名
                Dim strFileExtension As String = Path.GetExtension(strFilePath)
                '获取目录名
                Dim strDirPath As String = Path.GetDirectoryName(strFilePath)
                '以文件名开头和结尾的正则
                Dim strRegex As String = (Convert.ToString("^") & strFileNameWithoutExtension) + "(\d+)?"
                Dim regex As New Regex(strRegex)
                '获取该路径下类似的文件名
                Dim strFilePaths As String() = Directory.GetFiles(strDirPath, Convert.ToString("*") & strFileExtension).Where(Function(path__1) regex.IsMatch(Path.GetFileNameWithoutExtension(path__1))).ToArray()
                '获得新的文件名
                Return Convert.ToString((strFileNameWithoutExtension & Convert.ToString("(")) + (strFilePaths.Length + 1).ToString() + ")") & strFileExtension
            End If
        End Function
        ''' <summary>
        ''' 文件夹已存在，重命名
        ''' </summary>
        ''' <param name="strFolderPath"></param>
        ''' <returns></returns>
        Public Function FolderReName(strFolderPath As String) As String
            '判断该文件夹是否存在，存在则返回新名字，否则返回原来的名
            If Not Directory.Exists(strFolderPath) Then
                Return Path.GetFileName(strFolderPath)
            Else
                '获取文件夹名
                Dim strFolderName As String = Path.GetFileName(strFolderPath)
                '获取目录名
                Dim strDirPath As String = Path.GetDirectoryName(strFolderPath)
                '以文件夹名开头和结尾的正则
                Dim strRegex As String = (Convert.ToString("^") & strFolderName) + "(\d+)?"
                Dim regex As New Regex(strRegex)
                '获取该路径下类似的文件夹名
                Dim strFilePaths As String() = Directory.GetDirectories(strDirPath).Where(Function(path__1) regex.IsMatch(Path.GetFileName(path__1))).ToArray()
                '获得新的文件名
                Return (strFolderName & Convert.ToString("(")) + (strFilePaths.Length + 1).ToString() + ")"
            End If
        End Function

    End Module
End Namespace
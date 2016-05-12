Imports System.Reflection
Imports System.Reflection.Emit
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Collections.Specialized
Imports LSW.Net
Imports System.Text
Imports LSW.Exceptions

Namespace Common
    Public Module Runtime
        ''' <summary>获取PE文件类型。扩展方法</summary>
        ''' <param name="e"></param>
        ''' <returns></returns>
        <Extension> _
        Public Function GetPEFileKinds(e As MemberInfo) As PEFileKinds
            Return GetPEFileKinds(Path.GetFullPath(e.Module.Assembly.Location))
        End Function

        ''' <summary>Parses the PE header and determines whether the given assembly is a console application.</summary>
        ''' <param name="assemblyPath">The path of the assembly to check.</param>
        ''' <remarks>The magic numbers in this method are extracted from the PE/COFF file
        ''' format specification available from http://www.microsoft.com/whdc/system/platform/firmware/pecoff.mspx
        ''' </remarks>
        Public Function GetPEFileKinds(assemblyPath As String) As PEFileKinds
            Using s = New FileStream(assemblyPath, FileMode.Open, FileAccess.Read)
                Return GetPEFileKinds(s)
            End Using
        End Function

        Private Function GetPEFileKinds(s As Stream) As PEFileKinds
            Dim rawPeSignatureOffset = New Byte(3) {}
            s.Seek(&H3C, SeekOrigin.Begin)
            s.Read(rawPeSignatureOffset, 0, 4)
            Dim peSignatureOffset As Integer = rawPeSignatureOffset(0)
            peSignatureOffset = peSignatureOffset Or rawPeSignatureOffset(1) << 8
            peSignatureOffset = peSignatureOffset Or rawPeSignatureOffset(2) << 16
            peSignatureOffset = peSignatureOffset Or rawPeSignatureOffset(3) << 24
            Dim coffHeader = New Byte(23) {}
            s.Seek(peSignatureOffset, SeekOrigin.Begin)
            s.Read(coffHeader, 0, 24)
            Dim signature As Byte() = {CByte(AscW("P"c)), CByte(AscW("E"c)), CByte(AscW(ControlChars.NullChar)), CByte(AscW(ControlChars.NullChar))}
            For index As Integer = 0 To 3
                If coffHeader(index) <> signature(index) Then
                    Throw New InvalidOperationException("Attempted to check a non PE file for the console subsystem!")
                End If
            Next
            Dim subsystemBytes = New Byte(1) {}
            s.Seek(68, SeekOrigin.Current)
            s.Read(subsystemBytes, 0, 2)
            Dim subSystem As Integer = subsystemBytes(0) Or subsystemBytes(1) << 8
            ' http://support.microsoft.com/kb/90493
            Return If(subSystem = 3, PEFileKinds.ConsoleApplication, If(subSystem = 2, PEFileKinds.WindowApplication, PEFileKinds.Dll))
            'IMAGE_SUBSYSTEM_WINDOWS_CUI
        End Function

        <Extension>
        Public Sub Run(rawbyte() As Byte)
            Run(rawbyte, Nothing)
        End Sub

        <Extension>
        Public Sub Run(rawbyte() As Byte, ParamArray args() As Object)
            Dim asm = Assembly.Load(rawbyte)
            Dim mi = asm.EntryPoint
            mi.Invoke(Nothing, args)
        End Sub

        Public Sub Run(path As String)
            Run(path, Nothing)
        End Sub

        Public Sub Run(path As String, ParamArray args() As Object)
            Dim asm = Assembly.Load(path)
            Dim mi = asm.EntryPoint
            mi.Invoke(Nothing, args)
        End Sub

        Public Function CheckUpdate() As Boolean
            Try
                'If My.Computer.Network.Ping("softwave.lishewen.com") Then
                Const FeedBackUrl = "http://softwave.lishewen.com/api/checkupdate"
                Dim datas As New NameValueCollection
                datas.Add("a", "CheckUpdate")
                datas.Add("s", Assembly.GetExecutingAssembly.GetName.Version.ToString(4))
                Dim result = Url.Post(datas, FeedBackUrl, Encoding.UTF8)
                If Url.Post(datas, FeedBackUrl, Encoding.UTF8) = "T" Then
                    Return True
                Else
                    Return False
                End If
                'Else
                'Return False
                'End If
            Catch ex As Exception
                Dim e As New LSWFrameworkException(ex)
                Return False
            End Try
        End Function

        Public Function GetAssembly(exeFullName As String) As Assembly
            If String.IsNullOrEmpty(exeFullName) Then
                Return Nothing
            End If
            Return Assembly.LoadFile(exeFullName)
        End Function

        Public Function GetAssemblyDetails(file As String) As AssemblyDetails
            Dim assembInfo As New AssemblyDetails
            Using s As New FileStream(file, FileMode.Open, FileAccess.Read)
                Using r As New BinaryReader(s)
                    Dim bytes = r.ReadBytes(2)
                    If bytes(0) <> &H4D OrElse bytes(1) <> &H5A Then
                        Return Nothing
                    End If

                    s.Seek(&H3C, SeekOrigin.Begin)

                    Dim offset = r.ReadUInt32

                    s.Seek(offset, SeekOrigin.Begin)

                    bytes = r.ReadBytes(4)

                    If bytes(0) <> &H50 OrElse bytes(1) <> &H45 OrElse bytes(2) <> 0 OrElse bytes(3) <> 0 Then
                        Return Nothing
                    End If

                    Dim machineCode = r.ReadUInt16

                    If Not (machineCode = &H14C OrElse machineCode = &H8664) Then
                        Return Nothing
                    End If

                    s.Seek(18, SeekOrigin.Current)

                    Dim magic = r.ReadUInt16

                    Select Case magic
                        Case &H10B
                            assembInfo.CPUVersion = CPUVersion.AnyCPU
                        Case &H20B
                            assembInfo.CPUVersion = CPUVersion.X64
                        Case Else
                            Return Nothing
                    End Select

                    s.Seek(30, SeekOrigin.Current)

                    Dim sectionAlignment = r.ReadUInt32
                    Dim fileAlignment = r.ReadUInt32

                    s.Seek(IIf(magic = &H10B, 52, 68), SeekOrigin.Current)

                    Dim numDataDirs = r.ReadUInt32

                    If numDataDirs <> &H10 Then
                        Return Nothing
                    End If

                    s.Seek(112, SeekOrigin.Current)

                    Dim rvaCLIHeader = r.ReadUInt32

                    If rvaCLIHeader = 0 Then
                        Return Nothing
                    End If

                    s.Seek((rvaCLIHeader - sectionAlignment + fileAlignment) + 4, SeekOrigin.Begin)

                    Dim majorVersion = r.ReadUInt16
                    Dim minorVersion = r.ReadUInt16

                    Dim rvaMetaData = r.ReadUInt32
                    s.Seek(4, SeekOrigin.Current)

                    Dim cliFlags = r.ReadUInt32

                    If assembInfo.CPUVersion = CPUVersion.AnyCPU AndAlso (cliFlags And &H2) = &H2 Then
                        assembInfo.CPUVersion = CPUVersion.X86
                    End If

                    assembInfo.HasStrongName = (cliFlags And &H8) = &H8
                End Using
            End Using
            Return assembInfo
        End Function
    End Module
End Namespace
Imports System.CodeDom.Compiler
Imports Microsoft.CSharp
Imports System.ComponentModel
Imports System.Reflection
Imports System.Collections.Specialized

Namespace Common
    Public Module Compiler
        Public Event SetReferenced(ReferencedAssemblies As StringCollection)
        Public Event GetErrors(errors As CompilerErrorCollection)

        Public Sub CompileExecutable(ByVal source As String, ByVal iconName As String, Optional lang As Language = Language.VB, Optional assemblyname As String = "lswd.dll", Optional tar As Target = Target.WinExe)
            Dim provider As CodeDomProvider = Nothing
            Dim results As CompilerResults = Nothing
            Select Case lang
                Case Language.VB
                    provider = New VBCodeProvider
                Case Language.CS
                    provider = New CSharpCodeProvider
            End Select

            If (Not provider Is Nothing) Then
                Dim options As New CompilerParameters
                options.ReferencedAssemblies.Add("mscorlib.dll")
                options.ReferencedAssemblies.Add("system.dll")
                options.ReferencedAssemblies.Add("system.windows.forms.dll")
                If lang = Language.VB Then
                    options.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll")
                End If


                RaiseEvent SetReferenced(options.ReferencedAssemblies)

                If tar = Target.Exe OrElse tar = Target.WinExe Then
                    If (iconName.Trim <> "") Then
                        options.CompilerOptions = (options.CompilerOptions & " /win32icon:""" & iconName & """")
                    End If
                    options.GenerateExecutable = True
                Else
                    options.GenerateExecutable = False
                End If

                Dim tag As String
                Select Case tar
                    Case Target.AppcontainerExe
                        tag = "appcontainerexe"
                    Case Target.Exe
                        tag = "exe"
                    Case Target.Library
                        tag = "library"
                    Case Target.Module
                        tag = "module"
                    Case Target.WinExe
                        tag = "winexe"
                    Case Target.WinmdObj
                        tag = "winmdobj"
                    Case Else
                        tag = "winexe"
                End Select
                options.CompilerOptions = (options.CompilerOptions & " /target:" & tag)

                options.OutputAssembly = assemblyname
                options.GenerateInMemory = False
                options.TreatWarningsAsErrors = False
                results = provider.CompileAssemblyFromSource(options, New String() {source})

                If (results.Errors.Count > 0) Then
                    RaiseEvent GetErrors(results.Errors)
#If DEBUG Then
                    Debug.WriteLine("Errors building {0} into {1}", "Stub", results.PathToAssembly)
                    Dim [error] As CompilerError
                    For Each [error] In results.Errors
                        Debug.WriteLine("  {0}", [error].ToString)
                        Debug.WriteLine(" ")
                    Next
#End If
                End If
            End If
            'Return 0
        End Sub

        Public Function CompileExecutable(ByVal source As String, Optional lang As Language = Language.VB, Optional assemblyname As String = "lswd.dll", Optional tar As Target = Target.WinExe) As Assembly
            Dim provider As CodeDomProvider = Nothing
            Dim results As CompilerResults = Nothing
            Select Case lang
                Case Language.VB
                    provider = New VBCodeProvider
                Case Language.CS
                    provider = New CSharpCodeProvider
            End Select

            If (Not provider Is Nothing) Then
                Dim options As New CompilerParameters
                options.ReferencedAssemblies.Add("mscorlib.dll")
                options.ReferencedAssemblies.Add("system.dll")
                options.ReferencedAssemblies.Add("system.windows.forms.dll")
                If lang = Language.VB Then
                    options.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll")
                End If

                RaiseEvent SetReferenced(options.ReferencedAssemblies)

                If tar = Target.Exe OrElse tar = Target.WinExe Then
                    options.GenerateExecutable = True
                Else
                    options.GenerateExecutable = False
                End If

                Dim tag As String
                Select Case tar
                    Case Target.AppcontainerExe
                        tag = "appcontainerexe"
                    Case Target.Exe
                        tag = "exe"
                    Case Target.Library
                        tag = "library"
                    Case Target.Module
                        tag = "module"
                    Case Target.WinExe
                        tag = "winexe"
                    Case Target.WinmdObj
                        tag = "winmdobj"
                    Case Else
                        tag = "winexe"
                End Select
                options.CompilerOptions = (options.CompilerOptions & " /target:" & tag)

                options.OutputAssembly = assemblyname
                options.GenerateInMemory = True
                options.TreatWarningsAsErrors = False
                results = provider.CompileAssemblyFromSource(options, New String() {source})

                If (results.Errors.Count > 0) Then
                    RaiseEvent GetErrors(results.Errors)
#If DEBUG Then
                    Debug.WriteLine("Errors building {0} into {1}", "Stub", results.PathToAssembly)
                    Dim [error] As CompilerError
                    For Each [error] In results.Errors
                        Debug.WriteLine("  {0}", [error].ToString)
                        Debug.WriteLine(" ")
                    Next
#End If
                End If

                Return results.CompiledAssembly
            End If
            Return Nothing
        End Function
    End Module

    Public Enum Language
        <Description("Visual Basic")>
        VB
        <Description("CSharp")>
        CS
    End Enum

    Public Enum Target
        ''' <summary>
        ''' 创建 Windows 应用商店 app 的.exe 文件。
        ''' </summary>
        ''' <remarks></remarks>
        AppcontainerExe
        ''' <summary>
        ''' 创建 .exe 文件。
        ''' </summary>
        ''' <remarks></remarks>
        Exe
        ''' <summary>
        ''' 创建代码库。
        ''' </summary>
        ''' <remarks></remarks>
        Library
        ''' <summary>
        ''' 创建模块。
        ''' </summary>
        ''' <remarks></remarks>
        [Module]
        ''' <summary>
        ''' 创建 Windows 程序。
        ''' </summary>
        ''' <remarks></remarks>
        WinExe
        ''' <summary>
        ''' 创建一个中间.winmdobj 文件。
        ''' </summary>
        ''' <remarks></remarks>
        WinmdObj
    End Enum
End Namespace
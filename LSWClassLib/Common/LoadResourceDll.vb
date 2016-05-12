Imports System.Reflection
Imports LSW.Exceptions

Namespace Common
    Public Module LoadResourceDll
        Dim Dlls As New Dictionary(Of String, Assembly)
        Dim Assemblies As New Dictionary(Of String, Object)

        Public Function AssemblyResolve(sender As Object, args As ResolveEventArgs) As Assembly
            Dim ass As Assembly = Nothing
            Dim assName = New AssemblyName(args.Name).FullName
            If Dlls.TryGetValue(assName, ass) AndAlso ass IsNot Nothing Then
                Dlls(assName) = Nothing
                Return ass
            Else
                Throw New LSWFrameworkException(New DllNotFoundException(assName))
            End If
        End Function

        Public Sub RegistDLLFromResources()
            Dim ass = New StackTrace(0).GetFrame(1).GetMethod.Module.Assembly
            If Assemblies.ContainsKey(ass.FullName) Then
                Exit Sub
            End If
            Assemblies.Add(ass.FullName, Nothing)
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf AssemblyResolve
            Dim res = ass.GetManifestResourceNames
            For Each r In res
                If r.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) OrElse r.EndsWith(".lswdll", StringComparison.OrdinalIgnoreCase) Then
                    Try
                        Dim s = ass.GetManifestResourceStream(r)
                        Dim bts(s.Length - 1) As Byte
                        s.Read(bts, 0, s.Length)
                        Dim da = Assembly.Load(bts)
                        If Dlls.ContainsKey(da.FullName) Then
                            Continue For
                        End If
                        Dlls(da.FullName) = da
                    Catch ex As Exception
                        Dim e As New LSWFrameworkException(ex)
                    End Try
                End If
            Next
        End Sub
    End Module
End Namespace
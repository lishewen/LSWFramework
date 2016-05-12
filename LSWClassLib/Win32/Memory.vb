Imports System.Text
Imports System.Runtime.InteropServices

Namespace Win32
    Public Module Memory
        Private Allocated As List(Of IntPtr) = New List(Of IntPtr)
        Private processes As Process()
        Private SelectedProcess As Integer = 0

        Public Function Alloc(ByVal size As Integer) As IntPtr
            Dim item As IntPtr = VirtualAllocEx(Memory.processes(Memory.SelectedProcess).Handle, IntPtr.Zero, New IntPtr(size), (AllocationType.Reserve Or AllocationType.Commit), MemoryProtection.ExecuteReadWrite)
            Memory.Allocated.Add(item)
            Return item
        End Function

        Public Sub FreeAll()
            Dim ptr As IntPtr
            For Each ptr In Memory.Allocated
                VirtualFreeEx(Memory.processes(Memory.SelectedProcess).Handle, ptr, 0, AllocationType.Release)
            Next
        End Sub

        Public Function Read(Of T As Structure)(ByVal offset As IntPtr) As T
            Dim lpBuffer As Byte() = New Byte(Marshal.SizeOf(GetType(T)) - 1) {}
            ReadProcessMemory(Memory.processes(Memory.SelectedProcess).Handle, offset, lpBuffer, New IntPtr(lpBuffer.Length), IntPtr.Zero)
            Dim handle As GCHandle = GCHandle.Alloc(lpBuffer, GCHandleType.Pinned)
            Dim local As T = DirectCast(Marshal.PtrToStructure(handle.AddrOfPinnedObject, GetType(T)), T)
            handle.Free()
            Return local
        End Function

        Public Function Read(ByVal offset As IntPtr, ByVal length As Integer) As Byte()
            Dim lpBuffer As Byte() = New Byte(length - 1) {}
            ReadProcessMemory(Memory.processes(Memory.SelectedProcess).Handle, offset, lpBuffer, New IntPtr(length), IntPtr.Zero)
            Return lpBuffer
        End Function

        Public Function Read(Of T As Structure)(ByVal baseAddr As IntPtr, ByVal readbase As Boolean, ByVal ParamArray offsets As Integer()) As T
            Dim ptr As IntPtr
            If readbase Then
                ptr = Memory.Read(Of IntPtr)(baseAddr)
            Else
                ptr = baseAddr
            End If
            If (ptr <> IntPtr.Zero) Then
                Dim i As Integer
                For i = 0 To offsets.Length - 1
                    If (i = (offsets.Length - 1)) Then
                        Return Memory.Read(Of T)(New IntPtr((ptr.ToInt64 + offsets(i))))
                    End If
                    ptr = Memory.Read(Of IntPtr)(New IntPtr((ptr.ToInt64 + offsets(i))))
                    If (ptr = IntPtr.Zero) Then
                        Exit For
                    End If
                Next i
            End If
            Return CType(Nothing, T)
        End Function

        Public Function ReadCString(ByVal baseAddr As IntPtr, Optional ByVal maxLen As Integer = &HFF) As String
            Return Encoding.UTF8.GetString(Memory.Read(baseAddr, maxLen).TakeWhile(Function(b)
                                                                                       Return b <> 0
                                                                                   End Function))
        End Function

        Public Sub SelectProcess(ByVal index As Integer)
            Memory.SelectedProcess = index
        End Sub

        Public Sub SetProcesses(ByVal _processes As Process())
            Memory.processes = _processes
        End Sub

        Public Function Write(ByVal offset As IntPtr, ByVal data As Byte()) As Boolean
            Return WriteProcessMemory(Memory.processes(Memory.SelectedProcess).Handle, offset, data, New IntPtr(data.Length), IntPtr.Zero)
        End Function

        Public Sub Write(Of T As Structure)(ByVal offset As IntPtr, ByVal value As T)
            Dim destination As Byte() = New Byte(Marshal.SizeOf(value) - 1) {}
            Dim ptr As IntPtr = Marshal.AllocHGlobal(destination.Length)
            Try
                Marshal.StructureToPtr(value, ptr, False)
                Marshal.Copy(ptr, destination, 0, destination.Length)
                Memory.Write(offset, destination)
            Finally
                Marshal.FreeHGlobal(ptr)
            End Try
        End Sub

        Public Sub Write(Of T As Structure)(ByVal baseAddr As IntPtr, ByVal value As T, ByVal readbase As Boolean, ByVal ParamArray offsets As Integer())
            Dim ptr As IntPtr
            If readbase Then
                ptr = Memory.Read(Of IntPtr)(baseAddr)
            Else
                ptr = baseAddr
            End If
            If (ptr <> IntPtr.Zero) Then
                Dim i As Integer
                For i = 0 To offsets.Length - 1
                    If (i = (offsets.Length - 1)) Then
                        Memory.Write(Of T)(New IntPtr((ptr.ToInt64 + offsets(i))), value)
                        Return
                    End If
                    ptr = Memory.Read(Of IntPtr)(New IntPtr((ptr.ToInt64 + offsets(i))))
                    If (ptr = IntPtr.Zero) Then
                        Return
                    End If
                Next i
            End If
        End Sub

        Public Function WriteCString(ByVal offset As IntPtr, ByVal str As String) As Boolean
            Return Memory.Write(offset, Encoding.UTF8.GetBytes((str & ChrW(0))))
        End Function

        Public ReadOnly Property BaseAddress As IntPtr
            Get
                Return Memory.processes(Memory.SelectedProcess).MainModule.BaseAddress
            End Get
        End Property
    End Module
End Namespace
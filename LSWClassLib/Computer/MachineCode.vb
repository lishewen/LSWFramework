Imports System.Management
Imports LSW.Security

Namespace Computer
    Public Module MachineCode
        Public Function GetCpuInfo() As String
            Dim str As String = " "
            Using mc As ManagementClass = New ManagementClass("Win32_Processor")
                Dim instances As ManagementObjectCollection = mc.GetInstances
                For Each obj In instances
                    str = obj.Properties.Item("ProcessorId").Value.ToString
                    obj.Dispose()
                Next
            End Using
            Return str.ToString
        End Function

        Public Function GetHDid() As String
            Dim str As String = " "
            Using mc As ManagementClass = New ManagementClass("Win32_DiskDrive")
                Dim instances As ManagementObjectCollection = mc.GetInstances
                For Each obj In instances
                    str = CStr(obj.Properties.Item("Model").Value)
                    obj.Dispose()
                Next
            End Using
            Return str.ToString
        End Function

        Public Function GetMoAddress() As String
            Dim str As String = " "
            Using mc As ManagementClass = New ManagementClass("Win32_NetworkAdapterConfiguration")
                Dim instances As ManagementObjectCollection = mc.GetInstances
                For Each obj In instances
                    If CBool(obj.Item("IPEnabled")) Then
                        str = obj.Item("MacAddress").ToString
                    End If
                    obj.Dispose()
                Next
            End Using
            Return str.ToString
        End Function

        Public Function GetCode() As String
            Return DES.Encrypt(MachineCode.GetCpuInfo & MachineCode.GetHDid & MachineCode.GetMoAddress)
        End Function
    End Module
End Namespace
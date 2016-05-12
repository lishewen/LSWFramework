Namespace Security
    Public Module Flash
        Const Ey_str = "0123456789abcdefghijklmnopqrstuvwxyz"
        Dim Em_str() As String = {
            "xyi0grm9as78vhzu6k234lef1pqcw5ojtdnb",
            "vxyi0gr6kq4lef1pobm9a23s78hzcw5djtnu",
            "q4l78k23f1potnhzcudjyi0gr6bvxm9aw5se",
            "f1q4ldjpotnhi0grawbvxm9e5s3u78k2zc6y",
            "nh3zu78kc6ytf1aq4ldjpowbvxm9i0gre5s2",
            "um92re5snh3z678kcqytf1gvxaowb4ldjpi0",
            "re5swbao8kum91gvxc2nh3z67fdjpi0qyt4l",
            "ao8re5kusyt4nh3zwbxc2m910qgv67fdjpil",
            "5axc2m67f0qgvildjpkusywbnh3z91o8ret4",
            "7ldjpk8ruvixf05aqg3z9cnet41hwbo2m6sy"
        }
        Public Function Encrypt(str As String) As String
            Dim local2 = str.Split(",")
            Dim local3 = ",".Split(",")
            Dim local7 = CInt(local2(2).Substring(0, 1))
            Dim local6 = CInt(local2(2).Substring(3, 1))
            Dim i = 0
            Dim local4 As String
            Dim local5 As Integer
            While i < local2(0).Length
                local4 = local2(0).Substring(i, 1)
                local5 = Ey_str.IndexOf(local4)
                If local5 < 0 Then
                    local3(0) &= local4
                Else
                    local3(0) &= Em_str(local7).Substring(local5, 1)
                End If
                i += 1
            End While
            i = 0
            While i < local2(1).Length
                local4 = local2(1).Substring(i, 1)
                local5 = Ey_str.IndexOf(local4)
                If local5 < 0 Then
                    local3(1) &= local4
                Else
                    local3(1) &= Em_str(local6).Substring(local5, 1)
                End If
                i += 1
            End While
            Return local3(0) & "," & local3(1) & "," & local2(2)
        End Function

        Public Function Decrypt(str As String) As String
            Dim N_Txt = ""
            Dim i = 0
            While i < str.Length
                Dim VC_I = i
                If VC_I > 9 Then
                    VC_I = 0
                End If
                Dim t_Str = str.Substring(i, 1)
                Dim Ey_Index = Ey_str.IndexOf(t_Str)
                If Ey_Index < 0 Then
                    N_Txt &= t_Str
                Else
                    N_Txt &= Em_str(VC_I).Substring(Ey_Index, 1)
                End If
                i += 1
            End While
            Return N_Txt
        End Function
    End Module
End Namespace
Imports System

Namespace Files.SevenZip
    Friend Class InvalidParamException
        Inherits ApplicationException
        ' Methods
        Public Sub New()
            MyBase.New("Invalid Parameter")
        End Sub

    End Class
End Namespace


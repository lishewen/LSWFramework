Imports System

Namespace Files.SevenZip
    Friend Class DataErrorException
        Inherits ApplicationException
        ' Methods
        Public Sub New()
            MyBase.New("Data Error")
        End Sub

    End Class
End Namespace


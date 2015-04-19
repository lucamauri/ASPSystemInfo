Public Class ValuesPair
    Private _Label As String
    Private _Content As String

    Public Property Label As String
        Get
            Return _Label
        End Get
        Set(ByVal value As String)
            _Label = value
        End Set
    End Property

    Public Property Content As String
        Get
            Return _Content
        End Get
        Set(ByVal value As String)
            _Content = value
        End Set
    End Property

    Sub New(PassLabel As String, PassContent As String)
        _Label = PassLabel
        _Content = PassContent
    End Sub
End Class

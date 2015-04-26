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
Public Class ProcessInfo
    Private _Infos As List(Of ValuesPair)

    Public Property Info() As List(Of ValuesPair)
        Get
            Return _Infos
        End Get
        Set(ByVal value As List(Of ValuesPair))
            _Infos = value
        End Set
    End Property

    Public Sub New()
        _Infos = New List(Of ValuesPair)
    End Sub
End Class
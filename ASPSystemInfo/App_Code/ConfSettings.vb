Public Class ConfSettings
    Inherits ConfigurationSection

    <ConfigurationProperty("StyleSheet", defaultvalue:="default.css", isrequired:=False)> _
        Public Property StyelSheet() As String
        Get
            Return CType(Me("StyleSheet"), String)
        End Get
        Set(ByVal value As String)
            Me("StyleSheet") = value
        End Set
    End Property

    <ConfigurationProperty("ShowVoltage", DefaultValue:="true", IsRequired:=False)> _
    Public Property RemoteOnly() As Boolean
        Get
            Return CType(Me("ShowVoltage"), Boolean)
        End Get
        Set(ByVal value As Boolean)
            Me("ShowVoltage") = value
        End Set
    End Property


End Class

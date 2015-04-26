Imports System.Management

Public Class WMIQuery
    'Function ProcessesDetails() As List(Of ValuesPair)
    '    Dim Info As ValuesPair
    '    Dim AllInfo As New List(Of ValuesPair)
    '    Dim ProcessesSearcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM CIM_Process")
    '    Dim ProcessesCollection As ManagementObjectCollection = ProcessesSearcher.Get

    '    For Each Process As ManagementObject In ProcessesCollection
    '        For Each ProcessProperty As PropertyData In Process.Properties
    '            Select Case ProcessProperty.Name
    '                Case "Caption", "Description", "ProcessId", "ThreadCount"
    '                    Info = New ValuesPair(ProcessProperty.Name, ProcessProperty.Value.ToString())
    '                    AllInfo.Add(Info)
    '            End Select
    '        Next
    '    Next
    '    Return AllInfo
    'End Function
End Class

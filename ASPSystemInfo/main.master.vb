Imports System.Management
Imports System.Net
Imports System.IO

Public Class main
    Inherits System.Web.UI.MasterPage

    Public CurrentCSS As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim BasicVitals As List(Of ValuesPair)
        Dim CurrVital As ValuesPair

        Dim AllDrives() As DriveInfo
        Dim ReadyDrives() As DriveInfo

        Dim CPUCounter As PerformanceCounter

        Dim GenericQuery As New WMIQuery

        Call FillCSS()

        If Not IsPostBack Then
            BasicVitals = New List(Of ValuesPair)

            CurrVital = New ValuesPair("Operating System:", Environment.OSVersion.VersionString)
            BasicVitals.Add(CurrVital)

            If Environment.Is64BitOperatingSystem Then
                CurrVital = New ValuesPair("Architecture:", "64-bit")
            Else
                CurrVital = New ValuesPair("Architecture:", "32-bit")
            End If
            BasicVitals.Add(CurrVital)

            CurrVital = New ValuesPair("Hostname:", Dns.GetHostName().ToString)
            BasicVitals.Add(CurrVital)

            CurrVital = New ValuesPair("IP Address:", Request.ServerVariables("LOCAL_ADDR"))
            BasicVitals.Add(CurrVital)

            CurrVital = New ValuesPair("Software:", Request.ServerVariables("SERVER_SOFTWARE"))
            BasicVitals.Add(CurrVital)

            CurrVital = New ValuesPair("Uptime:", GetUptime.ToString)
            BasicVitals.Add(CurrVital)

            CurrVital = New ValuesPair("Total processes", GetProcessesCount.ToString)
            BasicVitals.Add(CurrVital)

            CurrVital = New ValuesPair("Total threads", GetThreadsCount.ToString)
            BasicVitals.Add(CurrVital)

            CPUCounter = New PerformanceCounter

            With CPUCounter
                .CategoryName = "Processor"
                .CounterName = "% Processor Time"
                .InstanceName = "_Total"
            End With
            CPUCounter.NextValue()
            System.Threading.Thread.Sleep(500)
            CurrVital = New ValuesPair("Processor Time", CPUCounter.NextValue() & "%")
            BasicVitals.Add(CurrVital)


            CurrVital = New ValuesPair("ASP System Information version", GetType(main).Assembly.GetName.Version.ToString)
            BasicVitals.Add(CurrVital)

            CurrVital = New ValuesPair("Environment version:", Environment.Version.ToString)
            BasicVitals.Add(CurrVital)


            RPTBasic.DataSource = BasicVitals
            RPTBasic.DataBind()

            RPTNetwork.DataSource = Dns.GetHostAddresses(Dns.GetHostName())
            RPTNetwork.DataBind()


            With RPTProcessor
                .DataSource = DeviceInformation("Win32_Processor")
                .DataBind()
            End With

            AllDrives = DriveInfo.GetDrives

            For Each Drive As DriveInfo In AllDrives
                If Drive.IsReady Then
                    If ReadyDrives Is Nothing Then
                        ReDim ReadyDrives(0)
                    Else
                        ReDim Preserve ReadyDrives(ReadyDrives.GetUpperBound(0) + 1)
                    End If
                    'ReDim Preserve ReadyDrives(ReadyDrives.GetUpperBound(0) + 1)
                    ReadyDrives(ReadyDrives.GetUpperBound(0)) = Drive
                End If
            Next
            RPTDrives.DataSource = ReadyDrives
            RPTDrives.DataBind()

            With RPTProcess
                '.DataSource = DeviceInformation("Win32_Process")
                '.DataSource = GenericQuery.ProcessesDetails
                .DataSource = ProcessesDetails()
                .DataBind()
            End With

            With RPTMemory
                .DataSource = WMIMemory()
                .DataBind()
            End With

            With RPTNetDetails
                .DataSource = NetworkDetails()
                .DataBind()
            End With

            With RPTVoltage
                .DataSource = DeviceInformation("Win32_VoltageProbe")
                .DataBind()
            End With

            With RPTOS
                .DataSource = DeviceInformation("Win32_OperatingSystem")
                .DataBind()
            End With

            'Dim WMICrawler As ManagementObjectSearcher
            'Dim Row As New System.Web.UI.WebControls.TableRow

            'WMICrawler = New ManagementObjectSearcher("Select  *  from  Win32_Processor")

            'For Each Service As ManagementObject In WMICrawler.Get
            '    Row.Cells(0).Text = Service("Caption")
            '    tblMain.Rows.Add(Row)
            'Next
        End If
    End Sub
    Public Function GetUptime() As TimeSpan
        Dim mo As New ManagementObject("\\.\root\cimv2:Win32_OperatingSystem=@")
        Dim lastBootUp As DateTime = ManagementDateTimeConverter.ToDateTime(mo("LastBootUpTime").ToString())
        Return DateTime.Now.ToUniversalTime() - lastBootUp.ToUniversalTime()
    End Function
    Public Function GetProcessesCount() As Integer
        Dim searchCIMV2 As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM CIM_Process")
        Return searchCIMV2.Get.Count
    End Function
    Public Function GetThreadsCount() As Integer
        Dim searchCIMV2 As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM CIM_Thread")
        Return searchCIMV2.Get.Count
    End Function
    Private Function DeviceInformation(QueryString As String) As List(Of ValuesPair)
        Dim AllInfo As New List(Of ValuesPair)
        Dim Info As ValuesPair
        Dim WMIManagement As New ManagementClass(QueryString)
        'Create a ManagementObjectCollection to loop through
        Dim WMIManagementObjCol As ManagementObjectCollection = WMIManagement.GetInstances()
        'Get the properties in the class
        Dim Properties As PropertyDataCollection = WMIManagement.Properties
        For Each MgmtObj As ManagementObject In WMIManagementObjCol
            For Each WMIProperty As PropertyData In Properties
                Try
                    Info = New ValuesPair(WMIProperty.Name, MgmtObj.Properties(WMIProperty.Name).Value.ToString())
                    AllInfo.Add(Info)
                Catch
                    'Info = New ValuesPair([property].Name, "[error getting value]")
                End Try
                'AllInfo.Add(Info)
            Next

        Next
        Return AllInfo
    End Function
    Sub FillCSS()
        If Not IsPostBack Then
            For Each CurrFile As FileInfo In New DirectoryInfo(Hosting.HostingEnvironment.MapPath("\styles")).EnumerateFiles
                If CurrFile.Extension.ToLower = ".css" Then
                    DropCSS.Items.Add(CurrFile.Name)
                End If
            Next
            'CurrentCSS = "'../styles/default.css'"
            Call SetCSS("default.css")
        End If
    End Sub

    Private Sub DropCSS_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropCSS.SelectedIndexChanged
        Call SetCSS(DropCSS.SelectedValue)
    End Sub



    Private Sub SetCSS(HRef As String)
        'Dim CSSLink As HtmlLink

        'Try
        '    'CSSLink.FindControl("cssmain")
        '    Page.Header.Controls.Remove(Page.Header.FindControl("cssmain"))
        'Catch ex As Exception

        'End Try

        'CSSLink = New HtmlLink()

        'With CSSLink
        '    .Href = HRef
        '    .Attributes.Add("id", "cssmain")
        '    .Attributes.Add("rel", "stylesheet")
        '    .Attributes.Add("type", "text/css")
        'End With
        'Page.Header.Controls.Add(CSSLink)
        cssmain.Href = "../styles/" & HRef
        DropCSS.ClearSelection()
        DropCSS.Items.FindByValue(HRef).Selected = True
    End Sub
    Function ProcessesDetails() As List(Of ProcessInfo)
        'TODO delete this function and use the one from _WMIQuery_ class
        Dim Info As ValuesPair
        'Dim AllInfo As New List(Of ValuesPair)
        Dim AllProcesses As New List(Of ProcessInfo)
        Dim ProcessesSearcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM CIM_Process")
        Dim ProcessesCollection As ManagementObjectCollection = ProcessesSearcher.Get

        For Each Process As ManagementObject In ProcessesCollection
            Dim CurrProcess = New ProcessInfo
            For Each ProcessProperty As PropertyData In Process.Properties
                Select Case ProcessProperty.Name
                    Case "Caption", "ParentProcessId", "ProcessId", "ThreadCount"
                        Info = New ValuesPair(ProcessProperty.Name, ProcessProperty.Value.ToString())
                        CurrProcess.Info.Add(Info)
                End Select
            Next
            AllProcesses.Add(CurrProcess)
        Next
        Return AllProcesses
    End Function
    Function WMIDetails(Query As String, FieldsNames As String()) As List(Of ValuesPair)

        Dim AllInfo As New List(Of ValuesPair)
        Dim Info As ValuesPair
        Dim WMIManagement As New ManagementClass(Query)
        'Create a ManagementObjectCollection to loop through
        Dim WMIManagementObjCol As ManagementObjectCollection = WMIManagement.GetInstances()
        'Get the properties in the class
        Dim Properties As PropertyDataCollection = WMIManagement.Properties
        For Each MgmtObj As ManagementObject In WMIManagementObjCol
            For Each WMIProperty As PropertyData In Properties
                Select Case WMIProperty.Name
                    Case FieldsNames(0), FieldsNames(1), FieldsNames(2), FieldsNames(3), FieldsNames(4), FieldsNames(5)
                        Try
                            Info = New ValuesPair(WMIProperty.Name, (CType(MgmtObj.Properties(WMIProperty.Name).Value, Integer) / 1024).ToString)
                        Catch
                            Info = New ValuesPair(WMIProperty.Name, "[error getting value]")
                        End Try
                        AllInfo.Add(Info)
                End Select
            Next

        Next
        Return AllInfo
    End Function
    Function WMIMemory() As List(Of ValuesPair)
        Dim MemoryQueryStrings As String() = {"TotalVisibleMemorySize", "FreePhysicalMemory", "TotalVirtualMemorySize", "FreeVirtualMemory"}
        Dim AllInfo As New List(Of ValuesPair)
        Dim Info As ValuesPair
        Dim WMIManagement As New ManagementClass("Win32_OperatingSystem")
        'Create a ManagementObjectCollection to loop through
        Dim WMIManagementObjCol As ManagementObjectCollection = WMIManagement.GetInstances()
        'Get the properties in the class
        Dim Properties As PropertyDataCollection = WMIManagement.Properties
        For Each MgmtObj As ManagementObject In WMIManagementObjCol
            For Each Query As String In MemoryQueryStrings
                Info = New ValuesPair(Query, String.Format("{0:n}", CType(MgmtObj(Query).ToString, Single) / 1024))
                AllInfo.Add(Info)
            Next
        Next
        Return AllInfo
    End Function
    Function NetworkDetails() As List(Of GenericInfo)
        Dim Info As ValuesPair
        Dim NetworkQueryStrings As String() = {"Name", "BytesReceivedPersec", "BytesSentPersec", "BytesTotalPersec", "CurrentBandwidth"}
        Dim AllNetwork As New List(Of GenericInfo)
        Dim NetworkSearcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_PerfRawData_Tcpip_NetworkInterface")
        Dim NetworkCollection As ManagementObjectCollection = NetworkSearcher.Get

        For Each Network As ManagementObject In NetworkCollection
            Dim CurrNetwork = New GenericInfo
            For Each NetworkProperty As PropertyData In Network.Properties

                'For Each Query As String In NetworkQueryStrings
                '    Info = New ValuesPair(NetworkProperty.Name, NetworkProperty.Value.ToString())
                '    CurrNetwork.Info.Add(Info)
                'Next

                Select Case NetworkProperty.Name
                    Case "Name", "BytesReceivedPersec", "BytesSentPersec", "BytesTotalPersec", "CurrentBandwidth"
                        Try
                            Info = New ValuesPair(NetworkProperty.Name, NetworkProperty.Value.ToString())
                        Catch ex As Exception
                            Info = New ValuesPair(NetworkProperty.Name, ex.ToString)
                        End Try

                        CurrNetwork.Info.Add(Info)
                End Select
            Next
            AllNetwork.Add(CurrNetwork)
        Next
        Return AllNetwork
    End Function
End Class
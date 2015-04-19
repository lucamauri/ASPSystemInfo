Imports System.Management
Imports System.Net
Imports System.IO

Public Class main
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim BasicVitals As List(Of ValuesPair)
        Dim CurrVital As ValuesPair

        Dim AllDrives() As DriveInfo
        Dim ReadyDrives() As DriveInfo

        BasicVitals = New List(Of ValuesPair)

        CurrVital = New ValuesPair("Operation System:", Environment.OSVersion.VersionString)
        BasicVitals.Add(CurrVital)

        CurrVital = New ValuesPair("Hostname:", Dns.GetHostName().ToString)
        BasicVitals.Add(CurrVital)

        'CurrVital = New ValuesPair("IP Address:", Dns.GetHostAddresses(Dns.GetHostName())(0).ToString)
        'BasicVitals.Add(CurrVital)

        CurrVital = New ValuesPair("IP Address:", Request.ServerVariables("LOCAL_ADDR"))
        BasicVitals.Add(CurrVital)

        CurrVital = New ValuesPair("Software:", Request.ServerVariables("SERVER_SOFTWARE"))
        BasicVitals.Add(CurrVital)

        CurrVital = New ValuesPair("Uptime:", GetUptime.ToString)
        BasicVitals.Add(CurrVital)

        CurrVital = New ValuesPair("Total processes", GetProcesses.ToString)
        BasicVitals.Add(CurrVital)

        CurrVital = New ValuesPair("Total threads", GetThreads.ToString)
        BasicVitals.Add(CurrVital)

        CurrVital = New ValuesPair("ASP System Information version", GetType(main).Assembly.GetName.Version.ToString)
        BasicVitals.Add(CurrVital)

        CurrVital = New ValuesPair("SystemPageSize(KB):", Environment.SystemPageSize / 1024)
        BasicVitals.Add(CurrVital)

        CurrVital = New ValuesPair("Version:", Environment.Version.ToString)
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
                ReadyDrives(ReadyDrives.GetUpperBound(0)) = Drive
            End If
        Next
        rptDrives.DataSource = readyDrives
        rptDrives.DataBind()

        With RPTVoltage
            .DataSource = DeviceInformation("Win32_VoltageProbe")
            .DataBind()
        End With

        'Dim WMICrawler As ManagementObjectSearcher
        'Dim Row As New System.Web.UI.WebControls.TableRow

        'WMICrawler = New ManagementObjectSearcher("Select  *  from  Win32_Processor")

        'For Each Service As ManagementObject In WMICrawler.Get
        '    Row.Cells(0).Text = Service("Caption")
        '    tblMain.Rows.Add(Row)
        'Next

    End Sub
    Public Function GetUptime() As TimeSpan
        Dim mo As New ManagementObject("\\.\root\cimv2:Win32_OperatingSystem=@")
        Dim lastBootUp As DateTime = ManagementDateTimeConverter.ToDateTime(mo("LastBootUpTime").ToString())
        Return DateTime.Now.ToUniversalTime() - lastBootUp.ToUniversalTime()
    End Function
    Public Function GetProcesses() As Integer
        Dim searchCIMV2 As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM CIM_Process")
        Return searchCIMV2.Get.Count
    End Function
    Public Function GetThreads() As Integer
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

End Class
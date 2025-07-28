Imports System.ComponentModel
Imports System.IO

Public Class SysCollectoPro
    Private ReadOnly BaseDir As String = ReadIni(Path.Combine(Application.StartupPath, "Configs", "SysCollectorPro.config.ini"), "Output", "OutputPath", "C:\SysCollectoPro")
    Private ReadOnly OutputFileName As String = ReadIni(Path.Combine(Application.StartupPath, "Configs", "SysCollectorPro.config.ini"), "Output", "FileName", "C:\SysCollectoPro")
    Private ReadOnly OutputFormat As String = ReadIni(Path.Combine(Application.StartupPath, "Configs", "SysCollectorPro.config.ini"), "Output", "FileFormat", "C:\SysCollectoPro")

    Private progressForm As ProgressForm

    Private Sub BtnExecute_Click(sender As Object, e As EventArgs) Handles BtnExecute.Click
        If Not ValidateInput() Then Exit Sub

        BtnExecute.Enabled = False
        TxtSite.Enabled = False
        TxtZone.Enabled = False
        TxtPosition.Enabled = False

        progressForm = New ProgressForm()
        progressForm.Show(Me)
        progressForm.Refresh()

        BgWorkerExport.RunWorkerAsync()
    End Sub

    Private Sub BgWorkerExport_DoWork(sender As Object, e As DoWorkEventArgs) Handles BgWorkerExport.DoWork
        Try
            Dim outputPath As String = Path.Combine(BaseDir, $"{OutputFileName}.{OutputFormat}")
            Dim audit As SystemAuditModel = CreateSystemAuditModel()
            CsvExportHelper.ExportAudit(audit, outputPath)
        Catch ex As Exception
            e.Result = ex
        End Try
    End Sub

    Private Sub BgWorkerExport_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BgWorkerExport.RunWorkerCompleted
        BtnExecute.Enabled = True

        If progressForm IsNot Nothing Then
            progressForm.Close()
            progressForm.Dispose()
            progressForm = Nothing
        End If

        If e.Result IsNot Nothing AndAlso TypeOf e.Result Is Exception Then
            Dim ex As Exception = DirectCast(e.Result, Exception)
            MessageBox.Show("Error writing system info:" & Environment.NewLine & ex.Message,
                        "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim outputPath As String = Path.Combine(BaseDir, $"{OutputFileName}.{OutputFormat}")
        MessageBox.Show($"System audit completed successfully." & Environment.NewLine &
                    $"Output saved to: {outputPath}",
                    "Audit Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Me.Close()
    End Sub


    Private Function CreateSystemAuditModel() As SystemAuditModel
        Dim cpuList = HardwareInfoCollector.GetCpuInfoList()
        Dim ramList = HardwareInfoCollector.GetRamInfoList()
        Dim storageList = HardwareInfoCollector.GetStorageInfo()
        Dim gpuList = HardwareInfoCollector.GetGpuInfoList()
        Dim netAdapters = NetworkInfoCollector.GetNetworkInfoList()
        Dim motherboard = HardwareInfoCollector.GetMotherboardInfo()
        Dim os = SystemInfoCollector.GetOSInfo()
        Dim bios = SystemInfoCollector.GetBIOSInfo()

        Return New SystemAuditModel With {
        .Hostname = Environment.MachineName,
        .ChassisType = HardwareInfoCollector.GetChassisType(),
        .SerialNumber = motherboard.SerialNumber,
        .Manufacturer = motherboard.Manufacturer,
        .Model = motherboard.Product,
        .BIOSVersion = bios.BIOSVersion,
        .BIOSDate = bios.BIOSDate,
        .Domain = SystemInfoCollector.GetDomainName(),
        .LoggedInUser = SystemInfoCollector.GetLoggedInUser(),
        .AdminUser = UserSessionCollector.IsCurrentUserAdmin(),
        .CPUs = cpuList,
        .TotalCPUCores = cpuList.Sum(Function(r) r.CoreCount),
        .RAMModules = ramList,
        .RAMTotalGB = CInt(Math.Floor(ramList.Sum(Function(r) r.CapacityMB) / 1024.0)),
        .StorageDevices = storageList,
        .StorageTotalGB = storageList.Sum(Function(r) r.SizeGB),
        .TotalStorageCount = storageList.Count,
        .GPUs = gpuList,
        .TotalGpuCount = gpuList.Count,
        .OSName = $"{os.Name} ({os.Version})",
        .OSVersion = $"{SystemInfoCollector.GetOSDisplayVersion()} (Build {SystemInfoCollector.GetOSFullBuildVersion()})",
        .InstallDate = os.InstallDate,
        .Architecture = os.Architecture,
        .LastBootTime = SystemInfoCollector.GetLastBootTime(),
        .WindowsActivated = SystemInfoCollector.IsWindowsActivated(),
        .NetworkAdapters = netAdapters,
        .SecureBoot = SecurityInfoCollector.GetSecureBootStatus(),
        .TPMVersion = SecurityInfoCollector.GetTPMStatus(),
        .BitLockerStatus = SecurityInfoCollector.GetBitLockerStatus(),
        .AntivirusStatus = SecurityInfoCollector.GetDefenderStatus(),
        .Site = TxtSite.Text.Trim(),
        .Zone = TxtZone.Text.Trim(),
        .Position = TxtPosition.Text.Trim(),
        .Notes = "",
        .AuditDate = Date.Now
    }
    End Function

    Private Function ValidateInput() As Boolean
        If String.IsNullOrWhiteSpace(TxtSite.Text) Then
            MessageBox.Show("Please enter a site name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtSite.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(TxtZone.Text) Then
            MessageBox.Show("Please enter a zone.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtPosition.Focus()
            Return False
        End If

        If String.IsNullOrEmpty(TxtPosition.Text) Then
            MessageBox.Show("Please enter a position.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtPosition.Focus()
            Return False
        End If

        Return True
    End Function
End Class

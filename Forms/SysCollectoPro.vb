Imports System.IO

Public Class SysCollectoPro
    Private ReadOnly BaseDir As String = ReadIni(Path.Combine(Application.StartupPath, "Configs", "SysCollectorPro.config.ini"), "Output", "OutputPath", "C:\SysCollectoPro")
    Private ReadOnly OutputFileName As String = ReadIni(Path.Combine(Application.StartupPath, "Configs", "SysCollectorPro.config.ini"), "Output", "FileName", "C:\SysCollectoPro")
    Private ReadOnly OutputFormat As String = ReadIni(Path.Combine(Application.StartupPath, "Configs", "SysCollectorPro.config.ini"), "Output", "FileFormat", "C:\SysCollectoPro")

    Private Sub BtnExecute_Click(sender As Object, e As EventArgs) Handles BtnExecute.Click
        If Not ValidateInput() Then Exit Sub

        Dim outputPath As String = Path.Combine(BaseDir, $"{OutputFileName}.{OutputFormat}")

        Dim cpuList = HardwareInfoCollector.GetCpuInfoList()
        Dim ramList = HardwareInfoCollector.GetRamInfoList()
        Dim storageList = HardwareInfoCollector.GetStorageInfo()
        Dim gpuList = HardwareInfoCollector.GetGpuInfoList()
        Dim netAdapters = NetworkInfoCollector.GetNetworkInfoList()
        Dim motherboard = HardwareInfoCollector.GetMotherboardInfo()
        Dim os = SystemInfoCollector.GetOSInfo()
        Dim bios = SystemInfoCollector.GetBIOSInfo()

        Dim audit As New SystemAuditModel With {
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

        CsvExportHelper.ExportAudit(audit, outputPath)
    End Sub

    Private Function ValidateInput() As Boolean
        Return True
    End Function
End Class

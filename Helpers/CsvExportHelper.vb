Imports System.IO
Imports System.Text

Public Class CsvExportHelper

    Public Shared Sub ExportAudit(audit As SystemAuditModel, outputPath As String)
        Dim headers As String() = {
            "Hostname", "ChassisType", "SerialNumber", "Manufacturer", "Model", "BIOSVersion", "BIOSDate",
            "CPUs", "TotalCPUCores",
            "RAMModules", "RAMTotalGB",
            "StorageDevices", "StorageTotalGB", "TotalStorageCount",
            "GPUs", "TotalGpuCount",
            "OSName", "OSVersion", "InstallDate", "Architecture", "SecureBoot", "TPMVersion",
            "BitLockerStatus", "Domain", "LoggedInUser", "LastBootTime",
            "NetworkAdapters",
            "AdminUser", "WindowsActivated", "AntivirusStatus",
            "Site", "Zone", "Position", "Notes", "AuditDate"
        }

        Dim values As String() = {
        Quote(audit.Hostname), Quote(audit.ChassisType), Quote(audit.SerialNumber), Quote(audit.Manufacturer), Quote(audit.Model), Quote(audit.BIOSVersion), Quote(audit.BIOSDate),
        Quote(FlattenCpuInfo(audit.CPUs)), audit.TotalCPUCores.ToString(),
        Quote(FlattenRamInfo(audit.RAMModules)), audit.RAMTotalGB.ToString(),
        Quote(FlattenStorageInfo(audit.StorageDevices)), audit.StorageTotalGB.ToString(), audit.TotalStorageCount.ToString(),
        Quote(FlattenGpuInfo(audit.GPUs)), audit.TotalGpuCount.ToString(),
        Quote(audit.OSName), Quote(audit.OSVersion), Quote(audit.InstallDate), Quote(audit.Architecture), Quote(audit.SecureBoot), Quote(audit.TPMVersion),
        Quote(audit.BitLockerStatus), Quote(audit.Domain), Quote(audit.LoggedInUser), Quote(audit.LastBootTime),
        Quote(FlattenNetworkInfo(audit.NetworkAdapters)),
        audit.AdminUser.ToString(), audit.WindowsActivated.ToString(), Quote(audit.AntivirusStatus),
        Quote(audit.Site), Quote(audit.Zone), Quote(audit.Position), Quote(audit.Notes), Quote(audit.AuditDate.ToString("yyyy-MM-dd HH:mm:ss"))
    }

        Try
            Dim fileExists As Boolean = File.Exists(outputPath)

            ' Ensure directory exists
            Dim dir = Path.GetDirectoryName(outputPath)
            If Not String.IsNullOrWhiteSpace(dir) AndAlso Not Directory.Exists(dir) Then
                Directory.CreateDirectory(dir)
            End If

            Using sw As New StreamWriter(outputPath, append:=True, encoding:=Encoding.UTF8)
                If Not fileExists Then
                    sw.WriteLine(String.Join(",", headers))
                End If

                sw.WriteLine(String.Join(",", values))
            End Using
        Catch ex As Exception
            ' Optional: handle error or log
            MessageBox.Show("Export failed: " & ex.Message, "ExportAudit", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Shared Function Quote(value As String) As String
        If value Is Nothing Then value = ""
        If value.Contains("""") Then value = value.Replace("""", """""")
        If value.Contains(",") OrElse value.Contains("""") OrElse value.Contains(ControlChars.NewLine) Then
            Return $"""{value}"""
        Else
            Return value
        End If
    End Function


    Public Shared Function FlattenCpuInfo(cpuList As List(Of CpuInfo)) As String
        If cpuList Is Nothing OrElse cpuList.Count = 0 Then Return ""
        Dim sb As New StringBuilder()
        For Each cpu In cpuList
            sb.Append($"{cpu.Name} ({cpu.CoreCount}C/{cpu.LogicalProcessorCount}T, {cpu.Architecture}); ")
        Next
        Return sb.ToString().TrimEnd(";"c, " "c)
    End Function

    Public Shared Function FlattenGpuInfo(gpuList As List(Of GpuInfo)) As String
        If gpuList Is Nothing OrElse gpuList.Count = 0 Then Return ""
        Dim sb As New StringBuilder()
        For Each gpu In gpuList
            sb.Append($"{gpu.Name} (Driver: {gpu.DriverVersion}, RAM: {gpu.AdapterRAMMB}MB); ")
        Next
        Return sb.ToString().TrimEnd(";"c, " "c)
    End Function

    Public Shared Function FlattenRamInfo(ramList As List(Of RamInfo)) As String
        If ramList Is Nothing OrElse ramList.Count = 0 Then Return ""
        Dim sb As New StringBuilder()
        For Each ram In ramList
            sb.Append($"{ram.Manufacturer} {ram.PartNumber} {ram.CapacityMB}MB {ram.SpeedMHz}MHz {ram.MemoryType} {ram.FormFactor}; ")
        Next
        Return sb.ToString().TrimEnd(";"c, " "c)
    End Function

    Public Shared Function FlattenStorageInfo(devices As List(Of StorageInfo)) As String
        If devices Is Nothing OrElse devices.Count = 0 Then Return ""
        Dim sb As New StringBuilder()
        For Each d In devices
            sb.Append($"{d.Model} {d.SizeGB}GB {d.InterfaceType} {d.MediaType}; ")
        Next
        Return sb.ToString().TrimEnd(";"c, " "c)
    End Function

    Public Shared Function FlattenNetworkInfo(adapters As List(Of NetworkInfo)) As String
        If adapters Is Nothing OrElse adapters.Count = 0 Then Return ""

        Dim sb As New StringBuilder()
        For Each nic In adapters
            Dim ips = If(nic.IPAddresses IsNot Nothing, String.Join("|", nic.IPAddresses), "")
            sb.Append($"{nic.AdapterName} ({nic.Description}) - MAC: {nic.MACAddress}, IPs: {ips}, Speed: {nic.SpeedMbps}Mbps, DHCP: {nic.IsDHCPEnabled}, Status: {nic.ConnectionStatus}, Type: {nic.InterfaceType}, Physical: {nic.IsPhysical}; ")
        Next
        Return sb.ToString().TrimEnd(";"c, " "c)
    End Function

End Class

Imports System.Management

Public Class HardwareInfoCollector
    Public Shared Function GetCpuInfoList() As List(Of CpuInfo)
        Dim cpuList As New List(Of CpuInfo)
        Try
            Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_Processor")
            For Each obj As ManagementObject In searcher.Get()
                Dim info As New CpuInfo With {
                .Name = obj("Name")?.ToString(),
                .CoreCount = Convert.ToInt32(obj("NumberOfCores")),
                .LogicalProcessorCount = Convert.ToInt32(obj("NumberOfLogicalProcessors")),
                .Architecture = CpuArchitectureToString(Convert.ToInt32(obj("Architecture")))
            }
                cpuList.Add(info)
            Next
        Catch ex As Exception
            cpuList.Add(New CpuInfo With {.Name = "[Error] " & ex.Message})
        End Try
        Return cpuList
    End Function

    Private Shared Function CpuArchitectureToString(arch As Integer) As String
        Select Case arch
            Case 0 : Return "x86"
            Case 1 : Return "MIPS"
            Case 2 : Return "Alpha"
            Case 3 : Return "PowerPC"
            Case 5 : Return "ARM"
            Case 6 : Return "ia64"
            Case 9 : Return "x64"
            Case Else : Return "Unknown"
        End Select
    End Function

    Public Shared Function GetGpuInfoList() As List(Of GpuInfo)
        Dim gpuList As New List(Of GpuInfo)
        Try
            Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_VideoController")
            For Each obj As ManagementObject In searcher.Get()
                Dim ramBytes As ULong = 0
                If obj("AdapterRAM") IsNot Nothing Then
                    ramBytes = Convert.ToUInt64(obj("AdapterRAM"))
                End If

                Dim gpu As New GpuInfo With {
                .Name = obj("Name")?.ToString(),
                .DriverVersion = obj("DriverVersion")?.ToString(),
                .AdapterRAMMB = CInt(Math.Round(ramBytes / 1024 / 1024))
            }
                gpuList.Add(gpu)
            Next
        Catch ex As Exception
            gpuList.Add(New GpuInfo With {.Name = "[Error] " & ex.Message})
        End Try
        Return gpuList
    End Function

    Private Shared Function GetMemoryTypeString(obj As ManagementObject) As String
        Dim memoryTypeCode As UShort = 0
        Dim smbiosTypeCode As UShort = 0

        Try
            ' Primary: Try MemoryType
            If obj("MemoryType") IsNot Nothing AndAlso UShort.TryParse(obj("MemoryType").ToString(), memoryTypeCode) Then
                If memoryTypeCode >= 20 Then
                    Return DecodeMemoryType(memoryTypeCode)
                End If
            End If

            ' Fallback: Try SMBIOSMemoryType
            If obj("SMBIOSMemoryType") IsNot Nothing AndAlso UShort.TryParse(obj("SMBIOSMemoryType").ToString(), smbiosTypeCode) Then
                If smbiosTypeCode >= 20 Then
                    Return DecodeMemoryType(smbiosTypeCode)
                End If
            End If
        Catch ex As Exception
            Return "[Error] " & ex.Message
        End Try

        Return "Unknown"
    End Function

    Private Shared Function DecodeMemoryType(code As UShort) As String
        Select Case code
            Case 20 : Return "DDR"
            Case 21 : Return "DDR2"
            Case 24 : Return "DDR3"
            Case 26 : Return "DDR4"
            Case 30 : Return "DDR5"
            Case Else : Return $"Unknown({code})"
        End Select
    End Function

    Private Shared Function GetFormFactorString(formFactorCode As UShort) As String
        Select Case formFactorCode
            Case 8 : Return "DIMM"
            Case 12 : Return "SODIMM"
            Case Else : Return "Other"
        End Select
    End Function

    Public Shared Function GetRamInfoList() As List(Of RamInfo)
        Dim ramList As New List(Of RamInfo)
        Try
            Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory")
            For Each obj As ManagementObject In searcher.Get()
                Dim ram As New RamInfo With {
                .Manufacturer = obj("Manufacturer")?.ToString()?.Trim(),
                .PartNumber = obj("PartNumber")?.ToString()?.Trim(),
                .CapacityMB = Convert.ToInt32(Convert.ToUInt64(obj("Capacity")) / 1024 / 1024),
                .SpeedMHz = If(obj("Speed") IsNot Nothing, Convert.ToInt32(obj("Speed")), 0),
                .BankLabel = obj("BankLabel")?.ToString(),
                .SerialNumber = obj("SerialNumber")?.ToString(),
                .MemoryType = GetMemoryTypeString(obj),
                .FormFactor = GetFormFactorString(Convert.ToUInt16(obj("FormFactor")))
            }
                ramList.Add(ram)
            Next
        Catch ex As Exception
            ramList.Add(New RamInfo With {.Manufacturer = "[Error] " & ex.Message})
        End Try
        Return ramList
    End Function

    Public Shared Function GetStorageInfo() As List(Of StorageInfo)
        Dim result As New List(Of StorageInfo)()

        Try
            Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive")
            For Each drive As ManagementObject In searcher.Get()
                Dim model As String = TryCast(drive("Model"), String)
                Dim sizeBytes As ULong = 0UL
                Dim interfaceType As String = TryCast(drive("InterfaceType"), String)
                Dim mediaType As String = "Unknown"

                If drive("Size") IsNot Nothing Then
                    ULong.TryParse(drive("Size").ToString(), sizeBytes)
                End If
                Dim sizeGB As Integer = CInt(sizeBytes \ (1024UL ^ 3))

                ' --- SAFELY detect RotationRate ---
                Dim hasRotationRateProp = drive.Properties.Cast(Of PropertyData).Any(Function(p) p.Name = "RotationRate")
                Dim rotationRate As UInteger
                Dim rotationKnown As Boolean = False

                If hasRotationRateProp AndAlso drive("RotationRate") IsNot Nothing Then
                    If UInteger.TryParse(drive("RotationRate").ToString(), rotationRate) Then
                        rotationKnown = True
                        If rotationRate = 0 Then
                            mediaType = "SSD"
                        Else
                            mediaType = "HDD"
                        End If
                    End If
                End If

                If Not rotationKnown Then
                    If model IsNot Nothing Then
                        Dim modelLower As String = model.ToLowerInvariant()

                        ' SSD/NVMe patterns
                        If modelLower.Contains("ssd") OrElse modelLower.Contains("nvme") Then
                            mediaType = "SSD"

                            ' Common HDD vendor hints
                        ElseIf modelLower.Contains("hgst") OrElse modelLower.Contains("hitachi") OrElse
               modelLower.Contains("st") OrElse modelLower.Contains("wd") OrElse
               modelLower.Contains("seagate") OrElse modelLower.Contains("toshiba") Then
                            mediaType = "HDD"

                            ' IDE/ATA fallback for legacy HDDs
                        ElseIf interfaceType IsNot Nothing AndAlso
               (interfaceType.ToLower().Contains("ide") OrElse interfaceType.ToLower().Contains("ata")) Then
                            mediaType = "HDD"
                        End If
                    End If
                End If

                result.Add(New StorageInfo With {
                    .Model = model,
                    .SizeGB = sizeGB,
                    .InterfaceType = interfaceType,
                    .MediaType = mediaType
                })
            Next

        Catch ex As Exception
            result.Add(New StorageInfo With {
                .Model = "[Error] " & ex.Message,
                .SizeGB = 0,
                .InterfaceType = "Unknown",
                .MediaType = "Unknown"
            })
        End Try

        Return result
    End Function

    Public Shared Function GetMotherboardInfo() As MotherboardInfo
        Dim info As New MotherboardInfo()
        Try
            Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard")
            For Each board As ManagementObject In searcher.Get()
                info.Manufacturer = TryCast(board("Manufacturer"), String)
                info.Product = TryCast(board("Product"), String)
                info.SerialNumber = TryCast(board("SerialNumber"), String)
                Exit For
            Next
        Catch
        End Try
        Return info
    End Function

    Public Shared Function GetChassisType() As String
        Try
            Dim chassisArray As UShort() = Nothing
            Dim manufacturer As String = ""
            Dim model As String = ""

            ' Query for chassis type
            Using searcher As New ManagementObjectSearcher("SELECT * FROM Win32_SystemEnclosure")
                For Each enclosure As ManagementObject In searcher.Get()
                    If enclosure("ChassisTypes") IsNot Nothing Then
                        chassisArray = CType(enclosure("ChassisTypes"), UShort())
                        Exit For
                    End If
                Next
            End Using

            ' Query system manufacturer/model (for VM detection)
            Using searcherSys As New ManagementObjectSearcher("SELECT Manufacturer, Model FROM Win32_ComputerSystem")
                For Each obj As ManagementObject In searcherSys.Get()
                    manufacturer = obj("Manufacturer")?.ToString()?.ToLowerInvariant()
                    model = obj("Model")?.ToString()?.ToLowerInvariant()
                Next
            End Using

            ' Detect common virtualization vendors
            If manufacturer.Contains("microsoft corporation") AndAlso model.Contains("virtual") Then
                Return "Virtual (Hyper-V)"
            ElseIf manufacturer.Contains("vmware") Then
                Return "Virtual (VMware)"
            ElseIf manufacturer.Contains("xen") Then
                Return "Virtual (Xen)"
            ElseIf manufacturer.Contains("qemu") OrElse manufacturer.Contains("kvm") Then
                Return "Virtual (KVM)"
            End If

            ' Interpret chassis type
            If chassisArray IsNot Nothing AndAlso chassisArray.Length > 0 Then
                Select Case chassisArray(0)
                    Case 3 : Return "Desktop"
                    Case 4 : Return "Low Profile Desktop"
                    Case 5 : Return "Pizza Box"
                    Case 6 : Return "Mini Tower"
                    Case 7 : Return "Tower"
                    Case 8 : Return "Portable"
                    Case 9 : Return "Laptop"
                    Case 10 : Return "Notebook"
                    Case 11 : Return "Handheld"
                    Case 12 : Return "Docking Station"
                    Case 14 : Return "Sub Notebook"
                    Case 30 : Return "Tablet"
                    Case Else : Return "Other"
                End Select
            End If

            Return "Unknown"
        Catch ex As Exception
            Return "[Error] " & ex.Message
        End Try
    End Function
End Class

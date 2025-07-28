Public Class SystemAuditModel

    ' --- System Identity ---
    Public Property Hostname As String
    Public Property ChassisType As String
    Public Property SerialNumber As String
    Public Property Manufacturer As String
    Public Property Model As String
    Public Property BIOSVersion As String
    Public Property BIOSDate As String
    Public Property Domain As String
    Public Property LoggedInUser As String
    Public Property AdminUser As String

    ' --- Hardware ---
    Public Property CPUs As List(Of CpuInfo)
    Public Property TotalCPUCores As Integer
    Public Property RAMModules As List(Of RamInfo)
    Public Property RAMTotalGB As Integer
    Public Property StorageDevices As List(Of StorageInfo)
    Public Property StorageTotalGB As Integer
    Public Property TotalStorageCount As Integer
    Public Property GPUs As List(Of GpuInfo)
    Public Property TotalGpuCount As Integer

    ' --- Operating System ---
    Public Property OSName As String
    Public Property OSVersion As String
    Public Property InstallDate As String
    Public Property Architecture As String
    Public Property LastBootTime As String
    Public Property WindowsActivated As String

    ' --- Network ---
    Public Property NetworkAdapters As List(Of NetworkInfo)

    ' --- Security ---
    Public Property SecureBoot As String
    Public Property TPMVersion As String
    Public Property BitLockerStatus As String
    Public Property AntivirusStatus As String

    ' --- Location ---
    Public Property Site As String
    Public Property Zone As String
    Public Property Position As String

    ' --- Metadata ---
    Public Property Notes As String
    Public Property AuditDate As Date

End Class

Public Class CpuInfo
    Public Property Name As String
    Public Property CoreCount As Integer
    Public Property LogicalProcessorCount As Integer
    Public Property Architecture As String
End Class


Public Class GpuInfo
    Public Property Name As String
    Public Property DriverVersion As String
    Public Property AdapterRAMMB As Integer
End Class

Public Class RamInfo
    Public Property Manufacturer As String
    Public Property PartNumber As String
    Public Property CapacityMB As Integer
    Public Property SpeedMHz As Integer
    Public Property BankLabel As String
    Public Property SerialNumber As String
    Public Property MemoryType As String
    Public Property FormFactor As String
End Class

Public Class StorageInfo
    Public Property Model As String
    Public Property SizeGB As Integer
    Public Property InterfaceType As String
    Public Property MediaType As String
End Class

Public Class NetworkInfo
    Public Property AdapterName As String
    Public Property Description As String
    Public Property MACAddress As String
    Public Property IPAddresses As List(Of String)
    Public Property SpeedMbps As Long
    Public Property IsDHCPEnabled As Boolean
    Public Property ConnectionStatus As String
    Public Property InterfaceType As String
    Public Property IsPhysical As Boolean
End Class

Public Class MotherboardInfo
    Public Property Manufacturer As String
    Public Property Product As String
    Public Property SerialNumber As String
End Class

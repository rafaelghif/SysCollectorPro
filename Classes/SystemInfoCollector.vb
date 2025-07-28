Imports System.Management
Imports Microsoft.Win32

Public Class SystemInfoCollector
    Public Shared Function GetOSInfo() As (Name As String, Version As String, Architecture As String, InstallDate As String)
        Dim name As String = "", version As String = "", arch As String = "", installDate As String = ""

        Try
            Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")
            For Each os As ManagementObject In searcher.Get()
                name = TryCast(os("Caption"), String)
                version = TryCast(os("Version"), String)
                arch = TryCast(os("OSArchitecture"), String)
                installDate = ManagementDateTimeConverter.ToDateTime(os("InstallDate").ToString()).ToString("yyyy-MM-dd")
                Exit For
            Next
        Catch
        End Try

        Return (name, version, arch, installDate)
    End Function

    Public Shared Function GetBIOSInfo() As (BIOSVersion As String, BIOSDate As String, SerialNumber As String)
        Dim biosVersion As String = ""
        Dim biosDate As String = ""
        Dim serialNumber As String = ""

        Try
            Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_BIOS")
            For Each bios As ManagementObject In searcher.Get()
                biosVersion = TryCast(bios("SMBIOSBIOSVersion"), String)
                serialNumber = TryCast(bios("SerialNumber"), String)

                Dim rawDate = TryCast(bios("ReleaseDate"), String)
                If Not String.IsNullOrEmpty(rawDate) Then
                    biosDate = ManagementDateTimeConverter.ToDateTime(rawDate).ToString("yyyy-MM-dd")
                End If

                Exit For ' Only use the first result
            Next
        Catch ex As Exception
            ' Optional: Log or handle error
        End Try

        Return (biosVersion, biosDate, serialNumber)
    End Function


    Public Shared Function GetWindowsActivationStatus() As String
        Try
            Dim key As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SoftwareProtectionPlatform")
            If key IsNot Nothing Then
                Dim value = key.GetValue("BackupProductKeyDefault")
                Return If(value IsNot Nothing, "Activated", "Not Activated")
            End If
        Catch
        End Try
        Return "Unknown"
    End Function

    Public Shared Function GetDomainName() As String
        Try
            Dim domain As String = Environment.UserDomainName
            Return If(Not String.IsNullOrWhiteSpace(domain), domain, "Unknown")
        Catch ex As Exception
            Return "[Error] " & ex.Message
        End Try
    End Function

    Public Shared Function GetLoggedInUser() As String
        Try
            Return System.Security.Principal.WindowsIdentity.GetCurrent().Name
        Catch ex As Exception
            Return "[Error] " & ex.Message
        End Try
    End Function

    Public Shared Function GetLastBootTime() As String
        Try
            Dim searcher As New ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem")
            For Each obj As ManagementObject In searcher.Get()
                If obj("LastBootUpTime") IsNot Nothing Then
                    Dim raw = obj("LastBootUpTime").ToString()
                    Dim bootTime = ManagementDateTimeConverter.ToDateTime(raw)
                    Return bootTime.ToString("yyyy-MM-dd HH:mm:ss")
                End If
            Next
            Return "Unknown"
        Catch ex As Exception
            Return "[Error] " & ex.Message
        End Try
    End Function

    Public Shared Function IsWindowsActivated() As String
        Try
            Dim searcher As New ManagementObjectSearcher("SELECT LicenseStatus FROM SoftwareLicensingProduct WHERE PartialProductKey IS NOT NULL")
            For Each obj As ManagementObject In searcher.Get()
                Dim status = Convert.ToInt32(obj("LicenseStatus"))
                If status = 1 Then Return "Yes"
            Next
            Return "No"
        Catch ex As Exception
            Return "[Error] " & ex.Message
        End Try
    End Function

    Public Shared Function GetOSBuildNumber() As String
        Try
            Dim searcher As New ManagementObjectSearcher("SELECT Version FROM Win32_OperatingSystem")
            For Each os As ManagementObject In searcher.Get()
                Dim version = os("Version")?.ToString()
                If Not String.IsNullOrWhiteSpace(version) Then
                    ' Format: 10.0.19045 -> build is the 3rd part
                    Dim parts = version.Split("."c)
                    If parts.Length >= 3 Then
                        Return parts(2) ' 19045
                    End If
                End If
            Next
            Return "Unknown"
        Catch ex As Exception
            Return "[Error] " & ex.Message
        End Try
    End Function

    Public Shared Function GetOSFullBuildVersion() As String
        Try
            Dim versionBase As String = ""
            Dim ubr As String = ""

            ' Get base version from WMI
            Dim searcher As New ManagementObjectSearcher("SELECT Version FROM Win32_OperatingSystem")
            For Each os As ManagementObject In searcher.Get()
                versionBase = TryCast(os("Version"), String)
                Exit For
            Next

            ' Get UBR from registry
            Using key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion")
                If key IsNot Nothing Then
                    Dim ubrValue = key.GetValue("UBR")
                    If ubrValue IsNot Nothing Then
                        ubr = ubrValue.ToString()
                    End If
                End If
            End Using

            If Not String.IsNullOrEmpty(versionBase) Then
                If Not String.IsNullOrEmpty(ubr) Then
                    Return $"{versionBase}.{ubr}"
                Else
                    Return versionBase
                End If
            Else
                Return "Unknown"
            End If
        Catch ex As Exception
            Return "[Error] " & ex.Message
        End Try
    End Function


    Public Shared Function GetOSDisplayVersion() As String
        Try
            Using key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion")
                If key IsNot Nothing Then
                    ' Windows 10/11 specific
                    Dim displayVersion = TryCast(key.GetValue("DisplayVersion"), String)
                    If Not String.IsNullOrEmpty(displayVersion) Then
                        Return displayVersion ' e.g., "22H2"
                    End If

                    ' Fallback for older versions (Windows 7/8/10 pre-20H2)
                    Dim releaseId = TryCast(key.GetValue("ReleaseId"), String)
                    If Not String.IsNullOrEmpty(releaseId) Then
                        Return releaseId ' e.g., "1909"
                    End If
                End If
            End Using
            Return "Unknown"
        Catch ex As Exception
            Return "[Error] " & ex.Message
        End Try
    End Function

    Public Shared Function GetSystemModelInfo() As (Manufacturer As String, Model As String, SKU As String, Name As String, Version As String, InstallDate As String, Architecture As String)
        Dim manufacturer As String = ""
        Dim model As String = ""
        Dim sku As String = ""
        Dim name As String = ""
        Dim version As String = ""
        Dim installDate As String = ""
        Dim architecture As String = ""

        Try
            ' Get Manufacturer, Model, SKU from ComputerSystem
            Using searcher As New ManagementObjectSearcher("SELECT Manufacturer, Model, SystemSKUNumber FROM Win32_ComputerSystem")
                For Each mo As ManagementObject In searcher.Get()
                    manufacturer = mo("Manufacturer")?.ToString().Trim()
                    model = mo("Model")?.ToString().Trim()
                    sku = mo("SystemSKUNumber")?.ToString().Trim()
                    Exit For
                Next
            End Using

            ' Get OS info from OperatingSystem
            Using osSearcher As New ManagementObjectSearcher("SELECT Caption, Version, InstallDate, OSArchitecture FROM Win32_OperatingSystem")
                For Each os As ManagementObject In osSearcher.Get()
                    name = os("Caption")?.ToString().Trim()
                    version = os("Version")?.ToString().Trim()
                    Dim rawDate = TryCast(os("InstallDate"), String)
                    If Not String.IsNullOrWhiteSpace(rawDate) Then
                        installDate = ManagementDateTimeConverter.ToDateTime(rawDate).ToString("yyyy-MM-dd")
                    End If
                    architecture = os("OSArchitecture")?.ToString().Trim()
                    Exit For
                Next
            End Using

        Catch ex As Exception
            ' Optional: log or handle
        End Try

        Return (manufacturer, model, sku, name, version, installDate, architecture)
    End Function

    Public Shared Function GetServiceTag() As String
        Try
            Using searcher As New ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS")
                For Each mo As ManagementObject In searcher.Get()
                    Dim sn = mo("SerialNumber")?.ToString().Trim()
                    If Not String.IsNullOrWhiteSpace(sn) Then
                        Return sn
                    End If
                Next
            End Using
        Catch ex As Exception
            ' Log error if needed
        End Try

        Return ""
    End Function

End Class

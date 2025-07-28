Imports System.Management

Public Class NetworkInfoCollector
    Public Shared Function GetNetworkInfoList() As List(Of NetworkInfo)
        Dim result As New List(Of NetworkInfo)

        Try
            Dim adapterSearcher As New ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter = TRUE")
            Dim configSearcher As New ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = TRUE")

            Dim configMap As New Dictionary(Of Integer, ManagementObject)
            For Each config As ManagementObject In configSearcher.Get()
                If config("Index") IsNot Nothing Then
                    configMap(Convert.ToInt32(config("Index"))) = config
                End If
            Next

            For Each adapter As ManagementObject In adapterSearcher.Get()
                Dim adapterTypeID As Integer = If(adapter("AdapterTypeID") IsNot Nothing, Convert.ToInt32(adapter("AdapterTypeID")), -1)

                ' Only include Ethernet (0) and Wireless (9)
                If adapterTypeID <> 0 AndAlso adapterTypeID <> 9 Then Continue For

                Dim index As Integer = Convert.ToInt32(adapter("Index"))
                Dim statusCode As Integer = If(adapter("NetConnectionStatus") IsNot Nothing, Convert.ToInt32(adapter("NetConnectionStatus")), -1)
                Dim connectionStatus As String = GetConnectionStatusText(statusCode)

                Dim config As ManagementObject = Nothing
                configMap.TryGetValue(index, config)

                Dim ipList As New List(Of String)
                Dim mac As String = ""
                Dim dhcp As Boolean = False

                If config IsNot Nothing Then
                    If config("IPAddress") IsNot Nothing Then
                        ipList.AddRange(DirectCast(config("IPAddress"), String()))
                    End If
                    mac = If(config("MACAddress") IsNot Nothing, config("MACAddress").ToString(), "")
                    dhcp = If(config("DHCPEnabled") IsNot Nothing, Convert.ToBoolean(config("DHCPEnabled")), False)
                End If

                Dim speed As Long = 0
                If adapter("Speed") IsNot Nothing Then
                    Long.TryParse(adapter("Speed").ToString(), speed)
                End If

                Dim description As String = If(adapter("Description") IsNot Nothing, adapter("Description").ToString(), "")
                Dim adapterType As String = DetectInterfaceType(description)
                Dim isPhysical As Boolean = IsPhysicalAdapter(adapter)

                Dim info As New NetworkInfo With {
                .AdapterName = If(adapter("NetConnectionID") IsNot Nothing, adapter("NetConnectionID").ToString(), "[Unnamed]"),
                .Description = description,
                .MACAddress = mac,
                .IPAddresses = ipList,
                .SpeedMbps = speed \ (1024 * 1024),
                .IsDHCPEnabled = dhcp,
                .ConnectionStatus = connectionStatus,
                .InterfaceType = adapterType,
                .IsPhysical = isPhysical
            }

                result.Add(info)
            Next

        Catch ex As Exception
            result.Add(New NetworkInfo With {
            .AdapterName = "[Error]",
            .Description = ex.Message,
            .MACAddress = "",
            .IPAddresses = New List(Of String),
            .SpeedMbps = 0,
            .IsDHCPEnabled = False,
            .ConnectionStatus = "Unknown",
            .InterfaceType = "Unknown",
            .IsPhysical = False
        })
        End Try

        Return result
    End Function

    Private Shared Function GetConnectionStatusText(code As Integer) As String
        Select Case code
            Case 0 : Return "Disconnected"
            Case 1 : Return "Connecting"
            Case 2 : Return "Connected"
            Case 3 : Return "Disconnecting"
            Case 4 : Return "Hardware not present"
            Case 5 : Return "Hardware disabled"
            Case 6 : Return "Hardware malfunction"
            Case 7 : Return "Media disconnected"
            Case 8 : Return "Authenticating"
            Case 9 : Return "Authentication succeeded"
            Case 10 : Return "Authentication failed"
            Case 11 : Return "Invalid address"
            Case 12 : Return "Credentials required"
            Case Else : Return "Unknown"
        End Select
    End Function

    Private Shared Function DetectInterfaceType(description As String) As String
        If String.IsNullOrEmpty(description) Then Return "Unknown"
        Dim descLower = description.ToLower()
        If descLower.Contains("wifi") Or descLower.Contains("wireless") Then
            Return "Wireless"
        ElseIf descLower.Contains("ethernet") Then
            Return "Ethernet"
        ElseIf descLower.Contains("loopback") Then
            Return "Loopback"
        ElseIf descLower.Contains("bluetooth") Then
            Return "Bluetooth"
        Else
            Return "Other"
        End If
    End Function

    Private Shared Function IsPhysicalAdapter(adapter As ManagementObject) As Boolean
        ' Try using the PhysicalAdapter flag if available
        If adapter.Properties("PhysicalAdapter") IsNot Nothing Then
            Return Convert.ToBoolean(adapter("PhysicalAdapter"))
        End If

        ' Fallback: use heuristic check from name/description
        Dim desc As String = ""
        If adapter("Description") IsNot Nothing Then
            desc = adapter("Description").ToString().ToLower()
        End If

        If desc.Contains("virtual") OrElse desc.Contains("vmware") OrElse desc.Contains("loopback") OrElse desc.Contains("tunnel") Then
            Return False
        End If

        Return True ' Assume physical if not otherwise identified
    End Function
End Class

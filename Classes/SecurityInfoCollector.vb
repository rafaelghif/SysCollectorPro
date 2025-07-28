Public Class SecurityInfoCollector
    Public Shared Function GetSecureBootStatus() As String
        Return PowerShellHelper.GetSecureBootStatus()
    End Function

    Public Shared Function GetTPMStatus() As String
        Return PowerShellHelper.GetTPMStatus()
    End Function

    Public Shared Function GetBitLockerStatus() As String
        Return PowerShellHelper.GetBitLockerStatus()
    End Function

    Public Shared Function GetDefenderStatus() As String
        Return PowerShellHelper.GetDefenderStatus()
    End Function
End Class

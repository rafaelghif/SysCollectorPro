Public Class AuditCommand
    Public Property Name As String
    Public Property PowerShellCommand As String
    Public Property MinPSVersion As Integer
    Public Property RequiresAdmin As Boolean
    Public Property IsEnabled As Boolean = True
End Class

Public Class UserSessionCollector
    Public Shared Function IsCurrentUserAdmin() As Boolean
        Try
            Dim identity = Security.Principal.WindowsIdentity.GetCurrent()
            Dim principal = New Security.Principal.WindowsPrincipal(identity)
            Return principal.IsInRole(Security.Principal.WindowsBuiltInRole.Administrator)
        Catch
            Return False
        End Try
    End Function
End Class

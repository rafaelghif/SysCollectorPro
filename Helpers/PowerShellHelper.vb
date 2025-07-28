Public Class PowerShellHelper

    Public Shared Function IsPowerShellAvailable() As Boolean
        Try
            Dim psi As New ProcessStartInfo("powershell", "-Command $PSVersionTable.PSVersion.Major") With {
                .RedirectStandardOutput = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
            }

            Using proc As Process = Process.Start(psi)
                Dim output As String = proc.StandardOutput.ReadToEnd().Trim()
                proc.WaitForExit()

                Dim version As Integer
                If Integer.TryParse(output, version) AndAlso version >= 3 Then
                    Return True
                End If
            End Using
        Catch
        End Try
        Return False
    End Function

    Public Shared Function GetPowerShellVersion() As Integer
        Try
            Dim psi As New ProcessStartInfo("powershell", "-Command $PSVersionTable.PSVersion.Major") With {
                .RedirectStandardOutput = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
            }

            Using proc As Process = Process.Start(psi)
                Dim output = proc.StandardOutput.ReadToEnd().Trim()
                proc.WaitForExit()
                Dim ver As Integer
                If Integer.TryParse(output, ver) Then
                    Return ver
                End If
            End Using
        Catch
        End Try
        Return 0
    End Function

    Public Shared Function RunExternal(command As String) As String
        Try
            If Not IsPowerShellAvailable() Then
                Return "[PowerShell Not Available]"
            End If

            Dim psi As New ProcessStartInfo("powershell", "-NoProfile -ExecutionPolicy Bypass -Command """ & command & """") With {
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
            }

            Using proc As Process = Process.Start(psi)
                Dim output As String = proc.StandardOutput.ReadToEnd()
                Dim errorOut As String = proc.StandardError.ReadToEnd()
                proc.WaitForExit()

                If Not String.IsNullOrWhiteSpace(errorOut) Then
                    Return "[Error] " & errorOut.Trim()
                End If

                Return output.Trim()
            End Using
        Catch ex As Exception
            Return "[Exception] " & ex.Message
        End Try
    End Function

    Public Shared Function GetSecureBootStatus() As String
        Dim output As String = RunExternal("Confirm-SecureBootUEFI")

        If output.Contains("True") Then
            Return "Enabled"
        ElseIf output.Contains("False") Then
            Return "Disabled"
        ElseIf output.Contains("not supported") OrElse output.Contains("requires") Then
            Return "[Not Supported or BIOS]"
        Else
            Return "[Unknown] " & output
        End If
    End Function

    Public Shared Function GetTPMStatus() As String
        Dim output = RunExternal("
        if (Get-Command Get-Tpm -ErrorAction SilentlyContinue) {
            $tpm = Get-Tpm
            if ($tpm -ne $null) { $tpm.TpmPresent } else { '[Not Available]' }
        } else {
            '[Not Supported]'
        }
        ")
        Return ParseSingleLine(output)
    End Function

    Public Shared Function GetBitLockerStatus() As String
        Dim output = RunExternal("
        try {
            $vol = Get-BitLockerVolume -MountPoint 'C:' -ErrorAction Stop
            switch ($vol.ProtectionStatus) {
                0 { 'Off' }
                1 { 'On' }
                2 { 'Unknown' }
                default { $vol.ProtectionStatus }
            }
        } catch {
            '[Admin Required]'
        }
        ")
        Return ParseSingleLine(output)
    End Function

    Public Shared Function GetDefenderStatus() As String
        Dim output = RunExternal("(Get-MpComputerStatus).AntivirusEnabled")
        Return ParseSingleLine(output)
    End Function

    Public Shared Function GetInstalledUpdates(Optional count As Integer = 5) As String
        Dim output = RunExternal("
        Get-HotFix | Sort-Object InstalledOn -Descending | Select-Object -First " & count & " | ForEach-Object { $_.HotFixID }
        ")
        Return ParseListOutput(output)
    End Function

    Private Shared Function ParseSingleLine(raw As String) As String
        If raw.StartsWith("[") Then Return raw ' Error or Not Supported
        Return raw.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim()
    End Function

    Private Shared Function ParseListOutput(raw As String) As String
        If raw.StartsWith("[") Then Return raw
        Dim lines = raw.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries).Select(Function(s) s.Trim())
        Return String.Join("; ", lines)
    End Function

End Class

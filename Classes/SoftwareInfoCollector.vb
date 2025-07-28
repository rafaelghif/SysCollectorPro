Imports System.Management

Public Class SoftwareInfoCollector
    Public Shared Function GetInstalledSoftwareList() As List(Of String)
        Dim softwareList As New List(Of String)()
        Try
            Dim searcher As New ManagementObjectSearcher("SELECT Name FROM Win32_Product")
            For Each app As ManagementObject In searcher.Get()
                Dim name = TryCast(app("Name"), String)
                If Not String.IsNullOrEmpty(name) Then softwareList.Add(name)
            Next
        Catch
        End Try
        Return softwareList
    End Function

    Public Shared Function GetThirdPartyAntivirus() As List(Of String)
        Dim avList As New List(Of String)()
        Try
            Dim searcher As New ManagementObjectSearcher("root\SecurityCenter2", "SELECT * FROM AntivirusProduct")
            For Each av As ManagementObject In searcher.Get()
                Dim name = TryCast(av("displayName"), String)
                If Not String.IsNullOrEmpty(name) Then avList.Add(name)
            Next
        Catch
        End Try
        Return avList
    End Function
End Class

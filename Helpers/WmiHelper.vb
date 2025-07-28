' File: Helpers/WmiHelper.vb
Imports System.Management

Public Class WmiHelper

    ''' <summary>
    ''' Query WMI and return list of ManagementObjects.
    ''' </summary>
    Public Shared Function QueryWmi(scopePath As String, query As String) As List(Of ManagementObject)
        Dim results As New List(Of ManagementObject)()
        Try
            Dim scope As New ManagementScope(scopePath)
            scope.Connect()

            Dim searcher As New ManagementObjectSearcher(scope, New ObjectQuery(query))
            For Each mo As ManagementObject In searcher.Get()
                results.Add(mo)
            Next
        Catch ex As Exception
            ' Handle logging or fallbacks here
            Console.WriteLine("WMI Query Failed: " & ex.Message)
        End Try
        Return results
    End Function

    ''' <summary>
    ''' Shortcut for local WMI queries.
    ''' </summary>
    Public Shared Function QueryLocal(query As String) As List(Of ManagementObject)
        Return QueryWmi("\\.\root\cimv2", query)
    End Function

    ''' <summary>
    ''' Gets a single property value from the first result.
    ''' </summary>
    Public Shared Function GetSingleWmiValue(query As String, propertyName As String) As String
        Dim results = QueryLocal(query)
        If results.Count > 0 AndAlso results(0).Properties(propertyName) IsNot Nothing Then
            Return results(0).Properties(propertyName).Value?.ToString()
        End If
        Return String.Empty
    End Function

End Class

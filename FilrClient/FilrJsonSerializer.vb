Imports System.Net
Imports Newtonsoft.Json

Public Class FilrJsonSerializer(Of T)
    Public Function Deserialize(wc As WebClient, url As String) As T
        Dim data = wc.DownloadString(url)
        Debug.Print(data)
        Return CType(JsonConvert.DeserializeObject(Of T)(data), T)
    End Function
End Class
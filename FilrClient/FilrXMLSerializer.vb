Imports System.IO
Imports System.Net
Imports System.Xml
Imports System.Xml.Serialization
Public Class FilrXMLSerializer(Of T)
    Dim serializer As New XmlSerializer(GetType(T))
    Public Function Deserialize(wc As WebClient, url As String) As T
        Dim data = wc.DownloadString(url)
        Debug.Print(data)
        Using reader = New StringReader(data)
            Return CType(serializer.Deserialize(reader), T)
        End Using
    End Function
End Class
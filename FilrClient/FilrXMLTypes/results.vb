Imports System.Xml.Serialization

Namespace FilrXMLTypes
    <XmlRoot("results")>
    Public Class results
        <XmlElement("count")>
        Public count As Integer
        <XmlElement("first")>
        Public first As Integer
        <XmlArray("items")>
        <XmlArrayItem("results")>
        Public items As List(Of result)
    End Class
    Public Class result
        <XmlElement("href")>
        Public href As String
        <XmlElement("doc_type")>
        Public doc_type As String
        <XmlElement("entity_type")>
        Public entity_type As String
        <XmlElement("title")>
        Public title As String
        <XmlElement("name")>
        Public name As String
        <XmlElement("icon_href")>
        Public icon_href As String
        Public links As List(Of additionalLinks)
        <XmlIgnore>
        ReadOnly Property IsFile As Boolean
            Get
                If doc_type = "file" Then
                    Return True
                End If
                Return False
            End Get
        End Property
        <XmlIgnore>
        ReadOnly Property IsFolder As Boolean
            Get
                If entity_type = "folder" Then
                    Return True
                End If
                Return False
            End Get
        End Property
    End Class
End Namespace
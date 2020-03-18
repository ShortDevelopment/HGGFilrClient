Imports System.IO
Imports System.Net
Imports System.Text
Imports FilrClient.FilrXMLTypes

Public Class Explorer
    ReadOnly Property BaseURL = "https://filr.hgg-online.de/rest"
    'ReadOnly Property Client As New WebClient
    Public Shared UserName As String
    Public Shared Password As String
    Public Shared WithEvents Client As New WebClient
    Property CurrentPath As String
    Property CurrentURL As String
    Shared Property LastFolderURL As String
    Shared Property CurrentFolderURL As String
    Shared ReadOnly Property CacheDirectory As String = Path.Combine(Application.StartupPath, "Cache")
    Shared Property Instance As Explorer
    ReadOnly Property Credentials As NetworkCredential
        Get
            Return New NetworkCredential(UserName, Password)
        End Get
    End Property
    Public Sub New()
        InitializeComponent()
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12
        Instance = Me
        AddHandler Client.DownloadProgressChanged, Sub(sender As Object, e As DownloadProgressChangedEventArgs)
                                                       Instance.Invoke(Sub()
                                                                           Instance.ProgressBar1.Value = e.BytesReceived
                                                                           Application.DoEvents()
                                                                       End Sub)
                                                   End Sub
    End Sub
    Public Function Login(user As String, pass As String) As Boolean
        UserName = user
        Password = pass
        Client.UseDefaultCredentials = False
        Client.Credentials = Credentials
        Client.Encoding = Encoding.UTF8
        Try
            Client.DownloadString(BaseURL)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
    Public Sub LoadData()
        Invoke(Sub() UserNameLabel.Text = UserName)
        RunOnNewThread(Sub()
                           'Client.Credentials = Credentials
                           Dim root = New FilrJsonSerializer(Of binder_brief)().Deserialize(Client, $"{BaseURL}/self/shared_with_me")
                           Dim url = root.links.Where(Function(x) x.rel = "library_children")(0).href
                           DisplayFolderContent(url)
                       End Sub)
    End Sub
    Sub DisplayFolderContent(url As String)
        Invoke(Sub()
                   Panel2.Enabled = False
                   ListView1.Enabled = False
                   DirectoryEmptyLabel.Hide()
                   LoadingLabel.Show()
               End Sub)
        Dim data = New FilrJsonSerializer(Of results)().Deserialize(Client, $"{BaseURL}{url}")
        Invoke(Sub() ListView1.Items.Clear())
        For Each entry In data.items
            Dim item As New ListViewItem
            Dim info As New EntryInfo
            If entry.IsFile Then
                item.Text = entry.name
                Dim ic As Icon = GetFileIcon(Path.GetExtension(entry.name), IconSize.Small, True)
                Invoke(Sub()
                           ImageList1.Images.Add(ic)
                           item.ImageIndex = ImageList1.Images.Count - 1
                       End Sub)
                info.IsFile = True
                info.FileLength = entry.length
                info.Name = entry.name
                item.SubItems.Add(entry.modification.date.ToString("dd.MM.yyyy"))
                If Not entry.shares Is Nothing Then
                    item.SubItems.Add(entry.shares(0).sharer.name)
                    item.SubItems.Add(entry.shares(0).role)
                End If
            ElseIf entry.IsFolder Then
                item.Text = entry.title
                item.ImageKey = "outline_folder_black_18dp.png"
                info.IsFile = False
                info.Name = entry.title
                item.SubItems.Add("")
                If Not entry.shares Is Nothing Then
                    item.SubItems.Add(entry.shares(0).sharer.name)
                    item.SubItems.Add(entry.shares(0).role)
                End If
            End If
            info.Links = entry.links
            item.Tag = info
            Me.Invoke(Sub() ListView1.Items.Add(item))
        Next
        Invoke(Sub()
                   Panel2.Enabled = True
                   ListView1.Enabled = True
                   LoadingLabel.Hide()
                   If ListView1.Items.Count = 0 Then
                       DirectoryEmptyLabel.Show()
                   End If
               End Sub)
        LastFolderURL = CurrentFolderURL
        CurrentFolderURL = url
    End Sub
    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        If ListView1.SelectedItems.Count = 0 Or Panel7.Visible Then Exit Sub
        Dim info = CType(ListView1.SelectedItems(0).Tag, EntryInfo)
        If info.IsFile Then
            OpenFile(info.Name, info.FileLength, info.Links)
        Else
            OpenFolder(info.Links)
        End If
    End Sub
    Sub OpenFile(name As String, length As Integer, links As List(Of additionalLinks))
        Panel7.Show()
        ProgressBar1.Maximum = length
        ProgressBar1.Value = 0
        RunOnNewThread(Sub()
                           Dim url = links.Where(Function(x) x.rel = "content")(0).href
                           If Not Directory.Exists(CacheDirectory) Then
                               Directory.CreateDirectory(CacheDirectory)
                           End If
                           Dim file = Path.Combine(CacheDirectory, name)
                           Try
                               If IO.File.Exists(file) Then
                                   IO.File.Delete(file)
                               End If
                               Client.DownloadFileAsync(New Uri($"{BaseURL}{url}"), file)
                               While Client.IsBusy : End While
                               Process.Start(file)
                               Invoke(Sub() Panel7.Hide())
                           Catch ex As Exception
                               Invoke(Sub()
                                          Panel7.Hide()
                                          MsgBox("Datei konnte nicht heruntergeladen werden:" + vbNewLine + ex.Message, MsgBoxStyle.Exclamation)
                                      End Sub)
                           End Try
                       End Sub)
    End Sub
    Sub OpenFolder(links As List(Of additionalLinks))
        RunOnNewThread(Sub()
                           Dim url = links.Where(Function(x) x.rel = "library_children")(0).href
                           DisplayFolderContent(url)
                       End Sub)
    End Sub
    Public Class EntryInfo
        Property IsFile As Boolean
        Property Links As List(Of additionalLinks)
        Property Name As String
        Property FileLength As Integer
    End Class
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        DisplayFolderContent(CurrentFolderURL)
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DisplayFolderContent(LastFolderURL)
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        LoadData()
    End Sub
    Private Sub Explorer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            'If Directory.Exists(CacheDirectory) Then
            '    My.Computer.FileSystem.DeleteDirectory(CacheDirectory, FileIO.DeleteDirectoryOption.DeleteAllContents)
            'End If
        Catch : End Try
    End Sub
    Private Sub Panel7_Resize() Handles Panel7.Resize
        ProgressBar1.Size = New Size(ProgressBar1.Parent.Width - ProgressBar1.Location.X * 2, ProgressBar1.Height)
    End Sub
    Private Sub Panel7_VisibleChanged(sender As Object, e As EventArgs) Handles Panel7.VisibleChanged
        Panel7_Resize()
    End Sub
End Class
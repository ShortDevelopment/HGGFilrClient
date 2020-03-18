Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Public Module Extensions
    <Extension>
    Public Function RunOnNewThread(this As Control, a As Action) As Thread
        Dim t As New Thread(Sub()
                                a.Invoke()
                            End Sub)
        t.IsBackground = True
        t.Start()
        Return t
    End Function
    Public Function GetFileIcon(ByVal name As String, ByVal size As IconSize, ByVal linkOverlay As Boolean) As Icon
        Dim shfi As Shell32.SHFILEINFO = New Shell32.SHFILEINFO()
        Dim flags As UInteger = Shell32.SHGFI.Icon Or Shell32.SHGFI.UseFileAttributes
        If True = linkOverlay Then flags += Shell32.SHGFI.LinkOverlay

        If IconSize.Small = size Then
            flags += Shell32.SHGFI.SmallIcon
        Else
            flags += Shell32.SHGFI.LargeIcon
        End If

        Shell32.SHGetFileInfo(name, &H80, shfi, CUInt(Marshal.SizeOf(shfi)), flags) 'Shell32.FILE_ATTRIBUTE_NORMAL
        Dim icon As Icon = CType(Icon.FromHandle(shfi.hIcon).Clone(), Icon)
        Shell32.DestroyIcon(shfi.hIcon)
        Return icon
    End Function
    Enum IconSize
        Small
        Large
    End Enum
End Module
Public Class Shell32

    '''https://tabbles.net/how-to-have-large-file-icons-with-shgetfileinfo-in-c/

    <DllImport("shell32.dll")>
    Shared Function SHGetFileInfo _
     (ByVal pszPath As String,
      ByVal dwFileAttributes As Integer,
      ByRef psfi As SHFILEINFO,
      ByVal cbFileInfo As Integer,
      ByVal uFlags As Integer) As IntPtr
    End Function
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Public Structure SHFILEINFO
        Public hIcon As IntPtr
        Public iIcon As Integer
        Public dwAttributes As UInteger
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)>
        Public szDisplayName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=80)>
        Public szTypeName As String
    End Structure
    Enum SHGFI
        Icon = 256
        DisplayName = 512
        TypeName = 1024
        Attributes = 2048
        IconLocation = 4096
        ExeType = 8192
        SysIconIndex = 16384
        LinkOverlay = 32768
        Selected = 65536
        Attr_Specified = 131072
        LargeIcon = 0
        SmallIcon = 1
        OpenIcon = 2
        ShellIconSize = 4
        PIDL = 8
        UseFileAttributes = 16
        AddOverlays = 32
        OverlayIndex = 64
    End Enum
    <DllImport("user32.dll", EntryPoint:="DestroyIcon",
      SetLastError:=True)>
    Shared Function DestroyIcon(ByVal hIcon As IntPtr) As Integer
    End Function
End Class
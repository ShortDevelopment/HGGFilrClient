Imports System.Runtime.CompilerServices
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
End Module
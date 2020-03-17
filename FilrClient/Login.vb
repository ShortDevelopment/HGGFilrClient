Public Class Login
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        HintTextbox1.Enabled = False
        HintTextbox2.Enabled = False
        Button1.Enabled = False
        Application.DoEvents()
        RunOnNewThread(Sub()
                           Dim login = Explorer.Login(HintTextbox1.Text, HintTextbox2.Text)
                           Invoke(Sub()
                                      If login Then
                                          Explorer.Show()
                                          Explorer.LoadData()
                                          Me.Close()
                                      Else
                                          MsgBox("Es ist ein Fehler bei der Anmeldung aufgetreten." + vbNewLine + "Bitte überprüfe deine Anmeldedaten!", MsgBoxStyle.Exclamation)
                                      End If
                                  End Sub)
                       End Sub)
        HintTextbox1.Enabled = True
        HintTextbox2.Enabled = True
        Button1.Enabled = True
    End Sub
End Class
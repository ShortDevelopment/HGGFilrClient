Public Class HintTextbox
    Inherits TextBox
    Public Sub New()
        SetStyle(ControlStyles.UserPaint, True)
    End Sub
    Dim _Hint As String
    Property Hint As String
        Get
            Return _Hint
        End Get
        Set(value As String)
            _Hint = value
            Me.Invalidate()
        End Set
    End Property
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        'MyBase.OnPaint(e)
        If String.IsNullOrEmpty(Text) Then
            Using brush = New SolidBrush(Color.Gray)
                e.Graphics.DrawString(Hint, Font, brush, New Point(1, 1))
            End Using
        End If
    End Sub
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = &H85 AndAlso String.IsNullOrEmpty(Text) Then
            Using g = CreateGraphics()
                Using brush = New SolidBrush(Color.Gray)
                    g.DrawString(Hint, Font, brush, New Point(1, 1))
                End Using
            End Using
        Else
            MyBase.WndProc(m)
        End If
    End Sub

    Private Sub HintTextbox_TextChanged(sender As Object, e As EventArgs) Handles Me.TextChanged
        If String.IsNullOrEmpty(Text) Then
            SetStyle(ControlStyles.UserPaint, True)
        Else
            SetStyle(ControlStyles.UserPaint, False)
        End If
        Me.Invalidate()
    End Sub
End Class
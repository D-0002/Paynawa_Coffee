Imports System.Windows.Forms

Module ApplicationStartup
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Try
            DatabaseHelper.InitializeDatabase()
        Catch ex As Exception
            MessageBox.Show("Critical Error: Could not initialize the database." & vbCrLf & ex.Message,
                            "Database Initialization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Dim loginForm As New LoginForm()
        Dim loginResult As DialogResult = loginForm.ShowDialog()
        loginForm.Dispose()

        If loginResult = DialogResult.OK Then
            Dim mainForm As New Form1()
            Application.Run(mainForm)
        Else
            Application.Exit()
        End If
    End Sub
End Module
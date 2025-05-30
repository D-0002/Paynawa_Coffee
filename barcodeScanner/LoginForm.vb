Imports System.Drawing
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D

Public Class LoginForm

    Private pnlHeader As Panel
    Private lblTitle As Label
    Private lblSubtitle As Label
    Private btnClose As Button

    Private pnlLoginView As Panel
    Private pnlSignupView As Panel

    Private pnlLoginUser As Panel
    Private txtLoginUser As TextBox
    Private lblLoginUserHint As Label
    Private pnlLoginPass As Panel
    Private txtLoginPass As TextBox
    Private lblLoginPassHint As Label
    Private btnLogin As Button
    Private lblLoginStatus As Label
    Private chkRememberMe As CheckBox
    Private lnkSwitchToSignup As LinkLabel

    Private pnlSignupUser As Panel
    Private txtSignupUser As TextBox
    Private lblSignupUserHint As Label
    Private pnlSignupPass As Panel
    Private txtSignupPass As TextBox
    Private lblSignupPassHint As Label
    Private pnlSignupConfirm As Panel
    Private txtSignupConfirmPass As TextBox
    Private lblSignupConfirmHint As Label
    Private btnSignup As Button
    Private lblSignupStatus As Label
    Private lnkSwitchToLogin As LinkLabel

    Private isDragging As Boolean = False
    Private dragStartPoint As Point = New Point(0, 0)

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Paynawa Coffee Shop"
        Me.FormBorderStyle = FormBorderStyle.None
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(400, 550)
        Me.BackColor = Color.FromArgb(245, 247, 250)

        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.DoubleBuffer, True)
        UpdateStyles()

        InitializeHeaderAndBase()
        InitializeLoginView()
        InitializeSignupView()

        ShowLoginView()
    End Sub

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ClassStyle = cp.ClassStyle Or &H20000
            Return cp
        End Get
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim rect As Rectangle = New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
        Using path As GraphicsPath = GetRoundedRectangle(rect, 12)
            Using borderPen As New Pen(Color.FromArgb(200, 200, 220), 1)
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
                e.Graphics.DrawPath(borderPen, path)
            End Using
            Me.Region = New Region(path)
        End Using
        MyBase.OnPaint(e)
    End Sub


    Private Function GetRoundedRectangle(rect As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim diameter As Integer = radius * 2
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90)
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90)
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90)
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90)
        path.CloseFigure()
        Return path
    End Function

    Private Sub InitializeHeaderAndBase()
        pnlHeader = New Panel() With {
            .Dock = DockStyle.Top,
            .Height = 100,
            .BackColor = Color.FromArgb(110, 70, 46)
        }
        AddHandler pnlHeader.MouseDown, AddressOf Form_MouseDown
        AddHandler pnlHeader.MouseMove, AddressOf Form_MouseMove
        AddHandler pnlHeader.MouseUp, AddressOf Form_MouseUp

        lblTitle = New Label() With {
            .Text = "PAYNAWA",
            .Font = New Font("Segoe UI Black", 22.0F),
            .ForeColor = Color.White,
            .Location = New Point(25, 20),
            .AutoSize = True
        }
        AddHandler lblTitle.MouseDown, AddressOf Form_MouseDown
        AddHandler lblTitle.MouseMove, AddressOf Form_MouseMove
        AddHandler lblTitle.MouseUp, AddressOf Form_MouseUp


        lblSubtitle = New Label() With {
            .Text = "Coffee Shop Portal",
            .Font = New Font("Segoe UI Semilight", 10.0F),
            .ForeColor = Color.FromArgb(230, 210, 190),
            .Location = New Point(28, lblTitle.Bottom + 2),
            .AutoSize = True
        }
        AddHandler lblSubtitle.MouseDown, AddressOf Form_MouseDown
        AddHandler lblSubtitle.MouseMove, AddressOf Form_MouseMove
        AddHandler lblSubtitle.MouseUp, AddressOf Form_MouseUp

        btnClose = New Button() With {
            .Text = "✕",
            .Font = New Font("Segoe UI Semibold", 12.0F),
            .ForeColor = Color.FromArgb(230, 210, 190),
            .BackColor = pnlHeader.BackColor,
            .FlatStyle = FlatStyle.Flat,
            .Size = New Size(40, 30),
            .Location = New Point(Me.Width - 45, 5),
            .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
            .Cursor = Cursors.Hand
        }
        btnClose.FlatAppearance.BorderSize = 0
        btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(150, 90, 60)
        AddHandler btnClose.Click, Sub() Me.Close()

        pnlHeader.Controls.AddRange({lblTitle, lblSubtitle, btnClose})
        Me.Controls.Add(pnlHeader)
    End Sub

    Private Sub InitializeLoginView()
        pnlLoginView = New Panel() With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .Padding = New Padding(20, 20, 20, 20)
        }
        Me.Controls.Add(pnlLoginView)

        Dim yPos As Integer = 30
        Dim inputPanelMargin As Integer = 20

        Dim lblWelcome As New Label() With {
            .Text = "Welcome Back!",
            .Font = New Font("Segoe UI", 18.0F, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 40, 40),
            .Location = New Point(0, yPos),
            .AutoSize = False,
            .Size = New Size(pnlLoginView.Width, 35),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        }
        pnlLoginView.Controls.Add(lblWelcome)
        yPos += 40

        Dim lblLoginMsg As New Label() With {
            .Text = "Please sign in to continue",
            .Font = New Font("Segoe UI", 9.5F),
            .ForeColor = Color.Gray,
            .Location = New Point(0, yPos),
            .AutoSize = False,
            .Size = New Size(pnlLoginView.Width, 20),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        }
        pnlLoginView.Controls.Add(lblLoginMsg)
        yPos += 45

        pnlLoginUser = CreateModernInputPanel(inputPanelMargin, yPos, pnlLoginView.ClientSize.Width - (inputPanelMargin * 2))
        lblLoginUserHint = CreateInputHint("Username or Email", pnlLoginUser)
        txtLoginUser = CreateModernTextBox(pnlLoginUser)
        AddHandler txtLoginUser.Enter, Sub() AnimateLabel(lblLoginUserHint, True, txtLoginUser)
        AddHandler txtLoginUser.Leave, Sub() AnimateLabel(lblLoginUserHint, txtLoginUser.Text.Length > 0, txtLoginUser)
        AddHandler txtLoginUser.KeyPress, AddressOf Inputs_KeyPress
        pnlLoginView.Controls.Add(pnlLoginUser)
        yPos += 65

        pnlLoginPass = CreateModernInputPanel(inputPanelMargin, yPos, pnlLoginView.ClientSize.Width - (inputPanelMargin * 2))
        lblLoginPassHint = CreateInputHint("Password", pnlLoginPass)
        txtLoginPass = CreateModernTextBox(pnlLoginPass)
        txtLoginPass.UseSystemPasswordChar = True
        AddHandler txtLoginPass.Enter, Sub() AnimateLabel(lblLoginPassHint, True, txtLoginPass)
        AddHandler txtLoginPass.Leave, Sub() AnimateLabel(lblLoginPassHint, txtLoginPass.Text.Length > 0, txtLoginPass)
        AddHandler txtLoginPass.KeyPress, AddressOf Inputs_KeyPress
        pnlLoginView.Controls.Add(pnlLoginPass)
        yPos += 65

        chkRememberMe = New CheckBox() With {
            .Text = "Remember me",
            .Font = New Font("Segoe UI", 9.0F),
            .ForeColor = Color.FromArgb(80, 80, 80),
            .Location = New Point(inputPanelMargin, yPos),
            .AutoSize = True,
            .BackColor = Color.Transparent
        }
        pnlLoginView.Controls.Add(chkRememberMe)
        yPos += 40

        btnLogin = CreateModernButton("LOGIN", Color.FromArgb(139, 69, 19))
        btnLogin.Location = New Point(inputPanelMargin, yPos)
        btnLogin.Size = New Size(pnlLoginView.ClientSize.Width - (inputPanelMargin * 2), 45)
        AddHandler btnLogin.Click, AddressOf BtnLogin_Click
        pnlLoginView.Controls.Add(btnLogin)
        yPos += 60

        lnkSwitchToSignup = New LinkLabel() With {
            .Text = "Don't have an account? Sign Up",
            .Font = New Font("Segoe UI", 9.0F),
            .LinkColor = Color.FromArgb(139, 69, 19),
            .ActiveLinkColor = Color.FromArgb(100, 50, 10),
            .Location = New Point(inputPanelMargin, yPos),
            .AutoSize = True,
            .BackColor = Color.Transparent
        }
        CenterHorizontally(lnkSwitchToSignup, pnlLoginView)
        AddHandler lnkSwitchToSignup.Click, Sub() ShowSignupView()
        pnlLoginView.Controls.Add(lnkSwitchToSignup)
        yPos += 30

        lblLoginStatus = New Label() With {
            .Location = New Point(inputPanelMargin, yPos),
            .Size = New Size(pnlLoginView.ClientSize.Width - (inputPanelMargin * 2), 25),
            .ForeColor = Color.FromArgb(220, 38, 38),
            .Font = New Font("Segoe UI", 9.0F),
            .TextAlign = ContentAlignment.MiddleCenter,
            .BackColor = Color.Transparent
        }
        pnlLoginView.Controls.Add(lblLoginStatus)
    End Sub

    Private Sub InitializeSignupView()
        pnlSignupView = New Panel() With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .Padding = New Padding(20, 20, 20, 20),
            .Visible = False
        }
        Me.Controls.Add(pnlSignupView)

        Dim yPos As Integer = 25
        Dim inputPanelMargin As Integer = 20

        Dim lblCreateAccount As New Label() With {
            .Text = "Create Your Account",
            .Font = New Font("Segoe UI", 18.0F, FontStyle.Bold),
            .ForeColor = Color.FromArgb(40, 40, 40),
            .Location = New Point(0, yPos),
            .AutoSize = False,
            .Size = New Size(pnlSignupView.Width, 35),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        }
        pnlSignupView.Controls.Add(lblCreateAccount)
        yPos += 40

        Dim lblSignupMsg As New Label() With {
            .Text = "It's quick and easy.",
            .Font = New Font("Segoe UI", 9.5F),
            .ForeColor = Color.Gray,
            .Location = New Point(0, yPos),
            .AutoSize = False,
            .Size = New Size(pnlSignupView.Width, 20),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        }
        pnlSignupView.Controls.Add(lblSignupMsg)
        yPos += 40


        pnlSignupUser = CreateModernInputPanel(inputPanelMargin, yPos, pnlSignupView.ClientSize.Width - (inputPanelMargin * 2))
        lblSignupUserHint = CreateInputHint("Choose Username", pnlSignupUser)
        txtSignupUser = CreateModernTextBox(pnlSignupUser)
        AddHandler txtSignupUser.Enter, Sub() AnimateLabel(lblSignupUserHint, True, txtSignupUser)
        AddHandler txtSignupUser.Leave, Sub() AnimateLabel(lblSignupUserHint, txtSignupUser.Text.Length > 0, txtSignupUser)
        AddHandler txtSignupUser.KeyPress, AddressOf Inputs_KeyPress
        pnlSignupView.Controls.Add(pnlSignupUser)
        yPos += 65

        pnlSignupPass = CreateModernInputPanel(inputPanelMargin, yPos, pnlSignupView.ClientSize.Width - (inputPanelMargin * 2))
        lblSignupPassHint = CreateInputHint("Create Password (min 6 chars)", pnlSignupPass)
        txtSignupPass = CreateModernTextBox(pnlSignupPass)
        txtSignupPass.UseSystemPasswordChar = True
        AddHandler txtSignupPass.Enter, Sub() AnimateLabel(lblSignupPassHint, True, txtSignupPass)
        AddHandler txtSignupPass.Leave, Sub() AnimateLabel(lblSignupPassHint, txtSignupPass.Text.Length > 0, txtSignupPass)
        AddHandler txtSignupPass.KeyPress, AddressOf Inputs_KeyPress
        pnlSignupView.Controls.Add(pnlSignupPass)
        yPos += 65

        pnlSignupConfirm = CreateModernInputPanel(inputPanelMargin, yPos, pnlSignupView.ClientSize.Width - (inputPanelMargin * 2))
        lblSignupConfirmHint = CreateInputHint("Confirm Password", pnlSignupConfirm)
        txtSignupConfirmPass = CreateModernTextBox(pnlSignupConfirm)
        txtSignupConfirmPass.UseSystemPasswordChar = True
        AddHandler txtSignupConfirmPass.Enter, Sub() AnimateLabel(lblSignupConfirmHint, True, txtSignupConfirmPass)
        AddHandler txtSignupConfirmPass.Leave, Sub() AnimateLabel(lblSignupConfirmHint, txtSignupConfirmPass.Text.Length > 0, txtSignupConfirmPass)
        AddHandler txtSignupConfirmPass.KeyPress, AddressOf Inputs_KeyPress
        pnlSignupView.Controls.Add(pnlSignupConfirm)
        yPos += 70

        btnSignup = CreateModernButton("CREATE ACCOUNT", Color.FromArgb(34, 177, 76))
        btnSignup.Location = New Point(inputPanelMargin, yPos)
        btnSignup.Size = New Size(pnlSignupView.ClientSize.Width - (inputPanelMargin * 2), 45)
        AddHandler btnSignup.Click, AddressOf BtnSignup_Click
        pnlSignupView.Controls.Add(btnSignup)
        yPos += 60

        lnkSwitchToLogin = New LinkLabel() With {
            .Text = "Already have an account? Login",
            .Font = New Font("Segoe UI", 9.0F),
            .LinkColor = Color.FromArgb(139, 69, 19),
            .ActiveLinkColor = Color.FromArgb(100, 50, 10),
            .Location = New Point(inputPanelMargin, yPos),
            .AutoSize = True,
            .BackColor = Color.Transparent
        }
        CenterHorizontally(lnkSwitchToLogin, pnlSignupView)
        AddHandler lnkSwitchToLogin.Click, Sub() ShowLoginView()
        pnlSignupView.Controls.Add(lnkSwitchToLogin)
        yPos += 30

        lblSignupStatus = New Label() With {
            .Location = New Point(inputPanelMargin, yPos),
            .Size = New Size(pnlSignupView.ClientSize.Width - (inputPanelMargin * 2), 25),
            .ForeColor = Color.FromArgb(220, 38, 38),
            .Font = New Font("Segoe UI", 9.0F),
            .TextAlign = ContentAlignment.MiddleCenter,
            .BackColor = Color.Transparent
        }
        pnlSignupView.Controls.Add(lblSignupStatus)
    End Sub

    Private Sub ShowLoginView()
        pnlSignupView.Visible = False
        pnlLoginView.Visible = True
        pnlLoginView.BringToFront()

        Me.AcceptButton = btnLogin
        If txtLoginUser IsNot Nothing Then txtLoginUser.Focus()
        lblLoginStatus.Text = ""
        lblSignupStatus.Text = ""
    End Sub

    Private Sub ShowSignupView()
        pnlLoginView.Visible = False
        pnlSignupView.Visible = True
        pnlSignupView.BringToFront()

        Me.AcceptButton = btnSignup
        If txtSignupUser IsNot Nothing Then txtSignupUser.Focus()
        lblLoginStatus.Text = ""
        lblSignupStatus.Text = ""
    End Sub

    Private Sub CenterHorizontally(ctrl As Control, parentCtrl As Control)
        ctrl.Location = New Point(CInt((parentCtrl.ClientSize.Width - ctrl.Width) / 2), ctrl.Location.Y)
    End Sub

    Private Function CreateModernInputPanel(x As Integer, y As Integer, width As Integer) As Panel
        Dim panel As New Panel() With {
            .Location = New Point(x, y),
            .Size = New Size(width, 48),
            .BackColor = Color.FromArgb(240, 243, 246),
            .Padding = New Padding(1),
            .Tag = Color.FromArgb(200, 205, 210),
            .Cursor = Cursors.IBeam
        }

        AddHandler panel.Paint, Sub(sender, e)
                                    Dim p As Panel = DirectCast(sender, Panel)
                                    Dim borderColor As Color = DirectCast(p.Tag, Color)
                                    Using pen As New Pen(borderColor, 1)
                                        Dim rect As Rectangle = New Rectangle(0, 0, p.ClientRectangle.Width - 1, p.ClientRectangle.Height - 1)
                                        Using path As GraphicsPath = GetRoundedRectangle(rect, 6)
                                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
                                            e.Graphics.DrawPath(pen, path)
                                        End Using
                                    End Using
                                End Sub
        AddHandler panel.Click, Sub(sender, e)
                                    Dim clickedPanel As Panel = DirectCast(sender, Panel)
                                    For Each ctrl As Control In clickedPanel.Controls
                                        If TypeOf ctrl Is TextBox Then
                                            DirectCast(ctrl, TextBox).Focus()
                                            Exit For
                                        End If
                                    Next
                                End Sub
        Return panel
    End Function

    Private Function CreateInputHint(text As String, parentPanel As Panel) As Label
        Dim label As New Label() With {
            .Text = text,
            .Font = New Font("Segoe UI", 9.5F),
            .ForeColor = Color.FromArgb(150, 150, 150),
            .Location = New Point(12, CInt((parentPanel.Height - .Height) / 2)),
            .AutoSize = True,
            .BackColor = Color.Transparent,
            .Cursor = Cursors.IBeam
        }
        parentPanel.Controls.Add(label)
        label.BringToFront()

        AddHandler label.Click, Sub(sender, e)
                                    If parentPanel IsNot Nothing Then
                                        For Each ctrl As Control In parentPanel.Controls
                                            If TypeOf ctrl Is TextBox Then
                                                DirectCast(ctrl, TextBox).Focus()
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End Sub
        Return label
    End Function

    Private Function CreateModernTextBox(parent As Panel) As TextBox
        Dim textBox As New TextBox() With {
            .Location = New Point(10, CInt((parent.Height - .Height) / 2) + 1),
            .Size = New Size(parent.Width - 20, 20),
            .Font = New Font("Segoe UI", 10.0F),
            .BorderStyle = BorderStyle.None,
            .BackColor = parent.BackColor,
            .ForeColor = Color.FromArgb(50, 50, 50)
        }
        parent.Controls.Add(textBox)

        AddHandler textBox.Enter, Sub()
                                      parent.Tag = Color.FromArgb(139, 69, 19)
                                      parent.BackColor = Color.White
                                      textBox.BackColor = Color.White
                                      parent.Invalidate()
                                  End Sub

        AddHandler textBox.Leave, Sub()
                                      parent.Tag = Color.FromArgb(200, 205, 210)
                                      parent.BackColor = Color.FromArgb(240, 243, 246)
                                      textBox.BackColor = parent.BackColor
                                      parent.Invalidate()
                                  End Sub
        Return textBox
    End Function

    Private Function CreateModernButton(text As String, backColor As Color) As Button
        Dim btn As New Button() With {
            .Text = text,
            .Font = New Font("Segoe UI Semibold", 10.0F),
            .ForeColor = Color.White,
            .BackColor = backColor,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .Height = 45
        }

        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(
            Math.Max(0, backColor.R - 20),
            Math.Max(0, backColor.G - 20),
            Math.Max(0, backColor.B - 20))
        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(
            Math.Max(0, backColor.R - 40),
            Math.Max(0, backColor.G - 40),
            Math.Max(0, backColor.B - 40))
        Return btn
    End Function

    Private Sub AnimateLabel(label As Label, moveUp As Boolean, associatedTextBox As TextBox)
        If associatedTextBox.Text.Length > 0 AndAlso Not moveUp Then
        ElseIf moveUp Then
            label.Font = New Font("Segoe UI", 7.5F)
            label.ForeColor = Color.FromArgb(139, 69, 19)
            label.Location = New Point(10, 4)
            label.BackColor = associatedTextBox.Parent.BackColor
        Else
            label.Font = New Font("Segoe UI", 9.5F)
            label.ForeColor = Color.FromArgb(150, 150, 150)
            label.Location = New Point(12, CInt((associatedTextBox.Parent.Height - label.Height) / 2))
            label.BackColor = Color.Transparent
        End If
        label.Parent.Invalidate()
    End Sub

    Private Sub Inputs_KeyPress(sender As Object, e As KeyPressEventArgs)
        If e.KeyChar = ChrW(Keys.Enter) Then
            If pnlLoginView.Visible Then
                BtnLogin_Click(Nothing, EventArgs.Empty)
            ElseIf pnlSignupView.Visible Then
                BtnSignup_Click(Nothing, EventArgs.Empty)
            End If
            e.Handled = True
        End If
    End Sub

    Private Sub BtnLogin_Click(sender As Object, e As EventArgs)
        lblLoginStatus.ForeColor = Color.FromArgb(220, 38, 38)
        lblLoginStatus.Text = ""
        Dim username As String = txtLoginUser.Text.Trim()
        Dim passwordVal As String = txtLoginPass.Text

        If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(passwordVal) Then
            lblLoginStatus.Text = "Username and password are required."
            Return
        End If

        Try
            If DatabaseHelper.VerifyUser(username, passwordVal) Then
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                lblLoginStatus.Text = "Invalid username or password."
                txtLoginPass.Clear()
                AnimateLabel(lblLoginPassHint, False, txtLoginPass)
                txtLoginPass.Focus()
            End If
        Catch ex As Exception
            lblLoginStatus.Text = "Login error. Please try again."
            Console.WriteLine("Login Error: " & ex.ToString())
        End Try
    End Sub

    Private Sub BtnSignup_Click(sender As Object, e As EventArgs)
        lblSignupStatus.ForeColor = Color.FromArgb(220, 38, 38)
        lblSignupStatus.Text = ""
        Dim username As String = txtSignupUser.Text.Trim()
        Dim passwordVal As String = txtSignupPass.Text
        Dim confirmPassword As String = txtSignupConfirmPass.Text

        If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(passwordVal) OrElse String.IsNullOrWhiteSpace(confirmPassword) Then
            lblSignupStatus.Text = "All fields are required."
            Return
        End If

        If passwordVal.Length < 6 Then
            lblSignupStatus.Text = "Password must be at least 6 characters."
            Return
        End If

        If passwordVal <> confirmPassword Then
            lblSignupStatus.Text = "Passwords do not match."
            Return
        End If

        Try
            If DatabaseHelper.UserExists(username) Then
                lblSignupStatus.Text = $"Username '{username}' is already taken."
                Return
            End If

            If DatabaseHelper.AddUser(username, passwordVal) Then
                lblSignupStatus.ForeColor = Color.FromArgb(34, 177, 76)
                lblSignupStatus.Text = "Account created! Please login."
                txtSignupUser.Clear()
                txtSignupPass.Clear()
                txtSignupConfirmPass.Clear()

                AnimateLabel(lblSignupUserHint, False, txtSignupUser)
                AnimateLabel(lblSignupPassHint, False, txtSignupPass)
                AnimateLabel(lblSignupConfirmHint, False, txtSignupConfirmPass)

                ShowLoginView()
                txtLoginUser.Text = username
                txtLoginPass.Focus()
            Else
                lblSignupStatus.Text = "Failed to create account."
            End If
        Catch exDb As MySql.Data.MySqlClient.MySqlException
            lblSignupStatus.Text = "Database error. Please try again."
            Console.WriteLine("Signup DB Error: " & exDb.ToString())
        Catch ex As Exception
            lblSignupStatus.Text = "Signup error. Please try again."
            Console.WriteLine("Signup Error: " & ex.ToString())
        End Try
    End Sub

    Private Sub Form_MouseDown(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            isDragging = True
            dragStartPoint = New Point(e.X, e.Y)
        End If
    End Sub

    Private Sub Form_MouseMove(sender As Object, e As MouseEventArgs)
        If isDragging Then
            Dim currentScreenPos As Point = PointToScreen(e.Location)
            Location = New Point(currentScreenPos.X - dragStartPoint.X, currentScreenPos.Y - dragStartPoint.Y)
        End If
    End Sub

    Private Sub Form_MouseUp(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            isDragging = False
        End If
    End Sub
End Class
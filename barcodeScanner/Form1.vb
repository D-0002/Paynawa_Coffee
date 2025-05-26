Imports System
Imports System.Drawing
Imports System.Media
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic

Public Class Form1
    Private txtBarcodeInput As TextBox
    Private lblCurrentScanStatus As Label

    Private gbTransaction As GroupBox
    Private lstTransactionItems As ListBox
    Private lblTransactionTotal As Label

    Private btnOpenItemManagement As Button
    Private btnConfirmOrder As Button
    Private btnClearTransaction As Button

    Private currentTransactionLineItems As New List(Of TransactionLineItem)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            DatabaseHelper.InitializeDatabase()
            Me.Text = "Nike Shoe Shop - Point of Sale System"
            Me.Size = New Size(900, 750)
            Me.MinimumSize = New Size(800, 650)
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.BackColor = Color.FromArgb(245, 248, 250)
            Me.Font = New Font("Segoe UI", 9.0F)

            CreateScannerControls()
            ClearTransaction()
            txtBarcodeInput.Focus()

        Catch ex As Exception
            MessageBox.Show("Error initializing application: " & ex.Message, "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CreateScannerControls()
        Dim margin As Integer = 25
        Dim currentY As Integer = margin

        ' Header Panel
        Dim headerPanel As New Panel()
        headerPanel.Location = New Point(0, 0)
        headerPanel.Size = New Size(Me.ClientSize.Width, 80)
        headerPanel.BackColor = Color.FromArgb(41, 128, 185)
        headerPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(headerPanel)

        Dim lblScannerTitle As New Label()
        lblScannerTitle.Text = "NIKE SHOE SHOP"
        lblScannerTitle.Font = New Font("Segoe UI", 22, FontStyle.Bold)
        lblScannerTitle.ForeColor = Color.White
        lblScannerTitle.Size = New Size(headerPanel.Width - 40, 35)
        lblScannerTitle.Location = New Point(20, 15)
        lblScannerTitle.TextAlign = ContentAlignment.MiddleLeft
        lblScannerTitle.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        headerPanel.Controls.Add(lblScannerTitle)

        currentY = headerPanel.Bottom + margin

        ' Scanner Input Section
        Dim scannerPanel As New Panel()
        scannerPanel.Location = New Point(margin, currentY)
        scannerPanel.Size = New Size(Me.ClientSize.Width - (margin * 2), 140)
        scannerPanel.BackColor = Color.White
        scannerPanel.BorderStyle = BorderStyle.FixedSingle
        scannerPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(scannerPanel)

        Dim lblScanInputInstruction As New Label()
        lblScanInputInstruction.Text = "🔍 SCAN OR ENTER ITEM DETAILS"
        lblScanInputInstruction.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        lblScanInputInstruction.ForeColor = Color.FromArgb(52, 73, 94)
        lblScanInputInstruction.Location = New Point(20, 15)
        lblScanInputInstruction.Size = New Size(scannerPanel.Width - 40, 25)
        lblScanInputInstruction.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        scannerPanel.Controls.Add(lblScanInputInstruction)

        Dim lblInstructionDetail As New Label()
        lblInstructionDetail.Text = "Enter Barcode, Item ID, or Item Name and press Enter"
        lblInstructionDetail.Font = New Font("Segoe UI", 9.5F)
        lblInstructionDetail.ForeColor = Color.FromArgb(100, 116, 139)
        lblInstructionDetail.Location = New Point(20, 40)
        lblInstructionDetail.Size = New Size(scannerPanel.Width - 40, 20)
        lblInstructionDetail.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        scannerPanel.Controls.Add(lblInstructionDetail)

        txtBarcodeInput = New TextBox()
        txtBarcodeInput.Location = New Point(20, 70)
        txtBarcodeInput.Size = New Size(scannerPanel.Width - 40, 35)
        txtBarcodeInput.Font = New Font("Consolas", 14)
        txtBarcodeInput.BorderStyle = BorderStyle.FixedSingle
        txtBarcodeInput.BackColor = Color.FromArgb(248, 249, 250)
        txtBarcodeInput.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        AddHandler txtBarcodeInput.KeyPress, AddressOf BarcodeInput_KeyPress
        AddHandler txtBarcodeInput.Enter, AddressOf TxtBarcodeInput_Enter
        AddHandler txtBarcodeInput.Leave, AddressOf TxtBarcodeInput_Leave
        scannerPanel.Controls.Add(txtBarcodeInput)

        currentY = scannerPanel.Bottom + 15

        ' Status Panel
        lblCurrentScanStatus = New Label()
        lblCurrentScanStatus.Location = New Point(margin, currentY)
        lblCurrentScanStatus.Size = New Size(Me.ClientSize.Width - (margin * 2), 45)
        lblCurrentScanStatus.BorderStyle = BorderStyle.FixedSingle
        lblCurrentScanStatus.BackColor = Color.White
        lblCurrentScanStatus.Font = New Font("Segoe UI", 11.0F)
        lblCurrentScanStatus.ForeColor = Color.FromArgb(71, 85, 105)
        lblCurrentScanStatus.TextAlign = ContentAlignment.MiddleLeft
        lblCurrentScanStatus.Padding = New Padding(15, 0, 0, 0)
        lblCurrentScanStatus.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(lblCurrentScanStatus)

        currentY += lblCurrentScanStatus.Height + 20

        ' Transaction Section
        gbTransaction = New GroupBox()
        gbTransaction.Text = "   🛒 CURRENT TRANSACTION"
        gbTransaction.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)
        gbTransaction.ForeColor = Color.FromArgb(52, 73, 94)
        gbTransaction.Location = New Point(margin, currentY)
        gbTransaction.Size = New Size(Me.ClientSize.Width - (margin * 2), Me.ClientSize.Height - currentY - 120)
        gbTransaction.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        gbTransaction.BackColor = Color.White
        gbTransaction.FlatStyle = FlatStyle.Flat
        Me.Controls.Add(gbTransaction)

        Dim gbPadding As Integer = 20
        Dim gbCurrentY As Integer = 35

        lstTransactionItems = New ListBox()
        lstTransactionItems.Location = New Point(gbPadding, gbCurrentY)
        lstTransactionItems.Size = New Size(gbTransaction.ClientSize.Width - (gbPadding * 2), gbTransaction.ClientSize.Height - gbCurrentY - 70)
        lstTransactionItems.Font = New Font("Courier New", 10.5F)
        lstTransactionItems.BorderStyle = BorderStyle.FixedSingle
        lstTransactionItems.IntegralHeight = False
        lstTransactionItems.BackColor = Color.FromArgb(248, 249, 250)
        lstTransactionItems.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        gbTransaction.Controls.Add(lstTransactionItems)

        ' Total Panel
        Dim totalPanel As New Panel()
        totalPanel.Location = New Point(gbPadding, lstTransactionItems.Bottom + 10)
        totalPanel.Size = New Size(gbTransaction.ClientSize.Width - (gbPadding * 2), 50)
        totalPanel.BackColor = Color.FromArgb(46, 125, 50)
        totalPanel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        gbTransaction.Controls.Add(totalPanel)

        lblTransactionTotal = New Label()
        lblTransactionTotal.Text = "TOTAL: ₱0.00"
        lblTransactionTotal.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
        lblTransactionTotal.ForeColor = Color.White
        lblTransactionTotal.Location = New Point(15, 0)
        lblTransactionTotal.Size = New Size(totalPanel.Width - 30, totalPanel.Height)
        lblTransactionTotal.TextAlign = ContentAlignment.MiddleRight
        lblTransactionTotal.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        totalPanel.Controls.Add(lblTransactionTotal)

        Dim buttonPanel As New Panel()
        buttonPanel.Location = New Point(0, gbTransaction.Bottom + 15)
        buttonPanel.Size = New Size(Me.ClientSize.Width, 70)
        buttonPanel.BackColor = Color.FromArgb(245, 248, 250)
        buttonPanel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(buttonPanel)

        btnOpenItemManagement = CreateStyledButton("⚙️ MANAGE & RECORDS", Color.FromArgb(108, 117, 125), Color.White)
        btnOpenItemManagement.Size = New Size(200, 45)
        btnOpenItemManagement.Location = New Point(margin, 12)
        btnOpenItemManagement.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        AddHandler btnOpenItemManagement.Click, AddressOf OpenItemManagement_Click
        buttonPanel.Controls.Add(btnOpenItemManagement)

        btnClearTransaction = CreateStyledButton("🗑️ CLEAR", Color.FromArgb(231, 76, 60), Color.White)
        btnClearTransaction.Size = New Size(120, 45)
        btnClearTransaction.Location = New Point(buttonPanel.Width - 350, 12)
        btnClearTransaction.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        AddHandler btnClearTransaction.Click, AddressOf ClearTransaction_Click
        buttonPanel.Controls.Add(btnClearTransaction)

        btnConfirmOrder = CreateStyledButton("✅ CONFIRM ORDER", Color.FromArgb(39, 174, 96), Color.White)
        btnConfirmOrder.Size = New Size(200, 45)
        btnConfirmOrder.Location = New Point(buttonPanel.Width - btnConfirmOrder.Width - margin, 12)
        btnConfirmOrder.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        AddHandler btnConfirmOrder.Click, AddressOf ConfirmOrder_Click
        buttonPanel.Controls.Add(btnConfirmOrder)
    End Sub

    Private Function CreateStyledButton(text As String, backColor As Color, foreColor As Color) As Button
        Dim btn As New Button()
        btn.Text = text
        btn.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        btn.BackColor = backColor
        btn.ForeColor = foreColor
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(Math.Max(0, backColor.R - 20), Math.Max(0, backColor.G - 20), Math.Max(0, backColor.B - 20))
        btn.Cursor = Cursors.Hand
        Return btn
    End Function

    Private Sub TxtBarcodeInput_Enter(sender As Object, e As EventArgs)
        txtBarcodeInput.BackColor = Color.White
    End Sub

    Private Sub TxtBarcodeInput_Leave(sender As Object, e As EventArgs)
        txtBarcodeInput.BackColor = Color.FromArgb(248, 249, 250)
    End Sub

    Private Sub BarcodeInput_KeyPress(sender As Object, e As KeyPressEventArgs)
        If e.KeyChar = Chr(Keys.Enter) Then
            ProcessScannedInput()
            e.Handled = True
        End If
    End Sub

    Private Sub ProcessScannedInput()
        Try
            Dim userInput As String = txtBarcodeInput.Text.Trim()
            Dim baseItem As BarcodeItem = Nothing

            If String.IsNullOrWhiteSpace(userInput) Then
                txtBarcodeInput.Focus()
                Return
            End If

            Dim itemId As Integer
            If Integer.TryParse(userInput, itemId) Then
                baseItem = DatabaseHelper.FindItemById(itemId)
            End If

            If baseItem Is Nothing Then
                baseItem = DatabaseHelper.FindItemByBarcode(userInput)
            End If

            If baseItem Is Nothing Then
                baseItem = DatabaseHelper.FindItemByName(userInput)
            End If

            If baseItem IsNot Nothing Then
                Dim quantityString As String = Interaction.InputBox($"Enter quantity for '{baseItem.Name}' (ID: {baseItem.Id}):{vbCrLf}Current Price: ₱{baseItem.Price:F2}", "Enter Quantity", "1")

                If String.IsNullOrWhiteSpace(quantityString) Then
                    UpdateScanStatus("⚠️ Quantity input cancelled for " & baseItem.Name, Color.FromArgb(255, 243, 205), Color.FromArgb(133, 77, 14))
                    SystemSounds.Asterisk.Play()
                    txtBarcodeInput.Clear()
                    txtBarcodeInput.Focus()
                    Return
                End If

                Dim quantity As Integer
                If Integer.TryParse(quantityString, quantity) AndAlso quantity > 0 Then
                    UpdateTransactionItem(baseItem, quantity)
                    UpdateScanStatus($"✅ Added: {baseItem.Name} (Qty: {quantity}) - ₱{baseItem.Price:F2} each", Color.FromArgb(212, 237, 218), Color.FromArgb(21, 87, 36))
                    SystemSounds.Beep.Play()
                Else
                    UpdateScanStatus($"❌ Invalid quantity '{quantityString}'. Must be a positive number.", Color.FromArgb(248, 215, 218), Color.FromArgb(114, 28, 36))
                    SystemSounds.Hand.Play()
                End If
            Else
                UpdateScanStatus($"❌ Item Not Found: '{userInput}'", Color.FromArgb(248, 215, 218), Color.FromArgb(114, 28, 36))
                SystemSounds.Hand.Play()
            End If

            txtBarcodeInput.Clear()
            txtBarcodeInput.Focus()

        Catch ex As Exception
            UpdateScanStatus("🚫 PROCESSING ERROR: " & ex.Message, Color.FromArgb(248, 215, 218), Color.FromArgb(114, 28, 36))
            SystemSounds.Exclamation.Play()
        End Try
    End Sub

    Private Sub UpdateScanStatus(message As String, backColor As Color, foreColor As Color)
        lblCurrentScanStatus.Text = message
        lblCurrentScanStatus.BackColor = backColor
        lblCurrentScanStatus.ForeColor = foreColor
    End Sub

    Private Sub UpdateTransactionItem(itemToAdd As BarcodeItem, newQuantity As Integer)
        Dim existingLineItem As TransactionLineItem = currentTransactionLineItems.FirstOrDefault(Function(line) line.Item.Id = itemToAdd.Id)

        If existingLineItem IsNot Nothing Then
            existingLineItem.Quantity = newQuantity
        Else
            Dim newLineItem As New TransactionLineItem(itemToAdd, newQuantity)
            currentTransactionLineItems.Add(newLineItem)
        End If

        RefreshTransactionDisplay()
    End Sub

    Private Sub RefreshTransactionDisplay()
        lstTransactionItems.BeginUpdate()
        lstTransactionItems.Items.Clear()

        Dim runningTotal As Decimal = 0D

        For Each lineItem In currentTransactionLineItems
            lstTransactionItems.Items.Add(lineItem.ToString())
            runningTotal += lineItem.LineTotal
        Next

        If lstTransactionItems.Items.Count > 0 Then
            lstTransactionItems.SelectedIndex = lstTransactionItems.Items.Count - 1
            lstTransactionItems.TopIndex = Math.Max(0, lstTransactionItems.Items.Count - CInt(lstTransactionItems.ClientSize.Height / lstTransactionItems.ItemHeight) + 1)
        End If

        lstTransactionItems.EndUpdate()
        lblTransactionTotal.Text = $"TOTAL: ₱{runningTotal:N2}"

        btnConfirmOrder.Enabled = currentTransactionLineItems.Count > 0
        btnClearTransaction.Enabled = currentTransactionLineItems.Count > 0
    End Sub

    Private Sub ClearTransaction()
        currentTransactionLineItems.Clear()
        RefreshTransactionDisplay()
        UpdateScanStatus("Ready to scan items...", Color.White, Color.FromArgb(71, 85, 105))
        txtBarcodeInput.Clear()
        txtBarcodeInput.Focus()
    End Sub

    Private Sub ClearTransaction_Click(sender As Object, e As EventArgs)
        If currentTransactionLineItems.Count > 0 Then
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to clear all items from the current transaction?", "Clear Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                ClearTransaction()
            End If
        End If
    End Sub

    Private Sub ConfirmOrder_Click(sender As Object, e As EventArgs)
        If currentTransactionLineItems.Count = 0 Then
            MessageBox.Show("There are no items in the current order.", "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim finalTotal As Decimal = currentTransactionLineItems.Sum(Function(li) li.LineTotal)
        Dim itemListSummary As String = String.Join(vbCrLf, currentTransactionLineItems.Select(Function(li) $"• {li.Item.Name} x {li.Quantity} = ₱{li.LineTotal:N2}").Take(8))

        If currentTransactionLineItems.Count > 8 Then
            itemListSummary &= vbCrLf & "... and more items."
        End If

        Dim confirmationMessage As String = $"CONFIRM ORDER DETAILS:{vbCrLf}{vbCrLf}" &
                                          $"Items ({currentTransactionLineItems.Count} type(s)):{vbCrLf}" &
                                          itemListSummary & vbCrLf & vbCrLf &
                                          $"TOTAL AMOUNT: ₱{finalTotal:N2}{vbCrLf}{vbCrLf}" &
                                          "This will record the sale and clear the current transaction."

        Dim result As DialogResult = MessageBox.Show(confirmationMessage, "Confirm Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Try
                Dim saleId As Integer = DatabaseHelper.RecordSale(currentTransactionLineItems, finalTotal)
                MessageBox.Show($"✅ ORDER CONFIRMED!{vbCrLf}{vbCrLf}Sale ID: {saleId}{vbCrLf}Total: ₱{finalTotal:N2}{vbCrLf}Date: {DateTime.Now:yyyy-MM-dd HH:mm}", "Order Processed", MessageBoxButtons.OK, MessageBoxIcon.Information)
                ClearTransaction()
            Catch ex As Exception
                MessageBox.Show("Failed to record sale: " & ex.Message, "Sale Recording Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub OpenItemManagement_Click(sender As Object, e As EventArgs)
        Try
            Using itemMgmtForm As New ItemManagementForm()
                itemMgmtForm.ShowDialog(Me)
            End Using
            txtBarcodeInput.Focus()
        Catch ex As Exception
            MessageBox.Show("Error opening Management & Records: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
    End Sub
End Class
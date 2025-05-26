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
    Private btnViewRecords As Button
    Private btnConfirmOrder As Button

    Private currentTransactionLineItems As New List(Of TransactionLineItem)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            DatabaseHelper.InitializeDatabase()
            Me.Text = "Barcode Scanner"
            Me.Size = New Size(780, 700)
            Me.MinimumSize = New Size(700, 600)
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.BackColor = Color.AliceBlue

            CreateScannerControls()
            ClearTransaction()

            txtBarcodeInput.Focus()

        Catch ex As Exception
            MessageBox.Show("Error initializing application: " & ex.Message, "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CreateScannerControls()
        Dim currentY As Integer = 20
        Dim bottomButtonY As Integer

        Dim lblScannerTitle As New Label()
        lblScannerTitle.Text = "Paynawa Coffee Shop"
        lblScannerTitle.Font = New Font("Segoe UI", 18, FontStyle.Bold)
        lblScannerTitle.ForeColor = Color.FromArgb(0, 120, 215)
        lblScannerTitle.Size = New Size(Me.ClientSize.Width - 40, 40)
        lblScannerTitle.Location = New Point(20, currentY)
        lblScannerTitle.TextAlign = ContentAlignment.MiddleCenter
        lblScannerTitle.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(lblScannerTitle)
        currentY += lblScannerTitle.Height + 20

        Dim lblScanInputInstruction As New Label()
        lblScanInputInstruction.Text = "Enter Barcode, Item ID, or Item Name and press Enter:"
        lblScanInputInstruction.Font = New Font("Segoe UI", 10.0F)
        lblScanInputInstruction.Location = New Point(20, currentY)
        lblScanInputInstruction.Size = New Size(Me.ClientSize.Width - 40, 25)
        lblScanInputInstruction.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(lblScanInputInstruction)
        currentY += lblScanInputInstruction.Height + 5

        txtBarcodeInput = New TextBox()
        txtBarcodeInput.Location = New Point(20, currentY)
        txtBarcodeInput.Size = New Size(Me.ClientSize.Width - 40, 34)
        txtBarcodeInput.Font = New Font("Consolas", 14)
        txtBarcodeInput.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        AddHandler txtBarcodeInput.KeyPress, AddressOf BarcodeInput_KeyPress
        Me.Controls.Add(txtBarcodeInput)
        currentY += txtBarcodeInput.Height + 15

        lblCurrentScanStatus = New Label()
        lblCurrentScanStatus.Location = New Point(20, currentY)
        lblCurrentScanStatus.Size = New Size(Me.ClientSize.Width - 40, 40)
        lblCurrentScanStatus.BorderStyle = BorderStyle.FixedSingle
        lblCurrentScanStatus.BackColor = Color.White
        lblCurrentScanStatus.Font = New Font("Segoe UI Semibold", 11.0F)
        lblCurrentScanStatus.ForeColor = Color.Black
        lblCurrentScanStatus.TextAlign = ContentAlignment.MiddleLeft
        lblCurrentScanStatus.Padding = New Padding(8, 0, 0, 0)
        lblCurrentScanStatus.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(lblCurrentScanStatus)
        currentY += lblCurrentScanStatus.Height + 15

        gbTransaction = New GroupBox()
        gbTransaction.Text = "Current Transaction"
        gbTransaction.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        gbTransaction.ForeColor = Color.DarkSlateGray
        gbTransaction.Location = New Point(20, currentY)
        gbTransaction.Size = New Size(Me.ClientSize.Width - 40, Me.ClientSize.Height - currentY - 80)
        gbTransaction.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(gbTransaction)

        Dim gbInternalTopPadding As Integer = 25
        Dim gbCurrentY As Integer = gbInternalTopPadding

        lstTransactionItems = New ListBox()
        lstTransactionItems.Location = New Point(10, gbCurrentY)
        lstTransactionItems.Size = New Size(gbTransaction.ClientSize.Width - 20, gbTransaction.ClientSize.Height - gbCurrentY - 45)
        lstTransactionItems.Font = New Font("Courier New", 10.0F)
        lstTransactionItems.BorderStyle = BorderStyle.Fixed3D
        lstTransactionItems.IntegralHeight = False
        lstTransactionItems.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        gbTransaction.Controls.Add(lstTransactionItems)

        lblTransactionTotal = New Label()
        lblTransactionTotal.Text = "Total: 0.00"
        lblTransactionTotal.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold)
        lblTransactionTotal.ForeColor = Color.DarkBlue
        lblTransactionTotal.Location = New Point(10, lstTransactionItems.Bottom + 5)
        lblTransactionTotal.Size = New Size(gbTransaction.ClientSize.Width - 20, 30)
        lblTransactionTotal.TextAlign = ContentAlignment.MiddleRight
        lblTransactionTotal.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        gbTransaction.Controls.Add(lblTransactionTotal)

        bottomButtonY = gbTransaction.Bottom + 15

        btnOpenItemManagement = New Button()
        btnOpenItemManagement.Text = "⚙️ Manage Items"
        btnOpenItemManagement.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        btnOpenItemManagement.Size = New Size(160, 40)
        btnOpenItemManagement.Location = New Point(20, bottomButtonY)
        btnOpenItemManagement.BackColor = Color.LightSteelBlue
        btnOpenItemManagement.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        AddHandler btnOpenItemManagement.Click, AddressOf OpenItemManagement_Click
        Me.Controls.Add(btnOpenItemManagement)

        btnViewRecords = New Button()
        btnViewRecords.Text = "📊 View Records"
        btnViewRecords.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        btnViewRecords.Size = New Size(160, 40)
        btnViewRecords.Location = New Point(btnOpenItemManagement.Right + 10, bottomButtonY)
        btnViewRecords.BackColor = Color.LightSkyBlue
        btnViewRecords.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        AddHandler btnViewRecords.Click, AddressOf OpenRecordsForm_Click
        Me.Controls.Add(btnViewRecords)

        btnConfirmOrder = New Button()
        btnConfirmOrder.Text = "🛒 Confirm Order"
        btnConfirmOrder.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        btnConfirmOrder.Size = New Size(180, 40)
        btnConfirmOrder.Location = New Point(Me.ClientSize.Width - btnConfirmOrder.Width - 20, bottomButtonY)
        btnConfirmOrder.BackColor = Color.MediumSeaGreen
        btnConfirmOrder.ForeColor = Color.White
        btnConfirmOrder.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        AddHandler btnConfirmOrder.Click, AddressOf ConfirmOrder_Click
        Me.Controls.Add(btnConfirmOrder)
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
                Dim quantityString As String = Interaction.InputBox($"Enter quantity for '{baseItem.Name}':", "Enter Quantity", "1")
                If String.IsNullOrWhiteSpace(quantityString) Then
                    lblCurrentScanStatus.Text = "Quantity input cancelled."
                    lblCurrentScanStatus.BackColor = Color.LightYellow
                    lblCurrentScanStatus.ForeColor = Color.DarkOrange
                    SystemSounds.Asterisk.Play()
                    txtBarcodeInput.Clear()
                    txtBarcodeInput.Focus()
                    Return
                End If
                Dim quantity As Integer
                If Integer.TryParse(quantityString, quantity) AndAlso quantity > 0 Then
                    UpdateTransactionItem(baseItem, quantity)
                    lblCurrentScanStatus.BackColor = Color.FromArgb(220, 255, 220)
                    lblCurrentScanStatus.ForeColor = Color.DarkGreen
                    lblCurrentScanStatus.Text = $"✔️ Updated: {baseItem.Name} (Qty: {quantity}) at ₱{baseItem.Price:F2} each."
                    SystemSounds.Beep.Play()
                Else
                    lblCurrentScanStatus.BackColor = Color.FromArgb(255, 220, 220)
                    lblCurrentScanStatus.ForeColor = Color.DarkRed
                    lblCurrentScanStatus.Text = $"❌ Invalid quantity entered: '{quantityString}'. Must be a positive number."
                    SystemSounds.Hand.Play()
                End If
            Else
                lblCurrentScanStatus.BackColor = Color.FromArgb(255, 220, 220)
                lblCurrentScanStatus.ForeColor = Color.DarkRed
                lblCurrentScanStatus.Text = $"❌ Item Not Found (Input: {userInput})"
                SystemSounds.Hand.Play()
            End If
            txtBarcodeInput.Clear()
            txtBarcodeInput.Focus()
        Catch ex As Exception
            lblCurrentScanStatus.BackColor = Color.FromArgb(255, 200, 200)
            lblCurrentScanStatus.ForeColor = Color.DarkRed
            lblCurrentScanStatus.Text = "🚫 PROCESSING ERROR: " & ex.Message
            SystemSounds.Exclamation.Play()
        End Try
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
        lblTransactionTotal.Text = $"Total: {runningTotal:F2}"
    End Sub

    Private Sub ClearTransaction()
        currentTransactionLineItems.Clear()
        RefreshTransactionDisplay()
        lblCurrentScanStatus.Text = "Scan an item or enter item details..."
        lblCurrentScanStatus.BackColor = Color.White
        lblCurrentScanStatus.ForeColor = Color.Black
        txtBarcodeInput.Clear()
        txtBarcodeInput.Focus()
    End Sub

    Private Sub ConfirmOrder_Click(sender As Object, e As EventArgs)
        If currentTransactionLineItems.Count = 0 Then
            MessageBox.Show("There are no items in the current order.", "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim finalTotal As Decimal = currentTransactionLineItems.Sum(Function(li) li.LineTotal)

        Dim confirmationMessage As String = $"Confirm order with {currentTransactionLineItems.Count} item(s) (Total: ₱{finalTotal:F2})?" & vbCrLf & vbCrLf &
                                            "This will record the sale and clear the current transaction."
        Dim result As DialogResult = MessageBox.Show(confirmationMessage,
                                                    "Confirm Order",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Try
                Dim saleId As Integer = DatabaseHelper.RecordSale(currentTransactionLineItems, finalTotal)
                MessageBox.Show($"Order Confirmed and Recorded!{vbCrLf}Sale ID: {saleId}{vbCrLf}Total: ₱{finalTotal:F2}",
                                "Order Processed", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            MessageBox.Show("Error opening Item Management: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub OpenRecordsForm_Click(sender As Object, e As EventArgs)
        Try
            Using recordsForm As New RecordsForm()
                recordsForm.ShowDialog(Me)
            End Using
            txtBarcodeInput.Focus()
        Catch ex As Exception
            MessageBox.Show("Error opening Records: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Add any cleanup logic here if needed
    End Sub
End Class
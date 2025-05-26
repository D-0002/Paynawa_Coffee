' ItemManagementForm.vb
Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports ZXing
Imports ZXing.Common
Imports System.Linq

Public Class ItemManagementForm
    Private txtItemName As TextBox
    Private txtPrice As TextBox
    Private btnGenerateOrUpdate As Button
    Private btnCancelEdit As Button
    Private lstItems As ListBox
    Private btnEditItem As Button
    Private btnDeleteItem As Button
    Private btnRefreshList As Button
    Private lblManagementFeedback As Label
    Private picGeneratedBarcode As PictureBox
    Private btnCloseManagement As Button

    Private editingItemId As Integer = -1

    Private Sub ItemManagementForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Me.Text = "Item Database Management"
            Me.Size = New Size(750, 700)
            Me.MinimumSize = New Size(700, 600)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.BackColor = Color.WhiteSmoke
            Me.FormBorderStyle = FormBorderStyle.SizableToolWindow

            CreateManagementControls()
            LoadAllItems()
            txtItemName.Focus()

        Catch ex As Exception
            MessageBox.Show("Error initializing Item Management: " & ex.Message, "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close() ' Close if initialization fails
        End Try
    End Sub

    Private Sub CreateManagementControls()
        Dim currentYManage As Integer = 20

        Dim lblTitle As New Label()
        lblTitle.Text = "Item Database Management"
        lblTitle.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        lblTitle.ForeColor = Color.DarkSlateGray
        lblTitle.Location = New Point(15, currentYManage)
        lblTitle.Size = New Size(Me.ClientSize.Width - 30, 30)
        lblTitle.TextAlign = ContentAlignment.MiddleCenter
        lblTitle.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(lblTitle)
        currentYManage += lblTitle.Height + 20

        Dim pnlAddEdit As New Panel()
        pnlAddEdit.Location = New Point(15, currentYManage)
        pnlAddEdit.Size = New Size(Me.ClientSize.Width - 30, 280)
        pnlAddEdit.BorderStyle = BorderStyle.FixedSingle
        pnlAddEdit.Padding = New Padding(10)
        pnlAddEdit.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(pnlAddEdit)

        Dim innerY As Integer = 10

        Dim lblSection1 As New Label()
        lblSection1.Text = "Add New Item / Edit Existing Item:"
        lblSection1.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblSection1.Location = New Point(10, innerY)
        lblSection1.AutoSize = True
        pnlAddEdit.Controls.Add(lblSection1)
        innerY += lblSection1.Height + 10

        Dim lblItemNameLabel As New Label()
        lblItemNameLabel.Text = "Item Name:"
        lblItemNameLabel.Location = New Point(10, innerY + 3)
        lblItemNameLabel.AutoSize = True
        lblItemNameLabel.Font = New Font("Segoe UI", 9.5F)
        pnlAddEdit.Controls.Add(lblItemNameLabel)

        txtItemName = New TextBox()
        txtItemName.Location = New Point(100, innerY)
        txtItemName.Size = New Size(pnlAddEdit.Width - 115, 28)
        txtItemName.Font = New Font("Segoe UI", 9.5F)
        txtItemName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        pnlAddEdit.Controls.Add(txtItemName)
        innerY += txtItemName.Height + 8

        Dim lblPriceLabel As New Label()
        lblPriceLabel.Text = "Price (₱):"
        lblPriceLabel.Location = New Point(10, innerY + 3)
        lblPriceLabel.AutoSize = True
        lblPriceLabel.Font = New Font("Segoe UI", 9.5F)
        pnlAddEdit.Controls.Add(lblPriceLabel)

        txtPrice = New TextBox()
        txtPrice.Location = New Point(100, innerY)
        txtPrice.Size = New Size(130, 28)
        txtPrice.Font = New Font("Segoe UI", 9.5F)
        pnlAddEdit.Controls.Add(txtPrice)
        innerY += txtPrice.Height + 12

        btnGenerateOrUpdate = New Button()
        btnGenerateOrUpdate.Text = "Add & Generate"
        btnGenerateOrUpdate.Size = New Size(140, 32)
        btnGenerateOrUpdate.Location = New Point(10, innerY)
        btnGenerateOrUpdate.BackColor = Color.LightGreen
        btnGenerateOrUpdate.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        AddHandler btnGenerateOrUpdate.Click, AddressOf GenerateOrUpdateBarcode_Click
        pnlAddEdit.Controls.Add(btnGenerateOrUpdate)

        btnCancelEdit = New Button()
        btnCancelEdit.Text = "❌ Cancel Edit"
        btnCancelEdit.Size = New Size(120, 32)
        btnCancelEdit.Location = New Point(btnGenerateOrUpdate.Right + 10, innerY)
        btnCancelEdit.BackColor = Color.Orange
        btnCancelEdit.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        btnCancelEdit.Visible = False
        AddHandler btnCancelEdit.Click, AddressOf CancelEditMode_Click
        pnlAddEdit.Controls.Add(btnCancelEdit)
        innerY += btnGenerateOrUpdate.Height + 10

        lblManagementFeedback = New Label()
        lblManagementFeedback.Location = New Point(10, innerY)
        lblManagementFeedback.Size = New Size(pnlAddEdit.Width - 20, 45)
        lblManagementFeedback.BorderStyle = BorderStyle.FixedSingle
        lblManagementFeedback.BackColor = Color.White
        lblManagementFeedback.Font = New Font("Segoe UI", 9.0F)
        lblManagementFeedback.Text = "Enter item details above or select an item from the list."
        lblManagementFeedback.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        pnlAddEdit.Controls.Add(lblManagementFeedback)
        innerY += lblManagementFeedback.Height + 5

        picGeneratedBarcode = New PictureBox()
        picGeneratedBarcode.Location = New Point(10, innerY)
        picGeneratedBarcode.Size = New Size(pnlAddEdit.Width - 20, pnlAddEdit.Height - innerY - 10)
        picGeneratedBarcode.BorderStyle = BorderStyle.FixedSingle
        picGeneratedBarcode.BackColor = Color.White
        picGeneratedBarcode.SizeMode = PictureBoxSizeMode.Zoom
        picGeneratedBarcode.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        pnlAddEdit.Controls.Add(picGeneratedBarcode)

        currentYManage += pnlAddEdit.Height + 15

        Dim lblSection2 As New Label()
        lblSection2.Text = "Saved Items List:"
        lblSection2.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblSection2.Location = New Point(15, currentYManage)
        lblSection2.AutoSize = True
        Me.Controls.Add(lblSection2)

        btnRefreshList = New Button()
        btnRefreshList.Text = "🔄 Refresh"
        btnRefreshList.Size = New Size(100, 28)
        btnRefreshList.Location = New Point(Me.ClientSize.Width - 15 - btnRefreshList.Width, currentYManage - 2)
        btnRefreshList.Font = New Font("Segoe UI", 9.0F)
        btnRefreshList.BackColor = Color.LightYellow
        btnRefreshList.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        AddHandler btnRefreshList.Click, AddressOf LoadAllItems_Click
        Me.Controls.Add(btnRefreshList)
        currentYManage += lblSection2.Height + 8

        lstItems = New ListBox()
        lstItems.Location = New Point(15, currentYManage)
        lstItems.Size = New Size(Me.ClientSize.Width - 30, Me.ClientSize.Height - currentYManage - 90)
        lstItems.Font = New Font("Segoe UI", 9.5F)
        lstItems.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        AddHandler lstItems.DoubleClick, AddressOf ShowSelectedItemBarcode_DoubleClick
        Me.Controls.Add(lstItems)

        currentYManage = lstItems.Bottom + 10

        btnEditItem = New Button()
        btnEditItem.Text = "✏️ Edit Selected"
        btnEditItem.Size = New Size(140, 35)
        btnEditItem.Location = New Point(15, currentYManage)
        btnEditItem.BackColor = Color.CornflowerBlue
        btnEditItem.ForeColor = Color.White
        btnEditItem.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        btnEditItem.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        AddHandler btnEditItem.Click, AddressOf EditSelectedItem_Click
        Me.Controls.Add(btnEditItem)

        btnDeleteItem = New Button()
        btnDeleteItem.Text = "🗑️ Delete Selected"
        btnDeleteItem.Size = New Size(150, 35)
        btnDeleteItem.Location = New Point(btnEditItem.Right + 10, currentYManage)
        btnDeleteItem.BackColor = Color.LightCoral
        btnDeleteItem.ForeColor = Color.White
        btnDeleteItem.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        btnDeleteItem.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        AddHandler btnDeleteItem.Click, AddressOf DeleteSelectedItem_Click
        Me.Controls.Add(btnDeleteItem)

        btnCloseManagement = New Button()
        btnCloseManagement.Text = "❌ Close Management"
        btnCloseManagement.Size = New Size(180, 35)
        btnCloseManagement.Location = New Point(Me.ClientSize.Width - 15 - btnCloseManagement.Width, currentYManage)
        btnCloseManagement.BackColor = Color.IndianRed
        btnCloseManagement.ForeColor = Color.White
        btnCloseManagement.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        btnCloseManagement.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        AddHandler btnCloseManagement.Click, Sub(s, ev) Me.Close()
        Me.Controls.Add(btnCloseManagement)
    End Sub

#Region "Item Management Logic"

    Private Sub LoadAllItems_Click(sender As Object, e As EventArgs)
        LoadAllItems()
    End Sub

    Private Sub GenerateOrUpdateBarcode_Click(sender As Object, e As EventArgs)
        Try
            If String.IsNullOrWhiteSpace(txtItemName.Text) Then
                MessageBox.Show("Please enter an item name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtItemName.Focus()
                Return
            End If

            Dim price As Decimal
            If Not Decimal.TryParse(txtPrice.Text, price) OrElse price <= 0 Then
                MessageBox.Show("Please enter a valid price (greater than 0).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtPrice.Focus()
                Return
            End If

            Dim itemName As String = txtItemName.Text.Trim()

            If editingItemId <> -1 Then
                Dim itemToUpdate As New BarcodeItem With {
                    .Id = editingItemId,
                    .Name = itemName,
                    .Price = price
                }
                itemToUpdate.BarcodeData = itemName & "-" & price.ToString("F2")

                If DatabaseHelper.UpdateItem(itemToUpdate) Then
                    lblManagementFeedback.Text = "✅ Item updated successfully!" & vbCrLf &
                                   $"📦 Name: {itemToUpdate.Name}, 💰 Price: ₱{itemToUpdate.Price:F2}"
                    lblManagementFeedback.ForeColor = Color.DarkBlue
                    GenerateAndDisplayManagementBarcode(itemToUpdate.BarcodeData)
                    ExitEditMode()
                Else
                    MessageBox.Show("Failed to update item. The new name/price might conflict with an existing item.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    lblManagementFeedback.Text = "❌ Error updating item."
                    lblManagementFeedback.ForeColor = Color.Red
                End If
            Else ' Add new item
                Dim newItem As BarcodeItem = DatabaseHelper.AddItem(itemName, price)
                lblManagementFeedback.Text = "✅ Barcode generated and saved!" & vbCrLf &
                               $"📦 Name: {newItem.Name}, 💰 Price: ₱{newItem.Price:F2}"
                lblManagementFeedback.ForeColor = Color.DarkGreen
                GenerateAndDisplayManagementBarcode(newItem.BarcodeData)
                txtItemName.Clear()
                txtPrice.Clear()
                txtItemName.Focus()
                LoadAllItems()
            End If

        Catch ex As Exception
            MessageBox.Show("Operation failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lblManagementFeedback.Text = "❌ Error: " & ex.Message
            lblManagementFeedback.ForeColor = Color.Red
        End Try
    End Sub

    Private Sub GenerateAndDisplayManagementBarcode(barcodeData As String)
        If String.IsNullOrWhiteSpace(barcodeData) Then
            If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
            picGeneratedBarcode.Image = Nothing
            Return
        End If
        Try
            Dim writer As New BarcodeWriter()
            writer.Format = BarcodeFormat.CODE_128
            writer.Options = New EncodingOptions() With {
                .Width = Math.Max(100, picGeneratedBarcode.Width - 20),
                .Height = Math.Max(50, picGeneratedBarcode.Height - 20),
                .Margin = 10,
                .PureBarcode = False
            }
            Dim barcodeImage As Bitmap = writer.Write(barcodeData)
            If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
            picGeneratedBarcode.Image = barcodeImage
        Catch ex As Exception
            If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
            picGeneratedBarcode.Image = Nothing
            lblManagementFeedback.Text &= vbCrLf & " (Barcode image generation failed)"
            System.Diagnostics.Debug.WriteLine("Management barcode image generation failed: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadAllItems()
        Try
            lstItems.Items.Clear()
            Dim items As List(Of BarcodeItem) = DatabaseHelper.GetAllItems()
            For Each item In items
                lstItems.Items.Add($"#{item.Id} - {item.Name} - ₱{item.Price:F2} ({item.DateCreated:MM/dd/yy HH:mm})")
            Next
            If items.Count = 0 Then
                lstItems.Items.Add("No items saved yet. Add items using the panel above.")
            End If
            lstItems.ClearSelected()
            If editingItemId = -1 Then ' Only reset if not actively in edit mode
                lblManagementFeedback.Text = "Enter item details above or select an item from the list."
                lblManagementFeedback.ForeColor = SystemColors.ControlText
                If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
                picGeneratedBarcode.Image = Nothing
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading items: " & ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetSelectedItemIdFromList() As Integer
        If lstItems.SelectedIndex = -1 OrElse Not lstItems.SelectedItem.ToString().StartsWith("#") Then
            Return -1
        End If
        Try
            Dim selectedText As String = lstItems.SelectedItem.ToString()
            Dim idStart As Integer = selectedText.IndexOf("#") + 1
            Dim idEnd As Integer = selectedText.IndexOf(" -")
            If idStart > 0 AndAlso idEnd > idStart Then
                Return Integer.Parse(selectedText.Substring(idStart, idEnd - idStart))
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error parsing item ID: " & ex.Message)
        End Try
        Return -1
    End Function

    Private Sub EditSelectedItem_Click(sender As Object, e As EventArgs)
        Dim selectedId As Integer = GetSelectedItemIdFromList()
        If selectedId = -1 Then
            MessageBox.Show("Please select an item from the list to edit.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim itemToEdit As BarcodeItem = DatabaseHelper.GetAllItems().FirstOrDefault(Function(i) i.Id = selectedId)
        If itemToEdit IsNot Nothing Then
            editingItemId = itemToEdit.Id
            txtItemName.Text = itemToEdit.Name
            txtPrice.Text = itemToEdit.Price.ToString("F2")

            lblManagementFeedback.Text = $"✏️ Editing item: #{itemToEdit.Id} - {itemToEdit.Name}"
            lblManagementFeedback.ForeColor = Color.DarkOrange
            GenerateAndDisplayManagementBarcode(itemToEdit.BarcodeData)

            btnGenerateOrUpdate.Text = "💾 Save Changes"
            btnCancelEdit.Visible = True

            lstItems.Enabled = False
            btnEditItem.Enabled = False
            btnDeleteItem.Enabled = False
            btnRefreshList.Enabled = False
            txtItemName.Focus()
        Else
            MessageBox.Show("Could not find the selected item details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub DeleteSelectedItem_Click(sender As Object, e As EventArgs)
        Dim selectedId As Integer = GetSelectedItemIdFromList()
        If selectedId = -1 Then
            MessageBox.Show("Please select an item from the list to delete.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim itemToDelete As BarcodeItem = DatabaseHelper.GetAllItems().FirstOrDefault(Function(i) i.Id = selectedId)
        If itemToDelete IsNot Nothing Then
            Dim confirmResult As DialogResult = MessageBox.Show($"Are you sure you want to delete '{itemToDelete.Name}' (ID: {itemToDelete.Id})?",
                                                                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If confirmResult = DialogResult.Yes Then
                Try
                    If DatabaseHelper.DeleteItem(selectedId) Then
                        lblManagementFeedback.Text = $"🗑️ Item '{itemToDelete.Name}' deleted successfully."
                        lblManagementFeedback.ForeColor = Color.DarkGreen
                        If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
                        picGeneratedBarcode.Image = Nothing
                        LoadAllItems()
                    Else
                        MessageBox.Show("Failed to delete the item.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error deleting item: " & ex.Message, "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        Else
            MessageBox.Show("Could not find item details for deletion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub CancelEditMode_Click(sender As Object, e As EventArgs)
        ExitEditMode()
        lblManagementFeedback.Text = "Edit cancelled. Ready for new item or selection."
        lblManagementFeedback.ForeColor = SystemColors.ControlText
    End Sub

    Private Sub ExitEditMode()
        editingItemId = -1
        txtItemName.Clear()
        txtPrice.Clear()
        If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
        picGeneratedBarcode.Image = Nothing

        btnGenerateOrUpdate.Text = "Add & Generate"
        btnCancelEdit.Visible = False

        lstItems.Enabled = True
        btnEditItem.Enabled = True
        btnDeleteItem.Enabled = True
        btnRefreshList.Enabled = True

        lblManagementFeedback.Text = "Enter item details above or select an item from the list."
        lblManagementFeedback.ForeColor = SystemColors.ControlText
        txtItemName.Focus()
        LoadAllItems()
    End Sub

    Private Sub ShowSelectedItemBarcode_DoubleClick(sender As Object, e As EventArgs)
        If editingItemId <> -1 Then Return

        Dim selectedId As Integer = GetSelectedItemIdFromList()
        If selectedId = -1 Then
            If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
            picGeneratedBarcode.Image = Nothing
            lblManagementFeedback.Text = "Select an item from the list to see its details."
            lblManagementFeedback.ForeColor = SystemColors.ControlText
            Return
        End If

        Try
            Dim selectedItem As BarcodeItem = DatabaseHelper.GetAllItems().FirstOrDefault(Function(i) i.Id = selectedId)

            If selectedItem IsNot Nothing Then
                GenerateAndDisplayManagementBarcode(selectedItem.BarcodeData)
                lblManagementFeedback.Text = $"📦 Viewing: {selectedItem.Name} | 💰 Price: ₱{selectedItem.Price:F2} | 🔢 Barcode: {selectedItem.BarcodeData}"
                lblManagementFeedback.ForeColor = Color.DarkBlue
            End If
        Catch ex As Exception
            MessageBox.Show("Error showing barcode: " & ex.Message, "Display Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

    Private Sub ItemManagementForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If picGeneratedBarcode.Image IsNot Nothing Then
            picGeneratedBarcode.Image.Dispose()
            picGeneratedBarcode.Image = Nothing
        End If
    End Sub
End Class
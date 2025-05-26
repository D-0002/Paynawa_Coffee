Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports ZXing
Imports ZXing.Common
Imports System.Linq
Imports System.Collections.Generic

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

    Private lstSalesHeaders As ListBox
    Private lstSalesDetails As ListBox
    Private lblSalesListTitle As Label
    Private lblSaleDetailsListTitle As Label
    Private btnRefreshSales As Button

    Private tcMain As TabControl
    Private tpItemManagement As TabPage
    Private tpSalesRecords As TabPage
    Private btnCloseMainForm As Button

    Private editingItemId As Integer = -1

    Private Sub ItemManagementForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Me.Text = "Item & Sales Management - Nike Shoe Shop"
            Me.Size = New Size(820, 750)
            Me.MinimumSize = New Size(780, 680)
            Me.StartPosition = FormStartPosition.CenterParent
            Me.BackColor = Color.FromArgb(240, 240, 240)
            Me.FormBorderStyle = FormBorderStyle.SizableToolWindow

            CreateMainLayoutAndTabs()
            CreateItemManagementTabControls(tpItemManagement)
            CreateSalesRecordsTabControls(tpSalesRecords)

            LoadAllItems()
            LoadSalesHeaders()

            If tcMain.SelectedTab Is tpItemManagement AndAlso txtItemName IsNot Nothing Then
                txtItemName.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show("Error initializing Management Form: " & ex.Message, "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        End Try
    End Sub

    Private Sub StyleFlatButton(button As Button, baseColor As Color, Optional useWhiteText As Boolean = False)
        button.FlatStyle = FlatStyle.Flat
        button.FlatAppearance.BorderSize = 1
        button.FlatAppearance.BorderColor = ControlPaint.Dark(baseColor, 0.2F)
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(baseColor, 0.1F)
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(baseColor, 0.1F)
        If useWhiteText Then
            button.ForeColor = Color.White
        Else
            button.ForeColor = Color.Black
        End If
    End Sub

    Private Sub CreateMainLayoutAndTabs()
        tcMain = New TabControl()
        tcMain.Location = New Point(10, 10)
        tcMain.Size = New Size(Me.ClientSize.Width - 20, Me.ClientSize.Height - 70)
        tcMain.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        tcMain.Font = New Font("Segoe UI", 10.0F)

        tpItemManagement = New TabPage("🔧 Item Management")
        tpItemManagement.Padding = New Padding(10)
        tpItemManagement.BackColor = Color.FromArgb(240, 240, 240)

        tpSalesRecords = New TabPage("📊 Sales Records")
        tpSalesRecords.Padding = New Padding(10)
        tpSalesRecords.BackColor = Color.WhiteSmoke

        tcMain.TabPages.Add(tpItemManagement)
        tcMain.TabPages.Add(tpSalesRecords)
        Me.Controls.Add(tcMain)

        btnCloseMainForm = New Button()
        btnCloseMainForm.Text = "❌ Close"
        btnCloseMainForm.Size = New Size(150, 40)
        btnCloseMainForm.Location = New Point(Me.ClientSize.Width - 15 - btnCloseMainForm.Width, Me.ClientSize.Height - 15 - btnCloseMainForm.Height)
        btnCloseMainForm.BackColor = Color.FromArgb(205, 92, 92)
        btnCloseMainForm.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        StyleFlatButton(btnCloseMainForm, btnCloseMainForm.BackColor, True)
        btnCloseMainForm.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        AddHandler btnCloseMainForm.Click, Sub(s As Object, ev As EventArgs) Me.Close()
        Me.Controls.Add(btnCloseMainForm)
    End Sub

    Private Sub CreateItemManagementTabControls(parentTabPage As TabPage)
        Dim currentYManage As Integer = 10

        Dim lblTitle As New Label()
        lblTitle.Text = "Item Database Management"
        lblTitle.Font = New Font("Segoe UI", 16, FontStyle.Bold)
        lblTitle.ForeColor = Color.DarkSlateGray
        lblTitle.Location = New Point(5, currentYManage)
        lblTitle.Size = New Size(parentTabPage.ClientSize.Width - 10, 35)
        lblTitle.TextAlign = ContentAlignment.MiddleCenter
        lblTitle.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        parentTabPage.Controls.Add(lblTitle)
        currentYManage += lblTitle.Height + 20

        Dim pnlAddEdit As New Panel()
        pnlAddEdit.Location = New Point(5, currentYManage)
        pnlAddEdit.Size = New Size(parentTabPage.ClientSize.Width - 10, 300)
        pnlAddEdit.BorderStyle = BorderStyle.FixedSingle
        pnlAddEdit.BackColor = Color.White
        pnlAddEdit.Padding = New Padding(15)
        pnlAddEdit.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        parentTabPage.Controls.Add(pnlAddEdit)

        Dim innerY As Integer = 15

        Dim lblSection1 As New Label()
        lblSection1.Text = "Add New Item / Edit Existing Item:"
        lblSection1.Font = New Font("Segoe UI", 10.5F, FontStyle.Bold)
        lblSection1.Location = New Point(10, innerY)
        lblSection1.AutoSize = True
        pnlAddEdit.Controls.Add(lblSection1)
        innerY += lblSection1.Height + 15

        Dim lblItemNameLabel As New Label()
        lblItemNameLabel.Text = "Item Name:"
        lblItemNameLabel.Location = New Point(10, innerY + 5)
        lblItemNameLabel.AutoSize = True
        lblItemNameLabel.Font = New Font("Segoe UI", 10.0F)
        pnlAddEdit.Controls.Add(lblItemNameLabel)

        txtItemName = New TextBox()
        txtItemName.Location = New Point(120, innerY)
        txtItemName.Size = New Size(pnlAddEdit.Width - 135, 30)
        txtItemName.Font = New Font("Segoe UI", 10.0F)
        txtItemName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        pnlAddEdit.Controls.Add(txtItemName)
        innerY += txtItemName.Height + 10

        Dim lblPriceLabel As New Label()
        lblPriceLabel.Text = "Price (₱):"
        lblPriceLabel.Location = New Point(10, innerY + 5)
        lblPriceLabel.AutoSize = True
        lblPriceLabel.Font = New Font("Segoe UI", 10.0F)
        pnlAddEdit.Controls.Add(lblPriceLabel)

        txtPrice = New TextBox()
        txtPrice.Location = New Point(120, innerY)
        txtPrice.Size = New Size(150, 30)
        txtPrice.Font = New Font("Segoe UI", 10.0F)
        pnlAddEdit.Controls.Add(txtPrice)
        innerY += txtPrice.Height + 15

        btnGenerateOrUpdate = New Button()
        btnGenerateOrUpdate.Text = "Add & Generate"
        btnGenerateOrUpdate.Size = New Size(150, 35)
        btnGenerateOrUpdate.Location = New Point(10, innerY)
        btnGenerateOrUpdate.BackColor = Color.FromArgb(144, 238, 144)
        btnGenerateOrUpdate.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        StyleFlatButton(btnGenerateOrUpdate, btnGenerateOrUpdate.BackColor)
        AddHandler btnGenerateOrUpdate.Click, AddressOf GenerateOrUpdateBarcode_Click
        pnlAddEdit.Controls.Add(btnGenerateOrUpdate)

        btnCancelEdit = New Button()
        btnCancelEdit.Text = "❌ Cancel Edit"
        btnCancelEdit.Size = New Size(130, 35)
        btnCancelEdit.Location = New Point(btnGenerateOrUpdate.Right + 10, innerY)
        btnCancelEdit.BackColor = Color.FromArgb(255, 192, 128)
        btnCancelEdit.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        StyleFlatButton(btnCancelEdit, btnCancelEdit.BackColor)
        btnCancelEdit.Visible = False
        AddHandler btnCancelEdit.Click, AddressOf CancelEditMode_Click
        pnlAddEdit.Controls.Add(btnCancelEdit)
        innerY += btnGenerateOrUpdate.Height + 12

        lblManagementFeedback = New Label()
        lblManagementFeedback.Location = New Point(10, innerY)
        lblManagementFeedback.Size = New Size(pnlAddEdit.Width - 20, 50)
        lblManagementFeedback.BorderStyle = BorderStyle.FixedSingle
        lblManagementFeedback.BackColor = Color.WhiteSmoke
        lblManagementFeedback.Font = New Font("Segoe UI", 9.5F)
        lblManagementFeedback.TextAlign = ContentAlignment.MiddleLeft
        lblManagementFeedback.Padding = New Padding(5)
        lblManagementFeedback.Text = "Enter item details above or select an item from the list."
        lblManagementFeedback.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        pnlAddEdit.Controls.Add(lblManagementFeedback)
        innerY += lblManagementFeedback.Height + 8

        picGeneratedBarcode = New PictureBox()
        picGeneratedBarcode.Location = New Point(10, innerY)
        picGeneratedBarcode.Size = New Size(pnlAddEdit.Width - 20, pnlAddEdit.Height - innerY - 10)
        picGeneratedBarcode.BorderStyle = BorderStyle.FixedSingle
        picGeneratedBarcode.BackColor = Color.White
        picGeneratedBarcode.SizeMode = PictureBoxSizeMode.CenterImage
        picGeneratedBarcode.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        pnlAddEdit.Controls.Add(picGeneratedBarcode)

        currentYManage += pnlAddEdit.Height + 20

        Dim lblSection2 As New Label()
        lblSection2.Text = "Saved Items List:"
        lblSection2.Font = New Font("Segoe UI", 10.5F, FontStyle.Bold)
        lblSection2.Location = New Point(5, currentYManage)
        lblSection2.AutoSize = True
        parentTabPage.Controls.Add(lblSection2)

        btnRefreshList = New Button()
        btnRefreshList.Text = "🔄 Refresh"
        btnRefreshList.Size = New Size(110, 30)
        btnRefreshList.Location = New Point(parentTabPage.ClientSize.Width - 5 - btnRefreshList.Width, currentYManage - 3)
        btnRefreshList.Font = New Font("Segoe UI", 9.5F)
        btnRefreshList.BackColor = Color.FromArgb(255, 255, 224)
        StyleFlatButton(btnRefreshList, btnRefreshList.BackColor) ' Default for useWhiteText is False
        btnRefreshList.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        AddHandler btnRefreshList.Click, AddressOf LoadAllItems_Click
        parentTabPage.Controls.Add(btnRefreshList)
        currentYManage += lblSection2.Height + 10

        lstItems = New ListBox()
        lstItems.Location = New Point(5, currentYManage)
        lstItems.Size = New Size(parentTabPage.ClientSize.Width - 10, parentTabPage.ClientSize.Height - currentYManage - 65)
        lstItems.Font = New Font("Segoe UI", 10.0F)
        lstItems.BackColor = Color.White
        lstItems.IntegralHeight = False
        lstItems.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        AddHandler lstItems.DoubleClick, AddressOf ShowSelectedItemBarcode_DoubleClick
        parentTabPage.Controls.Add(lstItems)

        currentYManage = lstItems.Bottom + 10

        btnEditItem = New Button()
        btnEditItem.Text = "✏️ Edit Selected"
        btnEditItem.Size = New Size(150, 40)
        btnEditItem.Location = New Point(5, currentYManage)
        btnEditItem.BackColor = Color.FromArgb(100, 149, 237)
        btnEditItem.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        StyleFlatButton(btnEditItem, btnEditItem.BackColor, True)
        btnEditItem.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        AddHandler btnEditItem.Click, AddressOf EditSelectedItem_Click
        parentTabPage.Controls.Add(btnEditItem)

        btnDeleteItem = New Button()
        btnDeleteItem.Text = "🗑️ Delete Selected"
        btnDeleteItem.Size = New Size(160, 40)
        btnDeleteItem.Location = New Point(btnEditItem.Right + 10, currentYManage)
        btnDeleteItem.BackColor = Color.FromArgb(240, 128, 128)
        btnDeleteItem.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        StyleFlatButton(btnDeleteItem, btnDeleteItem.BackColor, True)
        btnDeleteItem.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        AddHandler btnDeleteItem.Click, AddressOf DeleteSelectedItem_Click
        parentTabPage.Controls.Add(btnDeleteItem)
    End Sub

    Private Sub CreateSalesRecordsTabControls(parentTabPage As TabPage)
        Dim currentY As Integer = 10

        lblSalesListTitle = New Label()
        lblSalesListTitle.Text = "All Sales Transactions (Newest First):"
        lblSalesListTitle.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        lblSalesListTitle.Location = New Point(5, currentY)
        lblSalesListTitle.AutoSize = True
        parentTabPage.Controls.Add(lblSalesListTitle)

        btnRefreshSales = New Button()
        btnRefreshSales.Text = "🔄 Refresh Sales"
        btnRefreshSales.Size = New Size(140, 28)
        btnRefreshSales.Location = New Point(parentTabPage.ClientSize.Width - 5 - btnRefreshSales.Width, currentY - 2)
        btnRefreshSales.Font = New Font("Segoe UI", 9.0F)
        btnRefreshSales.BackColor = Color.LightYellow
        StyleFlatButton(btnRefreshSales, btnRefreshSales.BackColor) ' Default for useWhiteText is False
        btnRefreshSales.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        AddHandler btnRefreshSales.Click, AddressOf RefreshSalesButton_Click
        parentTabPage.Controls.Add(btnRefreshSales)
        currentY += lblSalesListTitle.Height + 8

        lstSalesHeaders = New ListBox()
        lstSalesHeaders.Location = New Point(5, currentY)
        lstSalesHeaders.Size = New Size(parentTabPage.ClientSize.Width - 10, 180)
        lstSalesHeaders.Font = New Font("Segoe UI", 9.5F)
        lstSalesHeaders.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        AddHandler lstSalesHeaders.SelectedIndexChanged, AddressOf SalesHeader_SelectedIndexChanged
        parentTabPage.Controls.Add(lstSalesHeaders)
        currentY += lstSalesHeaders.Height + 15

        lblSaleDetailsListTitle = New Label()
        lblSaleDetailsListTitle.Text = "Details for Selected Sale:"
        lblSaleDetailsListTitle.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        lblSaleDetailsListTitle.Location = New Point(5, currentY)
        lblSaleDetailsListTitle.AutoSize = True
        parentTabPage.Controls.Add(lblSaleDetailsListTitle)
        currentY += lblSaleDetailsListTitle.Height + 8

        lstSalesDetails = New ListBox()
        lstSalesDetails.Location = New Point(5, currentY)
        lstSalesDetails.Font = New Font("Courier New", 10.0F)
        lstSalesDetails.Size = New Size(parentTabPage.ClientSize.Width - 10, parentTabPage.ClientSize.Height - currentY - 10)
        lstSalesDetails.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        parentTabPage.Controls.Add(lstSalesDetails)
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
                                   $"ID: {itemToUpdate.Id}, Name: {itemToUpdate.Name}, Price: ₱{itemToUpdate.Price:N2}"
                    lblManagementFeedback.ForeColor = Color.DarkGreen
                    GenerateAndDisplayManagementBarcode(itemToUpdate.BarcodeData)
                    ExitEditMode()
                Else
                    MessageBox.Show("Failed to update item. The new name/price might conflict with an existing item.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    lblManagementFeedback.Text = "❌ Error updating item. Check for conflicts."
                    lblManagementFeedback.ForeColor = Color.Red
                End If
            Else ' Add new item
                Dim newItem As BarcodeItem = DatabaseHelper.AddItem(itemName, price)
                lblManagementFeedback.Text = "✅ Barcode generated and saved!" & vbCrLf &
                               $"ID: {newItem.Id}, Name: {newItem.Name}, Price: ₱{newItem.Price:N2}"
                lblManagementFeedback.ForeColor = Color.DarkGreen
                GenerateAndDisplayManagementBarcode(newItem.BarcodeData)
                txtItemName.Clear()
                txtPrice.Clear()
                txtItemName.Focus()
                LoadAllItems()
            End If

        Catch exSQL As MySql.Data.MySqlClient.MySqlException
            If exSQL.Number = 1062 Then
                MessageBox.Show($"Operation failed: An item with similar name/price resulting in the same barcode data already exists.{vbCrLf}(MySQL Error {exSQL.Number})", "Duplicate Item Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                MessageBox.Show("Database operation failed: " & exSQL.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            lblManagementFeedback.Text = "❌ DB Error: " & exSQL.Message.Split(vbCrLf)(0)
            lblManagementFeedback.ForeColor = Color.Red
        Catch ex As Exception
            MessageBox.Show("Operation failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lblManagementFeedback.Text = "❌ Error: " & ex.Message.Split(vbCrLf)(0)
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
            Dim barcodeWidth As Integer = Math.Max(100, picGeneratedBarcode.ClientSize.Width - 20)
            Dim barcodeHeight As Integer = Math.Max(50, picGeneratedBarcode.ClientSize.Height - 20)

            If picGeneratedBarcode.ClientSize.Height < 60 Then
                barcodeHeight = Math.Max(30, picGeneratedBarcode.ClientSize.Height - 10)
            End If

            writer.Options = New EncodingOptions() With {
                .Width = barcodeWidth,
                .Height = barcodeHeight,
                .Margin = 5,
                .PureBarcode = (barcodeHeight < 40)
            }
            Dim barcodeImage As Bitmap = writer.Write(barcodeData)
            If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
            picGeneratedBarcode.Image = barcodeImage
        Catch ex As Exception
            If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
            picGeneratedBarcode.Image = Nothing
            lblManagementFeedback.Text &= vbCrLf & " (Barcode image gen failed)"
            System.Diagnostics.Debug.WriteLine("Management barcode image generation failed: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadAllItems()
        Try
            Dim currentSelectedId As Integer = GetSelectedItemIdFromList()

            lstItems.Items.Clear()
            Dim items As List(Of BarcodeItem) = DatabaseHelper.GetAllItems()
            For Each item In items
                lstItems.Items.Add($"#{item.Id} - {item.Name} - ₱{item.Price:N2} (Created: {item.DateCreated:MM/dd/yy HH:mm})")
            Next
            If items.Count = 0 Then
                lstItems.Items.Add("No items saved yet. Add items using the panel above.")
            End If

            If editingItemId = -1 Then
                If lblManagementFeedback IsNot Nothing Then
                    lblManagementFeedback.Text = "Enter item details above or select an item from the list."
                    lblManagementFeedback.ForeColor = SystemColors.ControlText
                End If
                If picGeneratedBarcode IsNot Nothing AndAlso picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
                If picGeneratedBarcode IsNot Nothing Then picGeneratedBarcode.Image = Nothing
                If lstItems IsNot Nothing Then lstItems.ClearSelected()
            Else
                Dim itemToReselect = items.FirstOrDefault(Function(i) i.Id = editingItemId)
                If itemToReselect IsNot Nothing Then
                    For i As Integer = 0 To lstItems.Items.Count - 1
                        If lstItems.Items(i).ToString().StartsWith($"#{editingItemId} ") Then
                            lstItems.SelectedIndex = i
                            Exit For
                        End If
                    Next
                Else
                    ExitEditMode()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading items: " & ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetSelectedItemIdFromList() As Integer
        If lstItems.SelectedIndex = -1 OrElse lstItems.SelectedItem Is Nothing OrElse Not lstItems.SelectedItem.ToString().StartsWith("#") Then
            Return -1
        End If
        Try
            Dim selectedText As String = lstItems.SelectedItem.ToString()
            Dim idStart As Integer = selectedText.IndexOf("#") + 1
            Dim idEnd As Integer = selectedText.IndexOf(" -", idStart)
            If idStart > 0 AndAlso idEnd > idStart Then
                Return Integer.Parse(selectedText.Substring(idStart, idEnd - idStart))
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error parsing item ID from list: " & ex.Message)
        End Try
        Return -1
    End Function

    Private Sub EditSelectedItem_Click(sender As Object, e As EventArgs)
        Dim selectedId As Integer = GetSelectedItemIdFromList()
        If selectedId = -1 Then
            MessageBox.Show("Please select an item from the list to edit.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim itemToEdit As BarcodeItem = DatabaseHelper.FindItemById(selectedId)
        If itemToEdit IsNot Nothing Then
            editingItemId = itemToEdit.Id
            txtItemName.Text = itemToEdit.Name
            txtPrice.Text = itemToEdit.Price.ToString("N2")

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
            MessageBox.Show("Could not find the selected item details. It may have been deleted.", "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
            LoadAllItems()
        End If
    End Sub

    Private Sub DeleteSelectedItem_Click(sender As Object, e As EventArgs)
        Dim selectedId As Integer = GetSelectedItemIdFromList()
        If selectedId = -1 Then
            MessageBox.Show("Please select an item from the list to delete.", "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim itemToDelete As BarcodeItem = DatabaseHelper.FindItemById(selectedId)
        If itemToDelete IsNot Nothing Then
            Dim confirmResult As DialogResult = MessageBox.Show($"Are you sure you want to delete '{itemToDelete.Name}' (ID: {itemToDelete.Id})?{vbCrLf}This item will be removed from the database. Sales records referencing this item will have its link nulled but name/price preserved.",
                                                                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If confirmResult = DialogResult.Yes Then
                Try
                    If DatabaseHelper.DeleteItem(selectedId) Then
                        lblManagementFeedback.Text = $"🗑️ Item '{itemToDelete.Name}' (ID: {selectedId}) deleted successfully."
                        lblManagementFeedback.ForeColor = Color.DarkGreen
                        If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
                        picGeneratedBarcode.Image = Nothing
                        LoadAllItems()
                    Else
                        MessageBox.Show("Failed to delete the item. It might still be in use or an unknown error occurred.", "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error deleting item: " & ex.Message, "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        Else
            MessageBox.Show("Could not find item details for deletion. It may have already been deleted.", "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
            LoadAllItems()
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
            Dim selectedItem As BarcodeItem = DatabaseHelper.FindItemById(selectedId)

            If selectedItem IsNot Nothing Then
                GenerateAndDisplayManagementBarcode(selectedItem.BarcodeData)
                lblManagementFeedback.Text = $"📦 Viewing: {selectedItem.Name} | 💰 Price: ₱{selectedItem.Price:N2} | 🏷️ Barcode: {selectedItem.BarcodeData}"
                lblManagementFeedback.ForeColor = Color.DarkSlateBlue
            Else
                lblManagementFeedback.Text = "Could not retrieve details for the selected item. It might have been deleted."
                lblManagementFeedback.ForeColor = Color.OrangeRed
                If picGeneratedBarcode.Image IsNot Nothing Then picGeneratedBarcode.Image.Dispose()
                picGeneratedBarcode.Image = Nothing
                LoadAllItems()
            End If
        Catch ex As Exception
            MessageBox.Show("Error showing barcode: " & ex.Message, "Display Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Sales Records Logic (Ported from RecordsForm)"

    Private Sub LoadSalesHeaders()
        Try
            If lstSalesHeaders Is Nothing OrElse lstSalesDetails Is Nothing OrElse lblSaleDetailsListTitle Is Nothing Then Return

            lstSalesHeaders.Items.Clear()
            lstSalesDetails.Items.Clear()
            lblSaleDetailsListTitle.Text = "Details for Selected Sale:"

            Dim sales As List(Of SaleHeaderRecord) = DatabaseHelper.GetAllSalesHeaders()
            If sales.Count > 0 Then
                For Each sale In sales
                    lstSalesHeaders.Items.Add(sale)
                Next
            Else
                lstSalesHeaders.Items.Add("No sales records found.")
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading sales headers: " & ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SalesHeader_SelectedIndexChanged(sender As Object, e As EventArgs)
        If lstSalesDetails Is Nothing OrElse lstSalesHeaders Is Nothing OrElse lblSaleDetailsListTitle Is Nothing Then Return

        lstSalesDetails.Items.Clear()
        If lstSalesHeaders.SelectedItem Is Nothing OrElse Not TypeOf lstSalesHeaders.SelectedItem Is SaleHeaderRecord Then
            lblSaleDetailsListTitle.Text = "Details for Selected Sale:"
            Return
        End If

        Dim selectedSaleHeader As SaleHeaderRecord = DirectCast(lstSalesHeaders.SelectedItem, SaleHeaderRecord)
        lblSaleDetailsListTitle.Text = $"Details for Sale #{selectedSaleHeader.SaleId} ({selectedSaleHeader.SaleDateTime:g}):"

        Try
            Dim details As List(Of SaleDetailRecord) = DatabaseHelper.GetSaleDetailsBySaleId(selectedSaleHeader.SaleId)
            If details.Count > 0 Then
                For Each detail In details
                    lstSalesDetails.Items.Add(detail)
                Next
            Else
                lstSalesDetails.Items.Add("No details found for this sale.")
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading sale details: " & ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lstSalesDetails.Items.Add("Error loading details.")
        End Try
    End Sub

    Private Sub RefreshSalesButton_Click(sender As Object, e As EventArgs)
        LoadSalesHeaders()
    End Sub

#End Region

    Private Sub ItemManagementForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If picGeneratedBarcode IsNot Nothing AndAlso picGeneratedBarcode.Image IsNot Nothing Then
            picGeneratedBarcode.Image.Dispose()
            picGeneratedBarcode.Image = Nothing
        End If
    End Sub

    Public Sub SelectTab(tabKey As String)
        If tcMain Is Nothing Then Return
        Select Case tabKey.ToLower()
            Case "itemmanagement"
                If tpItemManagement IsNot Nothing Then tcMain.SelectedTab = tpItemManagement
            Case "salesrecords"
                If tpSalesRecords IsNot Nothing Then tcMain.SelectedTab = tpSalesRecords
        End Select
    End Sub
End Class
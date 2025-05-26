'RecordsForm.vb
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections.Generic

Public Class RecordsForm
    Private lstSalesHeaders As ListBox
    Private lstSalesDetails As ListBox
    Private lblSalesHeader As Label
    Private lblSalesDetailsHeader As Label
    Private btnRefresh As Button
    Private btnClose As Button

    Private Sub RecordsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Sales Records"
        Me.Size = New Size(800, 600)
        Me.MinimumSize = New Size(700, 500)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.BackColor = Color.WhiteSmoke

        CreateControls()
        LoadSalesHeaders()
    End Sub

    Private Sub CreateControls()
        Dim currentY As Integer = 15

        lblSalesHeader = New Label()
        lblSalesHeader.Text = "All Sales Transactions (Newest First):"
        lblSalesHeader.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        lblSalesHeader.Location = New Point(15, currentY)
        lblSalesHeader.AutoSize = True
        Me.Controls.Add(lblSalesHeader)

        btnRefresh = New Button()
        btnRefresh.Text = "🔄 Refresh"
        btnRefresh.Size = New Size(100, 28)
        btnRefresh.Location = New Point(Me.ClientSize.Width - 15 - btnRefresh.Width, currentY - 2)
        btnRefresh.Font = New Font("Segoe UI", 9.0F)
        btnRefresh.BackColor = Color.LightYellow
        btnRefresh.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        AddHandler btnRefresh.Click, AddressOf RefreshButton_Click
        Me.Controls.Add(btnRefresh)
        currentY += lblSalesHeader.Height + 8

        lstSalesHeaders = New ListBox()
        lstSalesHeaders.Location = New Point(15, currentY)
        lstSalesHeaders.Size = New Size(Me.ClientSize.Width - 30, 180)
        lstSalesHeaders.Font = New Font("Segoe UI", 9.5F)
        lstSalesHeaders.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        AddHandler lstSalesHeaders.SelectedIndexChanged, AddressOf SalesHeader_SelectedIndexChanged
        Me.Controls.Add(lstSalesHeaders)
        currentY += lstSalesHeaders.Height + 15

        lblSalesDetailsHeader = New Label()
        lblSalesDetailsHeader.Text = "Details for Selected Sale:"
        lblSalesDetailsHeader.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        lblSalesDetailsHeader.Location = New Point(15, currentY)
        lblSalesDetailsHeader.AutoSize = True
        Me.Controls.Add(lblSalesDetailsHeader)
        currentY += lblSalesDetailsHeader.Height + 8

        lstSalesDetails = New ListBox()
        lstSalesDetails.Location = New Point(15, currentY)
        lstSalesDetails.Font = New Font("Courier New", 10.0F) ' Match Form1's transaction list font
        lstSalesDetails.Size = New Size(Me.ClientSize.Width - 30, Me.ClientSize.Height - currentY - 60)
        lstSalesDetails.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.Controls.Add(lstSalesDetails)

        btnClose = New Button()
        btnClose.Text = "❌ Close"
        btnClose.Size = New Size(120, 35)
        btnClose.Location = New Point(Me.ClientSize.Width - 15 - btnClose.Width, Me.ClientSize.Height - 15 - btnClose.Height)
        btnClose.BackColor = Color.IndianRed
        btnClose.ForeColor = Color.White
        btnClose.Font = New Font("Segoe UI", 9.5F, FontStyle.Bold)
        btnClose.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        AddHandler btnClose.Click, Sub(s, ev) Me.Close()
        Me.Controls.Add(btnClose)
    End Sub

    Private Sub LoadSalesHeaders()
        Try
            lstSalesHeaders.Items.Clear()
            lstSalesDetails.Items.Clear() ' Clear details when headers are reloaded
            lblSalesDetailsHeader.Text = "Details for Selected Sale:"

            Dim sales As List(Of SaleHeaderRecord) = DatabaseHelper.GetAllSalesHeaders()
            If sales.Count > 0 Then
                For Each sale In sales
                    lstSalesHeaders.Items.Add(sale) ' Using SaleHeaderRecord.ToString()
                Next
            Else
                lstSalesHeaders.Items.Add("No sales records found.")
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading sales headers: " & ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SalesHeader_SelectedIndexChanged(sender As Object, e As EventArgs)
        lstSalesDetails.Items.Clear()
        If lstSalesHeaders.SelectedItem Is Nothing OrElse Not TypeOf lstSalesHeaders.SelectedItem Is SaleHeaderRecord Then
            lblSalesDetailsHeader.Text = "Details for Selected Sale:"
            Return
        End If

        Dim selectedSaleHeader As SaleHeaderRecord = DirectCast(lstSalesHeaders.SelectedItem, SaleHeaderRecord)
        lblSalesDetailsHeader.Text = $"Details for Sale #{selectedSaleHeader.SaleId} ({selectedSaleHeader.SaleDateTime:g}):"

        Try
            Dim details As List(Of SaleDetailRecord) = DatabaseHelper.GetSaleDetailsBySaleId(selectedSaleHeader.SaleId)
            If details.Count > 0 Then
                For Each detail In details
                    lstSalesDetails.Items.Add(detail) ' Using SaleDetailRecord.ToString()
                Next
            Else
                lstSalesDetails.Items.Add("No details found for this sale.")
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading sale details: " & ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lstSalesDetails.Items.Add("Error loading details.")
        End Try
    End Sub

    Private Sub RefreshButton_Click(sender As Object, e As EventArgs)
        LoadSalesHeaders()
    End Sub

End Class
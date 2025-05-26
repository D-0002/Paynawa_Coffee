' TransactionLineItem.vb
Public Class TransactionLineItem
    Public Property Item As BarcodeItem
    Public Property Quantity As Integer

    Public ReadOnly Property LineTotal As Decimal
        Get
            If Item IsNot Nothing Then
                Return Item.Price * Quantity
            Else
                Return 0D
            End If
        End Get
    End Property

    Public Sub New(scannedItem As BarcodeItem, initialQuantity As Integer)
        Me.Item = scannedItem
        Me.Quantity = initialQuantity
    End Sub

    Public Overrides Function ToString() As String
        If Item IsNot Nothing Then
            Dim priceString As String = "₱" & Item.Price.ToString("#,##0.00")
            Dim totalString As String = "₱" & LineTotal.ToString("#,##0.00")

            Return String.Format("{0,-20} {1,3} x {2,7} = {3,9}",
                                 Item.Name,
                                 Quantity,
                                 priceString,
                                 totalString)
        Else
            Return "Error: Invalid Item Data"
        End If
    End Function
End Class
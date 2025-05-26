'SaleDetailRecord.vb
Public Class SaleDetailRecord
    Public Property SaleDetailId As Integer
    Public Property SaleId As Integer ' FK
    Public Property ItemName As String
    Public Property QuantitySold As Integer
    Public Property PriceAtSale As Decimal
    Public Property LineTotal As Decimal

    Public Overrides Function ToString() As String
        Dim priceString As String = "₱" & PriceAtSale.ToString("#,##0.00")
        Dim totalString As String = "₱" & LineTotal.ToString("#,##0.00")

        Return String.Format("{0,-20} {1,3} x {2,7} = {3,9}",
                             ItemName,
                             QuantitySold,
                             priceString,
                             totalString)
    End Function
End Class
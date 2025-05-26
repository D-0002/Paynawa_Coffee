Public Class SaleHeaderRecord
    Public Property SaleId As Integer
    Public Property SaleDateTime As DateTime
    Public Property TotalAmount As Decimal

    Public Overrides Function ToString() As String 
        Return $"Sale #{SaleId} - {SaleDateTime:MM/dd/yyyy hh:mm tt} - Total: ₱{TotalAmount:N2}"
    End Function
End Class
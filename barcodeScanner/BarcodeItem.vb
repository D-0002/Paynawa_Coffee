' BarcodeItem.vb
Public Class BarcodeItem
    Public Property Id As Integer
    Public Property Name As String
    Public Property Price As Decimal
    Public Property BarcodeData As String
    Public Property DateCreated As DateTime

    Public Overrides Function ToString() As String
        Return $"{Name} - ₱{Price:F2}"
    End Function
End Class
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Collections.Generic

Public Class DatabaseHelper

    Private Shared ReadOnly serverIpAddress As String = "localhost"
    Private Shared ReadOnly databaseName As String = "nike_db"
    Private Shared ReadOnly userId As String = "root"
    Private Shared ReadOnly password As String = ""

    Private Shared ReadOnly connectionString As String =
        $"Server={serverIpAddress};Port=3306;Database={databaseName};Uid={userId};Pwd={password};Allow User Variables=True;Convert Zero Datetime=True;" 'Added Convert Zero Datetime

    Public Shared Sub InitializeDatabase()
        Try

            Dim initialConnectionString As String = $"Server={serverIpAddress};Port=3306;Uid={userId};Pwd={password};Allow User Variables=True;"
            Using tempConnection As New MySqlConnection(initialConnectionString)
                tempConnection.Open()
                Using cmd As New MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;", tempConnection)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            Using connection As New MySqlConnection(connectionString)
                connection.Open()

                Dim createItemsTableQuery As String = $"
                CREATE TABLE IF NOT EXISTS `Items` (
                    `Id` INT PRIMARY KEY AUTO_INCREMENT,
                    `Name` VARCHAR(255) NOT NULL,
                    `Price` DECIMAL(10,2) NOT NULL,
                    `BarcodeData` VARCHAR(255) NOT NULL UNIQUE,
                    `DateCreated` DATETIME DEFAULT CURRENT_TIMESTAMP
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                Using command As New MySqlCommand(createItemsTableQuery, connection)
                    command.ExecuteNonQuery()
                End Using

                Dim indexExistsQuery As String = "
                SELECT COUNT(1) FROM INFORMATION_SCHEMA.STATISTICS
                WHERE TABLE_SCHEMA = @dbName AND TABLE_NAME = 'Items' AND INDEX_NAME = 'idx_items_name';"
                Dim indexExists As Boolean = False
                Using checkCmd As New MySqlCommand(indexExistsQuery, connection)
                    checkCmd.Parameters.AddWithValue("@dbName", databaseName)
                    indexExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0
                End Using
                If Not indexExists Then
                    Dim createNameIndexQuery As String = "CREATE INDEX idx_items_name ON Items (Name);"
                    Using indexCommand As New MySqlCommand(createNameIndexQuery, connection)
                        indexCommand.ExecuteNonQuery()
                    End Using
                End If

                Dim createSalesHeaderTableQuery As String = $"
                CREATE TABLE IF NOT EXISTS `SalesHeader` (
                    `SaleID` INT PRIMARY KEY AUTO_INCREMENT,
                    `SaleDateTime` DATETIME DEFAULT CURRENT_TIMESTAMP,
                    `TotalAmount` DECIMAL(10,2) NOT NULL
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                Using command As New MySqlCommand(createSalesHeaderTableQuery, connection)
                    command.ExecuteNonQuery()
                End Using

                Dim createSalesDetailsTableQuery As String = $"
                CREATE TABLE IF NOT EXISTS `SalesDetails` (
                    `SaleDetailID` INT PRIMARY KEY AUTO_INCREMENT,
                    `SaleID` INT NOT NULL,
                    `ItemID` INT NULL,
                    `ItemNameSnapshot` VARCHAR(255) NOT NULL,
                    `QuantitySold` INT NOT NULL,
                    `PriceAtSale` DECIMAL(10,2) NOT NULL,
                    `LineTotal` DECIMAL(10,2) NOT NULL,
                    FOREIGN KEY (`SaleID`) REFERENCES `SalesHeader`(`SaleID`) ON DELETE CASCADE,
                    FOREIGN KEY (`ItemID`) REFERENCES `Items`(`Id`) ON DELETE SET NULL
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"
                Using command As New MySqlCommand(createSalesDetailsTableQuery, connection)
                    command.ExecuteNonQuery()
                End Using

            End Using
        Catch ex As Exception
            Throw New Exception("Failed to initialize MySQL database: " & ex.Message & vbCrLf & "Attempted Connection String (password omitted): " & connectionString.Replace("Pwd=" & password, "Pwd=****"), ex)
        End Try
    End Sub

    Public Shared Function AddItem(name As String, price As Decimal) As BarcodeItem
        Try
            Dim barcodeData As String = name.Trim() & "-" & price.ToString("F2")

            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim insertQuery As String = "
                INSERT INTO Items (Name, Price, BarcodeData)
                VALUES (@name, @price, @barcodeData);
                SELECT LAST_INSERT_ID();"

                Using command As New MySqlCommand(insertQuery, connection)
                    command.Parameters.AddWithValue("@name", name.Trim())
                    command.Parameters.AddWithValue("@price", price)
                    command.Parameters.AddWithValue("@barcodeData", barcodeData)
                    Dim newId As Integer = Convert.ToInt32(command.ExecuteScalar())
                    Return New BarcodeItem() With {
                        .Id = newId,
                        .Name = name.Trim(),
                        .Price = price,
                        .BarcodeData = barcodeData,
                        .DateCreated = DateTime.Now
                    }
                End Using
            End Using
        Catch exSQL As MySqlException
            If exSQL.Number = 1062 Then
                Dim conflictingBarcodeData As String = name.Trim() & "-" & price.ToString("F2")
                Throw New Exception($"Failed to add item: The name/price combination results in barcode data '{conflictingBarcodeData}' which is already used by another item. Please use a different name or price. (MySQL Error {exSQL.Number})")
            Else
                Throw New Exception("MySQL database error adding item: " & exSQL.Message, exSQL)
            End If
        Catch ex As Exception
            Throw New Exception("Failed to add item: " & ex.Message, ex)
        End Try
    End Function

    ' Update existing item in database
    Public Shared Function UpdateItem(item As BarcodeItem) As Boolean
        Try
            Dim updatedBarcodeData As String = item.Name.Trim() & "-" & item.Price.ToString("F2")
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim updateQuery As String = "
                UPDATE Items
                SET Name = @name,
                    Price = @price,
                    BarcodeData = @barcodeData
                WHERE Id = @id;"
                Using command As New MySqlCommand(updateQuery, connection)
                    command.Parameters.AddWithValue("@name", item.Name.Trim())
                    command.Parameters.AddWithValue("@price", item.Price)
                    command.Parameters.AddWithValue("@barcodeData", updatedBarcodeData)
                    command.Parameters.AddWithValue("@id", item.Id)
                    Dim rowsAffected As Integer = command.ExecuteNonQuery()
                    If rowsAffected > 0 Then item.BarcodeData = updatedBarcodeData ' Update in-memory object
                    Return rowsAffected > 0
                End Using
            End Using
        Catch exSQL As MySqlException
            If exSQL.Number = 1062 Then
                Dim conflictingBarcodeData As String = item.Name.Trim() & "-" & item.Price.ToString("F2")
                Throw New Exception($"Failed to update item: The new name/price results in barcode data '{conflictingBarcodeData}' which is already used by another item. (MySQL Error {exSQL.Number})")
            Else
                Throw New Exception("MySQL database error updating item: " & exSQL.Message, exSQL)
            End If
        Catch ex As Exception
            Throw New Exception("Failed to update item: " & ex.Message, ex)
        End Try
    End Function

    ' Find item by barcode data
    Public Shared Function FindItemByBarcode(barcodeData As String) As BarcodeItem
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim selectQuery As String = "
                SELECT Id, Name, Price, BarcodeData, DateCreated
                FROM Items
                WHERE BarcodeData = @barcodeData;"
                Using command As New MySqlCommand(selectQuery, connection)
                    command.Parameters.AddWithValue("@barcodeData", barcodeData)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapReaderToBarcodeItem(reader)
                        End If
                    End Using
                End Using
            End Using
            Return Nothing
        Catch ex As Exception
            Console.WriteLine("Error FindItemByBarcode: " & ex.ToString())
            Throw New Exception("Failed to find item by barcode: " & ex.Message, ex)
        End Try
    End Function

    Public Shared Function FindItemById(id As Integer) As BarcodeItem
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim selectQuery As String = "
                SELECT Id, Name, Price, BarcodeData, DateCreated
                FROM Items
                WHERE Id = @id;"
                Using command As New MySqlCommand(selectQuery, connection)
                    command.Parameters.AddWithValue("@id", id)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapReaderToBarcodeItem(reader)
                        End If
                    End Using
                End Using
            End Using
            Return Nothing
        Catch ex As Exception
            Console.WriteLine("Error FindItemById: " & ex.ToString())
            Throw New Exception("Failed to find item by ID: " & ex.Message, ex)
        End Try
    End Function

    Public Shared Function FindItemByName(name As String) As BarcodeItem
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim selectQuery As String = "
                SELECT Id, Name, Price, BarcodeData, DateCreated
                FROM Items
                WHERE Name = @name;"
                Using command As New MySqlCommand(selectQuery, connection)
                    command.Parameters.AddWithValue("@name", name)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            Return MapReaderToBarcodeItem(reader)
                        End If
                    End Using
                End Using
            End Using
            Return Nothing
        Catch ex As Exception
            Console.WriteLine("Error FindItemByName: " & ex.ToString())
            Throw New Exception("Failed to find item by Name: " & ex.Message, ex)
        End Try
    End Function

    Public Shared Function GetAllItems() As List(Of BarcodeItem)
        Dim items As New List(Of BarcodeItem)
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim selectQuery As String = "
                SELECT Id, Name, Price, BarcodeData, DateCreated
                FROM Items
                ORDER BY Name ASC;"
                Using command As New MySqlCommand(selectQuery, connection)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            items.Add(MapReaderToBarcodeItem(reader))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("Error GetAllItems: " & ex.ToString())
            Throw New Exception("Failed to get items: " & ex.Message, ex)
        End Try
        Return items
    End Function

    Public Shared Function DeleteItem(id As Integer) As Boolean
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim deleteQuery As String = "DELETE FROM Items WHERE Id = @id;"
                Using command As New MySqlCommand(deleteQuery, connection)
                    command.Parameters.AddWithValue("@id", id)
                    Dim rowsAffected As Integer = command.ExecuteNonQuery()
                    Return rowsAffected > 0
                End Using
            End Using
        Catch ex As MySqlException

            If ex.Number = 1451 Then
                Throw New Exception($"Failed to delete item: This item is referenced by other records that prevent its deletion (MySQL Error {ex.Number}). Check other tables if SalesDetails FK is already SET NULL.", ex)
            Else
                Throw New Exception("MySQL database error deleting item: " & ex.Message, ex)
            End If
        Catch ex As Exception
            Console.WriteLine("Error DeleteItem: " & ex.ToString())
            Throw New Exception("Failed to delete item: " & ex.Message, ex)
        End Try
    End Function

    Private Shared Function MapReaderToBarcodeItem(reader As MySqlDataReader) As BarcodeItem
        Return New BarcodeItem() With {
            .Id = Convert.ToInt32(reader("Id")),
            .Name = reader("Name").ToString(),
            .Price = Convert.ToDecimal(reader("Price")),
            .BarcodeData = reader("BarcodeData").ToString(),
            .DateCreated = If(reader.IsDBNull(reader.GetOrdinal("DateCreated")), DateTime.MinValue, Convert.ToDateTime(reader("DateCreated")))
        }
    End Function

    Public Shared Function RecordSale(transactionLineItems As List(Of TransactionLineItem), totalAmount As Decimal) As Integer
        Dim saleId As Integer = -1
        Using connection As New MySqlConnection(connectionString)
            connection.Open()
            Using transaction As MySqlTransaction = connection.BeginTransaction()
                Try
                    Dim insertHeaderQuery As String = "
                    INSERT INTO SalesHeader (TotalAmount, SaleDateTime)
                    VALUES (@totalAmount, NOW());
                    SELECT LAST_INSERT_ID();"

                    Using headerCmd As New MySqlCommand(insertHeaderQuery, connection, transaction)
                        headerCmd.Parameters.AddWithValue("@totalAmount", totalAmount)
                        saleId = Convert.ToInt32(headerCmd.ExecuteScalar())
                    End Using

                    If saleId <= 0 Then
                        transaction.Rollback()
                        Throw New Exception("Failed to create sales header record (SaleID not generated).")
                    End If

                    Dim insertDetailQuery As String = "
                    INSERT INTO SalesDetails (SaleID, ItemID, ItemNameSnapshot, QuantitySold, PriceAtSale, LineTotal)
                    VALUES (@saleID, @itemID, @itemNameSnapshot, @quantitySold, @priceAtSale, @lineTotal);"

                    For Each lineItem In transactionLineItems
                        Using detailCmd As New MySqlCommand(insertDetailQuery, connection, transaction)
                            detailCmd.Parameters.AddWithValue("@saleID", saleId)
                            detailCmd.Parameters.AddWithValue("@itemID", lineItem.Item.Id)
                            detailCmd.Parameters.AddWithValue("@itemNameSnapshot", lineItem.Item.Name)
                            detailCmd.Parameters.AddWithValue("@quantitySold", lineItem.Quantity)
                            detailCmd.Parameters.AddWithValue("@priceAtSale", lineItem.Item.Price)
                            detailCmd.Parameters.AddWithValue("@lineTotal", lineItem.LineTotal)
                            detailCmd.ExecuteNonQuery()
                        End Using
                    Next

                    transaction.Commit()
                    Return saleId
                Catch ex As Exception
                    transaction.Rollback()
                    Console.WriteLine("Error RecordSale: " & ex.ToString())
                    Throw New Exception("Failed to record sale transaction: " & ex.Message, ex)
                End Try
            End Using
        End Using
    End Function

    Public Shared Function GetAllSalesHeaders() As List(Of SaleHeaderRecord)
        Dim sales As New List(Of SaleHeaderRecord)()
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim selectQuery As String = "
                SELECT SaleID, SaleDateTime, TotalAmount
                FROM SalesHeader
                ORDER BY SaleDateTime DESC;"
                Using command As New MySqlCommand(selectQuery, connection)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            sales.Add(New SaleHeaderRecord() With {
                                .SaleId = Convert.ToInt32(reader("SaleID")),
                                .SaleDateTime = Convert.ToDateTime(reader("SaleDateTime")),
                                .TotalAmount = Convert.ToDecimal(reader("TotalAmount"))
                            })
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("Error GetAllSalesHeaders: " & ex.ToString())
            Throw New Exception("Failed to get sales headers: " & ex.Message, ex)
        End Try
        Return sales
    End Function

    Public Shared Function GetSaleDetailsBySaleId(saleId As Integer) As List(Of SaleDetailRecord)
        Dim details As New List(Of SaleDetailRecord)()
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim selectQuery As String = "
                SELECT sd.SaleDetailID, sd.SaleID, sd.ItemID, sd.ItemNameSnapshot, sd.QuantitySold, sd.PriceAtSale, sd.LineTotal
                FROM SalesDetails sd
                WHERE sd.SaleID = @saleId
                ORDER BY sd.SaleDetailID ASC;"
                Using command As New MySqlCommand(selectQuery, connection)
                    command.Parameters.AddWithValue("@saleId", saleId)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            Dim detail As New SaleDetailRecord() With {
                                .SaleDetailId = Convert.ToInt32(reader("SaleDetailID")),
                                .SaleId = Convert.ToInt32(reader("SaleID")),
                                .ItemName = reader("ItemNameSnapshot").ToString(),
                                .QuantitySold = Convert.ToInt32(reader("QuantitySold")),
                                .PriceAtSale = Convert.ToDecimal(reader("PriceAtSale")),
                                .LineTotal = Convert.ToDecimal(reader("LineTotal"))
                            }

                            If Not reader.IsDBNull(reader.GetOrdinal("ItemID")) Then
                            Else
                                detail.ItemName &= " (Original Item Deleted)"
                            End If
                            details.Add(detail)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("Error GetSaleDetailsBySaleId: " & ex.ToString())
            Throw New Exception("Failed to get sale details: " & ex.Message, ex)
        End Try
        Return details
    End Function

End Class
Public Class CartForm

    Dim con As New OleDb.OleDbConnection(MainMenu.connectionString)
    Private connectionString As String
    Private mainForm As MainMenu
    Public Sub New(parentForm As MainMenu, conStr As String)
        InitializeComponent()
        mainForm = parentForm
        connectionString = conStr
    End Sub

    ' Add items to the ListView instead of ListBox
    ' Variable to store the total sum
    Dim total As Decimal = 0

    Public Sub AddCart(items As List(Of List(Of String)))
        ' Temporary list to store items with updated quantities
        Dim cartItems As New List(Of List(Of String))

        ' Loop through the incoming items to process them
        For Each itemDetails As List(Of String) In items
            Dim productName As String = itemDetails(0)
            Dim found As Boolean = False

            ' Check if the product is already in the cart
            For Each cartItem As List(Of String) In cartItems
                If cartItem(0) = productName Then
                    ' Product is found, update the quantity
                    Dim currentQty As Integer = Integer.Parse(cartItem(6)) ' Get current quantity
                    cartItem(6) = (currentQty + 1).ToString() ' Increment quantity
                    found = True
                    Exit For
                End If
            Next

            ' If the product was not found, add it with initial quantity of 1
            If Not found Then
                ' Create a new product entry with the initial quantity of 1
                Dim newItem As New List(Of String)(itemDetails)
                newItem.Add("1") ' Add quantity as 1
                cartItems.Add(newItem)
            End If
        Next

        ' Clear the existing ListView before adding new data
        ListView1.Items.Clear()

        ' Set up the ListView columns (only if not already done)
        If ListView1.Columns.Count = 0 Then
            ListView1.View = View.Details
            ListView1.Columns.Add("Product", 120, HorizontalAlignment.Left)
            ListView1.Columns.Add("Description", 235, HorizontalAlignment.Left)
            ListView1.Columns.Add("Brand", 100, HorizontalAlignment.Left)
            ListView1.Columns.Add("Category", 100, HorizontalAlignment.Left)
            ListView1.Columns.Add("Stock", 70, HorizontalAlignment.Left)
            ListView1.Columns.Add("Price", 67, HorizontalAlignment.Left)
            ListView1.Columns.Add("Qty", 50, HorizontalAlignment.Left)
        End If

        ' Add the updated items to the ListView
        For Each itemDetails As List(Of String) In cartItems
            ' Create a new ListViewItem for the first column (Product Name)
            Dim item As New ListViewItem(itemDetails(0))

            ' Add the remaining subitems (columns) to the ListViewItem
            For i As Integer = 1 To itemDetails.Count - 1
                item.SubItems.Add(itemDetails(i))
            Next

            ' Add the ListViewItem to the ListView
            ListView1.Items.Add(item)
        Next

        ' Call the method to calculate the subtotal
        subTotal()
    End Sub


    Private Sub Removebtn_Click(sender As Object, e As EventArgs) Handles RemoveBtn.Click
        If ListView1.Items.Count = 0 Then
            MessageBox.Show("Cart is already empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        ElseIf ListView1.SelectedItems.Count > 0 Then
            Dim answer = MessageBox.Show("Are you sure you want to remove this item?", "Remove item", MessageBoxButtons.YesNo)
            If answer = DialogResult.Yes Then
                Dim selectedItem As ListViewItem = ListView1.SelectedItems(0)
                Dim productName As String = selectedItem.Text

                ' Open database connection
                Dim con As New OleDb.OleDbConnection(MainMenu.connectionString)
                con.Open()

                ' Retrieve the current stock of the product
                Dim getStockCmd As New OleDb.OleDbCommand("SELECT Stock FROM Products WHERE [Name] = ?", con)
                getStockCmd.Parameters.AddWithValue("?", productName)

                Dim currentStock As Integer = 0
                Using reader = getStockCmd.ExecuteReader()
                    If reader.Read() Then
                        currentStock = Convert.ToInt32(reader("Stock"))
                    Else
                        MessageBox.Show("Product not found: " & productName)
                        con.Close()
                        Exit Sub
                    End If
                End Using

                ' Here, we want to explicitly set the quantity to return as 1
                ' so that we only increase the stock by 1, regardless of cart quantity
                Dim quantityToReturn As Integer = 1  ' You only return one unit when removing

                ' Increase the stock by 1 (only 1 unit is returned to stock)
                Dim updateStockCmd As New OleDb.OleDbCommand("UPDATE Products SET Stock = Stock + ? WHERE [Name] = ?", con)
                updateStockCmd.Parameters.AddWithValue("?", quantityToReturn)
                updateStockCmd.Parameters.AddWithValue("?", productName)
                updateStockCmd.ExecuteNonQuery()

                ' Now remove the item from the ListView and savedItems
                For i As Integer = 0 To MainMenu.savedItems.Count - 1
                    If MainMenu.savedItems(i)(0) = productName Then
                        Dim currentQty As Integer = 1
                        If MainMenu.savedItems(i).Count > 6 Then
                            Integer.TryParse(MainMenu.savedItems(i)(6), currentQty)
                        End If

                        If currentQty > 1 Then
                            ' Decrease the quantity by 1 (since we are removing 1)
                            MainMenu.savedItems(i)(6) = (currentQty - 1).ToString()
                        Else
                            ' If quantity reaches 0, remove the item from savedItems and ListView
                            MainMenu.savedItems.RemoveAt(i)
                            ReceiptForm.listRec.Items.RemoveAt(i)
                        End If

                        Exit For
                    End If
                Next

                ' Refresh cart display
                AddCart(MainMenu.savedItems)
                
                ' Close database connection
                con.Close()

                MessageBox.Show("Item removed and stock updated successfully.")
            End If
        Else
            MessageBox.Show("Please select an item to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub






    ' Function to update the stock in the database when an item is removed
    Private Sub UpdateStockInDatabase(productId As Integer, quantity As Integer)
        ' Assuming you have a method to update the stock in the database
        ' Example SQL query to update stock:
        Dim query As String = "UPDATE Products SET Stock = Stock + @quantity WHERE ProductId = @productId"

        ' Execute the query using your database connection and command
        Using conn As New OleDb.OleDbConnection(connectionString)
            Using cmd As New OleDb.OleDbCommand(query, conn)
                cmd.Parameters.AddWithValue("@quantity", quantity)
                cmd.Parameters.AddWithValue("@productId", productId)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub



    Private Sub RemoveAllBtn_Click(sender As Object, e As EventArgs) Handles RemoveAllBtn.Click
        ' Check if the ListView is already empty before attempting to remove items
        If ListView1.Items.Count = 0 Then
            ' Display a message indicating that there are no items to remove
            MessageBox.Show("Cart is already empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            ' Ask the user for confirmation before clearing the cart
            Dim answer = MessageBox.Show("Are you sure you want to remove all items?", "Remove items", MessageBoxButtons.YesNo)

            If answer = DialogResult.Yes Then
                ' Remove all items from the ListView
                ListView1.Items.Clear()
                MainMenu.savedItems.Clear()
                ReceiptForm.listRec.Clear()

                ' Refresh the ListView to update the subtotal
                AddCart(MainMenu.savedItems)
            End If

        End If
    End Sub

    Private Sub returnButton_click(sender As Object, e As EventArgs) Handles ReturnBtn.Click
        ' Hide Cart Form
        Me.Hide()

        MainMenu.LoadInventory()
        ' Show Main Menu Form
        MainMenu.Show()
    End Sub

    Public Sub subTotal()
        total = 0 ' Reset the total before recalculating
        ' Loop through all the items in the ListView
        For Each item As ListViewItem In ListView1.Items
            ' Extract the price value from the price column (column 5)
            Dim priceText As String = item.SubItems(5).Text
            ' Extract the quantity value from the quantity column (column 6)
            Dim quantityText As String = item.SubItems(6).Text

            ' Remove the dollar sign from the price and convert to Decimal
            Dim price As Decimal
            If Decimal.TryParse(priceText.Replace("$", "").Trim(), price) Then
                ' Convert the quantity to an integer
                Dim quantity As Integer
                If Integer.TryParse(quantityText, quantity) Then
                    ' Multiply price by quantity and add to total
                    total += price * quantity
                End If
            End If
        Next
        ' Display the total sum on the subtotal label
        subTotallb.Text = "Subtotal: " & total.ToString("C2") ' Display in currency format
        ReceiptForm.lblTotal.Text = "" & total.ToString("C2")
        ReceiptForm.lblStotal.Text = "" & total.ToString("C2")
    End Sub

    Private Sub confirmbtn_Click(sender As Object, e As EventArgs) Handles confirmbtn.Click
        Dim amount As Decimal = 0
        Dim change As Decimal = 0
        ' Check if the ListView is empty before proceeding to checkout
        If ListView1.Items.Count = 0 Then
            ' Display a message indicating that the cart is empty
            MessageBox.Show("Cart is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            Dim userInput As String = InputBox("Enter the amount the customer gave:", "Transaction", "0")

            If Not Decimal.TryParse(userInput, amount) Then
                MessageBox.Show("Enter a Valid Amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            ElseIf amount < total Then ' Check if have sufficient money
                MessageBox.Show("Balance not enough.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            Dim con As New OleDb.OleDbConnection(connectionString)
            con.Open()

            For Each item As ListViewItem In ListView1.Items
                Dim productName As String = item.SubItems(0).Text
                Dim quantityPurchased As Integer = Integer.Parse(item.SubItems(6).Text)

                ' Get stock from database
                Dim getStockCmd As New OleDb.OleDbCommand("SELECT Stock FROM Products WHERE Name = ?", con)
                getStockCmd.Parameters.AddWithValue("Stock", productName)
                Dim currentStock As Integer = Convert.ToInt32(getStockCmd.ExecuteScalar())

                ' Actualize the Stock
                Dim newStock As Integer = currentStock - quantityPurchased
                Dim updateCmd As New OleDb.OleDbCommand("UPDATE Products SET Stock = ? WHERE Name = ?", con)
                updateCmd.Parameters.AddWithValue("Stock", newStock)
                updateCmd.Parameters.AddWithValue("Name", productName)
                updateCmd.ExecuteNonQuery()
            Next

            con.Close()
            MainMenu.savedItems.Clear()
            change = total - amount
            ReceiptForm.lblChange.Text = "" & change.ToString("N2")
            ReceiptForm.lblDate.Text = DateTime.Now
            ReceiptForm.Show()
            Me.Hide()
            MainMenu.LoadInventory()
        End If

    End Sub

    Private Sub MainMenu_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Application.Exit()
    End Sub

    Private Sub CartForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class

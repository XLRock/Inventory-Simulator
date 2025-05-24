Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Threading.Tasks

Public Class AdminForm
    Public connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\Inventory_Store.accdb"
    Private selectedImagePath As String = ""
    Private Async Sub AdminForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeListView()
        cmbCategory.Items.Clear()
        cmbCategory.Items.Add("Gaming")
        cmbCategory.Items.Add("Accessories")
        cmbCategory.Items.Add("Audio")
        cmbCategory.Items.Add("Computers")
        cmbCategory.Items.Add("Mobile Devices")
        cmbCategory.Items.Add("Other")
        cmbBrand.Items.Clear()
        cmbBrand.Items.Add("Microsoft")
        cmbBrand.Items.Add("Sony")
        cmbBrand.Items.Add("Nintendo")
        cmbBrand.Items.Add("Dell")
        cmbBrand.Items.Add("Samsung")
        cmbBrand.Items.Add("Apple")
        lblOverlay.Enabled = False
        Await LoadAllProductsAsync()  ' Automatically load products when form opens
    End Sub

    Private Sub InitializeListView()
        ListView1.Clear()
        ListView1.View = View.Details
        ListView1.Columns.Add("Product", 120, HorizontalAlignment.Left)
        ListView1.Columns.Add("Description", 200, HorizontalAlignment.Left)
        ListView1.Columns.Add("Brand", 100, HorizontalAlignment.Left)
        ListView1.Columns.Add("Category", 100, HorizontalAlignment.Left)
        ListView1.Columns.Add("Stock", 75, HorizontalAlignment.Left)
        ListView1.Columns.Add("Price", 75, HorizontalAlignment.Left)
    End Sub

    ' Load products into the ListView based on query and parameters
    Private Async Function LoadProductsAsync(query As String, parameters As List(Of OleDbParameter)) As Task
        Try
            Using con As New OleDbConnection(connectionString)
                Await con.OpenAsync()

                ' Execute the query with parameters
                Using cmd As New OleDbCommand(query, con)
                    cmd.Parameters.AddRange(parameters.ToArray())

                    Using reader As OleDbDataReader = Await cmd.ExecuteReaderAsync()
                        ' Clear existing items in ListView before loading new results
                        ListView1.Items.Clear()

                        ' Check if any records are found
                        If Not reader.HasRows Then
                            MessageBox.Show("No products found matching the search criteria.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If

                        ' Loop through all rows and add items to the ListView
                        While Await reader.ReadAsync()
                            Dim item As New ListViewItem(reader("Name").ToString())
                            item.SubItems.Add(reader("Description").ToString())
                            item.SubItems.Add(reader("Brand").ToString())
                            item.SubItems.Add(reader("Category").ToString())
                            item.SubItems.Add(reader("Stock").ToString())
                            Dim price As Decimal = Convert.ToDecimal(reader("Price"))
                            item.SubItems.Add("$" & price.ToString("0.00"))
                            item.SubItems.Add(reader("ImageUrl").ToString())
                            ListView1.Items.Add(item)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while searching products: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function


    Private Async Function LoadAllProductsAsync() As Task
        InitializeListView()
        Dim query As String = "SELECT Name, Price, Description, Stock, Category, Brand, ImageURL FROM Products"
        Await LoadProductsAsync(query, New List(Of OleDbParameter)())
    End Function
    '-------------------------SEARCH PRODUCTS------------------------------------------------
    ' Search products by Name, Category, or Brand
    Private Async Sub SearchProducts(searchText As String)
        ' Clear the current items in the ListView before populating new results
        ListView1.Items.Clear()

        ' Check if search text is empty
        If String.IsNullOrWhiteSpace(searchText) Then
            MessageBox.Show("Please enter a search term.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        MessageBox.Show("Searching for: " & searchText)

        ' SQL Query to search for products by Name, Category, or Brand
        Dim query As String = "SELECT Name, Price, Description, Stock, Category, Brand, ImageURL FROM Products WHERE Name LIKE @SearchText OR Category LIKE @SearchText OR Brand LIKE @SearchText"

        ' Parameters for SQL query
        Dim parameters As New List(Of OleDbParameter) From {
        New OleDbParameter("@SearchText", "%" & searchText & "%")
    }

        ' Load products based on the search criteria
        Await LoadProductsAsync(query, parameters)
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim searchText As String = txtSearchBox.Text.Trim().ToLower()
        SearchProducts(searchText)  ' Perform a search when clicking the Search button
    End Sub
    '--------------------------------------------------------------------------------------------------------------------------------
    '--------------------------------------------------------ADD BUTTON----------------------------------------------------------------
    Private Async Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If String.IsNullOrWhiteSpace(txtName.Text) OrElse String.IsNullOrWhiteSpace(txtPrice.Text) Then
            MessageBox.Show("Please fill in the required fields (Name and Price).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            ' Route for images
            Dim imageUrlToSave As String = ""

            If Not String.IsNullOrEmpty(selectedImagePath) Then
                Dim imageFileName As String = Path.GetFileName(selectedImagePath)
                Dim imageFolderPath As String = Path.Combine(Application.StartupPath, "images")

                If Not Directory.Exists(imageFolderPath) Then
                    Directory.CreateDirectory(imageFolderPath)
                End If

                Dim destPath As String = Path.Combine(imageFolderPath, imageFileName)
                If Not File.Exists(destPath) Then
                    File.Copy(selectedImagePath, destPath)
                End If

                imageUrlToSave = Path.Combine("images", imageFileName)
            End If

            Using con As New OleDbConnection(connectionString)
                Await con.OpenAsync()

                Dim query As String = "INSERT INTO Products (Name, Price, Description, Stock, Category, Brand, Deals, ImageURL) VALUES (@Name, @Price, @Description, @Stock, @Category, @Brand, @Deals, @ImageURL)"
                Using cmd As New OleDbCommand(query, con)
                    cmd.Parameters.Add("@Name", OleDbType.VarChar).Value = txtName.Text
                    cmd.Parameters.Add("@Price", OleDbType.Currency).Value = Convert.ToDecimal(txtPrice.Text)
                    cmd.Parameters.Add("@Description", OleDbType.VarChar).Value = txtDescription.Text
                    cmd.Parameters.Add("@Stock", OleDbType.Integer).Value = If(String.IsNullOrWhiteSpace(txtStock.Text), 0, Convert.ToInt32(txtStock.Text))
                    Dim category As String = If(cmbCategory.SelectedItem IsNot Nothing, cmbCategory.SelectedItem.ToString(), "")
                    cmd.Parameters.Add("@Category", OleDbType.VarChar).Value = category
                    Dim brand As String = If(cmbBrand.SelectedItem IsNot Nothing, cmbBrand.SelectedItem.ToString(), "")
                    cmd.Parameters.Add("@Brand", OleDbType.VarChar).Value = brand
                    cmd.Parameters.Add("@Deals", OleDbType.Boolean).Value = chkDeals.Checked
                    cmd.Parameters.Add("@ImageURL", OleDbType.VarChar).Value = imageUrlToSave

                    Await cmd.ExecuteNonQueryAsync()
                    MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            End Using

            ClearInputFields()
            Await LoadAllProductsAsync()

        Catch ex As Exception
            MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '--------------------------------------------------DELETE BUTTON----------------------------------------------------------------
    Private Async Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If ListView1.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select a product to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim productName As String = ListView1.SelectedItems(0).Text

        Try
            Using con As New OleDbConnection(connectionString)
                Await con.OpenAsync()
                Dim query As String = "DELETE FROM Products WHERE Name = @Name"
                Using cmd As New OleDbCommand(query, con)
                    cmd.Parameters.Add("@Name", OleDbType.VarChar).Value = productName
                    Await cmd.ExecuteNonQueryAsync()
                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            End Using

            Await LoadAllProductsAsync()

        Catch ex As Exception
            MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    '--------------------------------------------------------------------------------------------------------------------
    Private Sub ClearInputFields()
        txtName.Clear()
        txtPrice.Clear()
        txtDescription.Clear()
        txtStock.Clear()
        cmbCategory.SelectedIndex = -1
        cmbBrand.SelectedIndex = -1
        chkDeals.Checked = False
        selectedImagePath = ""
        picItem.Image = Nothing
    End Sub
    '---------UPDATE BUTTON--------------------------------------------------------------------------------------------------------------------
    Private Async Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        If ListView1.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select a product to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(txtName.Text) OrElse String.IsNullOrWhiteSpace(txtPrice.Text) Then
            MessageBox.Show("Please fill in the required fields (Name and Price).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim selectedItem As ListViewItem = ListView1.SelectedItems(0)
            Dim originalProductName As String = selectedItem.Text

            Dim imageUrlToSave As String = selectedItem.SubItems(6).Text ' Value of URL

            If Not String.IsNullOrEmpty(selectedImagePath) Then
                Dim imageFileName As String = Path.GetFileName(selectedImagePath)
                Dim imageFolderPath As String = Path.Combine(Application.StartupPath, "images")

                If Not Directory.Exists(imageFolderPath) Then
                    Directory.CreateDirectory(imageFolderPath)
                End If

                Dim destPath As String = Path.Combine(imageFolderPath, imageFileName)
                If Not File.Exists(destPath) Then
                    File.Copy(selectedImagePath, destPath)
                End If

                imageUrlToSave = Path.Combine("images", imageFileName)
            End If

            Using con As New OleDbConnection(connectionString)
                Await con.OpenAsync()

                Dim query As String = "UPDATE Products SET Name = @NewName, Price = @Price, Description = @Description, Stock = @Stock, Category = @Category, Brand = @Brand, Deals = @Deals, ImageURL = @ImageURL WHERE Name = @OriginalName"
                Using cmd As New OleDbCommand(query, con)
                    cmd.Parameters.Add("@NewName", OleDbType.VarChar).Value = txtName.Text
                    cmd.Parameters.Add("@Price", OleDbType.Currency).Value = Convert.ToDecimal(txtPrice.Text)
                    cmd.Parameters.Add("@Description", OleDbType.VarChar).Value = txtDescription.Text
                    cmd.Parameters.Add("@Stock", OleDbType.Integer).Value = If(String.IsNullOrWhiteSpace(txtStock.Text), 0, Convert.ToInt32(txtStock.Text))
                    Dim category As String = If(cmbCategory.SelectedItem IsNot Nothing, cmbCategory.SelectedItem.ToString(), "")
                    cmd.Parameters.Add("@Category", OleDbType.VarChar).Value = category
                    Dim brand As String = If(cmbBrand.SelectedItem IsNot Nothing, cmbBrand.SelectedItem.ToString(), "")
                    cmd.Parameters.Add("@Brand", OleDbType.VarChar).Value = brand
                    cmd.Parameters.Add("@Deals", OleDbType.Boolean).Value = chkDeals.Checked
                    cmd.Parameters.Add("@ImageURL", OleDbType.VarChar).Value = imageUrlToSave
                    cmd.Parameters.Add("@OriginalName", OleDbType.VarChar).Value = originalProductName

                    Await cmd.ExecuteNonQueryAsync()
                    MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            End Using

            ClearInputFields()
            Await LoadAllProductsAsync()

        Catch ex As Exception
            MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    '--------------------------------------------------------------------------------------------------------------------------------
    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        ' Checks if a product is selected
        If ListView1.SelectedItems.Count > 0 Then
            ' Retrieves the first selected product (you can select multiple, but here we only handle the first one)
            Dim selectedItem As ListViewItem = ListView1.SelectedItems(0)

            ' Assigns values to the controls in the GroupBox to allow editing
            txtName.Text = selectedItem.SubItems(0).Text ' Name
            txtDescription.Text = selectedItem.SubItems(1).Text ' Description
            cmbBrand.SelectedItem = selectedItem.SubItems(2).Text ' Brand
            cmbCategory.SelectedItem = selectedItem.SubItems(3).Text ' Category
            txtStock.Text = selectedItem.SubItems(4).Text ' Stock
            txtPrice.Text = selectedItem.SubItems(5).Text.Replace("$", "") ' Price (removing the dollar sign)
            Dim imagePath As String = selectedItem.SubItems(6).Text
            If IO.File.Exists(imagePath) Then
                picItem.Image = Image.FromFile(imagePath)
                lblOverlay.Visible = False
            Else
                picItem.Image = Nothing
                lblOverlay.Visible = True
                MessageBox.Show("Image was not found: " & imagePath)
            End If
        End If
    End Sub



    Private Async Sub btnAllProducts_Click(sender As Object, e As EventArgs) Handles btnAllProducts.Click
        Await LoadAllProductsAsync()
    End Sub
    '----------------------------------------------------ADD IMAGE------------------------------------------------------------------------
    Private Sub picItem_Click(sender As Object, e As EventArgs) Handles picItem.Click
        Using ofd As New OpenFileDialog
            ofd.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png"
            If ofd.ShowDialog() = DialogResult.OK Then
                selectedImagePath = ofd.FileName
                picItem.Image = Image.FromFile(selectedImagePath)
                picItem.SizeMode = PictureBoxSizeMode.StretchImage

                lblOverlay.Visible = False
            End If
        End Using
    End Sub

    ' Handles the KeyPress event for the txtStock and txtPrice textboxes
    ' Ensures that only numeric input (digits) or control characters (e.g., backspace) are allowed
    Private Sub txtNumberOnly_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtStock.KeyPress, txtPrice.KeyPress
        ' If the pressed key is neither a control character nor a digit, block the keypress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True ' Prevent the keypress from being registered
        End If
    End Sub

    ' Handles the FormClosing event for the Admin form
    ' Opens the MainMenu form when the Admin form is closed
    Private Sub Admin_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ' Create a new instance of the MainMenu form and show it
        Dim mainMenu As New MainMenu()
        mainMenu.Show()
    End Sub
End Class

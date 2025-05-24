Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.IO

Public Class MainMenu
    Private isMenuExpanded As Boolean = True
    Private isProductsExpanded As Boolean = False
    Private isCategoryExpanded As Boolean = False
    Private isBrandsExpanded As Boolean = False
    Private isFilterExpanded As Boolean = False
    Private isPriceExpanded As Boolean = False
    Private menuWidth As Integer
    Private productsSubMenuHeight As Integer
    Private categorySubMenuHeight As Integer
    Private brandsSubMenuHeight As Integer
    Private filterSubMenuHeight As Integer
    Private priceSubMenuHeight As Integer
    Public connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\Inventory_Store.accdb"
    Public savedItems As New List(Of List(Of String))
    Private cartItemQuantities As New Dictionary(Of String, Integer)

    Private Async Sub MainMenu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Store original sizes
        menuWidth = Menu.Width
        productsSubMenuHeight = Panel1.Height
        categorySubMenuHeight = Panel2.Height
        brandsSubMenuHeight = Panel3.Height
        ' Start with submenus collapsed
        CollapsePanel(Panel1)
        CollapsePanel(Panel2)
        CollapsePanel(Panel3)

        ' Calls method to display header for the listview
        MenuHeader()

        Await LoadProductsAsync("SELECT Name, Price, Description, Stock, Category, Brand FROM Products", New List(Of OleDbParameter)())
    End Sub

    ' This function returns a new database connection using the specified connection string.
    Private Function GetConnection() As OleDbConnection
        Return New OleDbConnection(connectionString)
    End Function

    ' Event handler for the SlideButton's Click event.
    ' It toggles the menu panel's width and updates the isMenuExpanded flag accordingly.
    Private Sub SlideButton_Click(sender As Object, e As EventArgs) Handles SlideButton.Click
        ToggleMenu(Menu, menuWidth, isMenuExpanded)
        isMenuExpanded = Not isMenuExpanded ' Toggle the menu state
    End Sub

    ' This subroutine animates the menu panel expansion or collapse.
    ' It gradually increases or decreases the panel width based on its current state.
    Private Sub ToggleMenu(panel As Panel, originalSize As Integer, ByRef isExpanded As Boolean)
        If isExpanded Then
            ' Collapse the menu by decreasing its width incrementally
            Do While panel.Width > 0
                panel.Width -= 10
                Application.DoEvents() ' Allows the UI to refresh during the loop
            Loop
        Else
            ' Expand the menu by increasing its width incrementally
            Do While panel.Width < originalSize
                panel.Width += 10
                Application.DoEvents() ' Allows the UI to refresh during the loop
            Loop
        End If
    End Sub

    ' This subroutine collapses a given panel by setting its height to 0.
    Private Sub CollapsePanel(panel As Panel)
        panel.Height = 0
    End Sub


    Public Sub MenuHeader()
        ' Display header for the listview
        ListView1.Clear()
        ListView1.View = View.Details
        ListView1.Columns.Add("Product", 120, HorizontalAlignment.Left)
        ListView1.Columns.Add("Description", 410, HorizontalAlignment.Left)
        ListView1.Columns.Add("Brand", 100, HorizontalAlignment.Left)
        ListView1.Columns.Add("Category", 100, HorizontalAlignment.Left)
        ListView1.Columns.Add("Stock", 75, HorizontalAlignment.Left)
        ListView1.Columns.Add("Price", 75, HorizontalAlignment.Left)
    End Sub

    ' Executes a database query asynchronously and returns a data reader.
    Private Async Function ExecuteQueryAsync(query As String, parameters As List(Of OleDbParameter)) As Task(Of OleDbDataReader)
        Dim con As OleDbConnection = GetConnection() ' Get a new database connection
        Dim cmd As New OleDbCommand(query, con) ' Create a command with the provided query and connection
        cmd.Parameters.AddRange(parameters.ToArray()) ' Add parameters to prevent SQL injection
        Await con.OpenAsync() ' Open the connection asynchronously
        Return Await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection) ' Execute reader and auto-close connection when done
    End Function

    ' Loads products from the database filtered by category.
    ' Clears the ListView and initiates the asynchronous product loading.
    Public Async Sub LoadProductsByCategory(category As String)
        ListView1.Items.Clear() ' Clear any existing items
        Dim query As String = "SELECT Name, Price, Description, Stock, Category, Brand FROM Products WHERE Category = @Category"
        Dim parameters As New List(Of OleDbParameter) From {
        New OleDbParameter("@Category", category) ' Parameterized query to prevent SQL injection
    }
        Await LoadProductsAsync(query, parameters) ' Load products using the shared method
    End Sub

    ' Loads products from the database filtered by brand.
    ' Clears the ListView and initiates the asynchronous product loading.
    Public Async Sub LoadProductsByBrands(brand As String)
        ListView1.Items.Clear() ' Clear any existing items
        Dim query As String = "SELECT Name, Price, Description, Stock, Category, Brand FROM Products WHERE Brand = @Brand"
        Dim parameters As New List(Of OleDbParameter) From {
        New OleDbParameter("@Brand", brand) ' Parameterized query to prevent SQL injection
    }
        Await LoadProductsAsync(query, parameters) ' Load products using the shared method
    End Sub

    ' Shared asynchronous method to load and display products based on a query and parameters.
    Private Async Function LoadProductsAsync(query As String, parameters As List(Of OleDbParameter)) As Task
        Try
            ' Execute the query and get a data reader
            Using reader As OleDbDataReader = Await ExecuteQueryAsync(query, parameters)
                ' Read each record asynchronously
                While Await reader.ReadAsync()
                    ' Create a new ListView item with product details
                    Dim item As New ListViewItem(reader("Name").ToString())
                    item.SubItems.Add(reader("Description").ToString())
                    item.SubItems.Add(reader("Brand").ToString())
                    item.SubItems.Add(reader("Category").ToString())
                    item.SubItems.Add(reader("Stock").ToString())

                    ' Format the price to two decimal places with a currency symbol
                    Dim price As Decimal = Convert.ToDecimal(reader("Price"))
                    item.SubItems.Add("$" & price.ToString("0.00"))

                    ' Add the item to the ListView
                    ListView1.Items.Add(item)
                End While
            End Using
        Catch ex As Exception
            ' Show an error message if something goes wrong during the operation
            MessageBox.Show("An error occurred while loading products: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    Public Sub LoadInventory()
        ' Loads all products into the ListView
        ListView1.Items.Clear()
        Dim con As New OleDb.OleDbConnection(connectionString)
        con.Open()
        Dim cmd As New OleDb.OleDbCommand("SELECT * FROM Products", con)
        Dim reader As OleDb.OleDbDataReader = cmd.ExecuteReader()
        While reader.Read()
            Dim item As New ListViewItem(reader("Name").ToString())
            item.SubItems.Add(reader("Description").ToString())
            item.SubItems.Add(reader("Brand").ToString())
            item.SubItems.Add(reader("Category").ToString())
            item.SubItems.Add(reader("Stock").ToString())
            item.SubItems.Add(reader("Price").ToString())
            ListView1.Items.Add(item)
        End While

        reader.Close()
        con.Close()
    End Sub

    ' Toggle Products submenu
    Private Sub Productsbtn_Click(sender As Object, e As EventArgs) Handles Productsbtn.Click
        ToggleSubMenu(Panel1, productsSubMenuHeight, isProductsExpanded, {AllProductbtn, Dealsbtn, InStockbtn})
        isProductsExpanded = Not isProductsExpanded
    End Sub

    ' Toggle Category submenu
    Private Sub Categorybtn_Click(sender As Object, e As EventArgs) Handles Categorybtn.Click
        ToggleSubMenu(Panel2, categorySubMenuHeight, isCategoryExpanded, {Accessoriescbtn, Audiobtn, Cablesbtn, Computerbtn, Gamingbtn, Mobilebtn})
        isCategoryExpanded = Not isCategoryExpanded
    End Sub

    ' Toggle Brands submenu
    Private Sub Brandsbtn_Click(sender As Object, e As EventArgs) Handles Brandsbtn.Click
        ToggleSubMenu(Panel3, brandsSubMenuHeight, isBrandsExpanded, {Applebtn, JBLbtn, Lenovobtn, samsungbtn, Sonybtn, Surf_onnbtn})
        isBrandsExpanded = Not isBrandsExpanded
    End Sub

    ' Expands/collapses submenu panel and shows/hides buttons
    Private Sub ToggleSubMenu(panel As Panel, originalHeight As Integer, ByRef isExpanded As Boolean, buttons As Button())
        If isExpanded Then
            Do While panel.Height > 0
                panel.Height -= 10
                Application.DoEvents()
            Loop
            For Each btn In buttons
                btn.Visible = False
            Next
        Else
            Do While panel.Height < originalHeight
                panel.Height += 10
                Application.DoEvents()
            Loop
            For Each btn In buttons
                btn.Visible = True
            Next
        End If
    End Sub


    ' Handles All Products, In Stock, and Deals button clicks
    Private Async Sub AllProductbtn_Click(sender As Object, e As EventArgs) Handles AllProductbtn.Click
        Await LoadFilteredProductsAsync("SELECT Name, Price, Description, Stock, Category, Brand FROM Products")
    End Sub

    Private Async Sub InStockbtn_Click(sender As Object, e As EventArgs) Handles InStockbtn.Click
        Await LoadFilteredProductsAsync("SELECT Name, Price, Description, Stock, Category, Brand FROM Products WHERE Stock > 0")
    End Sub

    Private Async Sub Dealsbtn_Click(sender As Object, e As EventArgs) Handles Dealsbtn.Click
        Await LoadFilteredProductsAsync("SELECT Name, Price, Description, Stock, Category, Brand FROM Products WHERE Deals = TRUE")
    End Sub

    ' Shared method to call MenuHeader and load products
    Private Async Function LoadFilteredProductsAsync(query As String) As Task
        MenuHeader()
        Await LoadProductsAsync(query, New List(Of OleDbParameter)())
    End Function

    ' Opens the cart form and reloads inventory
    Private Sub Cartbtn_Click(sender As Object, e As EventArgs) Handles Cartbtn.Click
        Dim cart As New CartForm(Me, connectionString)
        cart.AddCart(savedItems)
        cart.Show()
        Me.Hide()
        Me.LoadInventory()
    End Sub


    Private Sub AddButton_Click(sender As Object, e As EventArgs) Handles AddButton.Click
        If ListView1.SelectedItems.Count = 0 Then
            MessageBox.Show("No item was selected, Please choose an Item", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            Dim itemAdded As Boolean = False ' Flag to track if any item was added

            For Each selectedItem As ListViewItem In ListView1.SelectedItems
                ' Parse stock from subitem index 4
                Dim stock As Integer
                If Integer.TryParse(selectedItem.SubItems(4).Text, stock) AndAlso stock <= 0 Then
                    MessageBox.Show("Cannot add item: Item is out of stock.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Continue For ' Skip this item
                End If

                ' Ask user how many they want
                Dim itemName As String = selectedItem.Text
                Dim input As String = InputBox($"How many '{itemName}' would you like to add? (Max {stock})", "Select Quantity", "1")
                Dim quantityRequested As Integer
                If Not Integer.TryParse(input, quantityRequested) OrElse quantityRequested <= 0 Then
                    MessageBox.Show("Invalid quantity entered.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Continue For ' Skip 
                End If

                ' Check if requested quantity exceeds stock
                Dim currentInCart As Integer = 0
                If cartItemQuantities.ContainsKey(itemName) Then
                    currentInCart = cartItemQuantities(itemName)
                End If

                If (currentInCart + quantityRequested) > stock Then
                    MessageBox.Show($"Cannot add {quantityRequested} '{itemName}'. Only {stock - currentInCart} left in stock.", "Stock Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Continue For
                End If

                ' Update quantity in cart
                If cartItemQuantities.ContainsKey(itemName) Then
                    cartItemQuantities(itemName) += quantityRequested
                Else
                    cartItemQuantities(itemName) = quantityRequested
                End If

                ' Add multiple copies based on quantity requested
                For i As Integer = 1 To quantityRequested
                    ' Prepare item details for cart and receipt
                    Dim itemDetails As New List(Of String)
                    For j As Integer = 0 To selectedItem.SubItems.Count - 1
                        itemDetails.Add(selectedItem.SubItems(j).Text)
                    Next
                    savedItems.Add(itemDetails)

                    ' Receipt items
                    Dim receiptItem As New ListViewItem(selectedItem.Text)
                    For j As Integer = 1 To selectedItem.SubItems.Count - 1
                        receiptItem.SubItems.Add(selectedItem.SubItems(5).Text) ' Only repeating price
                    Next
                    ReceiptForm.listRec.Items.Add(receiptItem)
                Next

                itemAdded = True ' Mark that at least one item was added
            Next

            If itemAdded Then
                MessageBox.Show("Item(s) was added to cart!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    ' Handles the search button click and filters products based on user input
    Private Async Sub Searchbtn_Click(sender As Object, e As EventArgs) Handles Searchbtn.Click
        Dim searchText As String = "%" & searchBox.Text.Trim().ToLower() & "%" ' Add wildcard to search term
        ListView1.Items.Clear() ' Clear previous results
        Dim query As String = "SELECT Name, Price, Description, Stock, Category, Brand FROM Products WHERE Name LIKE @SearchText OR Category LIKE @SearchText OR Brand LIKE @SearchText"
        Dim parameters As New List(Of OleDbParameter) From {New OleDbParameter("@SearchText", searchText)}
        Await LoadProductsAsync(query, parameters) ' Load filtered products
    End Sub


    Private Sub Accessoriescbtn_Click(sender As Object, e As EventArgs) Handles Accessoriescbtn.Click
        LoadProductsByCategory("Accessories")
    End Sub

    Private Sub Audiobtn_Click(sender As Object, e As EventArgs) Handles Audiobtn.Click
        LoadProductsByCategory("Audio")
    End Sub

    Private Sub Cablesbtn_Click(sender As Object, e As EventArgs) Handles Cablesbtn.Click
        LoadProductsByCategory("Cables")
    End Sub

    Private Sub Computerbtn_Click(sender As Object, e As EventArgs) Handles Computerbtn.Click
        LoadProductsByCategory("Computers")
    End Sub

    Private Sub Gamingbtn_Click(sender As Object, e As EventArgs) Handles Gamingbtn.Click
        LoadProductsByCategory("Gaming")
    End Sub

    Private Sub Mobilebtn_Click(sender As Object, e As EventArgs) Handles Mobilebtn.Click
        LoadProductsByCategory("Mobile Devices")
    End Sub

    Private Sub Applebtn_Click(sender As Object, e As EventArgs) Handles Applebtn.Click
        LoadProductsByBrands("Apple")
    End Sub

    Private Sub JBLbtn_Click(sender As Object, e As EventArgs) Handles JBLbtn.Click
        LoadProductsByBrands("JBL")
    End Sub

    Private Sub Lenovobtn_Click(sender As Object, e As EventArgs) Handles Lenovobtn.Click
        LoadProductsByBrands("Lenovo")
    End Sub

    Private Sub samsungbtn_Click(sender As Object, e As EventArgs) Handles samsungbtn.Click
        LoadProductsByBrands("Samsung")
    End Sub

    Private Sub Sonybtn_Click(sender As Object, e As EventArgs) Handles Sonybtn.Click
        LoadProductsByBrands("Sony")
    End Sub

    Private Sub Surf_onnbtn_Click(sender As Object, e As EventArgs) Handles Surf_onnbtn.Click
        LoadProductsByBrands("Surf onn")
    End Sub

    ' Sorts the ListView items by price in descending order
    Private Sub Highestbtn_Click(sender As Object, e As EventArgs) Handles highestbtn.Click
        SortItemsByPrice(False) ' False for descending order
    End Sub

    ' Sorts the ListView items by price in ascending order
    Private Sub Lowestbtn_Click(sender As Object, e As EventArgs) Handles lowestbtn.Click
        SortItemsByPrice(True) ' True for ascending order
    End Sub

    ' Helper method to sort ListView items by price (ascending/descending)
    Private Sub SortItemsByPrice(isAscending As Boolean)
        Dim items As List(Of ListViewItem) = ListView1.Items.Cast(Of ListViewItem)().ToList()

        items.Sort(Function(x, y)
                       Dim priceX As Decimal = Convert.ToDecimal(x.SubItems(5).Text.Replace("$", ""))
                       Dim priceY As Decimal = Convert.ToDecimal(y.SubItems(5).Text.Replace("$", ""))
                       Return If(isAscending, priceX.CompareTo(priceY), priceY.CompareTo(priceX)) ' Sort based on direction
                   End Function)

        ListView1.Items.Clear()
        ListView1.Items.AddRange(items.ToArray())
    End Sub


    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count > 0 Then
            Dim selectedItem As ListViewItem = ListView1.SelectedItems(0)
            Dim productName As String = selectedItem.SubItems(0).Text ' adjust based on your columns

            ' Fetch full details from the database
            Dim query As String = "SELECT * FROM Products WHERE Name = @Name"
            Using conn As New OleDbConnection(connectionString)
                Using cmd As New OleDbCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Name", productName)
                    conn.Open()
                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            lblProduct.Text = reader("Name").ToString()
                            lblBrand.Text = reader("Brand").ToString()
                            lblCategory.Text = reader("Category").ToString()
                            lblPrice.Text = "$" & reader("Price").ToString()
                            lblStock.Text = "Stock: " & reader("Stock").ToString()
                            lblDescription.Text = reader("Description").ToString()

                            ' If image is stored as byte array (OLE Object)
                            Dim imagePath As String = reader("ImageURL").ToString().Trim()
                            If Not String.IsNullOrEmpty(imagePath) Then
                                Dim fullPath As String = Path.Combine(Application.StartupPath, imagePath)
                                If IO.File.Exists(fullPath) Then
                                    ProductPictureBox.Image = Image.FromFile(fullPath)
                                Else
                                    MessageBox.Show("Image not found at: " & fullPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    ProductPictureBox.Image = Nothing
                                End If
                            Else
                                MessageBox.Show("No image path provided for this product.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                ProductPictureBox.Image = Nothing
                            End If
                        End If
                    End Using
                End Using
            End Using
        End If
    End Sub

    Private Sub LoadProductImage(productName As String)
        ' Use Using block to manage database connection and command lifecycle
        Using conn As New OleDbConnection(connectionString)
            conn.Open()

            ' Query to retrieve the image URL for the given product name
            Dim query As String = "SELECT ImageURL FROM Products WHERE Name LIKE @Name"
            Using cmd As New OleDbCommand(query, conn)
                cmd.Parameters.AddWithValue("@Name", "%" & productName & "%")

                ' Execute the query and check if the image exists for the product
                Using reader As OleDbDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        Dim imagePath As String = reader("ImageURL").ToString()

                        ' Check if the file exists and display the image, otherwise show a message
                        If File.Exists(imagePath) Then
                            ProductPictureBox.Image = Image.FromFile(imagePath)
                        Else
                            ProductPictureBox.Image = Nothing
                            MessageBox.Show($"Image not found at: {imagePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Else
                        ' No image found for the product
                        ProductPictureBox.Image = Nothing
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Sub ProductPictureBox_Click(sender As Object, e As EventArgs) Handles ProductPictureBox.Click

    End Sub

    Private Sub LoginButton_Click(sender As Object, e As EventArgs) Handles btnlog.Click
        Dim loginForm As New LoginForm()
        If loginForm.ShowDialog() = DialogResult.OK Then
            ' Check the role after login
            If loginForm.UserRole = "Admin" Then
                ' Open the admin panel to allow database modifications
                Dim adminForm As New AdminForm()
                adminForm.ShowDialog()
                Me.Hide()
            Else
                ' For customers, simply continue to the main menu
                MessageBox.Show("Welcome, Customer!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    ' Shows a message box with support contact information when the support button is clicked
    Private Sub supportbtn_Click(sender As Object, e As EventArgs) Handles supportbtn.Click
        MessageBox.Show("For any Inconvenience, please communicate through our email at support@Inv.com. Alternatively you can also contact our support center by phone at 787-xxx-xxxx from 8 am to 10 pm Mondays to Fridays.", "Support", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub
    ' Handles form closing event and exits the application
    Private Sub MainMenu_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Application.Exit()
    End Sub
End Class
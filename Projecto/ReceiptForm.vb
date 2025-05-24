Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class ReceiptForm
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Dim userInput As DialogResult
        userInput = MessageBox.Show("Do you want to make another purchase?", "Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If userInput = DialogResult.Yes Then
            MainMenu.Show() ' Takes you back to the main menu form and closes the receipt form
            Me.Close()
        Else
            Application.Exit() ' Ends the program
        End If
    End Sub

    Private Sub ReceiptForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Sets the view mode of the ListView to Details (to display columns)
        listRec.View = View.Details

        ' Adds the "Product" column with a width of 150 and left-aligned text
        listRec.Columns.Add("Product", 150, HorizontalAlignment.Left)

        ' Adds the "Price" column with a width of 60 and left-aligned text
        listRec.Columns.Add("Price", 60, HorizontalAlignment.Left)

        ' Loops through all columns and sets their width automatically
        For Each column As ColumnHeader In listRec.Columns
            column.Width = -2 ' Auto-resizes the column to fit its content
        Next
    End Sub

End Class
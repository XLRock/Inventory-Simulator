Imports System.Data.OleDb

Public Class LoginForm

    ' Use the same connection string as your main project.
    Public connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\Inventory_Store.accdb"


    ' Property to expose the logged-in user's role to the calling form.
    Public ReadOnly Property UserRole As String
        Get
            Return _userRole
        End Get
    End Property
    Private _userRole As String = "Admin"

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLog.Click
        Dim username As String = txtUsername.Text.Trim()
        Dim password As String = txtPassword.Text.Trim()

        Using con As New OleDbConnection(connectionString)
            Dim query As String = "SELECT Role FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash"
            Dim cmd As New OleDbCommand(query, con)
            cmd.Parameters.AddWithValue("@Email", username)
            cmd.Parameters.AddWithValue("@PasswordHash", password)

            con.Open()
            Dim roleObj As Object = cmd.ExecuteScalar() ' Returns the role if the credentials match
            If roleObj IsNot Nothing Then
                _userRole = roleObj.ToString()
                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.DialogResult = DialogResult.OK
                MainMenu.Hide()
                Me.Close()
            Else
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Using
    End Sub

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Me.Close()
    End Sub


    Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPassword.CheckedChanged
        If chkShowPassword.Checked Then
            txtPassword.PasswordChar = ControlChars.NullChar ' shows the password
        Else
            txtPassword.PasswordChar = "*"c ' hides the password
        End If
    End Sub
End Class

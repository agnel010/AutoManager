Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel
Imports MySql.Data.MySqlClient

Public Class Form1
    Private connectionString As String = "server=localhost;username=root;password=101010;database=automanager"

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim username As String = TextBox1.Text
        Dim password As String = TextBox2.Text

        ' Validate input
        If Not ValidateInput(username, password) Then
            Return
        End If

        ' Check login credentials
        If CheckLogin(username, password) Then
            MessageBox.Show("Login Successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Form2.Show()
            Me.Hide()
        Else
            MessageBox.Show("Invalid username or password. Please try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Function ValidateInput(username As String, password As String) As Boolean
        If String.IsNullOrWhiteSpace(username) Then
            MessageBox.Show("Username cannot be empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If String.IsNullOrWhiteSpace(password) Then
            MessageBox.Show("Password cannot be empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True
    End Function

    Private Function CheckLogin(username As String, password As String) As Boolean
        Using conn As New MySqlConnection(connectionString)
            Try
                conn.Open()

                Dim query As String = "SELECT COUNT(*) FROM admin WHERE username = @username AND password = @password"
                Dim cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@username", username)
                cmd.Parameters.AddWithValue("@password", password)

                Dim result As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                Return result > 0
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try
        End Using
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        TextBox1.Text = ""
        TextBox2.Text = ""
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If Not String.IsNullOrEmpty(TextBox1.Text) AndAlso Not TextBox1.Text.All(Function(c) Char.IsLetter(c)) Then
            MessageBox.Show("Error: Only alphabets are allowed in Username", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox1.Text = ""
        End If
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        If Not String.IsNullOrEmpty(TextBox2.Text) AndAlso Not TextBox2.Text.All(Function(c) Char.IsDigit(c)) Then
            MessageBox.Show("Error: Only numbers are allowed in Password", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox2.Text = ""
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
    End Sub
End Class

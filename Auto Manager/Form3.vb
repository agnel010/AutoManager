Imports System.Security.Cryptography
Imports MySql.Data.MySqlClient
Public Class Form3
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load data into DataGridView when the form loads
        LoadDataIntoDataGridView()
        Me.WindowState = FormWindowState.Maximized
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not String.IsNullOrEmpty(TextBox7.Text) AndAlso Not (TextBox7.Text.Any(Function(c) Char.IsLetter(c)) AndAlso TextBox7.Text.Any(Function(c) Char.IsDigit(c))) Then
            MessageBox.Show("Please enter both alphabetic and numeric characters in Vehicle ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            SaveData()
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Not String.IsNullOrEmpty(TextBox7.Text) AndAlso Not (TextBox7.Text.Any(Function(c) Char.IsLetter(c)) AndAlso TextBox7.Text.Any(Function(c) Char.IsDigit(c))) Then
            MessageBox.Show("Please enter both alphabetic and numeric characters in Vehicle ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            ' Check if a row is selected in the DataGridView
            If DataGridView1.SelectedRows.Count > 0 Then
                ' Get the selected row
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

                ' Get the vid of the selected row
                Dim vid As String = selectedRow.Cells("vid").Value.ToString()

                ' Call the method to update the record in the database
                UpdateRecordInDatabase(vid)

                ' Refresh DataGridView to reflect the changes
                LoadDataIntoDataGridView()
            Else
                MessageBox.Show("Please select a row to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End If
    End Sub

    Private Sub UpdateRecordInDatabase(vid As String)
        Dim connectionString As String = "server=localhost;user=root;password=101010;database=automanager"
        Dim query As String = "UPDATE customer SET first_name = @first_name, last_name = @last_name, contact_no = @contact_no, model = @model, year = @year, gender = @gender WHERE vid = @vid"

        Try
            Using connection As New MySqlConnection(connectionString)
                Using command As New MySqlCommand(query, connection)
                    ' Parameters
                    command.Parameters.AddWithValue("@vid", vid)
                    command.Parameters.AddWithValue("@first_name", TextBox1.Text)
                    command.Parameters.AddWithValue("@last_name", TextBox2.Text)
                    command.Parameters.AddWithValue("@contact_no", TextBox3.Text)
                    command.Parameters.AddWithValue("@model", TextBox4.Text)
                    command.Parameters.AddWithValue("@year", TextBox5.Text)

                    Dim gender As String = ""
                    If RadioButton1.Checked Then
                        gender = "Male"
                    ElseIf RadioButton2.Checked Then
                        gender = "Female"
                    ElseIf RadioButton3.Checked Then
                        gender = "Other"
                    End If
                    command.Parameters.AddWithValue("@gender", gender)
                    connection.Open()
                    Dim rowsAffected As Integer = command.ExecuteNonQuery()

                    ' Check if the update was successful
                    If rowsAffected > 0 Then
                        MessageBox.Show("Record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show("Failed to update record.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        DeleteData()
    End Sub

    Private Sub LoadDataIntoDataGridView()
        Dim connectionString As String = "server=localhost;username=root;password=101010;database=automanager"
        Dim query As String = "SELECT * FROM customer"

        Using connection As New MySqlConnection(connectionString)
            Using command As New MySqlCommand(query, connection)
                Dim dataTable As New DataTable()
                Dim dataAdapter As New MySqlDataAdapter(command)
                dataAdapter.Fill(dataTable)
                DataGridView1.DataSource = dataTable
            End Using
        End Using
    End Sub

    Private Sub SaveData()
        Dim connectionString As String = "server=localhost;username=root;password=101010;database=automanager"
        Dim query As String = "INSERT INTO customer (vid, first_name, last_name, contact_no, model, year, gender) VALUES (@vid, @first_name, @last_name, @contact_no, @model, @year, @gender)"

        If ValidateInput() Then
            Using connection As New MySqlConnection(connectionString)
                Using command As New MySqlCommand(query, connection)
                    ' Parameters
                    command.Parameters.AddWithValue("@vid", TextBox7.Text)
                    command.Parameters.AddWithValue("@first_name", TextBox1.Text)
                    command.Parameters.AddWithValue("@last_name", TextBox2.Text)
                    command.Parameters.AddWithValue("@contact_no", TextBox3.Text)
                    command.Parameters.AddWithValue("@model", TextBox4.Text)
                    command.Parameters.AddWithValue("@year", TextBox5.Text)

                    Dim gender As String = ""
                    If RadioButton1.Checked Then
                        gender = "Male"
                    ElseIf RadioButton2.Checked Then
                        gender = "Female"
                    ElseIf RadioButton3.Checked Then
                        gender = "Other"
                    End If
                    command.Parameters.AddWithValue("@gender", gender)

                    ' Open the connection and execute the query
                    connection.Open()
                    Dim rowsAffected As Integer = command.ExecuteNonQuery()

                    ' Check if the insert was successful
                    If rowsAffected > 0 Then
                        MessageBox.Show("Record inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        ' Optionally, clear the textboxes after successful insert
                        ClearTextBoxes()
                    Else
                        MessageBox.Show("Failed to insert record.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    LoadDataIntoDataGridView()
                End Using
            End Using
        End If
    End Sub
    Private Sub DeleteRecordFromDatabase(vid As String)
        Dim connectionString As String = "server=localhost;username=root;password=101010;database=automanager"
        Dim query As String = "DELETE FROM customer WHERE vid = @vid"

        Using connection As New MySqlConnection(connectionString)
            Using command As New MySqlCommand(query, connection)
                ' Parameters
                command.Parameters.AddWithValue("@vid", vid)

                ' Open the connection and execute the query
                connection.Open()
                Dim rowsAffected As Integer = command.ExecuteNonQuery()

                ' Check if the deletion was successful
                If rowsAffected > 0 Then
                    MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("Failed to delete record.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End Using
        End Using
    End Sub
    Private Sub DeleteData()
        If DataGridView1.SelectedRows.Count > 0 Then
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                ' Get the selected row
                Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

                ' Get the vid of the selected row
                Dim vid As String = selectedRow.Cells("vid").Value.ToString()

                ' Delete the record from the database
                DeleteRecordFromDatabase(vid)

                ' Refresh DataGridView to reflect the changes
                LoadDataIntoDataGridView()
            End If
        Else
            MessageBox.Show("Please select a row to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub


    Private Function ValidateInput() As Boolean
        If String.IsNullOrWhiteSpace(TextBox1.Text) OrElse String.IsNullOrWhiteSpace(TextBox2.Text) _
                OrElse String.IsNullOrWhiteSpace(TextBox3.Text) OrElse String.IsNullOrWhiteSpace(TextBox4.Text) _
                OrElse String.IsNullOrWhiteSpace(TextBox5.Text) OrElse String.IsNullOrWhiteSpace(TextBox7.Text) Then
            MessageBox.Show("Fill every details", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If Not IsNumeric(TextBox5.Text) Then
            MessageBox.Show("Year must be a number", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If Not IsNumeric(TextBox3.Text) Then
            MessageBox.Show("Contact must be a number", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If Not (RadioButton1.Checked OrElse RadioButton2.Checked OrElse RadioButton3.Checked) Then
            MessageBox.Show("Please select a gender", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True
    End Function

    Private Sub ClearTextBoxes()
        TextBox7.Clear()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
        RadioButton1.Checked = False
        RadioButton2.Checked = False
        RadioButton3.Checked = False
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If Not String.IsNullOrWhiteSpace(TextBox1.Text) AndAlso Not TextBox1.Text.All(Function(c) Char.IsLetter(c) Or c = " ") Then
            MessageBox.Show("Error: Only alphabets are allowed in first name", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox1.Text = ""
        End If
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        If Not String.IsNullOrWhiteSpace(TextBox2.Text) AndAlso Not TextBox2.Text.All(Function(c) Char.IsLetter(c) Or c = " ") Then
            MessageBox.Show("Error: Only alphabets are allowed in last name", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox2.Text = ""
        End If
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        Dim maxDigits As Integer = 10 ' Define the maximum number of digits allowed for the contact number

        If TextBox3.Text.Length > maxDigits Then
            MessageBox.Show("Error: Maximum " & maxDigits & " digits allowed for the contact number", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox3.Text = TextBox3.Text.Substring(0, maxDigits) ' Trim the input to the maximum allowed digits
        ElseIf Not String.IsNullOrWhiteSpace(TextBox3.Text) AndAlso Not TextBox3.Text.All(Function(c) Char.IsDigit(c)) Then
            MessageBox.Show("Error: Only numbers are allowed in contact", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox3.Text = ""
        End If
    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        If Not String.IsNullOrWhiteSpace(TextBox4.Text) AndAlso Not TextBox4.Text.All(Function(c) Char.IsDigit(c) OrElse Char.IsLetter(c) Or c = " ") Then
            MessageBox.Show("Error: Only alphabets, numbers, and spaces are allowed in Model", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox4.Text = ""
        End If
    End Sub

    Private Sub TextBox5_TextChanged(sender As Object, e As EventArgs) Handles TextBox5.TextChanged
        Dim maxDigits As Integer = 4 ' Define the maximum number of digits allowed for the year

        If TextBox5.Text.Length > maxDigits Then
            MessageBox.Show("Error: Maximum " & maxDigits & " digits allowed for the year", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox5.Text = TextBox5.Text.Substring(0, maxDigits) ' Trim the input to the maximum allowed digits
        ElseIf Not String.IsNullOrWhiteSpace(TextBox5.Text) AndAlso Not TextBox5.Text.All(Function(c) Char.IsDigit(c)) Then
            MessageBox.Show("Error: Only numbers are allowed in year", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox5.Text = ""
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ClearTextBoxes()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        ' Check if the clicked cell is not the new row
        If e.RowIndex >= 0 AndAlso e.RowIndex < DataGridView1.Rows.Count Then
            Dim selectedRow As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            TextBox7.Text = selectedRow.Cells("vid").Value.ToString()
            TextBox1.Text = selectedRow.Cells("first_name").Value.ToString()
            TextBox2.Text = selectedRow.Cells("last_name").Value.ToString()
            TextBox3.Text = selectedRow.Cells("contact_no").Value.ToString()
            TextBox4.Text = selectedRow.Cells("model").Value.ToString()
            TextBox5.Text = selectedRow.Cells("year").Value.ToString()

            Dim gender As String = selectedRow.Cells("gender").Value.ToString()
            Select Case gender
                Case "Male"
                    RadioButton1.Checked = True
                Case "Female"
                    RadioButton2.Checked = True
                Case "Other"
                    RadioButton3.Checked = True
            End Select
        End If
    End Sub
End Class

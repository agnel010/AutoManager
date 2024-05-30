Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button
Imports MySql.Data.MySqlClient

Public Class Form6
    Private connectionString As String = "server=localhost;username=root;password=101010;database=automanager"

    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Assuming ComboBox name is ComboBox1
        ComboBox1.Items.Add("Cash")
        ComboBox1.Items.Add("Card")
        ' Populate Schedule ID ComboBox
        LoadScheduleIDs()
        LoadDataIntoDataGridView() ' Load data into DataGridView when the form loads
        Me.WindowState = FormWindowState.Maximized
        ' Load vehicle IDs into ComboBox3
        LoadVehicleIDs()
    End Sub
    Private Sub LoadVehicleIDs()
        ComboBox3.Items.Clear()
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim query As String = "SELECT vid FROM schedule"
                Using command As New MySqlCommand(query, connection)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            ComboBox3.Items.Add(reader("vid").ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading Vehicle IDs: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = "" OrElse ComboBox2.Text = "" OrElse ComboBox3.Text = "" OrElse TextBox5.Text = "" OrElse DateTimePicker1.Value = DateTime.MinValue OrElse ComboBox1.Text = "" OrElse TextBox6.Text = "" OrElse (RadioButton1.Checked = False And RadioButton2.Checked = False) Then
            MessageBox.Show("Fill every detail", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

        ElseIf Not String.IsNullOrEmpty(TextBox1.Text) AndAlso Not (TextBox1.Text.Any(Function(c) Char.IsLetter(c)) AndAlso TextBox1.Text.Any(Function(c) Char.IsDigit(c))) Then
            MessageBox.Show("Please enter both alphabetic and numeric characters in Invoice ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

        ElseIf Not String.IsNullOrEmpty(comboBox3.Text) AndAlso Not (comboBox3.Text.Any(Function(c) Char.IsLetter(c)) AndAlso combobox3.Text.Any(Function(c) Char.IsDigit(c))) Then
            MessageBox.Show("Please enter both alphabetic and numeric characters in Schedule ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            ' Check if the invoice ID already exists in the database
            If Not InvoiceIDExists(TextBox1.Text) Then
                ' If the invoice ID doesn't exist, insert the data
                InsertData()
                MessageBox.Show("Details entered successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadDataIntoDataGridView() ' Reload DataGridView
            Else
                MessageBox.Show("Invoice ID already exists. Please use a different ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End If
    End Sub

    Private Function InvoiceIDExists(invoiceID As String) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM billing WHERE invoice_id = @invoice_id"
        Dim count As Integer = 0

        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@invoice_id", invoiceID)
                    count = Convert.ToInt32(command.ExecuteScalar())
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error checking invoice ID existence: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return count > 0
    End Function
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        TextBox1.Text = ""
        ComboBox3.Text = ""
        TextBox5.Text = ""
        TextBox6.Text = ""
        DateTimePicker1.Value = DateTime.Now
        ComboBox1.SelectedIndex = -1 ' Clear the selected index
        ComboBox2.SelectedIndex = -1 ' Clear the selected index
        RadioButton1.Checked = False
        RadioButton2.Checked = False
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub LoadScheduleIDs()
        ComboBox2.Items.Clear()
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim query As String = "SELECT schedule_id FROM schedule"
                Using command As New MySqlCommand(query, connection)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            ComboBox2.Items.Add(reader("schedule_id").ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading Schedule IDs: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedItem IsNot Nothing Then
            Dim selectedScheduleID As String = ComboBox2.SelectedItem.ToString()
            Dim query As String = "SELECT vid FROM schedule WHERE schedule_id = @scheduleID"

            ' Fetch corresponding Vehicle ID from database
            Try
                Using connection As New MySqlConnection(connectionString)
                    connection.Open()
                    Using command As New MySqlCommand(query, connection)
                        command.Parameters.AddWithValue("@scheduleID", selectedScheduleID)
                        Dim result As Object = command.ExecuteScalar()
                        If result IsNot Nothing Then
                            ComboBox3.SelectedItem = result.ToString()
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error fetching Vehicle ID: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            ' Fetch corresponding service type and amount
            FetchServiceDetails(selectedScheduleID)
        Else
            ' Handle the case when no item is selected in ComboBox2
            ' For example, you might want to clear ComboBox3 and other related fields
            ComboBox3.SelectedIndex = -1
            ' You might also want to clear or reset other fields related to service type and amount
        End If
    End Sub

    Private Sub FetchServiceDetails(scheduleID As String)
        Dim query As String = "SELECT service_type, st.amount FROM schedule s JOIN service_type st ON s.service_id = st.Service_id WHERE s.schedule_id = @scheduleID"

        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@scheduleID", scheduleID)
                    Dim reader As MySqlDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        TextBox5.Text = reader("service_type").ToString()
                        TextBox6.Text = reader("amount").ToString()
                    Else
                        MessageBox.Show("No service details found for the provided schedule ID.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error fetching service details: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub InsertData()
        Dim paymentStatus As String = If(RadioButton1.Checked, "Paid", "Unpaid")
        Dim query As String = "INSERT INTO billing (invoice_id, vid, service_type, schedule_id, issue_date, payment_mode, amount, payment_status) VALUES (@invoice_id, @vehicle_id, @service_type, @schedule_id, @issue_date, @payment_mode, @amount, @payment_status)"

        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@invoice_id", TextBox1.Text)
                    command.Parameters.AddWithValue("@vehicle_id", ComboBox3.Text)
                    command.Parameters.AddWithValue("@service_type", TextBox5.Text)
                    command.Parameters.AddWithValue("@schedule_id", ComboBox2.SelectedItem.ToString())
                    command.Parameters.AddWithValue("@issue_date", DateTimePicker1.Value)
                    command.Parameters.AddWithValue("@payment_mode", ComboBox1.SelectedItem.ToString())
                    command.Parameters.AddWithValue("@amount", Decimal.Parse(TextBox6.Text))
                    command.Parameters.AddWithValue("@payment_status", paymentStatus)
                    command.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error inserting data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub LoadDataIntoDataGridView()
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM billing"
                Using command As New MySqlCommand(query, connection)
                    Using adapter As New MySqlDataAdapter(command)
                        Dim dataTable As New DataTable()
                        adapter.Fill(dataTable)
                        DataGridView1.DataSource = dataTable
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading data into DataGridView: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Handles the CellContentClick event of the DataGridView to trigger edit action
    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = DataGridView1.Columns("EditButtonColumn").Index AndAlso e.RowIndex >= 0 Then
            ' Populate the fields with the selected row's data
            PopulateFieldsWithRowData(e.RowIndex)
        End If
    End Sub

    ' Populate the fields with the selected row's data
    Private Sub PopulateFieldsWithRowData(rowIndex As Integer)
        Dim row As DataGridViewRow = DataGridView1.Rows(rowIndex)
        TextBox1.Text = row.Cells("InvoiceIDColumn").Value.ToString()
        ComboBox3.Text = row.Cells("VehicleIDColumn").Value.ToString()
        TextBox5.Text = row.Cells("ServiceTypeColumn").Value.ToString()
        ComboBox2.SelectedItem = row.Cells("ScheduleIDColumn").Value.ToString()
        DateTimePicker1.Value = DateTime.Parse(row.Cells("IssueDateColumn").Value.ToString())
        ComboBox1.SelectedItem = row.Cells("PaymentModeColumn").Value.ToString()
        TextBox6.Text = row.Cells("AmountColumn").Value.ToString()
        If row.Cells("PaymentStatusColumn").Value.ToString() = "Paid" Then
            RadioButton1.Checked = True
        Else
            RadioButton2.Checked = True
        End If
    End Sub

    ' Update the data in the database
    Private Sub UpdateData()
        Dim paymentStatus As String = If(RadioButton1.Checked, "Paid", "Unpaid")
        Dim query As String = "UPDATE billing SET service_type = @service_type, schedule_id = @schedule_id, issue_date = @issue_date, payment_mode = @payment_mode, amount = @amount, payment_status = @payment_status WHERE invoice_id = @invoice_id AND vid = @vehicle_id"

        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@service_type", TextBox5.Text)
                    command.Parameters.AddWithValue("@schedule_id", ComboBox2.SelectedItem.ToString())
                    command.Parameters.AddWithValue("@issue_date", DateTimePicker1.Value)
                    command.Parameters.AddWithValue("@payment_mode", ComboBox1.SelectedItem.ToString())
                    command.Parameters.AddWithValue("@amount", Decimal.Parse(TextBox6.Text))
                    command.Parameters.AddWithValue("@payment_status", paymentStatus)
                    command.Parameters.AddWithValue("@invoice_id", TextBox1.Text)
                    command.Parameters.AddWithValue("@vehicle_id", ComboBox3.SelectedItem.ToString())
                    command.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error updating data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' Handles the click event of the Update button to update the data
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If TextBox1.Text = "" OrElse ComboBox2.Text = "" OrElse ComboBox3.Text = "" OrElse TextBox5.Text = "" OrElse DateTimePicker1.Value = DateTime.MinValue OrElse ComboBox1.Text = "" OrElse TextBox6.Text = "" OrElse (RadioButton1.Checked = False And RadioButton2.Checked = False) Then
            MessageBox.Show("Fill every detail", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

        ElseIf Not String.IsNullOrEmpty(TextBox1.Text) AndAlso Not (TextBox1.Text.Any(Function(c) Char.IsLetter(c)) AndAlso TextBox1.Text.Any(Function(c) Char.IsDigit(c))) Then
            MessageBox.Show("Please enter both alphabetic and numeric characters in Invoice ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)

        ElseIf Not String.IsNullOrEmpty(comboBox3.Text) AndAlso Not (comboBox3.Text.Any(Function(c) Char.IsLetter(c)) AndAlso comboBox3.Text.Any(Function(c) Char.IsDigit(c))) Then
            MessageBox.Show("Please enter both alphabetic and numeric characters in Schedule ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            UpdateData()
            LoadDataIntoDataGridView() ' Reload DataGridView after updating data
            MessageBox.Show("Details updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then ' Check if the clicked cell is within a row
            Dim selectedRow As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            ' Populate the textboxes, combo boxes, and radio buttons with the selected row's data
            TextBox1.Text = selectedRow.Cells("invoice_id").Value.ToString()
            ComboBox3.Text = selectedRow.Cells("vid").Value.ToString()
            TextBox5.Text = selectedRow.Cells("service_type").Value.ToString()
            ComboBox2.SelectedItem = selectedRow.Cells("schedule_id").Value.ToString()
            ' Check if the issue date is valid before setting the value
            Dim issueDate As DateTime
            If DateTime.TryParse(selectedRow.Cells("issue_date").Value.ToString(), issueDate) AndAlso issueDate > DateTime.Now Then
                DateTimePicker1.Value = issueDate
            Else
                ' If issue date is invalid or before the system date, set it to the current date
                DateTimePicker1.Value = DateTime.Now
            End If
            ComboBox1.SelectedItem = selectedRow.Cells("payment_mode").Value.ToString()
            TextBox6.Text = selectedRow.Cells("amount").Value.ToString()

            Dim paymentStatus As String = selectedRow.Cells("payment_status").Value.ToString()
            If paymentStatus = "Paid" Then
                RadioButton1.Checked = True
            ElseIf paymentStatus = "Unpaid" Then
                RadioButton2.Checked = True
            End If
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If DataGridView1.SelectedRows.Count > 0 Then ' Check if any row is selected
            Dim selectedRowIndex As Integer = DataGridView1.SelectedRows(0).Index
            Dim selectedInvoiceID As String = DataGridView1.Rows(selectedRowIndex).Cells("invoice_id").Value.ToString()

            ' Delete the record from the database
            Dim query As String = "DELETE FROM billing WHERE invoice_id = @invoice_id"

            Try
                Using connection As New MySqlConnection(connectionString)
                    connection.Open()
                    Using command As New MySqlCommand(query, connection)
                        command.Parameters.AddWithValue("@invoice_id", selectedInvoiceID)
                        command.ExecuteNonQuery()
                    End Using
                End Using

                ' Remove the selected row from the DataGridView
                DataGridView1.Rows.RemoveAt(selectedRowIndex)

                MessageBox.Show("Data deleted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Error deleting data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Please select a row to delete", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged
        Dim selectedDate As DateTime = DateTimePicker1.Value
        Dim systemDate As DateTime = DateTime.Now

        If selectedDate < systemDate Then
            MessageBox.Show("Please select a date on or after the system date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            DateTimePicker1.Value = systemDate.AddDays(1)
        End If
    End Sub
End Class

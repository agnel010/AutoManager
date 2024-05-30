Imports System.Windows.Forms.VisualStyles
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form5
    Private connectionString As String = "server=localhost;username=root;password=101010;database=automanager"
    Private connection As MySqlConnection
    Private rowIndex As Integer = -1
    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        connection = New MySqlConnection(connectionString)
        Try
            connection.Open()
            ClearComboBoxes() ' Clear ComboBoxes before loading values
            LoadComboBoxValues() ' Load ComboBox values from database
            RefreshDataGridView()
        Catch ex As Exception
            MessageBox.Show("Error connecting to the database: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Me.WindowState = FormWindowState.Maximized
    End Sub
    Private Sub ClearComboBoxes()
        ComboBox1.DataSource = Nothing
        ComboBox1.Items.Clear()
        ComboBox2.DataSource = Nothing
        ComboBox2.Items.Clear()
    End Sub
    Private Sub LoadComboBoxValues()
        ' Load Vehicle ID values into ComboBox1
        Dim queryVehicles As String = "SELECT vid FROM customer"
        Dim adapterVehicles As New MySqlDataAdapter(queryVehicles, connection)
        Dim dataVehicles As New DataTable()
        adapterVehicles.Fill(dataVehicles)
        ComboBox1.DataSource = dataVehicles
        ComboBox1.DisplayMember = "vid"
        ComboBox1.ValueMember = "vid"

        ' Load Service ID values into ComboBox2
        Dim queryServices As String = "SELECT service_id FROM service_type"
        Dim adapterServices As New MySqlDataAdapter(queryServices, connection)
        Dim dataServices As New DataTable()
        adapterServices.Fill(dataServices)
        ComboBox2.DataSource = dataServices
        ComboBox2.DisplayMember = "service_id"
        ComboBox2.ValueMember = "service_id"
    End Sub

    Private Sub RefreshDataGridView()
        Try
            Dim query As String = "SELECT * FROM schedule"
            Dim adapter As New MySqlDataAdapter(query, connection)
            Dim data As New DataTable()
            adapter.Fill(data)
            DataGridView1.DataSource = data
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If rowIndex >= 0 Then
            Dim selectedRow As DataGridViewRow = DataGridView1.Rows(rowIndex)
            Dim scheduleIdToDelete As String = selectedRow.Cells("schedule_id").Value.ToString()

            If MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    Dim query As String = "DELETE FROM schedule WHERE schedule_id = @schedule_id"
                    Dim command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@schedule_id", scheduleIdToDelete)
                    command.ExecuteNonQuery()
                    MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    RefreshDataGridView()
                Catch ex As Exception
                    MessageBox.Show("Error deleting record: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        Else
            MessageBox.Show("Please select a row to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 Then
            Dim selectedRow As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            TextBox1.Text = selectedRow.Cells("schedule_id").Value.ToString()
            ComboBox1.SelectedValue = selectedRow.Cells("vid").Value.ToString()
            ComboBox2.SelectedValue = selectedRow.Cells("service_id").Value.ToString()
            DateTimePicker1.Value = DateTime.Parse(selectedRow.Cells("schedule_date").Value.ToString())
            rowIndex = e.RowIndex
        End If
        DataGridView1.BeginEdit(True)
    End Sub

    Private Sub Form5_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.SelectedValue Is Nothing OrElse ComboBox2.SelectedValue Is Nothing OrElse DateTimePicker1.Value <= DateTime.Now Then
            MessageBox.Show("Please fill in all details correctly.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Try
                Dim query As String = "INSERT INTO schedule (schedule_id, vid, service_id, schedule_date) VALUES (@schedule_id, @vid, @service_id, @schedule_date)"
                Dim command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@schedule_id", TextBox1.Text)
                command.Parameters.AddWithValue("@vid", ComboBox1.SelectedValue.ToString())
                command.Parameters.AddWithValue("@service_id", ComboBox2.SelectedValue.ToString())
                command.Parameters.AddWithValue("@schedule_date", DateTimePicker1.Value.Date)
                command.ExecuteNonQuery()
                MessageBox.Show("Details entered successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                RefreshDataGridView()
            Catch ex As Exception
                MessageBox.Show("Error inserting data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ComboBox1.SelectedIndex = -1
        ComboBox2.SelectedIndex = -1
        DateTimePicker1.Value = DateTime.Now
        rowIndex = -1
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        UpdateData()
    End Sub

    Private Sub UpdateData()
        If rowIndex >= 0 Then
            Dim updatedScheduleId As String = TextBox1.Text ' Assuming TextBox1 contains Schedule ID
            Dim updatedVid As String = ComboBox1.Text
            Dim updatedServiceId As String = ComboBox2.Text
            Dim updatedScheduleDate As Date = DateTimePicker1.Value.Date

            Try
                Dim query As String = "UPDATE schedule SET vid = @vid, service_id = @service_id, schedule_date = @schedule_date WHERE schedule_id = @schedule_id"
                Dim command As New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@vid", updatedVid)
                command.Parameters.AddWithValue("@service_id", updatedServiceId)
                command.Parameters.AddWithValue("@schedule_date", updatedScheduleDate)
                command.Parameters.AddWithValue("@schedule_id", updatedScheduleId)
                command.ExecuteNonQuery()
                MessageBox.Show("Data updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                RefreshDataGridView()
            Catch ex As Exception
                MessageBox.Show("Error updating data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        ElseIf ComboBox1.Text = "" OrElse ComboBox2.Text = "" OrElse textbox1.Text = "" OrElse DateTimePicker1.Value <= DateTime.Now Then
            MessageBox.Show("Please fill in all details correctly.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ElseIf Not String.IsNullOrEmpty(ComboBox1.Text) AndAlso Not (ComboBox1.Text.Any(Function(c) Char.IsLetter(c)) AndAlso ComboBox1.Text.Any(Function(c) Char.IsDigit(c))) Then
            MessageBox.Show("Please enter both alphabetic and numeric characters in Vehicle ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ElseIf Not String.IsNullOrEmpty(ComboBox2.Text) AndAlso Not (ComboBox2.Text.Any(Function(c) Char.IsLetter(c)) AndAlso ComboBox2.Text.Any(Function(c) Char.IsDigit(c))) Then
            MessageBox.Show("Please enter both alphabetic and numeric characters in Service ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            MessageBox.Show("Please select a row to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form4
    Dim serviceTypeAmounts As New Dictionary(Of String, Decimal)
    Private connectionString As String = "server=localhost;username=root;password=101010;database=automanager"
    Dim valuesAndDescriptions() As KeyValuePair(Of String, String) = {
        New KeyValuePair(Of String, String)("Routine Maintenance",
            "1. Oil change" & vbCrLf &
            "2. Tire rotation" & vbCrLf &
            "3. Brake inspection and service" & vbCrLf &
            "4. Fluid checks and top-ups (coolant, transmission fluid, brake fluid, etc.)" & vbCrLf &
            "5. Air filter replacement" & vbCrLf &
            "6. Cabin air filter replacement" & vbCrLf &
            "7. Spark plug replacement" & vbCrLf &
            "8. Battery inspection and replacement"),
        New KeyValuePair(Of String, String)("Diagnostic Services",
            "1. Engine diagnostics" & vbCrLf &
            "2. Electrical system diagnostics" & vbCrLf &
            "3. Computerized vehicle analysis"),
        New KeyValuePair(Of String, String)("Repair Services",
            "1. Engine repair" & vbCrLf &
            "2. Transmission repair" & vbCrLf &
            "3. Suspension repair" & vbCrLf &
            "4. Steering system repair" & vbCrLf &
            "5. Exhaust system repair" & vbCrLf &
            "6. HVAC system repair (Heating, Ventilation, and Air Conditioning)" & vbCrLf &
            "7. Bodywork and dent repair" & vbCrLf &
            "8. Paint touch-ups"),
        New KeyValuePair(Of String, String)("Tire Services",
            "1. Tire installation" & vbCrLf &
            "2. Tire balancing" & vbCrLf &
            "3. Tire alignment" & vbCrLf &
            "4. Tire repair (patching punctures, etc.)" & vbCrLf &
            "5. Tire replacement"),
        New KeyValuePair(Of String, String)("Detailing Services",
            "1. Interior cleaning (vacuuming, upholstery cleaning, etc.)" & vbCrLf &
            "2. Exterior cleaning (washing, waxing, polishing)" & vbCrLf &
            "3. Paint protection (ceramic coating, sealants)" & vbCrLf &
            "4. Headlight restoration" & vbCrLf &
            "5. Wheel cleaning and polishing"),
        New KeyValuePair(Of String, String)("Electronics Services",
            "1. Installation of aftermarket electronics (stereo systems, speakers, alarms, etc.)" & vbCrLf &
            "2. Troubleshooting and repair of electrical systems (lights, power windows, etc.)" & vbCrLf &
            "3. Installation of accessories (GPS systems, dash cams, etc.)")
     }
    Private Sub InsertData(ByVal service_Id As String, ByVal vid As String, ByVal service_Type As String, ByVal description As String, ByVal amount As Decimal)
        Using connection As New MySqlConnection(connectionString)
            Try
                connection.Open()

                ' Check if the service ID already exists
                Dim checkQuery As String = "SELECT COUNT(*) FROM service_type WHERE Service_id = @Service_Id"
                Using checkCommand As New MySqlCommand(checkQuery, connection)
                    checkCommand.Parameters.AddWithValue("@Service_Id", service_Id)
                    Dim count As Integer = Convert.ToInt32(checkCommand.ExecuteScalar())
                    If count > 0 Then
                        MessageBox.Show("Service ID already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return ' Exit the method if the service ID already exists
                    End If
                End Using

                ' If the service ID doesn't exist, proceed with inserting data into the database
                ' Insert data into the database
                Dim query As String = "INSERT INTO service_type (Service_id, vid, service_type, description, amount) VALUES (@Service_Id, @vid, @service_Type, @description, @amount)"
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Service_Id", service_Id)
                    command.Parameters.AddWithValue("@vid", vid)
                    command.Parameters.AddWithValue("@service_Type", service_Type)
                    command.Parameters.AddWithValue("@description", description)
                    command.Parameters.AddWithValue("@amount", amount)
                    command.ExecuteNonQuery()
                End Using

                MessageBox.Show("Data inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        serviceTypeAmounts.Add("Routine Maintenance", 6000.0)
        serviceTypeAmounts.Add("Diagnostic Services", 10000.0)
        serviceTypeAmounts.Add("Repair Services", 200000.0)
        serviceTypeAmounts.Add("Tire Services", 50000.0)
        serviceTypeAmounts.Add("Detailing Services", 70000.0)
        serviceTypeAmounts.Add("Electronics Services", 30000.0)

        For Each pair As KeyValuePair(Of String, String) In valuesAndDescriptions
            ComboBox1.Items.Add(pair.Key)
        Next

        ' Add columns to the DataGridView
        DataGridView1.Columns.Add("Service_id Column", "Service_id")
        DataGridView1.Columns.Add("vid Column", "vid")
        DataGridView1.Columns.Add(" service_type Column", "service_type")
        DataGridView1.Columns.Add("Description Column", "Description")
        DataGridView1.Columns.Add("Amount column", "amount")

        ' Load data into DataGridView
        LoadDataIntoDataGridView()

        Me.WindowState = FormWindowState.Maximized
        ' Load Vehicle IDs into ComboBox2
        LoadVehicleIDs()
    End Sub
    Private Sub LoadVehicleIDs()
        ' Query to retrieve Vehicle IDs from the database
        Dim query As String = "SELECT vid FROM customer"

        Using connection As New MySqlConnection(connectionString)
            Try
                connection.Open()
                Using command As New MySqlCommand(query, connection)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            ' Add each Vehicle ID to ComboBox2
                            ComboBox2.Items.Add(reader("vid").ToString())
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error loading Vehicle IDs: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Validate input fields
        If TextBox1.Text = "" OrElse ComboBox2.SelectedItem Is Nothing OrElse ComboBox1.SelectedItem Is Nothing OrElse RichTextBox1.Text = "" OrElse TextBox2.Text = "" Then
            MessageBox.Show("Fill every detail", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Validate amount input
        Dim amount As Decimal
        If Not Decimal.TryParse(TextBox2.Text, amount) Then
            MessageBox.Show("Enter a valid amount.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get data from TextBox, ComboBox, and RichTextBox
        Dim Service_Id As String = TextBox1.Text
        Dim vid As String = ComboBox2.SelectedItem.ToString()
        Dim service_Type As String = ComboBox1.SelectedItem.ToString()
        Dim description As String = RichTextBox1.Text

        ' Insert data
        InsertData(Service_Id, vid, service_Type, description, amount)

        ' Clear input controls
        ClearInputControls()
        LoadDataIntoDataGridView() ' Reload data
    End Sub

    Private Sub LoadDataIntoDataGridView()
        Using connection As New MySqlConnection(connectionString)
            Try
                connection.Open()
                Dim query As String = "SELECT Service_id, vid, service_type, description, amount FROM service_type"
                Using command As New MySqlCommand(query, connection)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        DataGridView1.Rows.Clear() ' Clear existing rows before loading new data
                        While reader.Read()
                            ' Add each row of data to the DataGridView
                            DataGridView1.Rows.Add(reader("Service_id"), reader("vid"), reader("service_type"), reader("description"), reader("amount"))
                        End While
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            End Try
        End Using
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedItem IsNot Nothing Then
            ' Display description corresponding to the selected value in ComboBox
            Dim selectedValue As String = ComboBox1.SelectedItem.ToString()
            Dim selectedDescription As String = ""

            ' Fetch description from valuesAndDescriptions array based on the selected value
            For Each pair As KeyValuePair(Of String, String) In valuesAndDescriptions
                If pair.Key.Equals(selectedValue, StringComparison.OrdinalIgnoreCase) Then
                    selectedDescription = pair.Value
                    Exit For
                End If
            Next

            ' Display the description in the RichTextBox
            RichTextBox1.Text = selectedDescription

            ' Fetch amount from dictionary based on the selected value
            Dim selectedAmount As Decimal
            If serviceTypeAmounts.TryGetValue(selectedValue, selectedAmount) Then
                TextBox2.Text = selectedAmount.ToString()
            Else
                ' If the selected value is not found in the dictionary, clear the TextBox
                TextBox2.Clear()
            End If
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Clear all input fields
        TextBox1.Text = ""
        ComboBox2.SelectedIndex = -1
        ComboBox1.SelectedIndex = -1
        RichTextBox1.Text = ""
        TextBox2.Text = ""
    End Sub

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged
        Dim maxWordLimit As Integer = 150
        Dim words As String() = RichTextBox1.Text.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
        If words.Length > maxWordLimit Then
            Dim trimmedText As String = String.Join(" ", words.Take(maxWordLimit))
            RichTextBox1.Text = trimmedText
            RichTextBox1.SelectionStart = RichTextBox1.TextLength
            MessageBox.Show($"Word limit exceeded. Maximum {maxWordLimit} words allowed.", "Word Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form2.Show()
        Me.Hide()
    End Sub
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        ' Check if the clicked cell is not a header cell
        If e.RowIndex >= 0 Then
            ' Get the clicked row
            Dim selectedRow As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            ' Fetch data from the clicked row
            Dim serviceId As String = selectedRow.Cells("Service_id Column").Value.ToString()
            Dim vid As String = selectedRow.Cells("vid Column").Value.ToString()
            Dim serviceType As String = selectedRow.Cells(" service_type Column").Value.ToString()
            Dim description As String = selectedRow.Cells("Description Column").Value.ToString()
            Dim amount As Decimal = Decimal.Parse(selectedRow.Cells("Amount column").Value.ToString())

            ' Populate TextBoxes and ComboBoxes with the fetched data
            TextBox1.Text = serviceId
            ComboBox2.SelectedItem = vid
            ComboBox1.SelectedItem = serviceType
            RichTextBox1.Text = description
            TextBox2.Text = amount.ToString()
        End If
    End Sub
    Private Sub EditData(ByVal service_Id As String, ByVal vid As String, ByVal service_Type As String, ByVal description As String, ByVal amount As Decimal)
        Using connection As New MySqlConnection(connectionString)
            Try
                connection.Open()

                ' Check if the new service ID already exists (excluding the current row being edited)
                Dim checkQuery As String = "SELECT COUNT(*) FROM service_type WHERE Service_id = @Service_Id AND Service_id <> @Current_Service_Id"
                Using checkCommand As New MySqlCommand(checkQuery, connection)
                    checkCommand.Parameters.AddWithValue("@Service_Id", service_Id)
                    checkCommand.Parameters.AddWithValue("@Current_Service_Id", service_Id) ' Pass the current service ID being edited
                    Dim count As Integer = Convert.ToInt32(checkCommand.ExecuteScalar())
                    If count > 0 Then
                        MessageBox.Show("Service ID already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return ' Exit the method if the service ID already exists
                    End If
                End Using

                ' If the new service ID doesn't exist, proceed with updating data in the database
                ' Update data in the database
                Dim query As String = "UPDATE service_type SET vid = @vid, service_type = @service_Type, description = @description, amount = @amount WHERE Service_id = @Service_Id"
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@vid", vid)
                    command.Parameters.AddWithValue("@service_Type", service_Type)
                    command.Parameters.AddWithValue("@description", description)
                    command.Parameters.AddWithValue("@amount", amount)
                    command.Parameters.AddWithValue("@Service_Id", service_Id)
                    command.ExecuteNonQuery()
                End Using

                MessageBox.Show("Data updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Error updating data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ' Validate input fields
        If TextBox1.Text = "" OrElse ComboBox2.SelectedItem Is Nothing OrElse ComboBox1.SelectedItem Is Nothing OrElse RichTextBox1.Text = "" OrElse TextBox2.Text = "" Then
            MessageBox.Show("Fill every detail", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Validate amount input
        Dim amount As Decimal
        If Not Decimal.TryParse(TextBox2.Text, amount) Then
            MessageBox.Show("Enter a valid amount.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get data from TextBox, ComboBox, and RichTextBox
        Dim Service_Id As String = TextBox1.Text
        Dim vid As String = ComboBox2.SelectedItem.ToString()
        Dim service_Type As String = ComboBox1.SelectedItem.ToString()
        Dim description As String = RichTextBox1.Text

        ' Update data
        EditData(Service_Id, vid, service_Type, description, amount)

        ' Clear input controls
        ClearInputControls()
        LoadDataIntoDataGridView() ' Reload data
    End Sub
    Private Sub ClearInputControls()
        ' Clear all input fields
        TextBox1.Text = ""
        ComboBox2.SelectedIndex = -1
        ComboBox1.SelectedIndex = -1
        RichTextBox1.Text = ""
        TextBox2.Text = ""
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ' Check if a row is selected in the DataGridView
        If DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

            ' Get the Service ID of the selected row
            Dim serviceId As String = selectedRow.Cells("Service_id Column").Value.ToString()

            ' Confirm deletion with the user
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                ' Delete the record from the database
                DeleteData(serviceId)

                ' Clear input controls and reload data
                ClearInputControls()
                LoadDataIntoDataGridView()
            End If
        Else
            MessageBox.Show("Please select a row to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub
    Private Sub DeleteData(ByVal serviceId As String)
        Using connection As New MySqlConnection(connectionString)
            Try
                connection.Open()

                ' Delete the record with the specified Service ID from the database
                Dim query As String = "DELETE FROM service_type WHERE Service_id = @Service_Id"
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@Service_Id", serviceId)
                    command.ExecuteNonQuery()
                End Using

                MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Error deleting record: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub
End Class

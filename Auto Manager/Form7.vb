Imports MySql.Data.MySqlClient
Imports System.Drawing.Printing

Public Class Form7
    Private connectionString As String = "server=localhost;username=root;password=101010;database=automanager"

    Private Sub Form7_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Populate ComboBox1 with Vehicle IDs
        LoadVehicleIDs()
        Me.WindowState = FormWindowState.Maximized
    End Sub

    Private Sub LoadVehicleIDs()
        ComboBox1.Items.Clear()
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim query As String = "SELECT vid FROM customer"
                Using command As New MySqlCommand(query, connection)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                        While reader.Read()
                            ComboBox1.Items.Add(reader("vid").ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading Vehicle IDs: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub FetchData(vehicleID As String, reportDate As Date)
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                Dim query As String = "SELECT c.first_name, c.last_name, c.contact_no, c.model, b.service_type, b.amount, sch.schedule_date, b.issue_date " &
                                      "FROM billing b " &
                                      "INNER JOIN customer c ON b.vid = c.vid " &
                                      "INNER JOIN schedule sch ON b.schedule_id = sch.schedule_id " &
                                      "WHERE b.vid = @vid AND DATE(b.issue_date) = @reportDate"
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@vid", vehicleID)
                    command.Parameters.AddWithValue("@reportDate", reportDate.Date)
                    Using adapter As New MySqlDataAdapter(command)
                        Dim dataTable As New DataTable()
                        adapter.Fill(dataTable)
                        DataGridView1.DataSource = dataTable
                        ' Set column headers
                        DataGridView1.Columns(0).HeaderText = "First Name"
                        DataGridView1.Columns(1).HeaderText = "Last Name"
                        DataGridView1.Columns(2).HeaderText = "Contact"
                        DataGridView1.Columns(3).HeaderText = "Model"
                        DataGridView1.Columns(4).HeaderText = "Service Type"
                        DataGridView1.Columns(5).HeaderText = "Amount"
                        DataGridView1.Columns(6).HeaderText = "Schedule Date"
                        DataGridView1.Columns(7).HeaderText = "Issue Date"
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error fetching data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub PrintDocument1_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument1.PrintPage
        ' Check if DataGridView1 is not null and has rows
        If DataGridView1 IsNot Nothing AndAlso DataGridView1.Rows.Count > 0 Then
            ' Define fonts and colors
            Dim headerFont As New Font("Arial", 12, FontStyle.Bold)
            Dim regularFont As New Font("Arial", 10)
            Dim headerBrush As New SolidBrush(Color.Black)
            Dim regularBrush As New SolidBrush(Color.Black)

            ' Define page settings
            Dim pageWidth As Single = e.PageSettings.PrintableArea.Width
            Dim pageHeight As Single = e.PageSettings.PrintableArea.Height
            Dim marginX As Single = 50 ' Left and right margin
            Dim marginY As Single = 50 ' Top and bottom margin

            Dim xPos As Single = marginX
            Dim yPos As Single = marginY
            Dim lineHeight As Single = regularFont.GetHeight() + 4 ' Add a small gap between lines

            ' Print header
            Dim headerText As String = "Receipt For Auto Manager"
            Dim headerWidth As Single = e.Graphics.MeasureString(headerText, headerFont).Width
            e.Graphics.DrawString(headerText, headerFont, Brushes.ForestGreen, xPos + (pageWidth - headerWidth) / 2, yPos)
            yPos += lineHeight * 2 ' Move down for spacing

            ' Print column headers with space and alignment
            Dim columnWidths As New List(Of Single)() ' Store column widths
            Dim padding As Single = 10 ' Padding between columns

            ' Calculate column widths based on content
            For Each column As DataGridViewColumn In DataGridView1.Columns
                Dim columnWidth As Single = e.Graphics.MeasureString(column.HeaderText, headerFont).Width + 2 * padding
                columnWidths.Add(columnWidth) ' Store column width including padding
            Next

            ' Adjust column widths if total width exceeds page width
            Dim totalWidth As Single = columnWidths.Sum() + (DataGridView1.Columns.Count - 1) * padding ' Total width including padding
            If totalWidth > pageWidth - 2 * marginX Then
                Dim scaleFactor As Single = (pageWidth - 2 * marginX) / totalWidth
                For i As Integer = 0 To columnWidths.Count - 1
                    columnWidths(i) *= scaleFactor ' Scale column widths based on the scale factor
                Next
            End If

            ' Print column headers with adjusted widths and spacing
            Dim currentXPos As Single = marginX
            For i As Integer = 0 To DataGridView1.Columns.Count - 1
                Dim columnWidth As Single = columnWidths(i)
                e.Graphics.DrawString(DataGridView1.Columns(i).HeaderText, headerFont, headerBrush, currentXPos, yPos) ' Display column header with alignment
                currentXPos += columnWidth + padding ' Move X position for the next column including padding
            Next
            yPos += lineHeight ' Move down for data rows

            ' Print data rows with alignment
            For Each row As DataGridViewRow In DataGridView1.Rows
                currentXPos = marginX ' Reset X position for each row
                For i As Integer = 0 To DataGridView1.Columns.Count - 1
                    Dim cellValue As String = If(row.Cells(i).Value IsNot Nothing, row.Cells(i).Value.ToString(), "")
                    Dim columnWidth As Single = columnWidths(i)

                    ' Format date values without time
                    If TypeOf row.Cells(i).Value Is Date Then
                        cellValue = DirectCast(row.Cells(i).Value, Date).ToString("yyyy-MM-dd") ' Adjust date format as needed
                    End If

                    ' Align cell value based on column width and position
                    Dim cellXPosition As Single = currentXPos + (columnWidth - e.Graphics.MeasureString(cellValue, regularFont).Width) / 2
                    e.Graphics.DrawString(cellValue, regularFont, regularBrush, cellXPosition, yPos) ' Display cell value with alignment
                    currentXPos += columnWidth + padding ' Move X position for the next cell including padding
                Next
                yPos += lineHeight ' Move down for the next row

                ' Check if the end of the page is reached
                If yPos + lineHeight > pageHeight - marginY Then
                    e.HasMorePages = True ' Set HasMorePages to True to print on the next page
                    Exit For ' Exit the loop to print on the next page
                End If
            Next
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ' Show print preview dialog
        PrintPreviewDialog1.Document = PrintDocument1
        PrintPreviewDialog1.ShowDialog()
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ComboBox1.SelectedText = ""
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.Text = "" Then
            MessageBox.Show("Please Select a Vehicle ID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Dim selectedDate As Date = DateTimePicker1.Value.Date
            MessageBox.Show("Receipt Generated Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ' Fetch data when the button is clicked
            FetchData(ComboBox1.SelectedItem.ToString(), selectedDate)
        End If
    End Sub
End Class
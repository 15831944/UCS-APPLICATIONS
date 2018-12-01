' (C) Copyright 2015 by  Steven H. Brubaker
' Purpose: Custom parser for CSV file generated with VBA in Microsoft Excel
' that creates a DataTable to iterate over.

Imports System.IO

Module Util_CSV

    Public Function CreateDataTableFromCSV(ByVal PathWithFileName As String) As System.Data.DataTable
        Try
            ' Read in entire CSV file
            Dim sr As New StreamReader(PathWithFileName)
            Dim fullFileStr As String = sr.ReadToEnd()
            sr.Close()
            sr.Dispose()

            ' Break file up into lines of strings
            Dim lines As String() = fullFileStr.Split(ControlChars.Lf)

            ' Create our new DataTable
            Dim recs As New System.Data.DataTable()

            ' Process the Header using the | character to identify the divisions in order to get our...
            Dim sArr As String() = lines(0).Split("|"c)

            ' ...Column Names and create the columns in our Data Table
            For Each s As String In sArr
                ' Get rid of the Special Character (Newline) at end of each s in sArr (Array)
                s = s.Replace(Convert.ToString(ControlChars.Cr), "")
                Dim colStr As System.Data.DataColumn = New System.Data.DataColumn(s)
                colStr.DataType = System.Type.GetType("System.String")
                recs.Columns.Add(colStr)

                ' Check if we are working with Block References or MTEXT objects
                If s.Contains("MTEXT") Then
                    WinForm_DPI.MyCommands.isBlockRef = False
                End If

                ' Determine how many Attributes to process later
                If s.Contains("ATTRIBUTE") Then
                    WinForm_DPI.MyCommands.attribCount = WinForm_DPI.MyCommands.attribCount + 1
                End If

                ' Determine how many Object Data Fields to process later
                If s.Contains("FIELD_NAME") Then
                    WinForm_DPI.MyCommands.objDataCount = WinForm_DPI.MyCommands.objDataCount + 1
                End If
            Next

            ' Create our Rows of Fields (aka DataRow)
            Dim row As DataRow

            ' Process each line into the appropriate Column in our DataTable
            For Each line As String In lines
                ' Skip the Header Row (first line in a 0 indexed array) of our String() array called 'lines'
                If Not line = lines(0) Then
                    ' Get rid of the Special Character (Newline) at end of each line
                    line = line.Replace(Convert.ToString(ControlChars.Cr), "")
                    row = recs.NewRow()
                    row.ItemArray = line.Split("|"c)
                    Dim str As String = row.Item("LATITUDE").ToString()
                    ' Prevent a blank row at the end of the DataTable because of the way the Excel VBA PRINT# command
                    ' works when creating a CSV using a "|" symbol
                    If str <> "" Then
                        recs.Rows.Add(row)
                    End If
                End If
            Next

            ' Return our new and now filled DataTable
            Return recs

        Catch
            MsgBox("Error in processing CSV file. Source: CreateDataTableFromCSV()")
            Return Nothing
        End Try

    End Function
End Module

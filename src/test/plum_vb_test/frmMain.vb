Imports plum_vb

Public Class frmMain

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim path = System.Reflection.Assembly.GetExecutingAssembly().Location()
        Dim file = System.IO.Path.GetDirectoryName(path) + "\\testdump.dmp"

        Try
            Throw New Exception("I killed my self!")
        Catch ex As Exception
            PlumVB.WriteDumpFile(file)
        End Try
    End Sub
End Class

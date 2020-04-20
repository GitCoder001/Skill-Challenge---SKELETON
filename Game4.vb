Module Game4
    Public Function StartGame4(Name As String) As (Single, Boolean)
        ' Returns game score and if the player chose to quit
        StartGame4.Item1 = 0
        StartGame4.Item2 = False
        If Name = "" Or Name = Nothing Then Name = "Anna Nimmity"


    End Function
End Module

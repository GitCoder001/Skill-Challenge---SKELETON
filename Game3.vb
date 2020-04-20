Module Game3
    Public Function StartGame3(Name As String) As (Single, Boolean)
        ' Returns game score and if the player chose to quit
        StartGame3.Item1 = 0
        StartGame3.Item2 = False
        If Name = "" Or Name = Nothing Then Name = "Anna Nimmity"


    End Function
End Module

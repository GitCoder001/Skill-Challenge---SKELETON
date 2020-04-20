Module Game2
    Public Function StartGame2(Name As String) As (Single, Boolean)
        ' Returns game score and if the player chose to quit
        StartGame2.Item1 = 0
        StartGame2.Item2 = False
        If Name = "" Or Name = Nothing Then Name = "Anna Nimmity"

    End Function
End Module

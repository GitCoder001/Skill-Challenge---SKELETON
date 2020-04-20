Imports System.Drawing
Imports System.Console

Module Game1
    ' Stop the ball
    Public Function StartGame1(Name As String) As (Single, Boolean)
        ' Returns tuple of game score and if the player chose to quit
        StartGame1.Item1 = 0
        StartGame1.Item2 = False
        If Name = "" Or Name = Nothing Then Name = "Anna Nimmity"

        DisplayGameGfx()

        Console.ReadLine()

    End Function

    Sub DisplayGameGfx()
        Clear()
        CGH.BmpToConsole(0, 0, CGH.TextToBMP("Game #1: Stop The Ball", Color.Yellow, 100, 100), &H25CB, False)

    End Sub

End Module

'#####################################################################
'#                Skill Testing Game: L Minett 2020                  #
'# ----------------------------------------------------------------- #
'# Ideas for improvment:                                             #
'#  1) add more challenges (possibly random)                         #
'#  2) add subscores to high-score table, so players can become      #
'#     competitive                                                   #
'#                                                                   #
'#                                                                   #
'#                                                                   #
'#                                                                   #
'#                                                                   #
'#                                                                   #
'#                                                                   #
'#####################################################################
Imports System.Drawing
Imports System.Console
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports System.Threading
' Each game is contained in a separate module
Structure StrGameLog ' used across all game modules
    Dim PlayerName As String
    Dim TotalScore As Single
    Dim Game1Score As Single
    Dim Game2Score As Single
    Dim Game3Score As Single
    Dim Game4Score As Single
    Dim Game5Score As Single
End Structure
Module Main
    Structure StrHighScore
        Dim Score As Single
        Dim Name As String
        Sub New(Sc As Single, Player As String)
            Score = Sc
            Name = If(Player.Length = 0, "Sum Ting Wong", Player) 'default name if nothing presented
        End Sub
    End Structure

#Region "Game Parameters"


#End Region
#Region "Globals"
    Dim FilePath As String = Directory.GetCurrentDirectory & "\" ' ToDoL persist into my.resources
    Dim HighScoreFile As String = FilePath & "score.dat"
    Dim HighScore As New List(Of StrHighScore)
#End Region

    Sub Main()

        ' Set up console environment
        CGH.Win32Native.SetConsoleWindowPosition(0, 0)
        CGH.Win32Native.MaximizeConsoleWindow()
        OutputEncoding = Encoding.UTF8
        CursorVisible = False

        Dim Quit As Boolean = False
        LoadHighScoreFile()

        'CreateDefaultHighScore() ' ToDo: clear this line once testing is complete

        Do
            Clear()

            ' Display intiial logo
            Dim c As Bitmap = CGH.TextToBMP("Skillz", Color.Blue, 120, 120,, 25) ' once rasterised, can see width so can centre on screen
            CGH.BmpToConsole(CInt((WindowWidth - (c.Width - 20)) / 2), 0, c, &H263B, False,,,,,, 5, 8)

            ForegroundColor = ConsoleColor.White

            DisplayHighScore(31)  ' Y position of where high score graphics can be placesQuit = GameIntroScreen()

            Dim CurrentGame As StrGameLog
            ' could use a WITH statement and assign each GameScore as a launching function within each game module, causing the game to run and return
            ' However, if the user decides to quit half way through any game, we need to handle that here
            ' Thus, each game will return a Boolean (true if game was successul, false is quit, or something) and score.
            CurrentGame.PlayerName = GetPlayerName()
            If CurrentGame.PlayerName <> Nothing Then ' User pressed 'esc' on name enter, so flow back to intro screen
                ' each game module returns a tuple of score and boolean if player has quit (or possibly booted from the game)
                ' Cascading IFs so next game only loads if previous game was seen through
                'load game 1
                Dim Game = Game1.StartGame1(CurrentGame.PlayerName)
                If Not Game.Item2 Then
                    CurrentGame.Game1Score = Game.Item1
                    CurrentGame.TotalScore += Game.Item1
                    ' Load game 2
                    Game = Game2.StartGame2(CurrentGame.PlayerName)
                    CurrentGame.Game2Score = Game.Item1
                    If Not Game.Item2 Then
                        CurrentGame.Game1Score = Game.Item1
                        CurrentGame.TotalScore += Game.Item1
                        ' Load game 3
                        Game = Game3.StartGame3(CurrentGame.PlayerName)
                        CurrentGame.Game3Score = Game.Item1
                        If Not Game.Item2 Then
                            CurrentGame.Game4Score = Game.Item1
                            CurrentGame.TotalScore += Game.Item1
                            ' Load game 4
                            Game = Game4.StartGame4(CurrentGame.PlayerName)
                            CurrentGame.Game4Score = Game.Item1
                            If Not Game.Item2 Then
                                CurrentGame.Game1Score = Game.Item1
                                CurrentGame.TotalScore += Game.Item1
                                ' Load game 5
                                Game = Game5.StartGame5(CurrentGame.PlayerName)
                                CurrentGame.Game5Score = Game.Item1
                                CurrentGame.TotalScore += Game.Item1
                            End If
                        End If
                    End If
                End If
            End If


        Loop
        ' the QuitGame method is used to 
    End Sub
    Public Function CheckQuit(QuitMsg As String) As Boolean
        Dim Result As MsgBoxResult = MsgBox(QuitMsg, MsgBoxStyle.YesNoCancel, "Leaving already?")
        Select Case Result
            Case MsgBoxResult.Yes
                Return True
            Case Else
                Return False
        End Select
    End Function
    Sub QuitGame()
        ' called when user wants to quit
        SaveHighScoreFile() ' save the high score back to game
        End ' close application
    End Sub
    Function GetPlayerName() As String
        ' allows the user to enter their name
        ' Two options here, generate a single bitmap for each char and position on the screen, or generate new complete bitmaps each time char is entered
        ' option two is best for code efficiency (not memory)

        ClearKeyboardBuffer() ' dispose of any leftover key presses

        Dim NameString As New StringBuilder ' stores the name
        NameString.Append(" ") ' cannot generate a blank bitmap - strip space out at end
        Dim NameGraphic As Bitmap = CGH.TextToBMP(NameString.ToString, Color.White, 120, 120, "Ariel", 10) ' generate initial bmp
        Dim Finished As Boolean = False
        Dim KeyPress As ConsoleKeyInfo

        Clear()

        CGH.BmpToConsole(0, 0, CGH.TextToBMP("Please enter your player name", Color.White, 110, 110, "Ariel", 8), , False)

        Do
            If KeyAvailable Then
                KeyPress = ReadKey(True) ' store key press - we may need it later
                Select Case KeyPress.Key
                    Case ConsoleKey.Spacebar And NameString.Length < 20
                        NameString.Append(" ")
                    Case ConsoleKey.Enter
                        Finished = True
                    Case ConsoleKey.Escape
                        GetPlayerName = Nothing
                        Finished = True
                    Case ConsoleKey.Backspace Or ConsoleKey.Delete
                        If NameString.Length > 1 Then
                            NameString.Remove(NameString.Length - 1, 1) ' keep initial space
                            CGH.BmpToConsole(0, 20, NameGraphic, &H25CF, True, ConsoleColor.Black) ' display as black to erase previous
                        End If
                End Select

                ' handle A-Z, 0-9 separately as key enums are a PITA
                ' add more statements to handle other types of chars
                If KeyPress.KeyChar.ToString.ToUpper >= "A" And KeyPress.KeyChar.ToString.ToUpper <= "Z" And NameString.Length < 21 Then
                    NameString.Append(KeyPress.KeyChar.ToString)
                ElseIf KeyPress.KeyChar.ToString >= "0" And KeyPress.KeyChar.ToString.ToUpper <= "9" And NameString.Length < 21 Then
                    NameString.Append(KeyPress.KeyChar.ToString)
                End If

                'refresh graphic
                NameGraphic = CGH.TextToBMP(NameString.ToString, Color.White, 120, 120, "Ariel", 10)
                CGH.BmpToConsole(0, 20, NameGraphic, &H25CF, True, ConsoleColor.Blue)
            End If
        Loop Until Finished
        Return NameString.ToString.Trim
    End Function
    Sub ClearKeyboardBuffer()
        ' This will cycle through any keys in the buffer and dispose of them
        ' used so that the player cannot accidentially skip through too many screens
        Do While KeyAvailable
            ReadKey()
        Loop
    End Sub
#Region "High Score"
    Sub CreateDefaultHighScore()
        ' will generate a high score table if one is not present or reset
        HighScore.Clear() ' clear if there are contents
        HighScore.Add(New StrHighScore(10, "Vic Torius"))
        HighScore.Add(New StrHighScore(12.5, "Hugh Jeego"))
        HighScore.Add(New StrHighScore(14, "Major Kickbutt"))
        HighScore.Add(New StrHighScore(17, "Mike Hockertz"))
        HighScore.Add(New StrHighScore(20, "Barb Dwyer"))
        HighScore.Add(New StrHighScore(22.5, "Al Pacca"))
        HighScore.Add(New StrHighScore(24, "Sue Perb"))
        HighScore.Add(New StrHighScore(30, "Tom Foolery"))
        HighScore.Add(New StrHighScore(35, "Betty Dident"))
        HighScore.Add(New StrHighScore(45, "Frank Furter"))

        ' ensure table is sorted
        HighScore = HighScore.OrderBy(Function(x) x.Score).ToList ' uses a lambda expression to define the property by which to order
    End Sub
    Sub AddHighScore(Score As Single, Player As String)
        Dim Position As Short = -1

        For Each Entry As StrHighScore In HighScore
            If Score <= Entry.Score Then ' if smaller, insert before current list entry
                HighScore.Insert(HighScore.IndexOf(Entry), New StrHighScore(Score, If(Player.Trim.Length > 20, Left(Player, 20), Player.Trim)))
                HighScore.Remove(HighScore.Last) ' take last item out of list as it's dropped out of table
                Position = HighScore.IndexOf(Entry)
                Exit For
            End If
        Next
        If Position <> -1 Then
            Clear()
            CGH.BmpToConsole(34, 0, CGH.TextToBMP("Congratulations!", Color.Blue, 120, 120, "Ariel", 12), &H2666, False)
            CGH.BmpToConsole(37, 20, CGH.TextToBMP("New High Score", Color.Blue, 120, 120, "Ariel", 12), &H2666, False)
            CGH.BmpToConsole(85, 40, CGH.TextToBMP("#" & Position, Color.Yellow, 150, 150, "Ariel", 14), &H2666, False)
            Thread.Sleep(3000)
        End If
    End Sub
    Sub DisplayHighScore(Optional Pos As Short = 0)
        ' Displays the high score page, the graphics and passes control back once a key has been pressed
        ' There is a limit to what can be achieved with the console buffer memory and with time, doublebuffering could be achieved

        Dim NamePad As Char = "." ' symbol used to pad out high score names
        Dim TopNameGraphic As Bitmap = CGH.TextToBMP(HighScore(0).Name, Color.White, 100, 100, "Consolas", 10, Color.Black) ' rasterize top score name
        Dim ScoreTablePos_X As Short = CInt(WindowWidth / 2) - 16 ' position for score table
        Dim ScoreTablePos_Y As Short = WindowHeight - 12 ' position for score table

        Dim Jiggle_Y1, Jiggle_X1, Jiggle_X2, Jiggle_Y2 As Short ' bounds in whicih top player can jiggle
        Jiggle_X1 = 15
        Jiggle_X2 = WindowWidth - (TopNameGraphic.Width + 1)
        Jiggle_Y1 = Pos + 2
        Jiggle_Y2 = ScoreTablePos_Y - TopNameGraphic.Height - 1

        Dim PressStartClrs As New List(Of Short)({0, 8, 7, 15, 7, 8}) ' this relates to the console enum values for the 'press key to start (black, dark grey, light grey, white and back to black
        Dim ClrIndex As Short = 0 ' which colour idnex in PressStartClrs is in use
        Dim HighScoreClrs As New List(Of Short)({1, 9, 11, 10, 14, 12, 5, 13}) ' this relates to the console enum values for the 'press key to start (black, dark grey, light grey, white and back to black
        Dim HighScoreClrIndex As Short = 0 ' which colour idnex in PressStartClrs is in use
        Dim HighScoreTopClr As Short = 0 ' this enables the smooth scrolling

        Dim Ran_X, Ran_Y, RanClr As Short
        Dim Rand As New Random
        Randomize()

        Dim graphic As Bitmap = CGH.TextToBMP("High Scores", Color.Yellow, 100, 100, "Ariel", 9, Color.Black)
        graphic.RotateFlip(RotateFlipType.Rotate270FlipNone)
        CGH.BmpToConsole(0, 0, graphic, &H25CF, False, ConsoleColor.Cyan,, ConsoleColor.Red,,,, 17,, 2)

        ForegroundColor = ConsoleColor.White
        SetCursorPosition(CInt(WindowWidth / 2) - 9, Pos)
        Write("TODAY'S TOP SCORER")

        ForegroundColor = ConsoleColor.White
        DrawBorder(ScoreTablePos_X - 2, ScoreTablePos_Y - 1, (CInt(WindowWidth / 2) - ScoreTablePos_X) + CInt(WindowWidth / 2) + 2, ScoreTablePos_Y + 10, BorderType.SingleLineCurved, ConsoleColor.Red)
        For Counter = 0 To HighScore.Count - 1
            SetCursorPosition(ScoreTablePos_X, ScoreTablePos_Y + Counter)
            WriteLine($"{(Counter + 1).ToString,2}  {HighScore(Counter).Name.PadRight(20, NamePad),-20}  {HighScore(Counter).Score,6:N3}") ' -ve value is left align, +ve is right align
        Next

        ' set up threading for score and 'key to start' animation 
        ' parameter passing is tricky with threads, so create anonymous sub and pass in reference to parameters
        'Dim anim As New Threading.Thread(Sub() AnimateHighScore(ScoreTablePos_X, ScoreTablePos_Y, NamePad)) ' NOT USED UNTIL RESOLVE CONSOLE BUFFER LOCKING

        ' loop flashing top player
        Dim NameStopwatch As New Stopwatch ' times name flash
        NameStopwatch.Start()
        Dim KeyStopwatch As New Stopwatch ' times 'press space to start'
        KeyStopwatch.Start()
        Dim ScoreStopwatch As New Stopwatch ' times highscore colour cascade
        ScoreStopwatch.Start()

        Ran_X = Rand.Next(Jiggle_X1, Jiggle_X2)
        Ran_Y = Rand.Next(Jiggle_Y1, Jiggle_Y2)
        RanClr = Rand.Next(1, 16)
        CGH.BmpToConsole(Ran_X, Ran_Y, TopNameGraphic, &H263A, True, RanClr)

        Do ' text animation loop
            ' key press
            If KeyAvailable Then
                Select Case ReadKey(True).Key
                    Case ConsoleKey.Spacebar
                        Exit Do
                    Case ConsoleKey.Escape
                        If CheckQuit("Exit the game?") = True Then QuitGame()
                End Select
            End If

            If NameStopwatch.ElapsedMilliseconds >= 2000 Then
                CGH.BmpToConsole(Ran_X, Ran_Y, TopNameGraphic, &H263A, True, ConsoleColor.Black) ' wipe over
                Ran_X = Rand.Next(Jiggle_X1, Jiggle_X2)
                Ran_Y = Rand.Next(Jiggle_Y1, Jiggle_Y2)
                RanClr = Rand.Next(1, 16)
                CGH.BmpToConsole(Ran_X, Ran_Y, TopNameGraphic, &H263A, True, RanClr)
                NameStopwatch.Restart()
            End If
            If KeyStopwatch.ElapsedMilliseconds >= 300 Then
                Console.SetCursorPosition(WindowWidth - 40, 2)
                Console.ForegroundColor = PressStartClrs.Item(ClrIndex)
                Console.WriteLine("Press 'space' to start")
                ClrIndex = If(ClrIndex = PressStartClrs.Count - 1, 0, ClrIndex + 1)
                KeyStopwatch.Restart()
            End If
            If ScoreStopwatch.ElapsedMilliseconds >= 500 Then
                HighScoreTopClr = HighScoreClrIndex
                HighScoreClrIndex = If(HighScoreClrIndex = HighScoreClrs.Count - 1, 0, HighScoreClrIndex + 1)
                For Counter = 0 To HighScore.Count - 1
                    Console.ForegroundColor = HighScoreClrs.Item(HighScoreTopClr)
                    HighScoreTopClr = If(HighScoreTopClr = HighScoreClrs.Count - 1, 0, HighScoreTopClr + 1)
                    SetCursorPosition(ScoreTablePos_X, ScoreTablePos_Y + Counter)
                    WriteLine($"{(Counter + 1).ToString,2}  {HighScore(Counter).Name.PadRight(20, NamePad),-20}  {HighScore(Counter).Score,6:N3}") ' -ve value is left align, +ve is right align
                Next
                ScoreStopwatch.Restart()
            End If
        Loop

    End Sub

    Sub LoadHighScoreFile()

        If File.Exists(FilePath & "Score.dat") Then
            Try
                HighScore.Clear()
                Using Reader As New IO.StreamReader(HighScoreFile)
                    Do Until Reader.EndOfStream
                        HighScore.Add(New StrHighScore(CSng(Reader.ReadLine), Reader.ReadLine))
                    Loop
                End Using

            Catch ex As Exception
                ' highscore not available, or error reading data so generate default
                MsgBox($"Error {ex.ToString}{vbCrLf}{vbCrLf}Loading default score table!", MsgBoxStyle.OkOnly, "Error readig score table")
                CreateDefaultHighScore()
            End Try

        Else
            ' doesn't exist so initialise new score table and save it
            CreateDefaultHighScore()
            SaveHighScoreFile()
        End If
    End Sub
    Sub SaveHighScoreFile()
        Try
            ' no need to check if folder exists, let it fail
            Using Writer As New StreamWriter(HighScoreFile, False)
                For Each entry As StrHighScore In HighScore
                    Writer.WriteLine(entry.Score)
                    Writer.WriteLine(entry.Name)
                Next
            End Using

        Catch ex As Exception
            ' Cannot save high score table, could be a permissions issue, or app loaded from readonly location
            MsgBox($"Error {ex.ToString}{vbCrLf}{vbCrLf}Could ot save score table!", MsgBoxStyle.OkOnly, "Error Saving")
            CreateDefaultHighScore()
        End Try
    End Sub
#End Region

End Module


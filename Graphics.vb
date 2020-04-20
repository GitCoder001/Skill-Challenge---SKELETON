Imports System.Drawing
Imports System.Runtime.InteropServices ' used for console maximisation
Imports System.Security.Principal
Imports Microsoft.VisualBasic.ApplicationServices

Module CGH
    Public Enum BorderType
        SingleLine
        SingleLineCurved
        SingleThick
        SingleDashed
        SingleThickDashed
        DoubleLine
        DoubleSideSingleTop
        SingleSideDoubleTop
        Solid
        Circle
    End Enum
    ' This is not the most recent version, look at the Text to Bitmap To Console project for the up-to-date code and features
    Public Class Win32Native
        ' class handles the Win32 pointers to control the console window as no native support in .NET for this
        Private Const SWP_NOZORDER As Integer = &H4
        Private Const SWP_NOACTIVATE As Integer = &H10
        Private Const SW_MAXIMIZE As Integer = &H3

        <StructLayout(LayoutKind.Sequential)>
        Private Structure RECT
            Public Left As Integer
            Public Top As Integer
            Public Right As Integer
            Public Bottom As Integer
        End Structure

        <DllImport("kernel32")>
        Private Shared Function GetConsoleWindow() As IntPtr
        End Function

        <DllImport("user32")>
        Private Shared Function SetWindowPos(hWnd As IntPtr, hWndInsertAfter As IntPtr,
        x As Integer, y As Integer, cx As Integer, cy As Integer, flags As Integer) As Boolean
        End Function

        <DllImport("user32.dll")>
        Private Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
        End Function

        <DllImport("user32.dll")>
        Public Shared Function ShowWindow(hWnd As IntPtr, cmdShow As Integer) As Boolean
        End Function

        Public Shared Sub SetConsoleWindowPosition(x As Integer, y As Integer)
            Dim r As RECT
            GetWindowRect(GetConsoleWindow(), r)

            SetWindowPos(GetConsoleWindow(), IntPtr.Zero,
                     x, y,
                     r.Left + r.Right, r.Top + r.Bottom,
                     SWP_NOZORDER Or SWP_NOACTIVATE)
        End Sub

        Public Shared Sub MaximizeConsoleWindow()
            Dim p As Process = Process.GetCurrentProcess()
            ShowWindow(p.MainWindowHandle, SW_MAXIMIZE)
        End Sub
    End Class
    Public Class Position
        Private _PosX As Short
        Private _PosY As Short
        Public Sub New(X As Short, Y As Short)
            _PosX = X
            _PosY = Y
        End Sub
        Public Property X() As Short
            Get
                Return _PosX
            End Get
            Set(value As Short)
                _PosX = value
            End Set
        End Property
        Public Property Y() As Short
            Get
                Return _PosY
            End Get
            Set(value As Short)
                _PosY = value
            End Set
        End Property
    End Class

#Region "Border"
    ''' <summary>
    ''' Draws a border between x1,y1 and x2,y2. Requires console to have output encoding set to UTF-8.
    ''' </summary>
    ''' <param name="X1">Starting X position</param>
    ''' <param name="Y1">Starting Y position</param>
    ''' <param name="X2">Finishing X position</param>
    ''' <param name="Y2">Finishing Y position</param>
    ''' <param name="BorderStyle">Style of border</param>
    ''' <param name="LineColour">Console colour of the border</param>
    ''' <param name="BackColour">Background colour of the border</param>
    ''' 
    Public Sub DrawBorder(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer, Optional BorderStyle As BorderType = BorderType.DoubleLine,
             Optional LineColour As ConsoleColor = ConsoleColor.White, Optional BackColour As ConsoleColor = ConsoleColor.Black)
        Dim TopLeft, TopRight, BottomLeft, BottomRight As Short ' corner symbols

        Select Case BorderStyle ' set corner shapes
            Case BorderType.SingleLine
                TopLeft = &H250C
                TopRight = &H2510
                BottomLeft = &H2514
                BottomRight = &H2518
            Case BorderType.SingleLineCurved
                TopLeft = &H256D
                TopRight = &H256E
                BottomLeft = &H2570
                BottomRight = &H256F
            Case BorderType.DoubleLine
                TopLeft = &H2554
                TopRight = &H2557
                BottomLeft = &H255A
                BottomRight = &H255D
            Case BorderType.DoubleSideSingleTop
                TopLeft = &H2553
                TopRight = &H2556
                BottomLeft = &H2559
                BottomRight = &H255C
            Case BorderType.SingleDashed
                TopLeft = &H250C
                TopRight = &H2510
                BottomLeft = &H2514
                BottomRight = &H2518
            Case BorderType.SingleSideDoubleTop
                TopLeft = &H2552
                TopRight = &H2555
                BottomLeft = &H2558
                BottomRight = &H255B
            Case BorderType.SingleThick
                TopLeft = &H250F
                TopRight = &H2513
                BottomLeft = &H2517
                BottomRight = &H251B
            Case BorderType.SingleThickDashed
                TopLeft = &H250F
                TopRight = &H2513
                BottomLeft = &H2517
                BottomRight = &H251B
            Case BorderType.Circle
                TopLeft = &H25CF
                TopRight = &H25CF
                BottomLeft = &H25CF
                BottomRight = &H25CF
            Case BorderType.Solid
                TopLeft = &H2588
                TopRight = &H2588
                BottomLeft = &H2588
                BottomRight = &H2588
        End Select

        ' paint specific lines - line shapes are determined by the DrawLine
        'top 
        DrawLine(X1, Y1, X2, Y1, BorderStyle, LineColour, BackColour)
        ' sides
        DrawLine(X1, Y1, X1, Y2, BorderStyle, LineColour, BackColour)
        DrawLine(X2, Y1, X2, Y2, BorderStyle, LineColour, BackColour)
        ' bottom
        DrawLine(X1, Y2, X2, Y2, BorderStyle, LineColour, BackColour)

        ' now paint corners C1-4
        Console.ForegroundColor = LineColour
        Console.BackgroundColor = BackColour

        Console.SetCursorPosition(X1, Y1)
        Console.Write(ChrW(TopLeft))
        Console.SetCursorPosition(X2, Y1)
        Console.Write(ChrW(TopRight))
        Console.SetCursorPosition(X1, Y2)
        Console.Write(ChrW(BottomLeft))
        Console.SetCursorPosition(X2, Y2)
        Console.Write(ChrW(BottomRight))

        'default colours back 
        Console.ForegroundColor = ConsoleColor.White
        Console.BackgroundColor = ConsoleColor.Black

    End Sub
#End Region

#Region "Custom Border"
    ''' <summary>
    ''' Draws a border between x1,y1 and x2,y2 using specified unicode character
    ''' </summary>
    ''' <param name="X1">Starting X position</param>
    ''' <param name="Y1">Starting Y position</param>
    ''' <param name="X2">Finishing X position</param>
    ''' <param name="Y2">Finishing Y position</param>
    ''' <param name="BorderSymbol">Specifies a char to be drawn</param>
    Public Sub DrawCustomBorder(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer, BorderSymbol As Char,
             Optional LineColour As ConsoleColor = ConsoleColor.White, Optional BackColour As ConsoleColor = ConsoleColor.Black)
        ' will call the custom line draw to paint the symbol on the screen

        'top 
        DrawCustomLine(X1, Y1, X2, Y1, BorderSymbol, LineColour, BackColour)
        ' sides
        DrawCustomLine(X1, Y1, X1, Y2, BorderSymbol, LineColour, BackColour)
        DrawCustomLine(X2, Y1, X2, Y2, BorderSymbol, LineColour, BackColour)

        ' bottom
        DrawCustomLine(X1, Y2, X2, Y2, BorderSymbol, LineColour, BackColour)
    End Sub
    ''' <summary>
    ''' Draws a border between x1,y1 and x2,y2 using specified unicode character
    ''' </summary>
    ''' <param name="X1">Starting X position</param>
    ''' <param name="Y1">Starting Y position</param>
    ''' <param name="X2">Finishing X position</param>
    ''' <param name="Y2">Finishing Y position</param>
    ''' <param name="UnicodeChar">Specifies a char to be drawn</param>
    Public Sub DrawCustomBorder(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer, UnicodeChar As Integer,
             Optional LineColour As ConsoleColor = ConsoleColor.White, Optional BackColour As ConsoleColor = ConsoleColor.Black)
        DrawCustomBorder(X1, Y1, X2, Y2, ChrW(UnicodeChar), LineColour, BackColour)
    End Sub
#End Region

#Region "Line"
    ''' <summary>
    ''' Draws a vertical or horizontal line. Default style is SingleLine
    ''' </summary>
    ''' <param name="X1">Starting coordinate</param>
    ''' <param name="Y1">Starting coordinate</param>
    ''' <param name="X2">Finishing coordinate</param>
    ''' <param name="Y2">Finishing coordinate</param>
    Public Sub DrawLine(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer)
        DrawLine(X1, Y1, X2, Y2, BorderType.SingleLine, ConsoleColor.White, ConsoleColor.Black)
    End Sub
    ''' <summary>
    ''' Draws a vertical or horizontal line. Default style is SingleLine
    ''' </summary>
    ''' <param name="X1">Starting coordinate</param>
    ''' <param name="Y1">Starting coordinate</param>
    ''' <param name="X2">Finishing coordinate</param>
    ''' <param name="Y2">Finishing coordinate</param>
    ''' <param name="LineStyle">Specifies which style to draw the line</param>
    ''' <param name="LineColour">The console colour of the line</param>
    ''' <param name="BackColour">The console colour of the background</param>
    Public Sub DrawLine(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer, LineStyle As BorderType,
             Optional LineColour As ConsoleColor = ConsoleColor.White, Optional BackColour As ConsoleColor = ConsoleColor.Black)
        ' called custom line to draw shape
        Dim Horizontal As Boolean = If(Y1 = Y2, True, False) ' is line horizontal or vertical
        Dim Symb As Integer
        Select Case LineStyle
            Case BorderType.SingleLine
                ' call custom line with correct symbol
                Symb = If(Horizontal, &H2500, &H2502)
            Case BorderType.SingleLineCurved
                Symb = If(Horizontal, &H2500, &H2502)
            Case BorderType.Circle
                Symb = &H25CF
            Case BorderType.DoubleLine
                Symb = If(Horizontal, &H2550, &H2551)
            Case BorderType.DoubleSideSingleTop
                Symb = If(Horizontal, &H2500, &H2551)
            Case BorderType.SingleDashed
                Symb = If(Horizontal, &H2504, &H2506)
            Case BorderType.SingleSideDoubleTop
                Symb = If(Horizontal, &H2550, &H2502)
            Case BorderType.SingleThick
                Symb = If(Horizontal, &H2501, &H2503)
            Case BorderType.Solid
                Symb = &H2588
            Case BorderType.SingleThickDashed
                Symb = If(Horizontal, &H2505, &H2507)
        End Select
        DrawCustomLine(X1, Y1, X2, Y2, Symb, LineColour, BackColour)
    End Sub

#End Region
#Region "Custom Line"
    ''' <summary>
    ''' Draws a horizontal or vertical line on the screen
    ''' </summary>
    ''' <param name="X1"></param>
    ''' <param name="Y1"></param>
    ''' <param name="X2"></param>
    ''' <param name="Y2"></param>
    ''' <param name="UnicodeChar">The unicode value of the symbol used to paint the line</param>
    ''' <param name="LineColour">The console colour of the line</param>
    ''' <param name="BackColour">The console colour of the background</param>
    Sub DrawCustomLine(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer, UnicodeChar As Integer,
                   Optional LineColour As ConsoleColor = ConsoleColor.White, Optional BackColour As ConsoleColor = ConsoleColor.Black)
        DrawCustomLine(X1, Y1, X2, Y2, ChrW(UnicodeChar), LineColour, BackColour) ' call other overload and pass in integer converted to char
    End Sub
    ''' <summary>
    ''' Draws a horizontal or vertical line on the screen
    ''' </summary>
    ''' <param name="X1"></param>
    ''' <param name="Y1"></param>
    ''' <param name="X2"></param>
    ''' <param name="Y2"></param>
    ''' <param name="LineSymbol">The char with which to draw the line</param>
    ''' <param name="LineColour">The console colour of the line</param>
    ''' <param name="BackColour">The console colour of the background</param>
    Sub DrawCustomLine(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer, LineSymbol As Char,
                   Optional LineColour As ConsoleColor = ConsoleColor.White, Optional BackColour As ConsoleColor = ConsoleColor.Black)
        ' draw a line from X1,Y1 to X2,Y2 using the supplied symbol
        ' ToDo: With more time, implement Bresenham's line algorithm, or even Wu's
        ' If X and Y are both different (i.e. diagonal, then horizontal assumed until diagonal line functionality supported

        Dim Horizontal As Boolean = If(Y1 = Y2, True, False) ' is line horizontal or vertical
        Console.ForegroundColor = LineColour
        Console.BackgroundColor = BackColour

        If Horizontal Then
            For cols = X1 To X2
                Console.SetCursorPosition(cols, Y1)
                Console.Write(LineSymbol)
            Next
        Else
            For rows = Y1 To Y2
                Console.SetCursorPosition(X1, rows)
                Console.Write(LineSymbol)
            Next
        End If
        'default colours back 
        Console.ForegroundColor = ConsoleColor.White
        Console.BackgroundColor = ConsoleColor.Black
    End Sub

#End Region
    ''' <summary>
    ''' Draws a filled rectangle on the console screen
    ''' </summary>
    ''' <param name="X1"></param>
    ''' <param name="Y1"></param>
    ''' <param name="X2"></param>
    ''' <param name="Y2"></param>
    ''' <param name="FillColour"></param>
    Public Sub DrawRectangle(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer, FillColour As ConsoleColor)
        DrawRectangle(X1, Y1, X2, Y2, FillColour, &H2588) ' default as a block
    End Sub
    ''' <summary>
    ''' Draws a filled rectangle on the console screen
    ''' </summary>
    ''' <param name="X1"></param>
    ''' <param name="Y1"></param>
    ''' <param name="X2"></param>
    ''' <param name="Y2"></param>
    ''' <param name="FillColour"></param>
    ''' <param name="FillChar">The character with which to fill the rectangle</param>
    Public Sub DrawRectangle(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer, FillColour As ConsoleColor, FillChar As Char)
        ' uses the drawcustomline to fill solid block
        If Y1 > Y2 Then Throw New Exception("Y2 cannot be less than Y1")
        If X1 > X2 Then Throw New Exception("X2 cannot be less than X1")

        For row As Integer = Y1 To Y2
            DrawCustomLine(X1, row, X2, row, FillChar, FillColour)
        Next
    End Sub
    ''' <summary>
    '''  Draws a filled rectangle on the console screen
    ''' </summary>
    ''' <param name="X1"></param>
    ''' <param name="Y1"></param>
    ''' <param name="X2"></param>
    ''' <param name="Y2"></param>
    ''' <param name="FillColour"></param>
    ''' <param name="UnicodeChar">The unicode value with which to fill the rectangle</param>
    Public Sub DrawRectangle(X1 As Integer, Y1 As Integer, X2 As Integer, Y2 As Integer, FillColour As ConsoleColor, UnicodeChar As Short)
        DrawRectangle(X1, Y1, X2, Y2, FillColour, ChrW(UnicodeChar))
    End Sub


    ''' <summary>
    ''' Will take a string of text and render this to a bitmap image
    ''' </summary>
    ''' <param name="text">The text to rasterise</param>
    ''' <param name="TextColour">What colour should the text be</param>
    ''' <param name="DPIx">The image DPI for x axis</param>
    ''' <param name="DPIy">The image DPI for y axis</param>
    ''' <param name="fnt">The name of the font family to use (default Consolas)</param>
    ''' <param name="Sze">The font size in points (default 12)</param>
    ''' <param name="BackgroundClr">The background colour to pain the canvas</param>
    ''' <returns></returns>
    Function TextToBMP(text As String, TextColour As Color, Optional DPIx As Single = 120, Optional DPIy As Single = 120, Optional fnt As String = "Consolas",
                       Optional Sze As Single = 12, Optional BackgroundClr As Drawing.Color = Nothing) As Bitmap

        If BackgroundClr = Nothing Then ' if no background colour supplied, assume black
            BackgroundClr = Color.Black
        End If

        ' Makes an attempt to calculate image width and height based on text length (assumes 6px x 12px @72dpi with standard kerning)
        Dim imgWidth As Short = text.Length * ((DPIx * (Sze * 0.64)) / 72)
        Dim imgHeight As Short = ((DPIy * Sze) / 72) + 2


        Dim Bmp As New Bitmap(imgWidth, imgHeight, Imaging.PixelFormat.Format32bppPArgb) ' define new graphics object

        'Set the resolution to 150 DPI
        Bmp.SetResolution(DPIx, DPIy)
        'Create a graphics object from the bitmap
        Using G = Graphics.FromImage(Bmp)
            'Paint the canvas white
            G.Clear(BackgroundClr)
            'Set various modes to higher quality
            G.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            G.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            G.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            'Create a font
            Using F As New Font(fnt, Sze)
                'Create a brush
                Using B As New SolidBrush(TextColour)
                    'Draw some text
                    G.DrawString(text, F, B, 0, 0)
                    'Bmp.Save("d:\please delete.bmp", Imaging.ImageFormat.Bmp)
                End Using
            End Using
        End Using
        Return Bmp
    End Function
    ''' <summary>
    ''' Will display a console text representation of a bitmap graphic
    ''' </summary>
    ''' <param name="PosX">The starting console x position of the render</param>
    ''' <param name="PosY">The starting console y position of the render</param>
    ''' <param name="image">The bitmap to render</param>
    ''' <param name="ShapeIndex">The unicode value of the shape to be used. Block is default</param>
    ''' <param name="SolidColour">Any non-background coloured pixel will be painted a single colour specified by [DefaultClr]</param>
    ''' <param name="DefaultClr">If using solid colour, this is the console colour to use for any non-transparent pixel</param>
    ''' <param name="OrigBkgClr">Specifies that this colour is treated as a background colour. Default is black.</param>
    ''' <param name="NewBkgClr">Specifies what console colour the background should be painted, if nothing specified, uses black</param>
    ''' <param name="ScaleX">If set, will skip the indicated number of colss to reduce the x axis. e.g. 1=1:1 scale</param>
    ''' <param name="ScaleY">If set, will skip the indicated number of rows to reduce the y axis. e.g. 1=1:1 scale</param>
    ''' <param name="IgnoreX1">X coordinate of first x pixel to draw (acts like crop)</param>
    ''' <param name="IgnoreY1">Y coordinate of first y pixel to draw (acts like crop)</param>
    ''' <param name="IgnoreX2">Specifies the last X coordinate of bitmap that will be drawn</param>
    ''' <param name="IgnoreY2">Specifies the last Y coordinate of bitmap that will be drawn</param>
    Sub BmpToConsole(PosX As Short, PosY As Short, image As Bitmap, Optional ShapeIndex As Short = &H2588, Optional SolidColour As Boolean = True,
                    Optional DefaultClr As ConsoleColor = ConsoleColor.White, Optional OrigBkgClr As Color = Nothing, Optional NewBkgClr As ConsoleColor = ConsoleColor.Black,
                    Optional ScaleX As Short = 1, Optional ScaleY As Short = 1,
                    Optional IgnoreX1 As Short = 2, Optional IgnoreY1 As Short = 2, Optional IgnoreX2 As Short = 0, Optional IgnoreY2 As Short = 0)
        ' will convert a bitmap image to console pixels

        If OrigBkgClr = Nothing Then ' default parameter values must be constants, which color is not 
            OrigBkgClr = Color.Black
        End If
        ScaleY = If(ScaleY < 1, 1, ScaleY) ' check not less than 1


        ' If SolidColour is true, then no determination will be made of the actual colour and will use the the Clr parameter
        ' IgnoreX/IgnoreY will ignore first x pixels in image (padding). Saves having to crop bitmap (different size fonts have different padding)

        ' As the intention here is to display a bitmap, the code ignores the background colour
        Dim Row As Short = PosY
        Dim Col As Short = PosX

        For y As Integer = IgnoreY1 To (image.Height - 1) - IgnoreY2 Step ScaleY
            For x As Integer = IgnoreX1 To (image.Width - 1) - IgnoreX2 Step ScaleX
                'If y >= Console.WindowHeight Then Throw New Exception("Outside Of Console window") ' allow scroll if exceed window height, as easy to do with graphics generation
                If x >= Console.WindowWidth Then Throw New Exception("Outside Of Console window")

                Console.SetCursorPosition(Col, Row)
                If SolidColour Then
                    ' paint console pixel as solid colour
                    If image.GetPixel(x, y).ToArgb = OrigBkgClr.ToArgb Then
                        Console.ForegroundColor = NewBkgClr ' convert new background colour to console colour
                    Else
                        Console.ForegroundColor = DefaultClr ' paint using the requested colour
                    End If
                Else
                    Console.ForegroundColor = GetConsoleColours(image.GetPixel(x, y).ToArgb).Item1 ' get consolecolour equivalent of current pixel
                End If
                Console.Write(ChrW(ShapeIndex)) 'write same shape incase justification issue with console font
                Col += 1
            Next
            Col = PosX ' reset column count
            Row += 1
        Next
    End Sub
    ''' <summary>
    ''' Converts a given ARGB integer to Returns equivalent foreground colour and contrasting background
    ''' </summary>
    ''' <param name="Clr">ARGB integer of system.color</param>
    ''' <returns>Tuple of consoleforeground and consolebackground colours</returns>
    Public Function GetConsoleColours(ByVal Clr As Integer) As (ConsoleColor, ConsoleColor)
        'Dim argb As Integer = Integer.Parse(Hex.Replace("#", ""), NumberStyles.HexNumber)
        Dim c As Color = Color.FromArgb(Clr)

        ' Counting the perceptive luminance - human eye favors green color... 
        Dim a As Double = 1 - (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255
        Dim index As Integer = If(c.R > 128 Or c.G > 128 Or c.B > 128, 8, 0) ' Bright bit
        index = index Or If(c.R > 64, 4, 0) ' Red bit
        index = index Or If(c.G > 64, 2, 0) ' Green bit
        index = index Or If(c.B > 64, 1, 0) ' Blue bit
        Dim foregroundColor As ConsoleColor = CType(index, System.ConsoleColor)
        Dim backgroundColor As ConsoleColor = If(a < 0.5, ConsoleColor.Black, ConsoleColor.White)
        Return (foregroundColor, backgroundColor)
    End Function

End Module

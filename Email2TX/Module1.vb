' GPL License
' GitHub: https://github.com/id4rk

Imports System.Speech.Synthesis
Imports Limilabs.Mail
Imports Limilabs.Client.POP3
Imports System.IO.Ports
Imports System.Reflection
Imports System.Runtime.InteropServices

Module Module1

    <FlagsAttribute()>
    Public Enum EXECUTION_STATE As UInteger
        ES_SYSTEM_REQUIRED = &H1
        ES_DISPLAY_REQUIRED = &H2
        ES_CONTINUOUS = &H80000000UI
    End Enum

    <DllImport("Kernel32.DLL", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Function SetThreadExecutionState(ByVal state As EXECUTION_STATE) As EXECUTION_STATE
    End Function

    Dim HostName As String = "pop.server.net"
    Dim Email As String = "user@server.net"
    Dim Password As String = "password"

    Dim mySerialPort As New SerialPort("COM7")

    Dim MsgList As New List(Of String)
    Dim Stage As String = "CheckMail"
    Dim Nag As Boolean = False

    Dim TimerTick As New System.Timers.Timer
    Dim TimerRTO As New System.Timers.Timer
    Dim SerialR As Boolean = False

    Dim speaker As New SpeechSynthesizer()

    Sub Main()
        SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED)
        Console.WriteLine("Email Morser 1.0")
        Console.WriteLine("by id4rk")
        Console.WriteLine()
        SerialConnect()
        TimerTick.AutoReset = True
        TimerTick.Interval = 100
        AddHandler TimerTick.Elapsed, AddressOf Tick
        TimerTick.Start()

        TimerRTO.AutoReset = True
        TimerRTO.Interval = 1000
        AddHandler TimerRTO.Elapsed, AddressOf SerialRR

        Console.ReadKey()
    End Sub
    Private Sub Tick(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)

        If Stage = "CheckMail" Then
            TimerTick.Interval = 5000 '10000
            TimerTick.Enabled = False
            GetEmail()
            TimerTick.Enabled = True
        End If

        If Stage = "MorseOut" Then
            TimerTick.Enabled = False
            MorseOut()
            TimerTick.Enabled = True
        End If

        If Nag And (MsgList.Count = 0) Then
            TimerTick.Enabled = False
            TimerRTO.Enabled = False
            mySerialPort.Close()
            AppReset()
        End If

    End Sub
    Private Sub SerialRR(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
        SerialR = False
    End Sub
    Sub GetEmail()
        ConOut(0, 0, ".", ConsoleColor.Cyan)
        Try
            Using pop3 As New Pop3()
                pop3.ConnectSSL(HostName)
                pop3.Login(Email, Password)

                Dim SendMessage As Boolean = True
                Dim builder As New MailBuilder()

                For Each uid As String In pop3.GetAll()
                    Dim email As IMail = builder.CreateFromEml(pop3.GetMessageByUID(uid))

                    If email.Subject = "Please purchase Mail.dll license at http://www.limilabs.com/mail" Then
                        SendMessage = False
                        ConOut(0, 0, ".", ConsoleColor.Yellow)
                        Nag = True
                    End If

                    If SendMessage Then
                        MsgList.Add(email.Subject)
                        ConOut(0, 0, ".", ConsoleColor.Green)
                        pop3.DeleteMessageByUID(uid)
                        Stage = "MorseOut"
                    End If

                Next

                pop3.Close()
                pop3.Dispose()
            End Using

        Catch ex As Exception
            ConOut(0, 0, ".", ConsoleColor.Red)
        End Try

    End Sub
    Sub MorseOut()
        ConOut(1, 0, "", ConsoleColor.Blue)

        For i As Integer = 0 To MsgList.Count - 1
            ConOut(1, 1, "Message: " & MsgList.Item(i), ConsoleColor.DarkGreen)
            ConOut(0, 1, "Morsing: ", ConsoleColor.Green)
            SerialR = True
            SendSerialData(MsgList.Item(i))

            While SerialR
                'idle
            End While

            Threading.Thread.Sleep(2000)

        Next
        MsgList.Clear()
        Stage = "CheckMail"
    End Sub
    Sub ConOut(ByVal NewLine As Boolean, ByVal IncDate As Boolean, ByVal Text As String, ByVal Color As ConsoleColor)
        Console.ForegroundColor = Color
        If NewLine Then
            If IncDate Then
                Console.WriteLine(Date.Now & " " & Text)
            Else
                Console.WriteLine(Text)
            End If
        Else
            If IncDate Then
                Console.Write(Date.Now & " " & Text)
            Else
                Console.Write(Text)
            End If
        End If
        Console.ResetColor()
    End Sub
    Private Sub Speak(ByVal text As String)
        speaker.Rate = Convert.ToInt32(-1)
        speaker.Volume = Convert.ToInt32(100)
        speaker.SpeakAsync(text)
    End Sub
    Sub SerialConnect()
        mySerialPort.BaudRate = 115200
        mySerialPort.Parity = Parity.None
        mySerialPort.StopBits = StopBits.One
        mySerialPort.DataBits = 8
        mySerialPort.Handshake = Handshake.XOnXOff
        mySerialPort.RtsEnable = True
        AddHandler mySerialPort.DataReceived, AddressOf DataReceivedHandler
        mySerialPort.Open()
    End Sub
    Private Sub DataReceivedHandler(sender As Object, e As SerialDataReceivedEventArgs)
        TimerRTO.Enabled = False
        Dim sp As SerialPort = CType(sender, SerialPort)
        Dim indata As String = sp.ReadExisting()
        Console.Write(indata)
        TimerRTO.Enabled = True
    End Sub
    Sub SendSerialData(ByVal data As String)
        mySerialPort.WriteLine(data)
    End Sub
    Sub AppReset()
        Dim asm As Assembly = Assembly.GetExecutingAssembly()
        Dim asmName As String = asm.GetName().Name() & ".exe"
        Dim p As New ProcessStartInfo
        p.FileName = System.Environment.CurrentDirectory & "\" & asmName
        p.WindowStyle = ProcessWindowStyle.Minimized
        Process.Start(p)
        Environment.Exit(0)
    End Sub
End Module

' SendSerialData(email.Subject)

'ConOut(0, 1, "Morsing: ", ConsoleColor.Cyan)
'Speak(email.Subject)

'If DeleteAllEmails  Then
'ConOut("Cleaning...", ConsoleColor.Red)
'For Each uid As String In pop3.GetAll()
'pop3.DeleteMessageByUID(uid)
'Next
'End If

' From
'For Each m As MailBox In email.From
'Console.WriteLine(m.Address)
'Console.WriteLine(m.Name)
'Next

'Console.WriteLine(email.Subject)

' Date
'Console.WriteLine(email.[Date])

' Text body of the message
'Console.WriteLine(email.Text)

' Html body of the message
'Console.WriteLine(email.Html)
' Custom header
'Console.WriteLine(email.Document.Root.Headers("x-spam-value"))

' Save all attachments to disk
'For Each mime As MimeData In email.Attachments
'MIME.Save("c:\" + mime.SafeFileName)
'Next


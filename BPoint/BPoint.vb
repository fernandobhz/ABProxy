Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Threading
Imports System.Net.Sockets

Public Module BPoint

    Sub Main()
        Dim Path = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim Params = System.IO.Path.GetFileNameWithoutExtension(Path)

        If Params.StartsWith("BPoint") Then
            Params = Mid(Params, "BPoint".Length + 1)
        End If

        If Not Params.Contains("@") Then Params = InputBox(
            "You can define the params the filename.exe" & vbCrLf & vbCrLf &
            "Example: localhost-3000@remote-proxy-8080" & vbCrLf & vbCrLf &
            "This will connect to the proxy server at: " & vbCrLf &
            "'remote-proxy-8080'" & vbCrLf & vbCrLf &
            "Incoming connections will be forwarded to" & vbCrLf &
            "'localhost:3000'", "Params: FinalServerHost-Port @ ProxyHost-Port", "localhost-3000@remote-proxy-8080")

        Dim Matches = Regex.Matches(Params, "([^:]+)[-]([0-9]+)[@]([^:]+)[-]([0-9]+)")

        Dim FinalServerHost As String = Matches(0).Groups(1).Value
        Dim FinalServerPort As String = Matches(0).Groups(2).Value
        Dim ProxyHost As String = Matches(0).Groups(3).Value
        Dim ProxyPort As String = Matches(0).Groups(4).Value

        Dim ABConnection As TcpClient = Nothing

        Do
            Try
                If ABConnection Is Nothing OrElse Not ABConnection.Connected Then
                    ABConnection = Tcp.Connect(ProxyHost, ProxyPort)
                    TcpProxy.Open(ABConnection, FinalServerHost, FinalServerPort)
                Else
                    Thread.Sleep(100)
                End If
            Catch ex As Exception 'When Not Debugger.IsAttached
                ABConnection = Nothing
                'MsgBox(ex.Message)
            End Try
        Loop

    End Sub

End Module

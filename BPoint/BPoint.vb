Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Threading
Imports System.Net.Sockets

Public Module BPoint

    Sub Main()
        Dim Path = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim Params = System.IO.Path.GetFileNameWithoutExtension(Path)
        If Not Params.Contains("@") Then Params = InputBox("Parametros", "Parametros", "localhost-80@localhost-32128")
        Dim Matches = Regex.Matches(Params, "([^:]+)[-]([0-9]+)[@]([^:]+)[-]([0-9]+)")

        Dim ServerHost As String = Matches(0).Groups(1).Value
        Dim ServerPort As String = Matches(0).Groups(2).Value
        Dim AHost As String = Matches(0).Groups(3).Value
        Dim APort As String = Matches(0).Groups(4).Value

        Dim ABConnection As TcpClient = Nothing

        Do
            Try
                If ABConnection Is Nothing OrElse Not ABConnection.Connected Then
                    ABConnection = Tcp.Connect(AHost, APort)
                    TcpProxy.Open(ABConnection, ServerHost, ServerPort)
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

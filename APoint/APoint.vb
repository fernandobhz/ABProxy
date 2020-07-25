Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Net.Sockets

Public Module APoint

    Sub Main()
        Dim Path = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim Params = System.IO.Path.GetFileNameWithoutExtension(Path)
        If Not Params.Contains("@") Then Params = InputBox("Parametros", "Parametros", "3264@32128")
        Dim Matches = Regex.Matches(Params, "([0-9]+)[@]([0-9]+)") : If Matches.Count = 0 Then Exit Sub

        Dim ClientPort = Matches(0).Groups(1).Value
        Dim APort = Matches(0).Groups(2).Value

        Dim ABConnection As TcpClient = Nothing

        Do
            Try
                If ABConnection Is Nothing OrElse Not ABConnection.Connected Then
                    ABConnection = Tcp.Wait(APort)
                    TcpProxy.Open(Tcp.Wait(ClientPort), ABConnection)
                Else
                    Thread.Sleep(100)
                End If
            Catch ex As Exception When Not Debugger.IsAttached
                ABConnection = Nothing
                'MsgBox(ex.Message)
            End Try
        Loop

    End Sub

End Module

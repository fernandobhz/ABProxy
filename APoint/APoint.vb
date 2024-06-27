Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Net.Sockets

Public Module APoint

    Sub Main()
        Dim Path = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim Params = System.IO.Path.GetFileNameWithoutExtension(Path)

        If Params.StartsWith("APoint") Then
            Params = Mid(Params, "APoint".Length + 1)
        End If

        If Not Params.Contains("@") Then Params = InputBox(
            "You can define the params the filename.exe" & vbCrLf & vbCrLf &
            "Example: 80@8080" & vbCrLf & vbCrLf &
            "This will expose the port 80 to the Web" & vbCrLf & vbCrLf &
            "While waiting proxy connections from port 8080", "Params: ExposedToWoldWideWebPort @ ProxyPort", "80@8080")

        Dim Matches = Regex.Matches(Params, "([0-9]+)[@]([0-9]+)") : If Matches.Count = 0 Then Exit Sub

        Dim ExposedToWoldWideWebPort = Matches(0).Groups(1).Value
        Dim ProxyPort = Matches(0).Groups(2).Value

        Dim ABConnection As TcpClient = Nothing

        Do
            Try
                If ABConnection Is Nothing OrElse Not ABConnection.Connected Then
                    ABConnection = Tcp.Wait(ProxyPort)
                    TcpProxy.Open(Tcp.Wait(ExposedToWoldWideWebPort), ABConnection)
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

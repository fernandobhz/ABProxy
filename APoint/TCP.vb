Imports System.Net.Sockets
Imports System.Net
Imports System.Threading

Class Tcp
    Private Shared Servers As List(Of TcpListener)

    Public Shared Function Wait(Port As Integer) As TcpClient
        If Servers Is Nothing Then
            Servers = New List(Of TcpListener)
        End If

        Dim Server As TcpListener = Servers.SingleOrDefault(Function(x) CType(x.LocalEndpoint, IPEndPoint).Port = Port)

        If Server Is Nothing Then
            Server = New TcpListener(Net.IPAddress.Any, Port)
            Server.Start()

            Servers.Add(Server)
        End If

        Dim TcpClient = Server.AcceptTcpClient
        Return TcpClient
    End Function

    Public Shared Function Connect(Host, Port) As TcpClient
        Do
            Try
                Return New TcpClient(Host, Port)
            Catch ex As Exception
                Thread.Sleep(100)
            End Try
        Loop
    End Function

End Class

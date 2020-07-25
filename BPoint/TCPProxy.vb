Imports System.Net.Sockets
Imports System.Threading

Class TcpProxy
    Public Shared Sub Open(AClient As TcpClient, BClient As TcpClient)
        Dim TcpProxy As New TcpProxy(AClient, BClient)
        TcpProxy.Open(Nothing)
    End Sub

    Public Shared Sub Open(AClient As TcpClient, BHost As String, BPort As Integer)
        Dim TcpProxy As New TcpProxy(AClient, BHost, BPort)
        TcpProxy.Open(Nothing)
    End Sub

    Private AClient As TcpClient
    Private BClient As TcpClient

    Sub New(AClient As TcpClient, BClient As TcpClient)
        Me.AClient = AClient
        Me.BClient = BClient
    End Sub

    Private BHost As String
    Private BPort As Integer

    Sub New(AClient As TcpClient, BHost As String, BPort As Integer)
        Me.AClient = AClient
        Me.BHost = BHost
        Me.BPort = BPort
    End Sub

    Private TA2B As Thread
    Private TB2A As Thread

    Private ConnectedTokenStr As String
    Private ConnectedTokenBuff As Byte()

    Private Sub Open(Sender As Object)
        ConnectedTokenStr = "[{(ABPROXY:CONNECTED)}]"
        ConnectedTokenBuff = System.Text.Encoding.UTF8.GetBytes(ConnectedTokenStr)

        TA2B = New Thread(AddressOf A2B)
        TA2B.Name = "TCPProxy A2B"
        TA2B.IsBackground = True

        TB2A = New Thread(AddressOf B2A)
        TB2A.Name = "TCPProxy B2A"
        TB2A.IsBackground = True

        If AClient IsNot Nothing Then TA2B.Start()
        If BClient IsNot Nothing Then
            BClient.GetStream.Write(ConnectedTokenBuff, 0, ConnectedTokenBuff.Length)
            TB2A.Start()
        End If

    End Sub

    Private Sub Close()
        If AClient IsNot Nothing Then AClient.Close()
        If BClient IsNot Nothing Then BClient.Close()
    End Sub

    Private Sub A2B()
        Try
            Dim Buffer(1024 * 1024) As Byte

            While AClient.Connected
                Dim bytesRead As Integer = AClient.GetStream.Read(Buffer, 0, Buffer.Length)
                If bytesRead = 0 Then
                    Me.Close()
                    Exit Sub
                ElseIf bytesRead = ConnectedTokenBuff.Length Then
                    Dim CompareStr = System.Text.Encoding.UTF8.GetString(ConnectedTokenBuff)
                    If CompareStr = ConnectedTokenStr Then
                        If BClient Is Nothing Then
                            BClient = New TcpClient
                            BClient.Connect(BHost, BPort)

                            TB2A.Start()

                        End If
                    End If
                Else
                    BClient.GetStream.Write(Buffer, 0, bytesRead)
                End If
            End While
        Catch ex As Exception 'When Not Debugger.IsAttached
            'MsgBox(ex.Message)
            Me.Close()
            Exit Sub
        End Try
    End Sub

    Private Sub B2A()
        Try
            Dim Buffer(1024 * 1024) As Byte

            While AClient.Connected
                Dim bytesRead As Integer = BClient.GetStream.Read(Buffer, 0, Buffer.Length)
                If bytesRead = 0 Then
                    Me.Close()
                    Exit Sub
                End If

                AClient.GetStream.Write(Buffer, 0, bytesRead)
            End While
        Catch ex As Exception 'When Not Debugger.IsAttached
            'MsgBox(ex.Message)
            Me.Close()
            Exit Sub
        End Try
    End Sub

End Class

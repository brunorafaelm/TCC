Imports System.ComponentModel
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Partial Public Class MacroSocket

    Dim tcpListener As New TcpListener(4400)

    Shared Sub New()
        DevExpress.UserSkins.BonusSkins.Register()
    End Sub
    Public Sub New()
        InitializeComponent()
    End Sub

    Dim estado As Boolean = False

    Private Function ValidaCampos() As Boolean
        If txIp.Text = Nothing Then
            Return False
        End If

        If txPorta.Text = Nothing Then
            Return False
        End If

        If txDelay.Text = Nothing Then
            Return False
        End If

        Return True
    End Function

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles btConectar.Click
        If Not ValidaCampos() Then
            MessageBox.Show("Algum dos campos de textos estão vazios ou invalidos!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim tcpClient As New TcpClient
        Dim numeroPorta As Integer = CInt(txPorta.Text)
        tcpListener = New TcpListener(numeroPorta)
        Dim t1 As New Thread(Sub() Me.ConectaCliente(tcpClient, tcpListener))

        If btConectar.Text = "Desconectar" Then
            Try
                t1.Abort()
                tcpClient.Close()
                tcpListener.Stop()
                MessageBox.Show("Encerrado conexão com o servidor", "Encerrado", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.txEstado.Text = "DESCONECTADO"
                Me.btConectar.Text = "Conectar"
                Me.txEstado.ForeColor = Color.Red
                Exit Sub
            Catch ex As Exception
                Throw
            End Try
        Else
            Try
                tcpListener.Start()
                Me.txEstado.Text = "CONECTADO"
                Me.btConectar.Text = "Desconectar"
                Me.txEstado.ForeColor = Color.Green
                t1.Start()
            Catch ex As Exception
                Throw
            End Try
        End If
    End Sub


    Private Sub ConectaCliente(tcpClient As TcpClient, tcpListener As TcpListener)
        Try
            tcpClient = tcpListener.AcceptTcpClient()

            Dim ultimoDadoRecbido As String = Nothing

            While tcpClient.Connected
                tcpClient = tcpListener.AcceptTcpClient()

                ' obtem o stream
                Dim networkStream As NetworkStream = tcpClient.GetStream()

                If Not networkStream.CanRead Then
                    Continue While
                End If

                ' le o stream em um array de bytes
                Dim bytes(tcpClient.ReceiveBufferSize) As Byte
                networkStream.Read(bytes, 0, CInt(tcpClient.ReceiveBufferSize))

                ' Retorna os dados recebidos do cliente para o console
                Dim clientdata As String = Encoding.ASCII.GetString(bytes)

                If clientdata.Substring(0, 1) = "0" Then
                    'MessageBox.Show("Cliente enviou : " + clientdata)
                    'EmulaPressionamentoTecla()
                ElseIf clientdata.Substring(0, 1) = "1" And ultimoDadoRecbido <> "1" Then
                    'MessageBox.Show("Cliente enviou : " + clientdata)
                    EmulaPressionamentoTecla()
                End If
            End While

        Catch ex As Exception
            Console.WriteLine(ex.ToString())
            Console.ReadLine()
        End Try

    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        Dim localIPs As New List(Of IPAddress)
        localIPs.AddRange(Dns.GetHostAddresses(Dns.GetHostName()))

        For Each ip As IPAddress In localIPs

            If ip.AddressFamily = AddressFamily.InterNetwork Then
                txIp.Text = ip.MapToIPv4.ToString
            End If
        Next
    End Sub

    Private Sub EmulaPressionamentoTecla()

        If cbMacro.SelectedItem Is "F12" Then
            SendKeys.SendWait("{F12}")
            System.Threading.Thread.Sleep(500)
            SendKeys.SendWait("{F12}")
        End If


    End Sub

    Private Sub MacroSocket_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        tcpListener.Stop()
    End Sub
End Class

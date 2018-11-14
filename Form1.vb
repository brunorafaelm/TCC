Imports System.ComponentModel
Imports System.Net
Imports System.Net.Sockets
Imports System.Text


Partial Public Class MacroSocket
    Shared Sub New()
        DevExpress.UserSkins.BonusSkins.Register()
    End Sub
    Public Sub New()
        InitializeComponent()
    End Sub

    Dim estado As Boolean = False
    Dim tcpClient As New TcpClient


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

        Dim numeroPorta As Integer = CInt(txPorta.Text)

        Dim tcpListener As New TcpListener(numeroPorta)

        If estado = True Then
            tcpClient.Close()
            tcpListener.Stop()
            MessageBox.Show("Encerrando conexão com o servidor", "Encerrando")
        Else


            tcpListener.Start()

            Dim janelaAguardando = MessageBox.Show("Aguardando uma conexão...", "Conectando")

            Try
                'aceita a conexao do cliente e retorna uma inicializacao TcpClient 
                tcpClient = tcpListener.AcceptTcpClient()
                MessageBox.Show("Conexão aceita.", "Conectado", MessageBoxButtons.OK)


                ' obtem o stream
                Dim networkStream As NetworkStream = tcpClient.GetStream()

                ' le o stream em um array de bytes
                Dim bytes(tcpClient.ReceiveBufferSize) As Byte
                networkStream.Read(bytes, 0, CInt(tcpClient.ReceiveBufferSize))


                '' Retorna os dados recebidos do cliente para o console
                'Dim clientdata As String = Encoding.ASCII.GetString(bytes)
                'Console.WriteLine(("Cliente enviou : " + clientdata))

                ''qualquer comunicacao com o cliente remoto usando o TcpClient pode comecar aqui
                'Dim responseString As String = "Conectado ao servidor."
                'Dim sendBytes As [Byte]() = Encoding.ASCII.GetBytes(responseString)
                'networkStream.Write(sendBytes, 0, sendBytes.Length)
                'Console.WriteLine(("Mensagem enviada /> : " + responseString))

                estado = True

            Catch ex As Exception
                Console.WriteLine(ex.ToString())
                Console.ReadLine()
            End Try
        End If


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

    Private Sub SimpleButton1_Click_1(sender As Object, e As EventArgs)
        SendKeys.Send("{F12}")
    End Sub
End Class

using System.Net;
using System.Net.Sockets;

namespace Dedicated_Server_using_dotnet
{
    public class Server
    {
        public static int MaxPlayers { get; private set; }  //서버로 연결해올 최대 클라 수
        public static int port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();  //연결된 클라 리스트
        
        private static TcpListener tcpListener; //서버의 tcp 리스너. 클라로부터의 연결시도를 감지함.

        //서버 시작 함수.
        //서버 실행에 필요한 초기 세팅이 들어감.
        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();     //서버 데이터 초기화.

            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();    //begin accept TCP Client
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server start on {port}");
        }

        //TCP Client로부터 연결 시도시 콜백함수.
        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);    //연결 시도한 TCP Client 받기
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);  //다음 사람 받기 위해 새로이 대기.
            Console.WriteLine($"Incoming connect from :: {_client.Client.RemoteEndPoint}");

            //연결된 클라를 클라 리스트에 넣기.
            for (int i = 1; i < MaxPlayers; i++)
            {
                if(clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            //여기까지 온거면 서버가 꽉 찬 거임.
            Console.WriteLine($"{_client.Client.RemoteEndPoint} :: failed to connect. - server is full.");
        }

        //서버 데이터 초기화. (connected client dictionary 초기화)
        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }
        }
    }
}
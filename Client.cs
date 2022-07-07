using System.Net;
using System.Net.Sockets;

namespace Dedicated_Server_using_dotnet
{
    public class Client
    {
        public static int dataBufferSize = 4096;

        public int id;
        public TCP tcp; //각 client 마다 가지게 되는 TCP 인스턴스

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
        }

        //Server.cs 에서 콜백함수로 받은 TCP 인스턴스 정보를 저장하기 위해 만든 클래스.
        public class TCP
        {
            public TcpClient socket;    //서버로 TCP 연결 시도한 클라 소켓.

            private readonly int id;
            private NetworkStream stream;   //서버와 클라간의 넷상 스트림
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                 id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;  //send-recv 버퍼 사이즈 지정
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();    //소켓으로부터 스트림 추출.

                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);  //버퍼 등록, 콜백함수 연결 후 receive 시작.

                //todo : 여기에 연결성공 후 welcome 패킷 send
            }

            //이 콜백함수가 클라로부터 온 데이터를 처리하는 거겠지.
            private void ReceiveCallback(IAsyncResult _result)
            {
                //try-catch 를 써서 통신 에러가 발생해도 서버가 죽지 않도록 하자.
                try
                {
                    int _byteLength = stream.EndRead(_result);  //도착한 데이터를 받고 스트림 종료.
                    if(_byteLength <= 0)
                    {
                        //데이터가 스트림에 없으면 콜백함수 종료. 나중엔 disconnect 처리할 것임.
                        return;
                    }

                    byte[] _data = new byte[_byteLength];   //스트림으로 온 데이터 받기 위한 배열
                    Array.Copy(receiveBuffer, _data, _byteLength);  //스트림으로 온 데이터 받기.

                    //나중엔 여기서 데이터를 핸들링 할 것임. 당장은 무처리.
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);  //데이터 받았으니 새로운 스트림 개최.

                } catch(Exception _ex)
                {
                    Console.WriteLine($"Error receiving TCP data : {_ex}");
                    //todo : 에러나면 해당 client disconnect
                }
            }
        }
    }
}
namespace Dedicated_Server_using_dotnet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Game Server";  //콘솔 프로세서 이름

            Server.Start(50, 34343);
            Console.WriteLine("Server is Online");

            Console.ReadKey();  //키 입력 대기. 안하면 콘솔이 바로 꺼짐.

            Console.WriteLine("Server is Offline");
        }
    }
}
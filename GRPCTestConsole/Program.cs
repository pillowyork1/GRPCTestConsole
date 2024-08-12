using Grpc.Core;
using Grpc.Net.Client;
using Proto.Messages;
using Proto.Greet;

namespace GRPCTestServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando GRPC Test Console");
            Console.WriteLine("Criando um server");
            criandoServer();
            Console.WriteLine("Server criado");
        }

        public class GreeterService : Greeter.GreeterBase
        {
            public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
            {
                return Task.FromResult(new HelloReply
                {
                    Message = $"Hello " + request.Name                    
                });

                
            }
        }

        static void criandoServer()
        {
            const int Port = 5000;

            Server server = null;
            try
            {
                //var keyCertificatePair = new KeyCertificatePair(
                //    File.ReadAllText("C:\\Dev\\GRPCTest\\localhost.pfx")
                //    );
                //var sslCredentials = new SslServerCredentials(new List<KeyCertificatePair> { keyCertificatePair });

                server = new Server
                {
                    Services = { Greeter.BindService(new GreeterService()) },
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("Greeter server listening on port " + Port);

                // Manter o servidor em execução até que o usuário o interrompa
                Console.WriteLine("Pressione qualquer tecla para parar o servidor...");
                Console.ReadKey();

                // Parar o servidor
                Console.WriteLine("Parando o servidor...");
                server.ShutdownAsync();
                Console.WriteLine("Servidor parado.");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server?.ShutdownAsync().Wait();
            }
        }


        async Task chamarSecurityListAsync(GrpcChannel channel)
        {
            // Create a client for the MarketDataService
            var client = new MarketDataService.MarketDataServiceClient(channel);

            // Create a request
            var request = new SecurityListRequest { };

            // Call the GetSecurityList service
            var reply = client.GetSecurityList(request);

            // Process the stream of responses
            await foreach (var response in reply.ResponseStream.ReadAllAsync())
            {
                // Do something with each response
                Console.WriteLine(response.Ticker);
            }
        }

    }
}





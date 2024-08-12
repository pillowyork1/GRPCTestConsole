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
            //criandoServer();
            CriarOuPararServidor();
            //Console.WriteLine("Server criado");         
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

        static void CriandoServer()
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

        static void CriarOuPararServidor()
        {
            const int Port = 5000;
            Server server = null;
            bool servidorRodando = false;

            while (true)
            {
                if (servidorRodando)
                {                    
                    // Parar o servidor
                    Console.WriteLine("\nParando o servidor...");
                    server.ShutdownAsync().Wait();
                    Console.WriteLine("Servidor parado.");
                    servidorRodando = false;
                    Console.WriteLine("Pressione qualquer tecla para iniciar o servidor...");
                    Console.ReadKey();
                }
                else
                {
                    // Iniciar o servidor
                    try
                    {                        
                        server = new Server
                        {
                            Services = { Greeter.BindService(new GreeterService()) },
                            Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                        };
                        server.Start();
                        Console.WriteLine("Servidor gRPC iniciado e ouvindo na porta " + Port);
                        servidorRodando = true;
                        Console.WriteLine("Pressione qualquer tecla para parar o servidor...");
                        Console.ReadKey();


                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Erro ao iniciar o servidor: " + e.Message);
                    }
                }
            }
        }

        async Task ChamarSecurityListAsync(GrpcChannel channel)
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





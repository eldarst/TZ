using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;

namespace Grpc_Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            using var call = client.SolveExpressionStream();

            var readTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(response.Answer);
                }
            });

            while(true)
            {
                var result = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(result))
                {
                    break;
                }

                if (!ExpressionChecker.ExpressionIsCorrect(result))
                {
                    Console.WriteLine("Expression is not correct! Please correct it and enter again.");
                    continue;
                }
                    

                await call.RequestStream.WriteAsync(new ExpressionRequest() { Expression = result });
            }

            await call.RequestStream.CompleteAsync();
            await readTask;

            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServer
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task SolveExpressionStream(IAsyncStreamReader<ExpressionRequest> requestStream,
            IServerStreamWriter<ExpressionReply> responseStream, ServerCallContext context)
        {
            await foreach(var request in requestStream.ReadAllAsync())
            {
                await responseStream.WriteAsync(new ExpressionReply()
                {
                    Answer = "Answer is: " + Calculator.Evaluator.Evaluate(request.Expression)
                }) ;
            }
        }
    }
}

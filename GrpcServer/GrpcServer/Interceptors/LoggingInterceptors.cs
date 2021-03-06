using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace GrpcServer.Interceptors
{
    public class LoggingInterceptors : Interceptor
    {
        private ILogger<LoggingInterceptors> logger;

        public LoggingInterceptors(ILogger<LoggingInterceptors> logger)
        {
            this.logger = logger;
        }


        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            logger.Log(LogLevel.Information, $"Start - {context.Method}");

            await base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);

            logger.Log(LogLevel.Information, $"End - {context.Method}");
        }

    }
}

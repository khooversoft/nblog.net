using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Extensions;

namespace Toolbox.Logging
{
    public static class LoggerExtensions
    {
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, string loggingFolder, string baseLogFileName, int limit = 10)
        {
            builder.AddProvider(new FileLoggerProvider(loggingFolder, baseLogFileName, limit));
            return builder;
        }

        public static ILoggingBuilder AddLogger(this ILoggingBuilder builder, ITargetBlock<string> queue)
        {
            builder.AddProvider(new TargetBlockLoggerProvider(queue));
            builder.AddFilter<TargetBlockLoggerProvider>(x => true);

            return builder;
        }

        public static void DumpHeaders(this IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, string label, ILogger logger) => headers
            .Select(x => $"Header {x.Key} = Value: {string.Join(", ", x.Value)}")
            .Aggregate(string.Empty, (a, x) => a += x + Environment.NewLine)
            .Action(x => logger.LogTrace($"{label}: {x}"));

        public static async Task LogTrace(this ILogger logger, HttpRequestMessage subject)
        {
            const string label = "httpRequest";
            logger.LogTrace($"{label}: Uri={subject.RequestUri}, Method={subject.Method.Method}");

            if (subject.Content != null)
            {
                logger.Log(LogLevel.Trace, $"{label}: Content: {await subject.Content.ReadAsStringAsync()}");
            }

            subject.Headers.DumpHeaders(label, logger);
        }

        public static async Task LogTrace(this ILogger logger, HttpResponseMessage message)
        {
            const string label = "httpResponse";
            if (message.RequestMessage == null) return;

            await logger.LogTrace(message);

            if (message.Content != null)
            {
                logger.Log(LogLevel.Trace, $"{label}: Content: {await message.Content.ReadAsStringAsync()}");
            }

            message.Headers.DumpHeaders(label, logger);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nancy;
using Serilog;
using TeamBot.Infrastructure.Messages;
using TeamBot.Infrastructure.Slack;
using TeamBot.Infrastructure.Slack.Models;

namespace TeamBot.Features.LinqPad
{
    public class LinqPadHandler : SlackMessageHandler
    {
        private readonly IRootPathProvider _pathProvider;

        public LinqPadHandler(ISlackClient slack, IRootPathProvider pathProvider) : base(slack)
        {
            _pathProvider = pathProvider;
        }

        public override async Task<Message> Handle(IncomingMessage incomingMessage)
        {
            if (incomingMessage == null)
                throw new ArgumentNullException(nameof(incomingMessage));

            var patterns = new Dictionary<string, Func<IncomingMessage, Match, Task<Message>>>
            {
                { "^(lp) (.*)", async (message, match) => await LinqpadAsync(message, match.Groups[2].Value) },
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.TriggerWord, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    return await pattern.Value(incomingMessage, match);
            }

            return null;
        }

        private async Task<Message> LinqpadAsync(IncomingMessage message, string value)
        {
            var fileName = await WriteScriptToFile(value);

            Log.Debug("Linqpad handling {Command} with input {Input} to filename {FileName}", message, value, fileName);

            var compilation = await LINQPad.Util.CompileAsync(fileName);

            if (compilation.Warnings.Any())
            {
                var errorsString = string.Join(Environment.NewLine, $"{compilation.Warnings.Length} found: ",
                    compilation.Warnings);

                var replyMessage = new Message()
                {
                    UserName = message.ReplyTo(),
                    Text = errorsString
                };

                Log.Information("Returning with compilation warnings {WarningMessage}", replyMessage);

                return replyMessage;
            }

            var executer = compilation.Run(LINQPad.QueryResultFormat.Text);
            var result = await executer.AsStringAsync();

            Log.Debug("Result of script {Script}: {Result}", value, result);

            return new Message()
            {
                Text = result,
                UserName = message.ReplyTo()
            };
        }

        private async Task<string> WriteScriptToFile(string script)
        {
            var fileName = Path.GetTempFileName();

            var fileContents = Encoding.UTF8.GetBytes(script);
            using (var fileStream = File.OpenWrite(fileName))
            {
                await fileStream.WriteAsync(fileContents, 0, fileContents.Length);
            }

            return fileName;
        }
    }
}
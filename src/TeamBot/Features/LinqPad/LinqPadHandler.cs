using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nancy;
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
                { "^(lp|linqpad|linq) (.*)", async (message, match) => await LinqpadAsync(message, match.Groups[2].Value) },
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(incomingMessage.Text, pattern.Key, RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    return await pattern.Value(incomingMessage, match);
            }

            return null;
        }

        private async Task<Message> LinqpadAsync(IncomingMessage message, string value)
        {
            var fileName = await WriteScriptToFile(message, value);

            var compilation = await LINQPad.Util.CompileAsync(fileName);

            if (compilation.Warnings.Any())
            {
                var errorsString = string.Join(Environment.NewLine, $"{compilation.Warnings.Length} found: ",
                    compilation.Warnings);

                await Slack.SendAsync(message.ReplyTo(), errorsString);
            }

            var executer = compilation.Run(LINQPad.QueryResultFormat.Html);
            var result = await executer.AsStringAsync();

            return new Message()
            {
                Text = result,
                UserName = message.ReplyTo()
            };
        }

        private async Task<string> WriteScriptToFile(IncomingMessage message, string script)
        {
            var fileName = $"{message.UserName}-{DateTime.UtcNow.ToString("o")}-{Guid.NewGuid()}";
            var scriptDirectory = Path.Combine(_pathProvider.GetRootPath(), "LinqPadScripts");

            if (!Directory.Exists(scriptDirectory)) Directory.CreateDirectory(scriptDirectory);

            var fullFileName = Path.Combine(scriptDirectory, fileName);

            var fileContents = Encoding.UTF8.GetBytes(script);
            using (var fileStream = File.OpenWrite(fileName))
            {
                await fileStream.WriteAsync(fileContents, 0, fileContents.Length);
            }

            return fullFileName;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Humanizer;
using Nancy;
using Serilog;
using Sprache;
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
                {"^(lp) (.*)", async (message, match) => await LinqpadAsync(message, match.Groups[2].Value)},
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match($"{incomingMessage.TriggerWord} {incomingMessage.Text}", pattern.Key,
                    RegexOptions.IgnoreCase);
                if (match.Length > 0)
                    return await pattern.Value(incomingMessage, match);
            }

            return null;
        }

        private class KeyValue
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private class ScriptToRun
        {
            public IList<KeyValue> Args { get; set; }
            public string Script { get; set; }

            public string ToArgsString()
            {
                return Args.Aggregate("", (argString, arg) => argString + $"{arg.Key}={arg.Value} ").Trim();
            }
        }

        private ScriptToRun ParseInput(string both)
        {
            var trimmed = both.Trim();

            var flagParser =
                from leading in Parse.Char('-')
                from flag in Parse.Letter.AtLeastOnce().Text()
                from separator in Parse.Char('=')
                from value in Parse.LetterOrDigit.AtLeastOnce().Text()
                from whiteSpace in Parse.WhiteSpace.AtLeastOnce()
                select new KeyValue {Key = flag, Value = value};

            var scriptParser =
                from parsedFlags in flagParser.Many()
                from parsedScript in Parse.AnyChar.AtLeastOnce().Text()
                select parsedScript;

            var flags = flagParser.Many().TryParse(trimmed).Value;
            var script = scriptParser.TryParse(trimmed).Value;

            return new ScriptToRun()
            {
                Args = flags.ToList(),
                Script = script,
            };
        }


        private async Task<Message> LinqpadAsync(IncomingMessage message, string value)
        {
            var script = ParseInput(value);

            var fileName = await WriteScriptToFile(script.Script);

            Log.Debug("Linqpad handling {Command} processed as args: {Args}, script {Script} to filename {FileName}", message, script.Args, script.Script, fileName);

            var compilation = await LINQPad.Util.CompileAsync(fileName);

            if (compilation.Warnings.Any())
            {
                var errorsString = string.Join(Environment.NewLine, $"{compilation.Warnings.Length} found: ",
                    compilation.Warnings);

                var replyMessage = new Message()
                {
                    UserName = message.ReplyTo(),
                    Text = errorsString.Truncate(500)
                };

                Log.Information("Returning with compilation warnings {WarningMessage}", replyMessage);

                return replyMessage;
            }

            var executer = compilation.Run(LINQPad.QueryResultFormat.Text, script.Args);

            var result = await executer.AsStringAsync();

            Log.Debug("Result of script {Script}: {Result}", value, result);

            return new Message()
            {
                Text = result.Truncate(1000),
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
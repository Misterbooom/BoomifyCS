using System.Collections.Generic;
using Spectre.Console;

namespace BoomifyCS.Exceptions
{
    public abstract class BifyError
    {
        public int CurrentLine { get; set; }
        public int Column { get; set; }
        public List<CallStackFrame> CallStack { get; set; } = new();
        public string LineTokensString { get; set; } = "";
        public string InvalidTokensString { get; set; }
        public string FileName { get; set; } = "0";
        public readonly string Message;

        protected BifyError() { }

        protected BifyError(string message, string tokens, string invalidTokens, int currentLine = 1, int column = 0)
        {
            InvalidTokensString = invalidTokens;
            LineTokensString = tokens;
            CurrentLine = currentLine;
            Column = column;
            Message = message;
        }

        public void PrintException()
        {
            string errorType = this.GetType().Name.Replace("Bify", "");

            var panel = new Panel($"[indianred]{Markup.Escape(Message)}[/]")
            {
                Header = new PanelHeader($"[red bold]{errorType}[/]"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0, 1, 0)
            };
            AnsiConsole.Write(panel);

            AnsiConsole.MarkupLine($"    File [orangered1]'{Markup.Escape(FileName)}'[/], Line [yellow]{CurrentLine}[/], Column [yellow]{Column}[/]");

            WriteLineTokens(10);
            PrintCallStack();
        }

        public void WriteLineTokens(int indentInt)
        {
            string indent = new string(' ', indentInt);

            if (string.IsNullOrEmpty(InvalidTokensString) || Column <= 0)
            {
                AnsiConsole.MarkupLine(indent + $"[grey]{Markup.Escape(LineTokensString)}[/]");
                return;
            }

            int errorIndex = Column - 1;
            int errorLength = InvalidTokensString.Length;

            if (errorIndex < 0 || errorIndex >= LineTokensString.Length)
            {
                AnsiConsole.MarkupLine(indent + $"[grey]{Markup.Escape(LineTokensString)}[/]");
                return;
            }

            if (errorIndex + errorLength > LineTokensString.Length)
            {
                errorLength = LineTokensString.Length - errorIndex;
            }

            string beforeError = LineTokensString.Substring(0, errorIndex);
            string errorPart = LineTokensString.Substring(errorIndex, errorLength);
            string afterError = LineTokensString.Substring(errorIndex + errorLength);

            AnsiConsole.Markup(indent);
            AnsiConsole.Markup($"[grey]{Markup.Escape(beforeError)}[/]");
            AnsiConsole.Markup($"[red underline]{Markup.Escape(errorPart)}[/]");
            AnsiConsole.MarkupLine($"[grey]{Markup.Escape(afterError)}[/]");

            AnsiConsole.MarkupLine(indent + new string(' ', errorIndex) + $"[red]{new string('^', errorLength)}[/]");
        }

        private void PrintCallStack()
        {
            if (CallStack == null || CallStack.Count == 0) return;

            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey)
                .Title("[indianred]Call Stack[/]");

            table.AddColumn("[grey]File[/]");
            table.AddColumn("[grey]Function[/]");
            table.AddColumn("[grey]Line[/]");
            table.AddColumn("[grey]Code[/]");

            foreach (var frame in CallStack)
            {
                table.AddRow(
                    $"[indianred]{Markup.Escape(frame.FilePath)}[/]",
                    $"[red]{Markup.Escape(frame.FunctionName)}[/]",
                    $"[yellow]{frame.LineNumber}[/]",
                    $"[darkgray]{Markup.Escape(frame.CodeLine ?? string.Empty)}[/]"
                );
            }

            AnsiConsole.Write(table);
        }

        public T Throw<T>()
        {
            Traceback.Instance.ThrowException(this);
            return default;
        }

        public void Throw()
        {
            Traceback.Instance.ThrowException(this);
        }
    }
}
using System.Diagnostics;

namespace BingedIt.Commands
{
    public sealed class NavigateToLinkCommand : BaseCommand
    {
        private NavigateToLinkCommand() { }
        private static NavigateToLinkCommand _sharedInstance = new();
        public static NavigateToLinkCommand Instance => _sharedInstance;

        public override bool CanExecute(object? parameter) => parameter is string str && !string.IsNullOrWhiteSpace(str);

        public override void Execute(object? parameter)
        {
            var startInfo = new ProcessStartInfo((string)parameter!) { UseShellExecute = true };
            Process.Start(startInfo);
        }
    }
}

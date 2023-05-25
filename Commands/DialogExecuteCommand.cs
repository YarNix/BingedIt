using System.Windows.Input;
using BingedIt.Contracts;
using ModernWpf.Controls;

namespace BingedIt.Commands
{
    public class DialogExecuteCommand : BaseCommand, IExecuteCommand
    {
        readonly ICommand _command;
        readonly IDialogService _dialogService;

        public ICommand Command => _command;

        public DialogExecuteCommand(ICommand command, IDialogService dialogService)
        {
            _command = command;
            _dialogService = dialogService;
        }

        public override bool CanExecute(object? parameter) => _command.CanExecute(parameter);

        public override async void Execute(object? parameter)
        {
            ContentDialogResult result = await _dialogService.ShowDialog();
            if (result == ContentDialogResult.Primary)
                _command.Execute(parameter);
        }
    }

}

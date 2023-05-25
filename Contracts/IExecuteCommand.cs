using System.Windows.Input;

namespace BingedIt.Contracts
{
    public interface IExecuteCommand : ICommand
    {
        public ICommand Command { get; }
    }
}

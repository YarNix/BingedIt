using System.Threading.Tasks;
using ModernWpf.Controls;

namespace BingedIt.Contracts
{
    public interface IDialogService
    {
        public Task<ContentDialogResult> ShowDialog();
    }

}

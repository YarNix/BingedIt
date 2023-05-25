using System;
using System.Threading.Tasks;
using System.Windows;
using BingedIt.Contracts;
using ModernWpf.Controls;

namespace BingedIt.Services
{
    public class ConfirmDialogService : IDialogService
    {
        readonly ContentDialog _dialog;
        readonly Func<object>? _getCotent;

        public ConfirmDialogService(object? title = null, string? confirmText = null, string? declineText = null, Func<object>? getContent = null, object? templateKey = null)
        {
            _dialog = new ContentDialog()
            {
                Title = title,
                PrimaryButtonText = confirmText ?? string.Empty,
                CloseButtonText = declineText ?? string.Empty,
                ContentTemplate = templateKey is null ? null : Application.Current.Resources[templateKey] as DataTemplate,
            };
            _getCotent = getContent;
        }

        public Task<ContentDialogResult> ShowDialog()
        {
            _dialog.Content = _getCotent?.Invoke();
            return _dialog.ShowAsync();
        }
    }
}

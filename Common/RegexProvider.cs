using System.Text.RegularExpressions;

namespace BingedIt.Common
{
    static partial class RegexProvider
    {
        // TODO: Decide to remove
        // Originally this was planned to be a input validation for MediaViewModel.Link, to ensure it's a legit link
        // But the support of loading local file breaks this. Futhermore, if the link was an invalid, new Uri() should stop it or it wouldn't load Uri
        // Format: (https://) (www.) (anysite) (.com.vn) (/path/to/selected_image) (.) (img)
        [GeneratedRegex(@"(https?:\/\/)?([a-zA-Z0-9]+\.)?([a-zA-Z0-9]+)(\.[a-zA-Z]+)+(\/.+)+\.([a-zA-Z])+")]
        private static partial Regex FileUrlRegex();

        public static Regex FileUrl => FileUrlRegex();

    }
}

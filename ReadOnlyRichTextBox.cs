using System.Runtime.InteropServices;

namespace KeepYourFocus
{
    /// <summary>
    /// A read-only RichTextBox that hides the caret (text cursor) by calling
    /// the Win32 HideCaret API after every window message, ensuring the caret
    /// never becomes visible in display-only text fields.
    /// </summary>
    public partial class ReadOnlyRichTextBox : RichTextBox
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        public ReadOnlyRichTextBox()
        {
            this.ReadOnly = true;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            HideCaret(this.Handle);
        }
    }
}

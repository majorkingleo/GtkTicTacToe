using Gtk;

namespace GtkTicTacToe
{
    internal class XXOButton : Gtk.ToggleButton
    {
        public enum XXOState
        {
            BLANK,
            X,
            O
        };

        XXOState state = XXOState.BLANK;

        public XXOButton() : base()
        {
            Label = "X";
            Expand = true;
        }
    }
}

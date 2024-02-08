using Gtk;
using GtkTicTacToe;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace TicTacToe
{

    internal class TicTacToe : Window
    {
        private Grid grid;

        public TicTacToe() 
            : base( "XXO" )
        {
            DeleteEvent += new DeleteEventHandler(OnDelete);

            Frame f = new Frame("XXO");
            f.BorderWidth = 2;
            Add(f);
            
            grid = new Grid();
            grid.Expand = true;
            grid.ColumnHomogeneous = true;
            grid.RowHomogeneous = true;
            grid.BorderWidth = 5;

            for (uint x = 0; x < 3; x++)
            {
                for (uint y = 0; y < 3; y++)
                {
                    create_button(x, y);
                }
            }

            f.Add(grid);
            grid.Show();
            f.Show();
            
        }

        private void create_button( uint x, uint y )
        {
            Button btn = new XXOButton();
            grid.Attach(btn, (int)x, (int)y, 1, 1);
            btn.Show();
        }

        void OnDelete(object obj, DeleteEventArgs args)
        {
            Application.Quit();
        }

        static void Main(string[] args)
        {
            Application.Init();

            TicTacToe xxo = new TicTacToe();
            xxo.DefaultWidth = 400;
            xxo.DefaultHeight = 400;
            xxo.ShowAll();
            
            Application.Run();
        }
    }

} // namespace TicTacToe
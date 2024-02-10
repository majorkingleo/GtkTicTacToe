using Gtk;
using GtkTicTacToe;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Collections.Generic;

using BUTTON_ROW = System.Collections.Generic.List<GtkTicTacToe.XYButton>;
using BUTTON_ROWS_AND_COLS = System.Collections.Generic.List<System.Collections.Generic.List<GtkTicTacToe.XYButton>>;
using SCORE = System.Collections.Generic.Dictionary<uint, System.Collections.Generic.List<GtkTicTacToe.XYButton>>;
using System.Linq;
using static TicTacToe.TicTacToe;
using System.Security.Cryptography;

namespace TicTacToe
{

    internal class TicTacToe : Window, UserPlayedInterface
    {
        enum Symbol
        {
            OWN = 0,
            OTHER,
            WINNER
        }

        const uint Symbol_LAST = (uint)Symbol.WINNER;

        internal class Game
        {
            public XYButton.XXOState[] symbols = new XYButton.XXOState[Symbol_LAST + 1];
            public XYButton.XXOState who_starts;
            
    		public Game(XYButton.XXOState symbol_own = XYButton.XXOState.O)
            {
                reset(symbol_own);
            }

            public void reset(XYButton.XXOState symbol_own)
            {
                if (symbols[(uint)Symbol.OWN] != symbol_own)
                {
                    who_starts = swapSymbol(who_starts);
                }

                symbols[(uint)Symbol.OWN] = symbol_own;

                if (symbols[(uint)Symbol.OWN] == XYButton.XXOState.X)
                {
                    symbols[(uint)Symbol.OTHER] = XYButton.XXOState.O;
                }
                else
                {
                    symbols[(uint)Symbol.OTHER] = XYButton.XXOState.X;
                }

                symbols[(uint)Symbol.WINNER] = XYButton.XXOState.BLANK;
            }

            void setWhoStarts(XYButton.XXOState symbol)
            {
                who_starts = symbol;
            }

            XYButton.XXOState getComputerSymbol() {
			    return symbols[((uint)Symbol.OWN)];
		    }

            XYButton.XXOState getUserSymbol() {
			    return symbols[(uint)Symbol.OTHER];
		    }

            XYButton.XXOState swapSymbol(XYButton.XXOState symbol)
            {
                switch (symbol)
                {
                    case XYButton.XXOState.X: return XYButton.XXOState.O;
                    case XYButton.XXOState.O: return XYButton.XXOState.X;
                    default:
                        return XYButton.XXOState.X;
                }
            }
	    } // class Game


        private Grid grid;
        private BUTTON_ROWS_AND_COLS buttons = new BUTTON_ROWS_AND_COLS();
        const uint SIZE = 3;
        Game game = new Game();
        BUTTON_ROWS_AND_COLS all_button_combinations;
        BUTTON_ROW all_buttons_linear = new BUTTON_ROW();
        uint turn = 0;
        Statusbar statusBar;

        public TicTacToe() 
            : base( "XXO" )              
        {
            /*
            MenuBar mb = new MenuBar();
            Menu menugame = new Menu();

            MenuItem m_game = new MenuItem("Game");
            MenuItem new_game = new MenuItem("new Game");
            new_game.Activated += delegate { newGame(); };

            m_game.Submenu = new_game;
            

            menugame.Append(m_game);
            mb.Append(menugame);
            */
            MenuBar mb = new MenuBar();

            Menu menu_game = new Menu();
            MenuItem m_game = new MenuItem("Game");
            m_game.Submenu = menu_game;

            MenuItem m_new_game = new MenuItem("_new Game");
            m_new_game.Activated += delegate { newGame(); };
            menu_game.Append(m_new_game);

            MenuItem m_quit_game = new MenuItem("_quit Game");
            m_quit_game.Activated += delegate { Application.Quit(); };
            menu_game.Append(m_quit_game);

            mb.Append(m_game);


            Menu menu_options = new Menu();
            MenuItem m_options = new MenuItem("Options");
            m_options.Submenu = menu_options;


            RadioMenuItem[] group = null;

            RadioMenuItem m_user_has_x = new RadioMenuItem(group, "I take X");
            m_user_has_x.Activated += delegate { userTakesX(); };
            menu_options.Append(m_user_has_x);
            group = m_user_has_x.Group;

            MenuItem m_user_has_o = new RadioMenuItem(group, "I take O");
            m_user_has_o.Activated += delegate { userTakesO(); };
            menu_options.Append(m_user_has_o);

            mb.Append(m_options);

            DeleteEvent += new DeleteEventHandler(OnDelete);

            VBox f = new VBox(false,2);            
            f.PackStart(mb, false, false, 0);


            grid = new Grid();
            grid.Expand = true;

            grid.ColumnHomogeneous = true;
            grid.RowHomogeneous = true;
            grid.BorderWidth = 5;            

            for (uint row = 0; row < SIZE; row++)
            {
                buttons.Add(new BUTTON_ROW());

                for (uint col = 0; col < SIZE; col++)
                {                    
                    XYButton button = create_button(row, col);
                    buttons[(int)row].Add(button);
                    all_buttons_linear.Add(button);
                }
            }

            all_button_combinations = getAllCombinationOfRows();

            f.PackStart(grid, true, true, 5);            
            Add(f);
            grid.Show();
            f.Show();

            statusBar = new Statusbar();
            statusBar.Show();
            statusBar.Expand = false;
            statusBar.Vexpand = false;
            statusBar.Hexpand = true;
            statusBar.Valign = Align.End;
            f.PackStart(statusBar, false, false, 0);

            createStatusMessage();

        }

        void userTakesX()
        {            
            newGame(XYButton.XXOState.O);
        }

        void userTakesO()
        {
            newGame(XYButton.XXOState.X);
        }

        private XYButton create_button( uint x, uint y )
        {
            XYButton btn = new XYButton(this);
            grid.Attach(btn, (int)x, (int)y, 1, 1);
            btn.Show();
            return btn;
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

        SCORE findBestScore()
        {
            SCORE my_score = find2InLine(game.symbols[(uint)Symbol.OWN]);
            SCORE other_score = find2InLine(game.symbols[(uint)Symbol.OTHER]);

            SCORE best_score = my_score;

            if (my_score.Count == 0 && other_score.Count > 0)
            {
                best_score = other_score;
            }

            if (best_score.Count == 0)
            {
                // game is already over
                return best_score;
            }

            if (best_score.Last().Key < my_score.Last().Key)
            {
                best_score = my_score;
            }

            if (best_score.Last().Key < other_score.Last().Key)
            {
                best_score = other_score;
            }

            return best_score;
        }

        void UserPlayedInterface.userPlayed()
        {
            turn++;

            SCORE best_score = findBestScore();

            if (best_score.Count == 0)
            {
                playRandomOf(all_buttons_linear);
                createStatusMessage();
                return;
            }

            if (best_score.Last().Key == SIZE)
            {
                game.symbols[(uint)Symbol.WINNER] = best_score.Last().Value.First().getState();
            }
            else
            {
                playRandomOf(best_score.Last().Value);
                best_score = findBestScore();

                if (best_score.Count == 0)
                {
                    // game is already over
                    return;
                }

                if (best_score.Last().Key == SIZE)
                {
                    game.symbols[(uint)Symbol.WINNER] = best_score.Last().Value.First().getState();
                }
            }

            createStatusMessage();
        }

        BUTTON_ROWS_AND_COLS getAllCombinationOfRows()
        {
            BUTTON_ROWS_AND_COLS all_rows = new BUTTON_ROWS_AND_COLS();

            // vertically
            foreach ( BUTTON_ROW row in buttons )
            {
                all_rows.Add(row);
            }

            // horizontal
            for (int col = 0; col < SIZE; col++)
            {
                BUTTON_ROW button_row = new BUTTON_ROW();
                for (int row = 0; row < SIZE; row++)
                {
                    button_row.Add(buttons[row][col]);
                }
                all_rows.Add(button_row);
            }

            // cross
            {
                BUTTON_ROW button_row = new BUTTON_ROW();

                for (int col = 0, row = 0; col < SIZE && row < SIZE; col++, row++)
                {
                    button_row.Add(buttons[row][col]);
                }
                all_rows.Add(button_row);
            }

            {
                BUTTON_ROW button_row = new BUTTON_ROW();
                for (int col = (int)SIZE - 1, row = 0; col >= 0 && row < (int)SIZE; col--, row++)
                {
                    button_row.Add(buttons[row][col]);
                }
                all_rows.Add(button_row);
            }

            return all_rows;
        }

        SCORE find2InLine(XYButton.XXOState symbol)
        {
            SCORE score = new SCORE();

            foreach ( BUTTON_ROW row in all_button_combinations )
            {
                SCORE score4row = find2InLine(row, symbol);
                //score.insert(score4row.begin(), score4row.end());
                foreach( uint key in  score4row.Keys )
                {
                    score.TryAdd(key, score4row[key]);
                }
            }

            return score;
        }

        SCORE find2InLine(BUTTON_ROW row, XYButton.XXOState symbol)
        {
            HashSet<XYButton.XXOState> symbols = new HashSet<XYButton.XXOState>();
            uint symbol_count = 0;

            foreach ( XYButton button in row)
            {
                XYButton.XXOState button_symbol = button.getState();

                if (button_symbol != XYButton.XXOState.BLANK)
                {
                    symbols.Add(button_symbol);

                    if (symbol == button_symbol)
                    {
                        symbol_count++;
                    }
                }
            }

            string buf = "";

            foreach (XYButton button in row)
            {
                switch (button.getState())
                {
                    case XYButton.XXOState.BLANK: buf += " "; break;
                    case XYButton.XXOState.X: buf += "X"; break;
                    case XYButton.XXOState.O: buf += "O"; break;
                }
            }

            Console.WriteLine("Symbol count: {0} {1}", symbol_count, buf);

            if (symbols.Count != 1)
            {
                return new SCORE();
            }

            SCORE ret = new SCORE();
            ret.Add(symbol_count, row);

            return ret;
        }


        uint countBlankButtons()
        {
            uint count = 0;
            
            foreach( XYButton button in all_buttons_linear )
            {
                if( button.isBlank() )
                {
                    count++;
                }
            }

            return count;
        }

        void endGame()
        {            
            foreach (XYButton button in all_buttons_linear)
            {
                button.Sensitive = false;
            }		
        }

        void createStatusMessage()
        {
            if (game.symbols[(uint)Symbol.WINNER] == XYButton.XXOState.BLANK)
            {

                if (countBlankButtons() == 0)
                {
                    uint id = statusBar.GetContextId("game ends");
                    statusBar.Push(id,"Tie! Let's try it again!");
                    endGame();
                    return;
                }

                if (turn == 0)
                {
                    uint id = statusBar.GetContextId("your turn");
                    statusBar.Push(id,"You start, it's your turn.");
                }
                else if (turn % 2 == 0)
                {
                    uint id = statusBar.GetContextId("your turn2");
                    statusBar.Push(id, "It's your turn.");
                }

                return;
            }

            if (game.symbols[(uint)Symbol.WINNER] == game.symbols[(uint)Symbol.OWN])
            {
                uint id = statusBar.GetContextId("Looser");
                statusBar.Push(id,"I'm the winner!!");
            }
            else
            {
                uint id = statusBar.GetContextId("Winner");
                statusBar.Push(id,"Congratulations, you win!");
            }

            endGame();
        }

        void playRandomOf(BUTTON_ROW buttons )
        {
            BUTTON_ROW empty_buttons = new BUTTON_ROW();

            foreach ( XYButton button in buttons)
            {
                if (button.isBlank())
                {
                    empty_buttons.Add(button);
                }
            }

            if (empty_buttons.Count == 0)
            {
                return;
            }

            Shuffle.doShuffle(empty_buttons);

            empty_buttons.First().setState(game.symbols[(uint)Symbol.OWN]);

            turn++;
        }

        void newGame(XYButton.XXOState symbol_own)
        {            
            foreach (XYButton button in all_buttons_linear)
            {
                button.reset();
                button.setComputerSymbol(symbol_own);
            }

            turn = 0;
            game.reset(symbol_own);
            Console.WriteLine("game.who_starts: {0} symbol_own: {1}", game.who_starts, symbol_own);

            if (game.who_starts == symbol_own)
            {
                ((UserPlayedInterface)this).userPlayed();                
            }

            createStatusMessage();
        }

        void newGame()
        {
            newGame(game.symbols[(uint)Symbol.OWN]);
        }
    }

} // namespace TicTacToe
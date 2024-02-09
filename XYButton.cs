using Gtk;
using Pango;
using System;

namespace GtkTicTacToe
{
    class XYButton : Gtk.ToggleButton
    {
        public enum XXOState
        {
            BLANK,
            X,
            O
        };

        XXOState state = XXOState.BLANK;
        XXOState userSymbol = XXOState.X;
        UserPlayedInterface parent;
        bool inResettingState = false;

        public XYButton( UserPlayedInterface parent_) : base()
        {
            Label = "";
            Expand = true;
            Clicked += delegate { userClicked(); };
            parent = parent_;
        }

        void clear()
        {
            Label = "";
        }

        public void setState(XXOState state_)
        {
            if (state == state_)
            {
                return;
            }

            state = state_;

            clear();

            String icon_name = null;
            String label_name = null;

            switch (state)
            {
                case XXOState.BLANK:                    
                    return;

                case XXOState.X: 
                    icon_name = "Red_X_Freehand.svg"; 
                    label_name = "X";
                    break;

                case XXOState.O: 
                    icon_name = "moon-hand-drawn-circle-svgrepo-com.svg"; 
                    label_name = "O";
                    break;
            }
            Sensitive = false;
            Label = label_name;
            //layout()->addWidget(new QSvgWidget(icon_name, this));
        }

        public XXOState getState() { 
            return state; 
        }

        public bool isBlank() {
		    return state == XXOState.BLANK;
	    }

        public void reset()
        {
            setState(XXOState.BLANK);
            Sensitive = true;
            inResettingState = true;
            Active = false;
            inResettingState = false;
        }

        public void setUserSymbol(XXOState state)
        {
            userSymbol = state;
        }

        public void setComputerSymbol(XXOState state)
        {
            switch (state)
            {
                case XXOState.X: setUserSymbol(XXOState.O); break;
                case XXOState.O: setUserSymbol(XXOState.X); break;
            }
        }
        
        void userClicked()
        {
            if( inResettingState )
            {
                return;
            }

            if (state != XXOState.BLANK)
            {
                // std::cout << "State is not blank its: " << static_cast<int>(state) << std::endl;
                return;
            }

            setState(userSymbol);

            parent.userPlayed();
        }

    }
}

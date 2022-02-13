#nullable enable
using System;
using static System.Console;

namespace Bme121
{
    record Player( string Colour, string Symbol, string Name );
    
    static partial class Program
    {
        // Display common text for the top of the screen.
        static void Welcome()
        {
			string[ , ] game = NewBoard( rows: 8, cols: 8 );
            Console.Clear( );
            WriteLine( );
            WriteLine( " Welcome to Othello!" );
            WriteLine( );
            DisplayBoard( game );
            WriteLine( );
        }
        
        // Collect a player name or default to form the player record.
        static Player NewPlayer( string colour, string symbol, string defaultName )
        {
            return new Player(colour, symbol, defaultName);
        }
        
        // Determine which player goes first or default.
        static int GetFirstTurn( Player[ ] players, int defaultFirst )
        {
            return 0;
        }
        
        // Get a board size (between 4 and 26 and even) or default, for one direction.
        static int GetBoardSize( string direction, int defaultSize )
        {
            return 8;
        }
        
        // Get a move from a player.
        static string GetMove( Player player )
        {
			Write($"Enter your move, {player.Name} ({player.Symbol}/{player.Colour}): ");
			string move = ReadLine();
			return move;
        }
        
        
        //Declaring methods to check the symbol of surrounding boxes
        //If there is no box left, right, up, down, or diagonal from move, return "N", otherwise, return symbol of box.
        static string L_Symbol(string[ ,] board, int rowIndex, int colIndex) //return symbol of box to the left
        {
			
			if (colIndex == 0)
				return ("N");
			else
				return (board[rowIndex, colIndex -1]);
		}
			
		static string R_Symbol(string[ ,] board, int rowIndex, int colIndex) //return symbol of box to the right
		{
			if (colIndex == board.GetLength(0)-1)
				return ("N");
			else
				return(board[rowIndex, colIndex+1]);
		}
        
        static string U_Symbol(string[ ,] board, int rowIndex, int colIndex) // return symbol of box above 
        {
			if (rowIndex == 0)
				return ("N");
			else
				return(board[rowIndex-1, colIndex]);
		}
        
        static string D_Symbol(string[ ,] board, int rowIndex, int colIndex) // return symbol of box below
        {
			if (rowIndex == board.GetLength(0)-1)
				return ("N");
			else
				return(board[rowIndex+1, colIndex]);
		}
	      
        static string UL_Symbol(string[ ,] board, int rowIndex, int colIndex) // return symbol of box in upper-left diagonal
        {
			if (rowIndex == 0 || colIndex ==0)
				return("N");
			else
				return(board[rowIndex -1, colIndex -1]);
		}
        
        static string UR_Symbol(string[ ,] board, int rowIndex, int colIndex) //return symbol of box in upper-right diagonal
        {
			if (rowIndex == 0 || colIndex == board.GetLength(0)-1)
				return("N");
			else
				return(board[rowIndex-1, colIndex+1]);
		}
        
        static string DL_Symbol(string[ ,] board, int rowIndex, int colIndex) //return symbol of box in lower-left diagonal
        {
			if (rowIndex == board.GetLength(0)-1 || colIndex == 0)
				return("N");
			else
				return(board[rowIndex+1, colIndex-1]);
		}
        
        static string DR_Symbol(string[ ,] board, int rowIndex, int colIndex) //return symbol of box in lower-right diagonal
		{
			if (rowIndex == board.GetLength(0)-1 || colIndex == board.GetLength(0)-1)
				return("N");
			else
				return(board[rowIndex +1, colIndex+1]);
		}


		//Method for determining the other player's symbol
		static string otherPlayerSymbol(Player player)
		{
			if (player.Symbol == "X") return "O";
			else return ("X");
		}

        
        // Try to make a move. Return true if it worked.
        static bool TryMove( string[ , ] board, Player player, string move )
        {
			if (move == "skip")
				return true;
				
			if (move.Length != 2)
				return false;
			
			string rowLetter = move[0].ToString();
			string colLetter = move[1].ToString();	
			int rowIndex = IndexAtLetter(rowLetter);
			int colIndex = IndexAtLetter(colLetter);

			
			//if move is not on the board, it is invalid
			if ((rowIndex < 0 || rowIndex > board.GetLength(0)-1) ||
				(rowIndex < 0 || colIndex > board.GetLength(0)-1))
				return false;
			
			//if move has already been taken by another piece, it is invalid
			if (board[rowIndex,colIndex] != " ")
				return false;		
                        
			bool canFlipL = false;
			bool canFlipR = false;
			bool canFlipU = false;
			bool canFlipD = false;
			bool canFlipUL = false;
			bool canFlipUR = false;
			bool canFlipDL = false;
			bool canFlipDR = false;
			
			//Checking each direction
			if (L_Symbol(board, rowIndex, colIndex) == otherPlayerSymbol(player))
				canFlipL = TryDirection(board, player, rowIndex, 0, colIndex, -1);
				
			if (R_Symbol(board, rowIndex, colIndex) == otherPlayerSymbol(player))
				canFlipR = TryDirection(board, player, rowIndex, 0, colIndex, 1);
				
			if (U_Symbol(board, rowIndex, colIndex) == otherPlayerSymbol(player))
				canFlipU = TryDirection(board, player, rowIndex, -1, colIndex, 0);
			
			if (D_Symbol(board, rowIndex, colIndex) == otherPlayerSymbol(player))
				canFlipD = TryDirection(board, player, rowIndex, 1, colIndex, 0);
			
			if (UL_Symbol(board, rowIndex, colIndex) == otherPlayerSymbol(player))
				canFlipUL = TryDirection(board, player, rowIndex, -1, colIndex, -1);
			
			if (UR_Symbol(board, rowIndex, colIndex) == otherPlayerSymbol(player))
				canFlipUR = TryDirection(board, player, rowIndex, -1, colIndex, 1);
			
			if (DL_Symbol(board, rowIndex, colIndex) == otherPlayerSymbol(player))
				canFlipDL = TryDirection(board, player, rowIndex, 1, colIndex, -1);
			
			if (DR_Symbol(board, rowIndex, colIndex) == otherPlayerSymbol(player))
				canFlipDR = TryDirection(board, player, rowIndex, 1, colIndex, 1);
			
			
			if (canFlipL == true 
				|| canFlipR == true
				|| canFlipU == true
				|| canFlipD == true
				|| canFlipUL == true
				|| canFlipUR == true
				|| canFlipDL == true
				|| canFlipDR == true)
				return true;
			
			return false;  
        }
        
        // Do the flips along a direction specified by the row and column delta for one step. 
        static bool TryDirection( string[ , ] board, Player player,
            int moveRow, int deltaRow, int moveCol, int deltaCol )
        {
			int numFlips = 0;
			int row = moveRow;
			int col = moveCol;
			bool counting = true;

			
			while (counting == true)
			{				
				row = row + deltaRow;
				col = col + deltaCol;
				
				if (row < 0 || col < 0 || row > board.GetLength(0) || col > board.GetLength(0))
					counting = false;
				
				if (board[row,col] == otherPlayerSymbol(player))
				{
					numFlips ++;
				}
					
				else
					counting = false;	
			}
			
			if (row == -1 || col == -1 || row == board.GetLength(0) || col == board.GetLength(0))
				return false;
			
			if (board[row,col] == player.Symbol && numFlips > 0)
			{
				bool flipping = true;
				int flipRow = moveRow;
				int flipCol = moveCol;
				int numFlipped = 0;
				
				while (numFlipped < numFlips+1)
				{
					flipRow = flipRow + deltaRow;
					flipCol = flipCol + deltaCol;
					Flip(board, player, flipRow, flipCol);
					numFlipped ++;
				}

				return true;
			}
			return false;	
		}
					
        
        
       static void Flip(string [ , ] board, Player player, int row, int col)
		{
			board[row,col] = player.Symbol;
		}
        
        // Count the discs to find the score for a player.
        static int GetScore( string[ , ] board, Player player )
        {
			int scoreCount = 0;
						
            for (int c = 0; c < board.GetLength(0)-1; c ++)
            {
				for (int r = 0; r < board.GetLength(0)-1; r++)
				{
					if (board[r,c] == player.Symbol)
						scoreCount ++;				
				}
					
			}
			return scoreCount;
        }
        
        // Display a line of scores for all players.
        static void DisplayScores( string[ , ] board, Player[ ] players )
        {
			WriteLine("Current Score:");
			WriteLine($"{players[0].Name} (X/Black): {GetScore(board, players[0])}");
			WriteLine($"{players[1].Name} (O/White): {GetScore(board, players[1])}");
			WriteLine();
        }
        
        // Display winner(s) and categorize their win over the defeated player(s).
        // This is displayed when "quit" is entered.
        static void DisplayWinners( string[ , ] board, Player[ ] players )
        {
			int scoreX = GetScore(board, players[0]);
			int scoreO = GetScore(board, players[1]);
			WriteLine();
			WriteLine("GAME OVER");
			if (scoreX > scoreO)
				WriteLine($"The winner is {players[0].Name} ({players[0].Symbol}/{players[0].Colour}) who wins by {scoreX-scoreO} points.");
			if (scoreX < scoreO)
				WriteLine($"The winner is {players[1].Name} ({players[1].Symbol}/{players[1].Colour}) who wins by {scoreO-scoreX} points.");
			if (GetScore(board,players[1]) == GetScore(board, players[0]))
				WriteLine("There is a tie.");
			
			WriteLine("Final Score:");
			WriteLine($"{players[0].Name} (X/Black): {GetScore(board, players[0])}");
			WriteLine($"{players[1].Name} (O/White): {GetScore(board, players[1])}");
			WriteLine();
        }
        
        static void Main( )
        {           
            Welcome();
            
            //Collect the players' names
            Write("Enter player name for X/Black: ");
            string blackName = ReadLine();
            Write("Enter player name for O/White: ");
            string whiteName = ReadLine();
            
            
            Console.Clear();
            
            Player[ ] players = new Player[ ] 
            {
                NewPlayer( colour: "Black", symbol: "X", defaultName: blackName ),
                NewPlayer( colour: "White", symbol: "O", defaultName: whiteName ),
            };
            
            int turn = GetFirstTurn( players, defaultFirst: 0 );
            int rows = GetBoardSize( direction: "rows",    defaultSize: 8 );
            int cols = GetBoardSize( direction: "columns", defaultSize: 8 );
            
            string[ , ] game = NewBoard( rows, cols );
            
            // Play the game.
            bool gameOver = false;
            while( ! gameOver )
            {
				WriteLine();
                DisplayBoard( game );
                DisplayScores( game, players );
                                
                string move = GetMove( players[ turn ] );
                if( move == "quit" ) gameOver = true;
                else
                {						
                    bool madeMove = TryMove( game, players[ turn ], move );
                    if( madeMove)
                    {	
						if (move != "skip")
						{
							string rowLetter = move[0].ToString();
							string colLetter = move[1].ToString();	
							int rowIndex = IndexAtLetter(rowLetter);
							int colIndex = IndexAtLetter(colLetter);		
							game[rowIndex, colIndex] = players[turn].Symbol;
						}
						
						Console.Clear();
						turn = (turn + 1)% players.Length;
					}
                    else 
                    {
                        Write( " Your choice didn't work!" );
                        Write( " Press <Enter> to try again." );
                        ReadLine( ); 
                        Console.Clear();
                    }
                }
            }
            
            // Show fhe final results. 
            DisplayWinners( game, players );
            WriteLine( );
        }
    }
}

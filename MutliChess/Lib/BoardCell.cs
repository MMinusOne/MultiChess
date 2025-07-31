using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiChess.Lib
{
    public class BoardCell
    {
        int _boardIndex;
        public int BoardIndex
        {
            get { return _boardIndex; } 
            set { _boardIndex = value; }
        }

        Piece _piece;
        public Piece PIECE
        {
            get { return _piece; }
            set { _piece = value; }
        }

        public BoardCell(int boardIndex, Piece newPiece)
        {
            BoardIndex = boardIndex;
        }

        public void setPiece(Piece newPiece)
        {
            PIECE = newPiece;
        }
    }
}

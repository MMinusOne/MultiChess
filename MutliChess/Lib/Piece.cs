using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiChess.Lib
{
    public class Piece
    {
        PIECE_TYPE _pieceType;
        public PIECE_TYPE PieceType
        {
            get { return _pieceType; }
            set { _pieceType = value;  }
        }

        PIECE_COLOR _pieceColor;
        public PIECE_COLOR PieceColor
        {
            get { return _pieceColor; }
            set { _pieceColor = value; }
        }

        public Piece()
        {
            this.PieceType = PIECE_TYPE.NULL_PIECE;
        }
        public Piece(PIECE_TYPE pieceType, PIECE_COLOR pieceColor)
        {
            this.PieceType = pieceType;
            this.PieceColor = pieceColor;
        }
    }
}

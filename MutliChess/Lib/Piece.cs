using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiChess.Lib
{
    public class Piece: INotifyPropertyChanged
    {
        PIECE_TYPE _pieceType = PIECE_TYPE.NULL_PIECE;
        public PIECE_TYPE PieceType
        {
            get { return _pieceType; }
            set { _pieceType = value; OnPropertyChanged(nameof(PieceType)); }
        }

        PIECE_COLOR _pieceColor;

        public event PropertyChangedEventHandler? PropertyChanged;

        public PIECE_COLOR PieceColor
        {
            get { return _pieceColor; }
            set { _pieceColor = value; OnPropertyChanged(nameof(PieceColor)); }
        }

        public Piece()
        {
            _pieceType=PIECE_TYPE.NULL_PIECE;
            this.PieceType = PIECE_TYPE.NULL_PIECE;
        }
        public Piece(PIECE_TYPE pieceType, PIECE_COLOR pieceColor)
        {
            _pieceType=pieceType;
            this.PieceType = pieceType;
            this.PieceColor = pieceColor;
        }

        bool _movedOnce = false;
        public bool MovedOnce
        {
            get { return _movedOnce; }
            set { _movedOnce = value; OnPropertyChanged(nameof(MovedOnce)); }
        }
        void setMovedOnce() { MovedOnce = true; }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

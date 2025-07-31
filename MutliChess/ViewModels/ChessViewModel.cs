using MultiChess.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiChess.ViewModels
{
    public class ChessViewModel: BaseVM, INotifyPropertyChanged
    {

        public ChessViewModel()
        {
            this.render();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        int _boardSize = 8;
        public int BoardSize
        {
            get { return _boardSize; }
            set
            {
                if (_boardSize != value)
                {
                     _boardSize = value;
                    OnPropertyChanged(nameof(BoardSize));
                }
            }
        }

        int _boardFixedSize = 500;
        public int BoardFixedSize
        {
            get { return _boardFixedSize; }
            set
            {
                if (_boardFixedSize != value)
                {
                    _boardFixedSize = value;
                    OnPropertyChanged(nameof(BoardFixedSize));
                }
            }
        }

        ObservableCollection<ObservableCollection<BoardCell>> _board = new ObservableCollection<ObservableCollection<BoardCell>>();

        public ObservableCollection<ObservableCollection<BoardCell>> Board
        {
            get { return _board; }
            set
            {
                if (_board != value)
                {
                    _board = value;
                    OnPropertyChanged(nameof(Board));
                }
            }
        }

        private static ChessViewModel _instance;

        public static ChessViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ChessViewModel();
                }
                return _instance;
            }
        }

        public void render()
        {
            for(int x = 0; x < this.BoardSize; x++)
            {
                ObservableCollection<BoardCell> row = new ObservableCollection<BoardCell>();
                for(int y = 0; y < this.BoardSize; y++)
                {
                    BoardCell boardCell = new BoardCell(x * BoardSize + y, new Piece());
                    row.Add(boardCell);
                }
                Board.Add(row);
            }

            this.InitilizePieces();
        }

       void InitilizePieces()
        {
            var firstRow = this.Board.First();

            firstRow[0].setPiece(new Piece(PIECE_TYPE.ROOK, PIECE_COLOR.WHITE));
            firstRow[1].setPiece(new Piece(PIECE_TYPE.KNIGHT, PIECE_COLOR.WHITE));
            firstRow[2].setPiece(new Piece(PIECE_TYPE.BISHOP, PIECE_COLOR.WHITE));
            firstRow[3].setPiece(new Piece(PIECE_TYPE.KING, PIECE_COLOR.WHITE));
            firstRow[4].setPiece(new Piece(PIECE_TYPE.QUEEN, PIECE_COLOR.WHITE));
            firstRow[5].setPiece(new Piece(PIECE_TYPE.BISHOP, PIECE_COLOR.WHITE));
            firstRow[6].setPiece(new Piece(PIECE_TYPE.KNIGHT, PIECE_COLOR.WHITE));
            firstRow[7].setPiece(new Piece(PIECE_TYPE.ROOK, PIECE_COLOR.WHITE));
            for(int i = 0; i < this.Board[1].Count; i++)
            {
                this.Board[1][i].setPiece(new Piece(PIECE_TYPE.PAWN, PIECE_COLOR.WHITE));
            }

            var lastRow = this.Board.Last();

            lastRow[0].setPiece(new Piece(PIECE_TYPE.ROOK, PIECE_COLOR.BLACK));
            lastRow[1].setPiece(new Piece(PIECE_TYPE.KNIGHT, PIECE_COLOR.BLACK));
            lastRow[2].setPiece(new Piece(PIECE_TYPE.BISHOP, PIECE_COLOR.BLACK));
            lastRow[3].setPiece(new Piece(PIECE_TYPE.KING, PIECE_COLOR.BLACK));
            lastRow[4].setPiece(new Piece(PIECE_TYPE.QUEEN, PIECE_COLOR.BLACK));
            lastRow[5].setPiece(new Piece(PIECE_TYPE.BISHOP, PIECE_COLOR.BLACK));
            lastRow[6].setPiece(new Piece(PIECE_TYPE.KNIGHT, PIECE_COLOR.BLACK));
            lastRow[7].setPiece(new Piece(PIECE_TYPE.ROOK, PIECE_COLOR.BLACK));
            for (int i = 0; i < this.Board[this.Board.Count-2].Count; i++)
            {
                this.Board[this.Board.Count-2][i].setPiece(new Piece(PIECE_TYPE.PAWN, PIECE_COLOR.BLACK));
            }

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using MultiChess.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiChess.Lib
{
    public class BoardCell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        int _boardIndex;
        public int BoardIndex
        {
            get { return _boardIndex; }
            set { _boardIndex = value; OnPropertyChanged(nameof(BoardIndex)); }
        }

        Piece _piece;
        public Piece PIECE
        {
            get { return _piece; }
            set { _piece = value; OnPropertyChanged(nameof(PIECE)); }
        }

        bool _isCellSelected;
        public bool IsCellSelected
        {
            get { return _isCellSelected; }
            set
            {
                _isCellSelected = value;
                OnPropertyChanged(nameof(IsCellSelected));
            }
        }

        ObservableCollection<ObservableCollection<BoardCell>> board = ChessViewModel.Instance.Board;

        public BoardCell(int boardIndex, Piece newPiece)
        {
            this.setPiece(newPiece);
            IsCellSelected = false;
            BoardIndex = boardIndex;
        }

        public BoardCell(int boardIndex, Piece newPiece, ObservableCollection<ObservableCollection<BoardCell>> board)
        {
            this.setPiece(newPiece);
            IsCellSelected = false;
            BoardIndex = boardIndex;
            this.board = board;
        }

        public void setPiece(Piece newPiece)
        {
            _piece = newPiece;
            PIECE = newPiece;
        }

        public void setCellSelected(bool state)
        {
            IsCellSelected = state;
        }

        public int Row;
        public int Column;

        public void SetRow(int row)
        {
            Row = row;
        }

        public void SetColumn(int column)
        {
            Column = column;
        }

        public void RenderOnBoard()
        {

            if (ChessViewModel.Instance.InitialRender) return;

            this.Column = BoardIndex% this.board.Count;
            this.Row = BoardIndex/ this.board.Count;

            this.board[this.Row][this.Column] = null;
            this.board[this.Row][this.Column] = this;
        }

        bool _isAvailable = false;
        public bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                _isAvailable = value;
                OnPropertyChanged(nameof(IsAvailable));
            }
        }
        public void setCellAvaiable(bool state)
        {
            IsAvailable = state;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {

            if (!ChessViewModel.Instance.InitialRender)
            {
                this.RenderOnBoard();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

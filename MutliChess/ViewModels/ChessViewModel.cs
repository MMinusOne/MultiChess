using MultiChess.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MultiChess.ViewModels
{
    public class ChessViewModel : BaseVM, INotifyPropertyChanged
    {
        bool _initialRender = true;
        public bool InitialRender
        {
            get { return _initialRender; }
            set { _initialRender = value; OnPropertyChanged(nameof(InitialRender)); }
        }

        public ChessViewModel()
        {
            _instance = this;
            this.render();
        }

        ICommand _selectCellCommand;
        public ICommand SelectCellCommand
        {
            get
            {
                if (_selectCellCommand == null)
                {
                    _selectCellCommand = new RelayCommand(SelectCellExecute, (object parameter) => { return true; });
                }
                return _selectCellCommand;
            }
        }

        ObservableCollection<BoardCell> _selectedCells = new ObservableCollection<BoardCell>();
        public ObservableCollection<BoardCell> SelectedCells
        {
            get { return _selectedCells; }
            set { _selectedCells = value; OnPropertyChanged(nameof(SelectedCells)); }
        }


        PIECE_COLOR turn = PIECE_COLOR.WHITE;
        void ChangeTurn()
        {
            turn = turn == PIECE_COLOR.WHITE ? PIECE_COLOR.BLACK : PIECE_COLOR.WHITE;
        }

        ObservableCollection<BoardCell> availableSquares = new ObservableCollection<BoardCell>();
        public void SelectCellExecute(object obj)
        {
            if (obj is not BoardCell) return;
            var cell = (BoardCell)obj;

            switch (SelectedCells.Count)
            {
                case 0:
                    if (cell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE || cell.PIECE.PieceColor != turn) return;
                    cell.setCellSelected(true);
                    SelectedCells.Add(cell);

                    var availableSquaresTemp = cell.PIECE.GetAvailableSquares(cell, null, true);
                    for (int i = 0; i < availableSquaresTemp.Count; i++)
                    {
                        availableSquares.Add(availableSquaresTemp[i]);
                    }
                    ;

                    for (int i = 0; i <availableSquares.Count; i++)
                    {
                        if (availableSquares[i] == null) continue;
                        availableSquares[i].setCellAvaiable(true);
                    }

                    break;
                case 1:
                    cell.setCellSelected(true);
                    SelectedCells.Add(cell);

                    var toCell = SelectedCells[1];
                    var fromCell = SelectedCells[0];
                    bool canMove = false;

                    for (int i = 0; i < availableSquares.Count; i++)
                    {
                        var availableSquare = availableSquares[i];
                        if (availableSquare.BoardIndex == toCell.BoardIndex)
                        {
                            canMove = true;
                        }
                    }


                    if (canMove)
                    {
                        var piece = fromCell.PIECE;
                        toCell.setPiece(piece);
                        if (fromCell.PIECE.PieceType == PIECE_TYPE.PAWN && (toCell.Row == 0 || toCell.Row == Board.Count-1))
                        {
                            toCell.setPiece(new Piece(PIECE_TYPE.QUEEN, fromCell.PIECE.PieceColor));
                        }

                        if (piece.PieceType == PIECE_TYPE.KING && Math.Abs(fromCell.Column - toCell.Column) >= 2)
                        {
                            if ((fromCell.Column - toCell.Column) > 0)
                            {
                                var rookCell = GetBoardAt(fromCell.Row, 0);
                                if (rookCell.PIECE.PieceType == PIECE_TYPE.ROOK)
                                {
                                    rookCell.setPiece(new Piece());
                                    var nextToKing = GetBoardAt(fromCell.Row, fromCell.Column-1);
                                    nextToKing.setPiece(new Piece(PIECE_TYPE.ROOK, piece.PieceColor));
                                }
                            }

                            if ((fromCell.Column - toCell.Column) < 0)
                            {
                                var rookCell = GetBoardAt(fromCell.Row, Board.Count-1);
                                if (rookCell.PIECE.PieceType == PIECE_TYPE.ROOK)
                                {
                                    rookCell.setPiece(new Piece());
                                    var nextToKing = GetBoardAt(fromCell.Row, fromCell.Column+1);
                                    nextToKing.setPiece(new Piece(PIECE_TYPE.ROOK, piece.PieceColor));
                                }
                            }
                        }
                        ;

                        fromCell.setPiece(new Piece());
                        piece.MovedOnce = true;

                        toCell.setCellSelected(false);
                        fromCell.setCellSelected(false);


                        SelectedCells.Clear();
                        for (int i = 0; i < availableSquares.Count; i++)
                        {
                            availableSquares[i].setCellAvaiable(false);
                        }
                        availableSquares.Clear();
                        this.ChangeTurn();

                    }
                    break;

            }
        }


        public void CheckmateHappened(PIECE_COLOR color)
        {
            Console.WriteLine("hi");
        }

        public ObservableCollection<ObservableCollection<BoardCell>> ColorBoardPerspective(PIECE_COLOR color)
        {
            if (color == PIECE_COLOR.WHITE)
            {
                return Board;
            }
            else
            {
                ObservableCollection<ObservableCollection<BoardCell>> reverseBoard = new ObservableCollection<ObservableCollection<BoardCell>>();
                for (int i = 0; i < Board.Count; i++)
                {
                    ObservableCollection<BoardCell> row = new ObservableCollection<BoardCell>();

                    for (int j = 0; j < Board[Board.Count-i-1].Count; j++)
                    {
                        row.Add(Board[i][j]);
                    }

                    reverseBoard.Add(row);
                }
                return reverseBoard;
            }
        }

        public int OffsetPerspective(int n)
        {
            if (SelectedCells[0].PIECE.PieceColor == PIECE_COLOR.WHITE)
            {
                return SelectedCells[0].Row+n;
            }
            else
            {
                return SelectedCells[0].Row-n;
            }
        }

        public int OffsetPerspective(int n, BoardCell relativeCell)
        {

            if (relativeCell.PIECE.PieceColor == PIECE_COLOR.WHITE)
            {
                return relativeCell.Row+n;
            }
            else
            {
                return relativeCell.Row-n;
            }

        }

        public BoardCell GetBoardAt(int x, int y)
        {
            if (x >= this.BoardSize || 0 > x) return null;
            if (y >= this.BoardSize || 0 > y) return null;
            return this.Board[x][y];
        }

        public BoardCell GetBoardAt(int x, int y, ObservableCollection<ObservableCollection<BoardCell>> board)
        {
            if (x >=  board.Count || 0 > x) return null;
            if (y >= board.Count || 0 > y) return null;
            return board[x][y];
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
                _board = value;
                OnPropertyChanged(nameof(Board));
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
            this.Board = new ObservableCollection<ObservableCollection<BoardCell>>();
            for (int x = 0; x < this.BoardSize; x++)
            {
                ObservableCollection<BoardCell> row = new ObservableCollection<BoardCell>();
                for (int y = 0; y < this.BoardSize; y++)
                {
                    BoardCell boardCell = new BoardCell(x * BoardSize + y, new Piece());
                    boardCell.SetRow(x);
                    boardCell.SetColumn(y);
                    row.Add(boardCell);
                }
                Board.Add(row);
            }

            if (InitialRender)
            {
                this.InitilizePieces();
                InitialRender = false;
            }
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
            for (int i = 0; i < this.Board[1].Count; i++)
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

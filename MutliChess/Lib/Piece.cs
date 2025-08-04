using MultiChess.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MultiChess.Lib
{
    public class Piece : INotifyPropertyChanged
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

        public List<BoardCell> GetAvailableSquares(BoardCell cell)
        {
            var chessViewModel = ChessViewModel.Instance;
            var Board = chessViewModel.Board;
            var GetBoardAt = chessViewModel.GetBoardAt;
            var OffsetPerspective = chessViewModel.OffsetPerspective;
            var ColorBoardPerspective = chessViewModel.ColorBoardPerspective;


            var availableSquares = new List<BoardCell>();

            if (cell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
            {

            }
            else if (cell.PIECE.PieceType == PIECE_TYPE.PAWN)
            {
                var nextCell = GetBoardAt(OffsetPerspective(1), cell.Column);
                if (nextCell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
                {
                    availableSquares.Add(nextCell);
                }
                var nextSecondCell = GetBoardAt(OffsetPerspective(2), cell.Column);
                if (cell.PIECE.MovedOnce == false && nextSecondCell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
                {
                    availableSquares.Add(nextSecondCell);
                }
                var sideRightCell = GetBoardAt(OffsetPerspective(1), cell.Column+1);
                var sideLeftCell = GetBoardAt(OffsetPerspective(1), cell.Column-1);
                if (sideRightCell != null && sideRightCell.PIECE.PieceColor != cell.PIECE.PieceColor)
                {
                    if (sideRightCell.PIECE.PieceType != PIECE_TYPE.NULL_PIECE)
                    {
                        availableSquares.Add(sideRightCell);
                    }
                }

                if (sideLeftCell != null && sideLeftCell.PIECE.PieceColor != cell.PIECE.PieceColor)
                {
                    if (sideLeftCell.PIECE.PieceType != PIECE_TYPE.NULL_PIECE)
                    {
                        availableSquares.Add(sideLeftCell);
                    }
                }
            }
            else
                if (cell.PIECE.PieceType == PIECE_TYPE.KNIGHT)
            {
                var boardPerspective = ColorBoardPerspective(cell.PIECE.PieceColor);

                List<BoardCell> cells = new List<BoardCell>
                    {
                GetBoardAt(OffsetPerspective(2), cell.Column-1),
                GetBoardAt(OffsetPerspective(1), cell.Column-2),
                GetBoardAt(OffsetPerspective(2), cell.Column+1),
                GetBoardAt(OffsetPerspective(1), cell.Column+2),
                GetBoardAt(OffsetPerspective(-2), cell.Column-1),
                GetBoardAt(OffsetPerspective(-1), cell.Column-2),
                GetBoardAt(OffsetPerspective(-2), cell.Column+1),
                GetBoardAt(OffsetPerspective(-1), cell.Column+2),
                };

                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i] == null) continue;
                    if (cells[i].PIECE.PieceType == PIECE_TYPE.NULL_PIECE || cells[i].PIECE.PieceColor !=  cell.PIECE.PieceColor)
                    {
                        availableSquares.Add(cells[i]);
                    }
                }
            }

            else if (cell.PIECE.PieceType == PIECE_TYPE.BISHOP)
            {
                var directions = new (int dx, int dy)[] {
                (-1, 1),
                (1, 1),
                (-1, -1),
                (1, -1)
                };

                foreach (var (dx, dy) in directions)
                {
                    int newX = cell.Row;
                    int newY = cell.Column;

                    while (true)
                    {
                        newX += dx;
                        newY += dy;

                        if (newX >= 0 && newX < Board.Count && newY >= 0 && newY < Board.Count)
                        {

                            if (Board[newX][newY].PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
                            {
                                availableSquares.Add(Board[newX][newY]);
                            }
                            else if (Board[newX][newY].PIECE.PieceColor != cell.PIECE.PieceColor)
                            {
                                availableSquares.Add(Board[newX][newY]);
                                break;
                            }
                            else
                            {
                                break;
                            }

                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else if (cell.PIECE.PieceType == PIECE_TYPE.ROOK)
            {
                var directions = new (int dx, int dy)[] {
                (-1, 0),
                (1, 0),
                (0, -1),
                (0, 1)
                };

                foreach (var (dx, dy) in directions)
                {
                    int newX = cell.Row;
                    int newY = cell.Column;

                    while (true)
                    {
                        newX += dx;
                        newY += dy;

                        if (newX >= 0 && newX < Board.Count && newY >= 0 && newY < Board.Count)
                        {

                            if (Board[newX][newY].PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
                            {
                                availableSquares.Add(Board[newX][newY]);
                            }
                            else if (Board[newX][newY].PIECE.PieceColor != cell.PIECE.PieceColor)
                            {
                                availableSquares.Add(Board[newX][newY]);
                                break;
                            }
                            else
                            {
                                break;
                            }

                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else if (cell.PIECE.PieceType == PIECE_TYPE.QUEEN)
            {
                var directions = new (int dx, int dy)[] {
                (-1, 1),
                (1, 1),
                (-1, -1),
                (1, -1),
                (1, 0),
                (-1, 0),
                (0, -1),
                (0, 1)
                };

                foreach (var (dx, dy) in directions)
                {
                    int newX = cell.Row;
                    int newY = cell.Column;

                    while (true)
                    {
                        newX += dx;
                        newY += dy;

                        if (newX >= 0 && newX < Board.Count && newY >= 0 && newY < Board.Count)
                        {

                            if (Board[newX][newY].PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
                            {
                                availableSquares.Add(Board[newX][newY]);
                            }
                            else if (Board[newX][newY].PIECE.PieceColor != cell.PIECE.PieceColor)
                            {
                                availableSquares.Add(Board[newX][newY]);
                                break;
                            }
                            else
                            {
                                break;
                            }

                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else if (cell.PIECE.PieceType == PIECE_TYPE.KING)
            {
                List<BoardCell> cells = new List<BoardCell>
                    {
                GetBoardAt(OffsetPerspective(1), cell.Column-1),
                GetBoardAt(OffsetPerspective(1), cell.Column),
                GetBoardAt(OffsetPerspective(1), cell.Column+1),
                GetBoardAt(OffsetPerspective(0), cell.Column+1),
                GetBoardAt(OffsetPerspective(0), cell.Column-1),
                GetBoardAt(OffsetPerspective(-1), cell.Column-1),
                GetBoardAt(OffsetPerspective(-1), cell.Column),
                GetBoardAt(OffsetPerspective(-1), cell.Column+1),
                };

                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i] == null) continue;
                    if (cells[i].PIECE.PieceType == PIECE_TYPE.NULL_PIECE || cells[i].PIECE.PieceColor !=  cell.PIECE.PieceColor)
                    {
                        var oppositeAvailableSquares = cells[i].PIECE.GetAvailableSquares(cells[i]);
                        bool valid = true;
                        for (int j = 0; j < oppositeAvailableSquares.Count; j++)
                        {
                            if (oppositeAvailableSquares[j].Column != cell.Row && oppositeAvailableSquares[j].Row != cell.Column)
                            {
                                valid = false;
                            }
                        }
                        if (valid) availableSquares.Add(cells[i]);
                    }
                }
            }

            return availableSquares;
        }
    }
}

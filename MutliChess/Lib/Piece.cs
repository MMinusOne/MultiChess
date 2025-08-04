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
            this.PieceColor = PIECE_COLOR.NULL;
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

        public List<BoardCell> GetAvailableSquares(BoardCell cell, BoardCell relativeCell)
        {
            var chessViewModel = ChessViewModel.Instance;
            var Board = chessViewModel.Board;
            var GetBoardAt = chessViewModel.GetBoardAt;
            var ColorBoardPerspective = chessViewModel.ColorBoardPerspective;

            if (relativeCell == null) relativeCell = ChessViewModel.Instance.SelectedCells[0];

            var availableSquares = new List<BoardCell>();

            if (cell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
            {

            }
            else if (cell.PIECE.PieceType == PIECE_TYPE.PAWN)
            {
                var nextCell = GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column);
                if (nextCell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
                {
                    availableSquares.Add(nextCell);
                }
                var nextSecondCell = GetBoardAt(chessViewModel.OffsetPerspective(2, relativeCell), cell.Column);
                if (cell.PIECE.MovedOnce == false && nextSecondCell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
                {
                    availableSquares.Add(nextSecondCell);
                }
                var sideRightCell = GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column+1);
                var sideLeftCell = GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column-1);
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
            else if (cell.PIECE.PieceType == PIECE_TYPE.KNIGHT)
            {
                var boardPerspective = ColorBoardPerspective(cell.PIECE.PieceColor);

                List<BoardCell> cells = new List<BoardCell>
                    {
                GetBoardAt(chessViewModel.OffsetPerspective(2, relativeCell), cell.Column-1),
                GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column-2),
                GetBoardAt(chessViewModel.OffsetPerspective(2, relativeCell), cell.Column+1),
                GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column+2),
                GetBoardAt(chessViewModel.OffsetPerspective(-2, relativeCell), cell.Column-1),
                GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column-2),
                GetBoardAt(chessViewModel.OffsetPerspective(-2, relativeCell), cell.Column+1),
                GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column+2),
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
                GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column-1),
                GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column),
                GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column+1),
                GetBoardAt(chessViewModel.OffsetPerspective(0, relativeCell), cell.Column+1),
                GetBoardAt(chessViewModel.OffsetPerspective(0, relativeCell), cell.Column-1),
                GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column-1),
                GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column),
                GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column+1),
                };

                List<BoardCell> allOppositeAvailableSquares = new List<BoardCell>();


                for (int x = 0; x < Board.Count; x++)
                {
                    for (int y = 0; y < Board[x].Count; y++)
                    {
                        var oppositePiece = GetBoardAt(x, y);
                        if (oppositePiece == null) continue;
                        if (oppositePiece.PIECE.PieceColor == cell.PIECE.PieceColor) continue;
                        if (oppositePiece.PIECE.PieceType == PIECE_TYPE.KING) continue;
                        if (oppositePiece.PIECE.PieceType == PIECE_TYPE.PAWN)
                        {
                            var sideRightPiece = GetBoardAt(oppositePiece.PIECE.PieceColor == PIECE_COLOR.WHITE ? oppositePiece.Row+1 : oppositePiece.Row-1, oppositePiece.Column-1);
                            var sideLeftPiece = GetBoardAt(oppositePiece.PIECE.PieceColor == PIECE_COLOR.WHITE ? oppositePiece.Row+1 : oppositePiece.Row-1, oppositePiece.Column+1);
                            if (sideRightPiece != null) allOppositeAvailableSquares.Add(sideRightPiece);
                            if (sideLeftPiece != null) allOppositeAvailableSquares.Add(sideLeftPiece);
                            continue;
                        }
                        ;

                        if (oppositePiece.PIECE.PieceType == PIECE_TYPE.NULL_PIECE) continue;

                        var avSquaresInversed = oppositePiece.PIECE.GetAvailableSquares(oppositePiece, oppositePiece);

                        for (int z = 0; z < avSquaresInversed.Count; z++)
                        {
                            allOppositeAvailableSquares.Add(avSquaresInversed[z]);
                        }
                    }
                }

                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i] == null) continue;
                    if (IsKingInCheck(cells[i], allOppositeAvailableSquares)) continue;
                    if (
                        cells[i].PIECE.PieceType == PIECE_TYPE.NULL_PIECE ||
                        cells[i].PIECE.PieceColor != cell.PIECE.PieceColor)
                    {
                        availableSquares.Add(cells[i]);
                    }
                }
            }

            return availableSquares;
        }

        public bool IsKingInCheck(BoardCell cell, List<BoardCell> allOppositeAvailableSquares)
        {
            bool kingChecked = false;

            for (int i = 0; i < allOppositeAvailableSquares.Count; i++)
            {
                if (allOppositeAvailableSquares[i].BoardIndex == cell.BoardIndex)
                {
                    kingChecked = true;
                }

            }

            return kingChecked;
        }
    }

}

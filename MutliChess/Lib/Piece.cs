using MultiChess.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public List<AvailableSquareCollection> GetAllOppositeAvailableSquares(PIECE_COLOR originColor, ObservableCollection<ObservableCollection<BoardCell>> board = null)
        {
            List<AvailableSquareCollection> allOppositeAvailableSquares = new List<AvailableSquareCollection>();
            var chessViewModel = ChessViewModel.Instance;
            var Board = chessViewModel.Board;
            var BoardTemp = chessViewModel.Board;
            if (board != null)
            {
                Board = board;
                chessViewModel.Board = board;
            }

            for (int x = 0; x < Board.Count; x++)
            {
                for (int y = 0; y < Board[x].Count; y++)
                {
                    var oppositePiece = chessViewModel.GetBoardAt(x, y);
                    if (oppositePiece == null) continue;
                    if (oppositePiece.PIECE.PieceColor == originColor) continue;
                    if (oppositePiece.PIECE.PieceType == PIECE_TYPE.KING) continue;
                    if (oppositePiece.PIECE.PieceType == PIECE_TYPE.PAWN)
                    {
                        var sideRightPiece = chessViewModel.GetBoardAt(oppositePiece.PIECE.PieceColor == PIECE_COLOR.WHITE ? oppositePiece.Row+1 : oppositePiece.Row-1, oppositePiece.Column-1);
                        var sideLeftPiece = chessViewModel.GetBoardAt(oppositePiece.PIECE.PieceColor == PIECE_COLOR.WHITE ? oppositePiece.Row+1 : oppositePiece.Row-1, oppositePiece.Column+1);
                        var collection = new AvailableSquareCollection();
                        if (sideRightPiece != null) collection.AddAvailableCell(sideRightPiece);
                        if (sideLeftPiece != null) collection.AddAvailableCell(sideLeftPiece);
                        collection.SetOriginator(oppositePiece);
                        allOppositeAvailableSquares.Add(collection);
                        continue;
                    }
                    ;

                    if (oppositePiece.PIECE.PieceType == PIECE_TYPE.NULL_PIECE) continue;

                    var avSquaresInversed = oppositePiece.PIECE.GetAvailableSquares(oppositePiece, oppositePiece, false, true);
                    var squaresCollection = new AvailableSquareCollection();
                    squaresCollection.SetOriginator(oppositePiece);
                    for (int z = 0; z < avSquaresInversed.Count; z++)
                    {
                        squaresCollection.AddAvailableCell(avSquaresInversed[z]);
                    }
                    allOppositeAvailableSquares.Add(squaresCollection);
                }
            }
            chessViewModel.Board = BoardTemp;
            return allOppositeAvailableSquares;
        }

        public List<BoardCell> GetAvailableSquares(BoardCell cell, BoardCell relativeCell, bool checkCheck = false, bool fromGetOpposite = false)
        {
            var chessViewModel = ChessViewModel.Instance;
            ObservableCollection<ObservableCollection<BoardCell>> Board = chessViewModel.Board;
            var ColorBoardPerspective = chessViewModel.ColorBoardPerspective;

            if (relativeCell == null) relativeCell = ChessViewModel.Instance.SelectedCells[0];

            var availableSquares = new List<BoardCell>();


            BoardCell kingCell = null;
            List<AvailableSquareCollection> allOppositeAvailableSquares = new List<AvailableSquareCollection>();

            if (checkCheck)
            {
                allOppositeAvailableSquares = GetAllOppositeAvailableSquares(cell.PIECE.PieceColor);
                kingCell = FindKing(cell.PIECE.PieceColor);

            }

            bool inCheck = false;
            List<BoardCell> piecesChecking = new List<BoardCell>();

            if (kingCell != null && checkCheck)
            {
                piecesChecking = GetPiecesChecking(kingCell, allOppositeAvailableSquares);
                if (piecesChecking.Count != 0)
                {
                    inCheck = true;
                }
            }

            if (cell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE)
            {

            }
            else if (cell.PIECE.PieceType == PIECE_TYPE.PAWN)
            {
                var nextCell = chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column);
                if (nextCell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE && !inCheck)
                {
                    availableSquares.Add(nextCell);
                }
                else if (nextCell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE && inCheck && isValidCheckCell(nextCell, cell, piecesChecking, kingCell))
                {
                    availableSquares.Add(nextCell);
                }
                var nextSecondCell = chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(2, relativeCell), cell.Column);
                if (cell.PIECE.MovedOnce == false && nextSecondCell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE  && !inCheck)
                {
                    availableSquares.Add(nextSecondCell);
                }
                else if (inCheck && cell.PIECE.MovedOnce == false && nextSecondCell.PIECE.PieceType == PIECE_TYPE.NULL_PIECE && isValidCheckCell(nextSecondCell, cell, piecesChecking, kingCell))
                {
                    availableSquares.Add(nextSecondCell);
                }

                var sideRightCell = chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column+1);

                if (sideRightCell != null && sideRightCell.PIECE.PieceColor != cell.PIECE.PieceColor)
                {
                    if (sideRightCell.PIECE.PieceType != PIECE_TYPE.NULL_PIECE && !inCheck)
                    {
                        availableSquares.Add(sideRightCell);
                    }
                    else if (inCheck && isValidCheckCell(sideRightCell, cell, piecesChecking, kingCell) && sideRightCell.PIECE.PieceType != PIECE_TYPE.NULL_PIECE)
                    {

                        availableSquares.Add(sideRightCell);
                    }

                }
                var sideLeftCell = chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column-1);

                if (sideLeftCell != null && sideLeftCell.PIECE.PieceColor != cell.PIECE.PieceColor)
                {
                    if (sideLeftCell.PIECE.PieceType != PIECE_TYPE.NULL_PIECE && !inCheck)
                    {
                        availableSquares.Add(sideLeftCell);
                    }
                    else if (inCheck && isValidCheckCell(sideLeftCell, cell, piecesChecking, kingCell) && sideLeftCell.PIECE.PieceType != PIECE_TYPE.NULL_PIECE)
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
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(2, relativeCell), cell.Column-1),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column-2),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(2, relativeCell), cell.Column+1),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column+2),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(-2, relativeCell), cell.Column-1),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column-2),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(-2, relativeCell), cell.Column+1),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column+2),
                };

                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i] == null) continue;
                    if (cells[i].PIECE.PieceType == PIECE_TYPE.NULL_PIECE || cells[i].PIECE.PieceColor !=  cell.PIECE.PieceColor)
                    {
                        if (!inCheck)
                        {
                            if (checkCheck && GetPiecesCheckingWithMutation(cell, cells[i]).Count == 0)
                            {
                                availableSquares.Add(cells[i]);
                            }
                            else
                            {
                                availableSquares.Add(cells[i]);
                            }
                        }
                        else if (inCheck)
                        {
                            if (isValidCheckCell(cells[i], cell, piecesChecking, kingCell))
                            {
                                availableSquares.Add(cells[i]);
                            }
                        }
                    }
                    else if (cells[i].PIECE.PieceColor == cell.PIECE.PieceColor && fromGetOpposite)
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
                                if (!inCheck)
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                                else if (inCheck && isValidCheckCell(Board[newX][newY], cell, piecesChecking, kingCell))
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                            }
                            else if (Board[newX][newY].PIECE.PieceColor != cell.PIECE.PieceColor)
                            {
                                if (!inCheck)
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                                else if (inCheck && isValidCheckCell(Board[newX][newY], cell, piecesChecking, kingCell))
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                                if (!fromGetOpposite)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (fromGetOpposite) availableSquares.Add(Board[newX][newY]);
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
                                if (!inCheck)
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                                else if (inCheck && isValidCheckCell(Board[newX][newY], cell, piecesChecking, kingCell))
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                            }
                            else if (Board[newX][newY].PIECE.PieceColor != cell.PIECE.PieceColor)
                            {
                                if (!inCheck)
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                                else if (inCheck && isValidCheckCell(Board[newX][newY], cell, piecesChecking, kingCell))
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                                if (!fromGetOpposite)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (fromGetOpposite) availableSquares.Add(Board[newX][newY]);
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
                                if (!inCheck)
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                                else if (inCheck && isValidCheckCell(Board[newX][newY], cell, piecesChecking, kingCell))
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                            }
                            else if (Board[newX][newY].PIECE.PieceColor != cell.PIECE.PieceColor)
                            {
                                if (!inCheck)
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                                else if (inCheck && isValidCheckCell(Board[newX][newY], cell, piecesChecking, kingCell))
                                {
                                    availableSquares.Add(Board[newX][newY]);
                                }
                                if (!fromGetOpposite)
                                {
                                    break;
                                }

                            }
                            else
                            {
                                if (fromGetOpposite) availableSquares.Add(Board[newX][newY]);
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
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column-1),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(1, relativeCell), cell.Column+1),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(0, relativeCell), cell.Column+1),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(0, relativeCell), cell.Column-1),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column-1),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column),
                chessViewModel.GetBoardAt(chessViewModel.OffsetPerspective(-1, relativeCell), cell.Column+1),
                };

                if (!cell.PIECE.MovedOnce)
                {
                    if (
                        Board[cell.Row][cell.Column+1].PIECE.PieceType == PIECE_TYPE.NULL_PIECE
                        &&
                        Board[cell.Row][cell.Column+2].PIECE.PieceType == PIECE_TYPE.NULL_PIECE
                        &&
                        Board[cell.Row][cell.Column+3].PIECE.PieceType == PIECE_TYPE.NULL_PIECE
                        &&
                        !inCheck
                        )
                    {
                        availableSquares.Add(Board[cell.Row][cell.Column+2]);
                    }

                    if (
                        Board[cell.Row][cell.Column-1].PIECE.PieceType == PIECE_TYPE.NULL_PIECE
                        &&
                        Board[cell.Row][cell.Column-2].PIECE.PieceType == PIECE_TYPE.NULL_PIECE
                        &&
                        !inCheck
                        )
                    {
                        availableSquares.Add(Board[cell.Row][cell.Column-2]);
                    }
                }

                for (int i = 0; i < cells.Count; i++)
                {
                    if (cells[i] == null) continue;
                    var futurePieceChecking = GetPiecesChecking(cells[i], allOppositeAvailableSquares);
                    if (futurePieceChecking.Count != 0) continue;
                    if (
                        cells[i].PIECE.PieceType == PIECE_TYPE.NULL_PIECE ||
                        cells[i].PIECE.PieceColor != cell.PIECE.PieceColor)
                    {
                        availableSquares.Add(cells[i]);
                    }
                    else if (cells[i].PIECE.PieceColor == cell.PIECE.PieceColor)
                    {
                        if (fromGetOpposite) availableSquares.Add(cells[i]);
                    }
                }
            }

            return availableSquares;
        }

        BoardCell FindKing(PIECE_COLOR color)
        {
            BoardCell kingCell = null;
            var Board = ChessViewModel.Instance.Board;
            for (int x = 0; x < Board.Count; x++)
            {
                for (int y = 0; y < Board[x].Count; y++)
                {
                    var oppositePiece = ChessViewModel.Instance.GetBoardAt(x, y);
                    if (oppositePiece == null) continue;
                    if (oppositePiece.PIECE.PieceColor == color && oppositePiece.PIECE.PieceType == PIECE_TYPE.KING)
                    {
                        kingCell = oppositePiece;
                        continue;
                    }
                }
            }

            return kingCell;
        }

        public List<BoardCell> GetPiecesCheckingWithMutation(BoardCell originalCell, BoardCell destinationCell)
        {
            var board = ChessViewModel.Instance.Board;
            var originalPieceTemp = originalCell.PIECE;
            var destinationPieceTemp = destinationCell.PIECE;
            var kingCell = FindKing(originalCell.PIECE.PieceColor);

            board[destinationCell.Row][destinationCell.Column].setPiece(originalPieceTemp);
            board[originalCell.Row][originalCell.Column].setPiece(destinationPieceTemp);

            var oppositeAvailableSquaresCollection = GetAllOppositeAvailableSquares(kingCell.PIECE.PieceColor, board);
            var piecesChecking = GetPiecesChecking(kingCell, oppositeAvailableSquaresCollection);

            board[destinationCell.Row][destinationCell.Column].setPiece(destinationPieceTemp);
            board[originalCell.Row][originalCell.Column].setPiece(originalPieceTemp);

            return piecesChecking;
        }

        public bool isValidCheckCell(BoardCell destination, BoardCell originalPiece, List<BoardCell> piecesChecking, BoardCell kingCell)
        {
            if (piecesChecking.Count == 1)
            {
                var piece = piecesChecking[0];

                if (destination.Row == piece.Row && destination.Column == piece.Column)
                {
                    return true;
                }

                //var piecesCheckingSim = GetPiecesCheckingWithMutation(originalPiece, destination);

                //if (piecesCheckingSim.Count == 0)
                //{
                //    return true;
                //}

                var Board = ChessViewModel.Instance.Board;
                var originalPieceTemp = originalPiece.PIECE;
                var destinationPieceTemp = destination.PIECE;
                Board[destination.Row][destination.Column].setPiece(originalPieceTemp);
                Board[originalPiece.Row][originalPiece.Column].setPiece(destinationPieceTemp);

                var checkPiecesCollection = new AvailableSquareCollection();
                var checkPieces = piece.PIECE.GetAvailableSquares(piece, piece, false);
                checkPiecesCollection.SetOriginator(piece);
                foreach (var checkPiece in checkPieces) { checkPiecesCollection.AddAvailableCell(checkPiece); }
                ;

                var piecesCheckingSim = GetPiecesChecking(kingCell, new List<AvailableSquareCollection> { checkPiecesCollection });

                if (piecesCheckingSim.Count == 0)
                {
                    Board[originalPiece.Row][originalPiece.Column].setPiece(originalPieceTemp);
                    Board[destination.Row][destination.Column].setPiece(destinationPieceTemp);
                    return true;
                }
                else
                {
                    Board[originalPiece.Row][originalPiece.Column].setPiece(originalPieceTemp);
                    Board[destination.Row][destination.Column].setPiece(destinationPieceTemp);
                }

            }
            else
            {
                return false;
            }

            return false;
        }

        public List<BoardCell> GetPiecesChecking(BoardCell kingCell, List<AvailableSquareCollection> oppositeAvailableSquaresCollection)
        {
            List<BoardCell> cellsChecking = new List<BoardCell>();

            for (int i = 0; i < oppositeAvailableSquaresCollection.Count; i++)
            {
                for (int j = 0; j <
                oppositeAvailableSquaresCollection[i].AvailableCells.Count; j++)
                {
                    if (oppositeAvailableSquaresCollection[i].AvailableCells[j].BoardIndex == kingCell.BoardIndex)
                    {
                        cellsChecking.Add(oppositeAvailableSquaresCollection[i].Originator);
                    }
                }

            }

            return cellsChecking;
        }
    }

    public class AvailableSquareCollection
    {
        public BoardCell Originator;
        public List<BoardCell> AvailableCells = new List<BoardCell>();

        public AvailableSquareCollection()
        {

        }

        public AvailableSquareCollection(BoardCell originator, List<BoardCell> availableCells)
        {
            Originator=originator;
            AvailableCells=availableCells;
        }

        public void SetOriginator(BoardCell originator)
        {
            this.Originator = originator;
        }

        public void AddAvailableCell(BoardCell cell)
        {
            this.AvailableCells.Add(cell);
        }
    }
}

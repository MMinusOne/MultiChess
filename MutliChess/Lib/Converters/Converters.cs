using MultiChess.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiChess.Lib.Converters
{
    internal class ParentSizeToPieceSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var boardSize = ChessViewModel.Instance.BoardSize;

                return (int)value / boardSize;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class IndexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BoardCell)
            {
                var boardCell = (BoardCell)value;

                if(boardCell.IsAvailable)
                {
                    return Brushes.Red;
                }
                if(boardCell.IsCellSelected)
                {
                    return Brushes.LightBlue;
                }


                var boardIndex = boardCell.BoardIndex;
                double rowNumber = (int)boardIndex / 8;
                bool isOddRow = rowNumber % 2 == 0;
                var v = isOddRow ? (int)boardIndex + 1 : (int)boardIndex;
                if (v % 2 == 0)
                {
                    return Brushes.Black;
                }
                else
                {
                    return Brushes.White;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class PieceEnumToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BoardCell)
            {
                BoardCell cell = (BoardCell)value;
                if (cell.PIECE == null) return null;
                var isBlack = cell.PIECE.PieceColor == PIECE_COLOR.BLACK;

                switch (cell.PIECE.PieceType)
                {
                    case PIECE_TYPE.NULL_PIECE:
                        return null;
                    case PIECE_TYPE.PAWN:
                        return this.PiecePath("P", isBlack);
                    case PIECE_TYPE.KNIGHT:
                        return this.PiecePath("N", isBlack);
                    case PIECE_TYPE.BISHOP:
                        return this.PiecePath("B", isBlack);
                    case PIECE_TYPE.ROOK:
                        return this.PiecePath("R", isBlack);
                    case PIECE_TYPE.QUEEN:
                        return this.PiecePath("Q", isBlack);
                    case PIECE_TYPE.KING:
                        return this.PiecePath("K", isBlack);
                }
            }
            else
            {
                return null;
            }
            return null;
        }

        private string PiecePath(string id, bool isBlack)
        {
            var prefix = isBlack ? "b" : "w";
            return "/Static/MidnightSet/" + prefix + id + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

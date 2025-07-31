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

        int rows = 8; 
        public int Rows
        {
            get { return rows; }
            set
            {
                if (rows != value)
                {
                    rows = value;
                    OnPropertyChanged(nameof(Rows));
                }
            }
        }

        int columns = 8;
        public int Columns
        {
            get { return columns; }
            set
            {
                if (columns != value)
                {
                    columns = value;
                    OnPropertyChanged(nameof(Columns));
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
            for(int x = 0; x < rows; x++)
            {
                ObservableCollection<BoardCell> row = new ObservableCollection<BoardCell>();
                for(int y = 0; y < columns; y++)
                {
                    BoardCell boardCell = new BoardCell();
                    row.Add(boardCell);
                }
                Board.Add(row);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

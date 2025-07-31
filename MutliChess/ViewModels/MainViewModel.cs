using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiChess.ViewModels
{
    public class MainViewModel: BaseVM, INotifyPropertyChanged
    {
        BaseVM _chessBoard = ChessViewModel.Instance;
        public BaseVM ChessBoard
        {
            get { return _chessBoard; }
            set { _chessBoard = value; }
        }

        static BaseVM _instance;
        static public BaseVM Instance {
            get { 
                if(_instance == null) {
                    _instance = new MainViewModel();
                }
                return _instance;
            }
        }

        public MainViewModel() {
            ChessBoard = ChessViewModel.Instance;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

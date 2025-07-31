using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiChess.ViewModels
{
    public class BaseVM
    {
        private static BaseVM _baseInstance;
        public static BaseVM BaseInstance
        {
            get
            {
                if (_baseInstance == null)
                {
                    _baseInstance = new BaseVM();
                }
                return _baseInstance;
            }
        }

        public BaseVM()
        {
            _baseInstance = this;
        }
    }
}

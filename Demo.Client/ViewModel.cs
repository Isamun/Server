using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Protocol;

namespace Demo.Client
{
    public class ViewModel : BaseViewModel, IViewModel
    {
        private Execution _currentExecution;
        private Boolean _connected;

        public ViewModel()
        {

            _instance = this;
            _connected = false;
            _currentExecution = new Execution();
        }

   


        public Execution CurrentExecution
        {
            get
            {
                return _currentExecution;
            }
            set
            {
                _currentExecution = value;
                OnPropertyChanged("CurrentExecution");
            }
        }

        public bool Connected
        {
            get
            {
               return _connected;
            }
            set
            {
                _connected = value;
                OnPropertyChanged("Connected");
            }
        }




        private static IViewModel _instance = null; 
        public static IViewModel Instance {
            get { return _instance; }
            set { _instance = value; }
        }
        
           
        
    }

   
}

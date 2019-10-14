using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalComm.ViewModel
{
    class MainWindowViewModel
    {
        private ObservableCollection<int> az = new ObservableCollection<int>();
        private int counter = 0;

        public ObservableCollection<int> Az { get => az; set => az = value; }

        public MainWindowViewModel() 
        {

        }

        public void addToCollection()
        {
            counter++;
            az.Add(counter);
            Console.WriteLine(counter);
            Console.WriteLine(az.Count);
        }


    }
}

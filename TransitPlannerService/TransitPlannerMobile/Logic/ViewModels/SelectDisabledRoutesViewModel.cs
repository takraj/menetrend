using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransitPlannerMobile.Logic.ViewModels
{
    public class SelectDisabledRoutesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Tram { get; set; }
        public bool Subway { get; set; }
        public bool Bus { get; set; }
        public bool Funicular { get; set; }
        public bool Ferry { get; set; }
        public bool CableCar { get; set; }
        public bool Rail { get; set; }
        public bool Gondola { get; set; }
    }
}

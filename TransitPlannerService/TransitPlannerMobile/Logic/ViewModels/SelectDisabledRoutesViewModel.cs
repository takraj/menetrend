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

        public void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        private bool _tram, _subway, _bus, _funicular, _ferry, _cablecar, _rail, _gondola;

        public bool Tram
        {
            get
            {
                return _tram;
            }
            set
            {
                _tram = value;
                OnPropertyChanged("Tram");
            }
        }

        public bool Subway
        {
            get
            {
                return _subway;
            }
            set
            {
                _subway = value;
                OnPropertyChanged("Subway");
            }
        }

        public bool Bus
        {
            get
            {
                return _bus;
            }
            set
            {
                _bus = value;
                OnPropertyChanged("Bus");
            }
        }

        public bool Funicular
        {
            get
            {
                return _funicular;
            }
            set
            {
                _funicular = value;
                OnPropertyChanged("Funicular");
            }
        }

        public bool Ferry
        {
            get
            {
                return _ferry;
            }
            set
            {
                _ferry = value;
                OnPropertyChanged("Ferry");
            }
        }

        public bool CableCar
        {
            get
            {
                return _cablecar;
            }
            set
            {
                _cablecar = value;
                OnPropertyChanged("CableCar");
            }
        }

        public bool Rail
        {
            get
            {
                return _rail;
            }
            set
            {
                _rail = value;
                OnPropertyChanged("Rail");
            }
        }

        public bool Gondola
        {
            get
            {
                return _gondola;
            }
            set
            {
                _gondola = value;
                OnPropertyChanged("Gondola");
            }
        }
    }
}

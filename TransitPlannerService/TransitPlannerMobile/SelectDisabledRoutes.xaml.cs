using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TransitPlannerMobile.Logic;

namespace TransitPlannerMobile
{
    public partial class SelectDisabledRoutes : PhoneApplicationPage
    {
        public SelectDisabledRoutes()
        {
            InitializeComponent();

            DataContext = God.disabledRoutes;
        }
    }
}
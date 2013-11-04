using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics;
using System.Threading;

namespace TUSZ.PhoneClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        private List<RoutePlannerService.VM_Stop> listOfStops;
        private int countOfResults;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            countOfResults = 0;
            listOfStops = new List<RoutePlannerService.VM_Stop>();
            RoutePlannerService.RoutePlannerServiceClient svc = new RoutePlannerService.RoutePlannerServiceClient();

            if ((fromStop.Text.Length > 3) && (toStop.Text.Length > 3))
            {
                svc.GetStopsAsync(fromStop.Text);
                svc.GetStopsAsync(toStop.Text);
                svc.GetStopsCompleted += svc_GetStopsCompleted;

                //while (countOfResults < 2)
                //{
                //    Thread.Sleep(100);
                //}

                //lock (listOfStops)
                //{
                //    int stop1 = listOfStops.First(s => s.name.Contains(fromStop.Text)).id;
                //    int stop2 = listOfStops.First(s => s.name.Contains(toStop.Text)).id;

                //    svc.PlanAsync(stop1, stop2);
                //    svc.PlanCompleted += svc_PlanCompleted;
                //}
            }
        }

        void svc_PlanCompleted(object sender, RoutePlannerService.PlanCompletedEventArgs e)
        {
            Debug.WriteLine("Message received: " + e.Result.ToString());
        }

        private void svc_GetStopsCompleted(object sender, RoutePlannerService.GetStopsCompletedEventArgs e)
        {
            countOfResults++;
            lock (listOfStops)
            {
                listOfStops.AddRange(e.Result);
            }
        }
    }
}
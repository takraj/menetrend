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
using Coding4Fun.Toolkit.Controls;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using TransitPlannerMobile.Logic;
using Windows.Devices.Geolocation;

namespace TransitPlannerMobile
{
    public partial class MainPage : PhoneApplicationPage
    {
        private double ReportTextBoxHeight;
        private Action progressDialogActionOnCancel;
        private bool progressDialogMustBeVisible;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            var rps = new RoutePlannerService.SoapServiceClient();
            rps.GetAllStopsCompleted += rps_GetAllStopsCompleted;
            rps.GetAllStopsAsync();

            DataContext = God.mainPageViewModel;

            ShowProgressDialog("Kommunikáció a szerverrel...");
            Utility.DelayedCall(() => HideProgressDialog(), 5000);
        }

        private void ShowProgressDialog(string message, Action actionOnCancel = null)
        {
            lock (progressOverlay)
            {
                progressDialogMustBeVisible = true;
                progressDialogActionOnCancel = actionOnCancel;
            }

            Dispatcher.BeginInvoke(() =>
            {
                lock (progressOverlay)
                {
                    lblProgressMessage.Text = message;

                    if (progressDialogMustBeVisible)
                    {
                        progressOverlay.Show();
                    }
                }
            });
        }

        private void HideProgressDialog(bool isCancelled = false)
        {
            lock (progressOverlay)
            {
                progressDialogMustBeVisible = false;
            }

            if (progressDialogActionOnCancel != null)
            {
                if (isCancelled)
                {
                    progressDialogActionOnCancel.Invoke();
                }
                
                progressDialogActionOnCancel = null;
            }
            
            Dispatcher.BeginInvoke(() => 
            {
                lock (progressOverlay)
                {
                    if (!progressDialogMustBeVisible)
                    {
                        progressOverlay.Hide();
                    }
                }
            });
        }

        private void tbReportMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                double CurrentReportTextBoxHeight = tbReportMessage.ActualHeight;
 
                if (CurrentReportTextBoxHeight > ReportTextBoxHeight)
                {
                    svReport.ScrollToVerticalOffset(svReport.VerticalOffset + CurrentReportTextBoxHeight - ReportTextBoxHeight);
                }
 
                ReportTextBoxHeight = CurrentReportTextBoxHeight;
            });
        }

        private void btnClearReport_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbReportMessage.Text = string.Empty;
                swIncludePosition.IsChecked = true;
            });
        }

        private void btnSubmitReport_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressDialog("Küldés...");
            Utility.DelayedCall(() => HideProgressDialog(), 5000);
        }

        private void btnSelectReportType_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void btnUseLocationFrom_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressDialog("Helymeghatározás...");
            Utility.DelayedCall(() => HideProgressDialog(), 5000);
        }

        private void btnUseLocationTo_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressDialog("Helymeghatározás...");
            Utility.DelayedCall(() => HideProgressDialog(), 5000);
        }

        private void btnNow_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                dpPlanDate.Value = DateTime.Now;
                dpPlanTime.Value = DateTime.Now;
            });
        }

        private void btnSelectDisabledRoutes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectDisabledRoutes.xaml", UriKind.Relative));
        }

        private void btnSelectWalkingSpeed_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void btnSelectMaxWaitingTime_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void btnClearPlanning_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbPlanFrom.Text = string.Empty;
                tbPlanTo.Text = string.Empty;

                dpPlanDate.Value = DateTime.Now;
                dpPlanTime.Value = DateTime.Now;

                cbWheelchair.IsChecked = false;
            });
        }

        private async void btnMakePlan_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressDialog("Keresés...");
            Utility.DelayedCall(() => HideProgressDialog(), 5000);

            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromMinutes(5),
                timeout: TimeSpan.FromSeconds(10)
            );

            Debug.WriteLine(String.Format("Lat: {0} Lon: {1}", geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude));
        }

        void rps_GetAllStopsCompleted(object sender, RoutePlannerService.GetAllStopsCompletedEventArgs e)
        {
            Debug.WriteLine("Count of stops: " + e.Result.Count);
            God.mainPageViewModel.AvailableStops = e.Result;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            lock (progressOverlay)
            {
                if (progressDialogMustBeVisible)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
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
using TransitPlannerMobile.Logic.ViewModels;
using System.Device.Location;

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

            if (God.mainPageViewModel.AvailableStops == null)
            {
                ShowProgressDialog("Megállók letöltése...");

                var rps = new RoutePlannerService.SoapServiceClient();
                rps.GetAllStopsCompleted += rps_GetAllStopsCompleted;
                rps.GetAllStopsAsync();
            }

            DataContext = God.mainPageViewModel;
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

        private async void btnSubmitReport_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressDialog("Küldés...");

            var request = new RoutePlannerService.SendTroubleReportRequest
            {
                description = God.mainPageViewModel.ReportText,
                has_location_data = false,
                category = 5,
                timestamp = new RoutePlannerService.TransitDateTime
                {
                    day = DateTime.Now.Day,
                    hour = DateTime.Now.Hour,
                    minute = DateTime.Now.Minute,
                    month = DateTime.Now.Month,
                    year = DateTime.Now.Year
                }
            };

            switch (God.mainPageViewModel.TypeOfReport)
            {
                case "Bűncselekmény": request.category = 0; break;
                case "Baleset": request.category = 1; break;
                case "Késés": request.category = 2; break;
                case "Műszaki hiba": request.category = 3; break;
                case "Alkalmazáshiba": request.category = 4; break;
                default: request.category = 5; break;
            }

            if (God.mainPageViewModel.PositionIncluded)
            {
                Geolocator geolocator = new Geolocator();
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                );

                Debug.WriteLine(String.Format("Lat: {0} Lon: {1}", geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude));

                request.has_location_data = true;
                request.latitude = geoposition.Coordinate.Latitude;
                request.longitude = geoposition.Coordinate.Longitude;
            }

            var svc = new RoutePlannerService.SoapServiceClient();
            svc.SendTroubleReportCompleted += svc_SendTroubleReportCompleted;
            svc.SendTroubleReportAsync(request);
        }

        void svc_SendTroubleReportCompleted(object sender, AsyncCompletedEventArgs e)
        {
            HideProgressDialog();
            MessageBox.Show("Hibajelentés elküldve.");
        }

        private void btnSelectReportType_Click(object sender, RoutedEventArgs e)
        {
            cmReportType.IsOpen = true;
        }

        private async void btnUseLocationFrom_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressDialog("Helymeghatározás...");

            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromMinutes(5),
                timeout: TimeSpan.FromSeconds(10)
            );

            Debug.WriteLine(String.Format("Lat: {0} Lon: {1}", geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude));

            try
            {
                tbPlanFrom.Text = God.mainPageViewModel.AvailableStops.OrderBy(s => new GeoCoordinate(s.latitude, s.longitude).GetDistanceTo(geoposition.Coordinate.ToGeoCoordinate())).First().UniqueName;
            }
            catch
            {
                Debug.WriteLine("Nincs legközelebbi megálló.");
            }

            HideProgressDialog();
        }

        private async void btnUseLocationTo_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressDialog("Helymeghatározás...");

            Geolocator geolocator = new Geolocator();
            Geoposition geoposition = await geolocator.GetGeopositionAsync(
                maximumAge: TimeSpan.FromMinutes(5),
                timeout: TimeSpan.FromSeconds(10)
            );

            Debug.WriteLine(String.Format("Lat: {0} Lon: {1}", geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude));

            try
            {
                tbPlanTo.Text = God.mainPageViewModel.AvailableStops.OrderBy(s => new GeoCoordinate(s.latitude, s.longitude).GetDistanceTo(geoposition.Coordinate.ToGeoCoordinate())).First().UniqueName;
            }
            catch
            {
                Debug.WriteLine("Nincs legközelebbi megálló.");
            }

            HideProgressDialog();
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
            cmWalkSpeed.IsOpen = true;
        }

        private void btnSelectMaxWaitingTime_Click(object sender, RoutedEventArgs e)
        {
            cmWaitingTime.IsOpen = true;
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

        private void btnMakePlan_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressDialog("Keresés...");
            Utility.DelayedCall(() => HideProgressDialog(), 5000);
        }

        void rps_GetAllStopsCompleted(object sender, RoutePlannerService.GetAllStopsCompletedEventArgs e)
        {
            Debug.WriteLine("Count of stops: " + e.Result.Count);
            God.mainPageViewModel.AvailableStops = e.Result.Select(s => new UniqueTransitStop(s)).ToList();

            HideProgressDialog();
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

        private void optAnyOfReportTypes_Click(object sender, RoutedEventArgs e)
        {
            God.mainPageViewModel.TypeOfReport = (sender as MenuItem).Header.ToString();
        }
    }
}
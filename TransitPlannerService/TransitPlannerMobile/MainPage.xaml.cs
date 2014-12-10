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
using System.Threading.Tasks;

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

            God.disabledRoutes.PropertyChanged += disabledRoutes_PropertyChanged;

            if (God.mainPageViewModel.AvailableStops == null)
            {
                ShowProgressDialog("Megállók letöltése...");

                var rps = new RoutePlannerService.SoapServiceClient();
                rps.GetAllStopsCompleted += rps_GetAllStopsCompleted;
                rps.GetAllStopsAsync();
            }

            DataContext = God.mainPageViewModel;
        }

        void disabledRoutes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            God.mainPageViewModel.OnPropertyChanged("CountOfDisabledRoutes");
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

            if (e.Error != null)
            {
                MessageBox.Show("Hibajelentés küldése sikertelen.");
            }
            else
            {
                MessageBox.Show("Hibajelentés elküldve.");
            }
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
            God.mainPageViewModel.When = DateTime.Now;
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
            God.mainPageViewModel.FromStopName = string.Empty;
            God.mainPageViewModel.ToStopName = string.Empty;
            God.mainPageViewModel.When = DateTime.Now;
            God.mainPageViewModel.WheelchairSupportNeeded = false;
        }

        private void DoMakePlan()
        {
            Debug.WriteLine("Starting to make a plan...");

            var svc = new RoutePlannerService.SoapServiceClient();
            var parameters = new RoutePlannerService.WebTransitPlanRequestParameters();

            Debug.WriteLine("Service is ready. Collecting disabled route types...");
            
            var disabled_route_types = new System.Collections.ObjectModel.ObservableCollection<int>(God.mainPageViewModel.GetDisabledRouteTypes());

            Debug.WriteLine("Done with collecting disabled route types. Now finding stop ids...");

            parameters.from = -1;
            parameters.to = -1;

            foreach (var stop in God.mainPageViewModel.AvailableStops)
            {
                if (stop.UniqueName == God.mainPageViewModel.FromStopName)
                {
                    Debug.WriteLine("From " + stop.UniqueName);
                    parameters.from = stop.id;
                }

                if (stop.UniqueName == God.mainPageViewModel.ToStopName)
                {
                    Debug.WriteLine("To " + stop.UniqueName);
                    parameters.to = stop.id;
                }
            }

            if (parameters.from == -1)
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show("A 'honnan' megálló nem létezik."));
                HideProgressDialog();
                return;
            }

            if (parameters.to == -1)
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show("A 'hova' megálló nem létezik."));
                HideProgressDialog();
                return;
            }

            Debug.WriteLine("Stops were found. Setting up other parameters...");

            parameters.walking_speed_category = God.mainPageViewModel.GetWalkingSpeedCategoryNumerically();
            parameters.max_waiting_time = God.mainPageViewModel.GetMaxWaitingTimeCategoryNumerically();
            parameters.when = new RoutePlannerService.TransitDateTime
            {
                year = God.mainPageViewModel.When.Year,
                month = God.mainPageViewModel.When.Month,
                day = God.mainPageViewModel.When.Day,
                hour = God.mainPageViewModel.When.Hour,
                minute = God.mainPageViewModel.When.Minute
            };

            Debug.WriteLine("Sending request...");

            svc.GetPlanCompleted += svc_GetPlanCompleted;
            svc.GetPlanAsync(parameters);

            Debug.WriteLine("Request sent.");
        }

        private void btnMakePlan_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressDialog("Keresés...");

            var rps = new RoutePlannerService.SoapServiceClient();
            rps.GetMetadataCompleted += rps_GetMetadataCompleted;
            rps.GetMetadataAsync();
        }

        void rps_GetMetadataCompleted(object sender, RoutePlannerService.GetMetadataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Nem sikerült lekérdezni a leíró adatokat.");
                HideProgressDialog();
                return;
            }

            var valid_from = new DateTime(e.Result.valid_from.year, e.Result.valid_from.month, e.Result.valid_from.day);
            var valid_to = new DateTime(e.Result.valid_to.year, e.Result.valid_to.month, e.Result.valid_to.day);

            if (God.mainPageViewModel.When > valid_to || valid_from > God.mainPageViewModel.When)
            {
                MessageBox.Show(String.Format("Az adatbázisban tárolt menetrend csak '{0}' és '{1}' között érvényes.", valid_from, valid_to));
                HideProgressDialog();
                return;
            }

            Task.Factory.StartNew(() =>
            {
                DoMakePlan();
            });
        }

        void svc_GetPlanCompleted(object sender, RoutePlannerService.GetPlanCompletedEventArgs e)
        {
            Debug.WriteLine("A tervező válaszolt.");

            HideProgressDialog();

            if (e.Error != null)
            {
                MessageBox.Show("Nincs útvonal.");
                return;
            }

            Debug.WriteLine("Sikerült a tervezés. Algoritmus: " + e.Result.algorithm);
        }

        void Exit()
        {
            while (((PhoneApplicationFrame)App.Current.RootVisual).CanGoBack)
            {
                ((PhoneApplicationFrame)App.Current.RootVisual).RemoveBackEntry();
            }
            Application.Current.Terminate();
        }

        void rps_GetAllStopsCompleted(object sender, RoutePlannerService.GetAllStopsCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                HideProgressDialog();
                MessageBox.Show("Megállók lekérése megszakítva. Az alkalmazás kilép.");
                Exit();
                return;
            }

            if (e.Error != null)
            {
                HideProgressDialog();
                MessageBox.Show("A megállók lekérése során hiba történt. Az alkalmazás kilép.");
                Exit();
                return;
            }

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

        private void optAnyOfWalkingSpeeds_Click(object sender, RoutedEventArgs e)
        {
            God.mainPageViewModel.WalkingSpeed = (sender as MenuItem).Header.ToString();
        }

        private void optAnyOfMaxWaitingTimes_Click(object sender, RoutedEventArgs e)
        {
            God.mainPageViewModel.MaxWaitingTime = (sender as MenuItem).Header.ToString();
        }

        private void optAnyOfReportTypes_Click(object sender, RoutedEventArgs e)
        {
            God.mainPageViewModel.TypeOfReport = (sender as MenuItem).Header.ToString();
        }
    }
}
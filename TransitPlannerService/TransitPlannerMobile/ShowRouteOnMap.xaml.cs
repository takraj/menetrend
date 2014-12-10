using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using TransitPlannerMobile.Logic;
using Microsoft.Phone.Maps.Toolkit;
using System.Windows.Media;
using System.Globalization;
using System.Device.Location;

namespace TransitPlannerMobile
{
    public partial class ShowRouteOnMap : PhoneApplicationPage
    {
        private MapLayer pushpinLayer;

        public ShowRouteOnMap()
        {
            InitializeComponent();

            pushpinLayer = new MapLayer();
            bing.Layers.Add(pushpinLayer);
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DetailedPlan.xaml", UriKind.Relative));
        }

        private Color ConvertFromHtml(string htmlColor)
        {
            byte r = byte.Parse(htmlColor.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(htmlColor.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(htmlColor.Substring(4, 2), NumberStyles.HexNumber);

            return Color.FromArgb(byte.MaxValue, r, g, b);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            bing.ZoomLevel = 16;

            pushpinLayer.Clear();

            var pathOverlay = new MapOverlay();

            var lastSection = God.planViewModel.Sections.Last();

            foreach (var section in God.planViewModel.Sections)
            {
                var firstStep = section.Steps.First();
                var lastStep = section.Steps.Last();

                #region Pushpin placement
                {
                    var pushpinOverlay = new MapOverlay();
                    var where = new GeoCoordinate(firstStep.Stop.Latitude, firstStep.Stop.Longitude);
                    pushpinOverlay.GeoCoordinate = where;

                    var pushpin = new Pushpin
                    {
                        BorderThickness = new Thickness(5),
                        FontSize = 24
                    };

                    if (section.IsWalking)
                    {
                        pushpin.Content = String.Format("Séta", firstStep.Stop.Id);
                    }
                    else
                    {
                        pushpin.Content = section.SectionBadge.BadgeLabel;

                        var bgcolor = ConvertFromHtml(section.SectionBadge.BadgeBackgroundColor);
                        var fgcolor = ConvertFromHtml(section.SectionBadge.BadgeLabelColor);

                        pushpin.Background = new SolidColorBrush(bgcolor);
                        pushpin.Foreground = new SolidColorBrush(fgcolor);
                    }

                    pushpinOverlay.Content = pushpin;
                    pushpinOverlay.PositionOrigin = new Point(0, 1);
                    pushpinLayer.Add(pushpinOverlay);

                    if ((section == lastSection) && ((firstStep != lastStep) || section.IsWalking))
                    {
                        var lastPushpinOverlay = new MapOverlay();
                        var lastWhere = new GeoCoordinate(lastStep.Stop.Latitude, lastStep.Stop.Longitude);
                        lastPushpinOverlay.GeoCoordinate = lastWhere;

                        var lastPushpin = new Pushpin
                        {
                            BorderThickness = new Thickness(3),
                            FontSize = 24
                        };

                        lastPushpin.Content = "Célállomás";

                        lastPushpinOverlay.Content = lastPushpin;
                        lastPushpinOverlay.PositionOrigin = new Point(0, 1);
                        pushpinLayer.Add(lastPushpinOverlay);

                        bing.Center = lastWhere;
                    }
                }
                #endregion

                #region Path placement
                {
                    for (int i = 0; i < section.Steps.Count - 1; i++)
                    {
                        MapPolyline line = new MapPolyline();

                        if (section.IsWalking)
                        {
                            line.StrokeThickness = 5;
                        }
                        else
                        {
                            line.StrokeThickness = 10;
                        }

                        if (!section.IsWalking)
                        {
                            line.StrokeColor = ConvertFromHtml(section.SectionBadge.BadgeBackgroundColor);
                        }

                        var stop1 = section.Steps[i].Stop;
                        var stop2 = section.Steps[i + 1].Stop;

                        line.Path.Add(new GeoCoordinate(stop1.Latitude, stop1.Longitude));
                        line.Path.Add(new GeoCoordinate(stop2.Latitude, stop2.Longitude));

                        bing.MapElements.Add(line);
                    }
                }
                #endregion
            }
        }
    }
}
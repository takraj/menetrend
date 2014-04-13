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

namespace TransitPlannerMobile
{
    public partial class MainPage : PhoneApplicationPage
    {
        double InputHeight;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void tbReportMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                double CurrentInputHeight = tbReportMessage.ActualHeight;
 
                //after the user starts typing text, text will eventually wrap to the next line
                //this ensures the textbox height doesnt sink below the bottom of the scrollviewer
                if (CurrentInputHeight > InputHeight)
                {
                    svReport.ScrollToVerticalOffset(svReport.VerticalOffset + CurrentInputHeight - InputHeight);
                }
 
                InputHeight = CurrentInputHeight;
            });
        }
    }
}
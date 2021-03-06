﻿using System.Windows;

using GMap.NET;

using SIGENCEScenarioTool.Models;



namespace SIGENCEScenarioTool.TestSuite
{
    /// <summary>
    /// Interaktionslogik für ScenarioWindow.xaml
    /// </summary>
    public partial class ScenarioWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioWindow"/> class.
        /// </summary>
        public ScenarioWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Displays the scenario.
        /// </summary>
        public void DisplayScenario()
        {
            dg.ItemsSource = RFDeviceList.CreateRandomizedRFDeviceList(16, new PointLatLng(49.7454, 6.6149));
            wb.NavigateToString("<h1>Simple Testscenario</h1><hr/><ul><li>Item 1</li><li>Item 2</li><li>Item 3</li></ul><table border=\"1\"><tr><th>Column1</th><th>Column2</th></tr><tr><td>Column1</td><td>Column2</td></tr></table><br/></br/><hr /><img src=\"https://upload.wikimedia.org/wikipedia/commons/thumb/7/7b/Geigentonspektrum.svg/262px-Geigentonspektrum.svg.png\"/><img src=\"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5d/Airbus_Logo_2017.svg/200px-Airbus_Logo_2017.svg.png\"/>");
            l.Content = "TestScenario Porta Nigra";
        }

    } // end public partial class ScenarioWindow 
}

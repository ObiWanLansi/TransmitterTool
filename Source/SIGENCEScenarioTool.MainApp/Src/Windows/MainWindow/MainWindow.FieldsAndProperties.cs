﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

using Markdig;

using Microsoft.Win32;

using SIGENCEScenarioTool.Datatypes.Geo;
using SIGENCEScenarioTool.Datatypes.Observable;
using SIGENCEScenarioTool.Models;
using SIGENCEScenarioTool.Models.MetaInformation;
using SIGENCEScenarioTool.Models.RxTxTypes;
using SIGENCEScenarioTool.Models.Templates;
using SIGENCEScenarioTool.Tools;
using SIGENCEScenarioTool.ViewModels;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ExplicitCallerInfoArgument



namespace SIGENCEScenarioTool.Windows.MainWindow
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MainWindow
    {
        /// <summary>
        /// The SFD save sigence scenario
        /// </summary>
        private readonly SaveFileDialog sfdSaveSIGENCEScenario = new SaveFileDialog();

        /// <summary>
        /// The ofd load sigence scenario
        /// </summary>
        private readonly OpenFileDialog ofdLoadSIGENCEScenario = new OpenFileDialog();

        /// <summary>
        /// The SFD export rf devices
        /// </summary>
        private readonly SaveFileDialog sfdExportSIGENCEScenario = new SaveFileDialog();

        /// <summary>
        /// The ofd import rf devices
        /// </summary>
        private readonly OpenFileDialog ofdImportSIGENCEScenario = new OpenFileDialog();

        /// <summary>
        /// The SFD save templates
        /// </summary>
        private readonly SaveFileDialog sfdSaveTemplates = new SaveFileDialog();

        /// <summary>
        /// The ofd load templates
        /// </summary>
        private readonly OpenFileDialog ofdLoadTemplates = new OpenFileDialog();

        /// <summary>
        /// The SFD save templates
        /// </summary>
        private readonly SaveFileDialog sfdExportSettings = new SaveFileDialog();

        /// <summary>
        /// The ofd load templates
        /// </summary>
        private readonly OpenFileDialog ofdImportSettings = new OpenFileDialog();

        /// <summary>
        /// The SFD save screenshot
        /// </summary>
        private readonly SaveFileDialog sfdSaveScreenshot = new SaveFileDialog();

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The empty template
        /// </summary>
        private static readonly RFDeviceTemplate EMPTY_TEMPLATE = new RFDeviceTemplate(new RFDevice { PrimaryKey = Guid.Empty, Name = "Empty Device" });

        /// <summary>
        /// The pipeline
        /// </summary>
        //private static readonly MarkdownPipeline MAPI = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseEmojiAndSmiley().UseBootstrap().Build();
        private static readonly MarkdownPipeline MAPI = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseEmojiAndSmiley().Build();

        /// <summary>
        /// The string header
        /// </summary>
        private static readonly string HEADER =
            "<!DOCTYPE html>\n" +
            "<html>\n" +
            "    <head>\n" +
            //"        <link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css\" integrity=\"sha384-HSMxcRTRxnN+Bdg0JdbxYKrThecOKuH5zCYotlSAcp1+c8xmyTe9GYg1l9a69psu\" crossorigin=\"anonymous\">\n" +
            //"        <link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap-theme.min.css\" integrity=\"sha384-6pzBo3FDv/PJ8r2KRkGHifhEocL+1X2rVCTTkUfGk7/0pbek5mMa1upzvWbrUbOZ\" crossorigin=\"anonymous\">\n" +
            //"        <script src=\"https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js\" integrity=\"sha384-aJ21OjlMXNL5UyIl/XNwTMqvzeRMZH2w8c5cRVpzpU8Y5bApTppSuUkhZXN0VxHd\" crossorigin=\"anonymous\"></script>\n" +
            "        <style>\n" +
            "           $STYLE$\n" +
            //"            table { border: 2px solid black; }\n" +
            //"            th { border: 1px solid black; padding: 3px; background: #D0D0D0; }\n" +
            //"            td { border: 1px solid black; padding: 3px; background: #F0F0F0; }\n" +
            "        </style>\n" +
            "    </head>\n" +
            "    <body style=\"padding: 10px;\">\n";

        /// <summary>
        /// The string footer
        /// </summary>
        private static readonly string FOOTER =
            "    </body>\n" +
            "</html>";

        /// <summary>
        /// The b data grid in edit mode
        /// </summary>
        private bool bDataGridInEditMode = false;

        /// <summary>
        /// The b no flash back
        /// </summary>
        private bool bNoFlashBack = false;

        /// <summary>
        /// The settings
        /// </summary>
        private Properties.Settings settings = Properties.Settings.Default;

        /// <summary>
        /// The missing
        /// </summary>
        private readonly object Missing = Type.Missing;

        /// <summary>
        /// A list with temporery devices to copy and paste.
        /// </summary>
        private readonly List<RFDeviceViewModel> lCopiedRFDevices = new List<RFDeviceViewModel>();

        /// <summary>
        /// The mr dalf
        /// </summary>
        private GMapRoute mrDALF = null;

        /// <summary>
        /// The DVM last selected device
        /// </summary>
        private RFDevice dvmLastSelectedDevice = null;

        /// <summary>
        /// The GFS
        /// </summary>
        private readonly GenericFoldingStrategy gfs = new GenericFoldingStrategy();

        /// <summary>
        /// The sb HTML
        /// </summary>
        private readonly StringBuilder sbHtml = new StringBuilder(8192);

        /// <summary>
        /// The sb markdown
        /// </summary>
        private readonly StringBuilder sbMarkdown = new StringBuilder(8192);

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Gets or sets the RFDevice collection.
        /// </summary>
        /// <value>
        /// The RFDevice collection.
        /// </value>
        public RFDeviceViewModelCollection RFDeviceViewModelCollection { get; set; } = new RFDeviceViewModelCollection();

        /// <summary>
        /// Gets or sets the rf device template collection.
        /// </summary>
        /// <value>
        /// The rf device template collection.
        /// </value>
        public RFDeviceTemplateCollection RFDeviceTemplateCollection { get; set; } = new RFDeviceTemplateCollection();

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The b description markdown changed
        /// </summary>
        private bool _bDescriptionMarkdownChanged = false;

        /// <summary>
        /// Gets or sets a value indicating whether [description markdown changed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [description markdown changed]; otherwise, <c>false</c>.
        /// </value>
        public bool DescriptionMarkdownChanged
        {
            get => this._bDescriptionMarkdownChanged;
            set
            {
                this._bDescriptionMarkdownChanged = value;

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The b description stylesheet changed
        /// </summary>
        private bool _bDescriptionStylesheetChanged = false;

        /// <summary>
        /// Gets or sets a value indicating whether [description stylesheet changed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [description stylesheet changed]; otherwise, <c>false</c>.
        /// </value>
        public bool DescriptionStylesheetChanged
        {
            get => this._bDescriptionStylesheetChanged;
            set
            {
                this._bDescriptionStylesheetChanged = value;

                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The dt current selected template
        /// </summary>
        private RFDeviceTemplate dtCurrentSelectedTemplate = EMPTY_TEMPLATE;

        /// <summary>
        /// Gets or sets the current selected template.
        /// </summary>
        /// <value>
        /// The current selected template.
        /// </value>
        public RFDeviceTemplate CurrentSelectedTemplate
        {
            get => this.dtCurrentSelectedTemplate;
            set
            {
                this.dtCurrentSelectedTemplate = value;

                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The b creating RFDevice
        /// </summary>
        private bool bCreatingRFDevice = false;


        /// <summary>
        /// Gets or sets a value indicating whether [creating rf device].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [creating rf device]; otherwise, <c>false</c>.
        /// </value>
        public bool CreatingRFDevice
        {
            get => this.bCreatingRFDevice;
            set
            {
                this.bCreatingRFDevice = value;

                SetMapToCreatingRFDeviceMode();

                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The b received data
        /// </summary>
        private bool bSimulationMode = true;

        /// <summary>
        /// Gets or sets a value indicating whether [received data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [received data]; otherwise, <c>false</c>.
        /// </value>
        public bool SimulationMode
        {
            get => this.bSimulationMode;
            set
            {
                this.bSimulationMode = value;

                FirePropertyChanged();
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        ///// <summary>
        ///// The b received data
        ///// </summary>
        //private bool bReceivedData = false;

        ///// <summary>
        ///// Gets or sets a value indicating whether [received data].
        ///// </summary>
        ///// <value>
        /////   <c>true</c> if [received data]; otherwise, <c>false</c>.
        ///// </value>
        //public bool ReceivedData
        //{
        //    get { return this.bReceivedData; }
        //    set
        //    {
        //        this.bReceivedData = value;

        //        SetBlink1();

        //        FirePropertyChanged();
        //    }
        //}


        ///// <summary>
        ///// The b is receive data UDP
        ///// </summary>
        //private bool bIsReceiveDataUDP = false;

        ///// <summary>
        ///// Gets or sets a value indicating whether this instance is receive data UDP.
        ///// </summary>
        ///// <value>
        /////   <c>true</c> if this instance is receive data UDP; otherwise, <c>false</c>.
        ///// </value>
        //public bool IsReceiveDataUDP
        //{
        //    get { return this.bIsReceiveDataUDP; }
        //    set
        //    {
        //        this.bIsReceiveDataUDP = value;

        //        if (this.bIsReceiveDataUDP == true)
        //        {
        //            StartUDPServer();
        //        }
        //        else
        //        {
        //            StopUDPServer();
        //        }

        //        FirePropertyChanged();
        //    }
        //}

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The b is tile loading
        /// </summary>
        private bool bIsTileLoading = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is tile loading.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is tile loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsTileLoading
        {
            get => this.bIsTileLoading;
            set
            {
                this.bIsTileLoading = value;

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The b is device moving mode
        /// </summary>
        private bool bIsDeviceMovingMode = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is device moving mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is device moving mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeviceMovingMode
        {
            get => this.bIsDeviceMovingMode;
            set
            {
                this.bIsDeviceMovingMode = value;

                this.mcMapControl.DragButton = this.bIsDeviceMovingMode ? MouseButton.Right : MouseButton.Left;

                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Gets or sets the map provider.
        /// </summary>
        /// <value>
        /// The map provider.
        /// </value>
        public GMapProvider MapProvider
        {
            get => this.mcMapControl.MapProvider;
            set
            {
                this.mcMapControl.MapProvider = value;
                // ReSharper disable PossibleUnintendedReferenceComparison
                this.mcMapControl.Manager.Mode = value != GMapProviders.EmptyProvider ? AccessMode.ServerAndCache : AccessMode.CacheOnly;
                // ReSharper restore PossibleUnintendedReferenceComparison

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// Gets or sets the zoom.
        /// </summary>
        /// <value>
        /// The zoom.
        /// </value>
        public double Zoom
        {
            get => this.mcMapControl.Zoom;
            set
            {
                this.mcMapControl.Zoom = value;

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        public double Latitude
        {
            get => this.mcMapControl.Position.Lat;
            set
            {
                this.mcMapControl.Position = new PointLatLng(value, this.mcMapControl.Position.Lng);

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        public double Longitude
        {
            get => this.mcMapControl.Position.Lng;
            set
            {
                this.mcMapControl.Position = new PointLatLng(this.mcMapControl.Position.Lat, value);

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [show center].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show center]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCenter
        {
            get => this.mcMapControl.ShowCenter;
            set
            {
                this.mcMapControl.ShowCenter = value;
                this.mcMapControl.ReloadMap();

                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Gets or sets the data grid visibility.
        /// </summary>
        /// <value>
        /// The data grid visibility.
        /// </value>
        public Visibility DataGridVisibility { get; set; } = Visibility.Visible;

        /// <summary>
        /// Gets or sets the information window visibility.
        /// </summary>
        /// <value>
        /// The information window visibility.
        /// </value>
        public Visibility InfoWindowVisibility { get; set; } = Visibility.Collapsed;


        /// <summary>
        /// The DVM current selected device
        /// </summary>
        private RFDeviceViewModel dvmCurrentSelectedDevice = null;

        /// <summary>
        /// Gets or sets the current selected device.
        /// </summary>
        /// <value>
        /// The current selected device.
        /// </value>
        public RFDeviceViewModel CurrentSelectedDevice
        {
            get => this.dvmCurrentSelectedDevice;
            set
            {
                this.dvmCurrentSelectedDevice = value;

                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The string current file
        /// </summary>
        private string strCurrentFile = null;

        /// <summary>
        /// Gets or sets the current file.
        /// </summary>
        /// <value>
        /// The current file.
        /// </value>
        public string CurrentFile
        {
            get => this.strCurrentFile;
            set
            {
                this.strCurrentFile = value;
                SetTitle();

                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The b synchronize map and grid
        /// </summary>
        private bool bSyncMapAndGrid = true;

        /// <summary>
        /// Gets or sets a value indicating whether [synchronize map and grid].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [synchronize map and grid]; otherwise, <c>false</c>.
        /// </value>
        public bool SyncMapAndGrid
        {
            get => this.bSyncMapAndGrid;
            set
            {
                this.bSyncMapAndGrid = value;

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The b use browser internal
        /// </summary>
        private bool bUseBrowserInternal = false;

        /// <summary>
        /// Gets or sets a value indicating whether [use browser internal].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use browser internal]; otherwise, <c>false</c>.
        /// </value>
        public bool UseBrowserInternal
        {
            get => this.bUseBrowserInternal;
            set
            {
                this.bUseBrowserInternal = value;

                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        ///// <summary>
        ///// Gets the UDP host.
        ///// </summary>
        ///// <value>
        ///// The UDP host.
        ///// </value>
        //public string UDPHost
        //{
        //    get { return this.settings.UDPHost; }
        //    set
        //    {
        //        this.settings.UDPHost = value;

        //        FirePropertyChanged();
        //    }
        //}


        ///// <summary>
        ///// Gets the UDP port.
        ///// </summary>
        ///// <value>
        ///// The UDP port.
        ///// </value>
        //public int UDPPort
        //{
        //    get { return this.settings.UDPPortSending; }
        //    set
        //    {
        //        this.settings.UDPPortSending = value;

        //        FirePropertyChanged();
        //    }
        //}


        ///// <summary>
        ///// Gets the UDP delay.
        ///// </summary>
        ///// <value>
        ///// The UDP delay.
        ///// </value>
        //public int UDPDelay
        //{
        //    get { return this.settings.UDPDelay; }
        //    set
        //    {
        //        this.settings.UDPDelay = value;

        //        FirePropertyChanged();
        //    }
        //}


        ///// <summary>
        ///// The string debug output
        ///// </summary>
        //private string strDebugOutput = null;

        ///// <summary>
        ///// Gets or sets the debug output.
        ///// </summary>
        ///// <value>
        ///// The debug output.
        ///// </value>
        //public string DebugOutput
        //{
        //    get { return this.strDebugOutput; }
        //    set
        //    {
        //        this.strDebugOutput = value;

        //        FirePropertyChanged();
        //    }
        //}

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Gets or sets the geo node collection.
        /// </summary>
        /// <value>
        /// The geo node collection.
        /// </value>
        public GeoNodeCollection GeoNodeCollection { get; set; }

        /// <summary>
        /// The LCV
        /// </summary>
        private readonly ListCollectionView lcvGeoNodes = null;

        /// <summary>
        /// Gets the current nodes.
        /// </summary>
        /// <value>
        /// The current nodes.
        /// </value>
        public int CurrentNodes
        {
            get { return this.lcvGeoNodes?.Count ?? 0; }
        }

        //---------------------------------------------------------------------


        #region GeoNodeFilter

        /// <summary>
        /// The gt filter
        /// </summary>
        private GeoTag gtGeoTagFilter = GeoTag.Place;

        /// <summary>
        /// Gets or sets the geo tag filter.
        /// </summary>
        /// <value>
        /// The geo tag filter.
        /// </value>
        public GeoTag GeoTagFilter
        {
            get => this.gtGeoTagFilter;
            set
            {
                this.gtGeoTagFilter = value;

                this.lcvGeoNodes.Refresh();

                FirePropertyChanged();
                FirePropertyChanged("CurrentNodes");
            }
        }


        /// <summary>
        /// The b use geo tag filter
        /// </summary>
        private bool bUseGeoTagFilter = false;

        /// <summary>
        /// Gets or sets a value indicating whether [use geo tag filter].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use geo tag filter]; otherwise, <c>false</c>.
        /// </value>
        public bool UseGeoTagFilter
        {
            get => this.bUseGeoTagFilter;
            set
            {
                this.bUseGeoTagFilter = value;

                this.lcvGeoNodes.Refresh();

                FirePropertyChanged();
                FirePropertyChanged("CurrentNodes");
            }
        }


        /// <summary>
        /// The string name filter
        /// </summary>
        private string strNameFilter = "";

        /// <summary>
        /// Gets or sets the name filter.
        /// </summary>
        /// <value>
        /// The name filter.
        /// </value>
        public string NameFilter
        {
            get => this.strNameFilter;
            set
            {
                this.strNameFilter = value;

                this.lcvGeoNodes.Refresh();

                FirePropertyChanged();
                FirePropertyChanged("CurrentNodes");
            }
        }


        /// <summary>
        /// The b use name filter
        /// </summary>
        private bool bUseNameFilter = false;

        /// <summary>
        /// Gets or sets a value indicating whether [use name filter].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use name filter]; otherwise, <c>false</c>.
        /// </value>
        public bool UseNameFilter
        {
            get => this.bUseNameFilter;
            set
            {
                this.bUseNameFilter = value;

                this.lcvGeoNodes.Refresh();

                FirePropertyChanged();
                FirePropertyChanged("CurrentNodes");
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The b started dalf
        /// </summary>
        private bool bStartedDALF = false;

        /// <summary>
        /// Gets or sets a value indicating whether [started dalf].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [started dalf]; otherwise, <c>false</c>.
        /// </value>
        public bool StartedDALF
        {
            get => this.bStartedDALF;
            set
            {
                if( value == true )
                {
                    // Nur wenn es erfolgreich gestartet werden konnte machen wir weiter ...
                    if( StartDALF() == false )
                    {
                        return;
                    }
                }
                else
                {
                    StopDALF();
                }

                this.bStartedDALF = value;
                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        #region RFDevice Filter

        /// <summary>
        /// The LCV
        /// </summary>
        private readonly ListCollectionView lcvRFDevices = null;

        /// <summary>
        /// The i identifier filter
        /// </summary>
        private int? iIdFilter = null;

        /// <summary>
        /// Gets or sets the identifier filter.
        /// </summary>
        /// <value>
        /// The identifier filter.
        /// </value>
        /// <remarks>We use a string so that we can provide a empty a string as integer NULL ...</remarks>
        public string IdFilter
        {
            get => this.iIdFilter != null ? this.iIdFilter.ToString() : "";
            set
            {
                if( string.IsNullOrEmpty(value) )
                {
                    this.iIdFilter = null;
                }
                else
                {
                    try
                    {

                        this.iIdFilter = int.Parse(value);
                    }
                    catch( Exception )
                    {
                        this.iIdFilter = null;
                    }
                }

                this.lcvRFDevices.Refresh();

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The b show receiver
        /// </summary>
        private bool bShowReceiver = true;

        /// <summary>
        /// Gets or sets a value indicating whether [show receiver].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show receiver]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowReceiver
        {
            get => this.bShowReceiver;
            set
            {
                this.bShowReceiver = value;

                this.lcvRFDevices.Refresh();

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The b show transmitter
        /// </summary>
        private bool bShowTransmitter = true;

        /// <summary>
        /// Gets or sets a value indicating whether [show transmitter].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show transmitter]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowTransmitter
        {
            get => this.bShowTransmitter;
            set
            {
                this.bShowTransmitter = value;

                this.lcvRFDevices.Refresh();

                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The RTT rx tx type filter
        /// </summary>
        private RxTxType rttRxTxTypeFilter = RxTxType.Empty;

        /// <summary>
        /// Gets or sets the rx tx type filter.
        /// </summary>
        /// <value>
        /// The rx tx type filter.
        /// </value>
        public RxTxType RxTxTypeFilter
        {
            get => this.rttRxTxTypeFilter;
            set
            {
                this.rttRxTxTypeFilter = value;

                this.lcvRFDevices.Refresh();

                FirePropertyChanged();
            }
        }
        #endregion

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The quick commands
        /// </summary>
        // ReSharper disable CollectionNeverQueried.Global
        public ObservableStringCollection QuickCommands { get; set; } = new ObservableStringCollection();
        // ReSharper restore CollectionNeverQueried.Global


        /// <summary>
        /// Gets or sets the validation result.
        /// </summary>
        /// <value>
        /// The validation result.
        /// </value>
        public ValidationResultViewModelList ValidationResult { get; set; } = new ValidationResultViewModelList();

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Gets the meta information.
        /// </summary>
        /// <value>
        /// The meta information.
        /// </value>
        public ScenarioMetaInformation MetaInformation { get; } = new ScenarioMetaInformation();

    } // end public partial class MainWindow
}

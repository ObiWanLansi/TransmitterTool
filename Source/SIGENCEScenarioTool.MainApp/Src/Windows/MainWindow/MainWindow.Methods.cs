﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using GMap.NET;
using GMap.NET.MapProviders;

using Markdig;

using NetTopologySuite.Densify;
using NetTopologySuite.Geometries;

using SIGENCEScenarioTool.Dialogs.RFDevice;
using SIGENCEScenarioTool.Dialogs.Scripting;
using SIGENCEScenarioTool.Dialogs.Settings;
using SIGENCEScenarioTool.Dialogs.Simulation;
using SIGENCEScenarioTool.Extensions;
using SIGENCEScenarioTool.Models;
using SIGENCEScenarioTool.Models.RxTxTypes;
using SIGENCEScenarioTool.Services.Help;
using SIGENCEScenarioTool.Tools;
using SIGENCEScenarioTool.ViewModels;



namespace SIGENCEScenarioTool.Windows.MainWindow
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Resets this instance.
        /// </summary>
        private void Reset()
        {
            this.Cursor = Cursors.Wait;

            this.CurrentFile = null;

            this.RFDeviceViewModelCollection.Clear();
            this.ValidationResult.Clear();

            this.mcMapControl.Markers.Clear();
            this.MetaInformation.Clear();

            // Ist ja noch nix passiert ...
            this.DescriptionMarkdownChanged = false;
            this.DescriptionStylesheetChanged = false;

            GC.WaitForPendingFinalizers();
            GC.Collect();

            this.Cursor = Cursors.Arrow;
        }


        /// <summary>
        /// Sets the title.
        /// </summary>
        private void SetTitle()
        {
            this.Title = $"{Tool.ProductTitle} (Version {Tool.Version}){( this.CurrentFile != null ? $" [{new FileInfo(this.CurrentFile).Name}]" : "" )}";
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Opens the cheat sheet.
        /// </summary>
        private void OpenCheatSheet()
        {
            new HelpWindow.HelpWindow().Show();
        }


        /// <summary>
        /// Creates the screenshot.
        /// </summary>
        private void CreateScreenshot()
        {
            try
            {
                if( this.CurrentFile != null )
                {
                    this.sfdSaveScreenshot.FileName = new FileInfo(this.CurrentFile).Name;
                }

                if( this.sfdSaveScreenshot.ShowDialog() == true )
                {
                    System.Windows.Media.Imaging.BitmapSource screenshot = Tools.Windows.GetWPFScreenshot(this.mcMapControl);

                    Tools.Windows.SaveWPFScreenshot(screenshot, this.sfdSaveScreenshot.FileName);

                    Tools.Windows.OpenWithDefaultApplication(this.sfdSaveScreenshot.FileName);
                }
            }
            catch( Exception ex )
            {
                MB.Error(ex);
            }
        }


        /// <summary>
        /// Opens the script editor.
        /// </summary>
        private void OpenScriptEditor()
        {
            ScriptingDialog sd = new ScriptingDialog(this);
            sd.ShowDialog();
            sd = null;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Updates the file history.
        /// </summary>
        private void UpdateFileHistory()
        {
            this.miRecentFiles.BeginInit();
            this.miRecentFiles.Items.Clear();

            foreach( string strFileName in Properties.Settings.Default.LastOpenFiles )
            {
                FileInfo fi = new FileInfo(strFileName);

                MenuItem mi = new MenuItem
                {
                    Header = fi.Name,
                    Foreground = fi.Exists == false ? Brushes.Red : SystemColors.MenuTextBrush,
                    IsEnabled = fi.Exists,
                    Tag = fi.FullName,
                    Icon = FindResource("OPEN"),
                    ToolTip = fi.FullName
                };
                mi.Click += MenuItem_FileHistory_Click;

                this.miRecentFiles.Items.Add(mi);
            }

            this.miRecentFiles.EndInit();
        }


        /// <summary>
        /// Adds the file history.
        /// </summary>
        /// <param name="strFilename">The string filename.</param>
        private void AddFileHistory( string strFilename )
        {
            StringCollection sc = Properties.Settings.Default.LastOpenFiles;

            //switch(fh)
            //{
            //    case FileHistory.MainFile:
            //        sc = Properties.Settings.Default.LastOpenFiles;
            //        break;

            //    case FileHistory.TemplateFile:
            //        sc = Properties.Settings.Default.LastOpenTemplates;
            //        break;

            //    case FileHistory.StyleFile:
            //        sc = Properties.Settings.Default.LastOpenStyles;
            //        break;
            //}

            if( sc.Contains(strFilename) == true )
            {
                sc.Remove(strFilename);
            }

            sc.Insert(0, strFilename);

            if( sc.Count > Properties.Settings.Default.MaxLastItems )
            {
                sc.RemoveAt((int) Properties.Settings.Default.MaxLastItems);
            }

            UpdateFileHistory();

            // Die geänderten Einstellungen müssen jedesmal von Hand gespeichert werden, dies übernimmt der Framwork nicht automatisch ...
            Properties.Settings.Default.Save();
        }


        ///// <summary>
        ///// Inserts the HTML snippet.
        ///// </summary>
        ///// <param name="strSnippetId">The string snippet identifier.</param>
        //private void InsertHtmlSnippet(string strSnippetId)
        //{
        //    string strSnippet = null;

        //    string GetDefaultTag(string tag) => string.Format("<{0}></{0}>", tag);

        //    strSnippetId = strSnippetId.ToLower();

        //    switch (strSnippetId)
        //    {
        //        case "table":
        //            strSnippet = "<table border=\"1\">\n<tr><th>Column1</th><th>Column2</th></tr>\n<tr><td></td><td></td></tr>\n</table>";
        //            break;

        //        case "br":
        //            strSnippet = "<br />";
        //            break;

        //        case "hr":
        //            strSnippet = "<hr />";
        //            break;

        //        case "image":
        //            strSnippet = "<image src=\"your_url\" />";
        //            break;

        //        case "link":
        //            strSnippet = "<a href=\"your_url\">Link Text</a>";
        //            break;

        //        default:
        //            strSnippet = GetDefaultTag(strSnippetId);
        //            break;
        //    }

        //    this.tecScenarioDescription.ActiveTextAreaControl.TextArea.InsertString(strSnippet);
        //}

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Toggles the move rf device.
        /// </summary>
        private void ToggleMoveRFDevice()
        {
            this.IsDeviceMovingMode = !this.IsDeviceMovingMode;
        }


        /// <summary>
        /// Toggles the dalf.
        /// </summary>
        private void ToggleDALF()
        {
            this.StartedDALF = !this.StartedDALF;
        }


        /// <summary>
        /// Toogles the synchronize map and grid.
        /// </summary>
        private void ToggleSyncMapAndGrid()
        {
            this.SyncMapAndGrid = !this.SyncMapAndGrid;
        }

        /// <summary>
        /// Views the validation results.
        /// </summary>
        private void ViewValidationResults()
        {
            this.tiValidation.IsSelected = true;
        }


        /// <summary>
        /// Views the device map.
        /// </summary>
        private void ViewDeviceMap()
        {
            this.tiMap.IsSelected = true;
        }


        /// <summary>
        /// Views the description hypertext.
        /// </summary>
        private void ViewDescriptionHypertext()
        {
            this.tiMetaInformation.IsSelected = true;
            this.tiViewWebbrowser.IsSelected = true;
        }


        /// <summary>
        /// Edits the description markdown.
        /// </summary>
        private void EditDescriptionMarkdown()
        {
            this.tiMetaInformation.IsSelected = true;
            this.tiEditDescription.IsSelected = true;

            // Warum auch immer, aber es hat nicht geholfen.
            //this.tecDescription.ActiveTextAreaControl.TextArea.Focus();
        }


        /// <summary>
        /// Edits the description stylesheet.
        /// </summary>
        private void EditDescriptionStylesheet()
        {
            this.tiMetaInformation.IsSelected = true;
            this.tiEditStylesheet.IsSelected = true;

            // Warum auch immer, aber es hat nicht geholfen.
            // Warum auch immer, aber es hat nicht geholfen.
            //this.tecStyleSheet.ActiveTextAreaControl.Focus();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Edit the settings.
        /// </summary>
        private void EditSettings()
        {
            SettingsDialog dlg = new SettingsDialog();
            dlg.ShowDialog();
            dlg = null;
        }


        /// <summary>
        /// Imports the settings.
        /// </summary>
        /// <param name="strFileName">Name of the string file.</param>
        private void ImportSettings( string strFileName )
        {
            try
            {
                string strContent = File.ReadAllText(strFileName);

                this.settings = Tool.GetObjectFromFlatXmlString<Properties.Settings>(strContent);

                this.settings.Save();
            }
            catch( Exception ex )
            {
                MB.Error(ex);
            }
        }


        /// <summary>
        /// Imports the settings.
        /// </summary>
        private void ImportSettings()
        {
            if( this.ofdImportSettings.ShowDialog() == true )
            {
                ImportSettings(this.ofdImportSettings.FileName);
            }
        }


        /// <summary>
        /// Exports the settings.
        /// </summary>
        /// <param name="strFileName">Name of the string file.</param>
        private void ExportSettings( string strFileName )
        {
            try
            {
                string strContent = Tool.GetFlatXmlStringFromObject(this.settings);

                File.WriteAllText(strFileName, strContent, Encoding.GetEncoding("ISO-8859-1"));
            }
            catch( Exception ex )
            {
                MB.Error(ex);
            }
        }


        /// <summary>
        /// Exports the settings.
        /// </summary>
        private void ExportSettings()
        {
            if( this.sfdExportSettings.ShowDialog() == true )
            {
                ExportSettings(this.sfdExportSettings.FileName);
            }
        }


        /// <summary>
        /// Resets the settings.
        /// </summary>
        private void ResetSettings()
        {
            this.settings.Reset();
            this.settings.Save();

            if( MessageBox.Show("The Settings Are Resetted!.\nPlease Restart The Application!\nWould You Restart Now?", Tool.ProductTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes )
            {
                Close();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Opens the scenario simulation player.
        /// </summary>
        private void OpenScenarioSimulationPlayer()
        {
            SimulationDialog sd = new SimulationDialog(this.RFDeviceViewModelCollection, this.mcMapControl);
            sd.ShowDialog();
            sd = null;

            // Wir müssen uns die Marker wieder von dem anderen MapControl zurück klauen ...
            foreach( RFDeviceViewModel device in this.RFDeviceViewModelCollection )
            {
                this.mcMapControl.Markers.Add(device.Marker);
            }
        }


        /// <summary>
        /// Opens the device edit dialog.
        /// </summary>
        //private void OpenDeviceEditDialog(RFDevice device)
        private void OpenDeviceEditDialog( RFDeviceViewModel device )
        {
            RFDeviceEditDialog ded = new RFDeviceEditDialog(device);

            if( ded.ShowDialog() ?? false == true )
            {
                DeleteRFDevice(device);
                AddRFDevice(ded.Device);
            }

            ded = null;
        }


        /// <summary>
        /// Updates the scenario description.
        /// </summary>
        private void UpdateScenarioDescriptionMarkdown()
        {
            if( this.MetaInformation.DescriptionMarkdown.IsNotEmpty() )
            {
                this.sbHtml.Clear();
                this.sbMarkdown.Clear();

                try
                {
                    string strHeaderWithCSS = HEADER.Replace("$STYLE$", this.MetaInformation.DescriptionStylesheet);

                    string [] strLines = this.tecDescriptionMarkdown.Text.Split(new [] { Environment.NewLine }, StringSplitOptions.None);

                    foreach( string strLine in strLines )
                    {
                        if( strLine.StartsWith(GenericFoldingStrategy.REGION_START) || strLine.StartsWith(GenericFoldingStrategy.REGION_END) )
                        {
                            continue;
                        }

                        this.sbMarkdown.AppendLine(strLine);
                    }

                    this.sbHtml.AppendLine(strHeaderWithCSS);
                    //sbHtml.AppendLine(Markdown.ToHtml(this.tecDescriptionMarkdown.Text, MAPI));
                    this.sbHtml.AppendLine(Markdown.ToHtml(this.sbMarkdown.ToString(), MAPI));
                    this.sbHtml.AppendLine(FOOTER);

                    //this.Dispatcher.Invoke(() => this.wbWebBrowser.NavigateToString(sbHtml.ToString()));
                    this.wbWebBrowser.NavigateToString(this.sbHtml.ToString());
                    //this.wbWebBrowser.Refresh();
                }
                catch( Exception ex )
                {
                    this.wbWebBrowser.NavigateToString($"<h1>{ex.Message}</h1>");
                }
            }
            else
            {
                this.wbWebBrowser.NavigateToString("<html/>");
            }
        }


        /// <summary>
        /// Initializes the meta information.
        /// </summary>
        private void InitMetaInformation()
        {
            if( this.MetaInformation.Version.IsEmpty() )
            {
                this.MetaInformation.Version = "1.0";
            }

            if( this.MetaInformation.ApplicationContext.IsEmpty() )
            {
                this.MetaInformation.ApplicationContext = "Please Type The Name Of The ApplicationContext Here ...";
            }

            if( this.MetaInformation.ContactPerson.IsEmpty() )
            {
                this.MetaInformation.ContactPerson = Environment.UserName;
            }

            if( this.MetaInformation.DescriptionMarkdown.IsEmpty() )
            {
                this.MetaInformation.DescriptionMarkdown = File.ReadAllText($"{Tool.StartupPath}\\ScenarioDescription.md");
            }

            if( this.MetaInformation.DescriptionStylesheet.IsEmpty() )
            {
                this.MetaInformation.DescriptionStylesheet = File.ReadAllText($"{Tool.StartupPath}\\ScenarioDescription.css");
            }

            // Ist ja noch nix passiert ...
            this.DescriptionMarkdownChanged = false;
            this.DescriptionStylesheetChanged = false;

            this.tiMetaInformation.IsSelected = true;
        }


        /// <summary>
        /// Creates the example rf devices.
        /// </summary>
        private void CreateExampleRFDevices()
        {
            AddRFDevice(new RFDevice
            {
                Id = 42,
                DeviceSource = DeviceSource.User,
                Latitude = 47.666557,
                Longitude = 9.386941,
                AntennaType = AntennaType.HyperLog60100,
                RxTxType = RxTxTypes.FMBroadcast,
                CenterFrequency_Hz = 90_000_000,
                Bandwidth_Hz = 30_000
            });

            AddRFDevice(new RFDevice
            {
                Id = -42,
                DeviceSource = DeviceSource.User,
                Latitude = 47.666100,
                Longitude = 9.172648,
                AntennaType = AntennaType.OmniDirectional,
                RxTxType = RxTxTypes.IdealSDR,
                CenterFrequency_Hz = 90_000_000,
                Bandwidth_Hz = 30_000
            });
        }


        ///// <summary>
        ///// Creates the scenario report.
        ///// </summary>
        //[Conditional("DEBUG")]
        private void CreateScenarioReport()
        {
            if( string.IsNullOrEmpty(this.CurrentFile) )
            {
                MB.Information("The scenario has not been saved yet.\nSave it first and then try again.");
                return;
            }

            this.Cursor = Cursors.Wait;

            FileInfo fiCurrentFile = new FileInfo(this.CurrentFile);

            string strScenarioName = fiCurrentFile.GetFilenameWithoutExtension();

            string strOutputDirectory = $"{fiCurrentFile.DirectoryName}\\{strScenarioName}.Report";
            if( Directory.Exists(strOutputDirectory) == false )
            {
                Directory.CreateDirectory(strOutputDirectory);
            }

            string strOutputFilenameMarkdown = $"{strOutputDirectory}\\{strScenarioName}.md";

            StringBuilder sb = new StringBuilder(8192);

            //-----------------------------------------------------------------

            sb.AppendLine($"# Scenario {fiCurrentFile.GetFilenameWithoutExtension()}");
            sb.AppendLine();

            //1. MetaInformation
            sb.AppendLine("## About This Scenario");
            sb.AppendLine();
            sb.AppendLine("|Property|Value|");
            sb.AppendLine("|--------|-----|");
            sb.AppendLine($"|Version|{this.MetaInformation.Version}|");
            sb.AppendLine($"|Application Context|{this.MetaInformation.ApplicationContext}|");
            sb.AppendLine($"|Contact Person|{this.MetaInformation.ContactPerson}|");
            if( this.MetaInformation.DescriptionMarkdown.IsNotEmpty() )
            {
                sb.AppendLine($"|User Description|[ScenarioDescription.md](./ScenarioDescription.md)|");
                string strOutputUserDescription = $"{strOutputDirectory}\\ScenarioDescription.md";
                File.WriteAllText(strOutputUserDescription, this.MetaInformation.DescriptionMarkdown, Encoding.UTF8);
            }
            sb.AppendLine();
            sb.AppendLine("---");

            //2. Devices
            sb.AppendLine();
            sb.AppendLine("## RFDevice List");
            sb.AppendLine();
            if( this.RFDeviceViewModelCollection != null && this.RFDeviceViewModelCollection.Count > 0 )
            {
                sb.AppendLine("|  Id|DeviceSource|StartTime|Name|Latitude|Longitude|Altitude|Roll|Pitch|Yaw|RxTxType|AntennaType|CenterFrequency|Bandwidth|Gain|SignalToNoiseRatio|");
                sb.AppendLine("|---:|:-----------|:-------:|:---|-------:|--------:|-------:|---:|----:|--:|:-------|:----------|--------------:|--------:|---:|-----------------:|");

                foreach( RFDeviceViewModel dev in from device in this.RFDeviceViewModelCollection orderby device.Id, device.StartTime select device )
                {
                    sb.AppendLine($"|{dev.Id}|{dev.DeviceSource}|{dev.StartTime}|{dev.Name}|{dev.HumanLatitude}|{dev.HumanLongitude}|{dev.HumanAltitude}|{dev.Roll}|{dev.Pitch}|{dev.Yaw}|{dev.RxTxType}|{dev.AntennaType}|{dev.HumanCenterFrequency}|{dev.HumanBandwidth}|{dev.HumanGain}|{dev.HumanSignalToNoiseRatio}|");
                }
            }
            sb.AppendLine();
            sb.AppendLine("---");

            // 3. Validation Results
            ExecuteValidateScenario();
            sb.AppendLine();
            sb.AppendLine("## Validation Results");
            sb.AppendLine();
            if( this.ValidationResult != null && this.ValidationResult.Count > 0 )
            {
                List<Models.Validation.ValidationResult> lResults = ( from result in this.ValidationResult select result.Result ).ToList();
                sb.AppendLine(lResults.Table());
            }
            sb.AppendLine("---");

            //4. Screenshot
            sb.AppendLine();
            sb.AppendLine("## Screenshot");
            sb.AppendLine();

            try
            {
                string strOutputFilenameScreenshot = $"{strOutputDirectory}\\ScenarioScreenshot.png";
                System.Windows.Media.Imaging.BitmapSource screenshot = Tools.Windows.GetWPFScreenshot(this.mcMapControl);
                Tools.Windows.SaveWPFScreenshot(screenshot, strOutputFilenameScreenshot);

                sb.AppendLine("![ScenarioScreenshot](./ScenarioScreenshot.png)");
            }
            catch( Exception ex )
            {
                sb.AppendLine($"**{ex.Message}**");
                MB.Error(ex);
            }
            //-----------------------------------------------------------------

            File.WriteAllText(strOutputFilenameMarkdown, sb.ToString(), Encoding.UTF8);

            Tools.Windows.OpenWithDefaultApplication(strOutputFilenameMarkdown);

            this.Cursor = Cursors.Arrow;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Restores the initial map values.
        /// </summary>
        private void RestoreInitialMapValues()
        {
            this.mcMapControl.Position = new PointLatLng(this.settings.InitialLatitude, this.settings.InitialLongitude);
            this.mcMapControl.Zoom = this.settings.InitialZoom;

            GMapProvider mapprovider = GetProviderFromString(this.settings.InitialMap);

            this.mcMapControl.MapProvider = mapprovider;
            this.cbMapProvider.SelectedItem = mapprovider;
        }


        /// <summary>
        /// Saves the initial map values.
        /// </summary>
        private void SaveInitialMapValues()
        {
            PointLatLng pll = this.mcMapControl.Position;
            this.settings.InitialLatitude = pll.Lat;
            this.settings.InitialLongitude = pll.Lng;

            this.settings.InitialZoom = (uint) this.mcMapControl.Zoom;
            this.settings.InitialMap = this.mcMapControl.MapProvider.ToString();

            this.settings.Save();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Gets the provider from string.
        /// </summary>
        /// <param name="strMapProvider">The string map provider.</param>
        /// <returns></returns>
        public static GMapProvider GetProviderFromString( string strMapProvider )
        {
            foreach( GMapProvider mp in GMapProviders.List )
            {
                if( mp.Name == strMapProvider )
                {
                    return mp;
                }
            }

            return GMapProviders.GoogleMap;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Resets the dalf.
        /// </summary>
        private void ResetDALF()
        {
            if( this.mrDALF != null )
            {
                if( this.mcMapControl.Markers.Contains(this.mrDALF) )
                {
                    this.mcMapControl.Markers.Remove(this.mrDALF);

                }

                this.mrDALF = null;
            }

            this.dvmLastSelectedDevice = null;
        }


        /// <summary>
        /// Starts the dalf.
        /// </summary>
        /// <returns></returns>
        private bool StartDALF()
        {
            ResetDALF();

            //if (this.mcMapControl.Markers.Contains(this.mrDALF))
            //{
            //    if (MessageBox.Show("Should The Existing Line Be Continued Or A New One Started?", Tool.ProductTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            //    {
            //        ResetDALF();
            //    }
            //}

            if( this.dvmLastSelectedDevice == null )
            {
                if( this.dgRFDevices.SelectedItems.Count != 1 )
                {
                    MB.Information("There Are No One Or More Than One RFDevice Selected In The DataGrid!");
                    return false;
                }

                RFDevice selectedDevice = ( this.dgRFDevices.SelectedItems [0] as RFDeviceViewModel ).RFDevice;

                if( selectedDevice.Id == 0 )
                {
                    MB.Information("The Reference Transmitter Is Not Good For DALF!");
                    return false;
                }

                if( selectedDevice.DeviceSource == DeviceSource.Automatic )
                {
                    MB.Information("The DeviceSource Of The Current Device Is Automatic. That's Not Good For Copying The Device!");
                    return false;
                }

                this.dvmLastSelectedDevice = selectedDevice;
            }

            this.mcMapControl.Cursor = Cursors.Cross;
            return true;
        }


        /// <summary>
        /// Stops the dalf.
        /// </summary>
        private void StopDALF()
        {
            this.mcMapControl.Cursor = Cursors.Arrow;

            if( this.mrDALF != null && this.mrDALF.Points.Count > 0 )
            {
                // Erst die alten löschen ...
                DeleteRFDevices(device => device.Id == this.dvmLastSelectedDevice.Id && device.DeviceSource == DeviceSource.Automatic);

                if( this.mrDALF.Points.Count == 2 )
                {
                    PointLatLng pllStart = this.mrDALF.Points [0];
                    PointLatLng pllEnd = this.mrDALF.Points [1];

                    Coordinate cStart = pllStart.ToCoordinate();
                    Coordinate cEnd = pllEnd.ToCoordinate();

                    LineString ls = new LineString(new [] { cStart, cEnd });

                    float fDensifyInKM = Properties.Settings.Default.DensifyInMeters / 100_000;

                    NetTopologySuite.Geometries.Geometry result = Densifier.Densify(ls, fDensifyInKM);

                    if( result is LineString lsDensified )
                    {
                        int iCounter = -1;
                        foreach( Coordinate c in lsDensified.Coordinates )
                        {
                            iCounter++;

                            // Den ersten müssen wir überspringen da es das Original selbst noch ist welches wir natürlich nicht zweimal brauchen ...    
                            if( iCounter == 0 )
                            {
                                continue;
                            }

                            PointLatLng pos = c.ToPointLatLng();

                            RFDevice device = this.dvmLastSelectedDevice.Clone();
                            device.PrimaryKey = Guid.NewGuid();
                            device.DeviceSource = DeviceSource.Automatic;
                            device.Latitude = pos.Lat;
                            device.Longitude = pos.Lng;
                            device.StartTime = this.settings.DeviceCopyTimeAddValue * iCounter;

                            AddRFDevice(device);
                        }

#if DEBUG
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Total Distance     : {0} Km", ( cStart.Distance(cEnd) * 100 ));
                        sb.AppendLine("Densify Settings   : {0} m", Properties.Settings.Default.DensifyInMeters);
                        sb.AppendLine("Densify Point Count: {0}", lsDensified.Coordinates.Length);
                        MB.Information(sb.ToString());
#endif
                    }
                }
                else
                {
                    int iCounter = -1;

                    foreach( PointLatLng pos in this.mrDALF.Points )
                    {
                        iCounter++;

                        // Den ersten müssen wir überspringen da es das Original selbst noch ist welches wir natürlich nicht zweimal brauchen ...    
                        if( iCounter == 0 )
                        {
                            continue;
                        }

                        RFDevice device = this.dvmLastSelectedDevice.Clone();

                        device.PrimaryKey = Guid.NewGuid();
                        device.DeviceSource = DeviceSource.Automatic;
                        device.Latitude = pos.Lat;
                        device.Longitude = pos.Lng;
                        device.StartTime = this.settings.DeviceCopyTimeAddValue * iCounter;

                        AddRFDevice(device);
                    }
                }
            }

            ResetDALF();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Marks the devices with the same value.
        /// </summary>
        private void MarkDevicesWithTheSameValue()
        {
            try
            {
                Tuple<PropertyInfo, object> tSource = this.dgRFDevices.GetFocusedItem();

                if( tSource == null )
                {
                    MB.Warning("No Cell Is Selected Or Focused!");
                    return;
                }

                if( tSource.Item1 == null )
                {
                    MB.Warning("Can't Detect The PropertyType Of This Cell!");
                    return;
                }

                if( tSource.Item2 == null )
                {
                    MB.Warning("Can't Get The Value Of This Cell!");
                    return;
                }

                this.dgRFDevices.SelectedItems.Clear();

                foreach( object item in this.dgRFDevices.Items )
                {
                    if( item is RFDeviceViewModel )
                    {
                        RFDeviceViewModel dvm = item as RFDeviceViewModel;

                        object oValue = tSource.Item1.GetValue(dvm);

                        if( tSource.Item2.Equals(oValue) )
                        {
                            this.dgRFDevices.SelectedItems.Add(dvm);
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                MB.Error(ex);
            }
        }


        /// <summary>
        /// Resets all device filter.
        /// </summary>
        private void ResetAllDeviceFilter()
        {
            if( this.bDataGridInEditMode == true )
            {
                MB.Warning("You Are In The Edit Mode From The DataGrid.\nPlease End The Editing And Try Again.");
                return;
            }

            // Hier werden die Felder gesetzt damit die Liste nicht jedesmal aktualisiert wird was wei den Properties passieren würde ...

            this.iIdFilter = null;
            this.bShowTransmitter = true;
            this.bShowReceiver = true;
            this.rttRxTxTypeFilter = RxTxType.Empty;

            // Jetzt nur einmal die Liste aktualisieren ...
            this.lcvRFDevices.Refresh();

            // Damit sich die UI jetzt natürlich den geänderten Werten wieder anpasst müssen wir allerdings die Events noch feuern ...

            // ReSharper disable ExplicitCallerInfoArgument
            FirePropertyChanged("IdFilter");
            FirePropertyChanged("ShowReceiver");
            FirePropertyChanged("ShowTransmitter");
            FirePropertyChanged("RxTxTypeFilter");
            // ReSharper restore ExplicitCallerInfoArgument
        }


        /// <summary>
        /// Determines whether [is wanted rf device] [the specified object].
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///   <c>true</c> if [is wanted rf device] [the specified object]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsWantedRFDevice( object obj )
        {
            if( obj == null || obj is RFDeviceViewModel == false )
            {
                return false;
            }

            RFDeviceViewModel device = (RFDeviceViewModel) obj;

            device.SetVisible(false);

            if( this.iIdFilter != null )
            {
                if( device.Id != this.iIdFilter )
                {
                    return false;
                }
            }

            if( this.ShowReceiver == false && device.DeviceType == DeviceType.Receiver )
            {
                return false;
            }

            if( this.ShowTransmitter == false && device.DeviceType == DeviceType.Transmitter )
            {
                return false;
            }

            if( this.rttRxTxTypeFilter != null && this.rttRxTxTypeFilter.Value != RxTxType.EmptyId )
            {
                // Wir müssen auf den Namen vergleichen da die ID mehrfach vorkommen kann (je nachdem ob Transmitter oder Receiver).
                if( this.rttRxTxTypeFilter.Name != device.RxTxType.Name )
                {
                    return false;
                }
            }

            device.SetVisible(true);

            return true;
        }


        ///// <summary>
        ///// Determines whether [is wanted geo node] [the specified object].
        ///// </summary>
        ///// <param name="obj">The object.</param>
        ///// <returns>
        /////   <c>true</c> if [is wanted geo node] [the specified object]; otherwise, <c>false</c>.
        ///// </returns>
        //private bool IsWantedGeoNode(object obj)
        //{
        //    if (obj == null || obj is GeoNode == false)
        //    {
        //        return false;
        //    }

        //    GeoNode gn = (GeoNode)obj;

        //    if (this.UseGeoTagFilter == true)
        //    {
        //        if (gn.Tag != this.GeoTagFilter)
        //        {
        //            return false;
        //        }
        //    }

        //    if (this.UseNameFilter == true && this.NameFilter.IsNotEmpty())
        //    {
        //        if (gn.Name.ToLower().Contains(this.NameFilter.ToLower()) == false)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Quicks the command action.
        /// </summary>
        /// <param name="strCommand">The string command.</param>
        private void QuickCommandAction( string strCommand )
        {
            if( strCommand.IsEmpty() )
            {
                return;
            }

            string [] strSplitted = strCommand.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if( strSplitted.Length > 0 )
            {
                string strMainCommand = strSplitted [0].ToLower();

                switch( strMainCommand )
                {
                    case "new":
                        NewFile();
                        break;

                    case "gc":
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        break;

                    case "rand":
                        if( strSplitted.Length > 1 )
                        {
                            try
                            {
                                int iCount = int.Parse(strSplitted [1]);
                                CreateRandomizedRFDevices(iCount);
                            }
                            catch( Exception ex )
                            {
                                MB.Error(ex);
                            }
                        }
                        break;


                    case "load":
                        if( strSplitted.Length > 1 )
                        {
                            LoadFile(strSplitted [1]);
                        }
                        else
                        {
                            LoadFile();
                        }
                        break;

                    case "save":
                        if( strSplitted.Length > 1 )
                        {
                            SaveFile(strSplitted [1]);
                        }
                        else
                        {
                            SaveFile();
                        }
                        break;

                    case "export":
                        ExportRFDevices();
                        break;

                    case "git":
                        OpenWebbrowser("https://github.com/ObiWanLansi/SIGENCE-Scenario-Tool");
                        break;
                    case "wiki":
                        OpenWebbrowser("https://de.wikipedia.org/wiki/Wikipedia:Hauptseite");
                        break;
                    case "go":
                        OpenWebbrowser("https://www.google.de/");
                        break;

                    case "xmp":
                        CreateExampleRFDevices();
                        break;

                    case "close":
                    case "exit":
                    case "quit":
                        Close();
                        break;

                    /*
                     * set "property" "value" --> --> Marked Rows
                     * export csv|xml|json  --> Marked Rows
                     * remove, delete --> Marked Rows
                     * goto lat/lon 
                     * zoom level
                     */

                    default:
                        MB.Warning("Unknown Command \"{0}\".", strMainCommand);
                        break;
                }
            }
        }


        /// <summary>
        /// Quicks the command action.
        /// </summary>
        private void QuickCommandAction()
        {
            string strCommand = this.cbQuickCommand.Text;

            if( strCommand.IsEmpty() )
            {
                return;
            }

            this.Cursor = Cursors.Wait;

            string [] strSplitted = strCommand.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if( strSplitted.Length > 0 )
            {
                foreach( string strSubCommand in strSplitted )
                {
                    QuickCommandAction(strSubCommand);
                }
            }

            this.QuickCommands.Add(strCommand);

            this.Cursor = Cursors.Arrow;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Executes the validate scenario.
        /// </summary>
        private void ExecuteValidateScenario()
        {
            this.ValidationResult.Clear();

            //-----------------------------------------------------------------

            if( this.RFDeviceViewModelCollection.Count == 0 )
            {
                this.ValidationResult.Add(new Models.Validation.ValidationResult(Servity.Information, "No Devices Are Configured.", "Scenario", "RFDevicesCollection", null));
                this.ValidationResult.EstimateCounts();

                // When we have no devices, we have nothing to validate ...
                return;
            }

            // Validation over the entire scenario, then only the individual RFDevices ...
            if( this.RFDeviceViewModelCollection.FirstOrDefault(d => d.Id == 0) == null )
            {
                this.ValidationResult.Add(new Models.Validation.ValidationResult(Servity.Warning, "No Reference Device Is Avaible.", "Scenario", "RFDevicesCollection", null));
            }

            // Check if all the PrimaryKeys of the RFDevices are Unique ...
            HashSet<Guid> hsGuids = new HashSet<Guid>();
            foreach( RFDevice device in this.RFDeviceViewModelCollection )
            {
                if( hsGuids.Contains(device.PrimaryKey) )
                {
                    this.ValidationResult.Add(new Models.Validation.ValidationResult(Servity.Error, "The PrimaryKey Is Not Unique!", device, "PrimaryKey", device.PrimaryKey));
                }
                else
                {
                    hsGuids.Add(device.PrimaryKey);
                }
            }

            //-----------------------------------------------------------------

            //TODO: Add Some Other Rules Here ...

            //-----------------------------------------------------------------

            foreach( RFDevice device in this.RFDeviceViewModelCollection )
            {
                this.ValidationResult.Add(device.Validate());
            }

            //-----------------------------------------------------------------

            this.ValidationResult.EstimateCounts();

            if( this.tiValidation.IsSelected == false )
            {
                this.tiValidation.IsSelected = true;
            }
        }


        /// <summary>
        /// Clears the scenario validation.
        /// </summary>
        private void ClearScenarioValidation()
        {
            this.ValidationResult.Clear();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Opens the webbrowser.
        /// </summary>
        /// <param name="strUrl">The string URL.</param>
        private void OpenWebbrowser( string strUrl )
        {
            if( this.UseBrowserInternal )
            {
                OpenWebbrowserInternal(strUrl);
            }
            else
            {
                OpenWebbrowserExternal(strUrl);
            }
        }


        /// <summary>
        /// Opens the webbrowser external.
        /// </summary>
        /// <param name="strUrl">The string URL.</param>
        private void OpenWebbrowserExternal( string strUrl )
        {
            Tools.Windows.OpenWebAdress(strUrl);
        }


        /// <summary>
        /// Opens the webbrowser internal.
        /// </summary>
        /// <param name="strUrl">The string URL.</param>
        /// <param name="strTitle">The string title.</param>
        private void OpenWebbrowserInternal( string strUrl, string strTitle = null )
        {
            StackPanel header = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            header.Children.Add(new Label
            {
                Content = FindResource("SERVER_EARTH")
            });

            header.Children.Add(new Label
            {
                Content = strTitle.IsNotEmpty() ? strTitle : "Webbrowser"
            });

            Button close = new Button
            {
                Content = "Ó",
                FontFamily = new FontFamily("Wingdings 2"),
                Width = 18,
                Height = 18,
                Foreground = Brushes.White,
                Background = Brushes.Red,
                Margin = new Thickness(3),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            close.Click += Webbrowser_Close_Click;

            header.Children.Add(close);

            WebBrowser webbrowser = new WebBrowser
            {
                Source = strUrl != null ? new Uri(strUrl) : null
            };

            //if (strTitle.IsEmpty())
            //{
            //    webbrowser.LoadCompleted += Webbrowser_LoadCompleted;
            //}

            this.tcTabControl.Items.Add(new TabItem
            {
                Header = header,
                Content = webbrowser,
                IsSelected = true
            });
        }


        /// <summary>
        /// Handles the Click event of the Webbrowser_Close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Webbrowser_Close_Click( object sender, RoutedEventArgs e )
        {
            this.tcTabControl.Items.Remove(( ( sender as Button ).Parent as StackPanel ).Parent);
        }


        ///// <summary>
        ///// Handles the LoadCompleted event of the Webbrowser control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="System.Windows.Navigation.NavigationEventArgs"/> instance containing the event data.</param>
        //private void Webbrowser_LoadCompleted(object sender, NavigationEventArgs e)
        //{
        //    ((((sender as WebBrowser).Parent as TabItem).Header as StackPanel).Children[1] as Label).Content = e.Uri != null ? e.Uri.AbsoluteUri : "Webbrowser";
        //}

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Checks the version.
        /// </summary>
        private void CheckVersion()
        {
            const string VERSION_URL = "https://raw.githubusercontent.com/ObiWanLansi/SIGENCE-Scenario-Tool/master/Source/LATESTVERSION";

            try
            {
                using( WebClient wc = new WebClient() )
                {
                    wc.Proxy = WebRequest.DefaultWebProxy;
                    wc.Proxy.Credentials = CredentialCache.DefaultCredentials;

                    int iGitHubVersion = int.Parse(wc.DownloadString(VERSION_URL));
                    int iLocalVersion = int.Parse(Tool.Version);

                    if( iGitHubVersion > iLocalVersion )
                    {
                        MB.Information($"There Is An Newer Version ({iGitHubVersion}) Avaible On GitHub.\nPlease Update The Tool.");
                    }
                    else
                    {
                        MB.Information("You Have The Newest Version Running.");
                    }
                }
            }
            catch( Exception ex )
            {
                MB.Error(ex);
            }
        }


        /// <summary>
        /// Toggles the information window.
        /// </summary>
        private void ToggleInfoWindow()
        {
            this.InfoWindowVisibility = this.InfoWindowVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            FirePropertyChanged("InfoWindowVisibility");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Opens the help.
        /// </summary>
        private void OpenHelp()
        {
            try
            {
                HelpServer.Port = Properties.Settings.Default.HelpPort;
                HelpServer.Path = Properties.Settings.Default.HelpPath;

                HelpServer.Instance.Start();

                IPEndPoint endpoint = HelpServer.Instance.EndPoint;

                // Wir geben unserem eigenenm Server natürlich erstmal ein bissel Zeit zum initialisieren
                // TODO: Sollte natürlich nur das erstmal von nöten sein, ist der Server bereits gestartet kann natürlich direkt los gelegt werden.
                Thread.Sleep(1000);

                Tools.Windows.OpenWebAdress($"http://{endpoint.Address}:{endpoint.Port}");
            }
            catch( Exception ex )
            {
                MB.Error(ex);
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        ///// <summary>
        ///// Toggles the data grid.
        ///// </summary>
        //private void ToggleDataGrid()
        //{
        //    this.DataGridVisibility = this.DataGridVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        //    FirePropertyChanged( "DataGridVisibility" );
        //}

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Creates the randomized RFDevices.
        /// </summary>
        /// <param name="iMaxCount">The i maximum count.</param>
        /// <param name="bEnsureRefDevice">if set to <c>true</c> [b ensure reference device].</param>
        private void CreateRandomizedRFDevices( int iMaxCount, bool bEnsureRefDevice = false )
        {
            this.Cursor = Cursors.Wait;

            foreach( RFDevice device in RFDeviceList.CreateRandomizedRFDeviceList(iMaxCount, this.mcMapControl.Position, bEnsureRefDevice) )
            {
                AddRFDevice(device);
            }

            this.Cursor = Cursors.Arrow;
        }

    } // end public partial class MainWindow
}

﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

using SIGENCEScenarioTool.Extensions;
using SIGENCEScenarioTool.Models;
using SIGENCEScenarioTool.Models.Validation;



namespace SIGENCEScenarioTool.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ValidationResultViewModel
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public ValidationResult Result { get; private set; }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp
        {
            get { return this.Result.Timestamp.Fmt_DD_MM_YYYY_HH_MM_SS(); }
        }


        /// <summary>
        /// Gets the servity.
        /// </summary>
        /// <value>
        /// The servity.
        /// </value>
        public Servity Servity
        {
            get { return this.Result.Servity; }
        }


        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get { return this.Result.Message; }
        }


        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source
        {
            get { return this.Result.Source != null ? this.Result.Source.ToString() : "Unknown Source."; }
        }


        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public string PropertyName
        {
            get { return this.Result.PropertyName; }
        }


        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get { return this.Result.Value; } }


        /// <summary>
        /// Gets the servity foreground.
        /// </summary>
        /// <value>
        /// The servity foreground.
        /// </value>
        public Brush ServityForeground
        {
            get
            {
                switch (this.Servity)
                {
                    case Servity.Information:
                        return Brushes.Blue;

                    case Servity.Warning:
                        return Brushes.Orange;

                    case Servity.Error:
                        return Brushes.Red;

                    case Servity.Fatal:
                        return Brushes.DarkRed;
                }

                return Brushes.Black;
            }
        }


        /// <summary>
        /// Gets the servity font weight.
        /// </summary>
        /// <value>
        /// The servity font weight.
        /// </value>
        public FontWeight ServityFontWeight
        {
            get { return this.Servity == Servity.Fatal ? FontWeights.Bold : FontWeights.Normal; }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResultViewModel"/> class.
        /// </summary>
        /// <param name="vr">The vr.</param>
        public ValidationResultViewModel(ValidationResult vr)
        {
            this.Result = vr;
        }

    } // end sealed public class ValidationResultViewModel



    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection{SIGENCEScenarioTool.ViewModels.ValidationResultViewModel}" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public sealed class ValidationResultViewModelList : ObservableCollection<ValidationResultViewModel>, INotifyPropertyChanged
    {
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        /// <summary>
        /// Tritt ein, wenn sich ein Eigenschaftswert ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

        /// <summary>
        /// Fires the property changed.
        /// </summary>
        /// <param name="strPropertyName">Name of the string property.</param>
        private void FirePropertyChanged([CallerMemberName]string strPropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(strPropertyName));
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Gets the empty.
        /// </summary>
        /// <value>
        /// The empty.
        /// </value>
        public static ValidationResultViewModelList Empty
        {
            get { return new ValidationResultViewModelList(); }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// The i information
        /// </summary>
        private int iInformation = 0;
        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>
        /// The information.
        /// </value>
        public int Information
        {
            get => this.iInformation;
            set
            {
                this.iInformation = value;
                FirePropertyChanged();
            }
        }

        /// <summary>
        /// The i warning
        /// </summary>
        private int iWarning = 0;
        /// <summary>
        /// Gets or sets the warning.
        /// </summary>
        /// <value>
        /// The warning.
        /// </value>
        public int Warning
        {
            get => this.iWarning;
            set
            {
                this.iWarning = value;
                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The i error
        /// </summary>
        private int iError = 0;
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public int Error
        {
            get => this.iError;
            set
            {
                this.iError = value;
                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The i fatal
        /// </summary>
        private int iFatal = 0;
        /// <summary>
        /// Gets or sets the fatal.
        /// </summary>
        /// <value>
        /// The fatal.
        /// </value>
        public int Fatal
        {
            get => this.iFatal;
            set
            {
                this.iFatal = value;
                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The i count
        /// </summary>
        private int iCount = 0;
        /// <summary>
        /// Ruft die Anzahl der Elemente ab, die tatsächlich in der <see cref="T:System.Collections.ObjectModel.Collection`1" /> enthalten sind.
        /// </summary>
        public new int Count
        {
            get => this.iCount;
            set
            {
                this.iCount = value;
                FirePropertyChanged();
            }
        }


        /// <summary>
        /// The string last run
        /// </summary>
        private string strLastRun = "Never";
        /// <summary>
        /// Gets or sets the last run.
        /// </summary>
        /// <value>
        /// The last run.
        /// </value>
        public string LastRun
        {
            get => this.strLastRun;
            set
            {
                this.strLastRun = value;
                FirePropertyChanged();
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Adds the specified vr.
        /// </summary>
        /// <param name="vr">The vr.</param>
        public void Add(ValidationResult vr)
        {
            Add(new ValidationResultViewModel(vr));
        }


        /// <summary>
        /// Adds the specified VRL.
        /// </summary>
        /// <param name="vrl">The VRL.</param>
        public void Add(ValidationResultList vrl)
        {
            vrl.ForEach(Add);
        }


        /// <summary>
        /// Entfernt alle Elemente aus der <see cref="T:System.Collections.ObjectModel.Collection`1" />.
        /// </summary>
        public new void Clear()
        {
            base.Clear();

            this.Count = 0;
            this.Information = 0;
            this.Warning = 0;
            this.Error = 0;
            this.Fatal = 0;

            this.LastRun = "-";
        }


        /// <summary>
        /// Estimates the counts.
        /// </summary>
        public void EstimateCounts()
        {
            this.Count = base.Count;
            this.Information = this.Count(vrvm => vrvm.Servity == Servity.Information);
            this.Warning = this.Count(vrvm => vrvm.Servity == Servity.Warning);
            this.Error = this.Count(vrvm => vrvm.Servity == Servity.Error);
            this.Fatal = this.Count(vrvm => vrvm.Servity == Servity.Fatal);

            this.LastRun = DateTime.Now.Fmt_DD_MM_YYYY_HH_MM_SS();
        }

    } // end sealed public class ValidationResultViewModelList
}

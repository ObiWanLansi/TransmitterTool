﻿using System.Collections.Generic;



namespace SIGENCEScenarioTool.Models
{
    /// <summary>
    /// 
    /// </summary>
    sealed public class RFDeviceList : List<RFDevice>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RFDeviceList"/> class.
        /// </summary>
        public RFDeviceList()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RFDeviceList"/> class.
        /// </summary>
        /// <param name="iInitialSize">Initial size of the i.</param>
        public RFDeviceList( int iInitialSize ) : base( iInitialSize )
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public RFDeviceList( IEnumerable<RFDevice> collection )
            : base( collection )
        {
        }

    } // end sealed public class RFDeviceList
}

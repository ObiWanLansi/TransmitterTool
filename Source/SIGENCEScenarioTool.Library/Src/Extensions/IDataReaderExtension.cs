﻿using System;
using System.Data;

using GeoAPI.Geometries;

using SIGENCEScenarioTool.Tools;

using NTS = NetTopologySuite.Geometries;



namespace SIGENCEScenarioTool.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    static public class IDataReaderExtension
    {
        /// <summary>
        /// Gets the string or null.
        /// </summary>
        /// <param name="dbResult">The database result.</param>
        /// <param name="iColumnIndex">Index of the i column.</param>
        /// <returns></returns>
        static public string GetStringOrNull(this IDataReader dbResult, int iColumnIndex)
        {
            if (dbResult.IsDBNull(iColumnIndex) == false)
            {
                return dbResult.GetString(iColumnIndex);
            }

            return null;
        }


        /// <summary>
        /// Gets the int32 or null.
        /// </summary>
        /// <param name="dbResult">The database result.</param>
        /// <param name="iColumnIndex">Index of the i column.</param>
        /// <returns></returns>
        static public int? GetInt32OrNull(this IDataReader dbResult, int iColumnIndex)
        {
            if (dbResult.IsDBNull(iColumnIndex) == false)
            {
                return dbResult.GetInt32(iColumnIndex);
            }

            return null;
        }


        /// <summary>
        /// Gets the int64 or null.
        /// </summary>
        /// <param name="dbResult">The database result.</param>
        /// <param name="iColumnIndex">Index of the i column.</param>
        /// <returns></returns>
        static public long? GetInt64OrNull(this IDataReader dbResult, int iColumnIndex)
        {
            if (dbResult.IsDBNull(iColumnIndex) == false)
            {
                return dbResult.GetInt64(iColumnIndex);
            }

            return null;
        }


        /// <summary>
        /// Gets the date time or null.
        /// </summary>
        /// <param name="dbResult">The database result.</param>
        /// <param name="iColumnIndex">Index of the i column.</param>
        /// <returns></returns>
        static public DateTime? GetDateTimeOrNull(this IDataReader dbResult, int iColumnIndex)
        {
            if (dbResult.IsDBNull(iColumnIndex) == false)
            {
                return dbResult.GetDateTime(iColumnIndex);
            }

            return null;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbResult"></param>
        /// <param name="iColumnIndex"></param>
        /// <returns></returns>
        static public IGeometry GetGeometryFromWKB(this IDataReader dbResult, int iColumnIndex)
        {
            if (dbResult.IsDBNull(iColumnIndex) == false)
            {
                string strWKB = dbResult.GetString(iColumnIndex);

                if (strWKB.IsNotEmpty())
                {
                    return GeoHelper.StringToGeometry(strWKB);
                }
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbResult"></param>
        /// <param name="iColumnIndex"></param>
        /// <returns></returns>
        static public NTS.MultiPolygon GetMultiPolygonFromWKB(this IDataReader dbResult, int iColumnIndex)
        {
            return (NTS.MultiPolygon)GetGeometryFromWKB(dbResult, iColumnIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbResult"></param>
        /// <param name="iColumnIndex"></param>
        /// <returns></returns>
        static public NTS.Polygon GetPolygonFromWKB(this IDataReader dbResult, int iColumnIndex)
        {
            return (NTS.Polygon)GetGeometryFromWKB(dbResult, iColumnIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbResult"></param>
        /// <param name="iColumnIndex"></param>
        /// <returns></returns>
        static public NTS.LineString GetLineStringFromWKB(this IDataReader dbResult, int iColumnIndex)
        {
            return (NTS.LineString)GetGeometryFromWKB(dbResult, iColumnIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbResult"></param>
        /// <param name="iColumnIndex"></param>
        /// <returns></returns>
        static public NTS.Point GetPointFromWKB(this IDataReader dbResult, int iColumnIndex)
        {
            return (NTS.Point)GetGeometryFromWKB(dbResult, iColumnIndex);
        }

    } // end static public class IDataReaderExtension
}
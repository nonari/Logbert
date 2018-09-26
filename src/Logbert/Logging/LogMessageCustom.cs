﻿#region Copyright © 2017 Couchcoding

// File:    LogMessageCustom.cs
// Package: Logbert
// Project: Logbert
// 
// The MIT License (MIT)
// 
// Copyright (c) 2017 Couchcoding
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using Couchcoding.Logbert.Receiver.CustomReceiver;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Couchcoding.Logbert.Properties;
using System.Text;
using System.Windows.Forms;

using Couchcoding.Logbert.Helper;
using MoonSharp.Interpreter;

namespace Couchcoding.Logbert.Logging
{
  /// <summary>
  /// Implements a <see cref="LogMessage"/> class for custom logger messages.
  /// </summary>
  public sealed class LogMessageCustom : LogMessage
  {
    #region Private Fields

    /// <summary>
    /// The <see cref="Columnizer"/> instance to use for parsing-
    /// </summary>
    private readonly Columnizer mColumnizer;

    /// <summary>
    /// A dictionary that holds the parsed information.
    /// </summary>
    private readonly Dictionary<int, string> mParsedValue = new Dictionary<int, string>();

    /// <summary>
    /// Holds the <see cref="DateTime"/> the <see cref="LogMessage"/> is received. 
    /// </summary>
    private DateTime mTimestamp;

    /// <summary>
    /// Holds the message of the <see cref="LogMessage"/>.
    /// </summary>
    private string mMessage;

    /// <summary>
    /// Holds the <see cref="LogLevel"/> of the <see cref="LogMessage"/>.
    /// </summary>
    private LogLevel mLevel = LogLevel.Info;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the <see cref="DateTime"/> the <see cref="LogMessage"/> is received.
    /// </summary>
    public override DateTime Timestamp
    {
      get
      {
        return mTimestamp == DateTime.MaxValue
          ? DateTime.Now
          : mTimestamp;
      }
    }

    /// <summary>
    /// Gets the message of the <see cref="LogMessage"/>.
    /// </summary>
    public override string Message
    {
      get
      {
        return mMessage ?? string.Empty;
      }
    }

    /// <summary>
    /// Gets the <see cref="LogLevel"/> of the <see cref="LogMessage"/>.
    /// </summary>
    public override LogLevel Level
    {
      get
      {
        return mLevel;
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Parses the given <paramref name="data"/> for possible log message parts.
    /// </summary>
    /// <param name="data">The data string to parse.</param>
    /// <returns><c>True</c> on success, otherwise <c>false</c>.</returns>
    private bool ParseData(string data)
    {
      int scope = 0;
      Match multiGroup = null;
      for (int i = 0; i < mColumnizer.Columns.Count; ++i)
      {
        Match mtc;
        if (scope <= 0)
        {
          mtc = Regex.Match(data, mColumnizer.Columns[i].Expression);
        }
        else {
          mtc = multiGroup;
          scope--;
        }

        if (!mtc.Success && !mColumnizer.Columns[i].Optional)
        {
          return false;
        }

        int group = mColumnizer.Columns[0].GroupScope;

        mParsedValue[i] = mtc.Success
          ? mtc.Groups[group].ToString()
          : string.Empty;

        switch (mColumnizer.Columns[i].ColumnType)
        {
          case LogColumnType.Timestamp:
            if (!DateTime.TryParseExact(mParsedValue[i], mColumnizer.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out mTimestamp))
            {
              mTimestamp = DateTime.MinValue;
            }
            break;

          case LogColumnType.Level:
            foreach (KeyValuePair<LogLevel, string> logLevelMap in mColumnizer.LogLevelMapping)
            {
              if (string.IsNullOrEmpty(logLevelMap.Value))
              {
                // Ignore empty patterns.
                continue;
              }

              if (Regex.Match(mParsedValue[i], logLevelMap.Value).Success)
              {
                mLevel = logLevelMap.Key;
                break;
              }
            }

            break;

          case LogColumnType.Message:
            mMessage = mParsedValue[i];
            break;

          case LogColumnType.Multigroup:
            scope = mColumnizer.Columns[i].GroupScope;
            multiGroup = mtc;
            break;
            
        }
      }

      return true;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the value to display within a <see cref="DataGridView"/> at the fiven <paramref name="columnIndex"/>.
    /// </summary>
    /// <param name="columnIndex">The index of the column at the <see cref="DataGridView"/>.</param>
    /// <returns>The value to display at the given <paramref name="columnIndex"/>, or <c>null</c> if nothing to display.</returns>
    public override object GetValueForColumn(int columnIndex)
    {
      switch (columnIndex)
      {
        case 1:
          return mIndex;
        default:
          if (columnIndex > 0 && columnIndex - 2 < mParsedValue.Count)
          {
            if (mColumnizer.Columns[columnIndex - 2].ColumnType == LogColumnType.Timestamp)
            {
              // Special handling for the timestamp column. Maybe the timeshift value has to be added.
              return mTimestamp.Add(mTimeShiftOffset).ToString(
                  Settings.Default.TimestampFormat
                , CultureInfo.InvariantCulture);
            }
            if (mColumnizer.Columns[columnIndex - 2].ColumnType == LogColumnType.Multigroup)
            {
              return GetValueForColumn(columnIndex + 1);
            }

            return mParsedValue[columnIndex - 2];
          }
          break;
      }

      return string.Empty;
    }

    /// <summary>
    /// Exports the <see cref="LogMessage"/> with its data into a comma seperated line.
    /// </summary>
    /// <returns>The <see cref="LogMessage"/> with its data as a comma seperated line.</returns>
    public override string GetCsvLine()
    {
      StringBuilder sBuilder = new StringBuilder();

      sBuilder.AppendFormat("\"{0}\"", Index.ToCsv());

      if (mParsedValue.Values.Count > 0)
      {
        sBuilder.Append(",");
      }

      foreach (string parsedValue in mParsedValue.Values)
      {
        sBuilder.AppendFormat("\"{0}\",", parsedValue.ToCsv());
      }

      if (sBuilder[sBuilder.Length - 1] == ',')
      {
        // Remove the very last comma.
        sBuilder.Remove(sBuilder.Length - 1, 1);
      }

      return sBuilder + Environment.NewLine;
    }

    /// <summary>
    /// Returns a <see cref="Table"/> that represents the current <see cref="LogMessage"/>.
    /// </summary>
    /// <param name="owner">The owner <see cref="Script"/> instance this <see cref="Table"/> is for.</param>
    /// <returns>A new <see cref="Table"/> object that represents the current <see cref="LogMessage"/>, or <c>null</c> on error.</returns>
    public override Table ToLuaTable(Script owner)
    {
      Table msgData = base.ToLuaTable(owner);

      if (msgData == null)
      {
        return null;
      }

      foreach (KeyValuePair<int, string> columnValue in mParsedValue)
      {
        msgData[mColumnizer.Columns[columnValue.Key].Name] = columnValue.Value;
      }
      
      return msgData;
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of the <see cref="LogMessageCustom"/> object.
    /// </summary>
    /// <param name="rawData">The data <see cref="Array"/> the new <see cref="LogMessageCustom"/> represents.</param>
    /// <param name="index">The index of the <see cref="LogMessage"/>.</param>
    /// <param name="columns">The <see cref="Columnizer"/> to use for parsing.</param>
    public LogMessageCustom(string rawData, int index, Columnizer columns) : base(rawData, index)
    {
      mColumnizer = columns;

      if (!ParseData(rawData))
      {
        throw new ApplicationException("Unable to parse the logger data.");
      }
    }

    #endregion
  }
}

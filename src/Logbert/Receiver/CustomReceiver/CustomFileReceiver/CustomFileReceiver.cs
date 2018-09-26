﻿#region Copyright © 2016 Couchcoding

// File:    CustomFileReceiver.cs
// Package: Logbert
// Project: Logbert
// 
// The MIT License (MIT)
// 
// Copyright (c) 2016 Couchcoding
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

using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Couchcoding.Logbert.Helper;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Logging;
using System;

using Couchcoding.Logbert.Controls;

namespace Couchcoding.Logbert.Receiver.CustomReceiver.CustomFileReceiver
{
  /// <summary>
  /// Implements a <see cref="ILogProvider"/> for the custom log file service.
  /// </summary>
  public sealed class CustomFileReceiver : ReceiverBase
  {
    #region Private Fields

    /// <summary>
    /// The linked <see cref="Columnizer"/> instance.
    /// </summary>
    private readonly Columnizer mColumnizer;

    /// <summary>
    /// Holds the name of the File to observe.
    /// </summary>
    private readonly string mFileToObserve;

    /// <summary>
    /// Determines whether the file to observed should be read from beginning, or not.
    /// </summary>
    private readonly bool mStartFromBeginning;

    /// <summary>
    /// The <see cref="FileSystemWatcher"/> used to observe file content changes.
    /// </summary>
    private FileSystemWatcher mFileWatcher;

    /// <summary>
    /// The <see cref="StreamReader"/> used to read the log file content.
    /// </summary>
    private StreamReader mFileReader;

    /// <summary>
    /// Holds the offset of the last read line within the log file.
    /// </summary>
    private long mLastFileOffset;

    /// <summary>
    /// Counts the received messages;
    /// </summary>
    private int mLogNumber;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the name of the <see cref="ILogProvider"/>.
    /// </summary>
    public override string Name
    {
      get
      {
        return "Custom File Receiver";
      }
    }

    /// <summary>
    /// Gets the description of the <see cref="ILogProvider"/>
    /// </summary>
    public override string Description
    {
      get
      {
        return string.Format(
            "{0} ({1})"
          , Name
          , !string.IsNullOrEmpty(mFileToObserve) ? Path.GetFileName(mFileToObserve) : "-");
      }
    }

    /// <summary>
    /// Gets the filename for export of the received <see cref="LogMessage"/>s.
    /// </summary>
    public override string ExportFileName
    {
      get
      {
        return Description;
      }
    }

    /// <summary>
    /// Determines whether this <see cref="ILogProvider"/> supports the logger tree window.
    /// </summary>
    public override bool HasLoggerTree
    {
      get
      {
        // Currently no logger tree is supported.
        return false;
      }
    }

    /// <summary>
    /// Gets the settings <see cref="Control"/> of the <see cref="ILogProvider"/>.
    /// </summary>
    public override ILogSettingsCtrl Settings
    {
      get
      {
        return new CustomFileReceiverSettings();
      }
    }

    /// <summary>
    /// Gets the columns to display of the <see cref="ILogProvider"/>.
    /// </summary>
    public override Dictionary<int, LogColumnData> Columns
    {
      get
      {
        Dictionary<int, LogColumnData> clmDict = new Dictionary<int, LogColumnData>
        {
          { 0, new LogColumnData("Number") }
        };

        foreach (LogColumn lgclm in mColumnizer.Columns)
        {
          if (lgclm.ColumnType != LogColumnType.Multigroup) {
            clmDict.Add(clmDict.Count, new LogColumnData(
              lgclm.Name
            , true
            , lgclm.ColumnType == LogColumnType.Message ? 1024 : 100));
          }
        }

        return clmDict;
      }
    }

    /// <summary>
    /// Get the <see cref="Control"/> to display details about a selected <see cref="LogMessage"/>.
    /// </summary>
    public override ILogPresenter DetailsControl
    {
      get
      {
        return new CustomDetailsControl(mColumnizer);
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the FileChanged event of the <see cref="FileSystemWatcher"/> instance.
    /// </summary>
    private void OnLogFileChanged(object sender, FileSystemEventArgs e)
    {
      if (e.ChangeType == WatcherChangeTypes.Changed)
      {
        ReadNewLogMessagesFromFile();
      }
    }

    /// <summary>
    /// Handles the Error event of the <see cref="FileSystemWatcher"/>.
    /// </summary>
    private void OnFileWatcherError(object sender, ErrorEventArgs e)
    {
      // Stop further listening on error.
      if (mFileWatcher != null)
      {
        mFileWatcher.EnableRaisingEvents = false;
        mFileWatcher.Changed            -= OnLogFileChanged;
        mFileWatcher.Error              -= OnFileWatcherError;
        mFileWatcher.Dispose();
      }

      string pathOfFile = Path.GetDirectoryName(mFileToObserve);
      string nameOfFile = Path.GetFileName(mFileToObserve);

      if (!string.IsNullOrEmpty(pathOfFile) && !string.IsNullOrEmpty(nameOfFile))
      {
        mFileWatcher = new FileSystemWatcher(
            pathOfFile
          , nameOfFile);

        mFileWatcher.NotifyFilter        = NotifyFilters.LastWrite | NotifyFilters.Size;
        mFileWatcher.Changed            += OnLogFileChanged;
        mFileWatcher.Error              += OnFileWatcherError;
        mFileWatcher.EnableRaisingEvents = IsActive;

        ReadNewLogMessagesFromFile();
      }
    }

    /// <summary>
    /// Reads possible new log file entries form the file that is observed.
    /// </summary>
    private void ReadNewLogMessagesFromFile()
    {
      if (mFileReader == null || Equals(mFileReader.BaseStream.Length, mLastFileOffset))
      {
        return;
      }

      mFileReader.BaseStream.Seek(
          mLastFileOffset
        , SeekOrigin.Begin);

      string line;

      List<LogMessage> messages = new List<LogMessage>();

      while ((line = mFileReader.ReadLine()) != null)
      {
        LogMessageCustom cstmLgMsg;

        try
        {
          cstmLgMsg = new LogMessageCustom(
              line
            , mLogNumber + 1
            , mColumnizer);
        }
        catch (Exception ex)
        {
          Logger.Warn(ex.Message);
          continue;
        }

        mLogNumber++;
        messages.Add(cstmLgMsg);
      }

      mLastFileOffset = mFileReader.BaseStream.Position;

      if (mLogHandler != null)
      {
        mLogHandler.HandleMessage(messages.ToArray());
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the header used for the CSV file export.
    /// </summary>
    /// <returns>The header used for the CSV file export.</returns>
    public override string GetCsvHeader()
    {
      string csvHdr = "\"Number\",";

      foreach (LogColumn lgclm in mColumnizer.Columns)
      {
        csvHdr += "\"" + lgclm.Name.ToCsv() + "\",";
      }

      if (csvHdr.EndsWith(","))
      {
        // Remove the very last comma.
        csvHdr.Remove(csvHdr.Length - 1, 1);
      }

      return csvHdr + Environment.NewLine;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
      return Name;
    }

    /// <summary>
    /// Intizializes the <see cref="ILogProvider"/>.
    /// </summary>
    /// <param name="logHandler">The <see cref="ILogHandler"/> that may handle incomming <see cref="LogMessage"/>s.</param>
    public override void Initialize(ILogHandler logHandler)
    {
      base.Initialize(logHandler);

      mFileReader = new StreamReader(new FileStream(
          mFileToObserve
        , FileMode.Open
        , FileAccess.Read
        , FileShare.ReadWrite)
        , mEncoding);

      mLogNumber      = 0;
      mLastFileOffset = mStartFromBeginning
        ? 0
        : mFileReader.BaseStream.Length;

      string pathOfFile = Path.GetDirectoryName(mFileToObserve);
      string nameOfFile = Path.GetFileName(mFileToObserve);

      if (!string.IsNullOrEmpty(pathOfFile) && !string.IsNullOrEmpty(nameOfFile))
      {
        mFileWatcher = new FileSystemWatcher(
            pathOfFile
          , nameOfFile);

        mFileWatcher.NotifyFilter        = NotifyFilters.LastWrite | NotifyFilters.Size;
        mFileWatcher.Changed            += OnLogFileChanged;
        mFileWatcher.Error              += OnFileWatcherError;
        mFileWatcher.EnableRaisingEvents = IsActive;

        ReadNewLogMessagesFromFile();
      }
    }

    /// <summary>
    /// Resets the <see cref="ILogProvider"/> instance.
    /// </summary>
    public override void Clear()
    {
      mLogNumber = 0;
    }

    /// <summary>
    /// Resets the <see cref="ILogProvider"/> instance.
    /// </summary>
    public override void Reset()
    {
      Shutdown();
      Initialize(mLogHandler);
    }

    /// <summary>
    /// Shuts down the <see cref="ILogProvider"/> instance.
    /// </summary>
    public override void Shutdown()
    {
      base.Shutdown();

      if (mFileWatcher != null)
      {
        mFileWatcher.EnableRaisingEvents = false;
        mFileWatcher.Changed            -= OnLogFileChanged;
        mFileWatcher.Error              -= OnFileWatcherError;
        mFileWatcher.Dispose();
      }

      if (mFileReader != null)
      {
        mFileReader.Close();
        mFileReader = null;
      }
    }

    /// <summary>
    /// Saves the current docking and collumn layout of the <see cref="ILogProvider"/> implementation.
    /// </summary>
    /// <param name="layout">The layout as string to save.</param>
    /// <param name="columnLayout">The current column layout to save.</param>
    public override void SaveLayout(string layout, List<LogColumnData> columnLayout)
    {
      Properties.Settings.Default.DockLayoutCustomReceiver = layout ?? string.Empty;
      Properties.Settings.Default.SaveSettings();
    }

    /// <summary>
    /// Loads the docking layout of the <see cref="ReceiverBase"/> instance.
    /// </summary>
    /// <returns>The restored layout, or <c>null</c> if none exists.</returns>
    public override string LoadLayout()
    {
      return Properties.Settings.Default.DockLayoutCustomReceiver;
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new and empty instance of the <see cref="CustomFileReceiver"/> class.
    /// </summary>
    public CustomFileReceiver()
    {

    }

    /// <summary>
    /// Creates a new and configured instance of the <see cref="CustomFileReceiver"/> class.
    /// </summary>
    /// <param name="fileToObserve">The file the new <see cref="CustomFileReceiver"/> instance should observe.</param>
    /// <param name="startFromBeginning">Determines whether the new <see cref="CustomFileReceiver"/> should read the given <paramref name="CustomFileReceiver"/> from beginnin, or not.</param>
    /// <param name="columnizer">The <see cref="Columnizer"/> instance to use for parsing.</param>
    /// <param name="codePage">The codepage to use for encoding of the data to parse.</param>
    public CustomFileReceiver(string fileToObserve, bool startFromBeginning, Columnizer columnizer, int codePage) : base (codePage)
    {
      mFileToObserve      = fileToObserve;
      mStartFromBeginning = startFromBeginning;
      mColumnizer         = columnizer;
    }

    #endregion
  }
}

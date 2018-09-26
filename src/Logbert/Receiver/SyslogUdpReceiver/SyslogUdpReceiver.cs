﻿#region Copyright © 2015 Couchcoding

// File:    SyslogUdpReceiver.cs
// Package: Logbert
// Project: Logbert
// 
// The MIT License (MIT)
// 
// Copyright (c) 2015 Couchcoding
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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using Couchcoding.Logbert.Controls;
using Couchcoding.Logbert.Helper;
using Couchcoding.Logbert.Interfaces;
using Couchcoding.Logbert.Logging;

namespace Couchcoding.Logbert.Receiver.SyslogUdpReceiver
{
  /// <summary>
  /// Implements a <see cref="ILogProvider"/> for the Syslog service.
  /// </summary>
  public class SyslogUdpReceiver : ReceiverBase
  {
    #region Private Fields

    /// <summary>
    /// Holds the multicast IP address to listen for.
    /// </summary>
    private IPAddress mMulticastIpAddress;

    /// <summary>
    /// The network interface to listen on.
    /// </summary>
    private readonly IPEndPoint mListenInterface;

    /// <summary>
    /// The <see cref="UdpClient"/> to reveive <see cref="LogMessage"/>s from.
    /// </summary>
    private UdpClient mUdpClient;

    /// <summary>
    /// The format of the timestamp of a received message.
    /// </summary>
    private readonly string mTimestampFormat;

    /// <summary>
    /// Counts the received messages;
    /// </summary>
    private int mLogNumber;

    #endregion

    #region Private Types

    /// <summary>
    /// Implements a state object for UDP communication.
    /// </summary>
    private class UdpState
    {
      #region Public Properties

      /// <summary>
      /// Gets the <see cref="UdpClient"/> that will receive messages.
      /// </summary>
      internal UdpClient Client
      {
        get;
        private set;
      }

      /// <summary>
      /// Gets the <see cref="IPEndPoint"/> to listen on.
      /// </summary>
      internal IPEndPoint EndPoint
      {
        get;
        private set;
      }

      #endregion

      #region Constructor

      /// <summary>
      /// Creates a new instance of the <see cref="UdpState"/> type.
      /// </summary>
      /// <param name="client">The <see cref="UdpClient"/> that will receive messages.</param>
      /// <param name="endPoint">The <see cref="IPEndPoint"/> to listen on.</param>
      internal UdpState(UdpClient client, IPEndPoint endPoint)
      {
        Client   = client;
        EndPoint = endPoint;
      }

      #endregion
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the name of the <see cref="ILogProvider"/>.
    /// </summary>
    public override string Name
    {
      get
      {
        return "Syslog UDP Receiver";
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
            "{0} (Port: {1})"
          , Name
          , mListenInterface.Port);
      }
    }

    /// <summary>
    /// Gets the filename for export of the received <see cref="LogMessage"/>s.
    /// </summary>
    public override string ExportFileName
    {
      get
      {
        return string.Format(
            "{0} (Port {1})"
          , Name
          , mListenInterface.Port);
      }
    }

    /// <summary>
    /// Gets the settings <see cref="Control"/> of the <see cref="ILogProvider"/>.
    /// </summary>
    public override ILogSettingsCtrl Settings
    {
      get
      {
        return new SyslogUdpReceiverSettings();
      }
    }

    /// <summary>
    /// Gets the columns to display of the <see cref="ILogProvider"/>.
    /// </summary>
    public override Dictionary<int, LogColumnData> Columns
    {
      get
      {
        string[] visibleVal = Properties.Settings.Default.ColumnVisibleSyslogUdpReceiver.Split(',');
        string[] widthVal   = Properties.Settings.Default.ColumnWidthSyslogUdpReceiver.Split(',');

        return new Dictionary<int, LogColumnData>
        {
          { 0, new LogColumnData("Number",             visibleVal[0] == "1", int.Parse(widthVal[0])) },
          { 1, new LogColumnData("Severity",           visibleVal[1] == "1", int.Parse(widthVal[1])) },
          { 2, new LogColumnData("Local Machine Time", visibleVal[2] == "1", int.Parse(widthVal[2])) },
          { 3, new LogColumnData("Time",               visibleVal[3] == "1", int.Parse(widthVal[3])) },
          { 4, new LogColumnData("Facility",           visibleVal[4] == "1", int.Parse(widthVal[4])) },
          { 5, new LogColumnData("Sender",             visibleVal[5] == "1", int.Parse(widthVal[5])) },
          { 6, new LogColumnData("Message",            visibleVal[6] == "1", int.Parse(widthVal[6])) }
        };
      }
    } 

    /// <summary>
    /// Gets the supported <see cref="LogLevel"/>s of the <see cref="ILogProvider"/>.
    /// </summary>
    public override LogLevel SupportedLevels => LogLevel.All;

    /// <summary>
    /// Determines whether this <see cref="ILogProvider"/> supports reloading of the content, ot not.
    /// </summary>
    public override bool SupportsReload
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Get the <see cref="Control"/> to display details about a selected <see cref="LogMessage"/>.
    /// </summary>
    public override ILogPresenter DetailsControl
    {
      get
      {
        return new SyslogDetailsControl();
      }
    }

	  /// <summary>
	  /// Gets or sets the active state if the <see cref="ILogProvider"/>.
	  /// </summary>
	  public override bool IsActive
    {
      get
      {
        return base.IsActive;
      }
      set
      {
        base.IsActive = value;

        if (!mIsActive)
        {
          Shutdown();
        }
        else
        {
          Initialize(mLogHandler);
        }
      }
    }

    /// <summary>
    /// Gets the path seperator for the logger tree.
    /// </summary>
    public override string LoggerTreePathSeperator
    {
      get
      {
        return "\t";
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the received UDP message event.
    /// </summary>
    /// <param name="ar">The <see cref="IAsyncResult"/> object that contains necessary meta data.</param>
    private void ReceiveUdpMessage(IAsyncResult ar)
    {
      UdpClient client = ((UdpState)ar.AsyncState).Client;

      IPEndPoint receivedIpEndPoint = new IPEndPoint(
          IPAddress.Any
        , 0);

      byte[] receiveBytes;

      try
      {
        receiveBytes = client.EndReceive(
            ar
          , ref receivedIpEndPoint);
      }
      catch (ObjectDisposedException)
      {
        // The socket seems to be already closed.
        return;
      }

      if (receiveBytes != null)
      {
        try
        {
          LogMessage newLogMsg = new LogMessageSyslog(
              mEncoding.GetString(receiveBytes)
            , ++mLogNumber
            , mTimestampFormat);

          if (mLogHandler != null)
          {
            mLogHandler.HandleMessage(newLogMsg);
          }
        }
        catch (Exception ex)
        {
          Logger.Warn(ex.Message);
        }
      }

      client.BeginReceive(ReceiveUdpMessage, ar.AsyncState);
    }

    #endregion

    #region Public Methods

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

      try
      {
        mUdpClient        = new UdpClient();
        mUdpClient.Client = new Socket(
            AddressFamily.InterNetwork
          , SocketType.Dgram
          , ProtocolType.Udp);

        IPEndPoint localEP = new IPEndPoint(
            mListenInterface.Address
          , mListenInterface.Port);

        mUdpClient.Client.Bind(localEP);

        if (mMulticastIpAddress != null)
        {
          try
          {
            mUdpClient.JoinMulticastGroup(
                mMulticastIpAddress
              , mListenInterface.Address);
          }
          catch (Exception ex)
          {
            Logger.Warn(ex.Message);
          }
        }

        UdpState state = new UdpState(
            mUdpClient
          , mListenInterface);
        
        mUdpClient.BeginReceive(
            ReceiveUdpMessage
          , state);
      }
      catch (Exception ex)
      {
        Logger.Warn(ex.Message);
      }
    }

    /// <summary>
    /// Shuts down the <see cref="ILogProvider"/> instance.
    /// </summary>
    public override void Shutdown()
    {
      if (mUdpClient != null)
      {
        mUdpClient.Close();
        mUdpClient = null;
      }

      base.Shutdown();
    }

    /// <summary>
    /// Gets the header used for the CSV file export.
    /// </summary>
    /// <returns></returns>
    public override string GetCsvHeader()
    {
      return "\"Number\","
           + "\"Severity\","
           + "\"Local Machine Time\","
           + "\"Time\","
           + "\"Facility\","
           + "\"Sender\","
           + "\"Message\""
           + Environment.NewLine;
    }

    /// <summary>
    /// Resets the <see cref="ILogProvider"/> instance.
    /// </summary>
    public override void Clear()
    {
      mLogNumber = 0;
    }

    /// <summary>
    /// Saves the current docking and collumn layout of the <see cref="ILogProvider"/> implementation.
    /// </summary>
    /// <param name="layout">The layout as string to save.</param>
    /// <param name="columnLayout">The current column layout to save.</param>
    public override void SaveLayout(string layout, List<LogColumnData> columnLayout)
    {
      Properties.Settings.Default.DockLayoutSyslogUdpReceiver = layout ?? string.Empty;

      Properties.Settings.Default.ColumnVisibleSyslogUdpReceiver = string.Format(
          "{0},{1},{2},{3},{4},{5},{6}"
        , columnLayout[0].Visible ? 1 : 0
        , columnLayout[1].Visible ? 1 : 0
        , columnLayout[2].Visible ? 1 : 0
        , columnLayout[3].Visible ? 1 : 0
        , columnLayout[4].Visible ? 1 : 0
        , columnLayout[5].Visible ? 1 : 0
        , columnLayout[6].Visible ? 1 : 0);

      Properties.Settings.Default.ColumnWidthSyslogUdpReceiver = string.Format(
          "{0},{1},{2},{3},{4},{5},{6}"
        , columnLayout[0].Width
        , columnLayout[1].Width
        , columnLayout[2].Width
        , columnLayout[3].Width
        , columnLayout[4].Width
        , columnLayout[5].Width
        , columnLayout[6].Width);

      Properties.Settings.Default.SaveSettings();
    }

    /// <summary>
    /// Loads the docking layout of the <see cref="ReceiverBase"/> instance.
    /// </summary>
    /// <returns>The restored layout, or <c>null</c> if none exists.</returns>
    public override string LoadLayout()
    {
      return Properties.Settings.Default.DockLayoutSyslogUdpReceiver;
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new and configured instance of the <see cref="SyslogUdpReceiver"/> class.
    /// </summary>
    public SyslogUdpReceiver()
    {

    }

    /// <summary>
    /// Creates a new and configured instance of the <see cref="SyslogUdpReceiver"/> class.
    /// </summary>
    /// <param name="multicastIp">The multicast IP address to listen for.</param>
    /// <param name="listenInterface">The network interface to listen on.</param>
    /// <param name="timestampFormat">The format of the timestamp of a received message.</param>
    /// <param name="codePage">The codepage to use for encoding of the data to parse.</param>
    public SyslogUdpReceiver(IPAddress multicastIp, IPEndPoint listenInterface, string timestampFormat, int codePage) : base(codePage)
    {
      mMulticastIpAddress = multicastIp;
      mListenInterface    = listenInterface;
      mTimestampFormat    = timestampFormat;
    }

    #endregion
  }
}

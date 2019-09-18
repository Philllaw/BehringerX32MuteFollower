using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static X32MuteFollow.SaveLoad;

namespace X32MuteFollow
{

    class Controller
    {
        GUI theGUI;
        ProgramSettings Settings;

        UdpClient udpClientMaster;
        IPEndPoint endpointMaster;
        Thread threadConnectionMaintainerMaster;
        Thread threadConnectionListenerMaster;
        ConnectionState connectionStateMaster;
        long lastMessagereceivedTimeMaster = 0;
        long lastRequestMessageSendTimeMaster = 0;
        Boolean[] onStatesMaster;

        UdpClient udpClientFollower;
        IPEndPoint endpointFollower;
        Thread threadConnectionMaintainerFollower;
        Thread threadConnectionListenerFollower;
        ConnectionState connectionStateFollower;
        long lastMessagereceivedTimeFollower = 0;
        long lastRequestMessageSendTimeFollower = 0;
        Boolean[] onStatesFollower;

        Boolean running = false;
        Boolean shuttingDown = false;

        //byte[] requestChOnList = System.Text.Encoding.UTF8.GetBytes("/formatsubscribe~~~~,ssiii~~/mutes~~/ch/**/mix/on~~~000100320002"); //needs null padding so use below
        byte[] requestChOnList = new byte[] { 0x2F, 0x66, 0x6F, 0x72, 0x6D, 0x61, 0x74, 0x73, 0x75, 0x62, 0x73, 0x63, 0x72, 0x69, 0x62, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x73, 0x73, 0x69, 0x69, 0x69, 0x00, 0x00, 0x2F, 0x6D, 0x75, 0x74, 0x65, 0x73, 0x00, 0x00, 0x2F, 0x63, 0x68, 0x2F, 0x2A, 0x2A, 0x2F, 0x6D, 0x69, 0x78, 0x2F, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x02 };
        //byte[] requestAushOnList = System.Text.Encoding.UTF8.GetBytes("/formatsubscribe~~~~,ssiii~~/amutes~/auxin/**/mix/on~~~~ 1 8 2"); //needs null padding so use below
        byte[] requestAushOnList = new byte[] { 0x2F, 0x66, 0x6F, 0x72, 0x6D, 0x61, 0x74, 0x73, 0x75, 0x62, 0x73, 0x63, 0x72, 0x69, 0x62, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x73, 0x73, 0x69, 0x69, 0x69, 0x00, 0x00, 0x2F, 0x61, 0x6D, 0x75, 0x74, 0x65, 0x73, 0x00, 0x2F, 0x61, 0x75, 0x78, 0x69, 0x6E, 0x2F, 0x2A, 0x2A, 0x2F, 0x6D, 0x69, 0x78, 0x2F, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x02 };
        //byte[] requestFxOnList = System.Text.Encoding.UTF8.GetBytes("/formatsubscribe~~~~,ssiii~~/fmutes~/fxrtn/**/mix/on~~~~ 1 8 2"); //needs null padding so use below
        byte[] requestFxOnList = new byte[] { 0x2F, 0x66, 0x6F, 0x72, 0x6D, 0x61, 0x74, 0x73, 0x75, 0x62, 0x73, 0x63, 0x72, 0x69, 0x62, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x73, 0x73, 0x69, 0x69, 0x69, 0x00, 0x00, 0x2F, 0x66, 0x6D, 0x75, 0x74, 0x65, 0x73, 0x00, 0x2F, 0x66, 0x78, 0x72, 0x74, 0x6E, 0x2F, 0x2A, 0x2A, 0x2F, 0x6D, 0x69, 0x78, 0x2F, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x02 };

        public Controller(GUI gui, ProgramSettings settings)
        {
            theGUI = gui;
            Settings = settings;
        }

        public void Start()
        {
            theGUI.listenToButton(new System.EventHandler(this.buttonGoStopPressed));
            theGUI.listenToFormClosing(new System.Windows.Forms.FormClosedEventHandler(this.GUI_FormClosed));

            threadConnectionMaintainerMaster = new Thread(connectionMaintainerMaster);
            threadConnectionMaintainerFollower = new Thread(connectionMaintainerFollower);
            connectionStateMaster = ConnectionState.Stopped;
            threadConnectionMaintainerMaster.Start();
            threadConnectionMaintainerFollower.Start();

            threadConnectionListenerMaster = new Thread(connectionListenerMaster);
            threadConnectionListenerFollower = new Thread(connectionListenerFollower);
            connectionStateFollower = ConnectionState.Stopped;
            threadConnectionListenerMaster.Start();
            threadConnectionListenerFollower.Start();

            if (Settings.Running)
            {
                buttonGoStopPressed(null, null);
            }
        }

        private void GUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            shuttingDown = true;
            if (null != udpClientMaster)
            {
                try
                {
                    udpClientMaster.Close();
                }
                catch (Exception ex) { }
            }
            if (null != udpClientFollower)
            {
                try
                {
                    udpClientFollower.Close();
                }
                catch (Exception ex) { }
            }
        }

        private void connectionMaintainerMaster()
        {
            long connectionMessageSendTime = 0;

            while (!shuttingDown)
            {
                long currentTime = DateTimeOffset.Now.Ticks / TimeSpan.TicksPerMillisecond;
                switch (connectionStateMaster)
                {
                    case ConnectionState.TryConnection:
                        udpClientMaster.Send(new byte[] { 0x2f, 0x69, 0x6E, 0x66, 0x6F, 0, 0, 0 }, 8); //send info request "/info~~~"
                        connectionMessageSendTime = currentTime;
                        theGUI.setStatusMessageMaster("Connecting");
                        connectionStateMaster = ConnectionState.Connecting;
                        break;
                    case ConnectionState.Connecting:
                        if (currentTime - connectionMessageSendTime > 1000) //if time since last connect message sent > 1 second
                        {
                            connectionStateMaster = ConnectionState.TryConnection;
                        }
                        break;
                    case ConnectionState.Connected:
                        if (currentTime - lastMessagereceivedTimeMaster > 1000) //if time since last message received > 1 second
                        {
                            connectionStateMaster = ConnectionState.TryConnection;
                            theGUI.setStatusMessageMaster("Timeout");
                        }
                        else if (currentTime - lastRequestMessageSendTimeMaster > 3000) //if time since last renew message sent > 3 seconds
                        {
                            udpClientMaster.Send(new byte[] { 0x2f, 0x72, 0x65, 0x6E, 0x65, 0x77, 0, 0 }, 8); //send renew message "/renew"
                            lastRequestMessageSendTimeMaster = currentTime;
                        }
                        break;
                    case ConnectionState.Stopped:
                    default:
                        break;
                }
                Thread.Sleep(10);
            }
        }

        private void connectionMaintainerFollower()
        {
            long connectionMessageSendTime = 0;

            while (!shuttingDown)
            {
                long currentTime = DateTimeOffset.Now.Ticks / TimeSpan.TicksPerMillisecond;
                switch (connectionStateFollower)
                {
                    case ConnectionState.TryConnection:
                        udpClientFollower.Send(new byte[] { 0x2f, 0x69, 0x6E, 0x66, 0x6F, 0, 0, 0 }, 8); //send info request "/info~~~"
                        connectionMessageSendTime = currentTime;
                        theGUI.setStatusMessageFollower("Connecting");
                        connectionStateFollower = ConnectionState.Connecting;
                        break;
                    case ConnectionState.Connecting:
                        if (currentTime - connectionMessageSendTime > 1000) //if time since last connect message sent > 1 second
                        {
                            connectionStateFollower = ConnectionState.TryConnection;
                        }
                        break;
                    case ConnectionState.Connected:
                        if (currentTime - lastMessagereceivedTimeFollower > 1000) //if time since last message received > 1 second
                        {
                            connectionStateFollower = ConnectionState.TryConnection;
                            theGUI.setStatusMessageFollower("Timeout");
                        }
                        else if (currentTime - lastRequestMessageSendTimeFollower > 3000) //if time since last renew message sent > 3 seconds
                        {
                            udpClientFollower.Send(new byte[] { 0x2f, 0x72, 0x65, 0x6E, 0x65, 0x77, 0, 0 }, 8); //send renew message "/renew"
                            lastRequestMessageSendTimeFollower = currentTime;
                        }
                        break;
                    case ConnectionState.Stopped:
                    default:
                        break;
                }
                Thread.Sleep(10);
            }
        }

        private void connectionListenerMaster()
        {
            while (!shuttingDown)
            {
                if (ConnectionState.Stopped == connectionStateMaster)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    try
                    {
                        byte[] receivedData = udpClientMaster.Receive(ref endpointMaster);
                        processMasterData(receivedData);
                    }
                    catch (Exception ex) { }
                }
            }
        }

        private void connectionListenerFollower()
        {
            while (!shuttingDown)
            {
                if (ConnectionState.Stopped == connectionStateFollower)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    try
                    {
                        byte[] receivedData = udpClientFollower.Receive(ref endpointFollower);
                        processFollowerData(receivedData);
                    }
                    catch (Exception ex) { }
                }
            }
        }

        private void processMasterData(byte[] receivedData)
        {
            long currentTime = DateTimeOffset.Now.Ticks / TimeSpan.TicksPerMillisecond;
            Boolean validMessage = false;

            if (receivedData.Length > 0)
            {
                if (bytesTheSame(receivedData, new byte[] { 0x2f, 0x69, 0x6E, 0x66, 0x6F }, 5))
                {
                    //if info response then set up mute info request
                    udpClientMaster.Send(requestChOnList, requestChOnList.Length); //send Ch On data request
                    udpClientMaster.Send(requestAushOnList, requestAushOnList.Length); //send Aux On data request
                    udpClientMaster.Send(requestFxOnList, requestFxOnList.Length); //send Fx On data request
                    lastRequestMessageSendTimeMaster = currentTime;
                    validMessage = true;
                }
                else if (receivedData.Length == 148 && bytesTheSame(receivedData, System.Text.Encoding.UTF8.GetBytes("/mutes"), 6))
                {
                    updateMutesFromMaster(receivedData);
                    validMessage = true;
                }
                else if (receivedData.Length == 0x34 && bytesTheSame(receivedData, System.Text.Encoding.UTF8.GetBytes("/amutes"), 6))
                {
                    updateAMutesFromMaster(receivedData);
                    validMessage = true;
                }
                else if (receivedData.Length == 0x34 && bytesTheSame(receivedData, System.Text.Encoding.UTF8.GetBytes("/fmutes"), 6))
                {
                    updateFMutesFromMaster(receivedData);
                    validMessage = true;
                }
                if (validMessage)
                {
                    connectionStateMaster = ConnectionState.Connected;
                    theGUI.setStatusMessageMaster("Connected");
                    lastMessagereceivedTimeMaster = currentTime;
                }
            }
        }

        private void updateMutesFromMaster(byte[] receivedData)
        {
            Boolean[] mutes = new Boolean[32];
            for (int i = 0; i < 32; i++)
            {
                mutes[i] = new Boolean();
                mutes[i] = receivedData[20 + i * 4] == 0x00;
            }
            theGUI.updateMutesMaster(mutes);
            for (int i = 0; i < 32; i++)
            {
                if (theGUI.checkBoxes[i].Checked)
                {
                    if (theGUI.radioButtonsMaster[i].Checked != theGUI.radioButtonsFollower[i].Checked)
                    {
                        setChannelMute(i, mutes[i]);
                    }
                }
            }
        }

        private void updateAMutesFromMaster(byte[] receivedData)
        {
            Boolean[] mutes = new Boolean[8];
            for (int i = 0; i < 8; i++)
            {
                mutes[i] = new Boolean();
                mutes[i] = receivedData[20 + i * 4] == 0x00;
            }
            theGUI.updateAMutesMaster(mutes);
            for (int i = 32; i < 40; i++)
            {
                if (theGUI.checkBoxes[i].Checked)
                {
                    if (theGUI.radioButtonsMaster[i].Checked != theGUI.radioButtonsFollower[i].Checked)
                    {
                        setChannelMute(i, mutes[i - 32]);
                    }
                }
            }
        }

        private void updateFMutesFromMaster(byte[] receivedData)
        {
            Boolean[] mutes = new Boolean[8];
            for (int i = 0; i < 8; i++)
            {
                mutes[i] = new Boolean();
                mutes[i] = receivedData[20 + i * 4] == 0x00;
            }
            theGUI.updateFMutesMaster(mutes);
            for (int i = 40; i < 48; i++)
            {
                if (theGUI.checkBoxes[i].Checked)
                {
                    if (theGUI.radioButtonsMaster[i].Checked != theGUI.radioButtonsFollower[i].Checked)
                    {
                        setChannelMute(i, mutes[i - 40]);
                    }
                }
            }
        }

        private void processFollowerData(byte[] receivedData)
        {
            long currentTime = DateTimeOffset.Now.Ticks / TimeSpan.TicksPerMillisecond;
            Boolean validMessage = false;

            if (receivedData.Length > 0)
            {
                if (bytesTheSame(receivedData, new byte[] { 0x2f, 0x69, 0x6E, 0x66, 0x6F }, 5))
                {
                    //if info response then set up mute info request
                    udpClientFollower.Send(requestChOnList, requestChOnList.Length); //send Ch On data request
                    udpClientFollower.Send(requestAushOnList, requestAushOnList.Length); //send Aux On data request
                    udpClientFollower.Send(requestFxOnList, requestFxOnList.Length); //send Fx On data request
                    lastRequestMessageSendTimeFollower = currentTime;
                    validMessage = true;
                }
                else if (receivedData.Length == 148 && bytesTheSame(receivedData, System.Text.Encoding.UTF8.GetBytes("/mutes"), 6))
                {
                    updateMutesFromFollower(receivedData);
                    validMessage = true;
                }
                else if (receivedData.Length == 0x34 && bytesTheSame(receivedData, System.Text.Encoding.UTF8.GetBytes("/amutes"), 6))
                {
                    updateAMutesFromFollower(receivedData);
                    validMessage = true;
                }
                else if (receivedData.Length == 0x34 && bytesTheSame(receivedData, System.Text.Encoding.UTF8.GetBytes("/fmutes"), 6))
                {
                    updateFMutesFromFollower(receivedData);
                    validMessage = true;
                }

                if (validMessage)
                {
                    connectionStateFollower = ConnectionState.Connected;
                    theGUI.setStatusMessageFollower("Connected");
                    lastMessagereceivedTimeFollower = currentTime;
                }
            }
        }

        private void updateMutesFromFollower(byte[] receivedData)
        {
            Boolean[] mutes = new Boolean[32];
            for (int i = 0; i < 32; i++)
            {
                mutes[i] = new Boolean();
                mutes[i] = receivedData[20 + i * 4] == 0x00;
            }
            theGUI.updateMutesFollower(mutes);
        }

        private void updateAMutesFromFollower(byte[] receivedData)
        {
            Boolean[] mutes = new Boolean[8];
            for (int i = 0; i < 8; i++)
            {
                mutes[i] = new Boolean();
                mutes[i] = receivedData[20 + i * 4] == 0x00;
            }
            theGUI.updateAMutesFollower(mutes);
        }

        private void updateFMutesFromFollower(byte[] receivedData)
        {
            Boolean[] mutes = new Boolean[8];
            for (int i = 0; i < 8; i++)
            {
                mutes[i] = new Boolean();
                mutes[i] = receivedData[20 + i * 4] == 0x00;
            }
            theGUI.updateFMutesFollower(mutes);
        }

        private bool bytesTheSame(byte[] bytesA, byte[] bytesB, int compareLength)
        {
            for (int i = 0; i < compareLength; i++)
            {
                if (bytesA[i] != bytesB[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void buttonGoStopPressed(object sender, EventArgs e)
        {
            running = !running;
            theGUI.setRunning(running);
            if (running)
            {
                //Start all the things
                udpClientMaster = new UdpClient();
                endpointMaster = new IPEndPoint(IPAddress.Parse("192.168.1.4"), 10023);
                udpClientFollower = new UdpClient();
                endpointFollower = new IPEndPoint(IPAddress.Parse("192.168.1.5"), 10023);

                onStatesMaster = new Boolean[48];
                onStatesFollower = new Boolean[48];

                for (int i = 0; i < 48; i++)
                {
                    onStatesMaster[i] = false;
                    onStatesFollower[i] = false;
                }

                theGUI.setStatusMessageMaster("Disconnected");
                theGUI.setStatusMessageFollower("Disconnected");

                udpClientMaster.Connect(endpointMaster);
                udpClientFollower.Connect(endpointFollower);

                connectionStateMaster = ConnectionState.TryConnection;
                connectionStateFollower = ConnectionState.TryConnection;

            }
            else
            {
                //stop all the things
                connectionStateMaster = ConnectionState.Stopped;
                connectionStateFollower = ConnectionState.Stopped;
                udpClientMaster.Close();
                udpClientFollower.Close();
                theGUI.setStatusMessageMaster("Disconnected");
                theGUI.setStatusMessageFollower("Disconnected");
            }
        }

        enum ConnectionState
        {
            TryConnection,
            Stopped,
            Connecting,
            Connected,
        }

        private void setChannelMute(int channelNumber, Boolean mute)
        {
            byte[] dataPacket = null;
            if (channelNumber < 0)
            {
                return;
            }
            else if (channelNumber < 32)
            {
                // "/ch/01.32/mix/on ,i ?"
                dataPacket = new byte[] { 0x2F, 0x63, 0x68, 0x2F, 0x30, 0x30, 0x2F, 0x6D, 0x69, 0x78, 0x2F, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                dataPacket[4] += (byte)((channelNumber + 1) / 10);
                dataPacket[5] += (byte)((channelNumber + 1) % 10);
                dataPacket[23] = (byte)(mute ? 0 : 1);
            }
            else if (channelNumber < 40)
            {
                // "/auxin/01..08/mix/on ,i ?"
                dataPacket = new byte[] { 0x2F, 0x61, 0x75, 0x78, 0x69, 0x6E, 0x2F, 0x30, 0x30, 0x2F, 0x6D, 0x69, 0x78, 0x2F, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                dataPacket[7] += (byte)((channelNumber - 31) / 10);
                dataPacket[8] += (byte)((channelNumber - 31) % 10);
                dataPacket[27] = (byte)(mute ? 0 : 1);
            }
            else if (channelNumber < 48)
            {
                // "/fxrtn/01..08/mix/on ,i ?"
                dataPacket = new byte[] { 0x2F, 0x66, 0x78, 0x72, 0x74, 0x6E, 0x2F, 0x30, 0x30, 0x2F, 0x6D, 0x69, 0x78, 0x2F, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                dataPacket[7] += (byte)((channelNumber - 39) / 10);
                dataPacket[8] += (byte)((channelNumber - 39) % 10);
                dataPacket[27] = (byte)(mute ? 0 : 1);
            }
            udpClientFollower.Send(dataPacket, dataPacket.Length);
        }
    }
}

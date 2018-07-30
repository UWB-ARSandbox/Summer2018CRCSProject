using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net.Sockets;

#if !UNITY_EDITOR && UNITY_WSA_10_0
using System;
using Windows.System.Threading;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams; // DataReader/DataWriter & Streams
using Windows.Security.Cryptography; // Convert string to bytes
using System.Runtime.InteropServices.WindowsRuntime; // Used for ToArray extension method for IBuffers (to convert them to byte arrays)
#endif

namespace UWBNetworkingPackage
{
    /// <summary>
    /// A Hololens implementation of the Socket_Base class. Used to send and
    /// receive files on a Hololens from the ASL content server (i.e. master
    /// client).
    /// </summary>
    public class Socket_Base_Hololens : Socket_Base
    {
#if !UNITY_EDITOR && UNITY_WSA_10_0
        /// <summary>
        /// Handles sending of files asynchronously to a given socket.
        /// </summary>
        /// 
        /// <param name="filepath">
        /// The filepath of the file to send over.
        /// </param>
        /// <param name="socket">
        /// The socket associated with the client that will be receiving 
        /// the information.
        /// </param>
        public static void SendFileAsync(string filepath, StreamSocket socket)
        {
            SendFilesAsync(new string[1] { filepath }, socket);
        }

        /// <summary>
        /// The asynchronous method that is used by SendFileAsync to actually
        /// handle the asynchronous aspect of the file sending.
        /// </summary>
        /// 
        /// <param name="filepaths">
        /// The filepaths of files to send over asynchronously.
        /// </param>
        /// <param name="socket">
        /// The socket associated with the client that will be receiving the
        /// information.
        /// </param>
        public static async void SendFilesAsync(string[] filepaths, StreamSocket socket)
        {
            foreach (string filepath in filepaths)
            {
                //Debug.Log("Sending " + Path.GetFileName(filepath));
            }

            MemoryStream ms = new MemoryStream();
            PrepSocketData(filepaths, ref ms);
            DataWriter writer = new DataWriter(socket.OutputStream);
            writer.WriteBytes(ms.ToArray());
            await writer.StoreAsync();
            // uint numBytesSend = await writer.StoreAsync();

            socket.Dispose();
        }

        /// <summary>
        /// Converts files at the given filepaths to a serialized stream and 
        /// pushes them into a MemoryStream buffer. Also inserts the header
        /// appropriately before adding the file information.
        /// </summary>
        /// 
        /// <param name="filepaths">
        /// The filepaths of files to send over asynchronously.
        /// </param>
        /// <param name="ms">
        /// The MemoryStream that will be used as a byte buffer to hold
        /// the information.
        /// </param>
        public static void PrepSocketData(string[] filepaths, ref MemoryStream ms)
        {
            string header = BuildSocketHeader(filepaths);
            
            byte[] headerData = StringToBytes(header);
            // Add header data length
            ms.Write(System.BitConverter.GetBytes(headerData.Length), 0, System.BitConverter.GetBytes(headerData.Length).Length);
            // Add header data
            ms.Write(headerData, 0, headerData.Length);

            foreach (string filepath in filepaths)
            {
                byte[] fileData = File.ReadAllBytes(filepath);
                // Add file data length
                ms.Write(System.BitConverter.GetBytes(fileData.Length), 0, System.BitConverter.GetBytes(fileData.Length).Length);
                // Add file data
                ms.Write(fileData, 0, fileData.Length);
            }
        }

        /// <summary>
        /// Constructs the header for sending information out. The header appends
        /// the name of each file being sent over in a semi-colon separated list.
        /// </summary>
        /// 
        /// <param name="filepaths">
        /// The filepaths of the files you want to send out.
        /// </param>
        ///
        /// <returns>
        /// The string representing the header for a message being sent out
        /// to an ASL node.
        /// </returns>
        public static string BuildSocketHeader(string[] filepaths)
        {
            System.Text.StringBuilder headerBuilder = new System.Text.StringBuilder();

            foreach (string filepath in filepaths)
            {
                headerBuilder.Append(Path.GetFileName(filepath));
                headerBuilder.Append(';');
            }
            headerBuilder.Remove(headerBuilder.Length - 1, 1); // Remove the last separator (';')

            return headerBuilder.ToString();
        }

        /// <summary>
        /// Handles reception of files from a given socket.
        /// </summary>
        /// 
        /// <param name="socket">
        /// The socket which connects the client to the server.
        /// </param>
        /// <param name="receiveDirectory">
        /// The directory which will be used to receive the files from the server.
        /// </param>
        public static async void ReceiveFilesAsync(StreamSocket socket, string receiveDirectory)
        {
            if (!Directory.Exists(receiveDirectory))
            {
                AbnormalDirectoryHandler.CreateDirectory(receiveDirectory);
            }

            MemoryStream fileStream = new MemoryStream();

            DataReader reader = new DataReader(socket.InputStream);

            uint bufferLength = 1048576;
            int numBytesReceived = 0;
            int headerIndex = 0;
            int dataLengthIndex = 0;
            string dataHeader = string.Empty;
            
            do
            {
                numBytesReceived = (int)(await reader.LoadAsync(bufferLength));
                int numBytesAvailable = numBytesReceived;
                int dataIndex = 0;

                // If there are any bytes that continue a file from the last buffer read, handle that here
                //if (numBytesRemaining > 0)
                if(dataLengthIndex > 0 && dataLengthIndex < numBytesReceived)
                {
                    byte[] bytesRemaining = reader.ReadBuffer((uint)numBytesAvailable).ToArray();
                    fileStream.Write(bytesRemaining, 0, numBytesAvailable);

                    string filename = dataHeader.Split(';')[headerIndex++];
                    File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileStream.ToArray());

                    // fileStream.Close(); // Doesn't exist in UWP?
                    fileStream.Dispose();
                    fileStream = new MemoryStream();
                }

                //while (reader.UnconsumedBufferLength > 0)
                while (dataLengthIndex >= 0 && dataLengthIndex < numBytesReceived)
                {
                    // Read in the first four bytes for the length
                    int dataLength = 0;
                    if (dataLengthIndex <= numBytesReceived - 4)
                    {
                        // Convert to length
                        byte[] dataLengthBuffer = reader.ReadBuffer(4).ToArray();
                        dataLength = System.BitConverter.ToInt32(dataLengthBuffer, 0);

                        if (dataLength <= 0)
                        {
                            // Handle case where end of stream is reached
                            break;
                        }
                    }
                    //else
                    else if (dataLengthIndex < numBytesReceived && dataLengthIndex > numBytesReceived - 4)
                    {
                        // Else length bytes are split between reads...
                        MemoryStream dataLengthMS = new MemoryStream();
                        uint numDataLengthBytes = (uint)(numBytesReceived - dataLengthIndex);
                        dataLengthMS.Write(reader.ReadBuffer(numDataLengthBytes).ToArray(), dataLengthIndex, (int)numDataLengthBytes);


                        numBytesReceived = (int)(await reader.LoadAsync(bufferLength));
                        numBytesAvailable = numBytesReceived;
                        numDataLengthBytes = 4 - numDataLengthBytes;
                        dataLengthMS.Write(reader.ReadBuffer(numDataLengthBytes).ToArray(), 0, (int)numDataLengthBytes);

                        dataLength = System.BitConverter.ToInt32(dataLengthMS.ToArray(), 0);
                        dataLengthIndex -= numBytesReceived;
                        dataLengthMS.Dispose();
                    }

                    // Handle instances where whole file is contained in part of buffer
                    if (numBytesAvailable > 0 && dataIndex + dataLength < numBytesAvailable)
                    {
                        if (dataHeader.Equals(string.Empty))
                        {
                            fileStream.Write(reader.ReadBuffer((uint)dataLength).ToArray(), 0, dataLength);

                            dataHeader = BytesToString(fileStream.ToArray());

                            fileStream.Dispose();
                            fileStream = new MemoryStream();
                        }
                        else
                        {
                            fileStream.Write(reader.ReadBuffer((uint)dataLength).ToArray(), 0, dataLength);

                            string filename = dataHeader.Split(';')[headerIndex++];
                            File.WriteAllBytes(Path.Combine(receiveDirectory, filename), fileStream.ToArray());

                            fileStream.Dispose();
                            fileStream = new MemoryStream();
                        }
                    }
                }

                // Write remainder of bytes in buffer to the file memory stream to store for the next buffer read
                if (numBytesAvailable < 0)
                {
                    break;
                }
                else
                {
                    fileStream.Write(reader.ReadBuffer((uint)numBytesAvailable).ToArray(), 0, numBytesAvailable);
                    dataLengthIndex -= (int)numBytesReceived;
                }
            } while (numBytesReceived > 0);

            fileStream.Dispose();
        }

        /// <summary>
        /// Helper method used to convert a byte array to a string. Uses UTF8
        /// conversion.
        /// </summary>
        /// 
        /// <param name="bytes">
        /// The byte array to convert.
        /// </param>
        /// 
        /// <returns>
        /// The string represented by the bytes passed in.
        /// </returns>
        public static string BytesToString(byte[] bytes)
        {
            //CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, reader.ReadBuffer(numBytes));
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Helper method used to convert a string to a byte array. Uses UTF8
        /// conversion.
        /// </summary>
        /// 
        /// <param name="str">
        /// The string to be converted to a byte array.
        /// </param>
        /// 
        /// <returns>
        /// A byte array representing the string.
        /// </returns>
        public static byte[] StringToBytes(string str)
        {
            //CryptographicBuffer.ConvertStringToBinary(header, BinaryStringEncoding.Utf8).ToArray();
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
#endif

    }
}
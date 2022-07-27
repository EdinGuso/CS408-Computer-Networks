using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace server_project
{
    /*
     * A Database object keeps a dictionary whose keys are client names,
     * and values are another dictionary whose keys are file names,
     * and values are a list which counts the reoccurence of the same file name.
     *   
     * Structure of the database object: 
     * -Database 
     *      -> Client name (dictionary)
     *          -> Client's file name (dictionary)
     *              -> Reoccurence counter of that file name (list)
    */
    public class Database
    {
        public Dictionary<string, Dictionary<string, List<int>>> db;
    }

    public class Filelist
    {
        public Dictionary<string, Dictionary<string, List<string>>> fl;
    }


    public partial class Form1 : Form
    {
        string folderPath = "";
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //All clients and their sockets are kept in a list of tuples.
        List<Tuple<Socket, string>> clients = new List<Tuple<Socket, string>>(); // keeps clients and their information

        bool terminating = false; // server connection
        bool listening = false; // we are not listening anyone at first

        Database database = new Database();
        Filelist filelist = new Filelist();

        const int packetSize = 10 * 1024 * 1024;
        //constructor
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false; // gui is controlled by threads
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        //browse button operation
        private void button_browse_Click(object sender, EventArgs e)
        {
            database.db = new Dictionary<string, Dictionary<string, List<int>>>();
            filelist.fl = new Dictionary<string, Dictionary<string, List<string>>>();
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            //If the selected folder is appropriate, it prints out the message and enables listen button.
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
                logs.AppendText("Path selected as " + folderPath + "\n");
                button_listen.Enabled = true;
            }
        }

        private void Accept()
        {
            //While the server is listening the port.
            while (listening)
            {
                try
                {
                    Socket newClient = serverSocket.Accept(); // checks if there is a client wants to connect, if none it is blocked here
                    bool rejected = true;
                    try
                    {
                        Byte[] buffer = new Byte[64]; // Stream type gets it as byte
                        newClient.Receive(buffer);
                        //Server receives the client name in a buffer.
                        string clientName = Encoding.Default.GetString(buffer);
                        // can be a null character, to handle it do this below
                        // to match the 64 bits null char may be padded, so remove it
                        clientName = clientName.Substring(0, clientName.IndexOf("\0"));
                        string message = "";

                        Byte[] buffer_send = new Byte[64];
                        //If the client name is unique, adds it to the clients list, 
                        //else, rejects it and terminates the connection.
                        if (uniqueName(clientName))
                        {
                            //Server sends a welcome message to the client.
                            message = "Welcome " + clientName + " \n";
                            logs.AppendText(clientName + " has connected.\n");

                            //It puts the client name and its socket in a tuple.
                            Tuple<Socket, string> temp = new Tuple<Socket, string>(newClient, clientName);
                            //Then it adds it to the list of clients.
                            clients.Add(temp);

                            buffer_send = Encoding.Default.GetBytes(message);
                            newClient.Send(buffer_send);
                            rejected = false;
                        }
                        else
                        {
                            //Server sends rejection message to client and closes the socket.                     
                            message = clientName + " already exists! \n";
                            logs.AppendText(message);
                            buffer_send = Encoding.Default.GetBytes(message);
                            newClient.Send(buffer_send);
                            newClient.Close();
                        }
                    }
                    catch
                    {
                        // client may disconnect or it may be terminating
                        if (!terminating)
                        {
                            logs.AppendText("Client could not connect \n");
                        }
                        newClient.Close(); // close the socket

                        //if an error occured after new client is added to the list, it finds and removes that client tuple.
                        updateList(newClient);

                    }

                    //If the client is not rejected, it starts the receive thread.
                    if (!rejected)
                    {
                        Thread receiveThread = new Thread(() => Receive(getClient(newClient)));
                        receiveThread.Start();
                    }
                }
                catch
                {
                    // Server may disconnect or it may be terminating
                    if (terminating)
                    {
                        listening = false;
                    }
                    else
                    {
                        logs.AppendText("The server socket stopped working. \n");
                    }
                }
            }
        }

        //Receive function which receives and process the file sent by the client.
        //Creates a folder in client's name for the client.
        //Saves the recevied file into proper folder. 
        private void Receive(Tuple<Socket, string> thisClient)
        {
            bool connected = true;
            Socket thisSocket = thisClient.Item1;
            //Forms a directory for the client folder.
            string currDir = folderPath + "\\" + thisClient.Item2;

            //While connected to the client and not terminating
            while (connected && !terminating)
            {
                try
                {
                    //If the directory is not exist, creates the client folder at the given directory.
                    if (!Directory.Exists(currDir))
                        Directory.CreateDirectory(currDir);


                    Byte[] bufferMode = new Byte[packetSize]; // Stream type gets it as byte
                    thisSocket.Receive(bufferMode);
                    string incomingMode = Encoding.Default.GetString(bufferMode);
                    incomingMode = incomingMode.Substring(0, incomingMode.IndexOf("\0"));

                    if (incomingMode == "<filelist>") { sendFileList(thisClient); }
                    else if (incomingMode == "<download>") { sendFile(thisClient); }
                    else if (incomingMode == "<copy>") { copyFile(thisClient); }
                    else if (incomingMode == "<delete>") { removeFile(thisClient); }
                    else if (incomingMode == "<make_public>") { makePublic(thisClient); }
                    else if (incomingMode == "<get_public>") { getPublicList(thisClient); }
                    //incoming mode is upload
                    else
                    {

                        Byte[] buffer = new Byte[packetSize]; // Stream type gets it as byte
                        //thisSocket.Receive(buffer);

                        //First buffer received from the client contains the name of the file.
                        //Thus, incommingMessage variable is the file name.
                        string incomingMessage = incomingMode;
                        if (incomingMessage.IndexOf("\0") != -1)
                        {
                            incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                        }

                        int filename_end_index = incomingMessage.IndexOf(".txt");
                        string filename_upload = incomingMessage.Substring(0, filename_end_index + 4);



                        //Shows this message while the file is being received.
                        logs.AppendText(thisClient.Item2 + " is uploading " + filename_upload + "\n");

                        //thisSocket.Receive(buffer);
                        //The client starts to send the content of the file with the very next buffer.
                        //string cont = Encoding.Default.GetString(buffer);
                        string cont = incomingMessage.Substring(filename_end_index + 4);
                        int ending_char = cont.IndexOf("\0");
                        //If the buffer is not full:
                        if (ending_char != -1)
                        {
                            //Updates cont.
                            cont = cont.Substring(0, cont.IndexOf("\0"));
                        }



                        string file = "";
                        bool is_ending = false;
                        //While the last buffer is not containing the terminal word "<file_ending>":
                        while (!is_ending)
                        {
                            //Adds the last buffer's content to the file content.
                            file += cont;

                            //If the file contains the terminal word "<file_ending>"
                            //or the length of the last buffer is less than 13, which is the length of the terminal word.
                            if ((file.Contains("<file_ending>") || cont.Length < 13) && cont.Length != 0)
                            {
                                is_ending = true;
                            }
                            else
                            {
                                //Cleans the content of the buffer.
                                buffer = new Byte[packetSize];
                                thisSocket.Receive(buffer);
                                //Server receives the next buffer.
                                cont = Encoding.Default.GetString(buffer);
                                ending_char = cont.IndexOf("\0");
                                //If the buffer is not full:
                                if (ending_char != -1)
                                {
                                    //Updates cont.
                                    cont = cont.Substring(0, cont.IndexOf("\0"));
                                }
                            }

                        }

                        //After the terminal word is detected, it is removed from the file content string.
                        int ending_index = file.LastIndexOf("<");
                        file = file.Substring(0, ending_index);


                        //Message showing the file is successfully created and saved.
                        logs.AppendText(thisClient.Item2 + " has uploaded  " + filename_upload + "\n");

                        try
                        {
                            Byte[] buffer_send = new Byte[64];
                            string message = "<ACK>";
                            buffer_send = Encoding.Default.GetBytes(message);
                            thisSocket.Send(buffer_send);
                        }
                        catch
                        {
                            logs.AppendText("Server couldn't send acknowledgement");
                        }

                        //The new naming must include the owner's name and the copy number of the file.          
                        string filename = "";
                        string justfilename = "";
                        //If the client has already registered in database:
                        if (database.db.ContainsKey(thisClient.Item2))
                        {
                            //If the client has already had a file with same file name:
                            if (database.db[thisClient.Item2].ContainsKey(filename_upload))
                            {

                                //If the reoccurence counter list is empty which means that there is only file with this name:
                                /*
                                if (!database.db[thisClient.Item2][incomingMessage].Any())
                                {   //Set the first index of the reoccurence counter list of the file as 1.
                                    database.db[thisClient.Item2][incomingMessage].Add(1);
                                }
                                else // else if there are at least two files with same name:
                                {
                                    //Set the next empty index in the counter list as the last element's value + 1.
                                    int lastElem = database.db[thisClient.Item2][incomingMessage].Last();
                                    database.db[thisClient.Item2][incomingMessage].Add(lastElem + 1);
                                }*/

                                int nullIndex = specialFind(database.db[thisClient.Item2][filename_upload]);

                                if (nullIndex != -1)
                                {                                   
                                    database.db[thisClient.Item2][filename_upload][nullIndex] = nullIndex;

                                    int txt_index = filename_upload.LastIndexOf(".txt");
                                    string without_txt = filename_upload.Substring(0, txt_index);

                                    if (nullIndex < 10)
                                    {
                                        if (nullIndex == 0)
                                        {
                                            filename = thisClient.Item2 + without_txt + ".txt";
                                            justfilename = without_txt + ".txt";
                                            filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                                        }
                                        else
                                        {
                                            filename = thisClient.Item2 + without_txt + "-0" + nullIndex.ToString() + ".txt";
                                            justfilename = without_txt + "-0" + nullIndex.ToString() + ".txt";
                                            filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                                        }
                                    }
                                    else
                                    {
                                        filename = thisClient.Item2 + without_txt + "-" + nullIndex.ToString() + ".txt";
                                        justfilename = without_txt + "-" + nullIndex.ToString() + ".txt";
                                        filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                                    }
                                }
                                else
                                {
                                    int lastElem = database.db[thisClient.Item2][filename_upload].Last();
                                    database.db[thisClient.Item2][filename_upload].Add(lastElem + 1);
                                    
                                    int txt_index = filename_upload.LastIndexOf(".txt");
                                    string without_txt = filename_upload.Substring(0, txt_index);

                                    if (database.db[thisClient.Item2][filename_upload].Last() < 10)
                                    {                                   
                                       
                                            filename = thisClient.Item2 + without_txt + "-0" + database.db[thisClient.Item2][filename_upload].Last().ToString() + ".txt";
                                            justfilename = without_txt + "-0" + database.db[thisClient.Item2][filename_upload].Last().ToString() + ".txt";
                                            filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());                                     
                                    }
                                    else
                                    {
                                        filename = thisClient.Item2 + without_txt + "-" + database.db[thisClient.Item2][filename_upload].Last().ToString() + ".txt";
                                        justfilename = without_txt + "-" + database.db[thisClient.Item2][filename_upload].Last().ToString() + ".txt";
                                        filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                                    }
                                }


                                //int txt_index = filename_upload.LastIndexOf(".txt");
                                //string without_txt = filename_upload.Substring(0, txt_index);

                                //If the last element of the counter list is less than 10, put "0" before the naming of the count digit.
                                //Builds the file name.

                                //if (database.db[thisClient.Item2][filename_upload].Last() < 10)
                                                                
                                //if (database.db[thisClient.Item2][filename_upload].Last() < 10)
                                //{
                                //    if (database.db[thisClient.Item2][filename_upload].Last() == 0)
                                //    {
                                //        filename = thisClient.Item2 + without_txt + ".txt";
                                //        justfilename = without_txt + ".txt";
                                //        filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                                //    }
                                //    else
                                //    {
                                //        filename = thisClient.Item2 + without_txt + "-0" + database.db[thisClient.Item2][filename_upload].Last().ToString() + ".txt";
                                //        justfilename = without_txt + "-0" + database.db[thisClient.Item2][filename_upload].Last().ToString() + ".txt";
                                //        filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                                //    }
                                //}
                                //else
                                //{
                                //    filename = thisClient.Item2 + without_txt + "-" + database.db[thisClient.Item2][filename_upload].Last().ToString() + ".txt";
                                //    justfilename = without_txt + "-" + database.db[thisClient.Item2][filename_upload].Last().ToString() + ".txt";
                                //    filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                                //}
                                
                            }
                            else //Else if the client has had no file with this name:
                            {
                                //Adds the file name to the database and creates its counter list.
                                database.db[thisClient.Item2].Add(filename_upload, new List<int>());
                                database.db[thisClient.Item2][filename_upload].Add(0);

                                filelist.fl[thisClient.Item2].Add(filename_upload, new List<string>());

                                //Builds the file name.
                                filename = thisClient.Item2 + filename_upload;
                                justfilename = filename_upload;
                            }

                        }
                        else //Else if the client has not had any record in database before:
                        {
                            //Registers the client to the database.
                            database.db.Add(thisClient.Item2, new Dictionary<string, List<int>>());
                            filelist.fl.Add(thisClient.Item2, new Dictionary<string, List<string>>());
                            //Adds the file name to the database and creates its counter list.
                            database.db[thisClient.Item2].Add(filename_upload, new List<int>());
                            database.db[thisClient.Item2][filename_upload].Add(0);

                            filelist.fl[thisClient.Item2].Add(filename_upload, new List<string>());

                            //Builds the file name.
                            filename = thisClient.Item2 + filename_upload;
                            justfilename = filename_upload;
                        }

                        //The file in the given path is created and the content is written inside.
                        string filepath = currDir + "\\" + filename;
                        File.WriteAllText(filepath, file);

                        long filelength = new System.IO.FileInfo(filepath).Length;
                        DateTime lastModified = System.IO.File.GetLastWriteTime(filepath);

                        filelist.fl[thisClient.Item2][justfilename].Add(thisClient.Item2);
                        filelist.fl[thisClient.Item2][justfilename].Add(filelength.ToString());
                        filelist.fl[thisClient.Item2][justfilename].Add(lastModified.ToString("dd/MM/yy HH:mm:ss"));
                        filelist.fl[thisClient.Item2][justfilename].Add("Private");
                    }
                }
                catch
                {
                    // client may disconnect or it may be terminating
                    if (!terminating)
                    {
                        logs.AppendText(thisClient.Item2 + " has disconnected. \n");
                    }
                    thisSocket.Close(); // close the socket
                    clients.Remove(thisClient);
                    connected = false;
                }
            }
        }

        private void sendFileList(Tuple<Socket, string> thisClient)
        {
            string outputStr = "";

            if (filelist.fl.ContainsKey(thisClient.Item2))
            {
                foreach (string fileName in filelist.fl[thisClient.Item2].Keys)
                {
                    outputStr += fileName + "," + filelist.fl[thisClient.Item2][fileName][1] + "," + filelist.fl[thisClient.Item2][fileName][2] + "," + filelist.fl[thisClient.Item2][fileName][3] + ";";
                }
            }
            else
            {
                outputStr = "<empty_filelist>";
            }
           

            Byte[] buffer = new Byte[packetSize]; // Stream type gets it as byte
            buffer = Encoding.Default.GetBytes(outputStr);
            thisClient.Item1.Send(buffer);
        }

        private void sendFile(Tuple<Socket, string> thisClient)
        {

            try
            {
                Byte[] buffer = new Byte[packetSize]; // Stream type gets it as byte
                thisClient.Item1.Receive(buffer);

                string filename = Encoding.Default.GetString(buffer);
                filename = filename.Substring(0, filename.IndexOf("\0"));
                string ownername = filename.Substring(0, filename.IndexOf(":"));
                filename = filename.Substring(filename.IndexOf(":") + 1);
                string currDir = folderPath + "\\" + ownername;
                string filepath = currDir + "\\" + ownername + filename;

                //read the contents of the file into a string
                string file_content = File.ReadAllText(filepath);

                //compute the size of the last packet
                int last_batch_size = file_content.Length % packetSize;

                logs.AppendText("Sending " + filename + " file to the client!\n");

                //start looping over the data to send as packets
                for (int i = 0; i <= (file_content.Length - 1) / packetSize; i++)
                {
                    string curr_batch;
                    //if it is the last packet (not fixed size)
                    if (i == (file_content.Length - 1) / packetSize)
                    {
                        //get last_batch_size many bytes
                        curr_batch = file_content.Substring(i * packetSize, last_batch_size);
                    }
                    //if it is not the last packet (fixed size = 1024)
                    else
                    {
                        //get 1024 bytes
                        curr_batch = file_content.Substring(i * packetSize, packetSize);
                    }

                    //send the selected bytes to the user
                    buffer = Encoding.Default.GetBytes(curr_batch);
                    thisClient.Item1.Send(buffer);
                }

                //send a message indicating end of file
                string message = "<file_ending>";
                buffer = Encoding.Default.GetBytes(message);
                thisClient.Item1.Send(buffer);

                logs.AppendText("Sent " + filename + " file to the client!\n");
            }
            //we go into this catch if there was a problem while sending the contents of the file
            catch
            {
                //display the appropriate message
                logs.AppendText("Ran into a problem while sending the file!\n");
            }
        }

        private void copyFile(Tuple<Socket, string> thisClient)
        {
            try
            {
                Byte[] buffer = new Byte[packetSize]; // Stream type gets it as byte
                thisClient.Item1.Receive(buffer);

                string copyfilename = "";
                string justfilename = "";
                string filename = Encoding.Default.GetString(buffer);
                filename = filename.Substring(0, filename.IndexOf("\0"));
                string currDir = folderPath + "\\" + thisClient.Item2;
                string filepath = currDir + "\\" + thisClient.Item2 + filename;

                int hyphenIndex = filename.LastIndexOf("-");
                int txtIndex = filename.LastIndexOf(".txt");
                int fileCount = 0;

                if (hyphenIndex != -1)
                {
                    if (Int32.TryParse(filename.Substring(hyphenIndex + 1, txtIndex - hyphenIndex - 1), out fileCount))
                    {
                        justfilename = filename.Substring(0, hyphenIndex) + ".txt";
                    }
                    else justfilename = filename;
                }
                else justfilename = filename;

                /*int lastNum = database.db[thisClient.Item2][justfilename].Last();
                database.db[thisClient.Item2][justfilename].Add(lastNum + 1);*/

                //PROBLEM WITH COPY FILES ENDING WITH -01
                /* if (database.db[thisClient.Item2][filename].Last() < 10)
                 {
                     copyfilename = thisClient.Item2 + without_txt + "-0" + database.db[thisClient.Item2][filename].Last().ToString() + ".txt";
                     justfilename = without_txt + "-0" + database.db[thisClient.Item2][filename].Last().ToString() + ".txt";
                     filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                 }
                 else
                 {
                     copyfilename = thisClient.Item2 + without_txt + "-" + database.db[thisClient.Item2][filename].Last().ToString() + ".txt";
                     justfilename = without_txt + "-" + database.db[thisClient.Item2][filename].Last().ToString() + ".txt";
                     filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                 }
                 */
                int nullIndex = specialFind(database.db[thisClient.Item2][justfilename]);

                if (nullIndex != -1)
                {
                    database.db[thisClient.Item2][justfilename][nullIndex] = nullIndex;

                    int txt_index = justfilename.LastIndexOf(".txt");
                    string without_txt = justfilename.Substring(0, txt_index);

                    if (nullIndex < 10)
                    {
                        if (nullIndex == 0)
                        {
                            copyfilename = thisClient.Item2 + without_txt + ".txt";
                            justfilename = without_txt + ".txt";
                            filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                        }
                        else
                        {
                            copyfilename = thisClient.Item2 + without_txt + "-0" + nullIndex.ToString() + ".txt";
                            justfilename = without_txt + "-0" + nullIndex.ToString() + ".txt";
                            filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                        }
                    }
                    else
                    {
                        copyfilename = thisClient.Item2 + without_txt + "-" + nullIndex.ToString() + ".txt";
                        justfilename = without_txt + "-" + nullIndex.ToString() + ".txt";
                        filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                    }
                }
                else
                {
                    int lastElem = database.db[thisClient.Item2][justfilename].Last();
                    database.db[thisClient.Item2][justfilename].Add(lastElem + 1);

                    int txt_index = justfilename.LastIndexOf(".txt");
                    string without_txt = justfilename.Substring(0, txt_index);

                    if (database.db[thisClient.Item2][justfilename].Last() < 10)
                    {

                        copyfilename = thisClient.Item2 + without_txt + "-0" + database.db[thisClient.Item2][justfilename].Last().ToString() + ".txt";
                        justfilename = without_txt + "-0" + database.db[thisClient.Item2][justfilename].Last().ToString() + ".txt";
                        filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                    }
                    else
                    {
                        copyfilename = thisClient.Item2 + without_txt + "-" + database.db[thisClient.Item2][justfilename].Last().ToString() + ".txt";
                        justfilename = without_txt + "-" + database.db[thisClient.Item2][justfilename].Last().ToString() + ".txt";
                        filelist.fl[thisClient.Item2].Add(justfilename, new List<string>());
                    }
                }

                string copyfilepath = currDir + "\\" + copyfilename;

                File.Copy(filepath, copyfilepath);

                long filelength = new System.IO.FileInfo(copyfilepath).Length;
                DateTime lastModified = System.IO.File.GetLastWriteTime(copyfilepath);

                filelist.fl[thisClient.Item2][justfilename].Add(thisClient.Item2);
                filelist.fl[thisClient.Item2][justfilename].Add(filelength.ToString());
                filelist.fl[thisClient.Item2][justfilename].Add(lastModified.ToString("dd/MM/yy HH:mm:ss"));
                filelist.fl[thisClient.Item2][justfilename].Add(filelist.fl[thisClient.Item2][filename][3]);

                logs.AppendText(thisClient.Item2 + " has copied " + filename + " as " + justfilename + "\n");

                try
                {
                    Byte[] buffer_send = new Byte[64];
                    string message = "<ACK>";
                    buffer_send = Encoding.Default.GetBytes(message);
                    thisClient.Item1.Send(buffer_send);
                }
                catch
                {
                    logs.AppendText("Server couldn't send acknowledgement\n");
                }

                try
                {
                    Byte[] buffer_send = new Byte[256];
                    string message = filename + " has been copied as " + justfilename + "\n";
                    buffer_send = Encoding.Default.GetBytes(message);
                    thisClient.Item1.Send(buffer_send);
                }
                catch {
                    logs.AppendText("Server couldn't send copy information.\n");
                }
            }
            catch
            {
                logs.AppendText("Could not copy the file!\n");
            }

        }

        private void removeFile(Tuple<Socket, string> thisClient)
        {
            try
            {
                Byte[] buffer = new Byte[packetSize]; // Stream type gets it as byte
                thisClient.Item1.Receive(buffer);

                string justfilename = "";
                string filename = Encoding.Default.GetString(buffer);
                filename = filename.Substring(0, filename.IndexOf("\0"));
                string currDir = folderPath + "\\" + thisClient.Item2;
                string filepath = currDir + "\\" + thisClient.Item2  + filename;

                int hyphenIndex = filename.LastIndexOf("-");
                int txtIndex = filename.LastIndexOf(".txt");
                int fileCount = 0;

                if (hyphenIndex != -1)
                {
                    if (Int32.TryParse(filename.Substring(hyphenIndex + 1, txtIndex - hyphenIndex - 1), out fileCount))
                    {
                        justfilename = filename.Substring(0, hyphenIndex) + ".txt";
                    }
                    else justfilename = filename;
                }
                else justfilename = filename;

                database.db[thisClient.Item2][justfilename][fileCount] = Int32.MaxValue;
                filelist.fl[thisClient.Item2].Remove(filename);

                File.Delete(filepath);

                logs.AppendText(filename + "has been removed!\n");

                try
                {
                    Byte[] buffer_send = new Byte[64];
                    string message = "<ACK>";
                    buffer_send = Encoding.Default.GetBytes(message);
                    thisClient.Item1.Send(buffer_send);
                }
                catch
                {
                    logs.AppendText("Server couldn't send acknowledgement");
                }

            }
            catch
            {
                logs.AppendText("Could not remove the file!\n");
            }

        }

        private void makePublic(Tuple<Socket, string> thisClient)
        {
            Byte[] buffer = new Byte[packetSize]; // Stream type gets it as byte
            thisClient.Item1.Receive(buffer);
                        
            string filename = Encoding.Default.GetString(buffer);
            filename = filename.Substring(0, filename.IndexOf("\0"));           

            filelist.fl[thisClient.Item2][filename][3] = "Public";

            logs.AppendText(filename + "is now public. \n");

            try
            {
                Byte[] buffer_send = new Byte[64];
                string message = "<ACK>";
                buffer_send = Encoding.Default.GetBytes(message);
                thisClient.Item1.Send(buffer_send);
            }
            catch
            {
                logs.AppendText("Server couldn't send acknowledgement");
            }
        }

        private void getPublicList(Tuple<Socket, string> thisClient)
        {           
            string publicList = "";

            foreach (string clientName in filelist.fl.Keys)
            {
                foreach (string fileName in filelist.fl[clientName].Keys)
                {
                    if (filelist.fl[clientName][fileName][3] == "Public")
                    {
                        publicList +=  filelist.fl[clientName][fileName][0] +  "," + fileName + "," + filelist.fl[clientName][fileName][1] + "," + filelist.fl[clientName][fileName][2] + ";";
                    }
                }
            }
            if(publicList == "")
            {
                publicList = "<empty_filelist>";
            }

            Byte[] buffer = new Byte[packetSize]; // Stream type gets it as byte
            buffer = Encoding.Default.GetBytes(publicList);
            thisClient.Item1.Send(buffer);
        }
        private int specialFind(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == Int32.MaxValue)
                    return i;
            }
            return -1;
        }

        //If the client name is a unique name, returns true, else false.
        private bool uniqueName(string name)
        {
            foreach (Tuple<Socket, string> client in clients)
            {
                if (client.Item2 == name)
                {
                    return false;
                }
            }
            return true;
        }

        //if the client is in the list, it finds and removes that client tuple.
        private void updateList(Socket sckt)
        {
            foreach (Tuple<Socket, string> client in clients)
            {
                if (client.Item1 == sckt)
                {
                    clients.Remove(client);
                }
            }
        }

        //Returns the client tuple of the given socket.
        private Tuple<Socket, string> getClient(Socket sckt)
        {
            foreach (Tuple<Socket, string> client in clients)
            {
                if (client.Item1 == sckt)
                {
                    return client;
                }
            }
            return null;
        }

        //handles the application closing
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if the folder path is selected from the browse button:
            if (folderPath != "")
            {
                //Converts the updated database object to JSON format
                string jsonString = JsonConvert.SerializeObject(database);
                //Writes the updated database query into "db.txt" file
                File.WriteAllText(folderPath + "\\db.txt", jsonString);

                string filelistString = JsonConvert.SerializeObject(filelist);
                File.WriteAllText(folderPath + "\\fl.txt", filelistString);
            }

            listening = false;
            terminating = true;
            Environment.Exit(0); // exit gui
        }

        //listen button operation
        private void button_listen_Click(object sender, EventArgs e)
        {

            string dbfile = "";
            Database deserializedProduct;

            //If "db.txt" file exists in browsed path, it reads the file, converts it from JSON format into Database object 
            if (File.Exists(folderPath + "\\db.txt"))
            {
                dbfile = File.ReadAllText(folderPath + "\\db.txt");
                deserializedProduct = JsonConvert.DeserializeObject<Database>(dbfile);
                database = deserializedProduct;

            }

            string flfile = "";
            Filelist deserializedFlProduct;

            //If "db.txt" file exists in browsed path, it reads the file, converts it from JSON format into Database object 
            if (File.Exists(folderPath + "\\fl.txt"))
            {
                flfile = File.ReadAllText(folderPath + "\\fl.txt");
                deserializedFlProduct = JsonConvert.DeserializeObject<Filelist>(flfile);
                filelist = deserializedFlProduct;
            }

            int serverPort;
            try
            {
                //If the port value taken from the GUI is successfully parsed:
                if (Int32.TryParse(textBox_port.Text, out serverPort))
                {
                    //Connection to the default IP address and given port for listening.
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
                    serverSocket.Bind(endPoint);
                    serverSocket.Listen(3);


                    listening = true; //Server has started to listening the port.
                    button_browse.Enabled = false;
                    textBox_port.Enabled = false;
                    button_listen.Enabled = false;

                    Thread acceptThread = new Thread(Accept);
                    acceptThread.Start(); //Thread which accepts the clients' connection requests has started.

                    logs.AppendText("Started listening on port: " + serverPort + "\n");

                }
                else
                {
                    //if the connection to the port is not successful, it prints out the message.
                    logs.AppendText("Please check port number \n");
                }
            }
            catch
            {
                //If the port is being used by another service, it prints out the message.
                logs.AppendText("Entered port number is being used! Try another\n");
            }

        }
    }
}

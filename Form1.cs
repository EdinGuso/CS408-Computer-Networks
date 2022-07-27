using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project_client
{
    enum ReceiveState { PublicFileList, FileList, Download, Other };

    public partial class Form1 : Form
    {
        //state variables we keep during the execution
        bool terminating = false;
        bool connected = false;
        Socket clientSocket;
        string username;
        string filepath;
        string filename;
        bool ack = false;
        const int packetSize = 10 * 1024 * 1024; //in bytes
        ReceiveState receiveState = ReceiveState.Other;
        string folderPath;
        string[] serverFiles;
        string[] serverPublicFiles;
        string downloadingFilename;

        //constructor
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();
        }

        //connect and disconnect button operation
        private void button_Connect_Click(object sender, EventArgs e)
        {
            //if the user is not already connected (i.e. button text is connect)
            if (!connected)
            {
                //create the socket
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //read the IP from the GUI
                string IP = textBox_IP.Text;

                int portNum;
                //check for correctness of the port number and whether IP address has been entered
                if (Int32.TryParse(textBox_Port.Text, out portNum) && textBox_IP.Text.Length != 0)
                {
                    try
                    {
                        //read the username from GUI
                        username = textBox_Username.Text;
                        string message = username;

                        //check for correctness of the entered name (not empty, shorter than 64 characters and is alphanumeric)
                        if (message != "" && message.Length <= 64 && Regex.IsMatch(message, "^[a-zA-Z0-9]*$"))
                        {
                            //connect the socket
                            clientSocket.Connect(IP, portNum);

                            //encode the username into the buffer
                            Byte[] buffer = new Byte[64];
                            buffer = Encoding.Default.GetBytes(message);

                            //send the username
                            clientSocket.Send(buffer);

                            try
                            {
                                //recieve a message from the server
                                buffer = new Byte[64];
                                clientSocket.Receive(buffer);

                                //turn it into a string
                                string incomingMessage = Encoding.Default.GetString(buffer);
                                incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));


                                //display the message received from the server (either positive or negative)
                                logs.AppendText("Server: " + incomingMessage);

                                //if the incoming message indicates that we successfully connected
                                if (incomingMessage == "Welcome " + username + " \n")
                                {
                                    //enable the buttons
                                    button_Browse.Enabled = true;
                                    button_GetFileList.Enabled = true;
                                    button_GetPublicFileList.Enabled = true;

                                    //update the state
                                    connected = true;

                                    //change the button text to disconnect
                                    button_Connect.Text = "Disconnect";

                                    //display the appropriate messages
                                    logs.AppendText("Connected to the server!\n");
                                    logs.AppendText("You can now upload, download or delete .txt files!\n");

                                    //start the receive thread
                                    Thread receiveThread = new Thread(Receive);
                                    receiveThread.Start();
                                }
                                //if the incoming message indicated that we were not able to successfully connect
                                else
                                {
                                    //update the state
                                    connected = false;

                                    //close the socket
                                    clientSocket.Close();
                                }
                            }
                            //we go into this catch if there was a problem while waiting a response from the server
                            catch
                            {
                                //if we are not terminating and we were connected while this happened
                                if (!terminating & connected)
                                {
                                    //display the appropriate message
                                    logs.AppendText("The server has disconnected\n");

                                    //change the button text to connect
                                    button_Connect.Text = "Connect";

                                    //disable the buttons
                                    disableButtons();
                                }

                                //close the socket
                                clientSocket.Close();

                                //update the state
                                connected = false;
                            }
                        }
                        //if the entered name was not in a correct format
                        else
                        {
                            //update the state
                            connected = false;

                            //display the appropriate message
                            logs.AppendText("Username cannot be empty or longer than 64 characters, and must consist of alphanumeric characters!\n");
                        }
                    }
                    //we go into this catch if there was a problem with connecting to the server
                    catch
                    {
                        //display the appropriate message
                        logs.AppendText("Could not connect to the server!\n");
                    }
                }
                //if the port or IP were erronous
                else
                {
                    //display the appropriate message
                    logs.AppendText("Check the port or IP\n");
                }
            }
            //if the user is already connected (i.e. button text is disconnect)
            else
            {
                //disable the buttons
                disableButtons();

                //update the state
                connected = false;

                //change the button text to connect
                button_Connect.Text = "Connect";

                //diplay the approptiate message
                logs.AppendText("Disconnected from the server!\n");

                //close the socket
                clientSocket.Close();
            }
        }

        /*
         * Later, when download etc. is required, we can store a state in the class,
         * and branch off using if statements in this function
         * to treat the data differently
         */
        private void Receive()
        {
            //while we are connected
            while (connected)
            {
                try
                {
                    //wait for the server to send data
                    Byte[] buffer = new Byte[packetSize];
                    clientSocket.Receive(buffer);

                    //convert it into a string
                    string incomingMessage = Encoding.Default.GetString(buffer);
                    //incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));

                    if (incomingMessage.IndexOf("\0") != -1)
                    {
                        incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                    }

                    if (receiveState == ReceiveState.FileList)
                    {
                        serverFiles = incomingMessage.Split(';');
                        receiveState = ReceiveState.Other;
                        logs.AppendText("Files stored in the server:\n");
                        if (incomingMessage == "<empty_filelist>")
                        {
                            logs.AppendText("No files in the list!\n");
                        }
                        else
                        {
                            foreach (string file in serverFiles)
                            {
                                if (file != "")
                                {
                                    string[] file_details = file.Split(',');
                                    logs.AppendText(file_details[0] + " " + file_details[1] + " bytes, upload date: " + file_details[2] + ", " + file_details[3] + "\n");
                                }
                            }
                        }
                        // Enable all the private controls
                        button_Browse.Enabled = true;
                        button_GetFileList.Enabled = true;
                        button_GetPublicFileList.Enabled = true;
                        textBox_Filename.Enabled = true;
                        button_BrowseFolder.Enabled = true;
                        button_Copy.Enabled = true;
                        button_Delete.Enabled = true;
                        button_MakePublic.Enabled = true;

                    }
                    else if (receiveState == ReceiveState.PublicFileList)
                    {
                        serverPublicFiles = incomingMessage.Split(';');
                        receiveState = ReceiveState.Other;
                        logs.AppendText("Public files stored in the server:\n");
                        if (incomingMessage == "<empty_filelist>")
                        {
                            logs.AppendText("No files in the list!\n");
                        }
                        else
                        {
                            foreach (string file in serverPublicFiles)
                            {
                                if (file != "")
                                {
                                    string[] file_details = file.Split(',');
                                    logs.AppendText(file_details[0] + " " + file_details[1] + " " + file_details[2] + " bytes, upload date: " + file_details[3] + "\n");
                                }
                            }
                        }
                        // enable public controls
                        button_Browse.Enabled = true;
                        button_GetFileList.Enabled = true;
                        button_GetPublicFileList.Enabled = true;
                        textBox_publicFilename.Enabled = true;
                        textBox_ClientName.Enabled = true;
                        button_publicBrowseFolder.Enabled = true;

                    }
                    else if (receiveState == ReceiveState.Download)
                    {
                        receiveState = ReceiveState.Other;
                        string fileContent = "";
                        bool is_ending = false;
                        //While the last buffer is not containing the terminal word "<file_ending>":
                        while (!is_ending)
                        {
                            //Adds the last buffer's content to the file content.
                            fileContent += incomingMessage;

                            //If the file contains the terminal word "<file_ending>"
                            //or the length of the last buffer is less than 13, which is the length of the terminal word.
                            if (fileContent.Contains("<file_ending>") || incomingMessage.Length < 13)
                            {
                                is_ending = true;
                            }
                            else
                            {
                                //Cleans the content of the buffer.
                                buffer = new Byte[packetSize];

                                //Server receives the next buffer.
                                clientSocket.Receive(buffer);
                                incomingMessage = Encoding.Default.GetString(buffer);

                                //If the buffer is not full:
                                int ending_char = incomingMessage.IndexOf("\0");
                                if (ending_char != -1)
                                {
                                    //Updates cont.
                                    incomingMessage = incomingMessage.Substring(0, incomingMessage.IndexOf("\0"));
                                }
                            }
                        }
                        //After the terminal word is detected, it is removed from the file content string.
                        int ending_index = fileContent.LastIndexOf("<");
                        fileContent = fileContent.Substring(0, ending_index);

                        //Message showing the file is successfully created and saved.
                        logs.AppendText(downloadingFilename + " downloaded from server\n");

                        //The file in the given path is created and the content is written inside.
                        string savepath = folderPath + "\\" + downloadingFilename;
                        File.WriteAllText(savepath, fileContent);

                        button_Browse.Enabled = true;
                        button_GetFileList.Enabled = true;
                        button_GetPublicFileList.Enabled = true;

                    }
                    else if (receiveState == ReceiveState.Other)
                    {
                        if (incomingMessage == "<ACK>")
                        {
                            button_Browse.Enabled = true;
                            button_GetFileList.Enabled = true;
                            button_GetPublicFileList.Enabled = true;

                            //display the appropriate message
                            logs.AppendText("Success!\n");
                        }
                        else
                        {
                            //display the incomming message
                            logs.AppendText("Server: " + incomingMessage + "\n");
                        }
                    }
                    //we should never enter here
                    else
                    {
                        logs.AppendText("Something went horribly wrong!\n");
                    }
                }
                //we go into this catch if there was a problem while receiving
                catch
                {
                    //if we are not terminating and are still connected
                    if (!terminating && connected)
                    {
                        //display the appropriate message
                        logs.AppendText("The server has disconnected\n");

                        //change to button text to connect
                        button_Connect.Text = "Connect";

                        //disable the buttons
                        disableButtons();
                    }

                    //close the socket
                    clientSocket.Close();

                    //update the state
                    connected = false;
                }
            }
        }

        //handles the application closing
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //update the states
            connected = false;
            terminating = true;

            //close the app
            Environment.Exit(0);
        }

        //allows the user to browse files and select one
        private void button_Browse_Click(object sender, EventArgs e)
        {
            //open a file dialog
            OpenFileDialog folderBrowserDialog1 = new OpenFileDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                //save the selected filepath
                filepath = folderBrowserDialog1.FileName;

                //find the name of the file from the filepath
                int length = filepath.Length;
                int backslash_index = filepath.LastIndexOf("\\");
                filename = filepath.Substring(backslash_index + 1, filepath.Length - backslash_index - 1);

                //if it is a txt file
                if (filepath.Substring(length - 4, 4) == ".txt")
                {
                    //display the appropriate message
                    logs.AppendText("File selected: " + filename + "\n");

                    //enable sending
                    button_Send.Enabled = true;
                }
                //if it is not a txt file
                else
                {
                    //display the appropriate message
                    logs.AppendText(filename + " is not a valid file!\n");
                }
            }
        }

        //handles all file sending operations
        private void button_Send_Click(object sender, EventArgs e)
        {
            //disable the buttons so that the user cannot spam
            disableButtons();

            //send the operation type to the server
            string message = "<upload>";
            Byte[] buffer = new Byte[packetSize];
            //if (message != "" && message.Length <= packetSize)
            //{
            //    buffer = Encoding.Default.GetBytes(message);
            //    clientSocket.Send(buffer);
            //}

            //send the filename to the server
            message = filename;
            buffer = new Byte[packetSize];
            if (message != "" && message.Length <= packetSize)
            {
                buffer = Encoding.Default.GetBytes(message);
                clientSocket.Send(buffer);
            }

            try
            {
                //read the contents of the file into a string
                string file_content = File.ReadAllText(filepath);

                //compute the size of the last packet
                int last_batch_size = file_content.Length % packetSize;

                logs.AppendText("Uploading " + filename + " file to the server!\n");

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
                    //if it is not the last packet (fixed size = packetSize)
                    else
                    {
                        //get packetSize bytes
                        curr_batch = file_content.Substring(i * packetSize, packetSize);
                    }

                    //send the selected bytes to the user
                    buffer = Encoding.Default.GetBytes(curr_batch);
                    clientSocket.Send(buffer);
                }

                //send a message indicating end of file
                message = "<file_ending>";
                buffer = Encoding.Default.GetBytes(message);
                clientSocket.Send(buffer);

            }
            //we go into this catch if there was a problem while sending the contents of the file
            catch
            {
                //display the appropriate message
                logs.AppendText("Ran into a problem while sending the file!\n");

                //update the state
                connected = false;
            }
        }

        private void button_GetFileList_Click(object sender, EventArgs e)
        {
            //disable the buttons so that the user cannot spam
            disableButtons();
            receiveState = ReceiveState.FileList;

            //send the operation type to the server
            string message = "<filelist>";
            Byte[] buffer = new Byte[packetSize];
            if (message != "" && message.Length <= packetSize)
            {
                buffer = Encoding.Default.GetBytes(message);
                clientSocket.Send(buffer);
            }
        }

        private void button_BrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            //If the selected folder is appropriate, it prints out the message and enables listen button.
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
                logs.AppendText("Path selected as " + folderPath + "\n");
                button_Download.Enabled = true;
            }
        }

        private bool doesFilenameExist(string filename)
        {
            //loops over each file in the message filelist received from server, and looks whether the given file exists
            foreach (string serverFile in serverFiles)
            {
                int firstComma = serverFile.IndexOf(',');
                if (firstComma != -1)
                {
                    string serverFilename = serverFile.Substring(0, firstComma);
                    if (filename == serverFilename)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool doesClientExist(string clientname, string filename)
        {
            // checks whether the clientname exists or not
            foreach (string serverFile in serverPublicFiles)
            {
                int firstComma = serverFile.IndexOf(',');
                if (firstComma != -1)
                {
                    string serverClientname = serverFile.Substring(0, firstComma);
                    int secondComma = serverFile.IndexOf(',', firstComma + 1);
                    string clientFilename = serverFile.Substring(firstComma + 1, secondComma - firstComma - 1);
                    if (clientname == serverClientname && clientFilename == filename)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void button_Download_Click(object sender, EventArgs e)
        {
            //if the filename is valid
            if (textBox_Filename.Text != "" && textBox_Filename.Text.Length <= packetSize)
            {
                if (doesFilenameExist(textBox_Filename.Text))
                {
                    //disable the buttons so that the user cannot spam
                    disableButtons();

                    //send the operation type to the server
                    string message = "<download>";
                    Byte[] buffer = new Byte[packetSize];
                    if (message != "" && message.Length <= packetSize)
                    {
                        buffer = Encoding.Default.GetBytes(message);
                        clientSocket.Send(buffer);
                    }
                    message = textBox_Filename.Text;
                    string to_server = textBox_Username.Text + ":" + message;
                    buffer = new Byte[packetSize];
                    if (to_server != "" && to_server.Length <= packetSize)
                    {
                        logs.AppendText("Downloading " + message + " from server!\n");
                        receiveState = ReceiveState.Download;
                        downloadingFilename = message;
                        buffer = Encoding.Default.GetBytes(to_server);
                        clientSocket.Send(buffer);
                    }
                }

                else
                {
                    logs.AppendText("This filename does not exist!\n");
                }
            }

            else
            {
                logs.AppendText("Invalid filename!\n");
            }

        }

        private void button_Copy_Click(object sender, EventArgs e)
        {
            //if the filename is valid
            if (textBox_Filename.Text != "" && textBox_Filename.Text.Length <= packetSize)
            {
                if (doesFilenameExist(textBox_Filename.Text))
                {
                    //disable the buttons so that the user cannot spam
                    disableButtons();

                    //send the operation type to the server
                    string message = "<copy>";
                    Byte[] buffer = new Byte[packetSize];
                    if (message != "" && message.Length <= packetSize)
                    {
                        buffer = Encoding.Default.GetBytes(message);
                        clientSocket.Send(buffer);
                    }

                    //send the filename to the server
                    message = textBox_Filename.Text;
                    buffer = new Byte[packetSize];
                    if (message != "" && message.Length <= packetSize)
                    {
                        logs.AppendText("Copying " + message + " at the server!\n");
                        buffer = Encoding.Default.GetBytes(message);
                        clientSocket.Send(buffer);
                    }
                }
                else
                {
                    logs.AppendText("This filename does not exist!\n");
                }

            }
            else
            {
                logs.AppendText("Invalid filename!\n");
            }
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            //if the filename is valid
            if (textBox_Filename.Text != "" && textBox_Filename.Text.Length <= packetSize)
            {
                if (doesFilenameExist(textBox_Filename.Text))
                {
                    //disable the buttons so that the user cannot spam
                    disableButtons();

                    //send the operation type to the server
                    string message = "<delete>";
                    Byte[] buffer = new Byte[packetSize];
                    if (message != "" && message.Length <= packetSize)
                    {
                        buffer = Encoding.Default.GetBytes(message);
                        clientSocket.Send(buffer);
                    }

                    //send the filename to the server
                    message = textBox_Filename.Text;
                    buffer = new Byte[packetSize];
                    if (message != "" && message.Length <= packetSize)
                    {
                        logs.AppendText("Deleting " + message + " at the server!\n");
                        buffer = Encoding.Default.GetBytes(message);
                        clientSocket.Send(buffer);
                    }
                }
                else
                {
                    logs.AppendText("This filename does not exist!\n");
                }
            }
            else
            {
                logs.AppendText("Invalid filename!\n");
            }
        }

        private void button_GetPublicFileList_Click(object sender, EventArgs e)
        {
            //disable the buttons so that the user cannot spam
            disableButtons();
            receiveState = ReceiveState.PublicFileList;

            //send the operation type to the server
            string message = "<get_public>";
            Byte[] buffer = new Byte[packetSize];
            if (message != "" && message.Length <= packetSize)
            {
                buffer = Encoding.Default.GetBytes(message);
                clientSocket.Send(buffer);
            }
        }

        private void button_MakePublic_Click(object sender, EventArgs e)
        {
            //if the filename is valid
            if (textBox_Filename.Text != "" && textBox_Filename.Text.Length <= packetSize)
            {
                if (doesFilenameExist(textBox_Filename.Text))
                {
                    if (isPublic(textBox_Filename.Text))
                    {
                        logs.AppendText("File is public already! \n");
                    }
                    else // make public iff it exists and private
                    {
                        //disable the buttons so that the user cannot spam
                        disableButtons();

                        //send the operation type to the server
                        string message = "<make_public>";
                        Byte[] buffer = new Byte[packetSize];
                        if (message != "" && message.Length <= packetSize)
                        {
                            buffer = Encoding.Default.GetBytes(message);
                            clientSocket.Send(buffer);
                        }

                        //send the filename to the server
                        message = textBox_Filename.Text;
                        buffer = new Byte[packetSize];
                        if (message != "" && message.Length <= packetSize)
                        {
                            logs.AppendText("Making " + message + " public!\n");
                            buffer = Encoding.Default.GetBytes(message);
                            clientSocket.Send(buffer);
                        }
                    }
                }
                else
                {
                    logs.AppendText("This filename does not exist!\n");
                }

            }
            else
            {
                logs.AppendText("Invalid filename!\n");
            }
        }

        private bool isPublic(string filename)
        {
            // checks whether the file is already made public or not
            foreach (string file in serverFiles)
            {
                int firstComma = file.IndexOf(',');
                if (firstComma != -1)
                {
                    string fname = file.Substring(0, firstComma);
                    int lastComma = file.LastIndexOf(',');
                    string status = file.Substring(lastComma + 1);
                    // check the file info
                    if (status == "Public" && fname == filename)
                    {
                        return true;
                    }
                    else if (status == "Private" && fname == filename)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private void disableButtons()
        {
            //disable File buttons
            button_Browse.Enabled = false;
            button_Send.Enabled = false;

            //disable Private buttons
            button_GetFileList.Enabled = false;
            textBox_Filename.Enabled = false;
            button_BrowseFolder.Enabled = false;
            button_Download.Enabled = false;
            button_Copy.Enabled = false;
            button_Delete.Enabled = false;
            button_MakePublic.Enabled = false;

            //disable Public buttons
            button_GetPublicFileList.Enabled = false;
            textBox_publicFilename.Enabled = false;
            textBox_ClientName.Enabled = false;
            button_publicBrowseFolder.Enabled = false;
            button_publicDownload.Enabled = false;

        }

        private void button_publicDownload_Click(object sender, EventArgs e)
        {
            //if the filename is valid
            if (textBox_publicFilename.Text != "" && textBox_ClientName.Text != "" && textBox_publicFilename.Text.Length <= packetSize)
            {
                if (doesClientExist(textBox_ClientName.Text, textBox_publicFilename.Text))
                {
                    //disable the buttons so that the user cannot spam
                    disableButtons();

                    //send the operation type to the server
                    string message = "<download>";
                    Byte[] buffer = new Byte[packetSize];
                    if (message != "" && message.Length <= packetSize)
                    {
                        buffer = Encoding.Default.GetBytes(message);
                        clientSocket.Send(buffer);
                    }
                    message = textBox_publicFilename.Text;
                    string to_server = textBox_ClientName.Text + ":" + message;
                    buffer = new Byte[packetSize];
                    if (to_server != "" && to_server.Length <= packetSize)
                    {
                        logs.AppendText("Downloading " + message + " from server!\n");
                        receiveState = ReceiveState.Download;
                        downloadingFilename = message;
                        buffer = Encoding.Default.GetBytes(to_server);
                        clientSocket.Send(buffer);
                    }
                }
                else
                {
                    logs.AppendText("Client name or filename does not exist!\n");
                }
            }

            else
            {
                logs.AppendText("Invalid client name or filename!\n\n");
            }
        }

        private void button_publicBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            //If the selected folder is appropriate, it prints out the message and enables listen button.
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
                logs.AppendText("Path selected as " + folderPath + "\n");
                button_publicDownload.Enabled = true;
            }
        }
    }
}
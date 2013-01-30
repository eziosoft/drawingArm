using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


    class pltFile
    {
        public List<string> fileContent = new List<string>();
        public string filePath;
        public string fileName;

        public void LoadFile()
        {
            fileContent.Clear();
            filePath = SelectFileDialog(Application.StartupPath+"\\"); //programy\\c#\\ploter\\ploter rysunki");
            try
            {

                if (filePath == "") return;

                //fileName = filePath.Split('\\')[filePath.Split('\\').Length - 1];


                StreamReader streamReader = new StreamReader(filePath);

                string line = streamReader.ReadLine();
                while (line != null)
                {
                    if (line.Contains("PU") || line.Contains("PD"))
                    {
                        fileContent.Add(line);
                        //Read the next line
                    }
                    line = streamReader.ReadLine();

                }

                //close the file
                streamReader.Close();
            }
            catch
            {
            }
        }


        private string SelectFileDialog(string initialDirectory)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "plt files (*.plt)|*.plt|All files (*.*)|*.*";
            dialog.InitialDirectory = initialDirectory;
            dialog.Title = "Select a text file";
            return (dialog.ShowDialog() == DialogResult.OK)
               ? dialog.FileName : null;
        }
    }


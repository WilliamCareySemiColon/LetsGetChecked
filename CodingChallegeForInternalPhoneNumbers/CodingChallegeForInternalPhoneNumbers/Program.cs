using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodingChallegeForInternalPhoneNumbers
{
    class Program
    {
        //The inner class and property to capture the csv details and be used for writing to csv file
        public class CSVSPec
        {
            public String Name { get; set; }
            public String Address { get; set; }
            public String Phone { get; set; }
            public String Title { get; set; }
            public String Company { get; set; }
            public String ID { get; set; }
        }

        //The expected arguements is the exact path and assuming the folder which storing the file will be allocated is already created, 
        //as well as the input file exists already. If the outfile already exists, it will overwrite it
        static void Main(string[] args)
        {
            String FileOne, FileTwo;
            if (args.Length != 4)
            {
                Console.Write("Arguements needs to be in the format ");
                Console.WriteLine(" Directory-to-find-files subdirectory-for-converted-file input-file output-file");

                Environment.Exit(-1);

            }
            else
            {
                FileOne = args[0] + args[2];
                FileTwo = args[0] + args[1] + args[3];
            }

            Console.WriteLine("Argument length: " + args.Length);
            Console.WriteLine("Supplied Arguments are:");
            foreach (Object obj in args)
            {
                Console.WriteLine(obj);
            }


            //the variables to read in the contents of the csv file
            List<String>[] ListCollectionOfUSerDetails = new List<string>[]
            {
                new List<String>(),
                 new List<String>(),
                  new List<String>()
            };
            String coloumnnames = "";
            StreamReader streamReader = null;

            try
            {
                //the streamreader to take in the csv file and read it
                streamReader = new StreamReader(File.Open(FileOne, FileMode.Open));
            }
            catch(IOException)
            {
                Console.WriteLine("File not avaibale to read from");
                streamReader.Close();
                Environment.Exit(-1);

            }
            catch(Exception)
            {
                Console.WriteLine("Stream reader cannot be used at this time");
                streamReader.Close();
                Environment.Exit(-1);
            }
           

            //Start the reading of the csv file and break it up into three sections, one is the name of the users, one is the address and the last is all the other key details
            //including the phone number
            while(!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string[] values = line.Split("\"");

                    foreach(var item in values)
                    {
                        Console.WriteLine(item);
                    }
                    if(values.Length == 3)
                    {
                        for(int i = 0; i < 3;i++)
                        {
                            ListCollectionOfUSerDetails[i].Add(values[i]);
                        }
                    }
                    else if (values.Length ==1)
                    {
                        coloumnnames = values[0];
                    }
                  
                }
                else
                {
                    Console.WriteLine("Uncertainy of contents within the read in file");
                    streamReader.Close();
                    Environment.Exit(-1);
                }
            }

            //gathered all the contents so the streamreader can be closed
            streamReader.Close();

            //Outside the stringreader as we read all the key details needed, including the phone number
            List<String> UsersExtraDetails = ListCollectionOfUSerDetails[2];
            List<String> UserPhoneNumber = new List<string>();
            List<String> InternationalUserPhoneNumber = new List<string>();

            //filter out the phone number section
            foreach (String ExtraDetail in UsersExtraDetails)
            {
                string[] values = ExtraDetail.Split(",");

                foreach(var item in values)
                {
                    Console.Write(" " + item + ",");
                }
                Console.WriteLine("\n");

                UserPhoneNumber.Add(values[1]);
            }
            //convert the numbers from national to international format or keep them in internatiol format
            foreach (string phonenumber in UserPhoneNumber)
            {
                char[] test = phonenumber.Take(1).ToArray();

                if (test[0].Equals('0'))
                {
                    string internationalnum = "+353" + phonenumber.Substring(1, phonenumber.Length - 1);
                    InternationalUserPhoneNumber.Add(internationalnum);
                }
                else
                {
                    InternationalUserPhoneNumber.Add(phonenumber);
                }
            }

            //Capture all the items into a dingular list to write to a csv file
            List<CSVSPec> csvdata = new List<CSVSPec>();
            var CSVData = new CSVSPec[InternationalUserPhoneNumber.Count];
            for (int i = 0; i < CSVData.Length; i++)
            {
                var namevalues = ListCollectionOfUSerDetails[0].ToArray()[i].Split(",");

                var csvcontent = new CSVSPec();

                csvcontent.Name = namevalues[0];
                csvcontent.Address = ListCollectionOfUSerDetails[1].ToArray()[i];

                var extraDetails = UsersExtraDetails.ToArray()[i].Split(",");

                if (extraDetails.Length == 5){
                    csvcontent.ID = extraDetails[4];
                }

                if (extraDetails.Length >= 4){
                    csvcontent.Company = extraDetails[3];
                }

                if (extraDetails.Length >= 3){
                    csvcontent.Title = extraDetails[2];
                }

                csvcontent.Phone = InternationalUserPhoneNumber.ToArray()[i];

                csvdata.Add(csvcontent);
            }

            //Writing the files to the documents themselves

            StreamWriter stream = null;
            try
            {
                stream = File.CreateText(FileTwo);
            }
            catch(Exception)
            {
                Console.WriteLine("Unable to write to csv file");
                Environment.Exit(-1);
            }
            
            stream.WriteLine(coloumnnames);

            foreach (CSVSPec data in csvdata)
            {
                string csvrow = data.Name + ",\"" + data.Address + "\"," + data.Phone + "," + data.Title + "," + data.Company + "," + data.ID;
                stream.WriteLine(csvrow);
            }

            stream.Close();
            //Environment.Exit(0);

        }
    }
}


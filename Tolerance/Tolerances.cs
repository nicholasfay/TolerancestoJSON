/* $RCSfile: StepNCtoJSON.cs $
 * $Revision: 1.1 $ $Date: 2015/10/19 15:54:59 $
 * Auth: Nicholas Fay (fayn@rpi.edu)
 * 
 * 	Copyright (c) 1991-2015 by STEP Tools Inc.
 * 	All Rights Reserved
 * 
 * 	This software is furnished under a license and may be used and
 * 	copied only in accordance with the terms of such license and with
 * 	the inclusion of the above copyright notice.  This software and
 * 	accompanying written materials or any other copies thereof may
 * 	not be provided or otherwise made available to any other person.
 * 	No title to or ownership of the software is hereby transferred.
 * 
 * 		----------------------------------------
 * 
 *  Convert tolerances to JSON.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Web;
using System.Diagnostics;

/*Command Line argument checks for: 
    arg[0]: (Input .stpnc file name to be used by the program) Must be in same directory as cs file (MANDATORY)
    arg[1]: Output file to be created or populated with output data, path is added to current working directory ("%CurrentWorkingDirectory% \ arg[1]")
*/
namespace JSON
{

    class Tolerances
    {
        //Function to convert size dimensions and location dimensions to JSON
        static string convertBounded(string type, double value, double upper, double lower, string faces, string data, long geom, bool last)
        {
            string temp = "";
            if (!last)
            {
                temp = "{ \"type\" : " + "\"" + type + "\"" + ", " + "\"upper\" : " + upper
                    + ", " + " \"lower\" : " + lower + ", " + "\"value\" : " + value;
                if (faces != "")
                    temp = temp + ", \"faces\" : " + faces;
                if (data != "")
                    temp = temp + ", \"data\" : " + data;
                if (geom != 0)
                    temp = temp + ", \"geom\" : " + geom;

                temp = temp + "}," + "\n";
            }
            else {
                temp = "{ \"type\" : " + "\"" + type + "\"" + ", " + "\"upper\" : " + upper
                    + ", " + " \"lower\" : " + lower + ", " + "\"value\" : " + value;
                if (faces != "")
                    temp = temp + ", \"faces\" : " + faces;
                if (data != "")
                    temp = temp + ", \"data\" : " + data;
                if (geom != 0)
                    temp = temp + ", \"geom\" : " + geom;

                temp = temp + "}";
            }
            return temp;
        }

        //Function to convert geometric tolerances and surface texture parameters to JSON
        static string convertUnbounded(string type, double value, string faces, string data, long geom, bool last)
        {
            string temp = "";
            if (!last)
            {
                temp = "{ \"type\" : " + "\"" + type + "\"" + ", " + "\"value\" : " + value;
                if (faces != "")
                    temp = temp + ", \"faces\" : " + faces;
                if (data != "")
                    temp = temp + ", \"data\" : " + data;
                if (geom != 0)
                    temp = temp + ", \"geom\" : " + geom;

                temp = temp + "}," + "\n";
            }
            else
            {
                temp = "{ \"type\" : " + "\"" + type + "\"" + ", " + "\"value\" : " + value;
                if (faces != "")
                    temp = temp + ", \"faces\" : " + faces;
                if (data != "")
                    temp = temp + ", \"data\" : " + data;
                if (geom != 0)
                    temp = temp + ", \"geom\" : " + geom;

                temp = temp + "}";
            }
            return temp;
        }

        //Posts the json string to a server specified in the .Create function below
        static string httpPost(string json)
        {
            string result = "";
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json"; //Adds contenttype header
                result = client.UploadString("http://localhost:8080/testpage", "POST", json); //POST to param<0> with content param<2>
            }
            return result;
        }

        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("usage: StepNCtoJSON <input-file> optional: <output_file>");
                Environment.Exit(1);
            }
            //STEPNCLib.Finder find = new STEPNCLib.Finder();
            STEPNCLib.AptStepMaker apt = new STEPNCLib.AptStepMaker();
            STEPNCLib.Tolerance tol = new STEPNCLib.Tolerance();
            string curr_path = Directory.GetCurrentDirectory();
            bool test;
            string file = args[0];
            string out_dirFile = "";
            if (args.Length == 2)
            {
                out_dirFile = curr_path + args[1];
                test = true;
            }
            else
                test = false;

            StringBuilder builder = new StringBuilder();
            apt.Open238(file); //Open 238 file
            long count = tol.GetToleranceAllCount(); //Gets the total tolerance count
            double value, upper, lower;
            builder.Append("{ \"tolerances\": ["); //Starts the tolerances JSON string
            for (int i = 0; i < count; i++) //Loops through all tolerances one by one
            {
                bool last = false; //Valid Json error checking
                if (i == (count - 1))
                    last = true;

                //Get the next tolerance id and get its type/value
                long tol_id = tol.GetToleranceAllNext(i);
                /*string uu = apt.SetUUID_if_not_set(tol_id);*/ //UUID code that doesn't work til DLL is updated
                string type = tol.GetToleranceType(tol_id);
                value = tol.GetToleranceValue(tol_id);

                //Makes the list of faces that are associated to this tolerance if they exist
                long facc = tol.GetToleranceFaceCount(tol_id);
                long geom = 0;
                string faces = "";
                if (facc != 0)
                {
                    faces = "[";
                    for (int j = 0; j < facc; j++)
                    {
                        long face = tol.GetToleranceFaceNext(tol_id, j);
                        geom = tol.GetFaceMeasureGeometry(face);
                        if ((j + 1) == facc)
                            faces = faces + face;
                        else
                            faces = faces + face + ", ";
                        //Console.WriteLine(face + " ");
                    }
                    faces = faces + "]";
                }

                //Makes the list of datums that are associated to this tolerance if they exist
                long datc = tol.GetToleranceDatumCount(tol_id);
                string data = "";
                if (datc != 0)
                {
                    data = "[";
                    for (int j = 0; j < datc; j++)
                    {
                        long datum = tol.GetToleranceDatumNext(tol_id, j);
                        if ((j + 1) == datc)
                            data = data + datum;
                        else
                            data = data + datum + ", ";
                        //Console.WriteLine(datum + " ");
                    }
                    data = data + "]";
                }

                //Does something different depending on Tolerance Class
                if (tol.IsSizeDimension(tol_id)) //Size Dimension : and all subclasses
                {
                    tol.GetTolerancePlusMinus(tol_id, out lower, out upper);
                    string temp = convertBounded(type, value, upper, lower, faces, data, geom, last);
                    builder.Append(temp);
                }
                else if (tol.IsLocationDimension(tol_id)) //Location Dimension : and all subclasses
                {
                    tol.GetTolerancePlusMinus(tol_id, out lower, out upper);
                    string temp = convertBounded(type, value, upper, lower, faces, data, geom, last);
                    builder.Append(temp);
                } 
                else if (tol.IsGeometricTolerance(tol_id)) //Geometric Tolerance : and all subclasses
                {
                    string temp = convertUnbounded(type, value, faces, data, geom, last);
                    builder.Append(temp);
                }
                else if (tol.IsSurfaceTextureParameter(tol_id)) //Surface Texture Parameter : and all subclasses
                {
                    string temp = convertUnbounded(type, value, faces, data, geom, last);
                    builder.Append(temp);
                }
                else //Invalid Tolerance
                {
                    Console.WriteLine("Invalid Tolerance");
                }
            }
            //Console.ReadLine();
            builder.Append(" ]}"); //Ends the JSON string

            if (test) //If user wants output file
            {
                string output = builder.ToString();
                string res = httpPost(output);
                if (!Directory.Exists(out_dirFile)) //See if the path exists
                    Directory.CreateDirectory(Path.GetDirectoryName(out_dirFile)); //Create if not

               using (StreamWriter out_file = //StreamWrite output to file
                    new StreamWriter(File.Open(out_dirFile, FileMode.Create)))
                {
                    //out_file.WriteLine(output);
                    out_file.WriteLine(res);
                }

                ProcessStartInfo pf = new ProcessStartInfo("chrome.exe", "file:///C:/Users/Nick/Documents/Visual%20Studio%202015/Projects/ToleranceViewer/stepnc_hello_cs/test.html");
                Process.Start(pf);
            }
            else //Otherwise Write to command line
            {
                string output = builder.ToString();
                string res = httpPost(output);
                Console.WriteLine(output);
                Console.ReadLine();
            }
        }
    }
}
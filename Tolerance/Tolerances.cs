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
        
        static void Main(string[] args)
        {
            
            STEPNCLib.Finder find = new STEPNCLib.Finder();
            find.OpenProject("C:/Users/Nick/Documents/GitHub/StepNCViewer/data/boxy/boxy.stpnc");
            Console.WriteLine(find.GetMainWorkplan());
            long count = find.GetToolAllCount();
            long[] tools = new long[count];
            for(long i = 0; i < count; i++)
            {
                tools[i] = find.GetToolAllNext(i);
            }

            for(long i = 0; i < tools.Length; i++)
            {
                bool test = false;
                Console.WriteLine(find.GetToolReferenceDataName(tools[i], out test));
                Console.WriteLine(test);
            }

            Console.ReadLine();
        }
    }
}
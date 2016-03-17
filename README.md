# TolerancestoJSON
Convert stp file tolerances to consumable JSON


Command Line argument checks for: 

      arg[0]: (Input .stpnc file name to be used by the program) Must be in same directory as cs file (MANDATORY)
      
      arg[1]: Output file to be created or populated with output data, path is added to current working directory
      ("%CurrentWorkingDirectory% \ arg[1]"). Also must be a .html since code opens chrome browser to review result
      
Must run server solution first before running Tolerance Solution.

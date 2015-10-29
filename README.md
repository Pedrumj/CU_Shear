# CU_Shear
Gets the shear in a cast unit, from multiple CSI SAP models using the SAP OAPI in VB.NET.

There were 2 reasons for writing this program:

1.	When designing structures, the engineer normally prefers to view the cast unit as a whole rather than as separate elements. CSI SAP does not give the engineers this option. This program will retrieve data associated with an entire cast unit. Later these values can be exported to a spreadsheet and viewed there. 
2.	Often a design will require forces from multiple files. This is specially the case when there are seismic loads involved. Some specification may require designing without shear walls under reduced seismic loads in addition to the normal analysis. This program gives the user the option to fetch results from multiple SAP models

#Requirements
In order for this program to run CSI SAP 2000 V16 must be installed on the system. The program should work for other SAP versions, by either:
<br>1-	Replacing all instances of <code>SAP2000v16 </code> with the appropriate namespace.
<br>or
<br>2-	Changing the code from early binding to late binding. 

#How to:
<b>Step 1</b>: You will need to create a new SapStructure object. There are several inputs to the constructor:
- __strPath: The path to one of the SAP model files.
- __X: The X coordinate of a point on the cast unit
- __Y: The Y coordinate of a point on the cast unit.
- __Z: The Z coordinate of a point on the cast unit. 
- __type: The type of cast unit, beam or column. Columns are assumed to be in the Z direction and beams are assumed to be in either the X or Y direction. Note for columns the __Z parameter is irrelevant. For beams depending on the direction the __X or __Y parameters will be irrelevant. 
- __dir: X or Y. This is only relevant for beams. 

<br><b>Step 2:</b>
For each model call the <code>GetResults_LoadCases</code> function. This will fetch the forces from the sap analysis. 

<br><b>Step 3:</b>
when finished working with the program release resources by calling <code>CloseApplication</code>

#Output
The SapStructure object provides 3 output:
<br><b>Forces:</b> This will be a list of force objects. Each object will be at one station of the cast unit. Note that there are multiple models and multiple load cases in each station. 
<br>The following code will print out the shear in the V2 direction at the first station of the cast unit for all models and load cases:
 <pre>
  'loop through load cases
  For i As Integer = 0 To Me.Forces.Item(0).V2.Cases.Count - 1
      'loop through models
      For j As Integer = 0 To Me.Forces.Item(0).V2.Cases.Item(i).Force_Infos.Count - 1
          Console.WriteLine(Me.Forces.Item(0).V2.Cases.Item(i).Force_Infos.Item(j).Value)
      Next
  Next
</pre>

<br><b>T3:</b> The T3 dimension of the cast unit at each station.
<br><b>T2:</b> The T2 dimension of the cast unit at each station.

#Output
This output below was generated by the program above. 
- **Type:** The type was specified to be a beam
- **Direction:** The beam direction was in the X direction
- **Elevation:** This is the Z parameter.
- **Y:** The beam was in the X direction
- **Element:** The values in this row specify the element being printed at each station. Note that each column is the value at a station
- **Max(row 9):** The maximum value of the V2 shear among all models for the DEAD load case
 

![Output](http://jtech-online.com/wp-content/uploads/2015/10/Output.png)


# License
[The MIT License (MIT)](http://opensource.org/licenses/MIT)

using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GCodeObjectClass;


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public abstract class Script_Instance_562ed : GH_ScriptInstance
{
  #region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { /* Implementation hidden. */ }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { /* Implementation hidden. */ }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { /* Implementation hidden. */ }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
  #endregion

  #region Members
  /// <summary>Gets the current Rhino document.</summary>
  private readonly RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private readonly GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private readonly IGH_Component Component;
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private readonly int Iteration;
  #endregion
  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments,
  /// Output parameters as ref arguments. You don't have to assign output parameters,
  /// they will have a default value.
  /// </summary>
  #region Runscript
  private void RunScript(List<string> gCode, ref object paths, ref object isTravel, ref object fValues, ref object eValues, ref object linesIndex)
  {

    //Construct GCodeLine Objects from .gcode strings
    GCodeLine[] test01 = new GCodeLine[gCode.Count];
    for (int i = 0; i < gCode.Count; i++)
    {
      test01[i] = new GCodeLine(gCode[i]);
    }

    //Construct GCode Object
    GCode g01 = new GCode(test01);

    //lists for collecting output data from g01 GCodeLines (where HasPt = true)
    List<Point3d> pts = new List<Point3d>();
    List<Line> glines = new List<Line>();
    List<bool> gIsTravels = new List<bool>();
    List<bool> gIsExtruding = new List<bool>();
    List<double> eVals = new List<double>();
    List<double> fVals = new List<double>();
    List<int> gIndex = new List<int>();
    List<double> eValsShifted = new List<double>();
    List<double> fValsShifted = new List<double>();
    List<int> gIndexShifted = new List<int>();
    //Populate lists with data from g01 GCodeLines (where HasPt = true)
    for (int i = 0; i < g01.Length; i++)
    {
      if (g01.Lines[i].HasPt)
      {
        Point3d pt = new Point3d(g01.Lines[i].X, g01.Lines[i].Y, g01.Lines[i].Z);
        pts.Add(pt);
        gIsExtruding.Add(g01.Lines[i].IsExtruding);
        eVals.Add(g01.Lines[i].E);
        fVals.Add(g01.Lines[i].F);
        gIndex.Add(g01.Lines[i].Index);
      }
    }
    //Generate list of lines from valid points in GCode g01
    for (int i = 0; i < (pts.Count - 1); i++)
    {
      Line ln = new Line(pts[i], pts[i + 1]);
      glines.Add(ln);
      bool iT = false;
      if (gIsExtruding[i + 1] == true)
      {
        iT = false;
      }
      else
      {
        iT = true;
      }
      gIsTravels.Add(iT);
      eValsShifted.Add(eVals[i + 1]);
      fValsShifted.Add(fVals[i + 1]);
      gIndexShifted.Add(gIndex[i + 1]);
    }

    //Outputs
    paths = glines;
    isTravel = gIsTravels;
    eValues = eValsShifted;
    fValues = fValsShifted;
    linesIndex = gIndexShifted;
  }
  #endregion
  #region Additional

  #endregion
}
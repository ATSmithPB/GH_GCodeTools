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
public abstract class Script_Instance_7dfa4 : GH_ScriptInstance
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
  private void RunScript(List<string> settings, List<Line> paths, List<bool> isTravel, List<double> fValues, List<double> eValues, ref object gCode)
  {
    //Sanity
    if (settings[0] != "::START::")
    {
      Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Settings Start Format is Incorrect");
      return;
    }
    if (isTravel.Count != paths.Count || isTravel.Count != fValues.Count || isTravel.Count != eValues.Count)
    {
      Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "paths, isTravel, fValues, and eValues do not have matching data structure and count");
    }

    //Construct Start and End Lines of GCode 
    List <GCodeLine> StartLines = new List<GCodeLine>();
    List<GCodeLine> EndLines = new List<GCodeLine>();
    bool isStart = true;
    for (int i = 0; i < settings.Count; i++)
    {
      if (settings[i] == "::END::")
      {
        isStart = false;
        i++;
      }
      else if (isStart)
      {
        StartLines.Add(new GCodeLine(settings[i]));
      }
      else
      {
        EndLines.Add(new GCodeLine(settings[i]));
      }
    }

    if (fValues.Count != paths.Count)
    {
      for (int i = 1; i < paths.Count; i++)
      {
        fValues.Add(fValues[0]);
      }
    }

    for (int i = 0; i < paths.Count; i++)
    {
      GCodeLine test = new GCodeLine(paths[i], fValues[i], eValues[i], isTravel[i]);
    }
  }
  #endregion
  #region Additional

  #endregion
}
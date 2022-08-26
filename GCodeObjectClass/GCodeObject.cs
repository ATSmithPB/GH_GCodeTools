using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace GCodeObjectClass
{
    /// <summary>
    /// Represents a Line of GCode
    /// </summary>
    public class GCodeLine
    {
        //Initial Values
        public GCodeLine()
        {
            X = double.NaN;
            Y = double.NaN;
            Z = double.NaN;
            E = double.NaN;
            F = 0;
            Code = "";
            Line = "";
            Index = 9999;
            Notes = "";
            HasCoord = false;
            HasPt = false;
            IsExtruding = false;
        }
        //Properties
        /// <summary>
        /// full string data of GCodeLine
        /// </summary>
        public string Line { get; set; }
        /// <summary>
        /// GCode command code for line (if available)
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// X coord. of GCodeLine (if available)
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Y coord. of GCodeLine (if available)
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Z coord. of GCodeLine (if available)
        /// </summary>
        public double Z { get; set; }
        /// <summary>
        /// E-value (mm of filiment to extrude at current feedrate)
        /// </summary>
        public double E { get; set; }
        /// <summary>
        /// Current Feedrate in mm/min
        /// </summary>
        public double F { get; set; }
        /// <summary>
        /// Index of GCodeLine object within a GCode object
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// optional notes following a ';' in a GCodeLine (if available)
        /// </summary>
        public string Notes { get; set; }
        /// <summary>
        /// GCodeLine data preculding any notes
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// bool value, true if GCodeLine has ANY X Y or Z coordinate data
        /// </summary>
        public bool HasCoord { get; set; }
        /// <summary>
        /// bool value, true if GCodeLine has ALL valid X Y and Z coordinate data
        /// </summary>
        public bool HasPt { get; set; }
        /// <summary>
        /// bool value, true if GCodeLine is extruding filament
        /// </summary>
        public bool IsExtruding { get; set; }

        //Constructors
        /// <summary>
        /// Constructs a GCodeLine object from a single line of G-code
        /// </summary>
        /// <param name="codeLine"></param>
        public GCodeLine(string codeLine)
        {
            this.Line = codeLine;
            string codeLineNoNotes;
            //Discretize string
            if (codeLine.Contains(";"))
            {
                string[] codeLineNotesSplit = codeLine.Split(';');
                this.Notes = codeLineNotesSplit[1].Trim();
                this.Command = codeLineNotesSplit[0].Trim();
                codeLineNoNotes = codeLineNotesSplit[0];
            }
            else
            {
                codeLineNoNotes = codeLine;
                this.Command = codeLine;
                this.Notes = "";
            }
            if (codeLineNoNotes.StartsWith("G1"))
            {
                this.Code = "G1";
                string[] G1Elements = codeLineNoNotes.Split(' ');
                for (int i = 0; i < G1Elements.Length; i++)
                {
                    if (G1Elements[i].StartsWith("X"))
                    {
                        this.X = Convert.ToDouble(G1Elements[i].Remove(0, 1));
                    }
                    else if (G1Elements[i].StartsWith("Y"))
                    {
                        this.Y = Convert.ToDouble(G1Elements[i].Remove(0, 1));
                    }
                    else if (G1Elements[i].StartsWith("Z"))
                    {
                        this.Z = Convert.ToDouble(G1Elements[i].Remove(0, 1));
                    }
                    else if (G1Elements[i].StartsWith("E"))
                    {
                        this.E = Convert.ToDouble(G1Elements[i].Remove(0, 1));
                    }
                    else if (G1Elements[i].StartsWith("F"))
                    {
                        this.F = Convert.ToDouble(G1Elements[i].Remove(0, 1));
                    }
                }
            }
            if (this.X + this.Y + this.Z != 0.0)
            {
                this.HasCoord = true;
            }
            if (this.X != 0.0 && this.Y != 0.0 && this.Z != 0.0)
            {
                this.HasPt = true;
            }
            if (this.E != 0.0)
            {
                this.IsExtruding = true;
            }
        }

        public GCodeLine(Line ln, double f, double e, bool iT)
        {
            this.X = Math.Round(ln.To.X,3);
            this.Y = Math.Round(ln.To.Y, 3);
            this.Z = Math.Round(ln.To.Z, 3);
            this.F = Math.Round(f, 0);
            this.IsExtruding = !iT;
            if (iT)
            {
                this.E = 0.0;
            }
            else
            {
                this.E = Math.Round(e, 5);
            }
            this.HasCoord = true;
            this.HasPt = true;
            this.Code = "G1";
            this.Line = $"G1 X{this.X} Y{this.Y} Z{this.Z} E{this.E} F{this.F}";
        }

        //GCodeLine Methods
        /// <summary>
        /// Returns linear distance between two GCodeLine coordinates (if both lines have coord. data)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public double Distance(GCodeLine a, GCodeLine b)
        {
            if (a.HasPt == false || b.HasPt == false)
            {
                throw new Exception("Cannot calculate distance. One or More G Code lines do not contain coordinate data");
            }
            else
            {
                double dist = Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));
                return dist;
            }
        }
    }

    /// <summary>
    /// an an array of GCodeLines
    /// </summary>
    public class GCode
    {
        //Initial Values
        public GCode()
        {
            Length = 0;
        }

        //Properties
        public GCodeLine[] Lines { get; set; }
        public int Length { get; set; }
        public Point3d[] Points { get; set; }

        //Constructors
        /// <summary>
        /// Constructs a GCode Object from an array of GCodeLines
        /// </summary>
        /// <param name="gCode"></param>
        public GCode(GCodeLine[] gCode)
        {
            //Populate GCode object with GCodeLine objects
            this.Lines = gCode;
            this.Length = gCode.Length;

            //Set Index Values for GCodeLines in GCode
            for (int i = 0; i < Length; i++)
            {
                this.Lines[i].Index = i;
            }

            //Sets missing coordinate data with current X,Y, and Z Values for all GCodeLine ojects in GCode
            double currentZ = 0.0;
            double currentY = 0.0;
            double currentX = 0.0;
            double currentF = 0.0;
            for (int i = 0; i < this.Length; i++)
            {
                if (this.Lines[i].HasCoord)
                {
                    //Parse and replace Z Values
                    if (this.Lines[i].Z != 0.0)
                    {
                        currentZ = this.Lines[i].Z;
                    }
                    else if (this.Lines[i].Z == 0.0)
                    {
                        this.Lines[i].Z = currentZ;
                    }
                    //Parse and replace Y Values
                    if (this.Lines[i].Y != 0.0)
                    {
                        currentY = this.Lines[i].Y;
                    }
                    else if (this.Lines[i].Y == 0.0)
                    {
                        this.Lines[i].Y = currentY;
                    }
                    //Parse and replace X Values
                    if (this.Lines[i].X != 0.0)
                    {
                        currentX = this.Lines[i].X;
                    }
                    else if (this.Lines[i].X == 0.0)
                    {
                        this.Lines[i].X = currentX;
                    }
                    //Update HasPt property for all GCodeLines in GCode
                    if (this.Lines[i].X != 0.0 && this.Lines[i].Y != 0.0 && this.Lines[i].Z != 0.0 && this.Lines[i].HasPt == false)
                    {
                        this.Lines[i].HasPt = true;
                    }
                }

                //Sets Feedrate for all G1 lines
                if (this.Lines[i].F != 0)
                {
                    currentF = this.Lines[i].F;
                }
                else
                {
                    this.Lines[i].F = currentF;
                }
            }
        }

        //GCode Class Methods
        /// <summary>
        /// Returns an array of GCode Line data for all GCodeLines in a GCode object
        /// </summary>
        /// <returns></returns>
        public string[] GetLines()
        {
            string[] val = new string[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                val[i] = this.Lines[i].Line;
            }
            return val;
        }
        /// <summary>
        /// Returns an array of X values for all GCodeLines in a GCode object
        /// </summary>
        /// <returns></returns>
        public double[] GetX()
        {
            double[] val = new double[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                val[i] = this.Lines[i].X;
            }
            return val;
        }

        /// <summary>
        /// Returns an array of Y values for all GCodeLines in a GCode object
        /// </summary>
        /// <returns></returns>
        public double[] GetY()
        {
            double[] val = new double[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                val[i] = this.Lines[i].Y;
            }
            return val;
        }

        /// <summary>
        /// Returns an array of Z values for all GCodeLines in a GCode object (you must run SetZ method before GetZ)
        /// </summary>
        /// <returns></returns>
        public double[] GetZ()
        {
            double[] val = new double[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                val[i] = this.Lines[i].Z;
            }
            return val;
        }

        /// <summary>
        /// Returns an array of F values for all GCodeLines in a GCode object
        /// </summary>
        /// <returns></returns>
        public double[] GetF()
        {
            double[] val = new double[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                val[i] = this.Lines[i].F;
            }
            return val;
        }

        /// <summary>
        /// Returns an array of E values for all GCodeLines in a GCode object
        /// </summary>
        /// <returns></returns>
        public double[] GetE()
        {
            double[] val = new double[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                val[i] = this.Lines[i].E;
            }
            return val;
        }

        /// <summary>
        /// Returns an array HasCoord bools (true if GCodeLine contains coordinate data)
        /// </summary>
        /// <returns></returns>
        public bool[] GetHasCoord()
        {
            bool[] val = new bool[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                val[i] = this.Lines[i].HasCoord;
            }
            return val;
        }

        /// <summary>
        /// Returns an array of HasPt bools (true if GCodeLine contains all XY and Z coordinate data)
        /// </summary>
        /// <returns></returns>
        public bool[] GetHasPt()
        {
            bool[] val = new bool[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                val[i] = this.Lines[i].HasPt;
            }
            return val;
        }

        public bool[] GetIsExtruding()
        {
            bool[] val = new bool[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                val[i] = this.Lines[i].IsExtruding;
            }
            return val;
        }

        /// <summary>
        /// Returns a list of all valid points in the GCode object
        /// </summary>
        /// <returns></returns>
        public Point3d?[] GetPts()
        {
            Point3d?[] pts = new Point3d?[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                if (this.Lines[i].HasPt)
                {
                    Point3d? pt = new Point3d(this.Lines[i].X, this.Lines[i].Y, this.Lines[i].Z);
                    pts[i] = pt;
                }
                else
                {
                    pts[i] = null;
                }
            }
            return pts;
        }
    }
}

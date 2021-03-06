﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ConvertToKicad
{
    public partial class ConvertPCBDoc
    {
        // see if module is in module list
        static bool InList(Module mod, List<Module> Modules)
        {
            foreach (var Mod in Modules)
            {
                if (Mod.Name == mod.Name)
                    return true;
            }
            return false;
        }

        // class for module objects
        class Module : Object
        {
            static int ID = 0;
            public string Name { get; set; }
            public string Layer { get; set; }
            string Tedit { get; set; }
            string Tstamp { get; set; }
            public string Designator { get; set; }
            public bool DesignatorOn { get; set; }
            public string Comment { get; set; }
            public bool CommentOn { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            string Path { get; set; }
            string Attr { get; set; }
            public double Rotation { get; set; }
            // primitives
            public ObjectList<Line> Lines { get; set; }
            public ObjectList<Pad> Pads { get; set; }
            public ObjectList<String> Strings { get; set; }
            public ObjectList<Via> Vias { get; set; }
            public ObjectList<Arc> Arcs { get; set; }
            public ObjectList<Fill> Fills { get; set; }
            public ObjectList<Polygon> Polygons { get; set; }
            public ObjectList<ComponentBody> ComponentBodies { get; set; }
            public ObjectList<ShapeBasedModel> ShapeBasedModels { get; set; }
            public ObjectList<Region> Regions { get; set; }

            public Module()
            {
            }

            public Module(string line)
            {
                string param;
                Name = GetString(line, "|PATTERN=");
                if (Name.Contains("\\"))
                    Name = Name.Replace("\\", "_"); // TODO check this out
                if (Name.Contains("\""))
                    Name = Name.Replace("\"", "_");
                if ((param = GetString(line, "|X=").Trim(charsToTrim)) != "")
                {
                    X = GetCoordinateX(param);
                }
                if ((param = GetString(line, "|Y=").Trim(charsToTrim)) != "")
                {
                    Y = GetCoordinateY(param);
                }
                Layer = "";
                if ((param = GetString(line, "|LAYER=")) != "")
                {
                    Layer = Brd.GetLayer(param);
                }
                DesignatorOn = true;
                if ((param = GetString(line, "|NAMEON=")) != "")
                {
                    DesignatorOn = param == "TRUE";
                }
                CommentOn = true;
                if ((param = GetString(line, "|COMMENTON=")) != "")
                {
                    CommentOn = param == "TRUE";
                }
                Rotation = 0;
                if ((param = GetString(line, "|ROTATION=").Trim(charsToTrim)) != "")
                {
                    Rotation = Convert.ToDouble(param);
                    if (Rotation == 360)
                        Rotation = 0;
                }
                if (Layer == "F.Cu" || Layer == "B.Cu")
                    Attr = "smd";
                else
                    Attr = "thru_hole";

                Tedit = "(tedit 0)";
                Tstamp = "(tstamp 0)";
                // create the object lists for this component
                Lines = new ObjectList<Line>();
                Pads = new ObjectList<Pad>();
                Strings = new ObjectList<String>();
                ViasL = new ObjectList<Via>();
                Arcs = new ObjectList<Arc>();
                Fills = new ObjectList<Fill>();
                Polygons = new ObjectList<Polygon>();
                Regions = new ObjectList<Region>();
                ComponentBodies = new ObjectList<ComponentBody>();
                ShapeBasedModels = new ObjectList<ShapeBasedModel>();
                ID++; // update for next Module
            }

            // constructor for free pads sconverted to modules
            public Module(string name, double x, double y, double xsize, double ysize, Pad Pad)
            {
                Name = name;
                Tedit = "(tedit 0)";
                Tstamp = "(tstamp 0)";
                ID = 0;
                Layer = "F.Cu";
                Designator = "";
                DesignatorOn = false;
                Comment = "";
                CommentOn = false;
                X = x;
                Y = y;
                Rotation = 0;
                Pads = new ObjectList<Pad>
                {
                    Pad
                };
            }

            void AddLine(Line line)
            {
                Lines.Add(line);
            }

            void AddPad(Pad pad)
            {
                Pads.Add(pad);
            }

            void AddText(String str)
            {
                Strings.Add(str);
            }

            public class PadComparer : IComparer<Pad>
            {
                public int Compare(Pad x, Pad y)
                {
                    if (x == null)
                    {
                        if (y == null)
                        {
                            // If x is null and y is null, they're
                            // equal. 
                            return 0;
                        }
                        else
                        {
                            // If x is null and y is not null, y
                            // is greater. 
                            return -1;
                        }
                    }
                    else
                    {
                        // If x is not null...
                        //
                        if (y == null)
                        // ...and y is null, x is greater.
                        {
                            return 1;
                        }
                        else
                        {
                            // sort them with ordinary string comparison.
                            //
                            return x.Number.CompareTo(y.Number);
                        }
                    }
                }
            }


            public override string ToString()
            {
                //    OutputString($"outputting {Name}");
                try
                {
                    string ret = "";
                    ret = $"  (module \"{Name}\" (layer {Layer}) {Tedit} {Tstamp}\n";
                    ret += $"    (at {X} {-Y} {Rotation})\n";
                    ret += $"    (attr {Attr})\n";

                    // this bit is for a particular test board where the idiot had put the Comment as .designator
                    if (Strings != null)
                        foreach (var str in Strings)
                        {
                            if (str.Value.ToLower() == ".comment")
                                str.Value = Comment;
                            if (str.Value.ToLower() == ".designator")
                                str.Value = Designator;
                        }

                    if (Strings != null) ret += Strings.ToString(X, Y, Rotation);

                    PadComparer pc = new PadComparer();
                    // put pads in numerical order (not really necessary)
                    Pads.Sort(pc);

                    if (Pads != null) ret += Pads.ToString(X, Y, Rotation);
                    // ret += Vias.ToString(X, Y, Rotation); // vias not allowed in modules...yet
                    if (Lines != null) ret += Lines.ToString(X, Y, -Rotation);
                    if (Arcs != null) ret += Arcs.ToString(X, Y, -Rotation);
                    if (Fills != null) ret += Fills.ToString(X, Y, -Rotation);
                    if (Polygons != null) ret += Polygons.ToString();
                    if (Regions != null) ret += Regions.ToString(X, Y, -Rotation);
                    CurrentLayer = Layer;
                    if (ComponentBodies != null) ret += ComponentBodies.ToString(X, Y, Rotation); // (Layer=="F.Cu")?-Rotation:-(Rotation-180));
                    if (ShapeBasedModels != null) ret += ShapeBasedModels.ToString(X, Y, -Rotation);
                    ret += "  )\n";
                    return ret;
                }
                catch (Exception Ex)
                {
                    CheckThreadAbort(Ex);
                    return "";
                }
            }

            // output the module as a Library component
            public string ToModule()
            {
                string ret = "";
                ret = $"  (module \"{Name}\" (layer {Layer}) {Tedit} {Tstamp}\n";
                ret += $"  (descr \"\")";
                ret += $"  (tags \"\")";
                if (Attr != "") ret += $"  (attr {Attr})\n";

                foreach (var String in Strings)
                {
                    string str;

                    str = String.ToString(X, Y, Rotation);

                    if (str.Contains("fp_text reference"))
                    {
                        str = String.ToRefString(X, Y, Rotation);
                    }

                    ret += str;
                }
                foreach (var Pad in Pads)
                {
                    ret += Pad.ToModuleString(X, Y, Rotation);
                }
                foreach (var Line in Lines)
                {
                    ret += Line.ToString(X, Y, Rotation);
                }
                // Vias in components are done as pads
                //                foreach (var Via in Vias)
                //                {
                //                    ret += Via.ToString(X, Y);
                //                }
                foreach (var Arc in Arcs)
                {
                    ret += Arc.ToString(X, Y, Rotation);
                }
                foreach (var Polygon in Polygons)
                {
                    ret += Polygon.ToString();
                }
                foreach (var Fill in Fills)
                    ret += Fill.ToString(X, Y, -Rotation);
                foreach (var region in Regions)
                    ret += region.ToString(X, Y, -Rotation);
                foreach (var ComponentBody in ComponentBodies)
                {
                    ret += ComponentBody.ToString(X, Y, -Rotation);
                }
                ret += "  )\n";
                return ret;
            }
        }
    }
}

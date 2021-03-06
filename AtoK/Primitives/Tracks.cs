﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ConvertToKicad
{
    public partial class ConvertPCBDoc
    {
        // class for track objects
        class Line : PCBObject
        {
            double X1 { get; set; }
            double Y1 { get; set; }
            double X2 { get; set; }
            double Y2 { get; set; }
            string Layer { get; set; }
            double Width { get; set; }
            bool Segment { get; set; }
            int Net { get; set; }
            bool InComponent { get; set; }

            private Line()
            {
                X1 = 0;
                Y1 = 0;
                X2 = 0;
                Y2 = 0;
                Layer = "";
                Width = 0;
                InComponent = false;
            }

            public Line(double x1, double y1, double x2, double y2, string layer, double width)
            {
                X1 = Math.Round(x1, Precision);
                Y1 = Math.Round(y1, Precision);
                X2 = Math.Round(x2, Precision);
                Y2 = Math.Round(y2, Precision);
                Layer = layer;
                Width = Math.Round(width, Precision);
                Net = 0;
                InComponent = true;
            }

            public Line(double x1, double y1, double x2, double y2, string layer, double width, int net)
            {
                X1 = x1;
                Y1 = y1;
                X2 = x2;
                Y2 = y2;
                Layer = layer;
                Width = width;
                Net = net;
                InComponent = false;
            }

            public override string ToString()
            {
                track_count++;
                string type;
                string net;
                if (Net == 0)
                {
                    net = "";
                    type = "gr_line";
                }
                else
                {
                    net = $"(net {Net})";
                    type = "segment";
                }
                return $"  ({type} (start {X1} {-Y1}) (end {X2} {-Y2}) (width {Width}) (layer {Layer}) {net})\n";
            }

            public string ToString(double x, double y)
            {
                // convert to relative to component origin
                Point2D p1 = new Point2D(X1 - x, Y1 - y);
                Point2D p2 = new Point2D(X2 - x, Y2 - y);

                // create line relative to component origin
                return $"    (fp_line (start {p1.X} {-p1.Y}) (end {p2.X} {-p2.Y}) (layer {Layer}) (width {Width}))\n";
            }

            public override string ToString(double x, double y, double ModuleRotation)
            {
                // convert to relative to component origin
                Point2D p1 = new Point2D(X1 - x, Y1 - y);
                Point2D p2 = new Point2D(X2 - x, Y2 - y);

                p1.Rotate(ModuleRotation);
                p2.Rotate(ModuleRotation);

                // create line relative to component origin
                 return $"    (fp_line (start {p1.X} {-p1.Y}) (end {p2.X} {-p2.Y}) (layer {Layer}) (width {Width}))\n";
            }
        }

        // class for the tracks document entry in the pcbdoc file
        class Tracks : PcbDocEntry
        {
            // record length varies
            public Tracks(string filename, string cmfilename, string record, Type type, int offset) : base(filename, cmfilename, record, type, offset)
            {
                if (CMFile)
                    Binary_size = 50;
                else
                    Binary_size = 46;
            }

            public override bool ProcessLine(byte[] line)
            {
                double X1, Y1, X2, Y2, width;
                int net;
                Layers layer;
                bool InComponent = false;
                Int16 component = 0;
                base.ProcessLine();
                net = B2UInt16(line, 3 + 5);
                if (net == 0x0000FFFF)
                    net = 0;
                else
                    net++;
                component = B2Int16(line, 7 + 5);
                InComponent = (component != -1);
                X1 = Math.Round(ToMM(line, 13 + 5) - originX, Precision);
                Y1 = Math.Round(ToMM(line, 17 + 5) - originY, Precision);
                X2 = Math.Round(ToMM(line, 21 + 5) - originX, Precision);
                Y2 = Math.Round(ToMM(line, 25 + 5) - originY, Precision);

                CheckMinMax(X1 + originX, Y1 + originY);
                CheckMinMax(X2 + originX, Y2 + originY);
                width = Math.Round(ToMM(line, 29 + 5), Precision);

                layer = (Layers)line[5];
                string Layer = Brd.GetLayer(layer);
                int ComponentNumber = 0;
                if (InComponent)
                {
                    // belongs to a component definition
                    ComponentNumber = component;
                }
                // check for and reject very short tracks
                if (Length(X1, Y1, X2, Y2) <= 0.001)
                {
                    OutputError($"Zero length track rejected at X1={X1} Y1={-Y1} X2={X2} y2={-Y2} ");
                    return true;
                }
                if (!InComponent)
                {
                    if (net == 0)
                    {
                        if (!Brd.OnInnerLayer(layer))
                        {
                            if ((Layer != "Edge.Cuts") || !Brd.CheckExistingLine(X1, -Y1, X2, -Y2))
                            {
                                List<string> Layers = Brd.GetLayers(Layer);
                                foreach (var L in Layers)
                                {
                                    Line Line = new Line(X1, Y1, X2, Y2, L, width, net);
                                    LinesL.Add(Line);
                                }                              
                            }
                        }
                    }
                    else
                    {
                        Line Line = new Line(X1, Y1, X2, Y2, Brd.GetLayer(layer), width, net);
                        LinesL.Add(Line);
                        //tracks.Append($"  (segment (start {X1} {-Y1}) (end {X2} {-Y2}) (width {width}) (layer {Layer}) (net {net}))\n");
                        //track_count++;
                    }
                }
                else
                {
                    Line Line = new Line(X1, Y1, X2, Y2, Brd.GetLayer(layer), width);
                    ModulesL[ComponentNumber].Lines.Add(Line);
                }
                return true;
            }
        }
    }
}

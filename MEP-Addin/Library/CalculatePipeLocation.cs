using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MEP_Addin.Library
{
    public class CalculatePipeLocation
    {
        #region Exam
        private static Line GetLineProjectOfPipe(Pipe pipe)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ p0 = line.GetEndPoint(0);
            XYZ p1 = line.GetEndPoint(1);
            double Zmin = Math.Min(p0.Z, p1.Z);
            XYZ p0A = new XYZ(p0.X, p0.Y, Zmin);
            XYZ p1A = new XYZ(p1.X, p1.Y, Zmin);
            return Line.CreateBound(p0A, p1A);
        }
        private static XYZ TranformPointPlumbingFixture(Connector connector, Pipe pipe)
        {
            XYZ origin = connector.Origin;
            return new XYZ(origin.X, origin.Y, GetZMinPipe(pipe));
        }
        private static double GetZMinPipe(Pipe pipe)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ p0 = line.GetEndPoint(0);
            XYZ p1 = line.GetEndPoint(1);
            return Math.Min(p0.Z, p1.Z);
        }
        private static double GetZMaxPipe(Pipe pipe)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ p0 = line.GetEndPoint(0);
            XYZ p1 = line.GetEndPoint(1);
            return Math.Max(p0.Z, p1.Z);
        }
        public static double GetSlopePipe(Pipe pipe)
        {
            return pipe.get_Parameter(BuiltInParameterID.SlopeID).AsDouble();
        }
        public static XYZ GetOriginConnectorPlumbingFixture(Element element, PipeSystemType pipeSystemType)
        {

            List<Connector> connectors = ConnectorProcess.GetConnectorsPlumbingFixture(element);

            return connectors.Where(x => x.PipeSystemType == pipeSystemType).FirstOrDefault().Origin;
        }
        #endregion
        #region General
        private static XYZ MinPointPipe(Pipe pipe, double Slope)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ p0 = line.GetEndPoint(0);
            XYZ p1 = line.GetEndPoint(1);
            if (PointModel.AreEqual(Slope, 0))
            {
                return p0;
            }
            else
            {

                if (p0.Z > p1.Z)
                {
                    return p1;
                }
                else
                {
                    return p0;
                }
            }
        }
        private static XYZ GetDirectionLineProject(Pipe pipe, Line line, XYZ Origin)
        {
            XYZ p0 = GetOriginProjectMinPlan(pipe, Origin);
            XYZ p1 = PointModel.ProjectToLine(p0, line);
            XYZ vector = p1 - line.GetEndPoint(0);
            if (PointModel.AreEqual((vector.AngleTo(line.Direction)), 0))
            {
                return line.Direction;
            }
            else
            {
                return -line.Direction;
            }
        }

        private static double GetDistance(Pipe pipe, XYZ Origin)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ p1 = GetOriginProjectMinPlan(pipe, Origin);
            XYZ p0 = PointModel.ProjectToLine(p1, line);
            return p0.DistanceTo(p1);
        }
        private static XYZ GetOriginProjectMinPlan(Pipe pipe, XYZ Origin)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ p0 = line.GetEndPoint(0);
            XYZ p1 = line.GetEndPoint(1);
            double Zmin = Math.Min(p0.Z, p1.Z);
            return new XYZ(Origin.X, Origin.Y, Zmin);
        }
        private static Line GetLineProjectOfPipe(Pipe pipe, double Slope)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ p0 = line.GetEndPoint(0);
            XYZ p1 = line.GetEndPoint(1);
            double Zmin = Math.Min(p0.Z, p1.Z);

            XYZ p0A, p1A;
            if (PointModel.AreEqual(Slope, 0))
            {
                p0A = new XYZ(p0.X, p0.Y, Zmin);
                p1A = new XYZ(p1.X, p1.Y, Zmin);
            }
            else
            {
                XYZ p0B, p1B;
                if (p0.Z > p1.Z)
                {
                    p0B = p1; p1B = p0;
                }
                else
                {
                    p0B = p0; p1B = p1;
                }
                p0A = new XYZ(p0B.X, p0B.Y, Zmin);
                p1A = new XYZ(p1B.X, p1B.Y, Zmin);
            }
            return Line.CreateBound(p0A, p1A);
        }

        public static XYZ GetVectorNormal(Pipe pipe, XYZ Origin)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ p1 = GetOriginProjectMinPlan(pipe, Origin);
            XYZ p0 = PointModel.ProjectToLine(p1, line);
            Line line1 = Line.CreateBound(p0, p1);
            return line1.Direction;
        }


        #endregion
        #region WaterCloset_Slanted
        private static XYZ GetXYZSlanted0(Pipe pipe, XYZ Origin, double Slope, double Start, double Diameter)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ direction = GetDirectionLineProject(pipe, line, Origin);
            
            XYZ p1 = GetOriginProjectMinPlan(pipe, Origin);
            XYZ p0 = PointModel.ProjectToLine(p1, line);
            XYZ min = MinPointPipe(pipe, Slope);
            return p0 + XYZ.BasisZ * Slope * (min.DistanceTo(p0)) - direction * Start * Diameter;
        }

        private static XYZ GetXYZSlanted1(Pipe pipe, XYZ Origin, double Slope, double Start, double Diameter, XYZ p0)
        {
            XYZ VectorNormal = GetVectorNormal(pipe, Origin);
            Line line = GetLineProjectOfPipe(pipe);
            XYZ direction = GetDirectionLineProject(pipe, line, Origin);
            return p0 + VectorNormal * Start * Diameter + direction * Start * Diameter + XYZ.BasisZ * Slope * Start * Diameter * Math.Sqrt(2);
        }
        private static XYZ GetXYZSlanted2(Pipe pipe, XYZ Origin, double Slope, double Start, double End, double Diameter, XYZ p1)
        {
            double distance = GetDistance(pipe, Origin);
            XYZ VectorNormal = GetVectorNormal(pipe, Origin);
            return p1 + VectorNormal * (distance - (Start + End) * Diameter) + XYZ.BasisZ * Slope * (distance - (Start + End) * Diameter);
        }
        private static XYZ GetXYZSlanted3(Pipe pipe, XYZ Origin, double End, double Diameter, XYZ p2)
        {
            XYZ VectorNormal = GetVectorNormal(pipe, Origin);
            return p2 + VectorNormal * End * Diameter + XYZ.BasisZ * End * Diameter;
        }
        public static List<XYZ> GetAllPointOptionSlanted(Pipe pipe,XYZ Origin, double Slope, double Start, double Mid, double End, double Top, double Diameter)
        {
            List<XYZ> AllPoints = new List<XYZ>();
            var p0 = GetXYZSlanted0(pipe,Origin,Slope,Start,Diameter);
            var p1 = GetXYZSlanted1(pipe, Origin, Slope, Start, Diameter, p0);
            var p2 = GetXYZSlanted2(pipe, Origin, Slope, Start,End, Diameter, p1);
            var p3 = GetXYZSlanted3(pipe,Origin,End,Diameter, p2);
            AllPoints.Add(p0);
            AllPoints.Add(p1);
            AllPoints.Add(p2);
            AllPoints.Add(p3);
            return AllPoints;
        }
        #region
        public static bool ConditionDistanceSlanted(Pipe pipe, XYZ Origin, double Slope, double Start, double Mid, double End, double Diameter)
        {
            double distance = GetDistance(pipe, Origin);

            return distance >= (Start + Mid + End) * Diameter;
        }
        public static bool ConditionHeightSlanted(Pipe pipe, XYZ Origin, double Slope, double Start, double Mid, double End, double Top, double Diameter, XYZ p0)
        {
            double distance = GetDistance(pipe, Origin);
            return ((Math.Abs(Origin.Z - p0.Z)) > (Slope * (Start * Math.Sqrt(2) + Mid) * Diameter + (Top + End) * Diameter));
        }
        public static bool ConditionXYZSlanted(Pipe pipe, XYZ p0)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ vec = p0 - line.GetEndPoint(0);
            return (vec.AngleTo(line.Direction) < Math.PI * 0.5);
        }
        #endregion
        #endregion
        #region WaterCloset_Strength
        private static XYZ GetXYZStrength0(Pipe pipe, XYZ Origin, double Slope,  double Diameter)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ direction = GetDirectionLineProject(pipe, line, Origin);
            double distance = GetDistance(pipe, Origin);
            XYZ p1 = GetOriginProjectMinPlan(pipe, Origin);
            XYZ p0 = PointModel.ProjectToLine(p1, line);
            
            XYZ min = MinPointPipe(pipe, Slope);
            return p0 + XYZ.BasisZ * Slope * (min.DistanceTo(p0)) - direction * (distance);
        }

        private static XYZ GetXYZStrength1(Pipe pipe, XYZ Origin, double Slope, double End, double Diameter, XYZ p0)
        {
            XYZ VectorNormal = GetVectorNormal(pipe, Origin);
            Line line = GetLineProjectOfPipe(pipe);
            XYZ direction = GetDirectionLineProject(pipe, line, Origin);
            double distance = GetDistance(pipe, Origin);
            double distance1 = distance - (End * Diameter / Math.Sqrt(2));
            return p0 + VectorNormal * distance1 + direction * distance1;
        }
        private static XYZ GetXYZStrength2(Pipe pipe, XYZ Origin, double End, double Diameter, XYZ p1)
        {
            double distance = GetDistance(pipe, Origin);
            double distance1 = distance - End * Diameter / Math.Sqrt(2);
            XYZ VectorNormal = GetVectorNormal(pipe, Origin);
            Line line = GetLineProjectOfPipe(pipe);
            XYZ direction = GetDirectionLineProject(pipe, line, Origin);
            return p1 + VectorNormal * (End * Diameter / Math.Sqrt(2)) + direction * (End * Diameter / Math.Sqrt(2)) + XYZ.BasisZ * ((End * Diameter ));
        }
       
        public static List<XYZ> GetAllPointOptionStrength(Pipe pipe, XYZ Origin, double Slope, double End,  double Diameter)
        {
            List<XYZ> AllPoints = new List<XYZ>();
            var p0 = GetXYZStrength0(pipe, Origin, Slope,  Diameter);
            var p1 = GetXYZStrength1(pipe, Origin, Slope, End, Diameter, p0);
            var p2 = GetXYZStrength2(pipe, Origin, End, Diameter, p1);
            AllPoints.Add(p0);
            AllPoints.Add(p1);
            AllPoints.Add(p2);
            return AllPoints;
        }
        #region
        public static bool ConditionDistanceStrength(Pipe pipe, XYZ Origin,  double Start,  double End, double Diameter)
        {
            double distance = GetDistance(pipe, Origin);

            return distance >= (Start ) * Diameter+ End * Diameter / Math.Sqrt(2);
        }
        public static bool ConditionHeightStrength(Pipe pipe, XYZ Origin, double Slope, double Start, double Mid, double End, double Top, double Diameter, XYZ p0)
        {
            double distance = GetDistance(pipe, Origin);
            return ((Math.Abs(Origin.Z - p0.Z)) > (Slope * Start * Diameter / Math.Sqrt(2) + (Top) * Diameter+ End * Diameter / Math.Sqrt(2)));
        }
        public static bool ConditionXYZStrength(Pipe pipe, XYZ p0)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ vec = p0 - line.GetEndPoint(0);
            return (vec.AngleTo(line.Direction) < Math.PI * 0.5);
        }
        #endregion
        #endregion

    }



}

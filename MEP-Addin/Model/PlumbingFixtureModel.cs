using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using MEP_Addin.Library;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MEP_Addin
{
    public class PlumbingFixtureModel
    {
        
        public int Start { get; set; }
        public int Mid { get; set; }
        public int End { get; set; }
        public int Top { get; set; }
        public Element PlumbingFixture { get; set; }
        public Pipe Down { get; set; }
        public Pipe DownElbow { get; set; }
        public Pipe Branch { get; set; }
        public Pipe BranchElbow { get; set; }
        
        public double Diameter { get; set; }
        public double DiameterMain { get; set; }
        public double Slope { get; set; }
       
        public XYZ XYZDown { get; set; }
        public XYZ XYZDownElbow { get; set; }
        public XYZ XYZBranch { get; set; }
        public XYZ XYZBranchElbow { get; set; }
        public XYZ VectorNormal { get; set; }
        public bool CanCreate { get; set; }
        public XYZ Origin { get; set; }
      
        public List<Pipe> Pipes { get; set; } = new List<Pipe>();
        public List<XYZ> AllPoints { get; set; } = new List<XYZ>();


        public PlumbingFixtureModel(Element plumbingFixture, Pipe pipe)
        {
            PlumbingFixture = plumbingFixture;
            Start = 3; Mid = 2; End = 2; Top = 2;
            GetConnectorPipe(pipe);
            VectorNormal = GetVectorNormal(pipe);
            GetAllPoint(pipe);
           
        }

        #region GetProperty
       
        public void GetConnectorPipe(Pipe pipe)
        {
            DiameterMain = pipe.get_Parameter(BuiltInParameterID.DiameterPipeID).AsDouble();
            Slope = pipe.get_Parameter(BuiltInParameterID.SlopeID).AsDouble();
            PipeSystemType pipeSystem = ConnectorProcess.GetPipeSystemType(pipe);
            List<Connector> connectors = ConnectorProcess.GetConnectorsPlumbingFixture(PlumbingFixture);
            Connector connector = connectors.Where(x => x.PipeSystemType == pipeSystem).FirstOrDefault();
            if (connector != null)
            {
                Origin = connector.Origin;
                Diameter = connector.Radius * 2;
            }
            else
            {
                Diameter = 0;
                Origin = null;
            }
        }
        private void GetAllPoint(Pipe pipe)
        {
            XYZBranchElbow = GetXYZBranchElbow(pipe);
           
            XYZBranch = GetXYZBranch(pipe);
            XYZDownElbow = GetXYZDownElbow(pipe);
            XYZDown = GetXYZDown(pipe);
            CanCreate = (Origin != null) && (ConditionDistance(pipe)) && (ConditionHeight(pipe)&&(ConditionXYZBranchElbow(pipe)));
        }
      
        #endregion

        #region ConditionCreate
        private bool ConditionDistance(Pipe pipe)
        {
            double distance = GetDistance(pipe);
            return distance >= (Start + Mid + End) * Diameter;
        }
        private bool ConditionHeight(Pipe pipe)
        {
            double distance = GetDistance(pipe);
            return ((Math.Abs(Origin.Z-XYZBranchElbow.Z)) > (Slope*(Start*Math.Sqrt(2)+Mid)*Diameter+(Top+End)*Diameter))&&(Origin.Z>XYZBranch.Z);
        }
        private bool ConditionXYZBranchElbow(Pipe pipe)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ vec = XYZBranchElbow - line.GetEndPoint(0);
            return (vec.AngleTo(line.Direction)<Math.PI*0.5);
        }
        #endregion
        #region AllPoint Option1     

        private XYZ GetXYZBranchElbow(Pipe pipe)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ direction = GetDirectionLineProject(pipe, line);
            XYZ p1 = GetOriginProjectMinPlan(pipe);
            XYZ p0 = PointModel.ProjectToLine(p1, line);
            XYZ min = MInPointPipe(pipe);
            return p0 + XYZ.BasisZ * Slope * (min.DistanceTo(p0))- direction * Start*Diameter;
        }
        private XYZ GetXYZBranchElbowReplace(Pipe pipe)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ direction = GetDirectionLineProject(pipe, line);
            return XYZBranchElbow + VectorNormal *  Diameter + direction * Diameter + XYZ.BasisZ * Slope *  Diameter * Math.Sqrt(2);
        }
        private XYZ GetXYZBranch(Pipe pipe)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ direction = GetDirectionLineProject(pipe, line);
            return XYZBranchElbow + VectorNormal * Start * Diameter + direction * Start * Diameter + XYZ.BasisZ * Slope * Start * Diameter*Math.Sqrt(2);
        }
        private XYZ GetXYZDownElbow(Pipe pipe)
        {
            double distance = GetDistance(pipe);
            return XYZBranch + VectorNormal * (distance - (Start + Mid) * Diameter) + XYZ.BasisZ * Slope * (distance - (Start + Mid) * Diameter);
        }
        private XYZ GetXYZDown(Pipe pipe)
        {

            return XYZDownElbow + VectorNormal * End * Diameter + XYZ.BasisZ * End * Diameter;
        }
        private XYZ MInPointPipe(Pipe pipe)
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
        #endregion
        #region General
         private XYZ GetDirectionLineProject(Pipe pipe,Line line)
        {
            XYZ p0 = GetOriginProjectMinPlan(pipe);
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

        private double GetDistance(Pipe pipe)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ p1 = GetOriginProjectMinPlan(pipe);
            XYZ p0 = PointModel.ProjectToLine(p1, line);
            return p0.DistanceTo(p1);
        }
        private XYZ GetOriginProjectMinPlan(Pipe pipe)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            XYZ p0 = line.GetEndPoint(0);
            XYZ p1 = line.GetEndPoint(1);
            double Zmin = Math.Min(p0.Z, p1.Z);
            return new XYZ(Origin.X, Origin.Y, Zmin);
        }
        private Line GetLineProjectOfPipe(Pipe pipe)
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
      
        public XYZ GetVectorNormal(Pipe pipe)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ p1 = GetOriginProjectMinPlan(pipe);
            XYZ p0 = PointModel.ProjectToLine(p1, line);
            Line line1 = Line.CreateBound(p0, p1);
            return line1.Direction;
        }
        private XYZ GetVectorSlantedNormal(Pipe pipe)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ p1a = GetOriginProjectMinPlan(pipe);
            XYZ p0a = PointModel.ProjectToLine(p1a, line);
            XYZ normal = GetVectorNormal(pipe);
            XYZ p0 = p0a - line.Direction * 4 * Diameter;
            XYZ p1 = p0a + normal * 4 * Diameter;
            Line line1 = Line.CreateBound(p0, p1);
            return line1.Direction;
        }
        private XYZ StartXYZPipe(Pipe pipe)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            double d0 = (line.Length);
            double d1 = XYZBranchElbow.DistanceTo(line.GetEndPoint(0));
            double d2 = XYZBranchElbow.DistanceTo(line.GetEndPoint(1));
            if (PointModel.AreEqual(d1+d2,d0))
            {
                return (PointModel.AreEqualXYZ(XYZBranchElbow, line.GetEndPoint(0))) ? line.GetEndPoint(1) : line.GetEndPoint(0);
            }
            else
            {
                return ((PointModel.AreEqual(Slope, 0)) ? (line.GetEndPoint(0)) : ((line.GetEndPoint(0).Z > line.GetEndPoint(1).Z) ? (line.GetEndPoint(1)) : (line.GetEndPoint(0))));
            }

        }
        #endregion
        #region Create
        
        private void SetDiameter(Pipe pipe)
        {
            if (pipe != null)
            {
                pipe.get_Parameter(BuiltInParameterID.DiameterPipeID).Set(Diameter);
            }
        }
        public void CreatePipe(Document document, PipeType pipeType, Level level, ElementId elementId)
        {
            try
            {
                if (CanCreate)
                {
                    BranchElbow = Pipe.CreatePlaceholder(document, elementId, pipeType.Id, level.Id, XYZBranchElbow, XYZBranch); SetDiameter(BranchElbow);
                    Branch = Pipe.CreatePlaceholder(document, elementId, pipeType.Id, level.Id, XYZBranch, XYZDownElbow); SetDiameter(Branch);
                    DownElbow = Pipe.CreatePlaceholder(document, elementId, pipeType.Id, level.Id, XYZDownElbow, XYZDown); SetDiameter(DownElbow);
                    Down = Pipe.CreatePlaceholder(document, elementId, pipeType.Id, level.Id, XYZDown, Origin); SetDiameter(Down);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            
          
        }
       
        public void SplitPipe(Document document, Pipe pipe, out Pipe NewPipe,out ElementId elementId)
        {
             elementId = PlumbingUtils.BreakCurve(document, pipe.Id, XYZBranchElbow);
            Pipe p1 = document.GetElement(elementId) as Pipe;
            NewPipe = (Math.Abs(p1.Id.IntegerValue) > Math.Abs(pipe.Id.IntegerValue)) ? pipe : p1;
          
        }
      
        #endregion



    }
}

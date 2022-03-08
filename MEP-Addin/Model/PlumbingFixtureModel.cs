using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using MEP_Addin.Library;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MEP_Addin
{
    public class PlumbingFixtureModel
    {
        private static readonly int Start = 4, Mid = 3, End = 3,Top=2;
        public Element PlumbingFixture { get; set; }
        public Pipe Down { get; set; }
        public Pipe DownElbow { get; set; }
        public Pipe Branch { get; set; }
        public Pipe BranchElbow { get; set; }
        public FamilyInstance ElbowUPDown { get; set; }
        public FamilyInstance ElbowDownBranch { get; set; }
        public FamilyInstance ElbowBranchElbow { get; set; }
        public FamilyInstance TeeElbowMain { get; set; }
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
        public Connector ConnectorOrigin { get; set; }
        public List<Connector> ConnectorDown { get; set; } = new List<Connector>();
        public List<Connector> ConnectorDownElbow { get; set; } = new List<Connector>();
        public List<Connector> ConnectorBranch { get; set; } = new List<Connector>();
        public List<Connector> ConnectorBranchElbow { get; set; } = new List<Connector>();

        public PlumbingFixtureModel(Element plumbingFixture, Pipe pipe)
        {
            PlumbingFixture = plumbingFixture;
            GetConnectorPipe(pipe);
            GetConnectorOrigin(pipe);
            VectorNormal = GetVectorNormal(pipe);
            GetAllPoint(pipe);
        }

        

        #region GetProperty
        private void GetConnectorOrigin(Pipe pipe)
        {
            PipeSystemType pipeSystemPipe = ConnectorProcess.GetPipeSystemType(pipe);
            PipeSystemType pipeSystemPlumbingFixture = ConnectorProcess.GetPipeSystemTypePlumbingFixture(PlumbingFixture);
            ConnectorOrigin = ConnectorProcess.GetConnectorsPlumbingFixture(PlumbingFixture).Where(x => x.PipeSystemType == pipeSystemPlumbingFixture).FirstOrDefault();
        }

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
            CanCreate = (Origin != null) && (ConditionDistance(pipe)) && (ConditionHeight(pipe));
        }
        private void GetAllConnector()
        {
            if (BranchElbow != null)
            {
                ConnectorBranchElbow.Add(ConnectorProcess.GetConnectorPipe(BranchElbow, XYZBranchElbow));
                ConnectorBranchElbow.Add(ConnectorProcess.GetConnectorPipe(BranchElbow, XYZBranch));
            }
            if (Branch != null)
            {
                ConnectorBranch.Add(ConnectorProcess.GetConnectorPipe(Branch, XYZBranch));
                ConnectorBranch.Add(ConnectorProcess.GetConnectorPipe(Branch, XYZDownElbow));
            }
            if (DownElbow != null)
            {
                ConnectorDownElbow.Add(ConnectorProcess.GetConnectorPipe(DownElbow, XYZDownElbow));
                ConnectorDownElbow.Add(ConnectorProcess.GetConnectorPipe(DownElbow, XYZDown));
            }
            if (Down != null)
            {
                ConnectorDown.Add(ConnectorProcess.GetConnectorPipe(Down, XYZDown));
                ConnectorDown.Add(ConnectorProcess.GetConnectorPipe(Down, Origin));
            }
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
        
        #endregion
        #region AllPoint

        private XYZ GetXYZBranchElbow(Pipe pipe)
        {
            Line line = GetLineProjectOfPipe(pipe);
            XYZ direction = GetDirectionLineProject(pipe, line);
            XYZ p1 = GetOriginProjectMinPlan(pipe);
            XYZ p0 = PointModel.ProjectToLine(p1, line);
            XYZ min = MInPointPipe(pipe);
            return p0 + XYZ.BasisZ * Slope * (min.DistanceTo(p0))- direction * Start*Diameter;
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
      
        private XYZ GetVectorNormal(Pipe pipe)
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
                    BranchElbow = Pipe.Create(document, elementId, pipeType.Id, level.Id, XYZBranchElbow, XYZBranch); SetDiameter(BranchElbow);
                    Branch = Pipe.Create(document, elementId, pipeType.Id, level.Id, XYZBranch, XYZDownElbow); SetDiameter(Branch);
                    DownElbow = Pipe.Create(document, elementId, pipeType.Id, level.Id, XYZDownElbow, XYZDown); SetDiameter(DownElbow);
                    Down = Pipe.Create(document, elementId, pipeType.Id, level.Id, XYZDown, Origin); SetDiameter(Down);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            
          
        }
        public void CreatePipeFitting(Document document, Level level)
        {
            GetAllConnector();
            if (ConnectorBranchElbow.Count != 0 && ConnectorBranch.Count != 0&& ConnectorDownElbow.Count!=0&& ConnectorDown.Count!=0)
            {
                try
                {
                    ElbowBranchElbow = document.Create.NewElbowFitting(ConnectorBranchElbow[ConnectorBranchElbow.Count - 1], ConnectorBranch[0]);
                    ElbowDownBranch = document.Create.NewElbowFitting(ConnectorBranch[ConnectorBranch.Count - 1], ConnectorDownElbow[0]);
                    ElbowUPDown = document.Create.NewElbowFitting(ConnectorDownElbow[ConnectorDownElbow.Count - 1], ConnectorDown[0]);
                    ConnectorDown[ConnectorDown.Count - 1].ConnectTo(ConnectorOrigin);
                    CreateBranchElbowTee(document, level);
                }
                catch (Exception e)
                {

                    System.Windows.Forms.MessageBox.Show(e.Message);
                }
                
            }

        }
        private void CreateBranchElbowTee(Document document,Level level)
        {
            List<FamilySymbol> familySymbols = PipeFittingType.GetPipeFittingType(document, PipeFittingType.Tee);
            familySymbols = familySymbols.Where(x => x.FamilyName.Equals("DSC_uPVC_Tee+Y_TienPhong")).ToList();
            if (familySymbols.Count != 0)
            {
                FamilySymbol familySymbol = familySymbols.Where(x => x.Name.Equals("Standard")).FirstOrDefault();
                if(familySymbol!=null)
                {
                    TeeElbowMain= document.Create.NewFamilyInstance(XYZBranchElbow, familySymbol, level,Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                    TeeElbowMain.LookupParameter("DSC_NHT_Angle").Set(Math.PI*0.25);
                    TeeElbowMain.LookupParameter("DSC_NHT_Nominal Diameter 2").Set(Diameter);
                    TeeElbowMain.LookupParameter("DSC_NHT_Nominal Diameter 1").Set(DiameterMain);
                }
              
            }
        }

        #endregion



    }
}

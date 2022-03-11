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
        
        public double Start { get; set; }
        public double Mid { get; set; }
        public double End { get; set; }
        public double Top { get; set; }
       
        public Element PlumbingFixture { get; set; }
        
        
        public double Diameter { get; set; }
        public double DiameterMain { get; set; }
        public double Slope { get; set; }
        public Connector Connector { get; set; }

        public bool CanCreate { get; set; }
        public XYZ Origin { get; set; }
      
        public List<Pipe> AllPipes { get; set; } = new List<Pipe>();
        public List<XYZ> AllPoints { get; set; } = new List<XYZ>();

        public PlumbingFixtureModel(Element plumbingFixture, Pipe pipe,double start, double mid,double end,double top)
        {
            PlumbingFixture = plumbingFixture;
            Start = start; Mid = mid; End = end; Top = top;
            GetConnectorPipe(pipe);
        }

        #region GetProperty
       
        public void GetConnectorPipe(Pipe pipe)
        {
            DiameterMain = pipe.get_Parameter(BuiltInParameterID.DiameterPipeID).AsDouble();
            Slope = pipe.get_Parameter(BuiltInParameterID.SlopeID).AsDouble();
            PipeSystemType pipeSystem = ConnectorProcess.GetPipeSystemType(pipe);
            List<Connector> connectors = ConnectorProcess.GetConnectorsPlumbingFixture(PlumbingFixture);
            Connector = connectors.Where(x => x.PipeSystemType == pipeSystem).FirstOrDefault();
            if (Connector != null)
            {
                Origin = Connector.Origin;
                Diameter = Connector.Radius * 2;
            }
            else
            {
                Diameter = 0;
                Origin = null;
            }
        }
        public void GetAllPointOptionSlanted(Pipe pipe)
        {
            AllPoints = CalculatePipeLocation.GetAllPointOptionSlanted(pipe, Origin, Slope, Start, Mid, End, Top, Diameter);
            CanCreate = (Origin != null) && (CalculatePipeLocation.ConditionDistanceSlanted(pipe, Origin, Slope, Start, Mid, End,  Diameter)) && (CalculatePipeLocation.ConditionHeightSlanted(pipe, Origin, Slope, Start, Mid, End, Top, Diameter,AllPoints[0]) &&(CalculatePipeLocation.ConditionXYZSlanted(pipe, AllPoints[0])));
           
        }
        public void GetAllPointOptionStrength(Pipe pipe)
        {
            AllPoints = CalculatePipeLocation.GetAllPointOptionStrength(pipe, Origin, Slope, End, Diameter);
            CanCreate = (Origin != null) && (CalculatePipeLocation.ConditionDistanceStrength(pipe, Origin,  Start,  End, Diameter)) && (CalculatePipeLocation.ConditionHeightStrength(pipe, Origin, Slope, Start, Mid, End, Top, Diameter, AllPoints[0]) && (CalculatePipeLocation.ConditionXYZStrength(pipe, AllPoints[0])));

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
                    for (int i = 0; i < AllPoints.Count; i++)
                    {
                        XYZ end = (i == AllPoints.Count - 1) ? (Origin) : (AllPoints[i + 1]);
                        var p0 = Pipe.CreatePlaceholder(document, elementId, pipeType.Id, level.Id, AllPoints[i], end);
                        SetDiameter(p0);
                        AllPipes.Add(p0);
                    }
                    SetConnectorPipeToPlumbingFixture();

                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            
          
        }

        private void SetConnectorPipeToPlumbingFixture()
        {
            Connector connectorPipe = ConnectorProcess.GetConnectorPipe(AllPipes[AllPipes.Count-1], Origin);
            if(connectorPipe!=null&& Connector != null)
            {
                if (Connector != null )
                {

                    Connector.ConnectTo(connectorPipe);
                }
            }
            
        }

        #endregion



    }
}

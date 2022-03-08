using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEP_Addin.Library
{
    public class CalculatePipeLocation
    {
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
        private static XYZ TranformPointPlumbingFixture(Connector connector,Pipe pipe)
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
    }
    

    public enum SLOPE
    {
        NONE, UP, DOWN
    }
}

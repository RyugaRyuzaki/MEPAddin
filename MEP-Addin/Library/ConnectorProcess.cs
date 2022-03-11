using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
namespace MEP_Addin.Library
{
    public class ConnectorProcess
    {
        public static PipeSystemType Sanity = PipeSystemType.Sanitary;
        private static MEPModel GetMEPModel( Element element)
        {
            if(element is FamilyInstance)
            {
                return ((FamilyInstance)element).MEPModel;
            }
            else
            {
                return null;
            }
        }
        public static List<Connector> GetConnectorsPlumbingFixture(Element element)
        {
            MEPModel mEP= ((FamilyInstance)element).MEPModel; 
            List<Connector> connectors = new List<Connector>();
            ConnectorManager connectorManager = mEP.ConnectorManager;
            ConnectorSet connectorSet = connectorManager.Connectors;
            foreach (var item in connectorSet)
            {
                Connector connector = item as Connector;
                if (connector != null) connectors.Add(connector);
            }
            return connectors;
        }
        public static Connector GetConnectorPipeDefault(Pipe pipe)
        {
            ConnectorManager connectorManager = pipe.ConnectorManager;
            ConnectorSet connectorSet = connectorManager.Connectors;
            List<Connector> connectors = new List<Connector>();
            foreach (var item in connectorSet)
            {
                Connector connector1 = item as Connector;
                if (connector1 != null ) connectors .Add(connector1);
            }
            return connectors[0];
        }
        public static Connector GetConnectorPipe(Pipe pipe,XYZ xYZ)
        {
            ConnectorManager connectorManager = pipe.ConnectorManager;
            ConnectorSet connectorSet = connectorManager.Connectors;
            Connector connector = null ;
            foreach (var item in connectorSet)
            {
                Connector connector1 = item as Connector;
                if (connector1 != null && PointModel.AreEqual(connector1.Origin.DistanceTo(xYZ),0)) connector = connector1;
            }
            return connector;
        }
        
        public static PipeSystemType GetPipeSystemType(Pipe pipe)
        {
            List<Connector> connectors = new List<Connector>();
            ConnectorManager connectorManager = pipe.ConnectorManager;
            ConnectorSet connectorSet = connectorManager.Connectors;
            foreach (var item in connectorSet)
            {
                Connector connector = item as Connector;
                if (connector != null) connectors.Add(connector);
            }
            return connectors[0].PipeSystemType;
        }
        public static PipeSystemType GetPipeSystemTypePlumbingFixture(Element element)
        {
            List<Connector> connectors = GetConnectorsPlumbingFixture(element);
           
            return connectors.Where(x => x.PipeSystemType == Sanity).FirstOrDefault().PipeSystemType;
        }
        public static bool CanConnectPlumbingFixture(Pipe pipe,Element element)
        {

            List<Connector> Connectors = GetConnectorsPlumbingFixture(element);
            foreach (var item in Connectors)
            {
                if (item.PipeSystemType == GetPipeSystemType(pipe)) return true;
            }
            return false;
        }
      
    }
}

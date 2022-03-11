using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;
    
namespace MEP_Addin.Library
{
   public class FilterPlumbingFixtures : ISelectionFilter
    {
        public Document Doc { get; set; }
        public Pipe Pipe { get; set; }
        public Connector Connector { get; set; }
        public FilterPlumbingFixtures(Document document,Pipe pipe)
        {
            Doc = document;
            Pipe = pipe;
            Connector = ConnectorProcess.GetConnectorPipeDefault(Pipe);
        }
        public bool AllowElement(Element elem)
        {
            return( elem.Category.Name.Equals(Category.GetCategory(Doc, BuiltInCategoryID.PlumbingFixtureID).Name)&&(!elem.get_Parameter(BuiltInParameterID.SystemClassification).AsString().Equals(""))&&SameConnector(elem));
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
        public bool SameConnector(Element elem)
        {
            List<Connector> connectors = ConnectorProcess.GetConnectorsPlumbingFixture(elem);
            connectors = connectors.Where(x => x.PipeSystemType == Connector.PipeSystemType).ToList();
            return connectors.Count>0;
        }
    }
}

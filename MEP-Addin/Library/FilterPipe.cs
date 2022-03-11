using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEP_Addin.Library
{
    public class FilterPipe : ISelectionFilter
    {
        public Document Doc { get; set; }
        public FilterPipe(Document document)
        {
            Doc = document;
        }
        public bool AllowElement(Element elem)
        {
            if( (elem.Category.Name.Equals(Category.GetCategory(Doc, BuiltInCategoryID.PileID).Name)) )
            {
                try
                {
                    return HasConnector(elem as Pipe);
                }
                catch (Exception)
                {

                    return false;
                } 
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
        public bool HasConnector(Pipe elem)
        {
            ConnectorManager connectorManager = elem.ConnectorManager;
            ConnectorSet connectorSet = connectorManager.Connectors;
            List<Connector> connectors = new List<Connector>();
            foreach (var item in connectorSet)
            {
                Connector connector1 = item as Connector;
                if (connector1 != null&&!connector1.IsConnected) connectors.Add(connector1);
            }
            return connectors.Count > 0;
        }
    }
}

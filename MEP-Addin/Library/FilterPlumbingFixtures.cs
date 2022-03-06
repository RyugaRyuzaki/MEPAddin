using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
    
namespace MEP_Addin.Library
{
   public class FilterPlumbingFixtures : ISelectionFilter
    {
        public Document Doc { get; set; }
        public FilterPlumbingFixtures(Document document)
        {
            Doc = document;
        }
        public bool AllowElement(Element elem)
        {
            return elem.Category.Name.Equals(Category.GetCategory(Doc, BuiltInCategoryID.PlumbingFixtureID).Name);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}

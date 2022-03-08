using Autodesk.Revit.DB;
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
            return (elem.Category.Name.Equals(Category.GetCategory(Doc, BuiltInCategoryID.PileID).Name) );
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}

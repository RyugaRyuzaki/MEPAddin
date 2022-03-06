﻿#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
#endregion

namespace MEP_Addin.Library.Filter
{
    public class BeamSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            string name = elem.Category.Name;
            return name.Equals("Structural Framing");

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}

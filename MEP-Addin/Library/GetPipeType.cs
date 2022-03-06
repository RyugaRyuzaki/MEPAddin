using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace MEP_Addin.Library
{
    public class GetPipeType
    {
        public static List<PipeType> GetAllPipeTypes(Document document)
        {
            return new FilteredElementCollector(document) .OfClass(typeof(PipeType)).Cast<PipeType>().ToList();
        }
    }
}

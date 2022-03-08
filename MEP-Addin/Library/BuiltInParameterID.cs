using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
namespace MEP_Addin.Library
{
    public class BuiltInParameterID
    {
        public static BuiltInParameter Type = (BuiltInParameter)(-1002050);
        public static BuiltInParameter SystemClassification = (BuiltInParameter)(-1140325);
        public static BuiltInParameter SystemTypePipe = (BuiltInParameter)(-1140334);
        public static BuiltInParameter PartTypeID = (BuiltInParameter)(-1114206);
        public static BuiltInParameter SlopeID = (BuiltInParameter)(-1140256);
        public static BuiltInParameter DiameterPipeID = (BuiltInParameter)(-1140225);
        public static BuiltInParameter FamilyInstanceLevelID = (BuiltInParameter)(-1001352);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB;
namespace MEP_Addin.Library
{
    public class PipeFittingType
    {
        public static PartType Elbow = (PartType)(5);
        public static PartType Tee = (PartType)(6);
        public static PartType Union = (PartType)(13);
        public static PartType Transition = (PartType)(7);
        private static List<Family> GetAllFamilyPipeFitting(Document document)
        {
            return new FilteredElementCollector(document).OfClass(typeof(Family)).Cast<Family>().Where(x => x.FamilyCategory.Name.Equals(Category.GetCategory(document, BuiltInCategoryID.PileFittingID).Name)).ToList();
        }
        public static List<FamilySymbol> GetPipeFittingType(Document document, PartType partType)
        {
            List<Family> families = GetAllFamilyPipeFitting(document);
            List<Family> elbowFamilies = new List<Family>(families.Where(x => x.get_Parameter(BuiltInParameterID.PartTypeID).AsInteger() == (int)partType).ToList());
            List<FamilySymbol> familySymbols = new List<FamilySymbol>();
            foreach (var item in elbowFamilies)
            {
                foreach (var item1 in item.GetFamilySymbolIds())
                {
                    familySymbols.Add( item.Document.GetElement(item1) as FamilySymbol);
                    
                }
            }
            return familySymbols;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using MEP_Addin.Library;
namespace MEP_Addin.Library
{
    public class PlumbingFixturesType
    {
        public static List<Family> GetAllToiletFamily(Document document)
        {
            return new FilteredElementCollector(document).OfClass(typeof(Family)).Cast<Family>().Where(x => x.FamilyCategory.Name.Equals(Category.GetCategory(document, BuiltInCategoryID.PlumbingFixtureID).Name)).ToList();
        }
    }
}

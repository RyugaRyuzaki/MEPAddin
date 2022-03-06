
#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using MEP_Addin.Library;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Message = System.Windows.Forms.MessageBox;
using Autodesk.Revit.DB.Plumbing;
#endregion


namespace MEP_Addin.Command
{
    [Transaction(TransactionMode.Manual)]
    public class ConnectPileToPlumbingFixtures : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // code

            FilterPlumbingFixtures filterPlumbingFixtures = new FilterPlumbingFixtures(doc);
            try
            {
                Reference references = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, filterPlumbingFixtures);
                List<PipeType> pipeTypes = GetPipeType.GetAllPipeTypes(doc);
                foreach (var item in pipeTypes)
                {
                    Message.Show(item.Name);
                }
                return Result.Succeeded;
            }
            catch (System.Exception e)
            {

                Message.Show(e.Message);
                return Result.Cancelled;
            }
           

           
        }
    }
}

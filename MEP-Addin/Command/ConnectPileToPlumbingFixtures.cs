
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
using  Autodesk.Revit.UI.Selection;
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
            FilterPipe filterPipe = new FilterPipe(doc);
            try
            {
                Reference referencePipe = uidoc.Selection.PickObject(ObjectType.Element, filterPipe);
                Pipe pipe = doc.GetElement(referencePipe) as Pipe;
                Line line = (pipe.Location as LocationCurve).Curve as Line;
                try
                {
                    Reference referencePlumbingFixture = uidoc.Selection.PickObject(ObjectType.Element, filterPlumbingFixtures);
                   
                    Element plumblingFixture = doc.GetElement(referencePlumbingFixture);

                    if (!ConnectorProcess.CanConnectPlumbingFixture(pipe,plumblingFixture) )
                    {
                        Message.Show("Can not connect Pipe to Plumbing Fixture"+"\n"+"Please change SystemClassification of pipe");
                        return Result.Cancelled;
                    }
                    else
                    {
                      
                        PipeType pipeType = pipe.PipeType;
                        Level level = doc.GetElement(plumblingFixture.get_Parameter(BuiltInParameterID.FamilyInstanceLevelID).AsElementId()) as Level;
                        ElementId elementId = pipe.get_Parameter(BuiltInParameterID.SystemTypePipe).AsElementId();
                        PlumbingFixtureModel plumbingFixtureModel = new PlumbingFixtureModel(plumblingFixture, pipe);

                        using (Transaction tran = new Transaction(doc))
                        {

                            tran.Start("Create Pipe");
                            plumbingFixtureModel.CreatePipe(doc, pipeType, level, elementId);
                            tran.Commit();
                        }
                        using (Transaction tran = new Transaction(doc))
                        {

                            tran.Start("Create Pipe Fittings");
                            plumbingFixtureModel.CreatePipeFitting(doc,level);
                            tran.Commit();
                        }
                        return Result.Succeeded;
                    }
                    
                }
                catch (System.Exception e)
                {

                    Message.Show(e.Message);
                    return Result.Cancelled;
                }
               
            }
            catch (System.Exception e)
            {

                Message.Show(e.Message);
                return Result.Cancelled;
            }
           

           
        }
    }
}

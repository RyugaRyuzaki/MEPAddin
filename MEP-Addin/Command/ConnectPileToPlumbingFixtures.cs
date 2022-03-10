
#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MEP_Addin.Library;
using System.Collections.Generic;
using System.Linq;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Message = System.Windows.Forms.MessageBox;
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
                double Slope = pipe.get_Parameter(BuiltInParameterID.SlopeID).AsDouble();
                bool a = (PointModel.AreEqual(Slope, 0)) ? true : (line.GetEndPoint(0).Z < line.GetEndPoint(1).Z);
                if (!a)
                {
                    Message.Show("Please Model Pipe as Slop To Up");
                    return Result.Cancelled;
                }
                try
                {
                    List<Reference> referencePlumbingFixtures = uidoc.Selection.PickObjects(ObjectType.Element, filterPlumbingFixtures).ToList();

                    List<Element> plumblingFixtures = GetElements(doc, referencePlumbingFixtures);

                    PipeType pipeType = pipe.PipeType;
                    Level level = doc.GetElement(pipe.get_Parameter(BuiltInParameterID.PipeLevelID).AsElementId()) as Level;
                    ElementId elementId = pipe.get_Parameter(BuiltInParameterID.SystemTypePipe).AsElementId();
                    List<PlumbingFixtureModel> plumbingFixtureModelAlls = new List<PlumbingFixtureModel>();


                    for (int i = 0; i < plumblingFixtures.Count; i++)
                    {
                        plumbingFixtureModelAlls.Add(new PlumbingFixtureModel(plumblingFixtures[i], pipe));
                    }
                    List<Pipe> AllPipe = new List<Pipe>();
                    List<Pipe> AllPipePlaceHolder = new List<Pipe>();
                    List<PlumbingFixtureModel> plumbingFixtureModels = new List<PlumbingFixtureModel>();
                    List<PlumbingFixtureModel> plumbingFixtureModelErrors = new List<PlumbingFixtureModel>();
                    plumbingFixtureModels = plumbingFixtureModelAlls.Where(x => x.CanCreate).OrderBy(y => y.XYZBranchElbow.DistanceTo(line.GetEndPoint(0))).ToList();
                    plumbingFixtureModelErrors = plumbingFixtureModelAlls.Where(x => !x.CanCreate).ToList();
                    using (Transaction tran = new Transaction(doc))
                    {
                        tran.Start("Create Pipe");
                        Pipe pipe4=ExtendPipe(doc, elementId, pipeType, level, pipe, plumbingFixtureModels[plumbingFixtureModels.Count - 1]);
                        Line line3 = (pipe.Location as LocationCurve).Curve as Line;
                        var pipe3 = Pipe.CreatePlaceholder(doc, elementId, pipeType.Id, level.Id, line3.GetEndPoint(0), line3.GetEndPoint(1));
                        pipe3.get_Parameter(BuiltInParameterID.DiameterPipeID).Set(plumbingFixtureModels[0].DiameterMain);
                       
                        ICollection<ElementId> t1 = doc.Delete(pipe.Id);
                     
                        for (int i = 0; i < plumbingFixtureModels.Count; i++)
                        {
                            plumbingFixtureModels[i].CreatePipe(doc, pipeType, level, elementId);
                        }

                        List<ElementId> placeholders = new List<ElementId>();
                       
                        for (int i = 0; i < plumbingFixtureModels.Count; i++)
                        {
                           
                            placeholders.Add(plumbingFixtureModels[i].BranchElbow.Id);
                            placeholders.Add(plumbingFixtureModels[i].Branch.Id);
                            placeholders.Add(plumbingFixtureModels[i].DownElbow.Id);
                            placeholders.Add(plumbingFixtureModels[i].Down.Id);
                           
                        }
                        placeholders.Add(pipe3.Id);
                        placeholders.Add(pipe4.Id);
                        PlumbingUtils.ConvertPipePlaceholders(doc, placeholders);
                        
                        tran.Commit();
                    }

                    uidoc.Selection.SetElementIds(plumbingFixtureModelErrors.Select(x => x.PlumbingFixture.Id).ToList());
                    return Result.Succeeded;
                  

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
       
        private static List<Element> GetElements(Document document, List<Reference> references)
        {
            List<Element> e = new List<Element>();
            for (int i = 0; i < references.Count; i++)
            {
                e.Add(document.GetElement(references[i]));
            }
            return e;
        }
    
        private static Pipe ExtendPipe(Document document, ElementId elementId, PipeType pipeType, Level level, Pipe pipe, PlumbingFixtureModel plumbingFixtureModel)
        {
            Pipe p;
            if (ConditionExtendPipe(pipe, plumbingFixtureModel))
            {
                Line line = (pipe.Location as LocationCurve).Curve as Line;
                Location location = pipe.Location;
                Line line1 = Line.CreateBound(line.GetEndPoint(0), plumbingFixtureModel.XYZBranchElbow + 4 * line.Direction * plumbingFixtureModel.DiameterMain);
                (pipe.Location as LocationCurve).Curve = line1;
                Line line2 = (pipe.Location as LocationCurve).Curve as Line;
                XYZ vecNormal = plumbingFixtureModel.GetVectorNormal(pipe);
                XYZ vectoSlanted = line2.Direction.CrossProduct(vecNormal);
                XYZ pointEnd = line2.GetEndPoint(1);
                 p = Pipe.CreatePlaceholder(document, elementId, pipeType.Id, level.Id, pointEnd, pointEnd - vectoSlanted * plumbingFixtureModel.DiameterMain * 2 + line2.Direction * plumbingFixtureModel.DiameterMain * 2);
                p.get_Parameter(BuiltInParameterID.DiameterPipeID).Set(plumbingFixtureModel.DiameterMain);
            }
            else
            {
                Line line = (pipe.Location as LocationCurve).Curve as Line;
                XYZ vecNormal = plumbingFixtureModel.GetVectorNormal(pipe);
                XYZ vectoSlanted = line.Direction.CrossProduct(vecNormal);
                XYZ pointEnd = line.GetEndPoint(1);
                 p = Pipe.CreatePlaceholder(document, elementId, pipeType.Id, level.Id, pointEnd, pointEnd - vectoSlanted * plumbingFixtureModel.DiameterMain * 2 + line.Direction * plumbingFixtureModel.DiameterMain * 2);
                p.get_Parameter(BuiltInParameterID.DiameterPipeID).Set(plumbingFixtureModel.DiameterMain);
            }
            return p;
        }
        private static bool ConditionExtendPipe(Pipe pipe, PlumbingFixtureModel plumbingFixtureModel)
        {
            Line line = (pipe.Location as LocationCurve).Curve as Line;
            if (plumbingFixtureModel.Origin.DistanceTo(line.GetEndPoint(0)) + 3 * plumbingFixtureModel.DiameterMain < line.Length) return false;
            return true;
        }

        
    }
}

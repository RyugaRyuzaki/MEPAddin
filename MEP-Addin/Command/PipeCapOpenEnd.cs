
#region Namespaces
using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MEP_Addin.Library;
using SettingMEP;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Message = System.Windows.Forms.MessageBox;
#endregion


namespace MEP_Addin.Command
{
    [Transaction(TransactionMode.Manual)]
    public class PipeCapOpenEnd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // code



            SaveModel SaveModel = SaveModel.GetSaveModel(doc);
            try
            {
                FilterPipe filterPipe = new FilterPipe(doc);
                Reference referencePipe = uidoc.Selection.PickObject(ObjectType.Element, filterPipe);
                Pipe pipe = doc.GetElement(referencePipe) as Pipe;
                Line line = (pipe.Location as LocationCurve).Curve as Line;
                double Slope = pipe.get_Parameter(BuiltInParameterID.SlopeID).AsDouble();
                XYZ Point = (PointModel.AreEqual(Slope, 0)) ? (line.GetEndPoint(1)) : ((line.GetEndPoint(0).Z < line.GetEndPoint(1).Z) ? (line.GetEndPoint(1)) : (line.GetEndPoint(0)));
                XYZ Direction = (PointModel.AreEqual(Slope, 0)) ? (line.Direction) : ((line.GetEndPoint(0).Z < line.GetEndPoint(1).Z) ? (line.Direction) : (-line.Direction));
                XYZ VectorNormal = GetVectorNormal(Direction, Point);
                PipeType pipeType = pipe.PipeType;
                Level level = doc.GetElement(pipe.get_Parameter(BuiltInParameterID.PipeLevelID).AsElementId()) as Level;
                ElementId elementId = pipe.get_Parameter(BuiltInParameterID.SystemTypePipe).AsElementId();
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Create Pipe");
                    Pipe pipecap = CreatePipeCap(doc,pipe, elementId, pipeType, level, Point, VectorNormal, Direction,SaveModel);
                    Connector connector1 = ConnectorProcess.GetConnectorPipe(pipe, Point);
                    Connector connector2 = ConnectorProcess.GetConnectorPipe(pipecap, Point);
                    if (connector1 != null && connector2 != null)
                    {
                        FamilyInstance familyInstance = doc.Create.NewElbowFitting(connector1, connector2);
                    }
                    try
                    {
                        PlumbingUtils.PlaceCapOnOpenEnds(doc, pipecap.Id, pipecap.PipeType.Id);
                    }
                    catch (Exception e)
                    {

                        Message.Show(e.Message);
                    }
                   
                    transaction.Commit();
                }
                return Result.Succeeded;



            }
            catch (System.Exception e)
            {

                Message.Show(e.Message);
                return Result.Cancelled;
            }

        }

        private Pipe CreatePipeCap(Document doc,Pipe pipe, ElementId elementId, PipeType pipeType, Level level, XYZ point,XYZ direction, XYZ vectorNormal, SaveModel saveModel)
        {
            double DiameterMain = pipe.get_Parameter(BuiltInParameterID.DiameterPipeID).AsDouble();
            XYZ a = point + vectorNormal * saveModel.S2* DiameterMain / Math.Sqrt(2)+ direction *saveModel.S2 * DiameterMain / Math.Sqrt(2);
               var pipecap = Pipe.Create(doc, elementId, pipeType.Id, level.Id, point, a);
            if (pipecap != null)
            {
               
                pipecap.get_Parameter(BuiltInParameterID.DiameterPipeID).Set(DiameterMain);
            }
            return pipecap;
        }

        private XYZ GetVectorNormal(XYZ direction, XYZ point)
        {
            XYZ a = point + XYZ.BasisZ * 2;
            Line line = Line.CreateBound(point, a);
            XYZ dir = line.Direction;
            double dot = direction.DotProduct(dir);
            XYZ b = a - direction * dot;
            Line line2 = Line.CreateBound(point, b);
            return line2.Direction;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Forms = System.Windows.Forms;

namespace RAA_Int_Module_04_Skills_Solution
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Your code goes here
            //METHOD A
            //1.Get all Links
            FilteredElementCollector linkCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkType));

            //Loop through links and get doc
            Document linkedDoc = null;
            RevitLinkInstance link = null;

            foreach (RevitLinkType rvtLink in linkCollector)
            {
                if (rvtLink.GetLinkedFileStatus() == LinkedFileStatus.Loaded)
                {
                    link = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_RvtLinks)
                        .OfClass(typeof(RevitLinkInstance))
                        .Where(x => x.GetTypeId() == rvtLink.Id)
                        .First() as RevitLinkInstance;

                    linkedDoc = link.GetLinkDocument();
                }
            }

            //Getting the rooms from linked model
            FilteredElementCollector collectorA = new FilteredElementCollector(linkedDoc)
                .OfCategory(BuiltInCategory.OST_Rooms);
            //TaskDialog.Show("Test Method", $"Linked model have {collectorA.Count()} Rooms");

            //Create space from rooms from linked model

            List<Room> allLinkRooms = new FilteredElementCollector(linkedDoc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<Room>()
                .ToList();

            //Getting level from current view
            Level curLevel = doc.ActiveView.GenLevel;

            foreach (Room room in allLinkRooms)
            {
                //Getting room data
                string roomName = room.Name;
                string roomNum = room.Number;
                string roomComments = room.LookupParameter("Comments").AsString();
                //Get Room location point
                LocationPoint roomPoint = room.Location as LocationPoint;

                //Transaction
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Space Creation");
                    SpatialElement newSpace = doc.Create.NewSpace(curLevel, new UV(roomPoint.Point.X, roomPoint.Point.Y));
                    newSpace.Name = roomName;
                    newSpace.Number = roomNum;
                    newSpace.LookupParameter("Comments").Set(roomComments);
                    t.Commit();
                }
            }
            return Result.Succeeded;
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData.Data;
        }
    }
}
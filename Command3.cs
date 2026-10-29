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
    public class Command3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // 3. get Group elements from opened file
            List<GroupType> allGroups = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_IOSModelGroups)
                .WhereElementIsElementType()
                .Cast<GroupType>()
                .ToList();

            // Collected all rooms from host model
            List<Space> allHostRooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_MEPSpaces)
                .WhereElementIsNotElementType()
                .Cast<Space>()
                .ToList();
            //Method B- Placing the groups at space locations


            int groupPlaced = 0;
            foreach (Space curSpace in allHostRooms)
            {
                string commentPara = curSpace.LookupParameter("Comments").AsString();
                LocationPoint locPoint = curSpace.Location as LocationPoint;
                XYZ insPoint = locPoint.Point;


                foreach(GroupType curGroup in allGroups)
                {
                    if (curGroup.Name == commentPara)
                    {
                        using (Transaction t = new Transaction(doc))
                        {
                            t.Start("Copiying the group");
                            doc.Create.PlaceGroup(new XYZ(insPoint.X, insPoint.Y, 0), curGroup);
                            
                            groupPlaced++;
                            t.Commit();
                        }
                    }

                }

            }
            TaskDialog.Show("Completed", $"Placed {groupPlaced} groups in host model");

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand3";
            string buttonTitle = "Button 3";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 3");

            return myButtonData.Data;
        }
    }

}

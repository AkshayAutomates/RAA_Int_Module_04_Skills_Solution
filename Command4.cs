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
    public class Command4 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Get open file with specifix name
            //1.create doc variable
            Document openDoc = null;

            // 2.Loop thought each opened doc and look for match
            foreach(Document curDoc in uiapp.Application.Documents)
            {
                if (curDoc.PathName.Contains("Sample 03"))
                    openDoc = curDoc;
            }

            // 3. Creating the multicat filter
            List<BuiltInCategory> catList = new List<BuiltInCategory>();
            catList.Add(BuiltInCategory.OST_Walls);
            catList.Add(BuiltInCategory.OST_GenericModel);

            ElementMulticategoryFilter catFilter = new ElementMulticategoryFilter(catList);

            //4. Elem Ids of filtered cat items
            List<ElementId> reqIdList = new FilteredElementCollector(openDoc)
                .WherePasses(catFilter)
                .WhereElementIsNotElementType()
                .Select(elem => elem.Id)
                .ToList();

            //Copy Paste Options
            CopyPasteOptions options = new CopyPasteOptions();

            //Transaction method
            using (Transaction t= new Transaction(doc))
            {
                t.Start("Copying the elements");
                ElementTransformUtils.CopyElements(openDoc, reqIdList, doc, null, options);
                t.Commit();
            }


            TaskDialog.Show("Completed", $"Coppied {reqIdList.Count()} Elements in Host Model");

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand4";
            string buttonTitle = "Button 4";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 4");

            return myButtonData.Data;
        }
    }

}

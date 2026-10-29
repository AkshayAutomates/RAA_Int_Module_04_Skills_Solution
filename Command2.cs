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
    public class Command2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Method B

            //1.prompt user to select RVT file
            
            string revitFile = "";

            Forms.OpenFileDialog ofd = new Forms.OpenFileDialog();
            ofd.Title = "Select Revit file";
            ofd.InitialDirectory = @"C:\";
            ofd.Filter = "Revit files (*.rvt)|*.rvt";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() != Forms.DialogResult.OK)
                return Result.Failed;

            revitFile = ofd.FileName;

            // 2. Open selected file in background
            UIDocument openUIDoc = uiapp.OpenAndActivateDocument(revitFile);
            Document openDoc = openUIDoc.Document;

            // 3. get Group elements from opened file
            List<ElementId> allGroupsId = new FilteredElementCollector(openDoc)
                .OfCategory(BuiltInCategory.OST_IOSModelGroups)
                .WhereElementIsElementType()
                .Cast<GroupType>()
                .Select(elem => elem.Id)
                .ToList();

            // 4.copy element
            CopyPasteOptions options = new CopyPasteOptions();

            //int groupCopied = 0;
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Copiying the group");
                List<ElementId> allgroupList = ElementTransformUtils.
                    CopyElements(openDoc, allGroupsId, doc, null, options).ToList();
                t.Commit();
            }
            TaskDialog.Show("Test Metho", $"Copied {allGroupsId.Count()} groups in host model");

            //Close opened document and make host model active
            uiapp.OpenAndActivateDocument(doc.PathName);
            openDoc.Close(false);

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand2";
            string buttonTitle = "Button 2";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 2");

            return myButtonData.Data;
        }
    }

}

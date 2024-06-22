using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using generalsolution.Addin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generalsolution
{
    internal class method
    {
        private Document doc;
        public method(Document document)
        {
           doc= document;
        }
        public void Showdialogbox(IEnumerable list)
        {
            dialogbox box = new dialogbox(list);
            box.ShowDialog();

        }
        public void showtask(string name)
        {
            TaskDialog.Show("title", name);
        }
        public void Visualized(IEnumerable<GeometryObject> GeometryObject) 
        {
            DirectShape.CreateElement(doc,new ElementId(BuiltInCategory.OST_GenericModel)).SetShape(GeometryObject.ToList());
        }
        public double Round(double Number )
        {
            return Math.Round(Number, 5);
        }
        public XYZ  Avarage_point(List<XYZ> Pt)
        {
           double xavarage = Pt.Select(p => p.X).ToList().Average();
           double yavarage = Pt.Select(p => p.Y).ToList().Average();
           double zavarage = Pt.Select(p => p.Z).ToList().Average();
            return  new XYZ(xavarage, yavarage, zavarage);
        }
        public  XYZ get_faceorigin(Face face)
        {
            List<XYZ> Points =  face.EdgeLoops.get_Item(0).Cast<Edge>().ToList().Select(e => e.AsCurve().Evaluate(.5, true)).ToList();
            return Avarage_point(Points);
        }
    }
}

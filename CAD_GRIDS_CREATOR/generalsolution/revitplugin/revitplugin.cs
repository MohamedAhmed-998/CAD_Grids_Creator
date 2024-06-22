using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics.CodeAnalysis;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.DB.Plumbing;
using System.Windows.Shapes;
using Line = Autodesk.Revit.DB.Line;
using Autodesk.Revit.DB.Structure.StructuralSections;
using System.Security.Cryptography;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Mechanical;
using System.Windows.Documents;
using System.Windows.Controls;
using Grid = Autodesk.Revit.DB.Grid;
using System.Security.AccessControl;

namespace generalsolution
{
    class Filter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Wall)
            {
                if (elem.Document.GetElement(elem.Id).Name == "Generic - 200mm")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    //filter_for mechducts
    class linesfilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is CurveElement)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    class textfilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is TextNote)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }


    class instancefilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    class gridfilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Grid)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    class levelfilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Level)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    class rebarfilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Rebar)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    class LinkCadFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is ImportInstance)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    class floorfilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
           if (elem is Floor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class revitplugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region generalmethod
            UIApplication uiapp = commandData.Application;
            Application app = commandData.Application.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            method m = new method(doc);

            #endregion
            TransactionGroup tg = new TransactionGroup(doc, "tg");
            tg.Start();
           
            #region grid2
            Element cadlink = null;
            try
            {
                 cadlink = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element, new LinkCadFilter(), "Select link cad"));

            }
            catch { }
            if(cadlink!=null)
            {
            //convert elements to geoboject then to lines
                var geoelement = (cadlink.get_Geometry(new Options()).First() as GeometryInstance).GetInstanceGeometry();
                List<Line>lines = new List<Line>();
                foreach (GeometryObject geobject in geoelement)
                {
                  if (geobject is Line)
                    {
                        lines.Add(geobject as Line);
                    }
                }
                List<Line>linex=new List<Line>();
                List<Line>liney=new List<Line>();
                List<Line>linexy=new List<Line>();
            // grouping lines to directions
                List<Grid>gridlines= new List<Grid>();
                foreach (Line line in lines)
                {
                  if (line.Direction.X != 0 && line.Direction.Y == 0 && line.Direction.Z == 0)
                    {
                        linex.Add(line);
                    }
                  else if(line.Direction.Y != 0 && line.Direction.X == 0 && line.Direction.Z == 0)
                    {
                        liney.Add(line);
                    }
                    else
                    {
                        linexy.Add(line);
                    }
                }
                //sorting lines 
                linex =linex.OrderBy(b => b.GetEndPoint(0).Y).ToList();
                liney =liney.OrderBy(c => c.GetEndPoint(0).X).ToList();
                List<Grid>gridx=new List<Grid>();
                List<Grid>gridy=new List<Grid>();
                List<Grid>gridxy=new List<Grid>();
                List<List<Line>> Groupedlines = lines.GroupBy(l => l.Direction.X).Select(p=>p.ToList()).ToList();
                //creting grids 
                Transaction tgrid = new Transaction(doc, "tgrid");
                {
                    tgrid.Start();
                    foreach (Line line1 in linex)
                    {
                        XYZ p1 = line1.GetEndPoint(0);
                        XYZ p2 = line1.GetEndPoint(1);
                        Line create_line = Line.CreateBound(p1, p2);
                        Grid grid = Grid.Create(doc, create_line);
                        gridx.Add(grid);
                    } 
                    foreach (Line line1 in liney)
                    {
                        XYZ p1 = line1.GetEndPoint(0);
                        XYZ p2 = line1.GetEndPoint(1);
                        Line create_line = Line.CreateBound(p1, p2);
                        Grid grid = Grid.Create(doc, create_line);
                        gridy.Add(grid);
                    } 
                    foreach (Line line1 in linexy)
                    {
                        XYZ p1 = line1.GetEndPoint(0);
                        XYZ p2 = line1.GetEndPoint(1);
                        Line create_line = Line.CreateBound(p1, p2);
                        Grid grid = Grid.Create(doc, create_line);
                        gridxy.Add(grid);
                    }
                
                    List<Grid>allgrid=gridx.Concat(gridy).Concat(gridxy).ToList();
                    char n = 'A';
                    int number = 1;
                    var grouped_grids = gridlines.GroupBy(g => (g.Curve as Line).Direction.X).Select(k=>k.ToList()).ToList();
                for (int i = 0; i < gridx.Count(); i++)
                {
                    gridx[i].Name = number.ToString();
                    number++;
                }
                for(int i = 0; i < gridy.Count; i++)
                {
                    gridy[i].Name = n.ToString();
                    n++;
                }
                for (int i = 0; i < gridxy.Count; i++)
                {
                    gridxy[i].Name = n.ToString();
                    n++;
                }
                   

                }
                    tgrid.Commit();
            }
            #endregion
            tg.Assimilate();
            return Result.Succeeded;

        }



    }
       
        }
    

      


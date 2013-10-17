using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void PrepareShapes(ref TransitDB tdb, ref OriginalMaps originalMaps)
        {
            tdb.shapeMatrix = new List<ShapeVector>();
            var cshapeseg = db.shapes.GroupBy(sh => sh.shape_id).ToDictionary(sh => sh.Key, sh => sh.ToList());

            foreach (var key in cshapeseg.Keys)
            {
                var rshape = new List<float>();
                
                var shape = cshapeseg[key].OrderBy(s => s.shape_dist_traveled).ToList();
                shape.ForEach(shape_element => {
                    rshape.Add((float)shape_element.shape_dist_traveled);
                    rshape.Add((float)shape_element.shape_pt_lat);
                    rshape.Add((float)shape_element.shape_pt_lon);
                });

                var arrayShape = new ShapeVector {
                    verticesVector = rshape.ToArray()
                };

                tdb.shapeMatrix.Add(arrayShape);
                originalMaps.originalShapeIndexMap[key] = tdb.shapeMatrix.Count;
            }
        }
    }
}

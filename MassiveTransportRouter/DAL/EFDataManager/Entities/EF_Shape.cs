using MTR.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.DataAccess.EFDataManager.Entities
{
    public class EF_Shape
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int32 Id { get; set; }

        public String OriginalId { get; set; }
        public Double ShapePointLatitude { get; set; }
        public Double ShapePointLongitude { get; set; }
        public Int32 ShapePointSequence { get; set; }
        public Double ShapeDistanceTravelled { get; set; }

        public EF_Shape() {}

        public EF_Shape(GTFS_Shape shape)
        {
            OriginalId = shape.ShapeId;
            ShapePointLatitude = shape.ShapePointLatitude;
            ShapePointLongitude = shape.ShapePointLongitude;
            ShapePointSequence = shape.ShapePointSequence;
            ShapeDistanceTravelled = shape.ShapeDistanceTravelled;
        }
    }
}

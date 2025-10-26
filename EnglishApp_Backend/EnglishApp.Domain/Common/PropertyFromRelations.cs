using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Common
{
    public class RelationProperty
    {
        public string Property { get; set; } = string.Empty;
        public string PropertyWithRelations { get; set; } = string.Empty;
    }

    public class PropertyFromRelations
    {
        public Dictionary<string, List<RelationProperty>> GetPropertyFromRelations { get; } =
            new Dictionary<string, List<RelationProperty>>
            {
                ["user"] = new List<RelationProperty>
                {
                    new() {
                        Property = "createdBy",
                        PropertyWithRelations = "CreatedByUser.Name"
                    }
                }
            };
    }
}

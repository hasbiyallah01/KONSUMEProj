﻿using DaticianProj.Core.Domain.Entities;
using System.Text.Json.Serialization;

namespace DaticianProj.Core.Domain.Entities
{
    public class Role : Auditables
    {
        [JsonInclude]
        public string Name { get; set; } = default!;
        [JsonInclude]
        public string? Description { get; set; }
        public ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}

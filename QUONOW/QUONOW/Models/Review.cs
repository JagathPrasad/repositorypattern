//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QUONOW.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Review
    {
        public System.Guid Id { get; set; }
        public string Comments { get; set; }
        public Nullable<int> Star { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.Guid> ProductId { get; set; }
        public Nullable<bool> IsApprove { get; set; }
    
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Final_Coursemo
{
    using System;
    using System.Collections.Generic;
    
    public partial class Waitlist
    {
        public int WID { get; set; }
        public string NetId { get; set; }
        public Nullable<int> CRN { get; set; }
    
        public virtual Cours Cours { get; set; }
        public virtual Student Student { get; set; }
    }
}

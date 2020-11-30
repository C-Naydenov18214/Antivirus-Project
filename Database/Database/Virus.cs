using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    /// <summary>
    /// Virus information.
    /// </summary>
    [Table("viruses")]
    public class Virus
    {
        public int Id { get; set; }
        
        public string VirusType { get; set; }
        
        public string Signature { get; set; }
        
        public Dictionary<string, object> Metadata { get; set; }
    }
}
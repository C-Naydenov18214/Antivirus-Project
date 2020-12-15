using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using DeveloperKit.Report;

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

        public Virus(VirusInfo virusInfo)
        {
            Metadata = new Dictionary<string, object>();
            VirusType = "unknown";
            Signature = BitConverter.ToString(virusInfo.Signature);
            foreach (var el in virusInfo.Inforamation)
            {
                Metadata.Add(el.Key, el.Value);
            }
        }
    }
}
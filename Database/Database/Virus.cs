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
        
        public List<KeyValuePair<string, object>> Metadata { get; set; }

        public Virus(VirusInfo virusInfo)
        {
            Metadata = new List<KeyValuePair<string, object>>();
            VirusType = "unknown";
            Signature = BitConverter.ToString(virusInfo.Signature);
            try
            {
                foreach (var el in virusInfo.Inforamation)
                {
                    Metadata.Add(el);
                }
            }
            catch (Exception e)
            {
                Console.Write("Got an exception {0}",e);
            }
        }
    }
}
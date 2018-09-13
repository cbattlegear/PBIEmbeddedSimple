using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//These are all downloaded via NuGet
using Microsoft.PowerBI.Api.V2.Models;

namespace PBIEmbeddedSimple
{

    //This class holds all the embedding information traditionally this should be broken out into a separate class file
    //but for ease of reading and simplification we are duplicating the class here.
    public class EmbedConfig
    {
        public string Id { get; set; }
        public string EmbedUrl { get; set; }
        public EmbedToken EmbedToken { get; set; }
        public int MinutesToExpiration
        {
            get
            {
                var minutesToExpiration = EmbedToken.Expiration.Value - DateTime.UtcNow;
                return minutesToExpiration.Minutes;
            }
        }
        public bool? IsEffectiveIdentityRolesRequired { get; set; }
        public bool? IsEffectiveIdentityRequired { get; set; }
        public bool EnableRLS { get; set; }
        public string Username { get; set; }
        public string Roles { get; set; }
        public string ErrorMessage { get; internal set; }
    }
}
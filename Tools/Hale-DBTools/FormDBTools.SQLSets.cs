using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale.DBTools
{
    partial class FormDBTools
    {
        public ReadOnlyDictionary<string, string[]> SQLSets = // Ugly, but unfortunatly the "right" way of doing this
            new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>()
        {
            {
                "CreateUSPs", new []
                {
                    "CreateUSPsForChecks",
                    "CreateUSPsForHosts",
                    "CreateUSPsForMetadata",
                    "CreateUSPsForUsers",
                    "CreateUtilityUSPs"
                }
            },
            {
                "DeleteUSPs", new []
                {
                    "DeleteUSPsForChecks",
                    "DeleteUSPsForHosts",
                    "DeleteUSPsForMetadata",
                    "DeleteUSPsForUsers",
                    "DeleteUtilityUSPs"
                }
            }
        });


    }
}

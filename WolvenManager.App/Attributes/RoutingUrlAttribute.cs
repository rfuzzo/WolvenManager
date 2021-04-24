using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenManager.App.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RoutingUrlAttribute : Attribute
    {
        public Constants.RoutingIDs RoutingId { get; }

        public RoutingUrlAttribute(Constants.RoutingIDs routingId)
        {
            this.RoutingId = routingId;
        }


    }
}

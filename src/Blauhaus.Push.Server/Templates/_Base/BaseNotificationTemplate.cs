using System.Collections.Generic;

namespace Blauhaus.Push.Server.Templates._Base
{
    public abstract class BaseNotificationTemplate
    {
        protected BaseNotificationTemplate(string name, List<string> properties)
        {
            Name = name;
            DataProperties = properties ?? new List<string>();
        }

        public string Name { get; }
        public List<string> DataProperties { get; }


        
    }
}
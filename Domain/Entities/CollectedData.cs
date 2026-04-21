using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CollectedData
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Source { get; private set; } = string.Empty;
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Url { get; private set; } = string.Empty;
        public DateTime CollectedAt { get; private set; } = DateTime.UtcNow;

        private CollectedData() { }

        public CollectedData(string source, string title, string description, string url)
        {
            Source = source;
            Title = title;
            Description = description;
            Url = url;
            CollectedAt = DateTime.UtcNow;
        }
    }
}

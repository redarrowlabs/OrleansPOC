using System;

namespace Common
{
    [Serializable]
    public class Entity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
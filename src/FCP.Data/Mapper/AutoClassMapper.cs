using System;

namespace FCP.Data
{
    /// <summary>
    /// Automatically maps an entity to a table using a combination of reflection and naming conventions for keys.
    /// </summary>
    public class AutoClassMapper<TEntity> : ClassMapper<TEntity> where TEntity : class
    {
        public AutoClassMapper()
        {            
            autoMap();
        }
    }
}

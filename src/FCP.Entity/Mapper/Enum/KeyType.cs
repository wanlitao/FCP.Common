
namespace FCP.Entity
{
    /// <summary>
    /// Used by ClassMapper to determine which entity property represents the key.
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// The property is not a key and is not automatically managed.
        /// </summary>
        notAKey,

        /// <summary>
        /// The property is an integery-based identity generated from the database.
        /// </summary>
        identity,

        /// <summary>
        /// The property is a Guid identity which is automatically managed.
        /// </summary>
        guid,

        /// <summary>
        /// The property is a key that is not automatically managed.
        /// </summary>
        assigned
    }
}

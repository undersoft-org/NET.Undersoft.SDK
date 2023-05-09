/// <summary>
/// The Deal namespace.
/// </summary>
namespace System.Deal
{
    using System;

    #region Enums

    /// <summary>
    /// Enum JsonModes
    /// </summary>
    public enum JsonModes
    {
        /// <summary>
        /// All
        /// </summary>
        All,
        /// <summary>
        /// The key value
        /// </summary>
        KeyValue,
        /// <summary>
        /// The array
        /// </summary>
        Array
    }

    #endregion




    /// <summary>
    /// Class JsonArrayAttribute. This class cannot be inherited.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class JsonArrayAttribute : Attribute
    {
        #region Constructors




        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArrayAttribute" /> class.
        /// </summary>
        public JsonArrayAttribute()
        {
        }

        #endregion
    }




    /// <summary>
    /// Class JsonIgnoreAttribute. This class cannot be inherited.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class JsonIgnoreAttribute : Attribute
    {
        #region Constructors




        /// <summary>
        /// Initializes a new instance of the <see cref="JsonIgnoreAttribute" /> class.
        /// </summary>
        public JsonIgnoreAttribute()
        {
        }

        #endregion
    }




    /// <summary>
    /// Class JsonMemberAttribute. This class cannot be inherited.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class JsonMemberAttribute : Attribute
    {
        #region Constructors




        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMemberAttribute" /> class.
        /// </summary>
        public JsonMemberAttribute()
        {
        }

        #endregion

        #region Properties




        /// <summary>
        /// Gets or sets the serial mode.
        /// </summary>
        /// <value>The serial mode.</value>
        public JsonModes SerialMode { get; set; } = JsonModes.All;

        #endregion
    }




    /// <summary>
    /// Class JsonObjectAttribute. This class cannot be inherited.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public sealed class JsonObjectAttribute : Attribute
    {
        #region Constructors




        /// <summary>
        /// Initializes a new instance of the <see cref="JsonObjectAttribute" /> class.
        /// </summary>
        public JsonObjectAttribute()
        {
        }

        #endregion
    }
}

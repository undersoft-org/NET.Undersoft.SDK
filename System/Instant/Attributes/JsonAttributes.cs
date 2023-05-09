namespace System.Instant.Json
{
    public enum JsonModes
    {
        All,
        KeyValue,
        Array
    }

    [AttributeUsage(
        AttributeTargets.Class
            | AttributeTargets.Struct
            | AttributeTargets.Enum
            | AttributeTargets.Delegate
            | AttributeTargets.Property
            | AttributeTargets.Field,
        Inherited = false
    )]
    public sealed class JsonArrayAttribute : JsonAttribute
    {
        public JsonArrayAttribute() { }
    }

    [AttributeUsage(
        AttributeTargets.Class
            | AttributeTargets.Struct
            | AttributeTargets.Enum
            | AttributeTargets.Delegate
            | AttributeTargets.Property
            | AttributeTargets.Field,
        Inherited = false
    )]
    public class JsonAttribute : Attribute
    {
        public JsonAttribute() { }
    }

    [AttributeUsage(
        AttributeTargets.Class
            | AttributeTargets.Struct
            | AttributeTargets.Enum
            | AttributeTargets.Delegate
            | AttributeTargets.Property
            | AttributeTargets.Field,
        Inherited = false
    )]
    public sealed class JsonIgnoreAttribute : JsonAttribute
    {
        public JsonIgnoreAttribute() { }
    }

    [AttributeUsage(
        AttributeTargets.Class
            | AttributeTargets.Struct
            | AttributeTargets.Enum
            | AttributeTargets.Delegate
            | AttributeTargets.Property
            | AttributeTargets.Field,
        Inherited = false
    )]
    public sealed class JsonMemberAttribute : JsonAttribute
    {
        public JsonMemberAttribute() { }

        public JsonModes SerialMode { get; set; } = JsonModes.All;
    }

    [AttributeUsage(
        AttributeTargets.Class
            | AttributeTargets.Struct
            | AttributeTargets.Enum
            | AttributeTargets.Delegate
            | AttributeTargets.Property
            | AttributeTargets.Field,
        Inherited = false
    )]
    public sealed class JsonObjectAttribute : JsonAttribute
    {
        public JsonObjectAttribute() { }
    }
}

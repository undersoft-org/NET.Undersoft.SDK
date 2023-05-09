
// <copyright file="ExceptionConverter.cs" company="UltimatR.Core">
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>



using System.Text.Json.Serialization;

/// <summary>
/// The Logs namespace.
/// </summary>
namespace System.Logs
{
    using Text.Json;

    /// <summary>
    /// Class ExceptionConverter.
    /// Implements the <see cref="System.Text.Json.Serialization.JsonConverter{System.Exception}" />
    /// </summary>
    /// <seealso cref="System.Text.Json.Serialization.JsonConverter{System.Exception}" />
    public class ExceptionConverter : JsonConverter<Exception>
    {
        /// <summary>
        /// Reads and converts the JSON to type <typeparamref name="T" />.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Exception Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="options">The options.</param>
        public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Message", value.Message);
            writer.WriteString("HelpLink", value.HelpLink);
            writer.WriteString("HResult", value.HResult.ToString());
            writer.WriteString("Source", value.Source);
            writer.WriteString("StackTrace", value.StackTrace);
            writer.WriteEndObject();
        }
    }
}


// <copyright file="ISortTerm.cs" company="UltimatR.Core">
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>



/// <summary>
/// The Treatments namespace.
/// </summary>
namespace System.Instant.Treatments
{
    using System.Linq;




    /// <summary>
    /// Interface ISortTerm
    /// </summary>
    public interface ISortTerm
    {
        #region Properties




        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        SortDirection Direction { get; set; }




        /// <summary>
        /// Gets or sets the rubric identifier.
        /// </summary>
        /// <value>The rubric identifier.</value>
        int RubricId { get; set; }




        /// <summary>
        /// Gets or sets the name of the rubric.
        /// </summary>
        /// <value>The name of the rubric.</value>
        string RubricName { get; set; }

        #endregion
    }
}

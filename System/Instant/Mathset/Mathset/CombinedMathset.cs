
// <copyright file="CombinedMathset.cs" company="UltimatR.Core">
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>



/// <summary>
/// The Mathset namespace.
/// </summary>
namespace System.Instant.Mathset
{




    /// <summary>
    /// Class CombinedMathset.
    /// </summary>
    public abstract class CombinedMathset
    {
        #region Fields

        /// <summary>
        /// The data parameters
        /// </summary>
        public IFigures[] DataParameters = new IFigures[1];
        /// <summary>
        /// The parameters count
        /// </summary>
        public int ParametersCount = 0;

        #endregion

        #region Methods




        /// <summary>
        /// Computes this instance.
        /// </summary>
        public abstract void Compute();






        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <param name="paramid">The paramid.</param>
        /// <returns>System.Int32.</returns>
        public int GetColumnCount(int paramid)
        {
            return DataParameters[paramid].Rubrics.Count;
        }






        /// <summary>
        /// Gets the index of.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>System.Int32.</returns>
        public int GetIndexOf(IFigures v)
        {
            for (int i = 0; i < ParametersCount; i++)
                if (DataParameters[i] == v) return 1 + i;
            return -1;
        }






        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <param name="paramid">The paramid.</param>
        /// <returns>System.Int32.</returns>
        public int GetRowCount(int paramid)
        {
            return DataParameters[paramid].Count;
        }






        /// <summary>
        /// Puts the specified v.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>System.Int32.</returns>
        public int Put(IFigures v)
        {
            int index = GetIndexOf(v);
            if (index < 0)
            {
                DataParameters[ParametersCount] = v;
                return 1 + ParametersCount++;
            }
            else
            {
                DataParameters[index] = v;
            }
            return index;
        }





        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="p">The p.</param>
        public void SetParams(IFigures p)
        {
            Put(p);
        }







        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetParams(IFigures p, int index)
        {
            if (index < ParametersCount)
            {
                if (ReferenceEquals(DataParameters[index], p))
                    return false;
                else
                    DataParameters[index] = p;
            }
            return false;
        }






        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="paramCount">The parameter count.</param>
        public void SetParams(IFigures[] p, int paramCount)
        {
            DataParameters = p;
            ParametersCount = paramCount;
        }

        #endregion
    }
}

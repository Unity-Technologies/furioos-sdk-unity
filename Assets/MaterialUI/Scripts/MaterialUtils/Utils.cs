//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

namespace MaterialUI
{
    /// <summary>
    /// Static class containing miscellanous utilities.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Sets a given bool value, if the other bool returns true. Otherwise does nothing.
        /// </summary>
        /// <param name="boolean">The boolean to modify.</param>
        /// <param name="otherBool">The boolean to check.</param>
        public static void SetBoolValueIfTrue(ref bool boolean, bool otherBool)
        {
            if (otherBool)
            {
                boolean = true;
            }
        }
    }
}
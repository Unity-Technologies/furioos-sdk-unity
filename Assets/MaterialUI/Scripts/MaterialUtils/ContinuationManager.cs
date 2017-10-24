//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace MaterialUI
{
    /// <summary>
    /// Static class to perform jobs over a series of Updates, in edit mode.
    /// </summary>
    public static class ContinuationManager
    {
        /// <summary>
        /// A job to be performed over a series of Updates.
        /// </summary>
        private class ContinuationJob
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ContinuationJob"/> class.
            /// </summary>
            /// <param name="completed">Function to check if the job is completed.</param>
            /// <param name="continueWith">Called upon job completion.</param>
            public ContinuationJob(Func<bool> completed, Action continueWith)
            {
                Completed = completed;
                ContinueWith = continueWith;
            }
            /// <summary>
            /// Function to check if a job is completed.
            /// </summary>
            public Func<bool> Completed { get; private set; }
            /// <summary>
            /// Called upon job completion.
            /// </summary>
            public Action ContinueWith { get; private set; }
        }

        /// <summary>
        /// The list of active jobs.
        /// </summary>
        private static readonly List<ContinuationJob> m_Jobs = new List<ContinuationJob>();

        /// <summary>
        /// Adds a job to the job list and starts working on it.
        /// </summary>
        /// <param name="completed">Function to check if the job is completed.</param>
        /// <param name="continueWith">Called upon job completion.</param>
        public static void Add(Func<bool> completed, Action continueWith)
        {
            if (!m_Jobs.Any())
            {
                EditorApplication.update += Update;
            }

            m_Jobs.Add(new ContinuationJob(completed, continueWith));
        }

        /// <summary>
        /// Called approximately 100 times per second.
        /// </summary>
        private static void Update()
        {
            for (int i = 0; i >= 0; --i)
            {
                var jobIt = m_Jobs[i];
                if (jobIt.Completed())
                {
                    jobIt.ContinueWith();
                    m_Jobs.RemoveAt(i);
                }
            }
            if (!m_Jobs.Any())
            {
                EditorApplication.update -= Update;
            }
        }
    }
}
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCron
{
    
    /// <summary>
    /// Class used to represent a crontab/configuration of jobs.
    /// </summary>
    [Serializable]
    public class CronTab
    {
        private List<CronJob> jobs;

        /// <summary>
        /// Constructor for the tab, creates a tab with no jobs.
        /// </summary>
        public CronTab()
        {
            jobs = new List<CronJob>();
        }

        /// <summary>
        /// Add a job to the tab.
        /// </summary>
        /// <remarks>Will not add null objects.</remarks>
        /// <param name="job">The job to add.</param>
        public void AddJob(CronJob job)
        {
            if (job != null)
            {
                jobs.Add(job);
            }
        }

        /// <summary>
        /// Clear all created jobs from the tab.
        /// </summary>
        public void Clear()
        {
            jobs.Clear();
        }

        /// <summary>
        /// Fetcha copy of the job list.
        /// </summary>
        /// <returns>A List<CronJob> containing a copy of the job list.</returns>
        public List<CronJob> GetJobs()
        {
            return new List<CronJob>(jobs);
        }
    }
}

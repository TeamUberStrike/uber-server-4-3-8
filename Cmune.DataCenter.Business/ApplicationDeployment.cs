using System;
using System.Collections.Generic;
using System.Linq;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.DataAccess;
using Cmune.DataCenter.Utils;

namespace Cmune.DataCenter.Business
{
    /// <summary>
    /// Manages the application versionning
    /// </summary>
    public static class ApplicationDeployment
    {
        /// <summary>
        /// Get all the application milestones for a specific application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<ApplicationMilestone> GetApplicationMilestones(int applicationId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                return cmuneDb.ApplicationMilestones.Where(aM => aM.ApplicationId == applicationId).OrderByDescending(mA => mA.Date).ToList();
            }
        }

        /// <summary>
        /// Get all the application milestones view for a specific application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static List<ApplicationMilestoneView> GetApplicationMilestonesView(int applicationId)
        {
            List<ApplicationMilestone> milestones = GetApplicationMilestones(applicationId);

            List<ApplicationMilestoneView> milestonesView = milestones.ConvertAll(new Converter<ApplicationMilestone, ApplicationMilestoneView>(m => new ApplicationMilestoneView(m.AmId, m.ApplicationId, m.Date, m.Description)));

            return milestonesView;
        }

        /// <summary>
        /// Create an application milestone
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="description"></param>
        /// <param name="creationTime"></param>
        /// <returns>False if the description is already used for the same application</returns>
        public static bool CreateApplicationMilestone(int applicationId, string description, DateTime creationTime)
        {
            bool isMilestoneCreated = false;

            // We need to shorten the description in case it's too long for the field

            description = description.ShortenText(50, true);

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                // We need to check if the description is not already used for the same application

                ApplicationMilestone applicationMilestone = cmuneDb.ApplicationMilestones.SingleOrDefault(aM => aM.ApplicationId == applicationId && aM.Description == description);

                if (applicationMilestone == null)
                {
                    // If not we can create it

                    applicationMilestone = new ApplicationMilestone();
                    applicationMilestone.ApplicationId = applicationId;
                    applicationMilestone.Date = creationTime;
                    applicationMilestone.Description = description;

                    cmuneDb.ApplicationMilestones.InsertOnSubmit(applicationMilestone);
                    cmuneDb.SubmitChanges();

                    isMilestoneCreated = true;
                }
            }

            return isMilestoneCreated;
        }

        /// <summary>
        /// Delete an application milestone
        /// </summary>
        /// <param name="milestoneId"></param>
        /// <returns></returns>
        public static bool DeleteApplicationMilestone(int milestoneId)
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                bool isApplicationMilestoneDeleted = false;

                ApplicationMilestone milestone = cmuneDb.ApplicationMilestones.SingleOrDefault(a => a.AmId == milestoneId);

                if (milestone != null)
                {
                    cmuneDb.ApplicationMilestones.DeleteOnSubmit(milestone);
                    cmuneDb.SubmitChanges();

                    isApplicationMilestoneDeleted = true;
                }

                return isApplicationMilestoneDeleted;
            }
        }

        /// <summary>
        /// Edits an application Milestone
        /// </summary>
        /// <param name="milestoneId"></param>
        /// <param name="description"></param>
        /// <param name="creationTime"></param>
        /// <returns></returns>
        public static bool EditApplicationMilestone(int milestoneId, string description, DateTime creationTime)
        {
            bool isMilestoneEdited = false;

            // We need to shorten the description in case it's too long for the field

            description = description.ShortenText(50, true);

            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                ApplicationMilestone milestone = GetApplicationMilestone(milestoneId, cmuneDb);

                if (milestone != null)
                {
                    // We need to check if the description is not already used for the same application

                    ApplicationMilestone duplicateMilestone = cmuneDb.ApplicationMilestones.SingleOrDefault(aM => aM.ApplicationId == milestone.ApplicationId && aM.Description == description && milestone.AmId != milestoneId);

                    if (duplicateMilestone == null)
                    {
                        milestone.Date = creationTime;
                        milestone.Description = description;

                        cmuneDb.SubmitChanges();

                        isMilestoneEdited = true;
                    }
                }
            }

            return isMilestoneEdited;
        }

        /// <summary>
        /// Gets an application milestone
        /// </summary>
        /// <param name="milestoneId"></param>
        /// <param name="cmuneDb"></param>
        /// <returns></returns>
        private static ApplicationMilestone GetApplicationMilestone(int milestoneId, CmuneDataContext cmuneDb)
        {
            ApplicationMilestone milestone = null;

            if (cmuneDb != null)
            {
                milestone = cmuneDb.ApplicationMilestones.SingleOrDefault(a => a.AmId == milestoneId);
            }

            return milestone;
        }

        /// <summary>
        /// Get list of applications
        /// </summary>
        /// <returns></returns>
        public static List<Application> GetApplications()
        {
            using (CmuneDataContext cmuneDb = new CmuneDataContext())
            {
                List<Application> applications = cmuneDb.Applications.ToList();
                return applications;
            }
        }
    }
}
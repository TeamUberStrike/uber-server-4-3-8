// -----------------------------------------------------------------------
// <copyright file="WeeklySpecialService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace UberStrike.DataCenter.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cmune.DataCenter.Business;
    using UberStrike.Core.Types;
    using UberStrike.DataCenter.Common.Entities;
    using UberStrike.DataCenter.DataAccess;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class WeeklySpecialService
    {
        public static readonly int MaxTitleLength = 100;
        public static readonly int MaxTextLength = 500;
        public static readonly int MaxImageUrlLength = 250;

        private static readonly int RecentCount = 10;

        /// <summary>
        /// </summary>
        /// <param name="weeklySpecialView"></param>
        /// <returns></returns>
        public static WeeklySpecialOperationResult CreateWeeklySpecial(WeeklySpecialView weeklySpecialView)
        {
            if (weeklySpecialView == null)
                throw new ArgumentNullException();

            WeeklySpecialOperationResult result = IsValidWeeklySpecial(weeklySpecialView);

            if (result == WeeklySpecialOperationResult.Ok)
            {
                if (IsThereAnyCurrentWeeklySpecial())
                {
                    result = WeeklySpecialOperationResult.ExistingWeeklySpecial;
                }
                else
                {
                    weeklySpecialView = SanitizeWeeklySpecial(weeklySpecialView);

                    using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                    {
                        WeeklySpecial weeklySpecial = new WeeklySpecial();
                        weeklySpecial.EndDate = null;
                        weeklySpecial.ImageUrl = weeklySpecialView.ImageUrl;
                        weeklySpecial.ItemId = weeklySpecialView.ItemId;
                        weeklySpecial.PopupText = weeklySpecialView.Text;
                        weeklySpecial.PopupTitle = weeklySpecialView.Title;
                        weeklySpecial.StartDate = DateTime.Now;

                        uberStrikeDb.WeeklySpecials.InsertOnSubmit(weeklySpecial);
                        uberStrikeDb.SubmitChanges();
                    }
                }
            }

            return result;
        }

        public static bool IsThereAnyCurrentWeeklySpecial()
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool isThereAnyCurrent = false;

                int weeklySpecialId = (from w in uberStrikeDb.WeeklySpecials
                                      where w.EndDate == null
                                      select w.Id).SingleOrDefault();

                if (weeklySpecialId != 0)
                {
                    isThereAnyCurrent = true;
                }

                return isThereAnyCurrent;
            }
        }

        public static WeeklySpecialView GetCurrentWeeklySpecial()
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                WeeklySpecial weeklySpecial = uberStrikeDb.WeeklySpecials.SingleOrDefault(q => q.EndDate == null);
                WeeklySpecialView weeklySpecialView = ToWeeklySpecialView(weeklySpecial);

                return weeklySpecialView;
            }
        }

        private static WeeklySpecialView ToWeeklySpecialView(WeeklySpecial weeklySpecial)
        {
            WeeklySpecialView weeklySpecialView = null;

            if (weeklySpecial != null)
            {
                weeklySpecialView = new WeeklySpecialView(weeklySpecial.PopupTitle,
                                                            weeklySpecial.PopupText,
                                                            weeklySpecial.ImageUrl,
                                                            weeklySpecial.ItemId,
                                                            weeklySpecial.Id,
                                                            weeklySpecial.StartDate,
                                                            weeklySpecial.EndDate);
            }

            return weeklySpecialView;
        }

        private static List<WeeklySpecialView> ToWeeklySpecialView(List<WeeklySpecial> weeklySpecial)
        {
            List<WeeklySpecialView> weeklySpecialViews = weeklySpecial.ConvertAll(new Converter<WeeklySpecial, WeeklySpecialView>(q => ToWeeklySpecialView(q)));

            return weeklySpecialViews;
        }

        /// <summary>
        /// Start Date and End Date are not editable
        /// </summary>
        /// <param name="weeklySpecialView"></param>
        public static WeeklySpecialOperationResult EditWeeklySpecial(WeeklySpecialView weeklySpecialView)
        {
            if (weeklySpecialView == null)
                throw new ArgumentNullException();

            WeeklySpecialOperationResult result = IsValidWeeklySpecial(weeklySpecialView);

            if (result == WeeklySpecialOperationResult.Ok)
            {
                using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
                {
                    WeeklySpecial weeklySpecial = GetWeeklySpecial(weeklySpecialView.Id, uberStrikeDb);

                    if (weeklySpecial == null)
                    {
                        result = WeeklySpecialOperationResult.NonExistingWeeklySpecial;
                    }
                    else
                    {
                        weeklySpecialView = SanitizeWeeklySpecial(weeklySpecialView);

                        weeklySpecial.ImageUrl = weeklySpecialView.ImageUrl;
                        weeklySpecial.ItemId = weeklySpecialView.ItemId;
                        weeklySpecial.PopupText = weeklySpecialView.Text;
                        weeklySpecial.PopupTitle = weeklySpecialView.Title;

                        uberStrikeDb.SubmitChanges();
                    }
                }
            }

            return result;
        }

        public static WeeklySpecialView GetWeeklySpecial(int id)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                WeeklySpecial weeklySpecial = GetWeeklySpecial(id, uberStrikeDb);

                return ToWeeklySpecialView(weeklySpecial);
            }
        }

        private static WeeklySpecial GetWeeklySpecial(int id, UberstrikeDataContext uberStrikeDb)
        {
            if (uberStrikeDb == null)
                throw new ArgumentNullException();

            WeeklySpecial weeklySpecial = uberStrikeDb.WeeklySpecials.SingleOrDefault(q => q.Id == id);

            return weeklySpecial;
        }

        public static bool EndWeeklySpecial(int id)
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                bool isEnded = false;

                WeeklySpecial weeklySpecial = GetWeeklySpecial(id, uberStrikeDb);

                if (weeklySpecial != null)
                {
                    weeklySpecial.EndDate = DateTime.Now;

                    uberStrikeDb.SubmitChanges();
                    isEnded = true;
                }

                return isEnded;
            }
        }

        private static WeeklySpecialOperationResult IsValidWeeklySpecial(WeeklySpecialView weeklySpecial)
        {
            if (weeklySpecial == null)
                throw new ArgumentNullException();

            WeeklySpecialOperationResult result = WeeklySpecialOperationResult.Error;

            if (String.IsNullOrWhiteSpace(weeklySpecial.Title))
                result = WeeklySpecialOperationResult.InvalidTitle;
            else if (weeklySpecial.Title.Length > MaxTitleLength)
                result = WeeklySpecialOperationResult.InvalidTitle;
            else if (String.IsNullOrWhiteSpace(weeklySpecial.Text))
                result = WeeklySpecialOperationResult.InvalidText;
            else if (weeklySpecial.Text.Length > MaxTextLength)
                result = WeeklySpecialOperationResult.InvalidText;
            else if (String.IsNullOrWhiteSpace(weeklySpecial.ImageUrl))
                result = WeeklySpecialOperationResult.InvalidImageUrl;
            else if (weeklySpecial.ImageUrl.Length > MaxImageUrlLength)
                result = WeeklySpecialOperationResult.InvalidImageUrl;
            else if (weeklySpecial.ItemId == 0)
                result = WeeklySpecialOperationResult.InvalidItemId;
            else if (!CmuneItem.IsItemOnSale(weeklySpecial.ItemId))
                result = WeeklySpecialOperationResult.InvalidItemId;
            else
                result = WeeklySpecialOperationResult.Ok;

            return result;
        }

        private static WeeklySpecialView SanitizeWeeklySpecial(WeeklySpecialView weeklySpecial)
        {
            if (weeklySpecial != null)
            {
                weeklySpecial.ImageUrl = weeklySpecial.ImageUrl.Trim();
                weeklySpecial.Text = weeklySpecial.Text.Trim();
                weeklySpecial.Title = weeklySpecial.Title.Trim();
            }

            return weeklySpecial;
        }

        public static List<WeeklySpecialView> GetRecentWeeklySpecials()
        {
            using (UberstrikeDataContext uberStrikeDb = new UberstrikeDataContext())
            {
                List<WeeklySpecial> weeklySpecials = uberStrikeDb.WeeklySpecials.OrderByDescending(w => w.Id).Take(RecentCount).ToList();

                return ToWeeklySpecialView(weeklySpecials);
            }
        }
    }
}

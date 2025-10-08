using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using Cmune.Channels.Instrumentation.Models;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class PaginationHelper
    {
        const int DefaultDisplayedPages = 9;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="paginationModel"></param>
        /// <param name="onclick"></param>
        /// <param name="displayedPages">Works better with an odd number greater than 3</param>
        /// <param name="displayFirstLast"></param>
        /// <param name="displayPreviousNext"></param>
        /// <returns></returns>
        public static MvcHtmlString GeneratePagination(this HtmlHelper htmlHelper, PaginationModel paginationModel, string onclick, int displayedPages = DefaultDisplayedPages, bool displayFirstLast = true, bool displayPreviousNext = true)
        {
            StringBuilder pagination = new StringBuilder();

            if (displayedPages < 3)
            {
                displayedPages = 3;
            }

            int pageCount = (int) Math.Ceiling(((decimal)paginationModel.TotalCount / (decimal)paginationModel.PageSize));
            int pageVariance = (int) Math.Floor((decimal) displayedPages / 2m);

            if (pageCount > 1)
            {
                pagination.Append("<div class=\"pagination\">");
                pagination.Append("<input type=\"hidden\" name=\"selectedPage" + paginationModel.Name + "\" id=\"selectedPage" + paginationModel.Name + "\" value=" + paginationModel.PageIndex + " />");

                // Compute start and end page index

                int startIndex = 0;
                int endIndex = 0;

                if (paginationModel.PageIndex - pageVariance > 1 && paginationModel.PageIndex + pageVariance < pageCount)
                {
                    startIndex = paginationModel.PageIndex - pageVariance;
                    endIndex = paginationModel.PageIndex + pageVariance;
                }
                else if (paginationModel.PageIndex - pageVariance <= 1)
                {
                    startIndex = 1;

                    if (paginationModel.PageIndex + (displayedPages - 1) > pageCount)
                    {
                        endIndex = pageCount;
                    }
                    else
                    {
                        endIndex = startIndex + (displayedPages - 1);
                    }
                }
                else if (paginationModel.PageIndex + pageVariance >= pageCount)
                {
                    endIndex = pageCount;

                    if (paginationModel.PageIndex - (displayedPages - 1) < 1)
                    {
                        startIndex = 1;
                    }
                    else
                    {
                        startIndex = endIndex - (displayedPages - 1);
                    }
                }

                // First - Previous links

                if (paginationModel.PageIndex == 1)
                {
                    if (displayPreviousNext)
                    {
                        pagination.Append("<span class=\"pageAnchor\">Prev</span>");
                    }
                }
                else
                {
                    if (displayPreviousNext)
                    {
                        pagination.Append("<a href=\"#\" class=\"pageAnchor\" onclick=\"$(\'#selectedPage" + paginationModel.Name + "\').val(" + (paginationModel.PageIndex - 1) + ");" + onclick + "; return false;\">Prev</a>");
                    }

                    if (displayFirstLast && startIndex > 1)
                    {
                        pagination.Append("<a href=\"#\" class=\"pageAnchor\" onclick=\"$(\'#selectedPage" + paginationModel.Name + "\').val(1);" + onclick + "; return false;\">1</a>");

                        if (startIndex > 2)
                        {
                            pagination.Append("<span class=\"pageAnchor\">...</span>");
                        }
                    }   
                }

                // Pagination

                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (paginationModel.PageIndex == i)
                    {
                        pagination.Append("<span class=\"pageAnchor selectedPageAnchor\">" + i + "</span>");
                    }
                    else
                    {
                        pagination.Append("<a href=\"#\" class=\"pageAnchor\" onclick=\"$(\'#selectedPage" + paginationModel.Name + "\').val(" + i + ");" + onclick + "; return false;\">" + i + "</a>");
                    }
                }

                // Next - Last links

                if (paginationModel.PageIndex == pageCount)
                {
                    if (displayPreviousNext)
                    {
                        pagination.Append("<span class=\"pageAnchor\">Next</span>");
                    }
                }
                else
                {
                    if (displayFirstLast && endIndex < pageCount)
                    {
                        if (endIndex < pageCount - 1)
                        {
                            pagination.Append("<span class=\"pageAnchor\">...</span>");
                        }
                        
                        pagination.Append("<a href=\"#\" class=\"pageAnchor\" onclick=\"$(\'#selectedPage" + paginationModel.Name + "\').val(" + pageCount + ");" + onclick + "; return false;\">" + pageCount + "</a>");
                    }

                    if (displayPreviousNext)
                    {
                        pagination.Append("<a href=\"#\" class=\"pageAnchor\" onclick=\"$(\'#selectedPage" + paginationModel.Name + "\').val(" + (paginationModel.PageIndex + 1) + ");" + onclick + "; return false;\">Next</a>");
                    }
                }

                pagination.Append("</div>");
            }

            return new MvcHtmlString(pagination.ToString());
        }
    }
}
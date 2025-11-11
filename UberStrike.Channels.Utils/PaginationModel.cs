using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UberStrike.Channels.Utils
{
    public class PaginationModel
    {
        #region Properties

        /// <summary>
        /// Starts a 1
        /// </summary>
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// Index in the source list
        /// </summary>
        private int LastElementIndex
        {
            get
            {
                if (TotalCount > 0)
                {
                    return TotalCount - 1;
                }

                return 0;
            }
        }

        public int TotalCount { get; set; }
        const int DefaultPageSize = 30;
        /// <summary>
        /// Allow to have more than one AJAX pagination per page
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default name and default page size
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex">Starting at 1</param>
        public PaginationModel(int totalCount, int pageIndex)
        {
            PageIndex = pageIndex;
            TotalCount = totalCount;
            PageSize = DefaultPageSize;
        }

        /// <summary>
        /// Default page size and sets a name
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex">Starting at 1</param>
        /// <param name="name"></param>
        public PaginationModel(int totalCount, int pageIndex, string name)
            : this(totalCount, pageIndex)
        {
            Name = name;
        }

        /// <summary>
        /// Sets the page size and sets a name
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex">Starting at 1</param>
        /// <param name="name"></param>
        /// <param name="pageSize"></param>
        public PaginationModel(int totalCount, int pageIndex, string name, int pageSize)
            : this(totalCount, pageIndex)
        {
            PageSize = pageSize;
            Name = name;
        }

        #endregion

        #region Methods

        private int GetPageStartElementIndex()
        {
            int startIndex = 0;

            // If element index of this page is bigger than total element then we set the page index to 1
            if (((PageIndex - 1) * PageSize) > LastElementIndex)
            {
                PageIndex = 1;
            }

            startIndex = ((PageIndex - 1) * PageSize);

            return startIndex;
        }

        private int GetPageElementCount(int startIndex)
        {
            if (startIndex + PageSize > LastElementIndex)
            {
                return TotalCount - startIndex;
            }

            return PageSize;
        }

        public List<T> GetCurrentPage<T>(List<T> source)
        {
            int startIndex = GetPageStartElementIndex();
            int elementCount = GetPageElementCount(startIndex);

            return source.GetRange(startIndex, elementCount);
        }

        #endregion
    }
}
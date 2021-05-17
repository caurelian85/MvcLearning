using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcLearningApp.Models
{
    public class PagerView
    {
        public int PagerLength = 5;
        public int PagerPages { get; set; }
        public int StartPage { get; set; }
        public int CurrentPage { get; set; }
        public int Pages { get; set; }
        public int Records { get; set; }
        public int CurrentRecords { get; set; }
        public int RecordsPerPage { get; set; }
        public int Colspan { get; set; }

        public int Init(int totalRecords, int pageSize, int pagerLength, int? page)
        {
            int reqPageNumber = page.HasValue ? page.Value : 1;
            int pages = (int)Math.Ceiling(totalRecords / (decimal)pageSize);
            if (pages < reqPageNumber || reqPageNumber == -1)
                reqPageNumber = pages;
            int startPage = reqPageNumber - pagerLength / 2;
            if (startPage < 1 || pagerLength > pages)
                startPage = 1;
            else if (reqPageNumber > pages - pagerLength / 2)
                startPage = pages - pagerLength + 1;

            //fill the model with pager info
            this.PagerPages = pagerLength < pages ? pagerLength : pages;
            this.StartPage = startPage;
            this.CurrentPage = reqPageNumber;
            this.Pages = pages;
            this.Records = totalRecords;
            this.RecordsPerPage = pageSize;

            return reqPageNumber;
        }
    }

    public class GridViewModel<T>
    {
        public PagerView Pager { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
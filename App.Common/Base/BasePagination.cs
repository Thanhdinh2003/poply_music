namespace App.Common.Base
{
    public class BasePagination<T>
    {
        public BasePagination(long totalItems, long page, long pageSize, IEnumerable<T> items)
        {
            TotalPage = (totalItems + pageSize - 1) / pageSize;
            TotalItems = totalItems;
            Page = page;
            PageSize = pageSize;
            Items = items;
        }

        public long TotalPage { get; set; }
        public long TotalItems { get; set; }
        public long Page { get; set; }
        public long PageSize { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}

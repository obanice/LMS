using X.PagedList;

namespace Core.ViewModels
{
    public class IPageListModel<T> : IIPageListModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Keyword { get; set; }
        public string? SearchAction { get; set; }
        public string? SearchController { get; set; }
        public IPagedList<T> Model { get; set; }
        public bool CanFilterByDateRange { get; set; }
        /// <summary>
        /// The page to be sent
        /// </summary>
        public int RequestedPage { get; set; }
        /// <summary>
        /// The number of items to be included in the return collection
        /// </summary>
        public int PageSize { get; set; }
        public IPageListModel()
        {
            CanFilterByDateRange = true;
            PageSize = 25;
        }
    }
    public interface IIPageListModel
    {
        DateTime? EndDate { get; set; }
        string? Keyword { get; set; }
        //int PageSize { get; set; }
        int RequestedPage { get; set; }
        string? SearchAction { get; set; }
        string? SearchController { get; set; }
        DateTime? StartDate { get; set; }
        bool CanFilterByDateRange { get; set; }
    }
}

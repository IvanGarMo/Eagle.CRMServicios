namespace CRM_SER_EWS.CRM.Helpers
{
    public interface IQuery
    {
        int Page { get; set; }
        int PageSize { get; set; }
        string SortBy { get; set; }
        bool IsSortAscending { get; set; }
    }
}

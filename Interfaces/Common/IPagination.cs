namespace OctopusAPI.Interfaces.Common
{
    public interface IPaginationOutput<TObject>
    {
        public IReadOnlyCollection<TObject> Objects { get; set; }
        public int Count { get; set; }
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
    }

    public interface IPaginationInput<IFilter>
    {
        public IFilter FilterOptions { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
    }
}
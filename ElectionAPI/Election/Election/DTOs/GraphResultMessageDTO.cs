using Election.Entity;

namespace Election.DTOs
{
    public class GraphResultMessageDTO
    {
        public List<Voter> SimpleSampleData { get; set; }
        public List<Voter> SystematicSampleData { get; set; }
        public List<Voter> StratifiedSampleData { get; set; }
        public string CityName { get; set; }
    }
}

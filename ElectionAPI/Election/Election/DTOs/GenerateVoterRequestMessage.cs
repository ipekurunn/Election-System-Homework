namespace Election.DTOs
{
    public class GenerateVoterRequestMessage
    {
        public int Population { get; set; }
        public int SampleSize { get; set; }
        public int SampleInterval { get; set; }
        public int SampleSizePerStratum { get; set; }
        public string StratumType { get; set; }
    }
}

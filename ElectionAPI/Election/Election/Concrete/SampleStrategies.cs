using Election.Abstractions;
using Election.Entity;

namespace Election.Concrete
{
    public class SimpleRandomSampleStrategy : ISamplingStrategy
    {
        public IEnumerable<Voter> Sample(IEnumerable<Voter> population, int sampleSize)
        {
            var random = new Random();
            return population.OrderBy(x => random.Next()).Take(sampleSize);
        }
    }

    public class SystematicSampleStrategy : ISamplingStrategy
    {
        public IEnumerable<Voter> Sample(IEnumerable<Voter> population, int sampleInterval)
        {
            return population.Where((voter, index) => index % sampleInterval == 0);
        }
    }

    public class StratifiedSampleStrategy : ISamplingStrategy
    {
        private readonly Func<Voter, object> _stratifier;

        public StratifiedSampleStrategy(Func<Voter, object> stratifier)
        {
            _stratifier = stratifier;
        }

        public IEnumerable<Voter> Sample(IEnumerable<Voter> population, int sampleSizePerStratum)
        {
            var stratified = population.GroupBy(_stratifier);
            List<Voter> sample = new List<Voter>();

            foreach (var stratum in stratified)
            {
                var stratumSample = stratum.OrderBy(x => Guid.NewGuid()).Take(sampleSizePerStratum);
                sample.AddRange(stratumSample);
            }

            return sample;
        }
    }

}

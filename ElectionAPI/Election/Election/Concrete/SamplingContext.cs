using Election.Abstractions;
using Election.Entity;

namespace Election.Concrete
{
    public class SamplingContext
    {
        private ISamplingStrategy _strategy;

        public void SetStrategy(ISamplingStrategy strategy)
        {
            _strategy = strategy;
        }

        public IEnumerable<Voter> ExecuteStrategy(IEnumerable<Voter> population, int parameter)
        {
            if (_strategy == null) throw new InvalidOperationException("Sampling strategy is not set.");
            return _strategy.Sample(population, parameter);
        }
    }

}

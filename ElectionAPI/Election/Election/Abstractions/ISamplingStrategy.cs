using Election.Entity;

namespace Election.Abstractions
{
    public interface ISamplingStrategy
    {
        IEnumerable<Voter> Sample(IEnumerable<Voter> population, int parameter);
    }
}

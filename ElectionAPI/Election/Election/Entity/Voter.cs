namespace Election.Entity
{
    public class Voter
    {
        public string Name { get; init; }
        public int Age { get; init; }
        public string City { get; init; }
        public string Vote { get; init; }
        public Gender VoterGender { get; init; }

        public Voter(string name, int age, string city, string vote, Gender voterGender)
        {
            Name = name;
            Age = age;
            City = city;
            Vote = vote;
            VoterGender = voterGender;
        }
    }
    public enum Gender
    {
        Male,
        Female
    }
}

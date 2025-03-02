namespace Political
{
    /// <summary>
    /// Represents a legislator (House or Senate member) with personal traits, trust relationships, and voting behavior.
    /// </summary>
    public class Legislator
    {
        /// <summary>
        /// The legislator's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The legislative chamber (House or Senate) the legislator belongs to.
        /// </summary>
        public string Chamber { get; }

        /// <summary>
        /// The political party the legislator belongs to.
        /// </summary>
        public string Party { get; }

        /// <summary>
        /// Loyalty to the legislator's party. Ranges from 0.5 to 1.0, representing moderate to extreme loyalty.
        /// </summary>
        public double PartyLoyalty { get; }

        /// <summary>
        /// Support for the current bill, randomly assigned for each bill (0.0 to 1.0).
        /// </summary>
        public double IdeologicalSupport { get; private set; }

        /// <summary>
        /// Concern for re-election. Higher values indicate the legislator is more risk-averse.
        /// </summary>
        public double ReelectionConcern { get; }

        /// <summary>
        /// Reputation for reliability. Starts at 0.5 (neutral) and changes based on behavior (0.0 to 1.0).
        /// </summary>
        public double Reputation { get; private set; } = 0.5;

        /// <summary>
        /// Voter approval rating. Starts at 0.5 and fluctuates based on voting behavior (0.0 to 1.0).
        /// </summary>
        public double VoterApproval { get; private set; } = 0.5;

        /// <summary>
        /// Whether the legislator is influenced by special interest groups.
        /// </summary>
        public bool CapturedBySpecialInterest { get; private set; }

        /// <summary>
        /// Trust levels with other legislators. Ranges from 0.0 (total distrust) to 1.0 (absolute trust).
        /// </summary>
        public Dictionary<string, double> TrustLevels { get; } = new();

        /// <summary>
        /// Temporary storage of lobbying pressure applied during the bill's preparation phase.
        /// </summary>
        private double LobbyPressure { get; set; } = 0;

        private static readonly Random random = new();

        /// <summary>
        /// Initializes a new legislator with randomized traits.
        /// </summary>
        public Legislator(string name, string chamber, string party)
        {
            Name = name;
            Chamber = chamber;
            Party = party;
            PartyLoyalty = RandomDouble(0.5, 1.0);   // Moderate to strong loyalty
            ReelectionConcern = RandomDouble(0, 1.0);
            CapturedBySpecialInterest = random.NextDouble() < 0.1;   // 10% chance of capture
        }

        /// <summary>
        /// Sets the lobbying pressure that will influence this legislator's vote.
        /// </summary>
        public void ReceiveLobbyPressure(double pressure) => LobbyPressure = pressure;

        /// <summary>
        /// Retrieves the stored lobbying pressure.
        /// </summary>
        public double GetLobbyPressure() => LobbyPressure;

        /// <summary>
        /// Initializes neutral trust relationships with all other legislators.
        /// </summary>
        public void InitializeTrust(List<Legislator> allLegislators)
        {
            foreach (var other in allLegislators.Where(l => l != this))
            {
                TrustLevels[other.Name] = 0.5;   // Neutral trust at start
            }
        }

        /// <summary>
        /// Randomizes ideological support for the next bill (0.0 to 1.0).
        /// </summary>
        public void SetIdeologicalSupport() => IdeologicalSupport = RandomDouble(0, 1.0);

        /// <summary>
        /// Updates voter approval based on whether the legislator voted with their party.
        /// </summary>
        public void UpdateVoterApproval(bool votedWithParty)
        {
            VoterApproval += votedWithParty ? 0.01 : -0.01;   // Small nudge per vote
            VoterApproval = Math.Clamp(VoterApproval, 0, 1);  // Keep within valid range
        }

        /// <summary>
        /// Determines how the legislator votes on a bill, factoring in party loyalty, personal belief, re-election concern,
        /// trust relationships, lobbying pressure, and special interest influence.
        /// </summary>
        public bool VoteOnBill(string billName, Dictionary<string, double> partyStances, List<Legislator> allLegislators, LobbyGroup lobbyGroup)
        {
            double partyStance = partyStances.GetValueOrDefault(Party, 0.5);
            double partyPressure = PartyLoyalty * partyStance;

            double personalBelief = IdeologicalSupport;
            double selfPreservation = ReelectionConcern * (1 - VoterApproval);

            double trustBonus = 0;
            double trustPenalty = 0;

            foreach (var other in allLegislators)
            {
                if (other == this) continue;

                double trust = TrustLevels[other.Name] * other.Reputation;

                // Trust thresholds: >0.7 means trusted ally, <0.3 means distrusted rival
                if (trust > 0.7 && other.VotedYesLastRound) trustBonus += 0.03 * trust;    // 3% influence from trusted allies
                if (trust < 0.3 && other.VotedYesLastRound) trustPenalty += 0.03 * (1 - trust);  // 3% penalty from distrusted rivals
            }

            double specialInterestBonus = CapturedBySpecialInterest ? 0.05 : 0.0;   // 5% influence if captured
            double lobbyPressure = GetLobbyPressure();

            // Weighted scoring: Party, personal belief, self-preservation, trust, lobbying, and special interests
            double finalScore = (partyPressure * 0.3) +    // 30% party pressure
                                (personalBelief * 0.25) +   // 25% personal belief
                                (selfPreservation * 0.15) + // 15% self-preservation
                                trustBonus - trustPenalty + 
                                specialInterestBonus + 
                                lobbyPressure;

            bool voteYes = finalScore >= 0.5;  // 0.5 threshold = neutral vote

            VotedYesLastRound = voteYes;
            UpdateVoterApproval(voteYes == (partyStance >= 0.5));

            return voteYes;
        }

        /// <summary>
        /// Forms a deal with another legislator, boosting mutual trust and reputation slightly.
        /// </summary>
        public void MakeDealWith(Legislator other, string billName)
        {
            TrustLevels[other.Name] = Math.Min(1.0, TrustLevels[other.Name] + 0.1);  // 10% trust gain
            other.TrustLevels[Name] = Math.Min(1.0, other.TrustLevels[Name] + 0.1);
            Reputation += 0.01;   // Small reputation boost for keeping deals
        }

        /// <summary>
        /// Betrays another legislator, wiping out mutual trust and reducing reputation.
        /// </summary>
        public void Betray(Legislator other)
        {
            TrustLevels[other.Name] = 0;
            other.TrustLevels[Name] = 0;
            Reputation -= 0.05;   // 5% reputation loss for betrayal
        }

        /// <summary>
        /// Generates a random double between min and max.
        /// </summary>
        private static double RandomDouble(double min, double max) => random.NextDouble() * (max - min) + min;

        /// <summary>
        /// Tracks how the legislator voted on the previous bill.
        /// </summary>
        public bool VotedYesLastRound { get; private set; }
    }
}
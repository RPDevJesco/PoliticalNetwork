namespace Political
{
    /// <summary>
    /// Represents the tally of votes for a bill, including total counts and party-specific results.
    /// </summary>
    class VoteTally
    {
        /// <summary>
        /// Total number of "YES" votes across all legislators.
        /// </summary>
        public int TotalYes { get; set; }

        /// <summary>
        /// Total number of "NO" votes across all legislators.
        /// </summary>
        public int TotalNo { get; set; }

        /// <summary>
        /// Tracks the number of "YES" and "NO" votes per party.
        /// </summary>
        public Dictionary<string, (int Yes, int No)> PartyResults { get; } = new();

        /// <summary>
        /// Records a vote for a given party and updates both party-specific and total counts.
        /// </summary>
        /// <param name="party">The party the legislator belongs to (e.g., Democrat, Republican, Independent).</param>
        /// <param name="votedYes">True if the legislator voted "YES", false if they voted "NO".</param>
        public void RecordVote(string party, bool votedYes)
        {
            // Initialize party vote counts if this is the first vote recorded for the party.
            if (!PartyResults.ContainsKey(party))
            {
                PartyResults[party] = (0, 0);
            }

            // Update party's specific "YES" or "NO" count and the overall counts.
            if (votedYes)
            {
                PartyResults[party] = (PartyResults[party].Yes + 1, PartyResults[party].No);
                TotalYes++;
            }
            else
            {
                PartyResults[party] = (PartyResults[party].Yes, PartyResults[party].No + 1);
                TotalNo++;
            }
        }
    }
}
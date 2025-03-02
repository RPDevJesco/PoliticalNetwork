namespace Political
{
    /// <summary>
    /// Represents a lobby group that exerts influence on legislators based on their party affiliation.
    /// </summary>
    public class LobbyGroup
    {
        /// <summary>
        /// Represents the amount of lobbying pressure applied to each party.
        /// Positive values indicate lobbying in favor of a bill, 
        /// and negative values indicate lobbying against a bill.
        /// </summary>
        /// <remarks>
        /// <para><b>Magic Number Explanation:</b></para>
        /// <para>Democrats receive +0.05 pressure, meaning they are slightly pushed to support bills favored by lobbyists.</para>
        /// <para>Republicans receive -0.05 pressure, meaning they are slightly pushed to oppose bills favored by lobbyists.</para>
        /// <para>Independents receive +0.02 pressure, representing minor influence since they are often less tied to specific lobbyist agendas.</para>
        /// <para>These numbers are adjustable if simulating different scenarios (e.g., stronger lobby influence for certain parties).</para>
        /// </remarks>
        private readonly Dictionary<string, double> partyPressures = new()
        {
            { "Democrat", 0.05 },      // 5% push toward YES
            { "Republican", -0.05 },   // 5% push toward NO
            { "Independent", 0.02 }    // 2% push toward YES (more balanced pressure)
        };

        /// <summary>
        /// Retrieves the lobbying pressure applied to a given legislator based on their party.
        /// </summary>
        /// <param name="legislator">The legislator for whom lobbying pressure is being retrieved.</param>
        /// <returns>
        /// A positive or negative value representing how much the lobby group influences the legislator's vote.
        /// </returns>
        public double GetPressureForLegislator(Legislator legislator)
        {
            return partyPressures.GetValueOrDefault(legislator.Party, 0); // Default to neutral if party is unrecognized
        }
    }
}
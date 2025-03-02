namespace Political
{
    class Program
    {
        /// <summary>
        /// Defines the initial party distribution in the House of Representatives.
        /// </summary>
        static readonly Dictionary<string, int> HouseMembers = new()
        {
            { "Democrat", 213 },
            { "Republican", 220 },
            { "Independent", 2 }
        };

        /// <summary>
        /// Defines the initial party distribution in the Senate.
        /// </summary>
        static readonly Dictionary<string, int> SenateMembers = new()
        {
            { "Democrat", 48 },
            { "Republican", 50 },
            { "Independent", 2 }
        };

        static readonly List<Legislator> House = new();
        static readonly List<Legislator> Senate = new();

        /// <summary>
        /// Main entry point. Initializes legislators, sets up trust levels, and runs a series of bill votes.
        /// </summary>
        static void Main()
        {
            InitializeChamber(House, "House", HouseMembers);
            InitializeChamber(Senate, "Senate", SenateMembers);

            var allLegislators = House.Concat(Senate).ToList();
            foreach (var legislator in allLegislators)
            {
                legislator.InitializeTrust(allLegislators);
            }

            ShowFinalLegislatorSummary();

            SimulateBillVote("Healthcare Reform Act", new Dictionary<string, double>
            {
                { "Democrat", 0.85 },      // Democrats strongly favor
                { "Republican", 0.25 },    // Republicans generally oppose
                { "Independent", 0.50 }    // Independents neutral
            });

            SimulateBillVote("Tax Cut Bill", new Dictionary<string, double>
            {
                { "Democrat", 0.30 },      // Democrats generally oppose
                { "Republican", 0.80 },    // Republicans strongly favor
                { "Independent", 0.50 }    // Independents neutral
            });

            SimulateBillVote("Defense Spending Increase", new Dictionary<string, double>
            {
                { "Democrat", 0.50 },      // Democrats split
                { "Republican", 0.90 },    // Republicans strongly favor
                { "Independent", 0.60 }    // Independents lean yes
            });
        }

        /// <summary>
        /// Initializes a legislative chamber (House or Senate) with legislators from different parties.
        /// </summary>
        static void InitializeChamber(List<Legislator> chamber, string chamberName, Dictionary<string, int> partyCounts)
        {
            foreach (var (party, count) in partyCounts)
            {
                for (int i = 1; i <= count; i++)
                {
                    chamber.Add(new Legislator($"{party} {chamberName} Member {i}", chamberName, party));
                }
            }
        }

        /// <summary>
        /// Simulates the full lifecycle of a bill, including pre-vote lobbying, deals, betrayals, and final voting.
        /// </summary>
        static void SimulateBillVote(string billName, Dictionary<string, double> partyStances)
        {
            Console.WriteLine($"\n==== Preparing for Vote on {billName} ====");

            var lobbyGroup = new LobbyGroup();

            // Pre-vote phase: apply ideology, lobbying pressure, and deals/betrayals.
            PrepareLegislatorsForVote(billName, lobbyGroup);

            // Voting phase: collect and display results.
            var houseResults = TallyVotes(House, billName, partyStances, lobbyGroup);
            var senateResults = TallyVotes(Senate, billName, partyStances, lobbyGroup);

            Console.WriteLine($"\n--- Vote Results ---");
            Console.WriteLine($"House: YES: {houseResults.TotalYes} / NO: {houseResults.TotalNo}");
            ShowChamberBreakdown("House", houseResults.PartyResults);

            Console.WriteLine($"Senate: YES: {senateResults.TotalYes} / NO: {senateResults.TotalNo}");
            ShowChamberBreakdown("Senate", senateResults.PartyResults);

            Console.WriteLine(houseResults.TotalYes > houseResults.TotalNo && senateResults.TotalYes > senateResults.TotalNo
                ? $"{billName} PASSES Congress!"
                : $"{billName} FAILS to pass.");
        }

        /// <summary>
        /// Prepares all legislators for a vote by setting ideological support, applying lobbying pressure,
        /// and simulating pre-vote deals and betrayals.
        /// </summary>
        static void PrepareLegislatorsForVote(string billName, LobbyGroup lobbyGroup)
        {
            House.ForEach(l => l.SetIdeologicalSupport());
            Senate.ForEach(l => l.SetIdeologicalSupport());

            var allLegislators = House.Concat(Senate).ToList();

            foreach (var legislator in allLegislators)
            {
                double lobbyPressure = lobbyGroup.GetPressureForLegislator(legislator);
                legislator.ReceiveLobbyPressure(lobbyPressure);
            }

            ProcessDealsAndBetrayals(allLegislators, billName);
        }

        /// <summary>
        /// Simulates pre-vote deals and betrayals between legislators, which affect future trust and reputation.
        /// </summary>
        static void ProcessDealsAndBetrayals(List<Legislator> allLegislators, string billName)
        {
            var random = new Random();

            foreach (var legislator in allLegislators)
            {
                if (random.NextDouble() < 0.1)  // 10% chance to make a deal
                {
                    var dealPartner = allLegislators[random.Next(allLegislators.Count)];
                    if (dealPartner != legislator) legislator.MakeDealWith(dealPartner, billName);
                }

                if (random.NextDouble() < 0.05)  // 5% chance to betray someone
                {
                    var betrayalTarget = allLegislators[random.Next(allLegislators.Count)];
                    if (betrayalTarget != legislator) legislator.Betray(betrayalTarget);
                }
            }
        }

        /// <summary>
        /// Tallies the votes for a specific chamber on a bill.
        /// </summary>
        static VoteTally TallyVotes(List<Legislator> chamber, string billName, Dictionary<string, double> partyStances, LobbyGroup lobbyGroup)
        {
            var allLegislators = House.Concat(Senate).ToList();
            var tally = new VoteTally();

            foreach (var member in chamber)
            {
                if (member.VoteOnBill(billName, partyStances, allLegislators, lobbyGroup))
                    tally.RecordVote(member.Party, true);
                else
                    tally.RecordVote(member.Party, false);
            }

            return tally;
        }

        /// <summary>
        /// Displays a summary of votes by party for a given chamber.
        /// </summary>
        static void ShowChamberBreakdown(string chamber, Dictionary<string, (int Yes, int No)> partyResults)
        {
            Console.WriteLine($"{chamber} Breakdown: " +
                string.Join(" | ", partyResults.Select(pr => $"{pr.Key} YES: {pr.Value.Yes}, NO: {pr.Value.No}")));
        }

        /// <summary>
        /// Displays a final summary of each legislator's reputation, alliances, and voter approval.
        /// </summary>
        static void ShowFinalLegislatorSummary()
        {
            Console.WriteLine("\n==== Final Legislator Summary ====");

            var allLegislators = House.Concat(Senate).ToList();

            foreach (var legislator in allLegislators)
            {
                Console.WriteLine($"{legislator.Chamber} - {legislator.Name} ({legislator.Party})");
                Console.WriteLine($"   Party Loyalty: {legislator.PartyLoyalty:F2}");
                Console.WriteLine($"   Final Reputation: {legislator.Reputation:F2}");
                Console.WriteLine($"   Voter Approval: {legislator.VoterApproval:P1}");
                Console.WriteLine($"   Captured by Special Interest: {(legislator.CapturedBySpecialInterest ? "Yes" : "No")}");

                var topTrusted = legislator.TrustLevels.OrderByDescending(t => t.Value).Take(3)
                    .Select(t => $"{t.Key} ({t.Value:F2})").ToList();

                var leastTrusted = legislator.TrustLevels.OrderBy(t => t.Value).Take(3)
                    .Select(t => $"{t.Key} ({t.Value:F2})").ToList();

                Console.WriteLine($"   Top Trusted Allies: {string.Join(", ", topTrusted)}");
                Console.WriteLine($"   Least Trusted: {string.Join(", ", leastTrusted)}\n");
            }
        }
    }
}
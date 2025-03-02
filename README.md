# Political Neural Network Simulation

## Overview

This project implements a **politically-themed simulation** that uses concepts inspired by **neural networks** but adapted to model the behavior of legislators. Instead of neurons processing signals mathematically, **legislators negotiate, build alliances, betray each other, and respond to external pressures (like lobbyists)**. Their final vote represents the "output" after this web of influences.

This project covers:

- House and Senate members, randomly generated for each party.
- Each legislator has:
    - **Party Loyalty** — how strongly they follow the party line.
    - **Reelection Concern** — how much they worry about keeping their seat.
    - **Ideological Support** — how strongly they personally feel about a bill.
    - **Trust Levels** — relationships with other legislators, which change based on deals and betrayals.
    - **Reputation & Voter Approval** — evolving metrics based on behavior.
    - **Special Interest Capture** — 10% of legislators start with lobbyist control.
- Bills go through:
    1. **Pre-vote Phase**: Ideology set, lobbying applied, deals and betrayals processed.
    2. **Voting Phase**: Legislators weigh all factors and cast their vote.
- Voting outcomes are tallied by chamber and party, and the final result (PASS or FAIL) is announced.

---

## Magic Numbers Reference Table

| Number | Location | Meaning / Explanation |
|---|---|---|
| **0.5 - 1.0** | `Legislator.PartyLoyalty` | Party loyalty range, from moderate loyalty (0.5) to full loyalty (1.0). |
| **0 - 1.0** | `Legislator.ReelectionConcern` | How much the legislator worries about reelection — 0 is carefree, 1.0 is paranoid. |
| **0.5** | `Legislator.TrustLevels` | Initial neutral trust level between all legislators. |
| **0.1** | `Legislator.CapturedBySpecialInterest` | 10% chance a legislator is "captured" by special interest groups, influencing their votes. |
| **0.3** | `VoteOnBill - partyPressure weight` | 30% weight for following party stance when deciding how to vote. |
| **0.25** | `VoteOnBill - personalBelief weight` | 25% weight for the legislator's personal ideological belief. |
| **0.15** | `VoteOnBill - selfPreservation weight` | 15% weight for voting based on reelection concern. |
| **0.03** | `VoteOnBill - trust bonus/penalty` | Trusted allies add up to 3% influence, distrusted rivals subtract up to 3%. |
| **0.05** | `VoteOnBill - specialInterestBonus` | 5% extra influence if the legislator is captured by special interests. |
| **0.05** | `LobbyGroup.partyPressures["Democrat"]` | Lobbying pressure nudges Democrats 5% toward YES votes. |
| **-0.05** | `LobbyGroup.partyPressures["Republican"]` | Lobbying pressure nudges Republicans 5% toward NO votes. |
| **0.02** | `LobbyGroup.partyPressures["Independent"]` | Independents receive a smaller 2% nudge toward YES. |
| **0.1** | `MakeDealWith` | Successful deals increase mutual trust by 10%. |
| **0.01** | `MakeDealWith` | Deals slightly boost reputation by 1%. |
| **0.05** | `Betray` | Betrayals drop reputation by 5%. |
| **0.01** | `UpdateVoterApproval` | Voter approval rises or falls by 1% per vote, based on party alignment. |
| **0.1** | `ProcessDealsAndBetrayals - deal chance` | Each legislator has a 10% chance to make a deal with someone else before a vote. |
| **0.05** | `ProcessDealsAndBetrayals - betrayal chance` | Each legislator has a 5% chance to betray someone else before a vote. |
| **0.5** | `VoteOnBill - pass threshold` | Legislators vote YES if their final influence score is ≥ 0.5. |

---

## Class Summaries

### Legislator

The core actor in the simulation. Each legislator has:

- Party loyalty, ideology, reelection concern, etc.
- Trust levels with all other legislators.
- Tracks deals, betrayals, lobbying pressure, and special interest influence.
- Votes based on internal and external pressures.

---

### VoteTally

- Tracks the **final count of YES/NO votes**.
- Provides a **party-level breakdown** for each chamber.
- Supports simple vote recording and lookup.

---

### LobbyGroup

- Applies **small lobby pressures based on party affiliation**.
- Democratic legislators are slightly pushed toward YES.
- Republican legislators are slightly pushed toward NO.
- Independents receive minor positive pressure.

---

### Program

The central class that:

- Initializes all legislators for the House and Senate.
- Assigns trust levels between all legislators.
- Runs the full bill voting simulation for multiple bills.
- Displays:
    - Per-bill results.
    - Final legislator summaries (trust, reputation, voter approval).

---

## Process Flow

1. **Initialization**
    - Legislators created and trust initialized.
2. **Pre-Vote Phase**
    - Ideological support randomized per bill.
    - Lobbying pressure applied.
    - Legislators form deals or betray others.
3. **Vote Phase**
    - Each legislator votes based on all factors.
    - Votes tallied per chamber and party.
4. **Results Phase**
    - Bill passes if both House and Senate approve.
5. **End Summary**
    - Displays final legislator standings, reputation, voter approval, and trust networks.

---

## Future Enhancements

- Add public opinion swings to influence ideological support.
- Track long-term relationships across multiple bills.
- Introduce lobbying factions with competing interests.
- Simulate committee hearings before the full vote.
- Add scandals, retirements, and mid-session replacements.

---

## License

MIT License – Feel free to use, modify, and contribute.

---

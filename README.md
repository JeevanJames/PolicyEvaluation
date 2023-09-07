![Banner](Images/Banner.png)

# PolicyEvaluation

[![Package NuGet Package](https://img.shields.io/nuget/v/Jeevan.PolicyEvaluation.svg)](https://www.nuget.org/packages/Jeevan.PolicyEvaluation/) [![Package NuGet Package Downloads](https://img.shields.io/nuget/dt/Jeevan.PolicyEvaluation)](https://www.nuget.org/packages/Jeevan.PolicyEvaluation) [![GitHub Actions Status](https://github.com/JeevanJames/PolicyEvaluation/workflows/Build/badge.svg?branch=main)](https://github.com/JeevanJames/PolicyEvaluation/actions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/JeevanJames/PolicyEvaluation?branch=main&includeBuildsFromPullRequest=false)](https://github.com/JeevanJames/PolicyEvaluation/actions)

PolicyEvaluation is a small library for parsing logical expressions containing policy names, to evaluate if the expression passes or fails. The policy evaluation itself is provided externally (through a delegate); the main job of this library is to evaluate the policies in the context of a logical expression.

A few sample expressions (where `Policy1`, `Policy2`, `Policy3` and `Policy4` are policy names):
* `Policy1 AND Policy2`
* `Policy1 OR Policy2`
* `Policy1 AND (Policy2 OR Policy3)`
* `(Policy1 AND Policy2) OR Policy3`
* `(Policy1 OR Policy2) AND (Policy3 OR Policy4)`

Rules for expressions:
* Policy names should start with a letter, followed by alphanumeric characters and underscores.
* Only AND and OR logical operators are allowed, and they are case-sensitive.
* Nesting is allowed using parentheses. Only one level of nesting is allowed.

## Installation

Via Package Manager:

```
PM> Install-Package Jeevan.PolicyEvaluation
```

Via `dotnet` CLI:

```
dotnet add package Jeevan.PolicyEvaluation

dotnet add <project> package Jeevan.PolicyEvaluation
```

## Usage

```cs
// Import the namespace Jeevan.PolicyEvaluation
using Jeevan.PolicyEvaluation;

// Declare a function to evaluate a policy, given its name.
Func<string, IPolicyOutcome> policyEvaluatorLogic = policyName =>
{
    // Perform policy evaluation here.
    // Use methods from PolicyOutcome to generate the result.
};

// Instantiate the PolicyEvaluator class
PolicyEvaluator evaluator = new(policyEvaluatorLogic);

// Call the EvaluateExpression method to evaluate an expression.
const string expression = "Policy1 AND (Policy2 OR Policy3)";
var result = evaluator.EvaluateExpression(expression);

// You can check whether the expression was satisfied.
if (result.IsSatisfied)
    Console.WriteLine("Expression was satisfied.");

// OR you can check whether the expression was not satisfied and get the reason and which policy
// caused the expression to fail.
if (result.IsNotSatisfied(out string? policyName, out string? failureMessage))
    Console.WriteLine($"Policy {policyName} failed with error {failureMessage}.");
```

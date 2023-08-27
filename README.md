![Banner](Images/Banner.png)

# PolicyEvaluation

[![Package NuGet Package](https://img.shields.io/nuget/v/PolicyEvaluation.svg)](https://www.nuget.org/packages/PolicyEvaluation/) [![Package NuGet Package Downloads](https://img.shields.io/nuget/dt/PolicyEvaluation)](https://www.nuget.org/packages/PolicyEvaluation) [![GitHub Actions Status](https://github.com/JeevanJames/PolicyEvaluation/workflows/Build/badge.svg?branch=main)](https://github.com/JeevanJames/PolicyEvaluation/actions)

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

Include the namespace `Jeevan.PolicyEvaluation` and use the static `PolicyEvaluator.EvaluateExpression` method to perform the expression evaluation. This method accepts a delegate that will evaluate the policies based on their name.

```cs
string expression = "Policy1 AND (Policy2 OR Policy3)";
bool evaluationResult = PolicyEvaluator.EvaluateExpression(expression,
    policyName =>
    {
        // Perform policy evaluation here.
    });
```

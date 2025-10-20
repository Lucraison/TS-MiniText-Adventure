using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TextAdventure.Tests;

public class WinPathFeatureTests
{
    [Fact]
    public void WinPath_Scenario_Runs()
    {
        var scenarios = BddRunner.LoadFeature("WinPath.feature");
        Assert.True(scenarios.Any());

        foreach (var steps in scenarios)
        {
            var runner = new BddRunner();
            foreach (var step in steps)
            {
                if (step.StartsWith("Given", System.StringComparison.OrdinalIgnoreCase))
                    runner.Given(step["Given ".Length..]);
                else if (step.StartsWith("When", System.StringComparison.OrdinalIgnoreCase))
                    runner.When(step["When ".Length..]);
                else if (step.StartsWith("Then", System.StringComparison.OrdinalIgnoreCase))
                    runner.Then(step["Then ".Length..]);
                else if (step.StartsWith("And", System.StringComparison.OrdinalIgnoreCase))
                    runner.When(step["And ".Length..]);
            }
        }
    }
}

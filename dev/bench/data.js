window.BENCHMARK_DATA = {
  "lastUpdate": 1789844250584,
  "repoUrl": "https://github.com/Chris-Wolfgang/Try-Pattern",
  "entries": {
    "BenchmarkDotNet": [
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "b3a840b09437035ab14b8c7795de48e2dac9e38a",
          "message": "Merge pull request #166 from Chris-Wolfgang/vNext\n\nRelease v0.3.2: canonical maintenance round + AssemblyVersion fix",
          "timestamp": "2026-06-27T17:10:29-04:00",
          "tree_id": "ebd5c5362e24c12f66f744973cc4364cf6975c48",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/b3a840b09437035ab14b8c7795de48e2dac9e38a"
        },
        "date": 1782594872471,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 2501.273443222046,
            "unit": "ns",
            "range": "± 5.122858801890151"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 9789196.69140625,
            "unit": "ns",
            "range": "± 10925.762752249604"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 693283.339453125,
            "unit": "ns",
            "range": "± 7607.952808415729"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 48993311.00645162,
            "unit": "ns",
            "range": "± 1494080.8164064111"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 9710.254013061523,
            "unit": "ns",
            "range": "± 41.93233071363464"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 9755213.1,
            "unit": "ns",
            "range": "± 29402.558866039995"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 47370.66650390625,
            "unit": "ns",
            "range": "± 288.6867121580512"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 21793111.09598214,
            "unit": "ns",
            "range": "± 106154.11349330173"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 2191.1768913269043,
            "unit": "ns",
            "range": "± 2.6780552622815246"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 9906356.197916666,
            "unit": "ns",
            "range": "± 73123.69819301521"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 698820.7652994791,
            "unit": "ns",
            "range": "± 6860.5380115459875"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 50406814.03030303,
            "unit": "ns",
            "range": "± 1203222.114257168"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 9938.262247721354,
            "unit": "ns",
            "range": "± 90.3326959139449"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 9866183.661458334,
            "unit": "ns",
            "range": "± 42577.10099260121"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 46779.46221923828,
            "unit": "ns",
            "range": "± 549.827278179399"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 21830051.927083332,
            "unit": "ns",
            "range": "± 130646.3942251359"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "49699333+dependabot[bot]@users.noreply.github.com",
            "name": "dependabot[bot]",
            "username": "dependabot[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "ccee1a04b5f9921f6d38670976451dfcc9b52eaf",
          "message": "chore(deps): bump benchmark-action/github-action-benchmark (#356)\n\nBumps the github-actions group with 1 update: [benchmark-action/github-action-benchmark](https://github.com/benchmark-action/github-action-benchmark).\n\n\nUpdates `benchmark-action/github-action-benchmark` from 1.22.1 to 1.22.2\n- [Release notes](https://github.com/benchmark-action/github-action-benchmark/releases)\n- [Changelog](https://github.com/benchmark-action/github-action-benchmark/blob/master/CHANGELOG.md)\n- [Commits](https://github.com/benchmark-action/github-action-benchmark/compare/52576c92bccf6ac60c8223ec7eb2565637cae9ba...4322e5726e6334590d251fc4f92bec0efafc45dc)\n\n---\nupdated-dependencies:\n- dependency-name: benchmark-action/github-action-benchmark\n  dependency-version: 1.22.2\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>",
          "timestamp": "2026-09-19T14:51:59-04:00",
          "tree_id": "9487a8eff0fdca821e90c97d1d720e8604ed8f2b",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/ccee1a04b5f9921f6d38670976451dfcc9b52eaf"
        },
        "date": 1789844248992,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 2505.0413773854575,
            "unit": "ns",
            "range": "± 4.997865684808362"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 2382824.3515625,
            "unit": "ns",
            "range": "± 7114.141441793392"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 581995.4389648438,
            "unit": "ns",
            "range": "± 2548.5763729793794"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 13282958.364583334,
            "unit": "ns",
            "range": "± 246191.1046360991"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 7347.305088043213,
            "unit": "ns",
            "range": "± 45.00821461289807"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 2242174.2356770835,
            "unit": "ns",
            "range": "± 8428.398021100598"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 40036.07860310873,
            "unit": "ns",
            "range": "± 137.9290213581013"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 4965281.717447917,
            "unit": "ns",
            "range": "± 43356.09256580377"
          }
        ]
      }
    ]
  }
}
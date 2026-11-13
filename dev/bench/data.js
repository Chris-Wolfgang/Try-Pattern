window.BENCHMARK_DATA = {
  "lastUpdate": 1794533284476,
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
      },
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
          "id": "8e98767b8a0580131061bed44b4d55b8c7ee7d5b",
          "message": "build: mark the net8.0+ assemblies IsAotCompatible and IsTrimmable (#371)\n\nBaseline item 18. Set in the src csproj under a net8.0-or-later\ncondition (the analyzers only exist there); the Release build with\nTreatWarningsAsErrors reports no IL2xxx/IL3xxx findings.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-09-26T17:32:41-04:00",
          "tree_id": "ea911acf7a8fadf1db9c5f6d0a39d26376de6e2c",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/8e98767b8a0580131061bed44b4d55b8c7ee7d5b"
        },
        "date": 1790458623373,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 1371.810043970744,
            "unit": "ns",
            "range": "± 10.015463789124155"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 1230497.3645833333,
            "unit": "ns",
            "range": "± 9398.46080860814"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 273967.1217447917,
            "unit": "ns",
            "range": "± 9783.38363052222"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 5798058.390625,
            "unit": "ns",
            "range": "± 10419.381315849128"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 3916.771194458008,
            "unit": "ns",
            "range": "± 56.3744753432902"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 1222671.7659505208,
            "unit": "ns",
            "range": "± 6958.839057895006"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 18428.712412516277,
            "unit": "ns",
            "range": "± 277.41502419460704"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 2597007.4973958335,
            "unit": "ns",
            "range": "± 5940.490404431063"
          }
        ]
      },
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
          "id": "f2ab716e4dfd7b77561e92718cb574f23a9f29e5",
          "message": "release: v0.5.0 — trim / Native AOT-compatible (#393)\n\nMINOR: the net8.0+ assemblies are now marked IsAotCompatible/IsTrimmable (#371), new consumer-facing behavior. Public API unchanged (no PublicAPI baseline edits); AssemblyVersion stays 1.0.0.0. CHANGELOG assembled from changelog/unreleased by scripts/changelog.ps1.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-09-27T15:56:49-04:00",
          "tree_id": "2b9afda20c502a26a5389a45b286782b23c7841d",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/f2ab716e4dfd7b77561e92718cb574f23a9f29e5"
        },
        "date": 1790539097593,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 2504.333938598633,
            "unit": "ns",
            "range": "± 5.006534296503212"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 2344363.482421875,
            "unit": "ns",
            "range": "± 45366.48860780495"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 624271.3974609375,
            "unit": "ns",
            "range": "± 11342.026172614636"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 11994410.25,
            "unit": "ns",
            "range": "± 203228.30107247294"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 6807.361012776692,
            "unit": "ns",
            "range": "± 65.2547580968393"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 2295457.1067708335,
            "unit": "ns",
            "range": "± 9403.398251986375"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 36078.33190917969,
            "unit": "ns",
            "range": "± 796.1308551689518"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 4803217.6328125,
            "unit": "ns",
            "range": "± 27772.94611725449"
          }
        ]
      },
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
          "id": "cd79172fbcf5471d00394384bb1e73e760fb0355",
          "message": "build: PackageValidation baseline 0.4.1 -> 0.5.0 (#395)\n\nPost-release step for v0.5.0: 0.5.0 is indexed on the NuGet flatcontainer, so pack now validates against it.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-09-27T16:35:05-04:00",
          "tree_id": "0e30e20731f92902cf21910e2c2a295d98f40de7",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/cd79172fbcf5471d00394384bb1e73e760fb0355"
        },
        "date": 1790541394530,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 2193.6394627889,
            "unit": "ns",
            "range": "± 8.436801594853687"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 2283327.5494791665,
            "unit": "ns",
            "range": "± 8968.909383803957"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 566970.2900390625,
            "unit": "ns",
            "range": "± 11483.91513173369"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 10788043.625,
            "unit": "ns",
            "range": "± 134371.38907295067"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 6632.555899302165,
            "unit": "ns",
            "range": "± 65.08725011912189"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 2253538.95703125,
            "unit": "ns",
            "range": "± 21128.541169989447"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 35937.77854410807,
            "unit": "ns",
            "range": "± 818.5343934262241"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 4878522.276041667,
            "unit": "ns",
            "range": "± 14253.971755204831"
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
          "id": "e33561931fa77d67f18dc10da3eda3ce95e3174c",
          "message": "build(deps): bump the github-actions group with 4 updates (#494)\n\nBumps the github-actions group with 4 updates: [github/codeql-action/upload-sarif](https://github.com/github/codeql-action), [actions/setup-dotnet](https://github.com/actions/setup-dotnet), [github/codeql-action/init](https://github.com/github/codeql-action) and [github/codeql-action/analyze](https://github.com/github/codeql-action).\n\n\nUpdates `github/codeql-action/upload-sarif` from 4.39.1 to 4.39.3\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/9b3254085494b55a0f7c4c2b3d2d52cc712ce370...33d7b85da1a04171e211befe2c739303b9a4c066)\n\nUpdates `actions/setup-dotnet` from 6.0.0 to 6.1.0\n- [Release notes](https://github.com/actions/setup-dotnet/releases)\n- [Commits](https://github.com/actions/setup-dotnet/compare/a98b56852c35b8e3190ac28c8c2271da59106c68...0f4a354dd5547a28c70b7853dbd95d5b67886e73)\n\nUpdates `github/codeql-action/init` from 4.39.1 to 4.39.3\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/9b3254085494b55a0f7c4c2b3d2d52cc712ce370...33d7b85da1a04171e211befe2c739303b9a4c066)\n\nUpdates `github/codeql-action/analyze` from 4.39.1 to 4.39.3\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/9b3254085494b55a0f7c4c2b3d2d52cc712ce370...33d7b85da1a04171e211befe2c739303b9a4c066)\n\n---\nupdated-dependencies:\n- dependency-name: github/codeql-action/upload-sarif\n  dependency-version: 4.39.3\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n- dependency-name: actions/setup-dotnet\n  dependency-version: 6.1.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: github-actions\n- dependency-name: github/codeql-action/init\n  dependency-version: 4.39.3\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n- dependency-name: github/codeql-action/analyze\n  dependency-version: 4.39.3\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>\nCo-authored-by: Chris Wolfgang <210299580+Chris-Wolfgang@users.noreply.github.com>",
          "timestamp": "2026-11-08T20:19:26-05:00",
          "tree_id": "8b520c8cf6e59bbd36fd0ad257c69d614e763824",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/e33561931fa77d67f18dc10da3eda3ce95e3174c"
        },
        "date": 1794187469057,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 1653.6172873179119,
            "unit": "ns",
            "range": "± 17.385198216618825"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 1711113.6741536458,
            "unit": "ns",
            "range": "± 15404.569339872542"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 381470.8614095052,
            "unit": "ns",
            "range": "± 1063.497251945833"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 8046366.46875,
            "unit": "ns",
            "range": "± 78456.12258835741"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 5563.250625610352,
            "unit": "ns",
            "range": "± 128.8485851031944"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 1754655.9680989583,
            "unit": "ns",
            "range": "± 2811.418723276248"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 30297.842783610027,
            "unit": "ns",
            "range": "± 195.70362042496018"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 3752152.0026041665,
            "unit": "ns",
            "range": "± 5862.503207682021"
          }
        ]
      },
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
          "id": "c83905947812b29945067133a89872461a4bfd79",
          "message": "chore(pack): stop requiring licence acceptance for the MIT package (#477)\n\nPackageRequireLicenseAcceptance=True makes Visual Studio show an\naccept-licence prompt on install. MIT does not require acceptance, so the\nprompt is only friction.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-08T21:44:18-05:00",
          "tree_id": "4f737bfd79456fe2bf253e8951bdc9e69fa91c32",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/c83905947812b29945067133a89872461a4bfd79"
        },
        "date": 1794192355829,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 1659.6732730865479,
            "unit": "ns",
            "range": "± 24.677304951591513"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 1716182.2688802083,
            "unit": "ns",
            "range": "± 17874.832233424488"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 369019.6393229167,
            "unit": "ns",
            "range": "± 11294.15105732223"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 8058436.625,
            "unit": "ns",
            "range": "± 68227.29733054445"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 5468.215293884277,
            "unit": "ns",
            "range": "± 36.79634561108848"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 1674756.8916015625,
            "unit": "ns",
            "range": "± 6286.45804602367"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 29538.61587524414,
            "unit": "ns",
            "range": "± 582.0673290132264"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 3701385.640625,
            "unit": "ns",
            "range": "± 43200.24929783573"
          }
        ]
      },
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
          "id": "89aaff12b059d8746c5e06fa49130173e19018f5",
          "message": "docs: align XML docs and ADRs with the v0.4.0 behaviour (#453)\n\n- Try: class <remarks> states the SafeErrorMessage fallback (#273); each\n  overload's <returns> references it.\n- Result/Result<T> ctors: \"error message\", not \"exception information\";\n  Result<T> ctor remark says null or empty on success (both become null).\n- Flatten: ErrorMessage as a <see cref>.\n- ADR 0003/0004: snippets match the code (SafeErrorMessage(ex),\n  errorMessage: null) with a dated transcription note.\n- ResultTests: drop a hard-coded Result.cs line number from a comment.\n\nRefs #435\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-12T09:23:55-05:00",
          "tree_id": "fbff40acc15771364d584b2e5133a9d68401c384",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/89aaff12b059d8746c5e06fa49130173e19018f5"
        },
        "date": 1794493749826,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 1452.9420744578044,
            "unit": "ns",
            "range": "± 21.106835027479033"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 1353720.5361328125,
            "unit": "ns",
            "range": "± 96131.9063035297"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 324093.64371744794,
            "unit": "ns",
            "range": "± 2538.1070054861025"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 6451258.231770833,
            "unit": "ns",
            "range": "± 99774.75434103949"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 4872.694163004558,
            "unit": "ns",
            "range": "± 15.12594209760423"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 1291287.3681640625,
            "unit": "ns",
            "range": "± 3284.97977668657"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 22460.130930582684,
            "unit": "ns",
            "range": "± 947.5746588501625"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 2759724.7109375,
            "unit": "ns",
            "range": "± 10857.2855167187"
          }
        ]
      },
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
          "id": "2da2ffbb0e65217adc1119e3f6e301025fc00643",
          "message": "tests/.editorconfig: drop unused and no-longer-needed suppressions (#457)\n\n- S1215, S6562, S6610, xUnit2013: template boilerplate; nothing in tests/\n  triggers them (xUnit2013 needs xunit.analyzers, not referenced).\n- S2930, S2699, S1481: their only justifications (undisposed CTS, the\n  assert-less \"does not throw\" test) are gone after #456.\n- Section header: this file is the single source for test-scope\n  suppressions once the csproj <NoWarn> blocks are removed.\n\nRefs #439, #437\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-12T16:14:36-05:00",
          "tree_id": "6ad1412b2f0fbc06b55728fa6e7c7b988b750960",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/2da2ffbb0e65217adc1119e3f6e301025fc00643"
        },
        "date": 1794518265817,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.Flatten_AllSucceeded",
            "value": 12.753652016321817,
            "unit": "ns",
            "range": "± 0.20310793399702579"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.Flatten_OneFailed",
            "value": 18.272124181191128,
            "unit": "ns",
            "range": "± 0.27571378929785767"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.Flatten_AllFailed",
            "value": 258.3052142461141,
            "unit": "ns",
            "range": "± 4.078519945070249"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.AnyFailed_AllSucceeded",
            "value": 10.458071077863375,
            "unit": "ns",
            "range": "± 0.056635009593920094"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.AllSucceeded_AllSucceeded",
            "value": 10.40492032468319,
            "unit": "ns",
            "range": "± 0.2113904175068463"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 2191.5679410298667,
            "unit": "ns",
            "range": "± 3.4275395943974836"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 2213167.7122395835,
            "unit": "ns",
            "range": "± 10214.27846811344"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 572970.4049479166,
            "unit": "ns",
            "range": "± 12302.219523930882"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 11392217.84375,
            "unit": "ns",
            "range": "± 323894.63063206867"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 6956.983664194743,
            "unit": "ns",
            "range": "± 25.988567875442524"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 2189122.8020833335,
            "unit": "ns",
            "range": "± 12242.356839802635"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 38928.500244140625,
            "unit": "ns",
            "range": "± 1526.819445672285"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 4834655.846354167,
            "unit": "ns",
            "range": "± 36343.616728622546"
          }
        ]
      },
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
          "id": "90b1447b7fd602140a0121f76979c201b3f57bc4",
          "message": "Align Microsoft.NET.Test.Sdk to 17.13.0 on every test slot (#439) (#476)\n\nThe Unit test project pinned Microsoft.NET.Test.Sdk to 17.11.1 (net462-\nnet481, net5.0), 17.12.0 (net6.0/net7.0) and 17.13.0 (net8.0+) with no\ncomment. The 17.11.1, 17.12.0 and 17.13.0 nuspecs declare the same\ndependency groups (net462 and netcoreapp3.1), so the split was not a\nTFM-support limit. All slots now use 17.13.0, the version the net8.0+\nslot and the Concurrency project already use.\n\nCo-authored-by: Claude Fable 5.1 <noreply@anthropic.com>",
          "timestamp": "2026-11-12T17:30:12-05:00",
          "tree_id": "d1afcce278a1687e88bd2b862c407e21393e17d2",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/90b1447b7fd602140a0121f76979c201b3f57bc4"
        },
        "date": 1794523151323,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.Flatten_AllSucceeded",
            "value": 5.022242963314056,
            "unit": "ns",
            "range": "± 0.019761495963195815"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.Flatten_OneFailed",
            "value": 10.243039528528849,
            "unit": "ns",
            "range": "± 0.09815846094301553"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.Flatten_AllFailed",
            "value": 120.92573563257854,
            "unit": "ns",
            "range": "± 0.85760641722521"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.AnyFailed_AllSucceeded",
            "value": 4.179766692221165,
            "unit": "ns",
            "range": "± 0.23167360638615322"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.AllSucceeded_AllSucceeded",
            "value": 3.968304418027401,
            "unit": "ns",
            "range": "± 0.03015515255243098"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 1344.11976369222,
            "unit": "ns",
            "range": "± 1.6583996588864656"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 1184176.3151041667,
            "unit": "ns",
            "range": "± 3561.2152240014357"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 269381.95524088544,
            "unit": "ns",
            "range": "± 5721.710941540431"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 6218703.524739583,
            "unit": "ns",
            "range": "± 93002.66106149035"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 4134.010999043782,
            "unit": "ns",
            "range": "± 45.19628830699804"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 1200383.9524739583,
            "unit": "ns",
            "range": "± 24147.630182572877"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 18808.143412272137,
            "unit": "ns",
            "range": "± 44.106729293988586"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 2514903.1959635415,
            "unit": "ns",
            "range": "± 5445.368469446395"
          }
        ]
      },
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
          "id": "91d5358e4fe209747a73fc4868be51e8a4779742",
          "message": "docs(reproducible-build): describe the cross-OS byte-identical build (#510)\n\nMoved from #429 so that PR stays configuration-only for the Protected\nFiles Guard. Content is #429's original docs/REPRODUCIBLE-BUILD.md change.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-12T20:24:24-05:00",
          "tree_id": "03d300a6e8350113f023797238296ef79c67b8e0",
          "url": "https://github.com/Chris-Wolfgang/Try-Pattern/commit/91d5358e4fe209747a73fc4868be51e8a4779742"
        },
        "date": 1794533282668,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.Flatten_AllSucceeded",
            "value": 9.130126347144445,
            "unit": "ns",
            "range": "± 0.1419598821376465"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.Flatten_OneFailed",
            "value": 15.139345477024714,
            "unit": "ns",
            "range": "± 0.43631584811903273"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.Flatten_AllFailed",
            "value": 196.69713672002158,
            "unit": "ns",
            "range": "± 5.976607111001373"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.AnyFailed_AllSucceeded",
            "value": 8.243144690990448,
            "unit": "ns",
            "range": "± 0.04839273411059646"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.ResultBenchmarks.AllSucceeded_AllSucceeded",
            "value": 8.047743951280912,
            "unit": "ns",
            "range": "± 0.013481685499404704"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_Success",
            "value": 1670.242436726888,
            "unit": "ns",
            "range": "± 27.9657795864851"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Action_WithException",
            "value": 1714008.3658854167,
            "unit": "ns",
            "range": "± 12870.130221123834"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_Success",
            "value": 447412.962890625,
            "unit": "ns",
            "range": "± 10651.283380735647"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Action_WithException",
            "value": 7838455.307291667,
            "unit": "ns",
            "range": "± 83445.0772635253"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_Success",
            "value": 5630.6723225911455,
            "unit": "ns",
            "range": "± 93.91897007320077"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.Run_Func_WithException",
            "value": 1698926.6875,
            "unit": "ns",
            "range": "± 2578.4980002997077"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_Success",
            "value": 30292.55974324544,
            "unit": "ns",
            "range": "± 108.51840599685231"
          },
          {
            "name": "Wolfgang.TryPattern.Benchmarks.TryBenchmarks.RunAsync_Func_WithException",
            "value": 3652991.9453125,
            "unit": "ns",
            "range": "± 15982.720761879358"
          }
        ]
      }
    ]
  }
}
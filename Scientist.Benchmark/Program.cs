// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Scientist.Benchmark;

var summary = BenchmarkRunner.Run<ExperimentBenchmarks>();

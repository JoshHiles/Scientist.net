using BenchmarkDotNet.Attributes;

namespace Scientist.Benchmark
{
    [HtmlExporter]
    public class ExperimentBenchmarks
    {
        readonly Experiment<int> experiment;

        public ExperimentBenchmarks()
        {
            experiment = new Experiment<int>(name: "Instance", control: FortyTwo);
        }

        [Benchmark(Baseline = true)]
        public int Static()
        {
            return new Experiment<int>(nameof(Static), control: FortyTwo)
                .AddCandidate(FortyTwo)
                .Run();
        }

        // Need to name candidates something
        //[Benchmark]
        //public int Instance()
        //{
        //    return experiment.AddCandidate(FortyTwo).Run();
        //}

        [Benchmark]
        public int Multiple()
        {
            return new Experiment<int>(nameof(Multiple), control: FortyTwo)
                .AddCandidate(FortyTwo)
                .Run();
        }

        private static int FortyTwo() => 42;
    }
}

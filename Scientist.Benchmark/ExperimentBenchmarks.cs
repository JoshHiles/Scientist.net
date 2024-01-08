using BenchmarkDotNet.Attributes;

namespace Scientist.Benchmark
{
    [MemoryDiagnoser]
    public class ExperimentBenchmarks
    {
        readonly Experiment<int> experiment;

        public ExperimentBenchmarks()
        {
            experiment = new Experiment<int>(name: "Instance", control: FortyTwo);
        }

        [Benchmark]
        public int Static()
        {
            return new Experiment<int>(name: "Static", control: FortyTwo).Run();
        }

        [Benchmark(Baseline = false)]
        public int Instance()
        {
            return experiment.Run();
        }

        private static int FortyTwo() => 42;
    }
}

namespace ConsoleApp1;

public sealed class OptionsHash
{
    public int Iterations { get; set; }
    
    public OptionsHash(int iterations)
    {
        Iterations = iterations;
    }
}
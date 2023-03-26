using static ThirdParty.SDL.SDL;

namespace SBEngine;

public class Time
{
    public float DeltaTime { get; private set; }
    public float TimeSinceProgramStart { get; private set; }
    public long Updates { get; private set; }
    public ulong LastTimeCounter { get; private set; }
    
    private ulong _cpuFreq;

    public Time()
    {
        LastTimeCounter = SDL_GetPerformanceCounter();
        _cpuFreq = SDL_GetPerformanceFrequency();
    }

    public void Update()
    {
        ulong nowCounter = SDL_GetPerformanceCounter();
        DeltaTime = (float)(nowCounter - LastTimeCounter) / (float)_cpuFreq;
        LastTimeCounter = nowCounter;

        TimeSinceProgramStart += DeltaTime;
        Updates++;
    }

    public ulong GetPerformanceCounter()
    {
        return SDL_GetPerformanceCounter();
    }

    public float GetDelta(ulong counter)
    {
        ulong nowCounter = SDL_GetPerformanceCounter();
        DeltaTime = (float)(nowCounter - counter) / (float)_cpuFreq;
        return DeltaTime;
    }

    public float TicksToSeconds(ulong ticks)
    {
        return (float)(ticks) / (float)_cpuFreq;
    }

    public float TicksToMilliseconds(ulong ticks)
    {
        return (float)(ticks) / (float)_cpuFreq * 1000f;
    }
}
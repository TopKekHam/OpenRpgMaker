using static SDL;

/*
    HOW TO USE:

    Load() - before use call this function to init the state.
    Update() - in the start of each frame call this function to update the data.
 */

public static class Time
{

    public static float deltaTime;
    public static float timeSinceProgramStart;
    public static long updates;

    public static ulong lastTimeCounter;
    public static ulong cpuFreq;

    public static void Load()
    {
        lastTimeCounter = SDL_GetPerformanceCounter();
        cpuFreq = SDL_GetPerformanceFrequency();
    }

    public static void Update()
    {
        ulong now_counter = SDL_GetPerformanceCounter();
        deltaTime = (float)(now_counter - lastTimeCounter) / (float)cpuFreq;
        lastTimeCounter = now_counter;

        timeSinceProgramStart += deltaTime;
        updates++;
    }

    public static ulong GetPerformanceCounter()
    {
        return SDL_GetPerformanceCounter();
    }

    public static float GetDelta(ulong counter)
    {
        ulong now_counter = SDL_GetPerformanceCounter();
        deltaTime = (float)(now_counter - counter) / (float)cpuFreq;
        return deltaTime;
    }

    public static float TicksToSeconds(ulong ticks)
    {
        return (float)(ticks) / (float)cpuFreq;
    }

    public static float TicksToMilliseconds(ulong ticks)
    {
        return (float)(ticks) / (float)cpuFreq * 1000f;
    }

}
#include <windows.h>

//not thread safe
LARGE_INTEGER m_nFreq;
LARGE_INTEGER m_nBeginTime;
void startTimer()
{
		QueryPerformanceFrequency(&m_nFreq);
        QueryPerformanceCounter(&m_nBeginTime);
}

__int64 stopTimer()
{
        LARGE_INTEGER nEndTime;
        __int64 nCalcTime;

    	QueryPerformanceCounter(&nEndTime);
        nCalcTime = (nEndTime.QuadPart - m_nBeginTime.QuadPart) *
            1000/m_nFreq.QuadPart;

        return nCalcTime;
}

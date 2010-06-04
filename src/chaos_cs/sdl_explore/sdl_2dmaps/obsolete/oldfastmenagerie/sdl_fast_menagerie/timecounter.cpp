#include <windows.h>

//not thread safe :)
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

#define _MMX_FEATURE_BIT        0x00800000      // bit 23
#define _SSE_FEATURE_BIT        0x02000000      // bit 25

void TestFeatures(bool* pbMMX,              // [out] true - MMX is supported
                      bool* pbSSE)              // [out] true - SSE is supported
    {
        *pbMMX = false;
        *pbSSE = false;

        unsigned int dwFeature = 0;

        // test processor support
        __try 
        {
            _asm 
            {
                mov eax, 1
                cpuid
                mov dwFeature, edx      // this value defines support for MMX, SSE ...
            }
        }
        __except (EXCEPTION_EXECUTE_HANDLER) 
        {
            return;         // cpuid is not supported; MMX and SSE are not supported
        }

        if (dwFeature & _MMX_FEATURE_BIT )  // processor supports MMX
        {
            // test OS support for MMX
            __try 
            {
                _asm
                {
                    pxor mm0, mm0           // executing any MMX instruction
                    emms
                }

                *pbMMX = true;
            }
            __except (EXCEPTION_EXECUTE_HANDLER) 
            {
            }
        }

        if (dwFeature & _SSE_FEATURE_BIT )  // processor supports SSE
        {
            // test OS support for SSE
            __try 
            {
                _asm
                {
                    xorps xmm0, xmm0        // executing any SSE instruction
                }

                *pbSSE = true;
            }
            __except (EXCEPTION_EXECUTE_HANDLER) 
            {
            }
        }
    }
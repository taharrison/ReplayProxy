using System;

namespace ReplayMocks
{
    [Flags]
    public enum VerifierOptions
    {
        None = 0,
        OnlyExpectedCallsAreAllowed = 0x1,
        AllExpectedCallsMustBeObserved = 0x2,
        ExpectedCallsMustBeObservedTheExactNumberOfTimes = 0x4,

        Default_SameCallsSameNumberOfTimesInAnyOrder = 0x7,

        CallsMustOccurInTheExactSameOrder = 0x8
    }
}
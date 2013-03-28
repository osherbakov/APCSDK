using System;
using System.Collections.Generic;
using System.Text;

namespace CS_TEST
{
    public class Test : Diacom.APCLineControl
    {
        public Test()
        {
            FunctionTest("Constructor Called!!!!!");
        }
        public void Main()
        {
            FunctionTest("WWWWWW");
        }

	    public void Z()
	    {
		    Init();
	    }

	    public void FreeLine()
	    {
            ResetAll();
            OnEvent("XYSTWZ", "Z");
            DropCall(2);
            WaitComplete();
	    }

	    public void ZJ()    // Event RING "J" came from the line
	    {
            Timeout = 45;
            OnEvent("XYSTW", "FreeLine");  // On any error or disconnect event initialize the line        
		    set_Var("Attempts", 3);     // Initialize retries counter
            set_Var("ValidPassword", false);	
	    }

	    public void ZJI()   // Event CONNECT "I" came - now we are connected
	    {
		    Timeout = 45;
		    GotoState("InvalidTrunk");
	    }

	    public void InvalidTrunk()
	    {
            Reset();
            OnEvent("XYSTW", "FreeLine");  // On any error or disconnect event init the line
            PlayFile("202");     // Play "Sorry that you are having problems" phrase
            DropCall(2);
	    }
    }
}

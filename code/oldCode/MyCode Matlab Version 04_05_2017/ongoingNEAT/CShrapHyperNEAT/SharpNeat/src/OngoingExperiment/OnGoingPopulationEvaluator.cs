using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeatLib.Experiments;


namespace OngoingExperimentNS
{
    class OnGoingPopulationEvaluator : MultiThreadedPopulationEvaluator
    {

        public OnGoingPopulationEvaluator(INetworkEvaluator eval)
            : base(eval, null)
        {

        }
    }
}

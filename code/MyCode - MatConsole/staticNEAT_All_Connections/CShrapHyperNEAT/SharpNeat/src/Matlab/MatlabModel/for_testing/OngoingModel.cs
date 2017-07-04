/*
* MATLAB Compiler: 6.4 (R2017a)
* Date: Tue May 23 15:05:06 2017
* Arguments:
* "-B""macro_default""-W""dotnet:MatlabModel,OngoingModel,4.0,private""-T""link:lib""-d""C
* :\Users\Maximus Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
* withoutCOM\ongoingNEAT\CShrapHyperNEAT\SharpNeat\src\Matlab\MatlabModel\for_testing""-v"
* "class{OngoingModel:C:\Users\Maximus
* Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
* withoutCOM\ongoingNEAT\matlab\getModelParams.m,C:\Users\Maximus
* Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
* withoutCOM\ongoingNEAT\matlab\runModel.m}"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace MatlabModel
{

  /// <summary>
  /// The OngoingModel class provides a CLS compliant, MWArray interface to the MATLAB
  /// functions contained in the files:
  /// <newpara></newpara>
  /// C:\Users\Maximus Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
  /// withoutCOM\ongoingNEAT\matlab\getModelParams.m
  /// <newpara></newpara>
  /// C:\Users\Maximus Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
  /// withoutCOM\ongoingNEAT\matlab\runModel.m
  /// </summary>
  /// <remarks>
  /// @Version 4.0
  /// </remarks>
  public class OngoingModel : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Runtime instance.
    /// </summary>
    static OngoingModel()
    {
      if (MWMCR.MCRAppInitialized)
      {
        try
        {
          Assembly assembly= Assembly.GetExecutingAssembly();

          string ctfFilePath= assembly.Location;

          int lastDelimiter= ctfFilePath.LastIndexOf(@"\");

          ctfFilePath= ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

          string ctfFileName = "MatlabModel.ctf";

          Stream embeddedCtfStream = null;

          String[] resourceStrings = assembly.GetManifestResourceNames();

          foreach (String name in resourceStrings)
          {
            if (name.Contains(ctfFileName))
            {
              embeddedCtfStream = assembly.GetManifestResourceStream(name);
              break;
            }
          }
          mcr= new MWMCR("",
                         ctfFilePath, embeddedCtfStream, true);
        }
        catch(Exception ex)
        {
          ex_ = new Exception("MWArray assembly failed to be initialized", ex);
        }
      }
      else
      {
        ex_ = new ApplicationException("MWArray assembly could not be initialized");
      }
    }


    /// <summary>
    /// Constructs a new instance of the OngoingModel class.
    /// </summary>
    public OngoingModel()
    {
      if(ex_ != null)
      {
        throw ex_;
      }
    }


    #endregion Constructors

    #region Finalize

    /// <summary internal= "true">
    /// Class destructor called by the CLR garbage collector.
    /// </summary>
    ~OngoingModel()
    {
      Dispose(false);
    }


    /// <summary>
    /// Frees the native resources associated with this object
    /// </summary>
    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary internal= "true">
    /// Internal dispose function
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        disposed= true;

        if (disposing)
        {
          // Free managed resources;
        }

        // Free native resources
      }
    }


    #endregion Finalize

    #region Methods

    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the getModelParams MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray getModelParams()
    {
      return mcr.EvaluateFunction("getModelParams", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the getModelParams MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numNeurons">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray getModelParams(MWArray numNeurons)
    {
      return mcr.EvaluateFunction("getModelParams", numNeurons);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the getModelParams MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numNeurons">Input argument #1</param>
    /// <param name="weightPath">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray getModelParams(MWArray numNeurons, MWArray weightPath)
    {
      return mcr.EvaluateFunction("getModelParams", numNeurons, weightPath);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the getModelParams MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numNeurons">Input argument #1</param>
    /// <param name="weightPath">Input argument #2</param>
    /// <param name="equParamsPath">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray getModelParams(MWArray numNeurons, MWArray weightPath, MWArray 
                            equParamsPath)
    {
      return mcr.EvaluateFunction("getModelParams", numNeurons, weightPath, equParamsPath);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the getModelParams MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] getModelParams(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "getModelParams", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the getModelParams MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="numNeurons">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] getModelParams(int numArgsOut, MWArray numNeurons)
    {
      return mcr.EvaluateFunction(numArgsOut, "getModelParams", numNeurons);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the getModelParams MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="numNeurons">Input argument #1</param>
    /// <param name="weightPath">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] getModelParams(int numArgsOut, MWArray numNeurons, MWArray 
                              weightPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "getModelParams", numNeurons, weightPath);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the getModelParams MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="numNeurons">Input argument #1</param>
    /// <param name="weightPath">Input argument #2</param>
    /// <param name="equParamsPath">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] getModelParams(int numArgsOut, MWArray numNeurons, MWArray 
                              weightPath, MWArray equParamsPath)
    {
      return mcr.EvaluateFunction(numArgsOut, "getModelParams", numNeurons, weightPath, equParamsPath);
    }


    /// <summary>
    /// Provides an interface for the getModelParams function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void getModelParams(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("getModelParams", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel()
    {
      return mcr.EvaluateFunction("runModel", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="name">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel(MWArray name)
    {
      return mcr.EvaluateFunction("runModel", name);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel(MWArray name, MWArray numNeurons)
    {
      return mcr.EvaluateFunction("runModel", name, numNeurons);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel(MWArray name, MWArray numNeurons, MWArray runTime)
    {
      return mcr.EvaluateFunction("runModel", name, numNeurons, runTime);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel(MWArray name, MWArray numNeurons, MWArray runTime, MWArray 
                      runSettings)
    {
      return mcr.EvaluateFunction("runModel", name, numNeurons, runTime, runSettings);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel(MWArray name, MWArray numNeurons, MWArray runTime, MWArray 
                      runSettings, MWArray savePath)
    {
      return mcr.EvaluateFunction("runModel", name, numNeurons, runTime, runSettings, savePath);
    }


    /// <summary>
    /// Provides a single output, 6-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <param name="equationParams">Input argument #6</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel(MWArray name, MWArray numNeurons, MWArray runTime, MWArray 
                      runSettings, MWArray savePath, MWArray equationParams)
    {
      return mcr.EvaluateFunction("runModel", name, numNeurons, runTime, runSettings, savePath, equationParams);
    }


    /// <summary>
    /// Provides a single output, 7-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <param name="equationParams">Input argument #6</param>
    /// <param name="weightsMatrix">Input argument #7</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel(MWArray name, MWArray numNeurons, MWArray runTime, MWArray 
                      runSettings, MWArray savePath, MWArray equationParams, MWArray 
                      weightsMatrix)
    {
      return mcr.EvaluateFunction("runModel", name, numNeurons, runTime, runSettings, savePath, equationParams, weightsMatrix);
    }


    /// <summary>
    /// Provides a single output, 8-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <param name="equationParams">Input argument #6</param>
    /// <param name="weightsMatrix">Input argument #7</param>
    /// <param name="em_esWeights">Input argument #8</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel(MWArray name, MWArray numNeurons, MWArray runTime, MWArray 
                      runSettings, MWArray savePath, MWArray equationParams, MWArray 
                      weightsMatrix, MWArray em_esWeights)
    {
      return mcr.EvaluateFunction("runModel", name, numNeurons, runTime, runSettings, savePath, equationParams, weightsMatrix, em_esWeights);
    }


    /// <summary>
    /// Provides a single output, 9-input MWArrayinterface to the runModel MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <param name="equationParams">Input argument #6</param>
    /// <param name="weightsMatrix">Input argument #7</param>
    /// <param name="em_esWeights">Input argument #8</param>
    /// <param name="isSave">Input argument #9</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray runModel(MWArray name, MWArray numNeurons, MWArray runTime, MWArray 
                      runSettings, MWArray savePath, MWArray equationParams, MWArray 
                      weightsMatrix, MWArray em_esWeights, MWArray isSave)
    {
      return mcr.EvaluateFunction("runModel", name, numNeurons, runTime, runSettings, savePath, equationParams, weightsMatrix, em_esWeights, isSave);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="name">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut, MWArray name)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", name);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut, MWArray name, MWArray numNeurons)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", name, numNeurons);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut, MWArray name, MWArray numNeurons, MWArray 
                        runTime)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", name, numNeurons, runTime);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut, MWArray name, MWArray numNeurons, MWArray 
                        runTime, MWArray runSettings)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", name, numNeurons, runTime, runSettings);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut, MWArray name, MWArray numNeurons, MWArray 
                        runTime, MWArray runSettings, MWArray savePath)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", name, numNeurons, runTime, runSettings, savePath);
    }


    /// <summary>
    /// Provides the standard 6-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <param name="equationParams">Input argument #6</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut, MWArray name, MWArray numNeurons, MWArray 
                        runTime, MWArray runSettings, MWArray savePath, MWArray 
                        equationParams)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", name, numNeurons, runTime, runSettings, savePath, equationParams);
    }


    /// <summary>
    /// Provides the standard 7-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <param name="equationParams">Input argument #6</param>
    /// <param name="weightsMatrix">Input argument #7</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut, MWArray name, MWArray numNeurons, MWArray 
                        runTime, MWArray runSettings, MWArray savePath, MWArray 
                        equationParams, MWArray weightsMatrix)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", name, numNeurons, runTime, runSettings, savePath, equationParams, weightsMatrix);
    }


    /// <summary>
    /// Provides the standard 8-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <param name="equationParams">Input argument #6</param>
    /// <param name="weightsMatrix">Input argument #7</param>
    /// <param name="em_esWeights">Input argument #8</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut, MWArray name, MWArray numNeurons, MWArray 
                        runTime, MWArray runSettings, MWArray savePath, MWArray 
                        equationParams, MWArray weightsMatrix, MWArray em_esWeights)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", name, numNeurons, runTime, runSettings, savePath, equationParams, weightsMatrix, em_esWeights);
    }


    /// <summary>
    /// Provides the standard 9-input MWArray interface to the runModel MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="name">Input argument #1</param>
    /// <param name="numNeurons">Input argument #2</param>
    /// <param name="runTime">Input argument #3</param>
    /// <param name="runSettings">Input argument #4</param>
    /// <param name="savePath">Input argument #5</param>
    /// <param name="equationParams">Input argument #6</param>
    /// <param name="weightsMatrix">Input argument #7</param>
    /// <param name="em_esWeights">Input argument #8</param>
    /// <param name="isSave">Input argument #9</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] runModel(int numArgsOut, MWArray name, MWArray numNeurons, MWArray 
                        runTime, MWArray runSettings, MWArray savePath, MWArray 
                        equationParams, MWArray weightsMatrix, MWArray em_esWeights, 
                        MWArray isSave)
    {
      return mcr.EvaluateFunction(numArgsOut, "runModel", name, numNeurons, runTime, runSettings, savePath, equationParams, weightsMatrix, em_esWeights, isSave);
    }


    /// <summary>
    /// Provides an interface for the runModel function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void runModel(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("runModel", numArgsOut, ref argsOut, argsIn);
    }



    /// <summary>
    /// This method will cause a MATLAB figure window to behave as a modal dialog box.
    /// The method will not return until all the figure windows associated with this
    /// component have been closed.
    /// </summary>
    /// <remarks>
    /// An application should only call this method when required to keep the
    /// MATLAB figure window from disappearing.  Other techniques, such as calling
    /// Console.ReadLine() from the application should be considered where
    /// possible.</remarks>
    ///
    public void WaitForFiguresToDie()
    {
      mcr.WaitForFiguresToDie();
    }



    #endregion Methods

    #region Class Members

    private static MWMCR mcr= null;

    private static Exception ex_= null;

    private bool disposed= false;

    #endregion Class Members
  }
}

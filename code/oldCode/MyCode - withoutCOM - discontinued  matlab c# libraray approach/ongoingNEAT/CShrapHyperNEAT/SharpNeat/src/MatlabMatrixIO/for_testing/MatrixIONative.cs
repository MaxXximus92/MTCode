/*
* MATLAB Compiler: 6.4 (R2017a)
* Date: Mon May 22 16:32:09 2017
* Arguments:
* "-B""macro_default""-W""dotnet:MatlabMatrixIO,MatrixIO,4.0,private""-T""link:lib""-d""C:
* \Users\Maximus Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
* withoutCOM\ongoingNEAT\CShrapHyperNEAT\SharpNeat\src\MatlabMatrixIO\for_testing""-v""cla
* ss{MatrixIO:C:\Users\Maximus
* Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
* withoutCOM\ongoingNEAT\matlab\CSharpCommunication\loadMatrix.m,C:\Users\Maximus
* Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
* withoutCOM\ongoingNEAT\matlab\CSharpCommunication\saveMatrix.m}"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace MatlabMatrixIONative
{

  /// <summary>
  /// The MatrixIO class provides a CLS compliant, Object (native) interface to the
  /// MATLAB functions contained in the files:
  /// <newpara></newpara>
  /// C:\Users\Maximus Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
  /// withoutCOM\ongoingNEAT\matlab\CSharpCommunication\loadMatrix.m
  /// <newpara></newpara>
  /// C:\Users\Maximus Mutschler\Dropbox\SS17\MasterArbeit\MasterArbeit\code\MyCode -
  /// withoutCOM\ongoingNEAT\matlab\CSharpCommunication\saveMatrix.m
  /// </summary>
  /// <remarks>
  /// @Version 4.0
  /// </remarks>
  public class MatrixIO : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Runtime instance.
    /// </summary>
    static MatrixIO()
    {
      if (MWMCR.MCRAppInitialized)
      {
        try
        {
          Assembly assembly= Assembly.GetExecutingAssembly();

          string ctfFilePath= assembly.Location;

          int lastDelimiter= ctfFilePath.LastIndexOf(@"\");

          ctfFilePath= ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

          string ctfFileName = "MatlabMatrixIO.ctf";

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
    /// Constructs a new instance of the MatrixIO class.
    /// </summary>
    public MatrixIO()
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
    ~MatrixIO()
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
    /// Provides a single output, 0-input Objectinterface to the loadMatrix MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object loadMatrix()
    {
      return mcr.EvaluateFunction("loadMatrix", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the loadMatrix MATLAB function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] loadMatrix(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "loadMatrix", new Object[]{});
    }


    /// <summary>
    /// Provides an interface for the loadMatrix function in which the input and output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("loadMatrix", 0, 1, 0)]
    protected void loadMatrix(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("loadMatrix", numArgsOut, ref argsOut, argsIn, varArgsIn);
    }
    /// <summary>
    /// Provides a single output, 0-input Objectinterface to the saveMatrix MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// UNTITLED Summary of this function goes here
    /// Detailed explanation goes here
    /// </remarks>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object saveMatrix()
    {
      return mcr.EvaluateFunction("saveMatrix", new Object[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input Objectinterface to the saveMatrix MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// UNTITLED Summary of this function goes here
    /// Detailed explanation goes here
    /// </remarks>
    /// <param name="matrix">Input argument #1</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object saveMatrix(Object matrix)
    {
      return mcr.EvaluateFunction("saveMatrix", matrix);
    }


    /// <summary>
    /// Provides a single output, 2-input Objectinterface to the saveMatrix MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// UNTITLED Summary of this function goes here
    /// Detailed explanation goes here
    /// </remarks>
    /// <param name="matrix">Input argument #1</param>
    /// <param name="path">Input argument #2</param>
    /// <returns>An Object containing the first output argument.</returns>
    ///
    public Object saveMatrix(Object matrix, Object path)
    {
      return mcr.EvaluateFunction("saveMatrix", matrix, path);
    }


    /// <summary>
    /// Provides the standard 0-input Object interface to the saveMatrix MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// UNTITLED Summary of this function goes here
    /// Detailed explanation goes here
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] saveMatrix(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "saveMatrix", new Object[]{});
    }


    /// <summary>
    /// Provides the standard 1-input Object interface to the saveMatrix MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// UNTITLED Summary of this function goes here
    /// Detailed explanation goes here
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="matrix">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] saveMatrix(int numArgsOut, Object matrix)
    {
      return mcr.EvaluateFunction(numArgsOut, "saveMatrix", matrix);
    }


    /// <summary>
    /// Provides the standard 2-input Object interface to the saveMatrix MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// UNTITLED Summary of this function goes here
    /// Detailed explanation goes here
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="matrix">Input argument #1</param>
    /// <param name="path">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public Object[] saveMatrix(int numArgsOut, Object matrix, Object path)
    {
      return mcr.EvaluateFunction(numArgsOut, "saveMatrix", matrix, path);
    }


    /// <summary>
    /// Provides an interface for the saveMatrix function in which the input and output
    /// arguments are specified as an array of Objects.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// UNTITLED Summary of this function goes here
    /// Detailed explanation goes here
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of Object output arguments</param>
    /// <param name= "argsIn">Array of Object input arguments</param>
    /// <param name= "varArgsIn">Array of Object representing variable input
    /// arguments</param>
    ///
    [MATLABSignature("saveMatrix", 2, 1, 0)]
    protected void saveMatrix(int numArgsOut, ref Object[] argsOut, Object[] argsIn, params Object[] varArgsIn)
    {
        mcr.EvaluateFunctionForTypeSafeCall("saveMatrix", numArgsOut, ref argsOut, argsIn, varArgsIn);
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

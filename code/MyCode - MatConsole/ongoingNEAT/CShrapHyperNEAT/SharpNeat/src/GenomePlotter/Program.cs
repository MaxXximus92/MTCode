using SharpNeatLib;
using SharpNeatLib.Evolution;
using SharpNeatLib.NeatGenome;
using SharpNeatLib.NeatGenome.Xml;
using SharpNeatLib.NetworkVisualization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static OngoingExperimentNS.Helper;

namespace GenomePlotter
{
    class GenomePlotter
    {
        static void Main(string[] args)
        {
            PlotGenomeGraphs(getGenomeSavePath(), getGenomeImageSavePath());

        }

        static private string getGenomeSavePath()
        {
            string path = toFullPath(@"genomes");
            //Directory.GetCurrentDirectory();
            // path = path + pathSep+@"genomes"+ pathSep;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;

        }

        static private string getGenomeImageSavePath()
        {
            string path = toFullPath(@"genomeImages");
            // string path = Directory.GetCurrentDirectory();
            //path = path + pathSep+@"genomeImages"+ pathSep;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;

        }

        static private string getGenomeDotSavePath()
        {
            string path = toFullPath(@"genomeDotFiles");
            // string path = Directory.GetCurrentDirectory();
            //path = path + pathSep+@"genomeImages"+ pathSep;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
             }
            return path;

        }


        private static void PlotGenomeGraphs(string genomeSavePath, string GenomeImageSavePath)
        {
            string[] files = Directory.GetFiles(genomeSavePath).Where(s => s.EndsWith(".genome")).ToArray();
            GridLayoutManager gridManager = new GridLayoutManager();
            NetworkControl control = new NetworkControl();
            NetworkModel model;
            for (int i = 0; i < files.Length; i++)
            {
                XmlDocument document = new XmlDocument();
                document.Load(files[i]);
                IGenome g = XmlNeatGenomeReaderStatic.Read(document);
                string name = Path.GetFileNameWithoutExtension(files[i]);
                model = GenomeDecoder.DecodeToNetworkModel((NeatGenome)g);
                gridManager.Layout(model, control.Size);
                control.NetworkModel = model;

                using (Graphics gfx = control.CreateGraphics())
                {

                    using (Bitmap bmp = new Bitmap(control.Viewport.image))
                    {
                        bmp.Save(GenomeImageSavePath + pathSep + name + ".bmp");

                    }
                }

                CPPNDotWriterStatic.saveCPPNasDOT((NeatGenome)g,getGenomeDotSavePath()+ pathSep + name + ".dot");

            }

        }
    }
}

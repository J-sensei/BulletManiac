using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BulletManiac.Tiled
{
    /// <summary>
    /// Tileset contains each tile position from tile sprite sheet
    /// </summary>
    public class Tileset
    {
        /// <summary>
        /// File store location
        /// </summary>
        private string path;
        /// <summary>
        /// Name of the file (end with tsx extension)
        /// </summary>
        private string filename;
        /// <summary>
        /// Sprite Sheet of the tileset
        /// </summary>
        private Texture2D spriteSheet;
        /// <summary>
        /// Bounding rectangle of each individual tile
        /// </summary>
        private readonly List<Rectangle> bounds = new List<Rectangle>();

        public void Load(XmlDocument xDoc)
        {
            // Get the project directory
            //string workingDirectory = Environment.CurrentDirectory;
            //string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            //string filePath = projectDirectory + "/" + path;

            //XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(filePath);
            xDoc.Save(Console.Out);
            Console.WriteLine("");

            XmlReader xReader = new XmlNodeReader(xDoc);
            while (xReader.Read())
            {
                //if(xReader.NodeType == XmlNodeType.Element && xReader.Name == "tileset")
                //{
                //    Console.WriteLine(xReader.ReadElementString());
                //}
                Console.WriteLine("Name: " + xReader.Name);
                Console.WriteLine("Value: " + xReader.Value);
                for (int attInd = 0; attInd < xReader.AttributeCount; attInd++)
                {
                    xReader.MoveToAttribute(attInd);
                    Console.WriteLine("Attribute ID: " + attInd);
                    Console.WriteLine("Attribute Name: " + xReader.Name);
                    Console.WriteLine("Attribute Value: " + xReader.Value);
                }
            }
        }
    }
}

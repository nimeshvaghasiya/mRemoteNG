﻿using mRemoteNG.Themes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Xml;
using System.Xml.Linq; 

namespace mRemoteNG.Themes 
{
    //Class to extract the rest of the required theme colors for MremoteNG from the vstheme file
    public class MremoteNGPaletteManipulator 
    {
        private XmlDocument _xml;
        private ExtendedColorPalette  _defaultPalette;

        

        //warning, defaultpalette should always contain all the values, because when is loaded there is no default palette (parameter is null
        public MremoteNGPaletteManipulator(byte[] file, ExtendedColorPalette defaultPalette = null )
        {
            _xml = new XmlDocument();
            _xml.LoadXml(new StreamReader(new MemoryStream(file)).ReadToEnd());   
            _defaultPalette = defaultPalette ?? new ExtendedColorPalette();
        }
        


        //Load the colors for the mRemoteNG own components as Dockpanel only have a menus and docks palette
        public ExtendedColorPalette getColors()
        {
            ExtendedColorPalette newPalette = new ExtendedColorPalette();
            newPalette.setDefault(_defaultPalette);
            ResourceSet resourceSet = mRemoteNG.ColorMapTheme.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            //
            foreach (DictionaryEntry entry in resourceSet)
            {
                string colorName  = entry.Key.ToString();
                String xmlQueryPath = (String)entry.Value;
                XmlNodeList colorNodeList = _xml.DocumentElement.FirstChild.SelectNodes(xmlQueryPath);
                String color = colorNodeList.Count > 0 ? colorNodeList[0].Value : null;
                if (color != null )
                {
                    newPalette.addColor(colorName , ColorTranslator.FromHtml($"#{color}"));
                }
            } 

            return newPalette;
        }  


        /// <summary>
        /// Takes a palette from memory and update the xml elements in disk
        /// </summary>
        /// <param name="colorPalette"></param>
        /// <returns></returns>
        public byte[] mergePalette(ExtendedColorPalette colorPalette) 
        {
            ResourceSet resourceSet = mRemoteNG.ColorMapTheme.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            
            foreach (DictionaryEntry entry in resourceSet)
            {
                string colorName = entry.Key.ToString();
                String xmlQueryPath = (String)entry.Value;
                XmlNodeList colorNodeList = _xml.DocumentElement.FirstChild.SelectNodes(xmlQueryPath);
                if(colorNodeList.Count > 0)
                {
                    Color paletteColor = colorPalette.getColor(colorName);
                    colorNodeList[0].Value = string.Format("FF{0:X2}{1:X2}{2:X2}", paletteColor.R, paletteColor.G, paletteColor.B);
                }
              
            }
            MemoryStream ms = new MemoryStream();
            _xml.Save(ms);
            byte[] bytes = ms.ToArray();

            return bytes;
        }

    }
}
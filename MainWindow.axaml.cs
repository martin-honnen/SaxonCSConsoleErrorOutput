using Avalonia.Controls;
using Avalonia.Interactivity;
using Saxon.Api;
using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace SaxonCSConsoleErrorOutput
{
    public partial class MainWindow : Window
    {
        private static Processor processor = new Processor();
        public MainWindow()
        {
            InitializeComponent();
        }

        //private void netParsing_Clicked(object sender, RoutedEventArgs e)
        //{
        //    var inputString = XmlCode.Text;

        //    using var inputStringReader = new StringReader(inputString);

        //    var xmlDocument = new XmlDocument();

        //    try
        //    {
        //        xmlDocument.Load(inputStringReader);
        //        Result.Text = xmlDocument.OuterXml;
        //    }
        //    catch (XmlException ex)
        //    {
        //        Result.Text = $"XML parsing failed: {ex.Message}";
        //    }
        //}

        //private void saxonParsing_Clicked(object sender, RoutedEventArgs e)
        //{
        //    var inputString = XmlCode.Text;

        //    using var inputStringReader = new StringReader(inputString);

        //    var docBuilder = processor.NewDocumentBuilder();
        //    docBuilder.BaseUri = new Uri("urn:from-string");

        //    try
        //    {
        //        var xdmDocument = docBuilder.Build(inputStringReader);
        //        Result.Text = xdmDocument.OuterXml;
        //    }
        //    catch (SaxonApiException ex)
        //    {
        //        Result.Text = $"XML parsing failed: {ex.Message}";
        //    }

        //}

        private void netXslt_Clicked(object sender, RoutedEventArgs e)
        {
            var inputString = XmlCode.Text;

            using var inputStringReader = new StringReader(inputString);

            XPathDocument input;
            try
            {
                input = new XPathDocument(inputStringReader);
            }
            catch (XmlException ex)
            {
                Result.Text = $"XML parsing failed: {ex.Message}";
                return;
            }

            var xsltProcessor = new XslCompiledTransform();

            var xsltString = XsltCode.Text;

            using var xsltStringReader = new StringReader(xsltString);

            using var xsltReader = XmlReader.Create(xsltStringReader);

            try
            {
                xsltProcessor.Load(xsltReader);
            }
            catch (XsltException ex)
            {
                Result.Text = $"XSLT compilation failed: {ex.Message}";
                return;
            }

            using var resultWriter = new StringWriter();

            try
            {
                xsltProcessor.Transform(input, null, resultWriter);
                Result.Text = resultWriter.ToString();
            }
            catch (XsltException ex)
            {
                Result.Text = $"XSLT excecution failed: {ex.Message}";
            }

        }
        private void saxonXslt_Clicked(object sender, RoutedEventArgs e)
        {
            var inputString = XmlCode.Text;

            using var inputStringReader = new StringReader(inputString);

            var docBuilder = processor.NewDocumentBuilder();

            XdmNode input;
            try
            {
                input = docBuilder.Build(inputStringReader);
            }
            catch (SaxonApiException ex)
            {
                Result.Text = $"XML parsing failed: {ex.Message}";
                return;
            }

            var xsltCompiler = processor.NewXsltCompiler();
            xsltCompiler.BaseUri = new Uri("urn:from-string");

            var xsltString = XsltCode.Text;

            using var xsltStringReader = new StringReader(xsltString);

            XsltExecutable xsltExecutable;

            try
            {
                xsltExecutable = xsltCompiler.Compile(xsltStringReader);
            }
            catch (SaxonApiException ex)
            {
                Result.Text = $"XSLT compilation failed: {ex.Message}";
                return;
            }

            var xslt30Transformer = xsltExecutable.Load30();

            xslt30Transformer.GlobalContextItem = input;

            using var resultWriter = new StringWriter();

            try
            {
                xslt30Transformer.ApplyTemplates(input, processor.NewSerializer(resultWriter));
                Result.Text = resultWriter.ToString();
            }
            catch (SaxonApiException ex)
            {
                Result.Text = $"XSLT excecution failed: {ex.Message}";
            }

        }
    }
}
namespace XkcdViewer.Forms.NetStandard.Models
{
    /// <summary>
    /// Extrapolation of platform dependant file systems
    /// </summary>
    public interface ISaveAndLoad
    {
        /// <summary>
        /// Method that will save text to a file
        /// </summary>
        /// <param name="fileName">Name of the text file</param>
        /// <param name="fileContent">Content of the text file. Can also be json data.</param>
        void SaveText(string fileName, string fileContent);

        /// <summary>
        /// Method to load text from a file
        /// </summary>
        /// <param name="fileName">Name of file to load</param>
        /// <returns></returns>
        string LoadText(string fileName);
    }
}
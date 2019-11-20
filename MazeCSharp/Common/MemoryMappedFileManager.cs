using System;
using System.Collections.Concurrent;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Common
{
    public class MemoryMappedFileManager
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        public ConcurrentStack<MemoryMappedFile> MemoryMappedFileCollection { get; set; }
        #endregion Properties..

        #region Constructors..
        public MemoryMappedFileManager()
        {
            MemoryMappedFileCollection = new ConcurrentStack<MemoryMappedFile>();
        }
        #endregion Constructors..

        #region Methods..
        public void CreateNewMappedFile(string fileContent)
        {
            Guid UniqueFileId = Guid.NewGuid();
            var MappedFile = MemoryMappedFile.CreateNew(UniqueFileId.ToString(), fileContent.Length * 2);
            
            using (var viewAcessor = MappedFile.CreateViewStream())
            {
                byte[] Bytes = Encoding.ASCII.GetBytes(fileContent);
                viewAcessor.Write(Bytes, 0, Bytes.Length);
            }

            MemoryMappedFileCollection.Push(MappedFile);
        }

        /// <summary>
        /// Gets the file content from the next mapped file on the stack as a string.
        /// Results null on an empty stack.
        /// </summary>
        /// <returns></returns>
        public string GetFileContent()
        {
            string Result = null;

            if (MemoryMappedFileCollection.TryPop(out MemoryMappedFile memoryMappedFile))
            {
                using (var viewStream = memoryMappedFile.CreateViewStream())
                {
                    byte[] Bytes = new byte[viewStream.Capacity];
                    viewStream.Read(Bytes, 0, Bytes.Length);
                    Result = Encoding.UTF8.GetString(Bytes);
                }

                memoryMappedFile.Dispose();
            }

            return Result;
        }
        #endregion Methods..
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkStorage.Application.DTOs.VkFileDtos
{
    internal class FileStreamDelete : FileStream
    {
        readonly string path;

        public FileStreamDelete(string path, FileMode mode) : base(path, mode) // NOTE: must create all the constructors needed first
        {
            this.path = path;
        }

        protected override void Dispose(bool disposing) // NOTE: override the Dispose() method to delete the file after all is said and done
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
        }
    }
}

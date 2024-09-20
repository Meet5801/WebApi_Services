using System.ComponentModel.DataAnnotations;

namespace Crud_Operation.Model
{
    public class UploadedFile
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}

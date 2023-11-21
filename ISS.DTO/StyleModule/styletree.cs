
using System.Collections.Generic;
namespace StyleNavigator.DTO.StyleModule
{
    public class StyleTree
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public int Level { get; set; }
        public bool HasChildren
        {
            get
            {
                return (this.Children > 0);
            }
        }
        public long Children { get; set; }

        public string StyleCode { get { return Id.Split('|')[0]; } }
        public string ColorCode { get { return Id.Split('|')[1]; } }
        public string AttributeCode { get { return Id.Split('|')[2]; } }
        public string SizeCode { get { return Id.Split('|')[3]; } }
        public string Url { get { return GetUrl(Id.Split('|')); } }

        private string GetUrl(string[] parameters)
        {
            return string.Format("Home/MySearch/{0}/{1}/{2}/{3}", parameters[0], parameters[1], parameters[2], parameters[3]);
        }
    }
}
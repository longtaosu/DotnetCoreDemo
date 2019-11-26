using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitmqMVCDemo.Models
{
    public class Question
    {
        public string Text { get; set; }

        public Question(string text)
        {
            Text = text;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XDNet
{
    public class EmojiLoader
    {
        public static Random Rand = new Random();

        public event EventHandler<string> FileNotFound;

        string[] EmojiStrings { get; set; }

        public EmojiLoader()
        {

        }

        public EmojiLoader(string filename)
        {
            Load(filename);
        }

        public void Load(string filename)
        {
            try
            {
                EmojiStrings = File.ReadAllLines(filename);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                FileNotFound?.Invoke(this, filename);
                EmojiStrings = DefaultEmojis;
            }
        }

        public string GetRandomEmoji()
        {
            return EmojiStrings == null ? "" : EmojiStrings[Rand.Next(0, EmojiStrings.Length - 1)];
        }

        public static readonly string[] DefaultEmojis = new string[] 
        {
            "(｡◕‿◕｡)",
            "(`･ω･´)",
            "（ ^_^）o自自o（^_^ ）",
            "⊂(◉‿◉)つ",
            "q(❂‿❂)p",
            "°‿‿°",
            "(づ｡◕‿‿◕｡)づ",
            "乁( ◔ ౪◔)「      ┑(￣Д ￣)┍",
            "♥‿♥",
            "ԅ(≖‿≖ԅ)",
            "( ˘ ³˘)♥ ",
            "♪♪ ヽ(ˇ∀ˇ )ゞ",
            "(☞ﾟヮﾟ)☞",
            "ƪ(ړײ)‎ƪ​​",
            "(⊃｡•́‿•̀｡)⊃",
            "ლ(=ↀωↀ=)ლ",
            "]*ΦωΦ)ノ",
            "(=^-ω-^=)",
            "ヽ(=^･ω･^=)丿",
            "o(^・x・^)o",
            "(^･ｪ･^)",
            "ヽ(^‥^=ゞ)",
            "(^・ω・^ )",
            "=＾● ⋏ ●＾="
        };

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XDNet
{
    public class EmojiLoader
    {
        string[] EmojiStrings { get; set; }

        public EmojiLoader(string filename)
        {
            try
            {
                EmojiStrings = File.ReadAllLines(filename);
            }
            catch (Exception)
            {
                MessageBox.Show($"Can't find emoji file {filename}! Using default emojis instead!", "XDNet - File error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmojiStrings = DefaultEmojis;
            }
        }

        static Random rng = new Random();

        public string GetRandomEmoji()
        {
            return EmojiStrings == null ? "" : EmojiStrings[rng.Next(0, EmojiStrings.Length - 1)];
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

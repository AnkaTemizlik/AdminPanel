using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DNA.Domain.Utils {
    public static class Utils {
        public static string Pijama() => "Pijamalı hasta yağız şoföre çabucak güvendi.";

        public static string Lorem(int words) {
            var result = string.Join(' ', _lorem.Split(' ').OrderBy(c => Guid.NewGuid()).Take(words));
            return result.Substring(0, 1).ToUpper() + result.Substring(1, result.Length - 1);
        }

        static string _lorem = "lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua eu facilisis sed odio morbi quis commodo odio aenean facilisis magna etiam tempor orci eu lobortis elementum nibh tellus tincidunt tortor aliquam nulla facilisi cras in massa tempor nec feugiat integer malesuada nunc vel risus commodo viverra maecenas accumsan malesuada fames ac turpis egestas sed nisl rhoncus mattis rhoncus urna neque viverra ipsum dolor sit amet consectetur adipiscing elit pellentesque habitant id nibh tortor id aliquet imperdiet nulla malesuada pellentesque elit eget gravida purus in mollis nunc sed id semper faucibus vitae aliquet nec ullamcorper sit amet risus vel facilisis volutpat est velit sed euismod nisi porta lorem mollis quis blandit turpis cursus in hac habitasse hendrerit gravida rutrum quisque non tellus fermentum leo vel orci porta commodo ullamcorper a lacus vestibulum sed arcu non odio euismod senectus et netus et malesuada fames ac turpis tellus pellentesque eu tincidunt tortor convallis a cras semper auctor neque vitae tempus quam sit amet nisl suscipit adipiscing bibendum est ultricies integer lorem dolor sed viverra ipsum nunc aliquet bibendum vitae justo eget magna fermentum ac turpis egestas sed tempus urna et pulvinar mattis nunc sed blandit consequat mauris nunc congue nisi vitae etiam sit amet nisl purus in mollis nunc sed id ipsum suspendisse ultrices gravida dictum arcu cursus euismod quis viverra nibh cras pulvinar mattis elementum integer enim neque volutpat ac tincidunt viverra adipiscing at in tellus varius vel pharetra vel turpis nunc eget lorem quis vel eros donec ac odio tempor orci pulvinar mattis nunc sed blandit libero volutpat sed cras ornare lobortis scelerisque fermentum dui faucibus in ornare";

        public static string Word(int requestedLength) {
            Random rnd = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
            string[] vowels = { "a", "e", "i", "o", "u" };

            string word = "";

            if (requestedLength == 1) {
                word = GetRandomLetter(rnd, vowels);
            }
            else {
                for (int i = 0; i < requestedLength; i += 2) {
                    word += GetRandomLetter(rnd, consonants) + GetRandomLetter(rnd, vowels);
                }

                //word = word.Replace("q", "qu").Substring(0, requestedLength); // We may generate a string longer than requested length, but it doesn't matter if cut off the excess.
            }

            return word;
        }

        private static string GetRandomLetter(Random rnd, string[] letters) {
            return letters[rnd.Next(0, letters.Length - 1)];
        }

    }
}

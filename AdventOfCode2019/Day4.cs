using System;
using System.Collections.Generic;

namespace Day4
{
    class Day4Code
    {
        public void main()
        {
            //string puzzleInput = "382345-843167";
            // Six digit password
            // within range of puzzle so min = 382345, max = 843167
            // there are 2 digits that are the same that are next to each other
            // never decreases as continues, must increase or stay the same

            // The number of valid passwords

            int smallest = 382345;
            int largest = 843167;

            int validPasswordCount = 0;


            for (int count = 0; count < largest - smallest; count++)
            {

                bool containsValidDouble = false;
                bool ok = false;

                int currentNumInt = smallest + count;
                string currentNumStr = currentNumInt.ToString();

                char previous2 = '#';
                char previous = '#';

                List<char> ofDouble = new List<char>();
                List<char> ofTriple = new List<char>();
                List<char> validDoubles = new List<char>();

                foreach (char element in currentNumStr)
                {
                    //must contain double adjacent numbers

                    if (previous == element && previous == previous2)
                    {
                        ofTriple.Add(element);
                        foreach (char doubleChar in ofDouble)
                        {
                            if (element == doubleChar) { if (validDoubles.Contains(element)) { validDoubles.Remove(element); } }
                        }
                    }

                    else if (previous == element)
                    {
                        if (!validDoubles.Contains(element))
                        {
                            ofDouble.Add(element);
                            int why = 0;
                            foreach (char tripleChar in ofTriple)
                            {
                                if (element == tripleChar) { ofDouble.Remove(element); why = -1; }
                            }
                            if (why != -1) { validDoubles.Add(element); }
                        }



                    }

                    // if the password is only increasing
                    if (previous <= element) { ok = true; }
                    else { ok = false; break; }

                    previous2 = previous;
                    previous = element;

                }
                if (validDoubles.Count > 0) { containsValidDouble = true; }
                else { containsValidDouble = false; }
                if (ok && containsValidDouble) { Console.WriteLine(currentNumInt); validPasswordCount += 1; }

            }

            Console.WriteLine("The number of valid passwords : {0}", validPasswordCount);

        }
    }
}
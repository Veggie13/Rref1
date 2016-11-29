using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rref
{
    class Program
    {
        /*
        function ToReducedRowEchelonForm(Matrix M) is
            lead := 0
            rowCount := the number of rows in M
            columnCount := the number of columns in M
            for 0 ≤ r < rowCount do
                if columnCount ≤ lead then
                    stop
                end if
                i = r
                while M[i, lead] = 0 do
                    i = i + 1
                    if rowCount = i then
                        i = r
                        lead = lead + 1
                        if columnCount = lead then
                            stop
                        end if
                    end if
                end while
                Swap rows i and r
                If M[r, lead] is not 0 divide row r by M[r, lead]
                for 0 ≤ i < rowCount do
                    if i ≠ r do
                        Subtract M[i, lead] multiplied by row r from row i
                    end if
                end for
                lead = lead + 1
            end for
        end function*/
        static void SwapRows<T>(T[,] m, int r1, int r2)
        {
            if (r1 == r2)
                return;
            for (int c = 0; c < m.GetLength(1); c++)
            {
                T temp = m[r1, c];
                m[r1, c] = m[r2, c];
                m[r2, c] = temp;
            }
        }

        static void DivideRow(Expression[,] m, int r, Expression v)
        {
            for (int c = 0; c < m.GetLength(1); c++)
            {
                m[r, c] = m[r, c] / v;
            }
        }

        static void SubtractScaledRow(Expression[,] m, int r, int i, Expression v)
        {
            for (int c = 0; c < m.GetLength(1); c++)
            {
                m[i, c] = m[i, c] - (v * m[r, c]);
            }
        }

        static void ToRREF(Expression[,] m)
        {
            int lead = 0;
            int rowCount = m.GetLength(0);
            int columnCount = m.GetLength(1);
            for (int r = 0; r < rowCount; r++)
            {
                if (columnCount <= lead) return;
                int i = r;
                while (m[i, lead] == 0.0)
                {
                    i++;
                    if (rowCount == i)
                    {
                        i = r;
                        lead++;
                        if (columnCount == lead) return;
                    }
                }
                SwapRows(m, i, r);
                if (m[r, lead] != 0.0)
                {
                    DivideRow(m, r, m[r, lead]);
                }
                for (i = 0; i < rowCount; i++)
                {
                    if (i != r)
                    {
                        SubtractScaledRow(m, r, i, m[i, lead]);
                    }
                }
                lead++;
            }
        }

        static void Main(string[] args)
        {
            Variable U = new Variable('U');
            Variable V = new Variable('V');
            Variable W = new Variable('W');
            Variable X = new Variable('X');
            Variable Y = new Variable('Y');
            Variable Z = new Variable('Z');

            Expression[,] M = new Expression[,] {
                {U, Z, Y, X, W, V, 2},
                {V, U, Z, Y, X, W, 2},
                {W, V, U, Z, Y, X, 2},
                {X, W, V, U, Z, Y, 2},
                {Y, X, W, V, U, Z, 2},
                {Z, Y, X, W, V, U, 1}};

            DateTime start = DateTime.UtcNow;
            ToRREF(M);
            DateTime end = DateTime.UtcNow;

            Console.WriteLine("Time elapsed: {0}", end - start);
            Console.Write("Press any key...");
            Console.ReadLine();
        }
    }
}

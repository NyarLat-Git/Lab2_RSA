using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Lab2_RSA.Utils;

namespace Lab2_RSA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";

        }

        private void button1_Click(object sender, EventArgs e)
        {

            int digits = 20;
            if (textBox1.Text != "")
                digits = Convert.ToInt32(textBox1.Text);

            BigInteger min = BigInteger.Pow(10, digits - 1); // диапазон, в котором будем искать p и q, чтобы они содержали ровно digits цифр
            BigInteger max = BigInteger.Subtract(BigInteger.Pow(10, digits), BigInteger.One);
            BigInteger p;
            do
            {
                // находим ближайшее простое число p ≥ randP
                BigInteger randP = BigInteger.Add(RandomBigInteger(BigInteger.Add(BigInteger.One, BigInteger.Subtract(max, min))), min);
                p = FindNextPrimeM(randP);
            } while (p.ToString().Length != digits);

            BigInteger q;
            do
            {
                // находим ближайшее простое число q ≥ randQ
                BigInteger randQ = BigInteger.Add(RandomBigInteger(BigInteger.Add(BigInteger.One, BigInteger.Subtract(max, min))), min);
                q = FindNextPrimeM(randQ);
            } while (q.ToString().Length != digits);

            textBox2.Text = p.ToString();
            textBox3.Text = q.ToString();

            BigInteger n = BigInteger.Multiply(p, q);
            int eDigits = (digits * 2 + 1) / 2; // нахождение количества цифр в ee (в 2 раза меньше, округляем вверх)
            BigInteger minE = BigInteger.Pow(10, eDigits - 1);
            BigInteger maxE = BigInteger.Subtract(BigInteger.Pow(10, eDigits), BigInteger.One);
            BigInteger ee;
            do
            {
                // ee, не больше n и с нужным количеством цифр
                ee = BigInteger.Add(RandomBigInteger(BigInteger.Add(BigInteger.One, BigInteger.Subtract(maxE, minE))), minE);
            } while (ee.CompareTo(n) >= 0 || ee.ToString().Length != eDigits);

            textBox4.Text = ee.ToString();
        


    }
}
}

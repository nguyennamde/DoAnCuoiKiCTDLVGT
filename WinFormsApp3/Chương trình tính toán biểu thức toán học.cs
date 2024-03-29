﻿using System.Data;
using System;
using System.Text.RegularExpressions;
using System.Text;
namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void click(object sender, EventArgs e)
        {
            try
            {
                Button button = sender as Button;
                txt.Text += button.Text;
                if (button.Text == "=")
                {
                    double result = Calculator.Evaluate(txt.Text);
                    label2.Text = Convert.ToString(result);
                    txt.Text = txt.Text.Remove(txt.Text.Length - 1);
                }
                else if (button.Text == "DEL")
                    txt.Text = txt.Text.Remove(txt.Text.Length - 4);
                else if (button.Text == "AC")
                    txt.Text = "";
            }
            catch { label2.Text="Có lỗi xảy ra. Vui lòng nhập lại biểu thức:"; }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = "";
        }
        // Lớp Node
        public class Node
        {
            public Node next;
            public object data;
        }
    // Lớp Stack
        public class Stack
        {
            Node top;
            public bool IsEmpty()
            {
                if (top == null)
                    return true;
                return false;
            }
            public void Push(object ele)
            {
                Node n = new Node();
                n.data = ele;
                n.next = top;
                top = n;
            }
            public Node Pop()
            {
                if (top == null)
                    return null;
                Node d = top.next;
                return d;
            }
            public object Peek(int index)
            {
                Stack temp = new Stack();
                object data = 0;
                int i = -1;
                do
                {
                    data = Pop().data;
                    temp.Push(data);
                    i++;
                }
                while (i != index);
                while (!temp.IsEmpty())
                    Push(temp.Pop().data);
                return data;
            }
        }

        public class Calculator {
            public static void FormatExpression(ref string expression)
            {
                expression = expression.Replace(" ", "");
                expression = Regex.Replace(expression, @"\+|\-|\*|\/|\)|\(", delegate (Match match)
                {
                    return " " + match.Value + " ";
                });
                expression = expression.Replace("  ", " ");
                expression = expression.Trim();
            }
            public static double Evaluate(string expression)
            {
                FormatExpression(ref expression);
                expression = expression.Trim();
                char[] tokens = expression.ToCharArray();
                // stack chứa toán hạng
                Stack<double> operands = new Stack<double>();
                // stack chứa toán tử
                Stack<char> operators = new Stack<char>();
                for (int i = 0; i < tokens.Length; i++)
                {
                    // Nếu gặp ký tự chữ số sẽ quét hết các ký tự chữ số tiếp theo,
                    // chuyển đổi nó về kiểu int và push vào stack cho toán hạng
                    if (tokens[i] >= '0' && tokens[i] <= '9' || tokens[i] == '.')
                    {
                        StringBuilder digitBuffer = new StringBuilder();
                        // quét các ký tự chữ số tiếp theo  
                        while (i < tokens.Length && tokens[i] >= '0' && tokens[i] <= '9' || tokens[i] == '.')
                        {
                            digitBuffer.Append(tokens[i++]);
                        }
                        operands.Push(double.Parse(digitBuffer.ToString()));
                    }
                    // Nếu gặp dấu mở ngoặc thì push vào stack toán tử
                    else if (tokens[i] == '(')
                    {
                        operators.Push(tokens[i]);
                    }
                    // Nếu gặp dấu đóng ngoặc thì phải giải quyết mớ ở trong ngoặc                
                    else if (tokens[i] == ')')
                    {
                        while (operators.Peek() != '(')
                        {
                            operands.Push(Calculate(operators.Pop(), operands.Pop(), operands.Pop()));
                        }
                        operators.Pop();
                    }
                    // Nếu gặp ký tự toán tử (+,-,*,/)
                    else if (tokens[i] == '+' || tokens[i] == '-' || tokens[i] == '*' || tokens[i] == '/')
                    {
                        while (operators.Count > 0 && HasPrecedence(tokens[i], operators.Peek()))
                        {
                            operands.Push(Calculate(operators.Pop(), operands.Pop(), operands.Pop()));
                        }
                        // Push toán tử vào stack dành cho nó
                        operators.Push(tokens[i]);
                    }
                }
                // Đã kết thúc giai đoạn phân tích chuỗi
                // Bắt đầu thực hiện phép toán
                while (operators.Count > 0)
                {
                    operands.Push(Calculate(operators.Pop(), operands.Pop(), operands.Pop()));
                }
                return operands.Pop();
            }
            // So sánh độ ưu tiên của các phép toán
            public static bool HasPrecedence(char op1, char op2)
            {
                if (op2 == '(' || op2 == ')')
                {
                    return false;
                }
                if ((op1 == '*' || op1 == '/') && (op2 == '+' || op2 == '-')) 
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            // Thực hiện các phép toán cơ bản
            public static double Calculate(char @operator, double b, double a)
            {
                switch (@operator)
                {
                    // Thực hiện phép toán cộng nếu gặp ký tự +
                    case '+':
                        return a + b;
                    // Thực hiện phép toán trừ nếu gặp ký tự -
                    case '-':
                        return a - b;
                    // Thực hiện phép toán nhân nếu gặp ký tự *
                    case '*':
                        return a * b;
                    // Thực hiện phép toán chia nếu gặp ký tự /
                    case '/':
                        return a / b;
                }
                // Nếu không duyệt qua toán tử nào thì mặc định trả về giá trị 0
                return 0;
            }
        }
    }
}
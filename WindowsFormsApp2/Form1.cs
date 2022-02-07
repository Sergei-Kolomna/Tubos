using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{


	public partial class Form1 : Form
	{

		public class My_defect
		{
			public int x=0; //координата x начала точки
			public int y=0; //координата y начала точки
			public int len_x=0; //длина по иксу
			public int len_y=0; //длина по игреку
		}


		double[,] values1;  //тут храним считанный массив
		int XSize;          //размерность по Х
		int Ysize;          // разерность по Y
		Bitmap my_bitmap;
		List<string> verified = new List<string>(); //тут храним координаты уже проверенных точек
		int count_big=0;

		public Form1()
		{
			InitializeComponent();
		}

		static double find_max(double[,] array)
		{
			double max = 0;
			foreach (var item in array)
			{
				if (item > max) max = item;
			}
			return max;
		}

		int find_needed_rows(double[,] array, int tres)
		{
			bool bb = false;
			int count = 0;
			for (int i = 0; i < array.GetLength(0); i++)
			{
				bb = false;
				for (int j = 0; j < array.GetLength(1); j++)
				{
					if (array[i, j] >= tres) bb = true;
				}
				if (bb == true)
				{
					count++;
					bb = false;
				}
			}
			return count;
		}

		int find_needed_columns(double[,] array, int tres)
		{
			bool bb = false;
			int count = 0;
			for (int i = 0; i < array.GetLength(1); i++)
			{
				bb = false;
				for (int j = 0; j < array.GetLength(0); j++)
				{
					if (array[j, i] >= tres) bb = true;
				}
				if (bb == true)
				{
					count++;
					bb = false;
				}
			}
			return count;
		}



		My_defect [] find_defect(double[,] array, int threshold)
		{
			My_defect dd = new My_defect();
			My_defect[] ddd = new My_defect[100];
			int resolution = 200;
			int count_y = 2;
			int count_x = 2;
			int count_y2 = 2;
			int count_x2 = 2;
			int count_dd = 0;
			int sum = 1;
			int sum1 = 0;
			int sum2 = 1;
			int sum3 = 0;
			double[,] sub_array = new double[resolution, resolution];
			double[,] sub_array2 = new double[resolution, resolution];
			bool full = false;

			for (int x = 0; x < array.GetLength(0) - 2; x++)
			{
				for (int y = 0; y < array.GetLength(1) - 2; y++)
				{
					if ((array[x, y] > threshold) & !verified.Contains(Convert.ToString(String.Concat(Convert.ToString(x),",",Convert.ToString(y)))))                                    //нашли точку выше порога
					{
						ddd[count_dd] = new My_defect();
						while (sum - sum1 > 0)
						{
							//ddd[count_dd]=;
							sum1 = sum;
							sum = 0;
							for (var i = 0; i < count_x; i++)                       //заполняю подмассив
							{
								for (var j = 0; j < count_y; j++)
								{
									if ((y + j) < array.GetLength(1) & (x + i) < array.GetLength(0))
									{
										sub_array[i, j] = array[x + i, y + j];
										verified.Add(Convert.ToString(x + i)+","+ Convert.ToString(y + j));
										sub_array2[i, j] = array[x + i, y - j];
										verified.Add(Convert.ToString(x + i) + "," + Convert.ToString(y - j));
									}
									else
									{
										full = true;
										break;
									}
								}
							}

							foreach (double nn in sub_array)						//проверяем увеличилось ли количество членов первого подмассива больше порога
							{
								if (nn > threshold) sum++;
							}
							count_x++;
							count_y++;
							if (count_x > 195 | count_y > 195)
							{
								MessageBox.Show("Большая область", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Hand);
								full = true;
								break;
							}
							if (full) break;
						}
						//смотрим вверх
						while (sum2 - sum3 > 0)
						{
							sum3 = sum2;
							sum2 = 0;
							for (var i = 0; i < count_x2; i++)                       //заполняю подмассив
							{
								for (var j = 0; j < count_y2; j++)
								{
									if ((y - j) < array.GetLength(1) & (x + i) < array.GetLength(0))
									{
										sub_array2[i, j] = array[x + i, y - j];
										verified.Add(Convert.ToString(x + i) + "," + Convert.ToString(y - j));
									}
									else
									{
										full = true;
										break;
									}
								}
							}

							foreach (double nn in sub_array2)                        //проверяем увеличилось ли количество членов первого подмассива больше порога
							{
								if (nn > threshold) sum2++;
							}
							count_x2++;
							count_y2++;
							if (count_x2 > 195 | count_y2 > 195)
							{
								MessageBox.Show("Большая область2", "Внимание2", MessageBoxButtons.OK, MessageBoxIcon.Hand);
								full = true;
								break;
							}
							if (full) break;
						}

						int len_x1 = find_needed_columns(sub_array, threshold);
						int len_x2 = find_needed_columns(sub_array2, threshold);
						ddd[count_dd].len_x = len_x1+len_x2;                            //задаем размеры и координаты области

						int len_y1 = find_needed_rows(sub_array, threshold);
						int len_y2 = find_needed_rows(sub_array2, threshold);
						if (len_y1 > len_y2) ddd[count_dd].len_y = len_y1;
						else ddd[count_dd].len_y = len_y2;
						ddd[count_dd].x = x;
						ddd[count_dd].y = y-len_y2;
						Array.Clear(sub_array,0,10000);
						sum = 1;
						sum1 = 0;
						sum2 = 1;
						sum3 = 0;
						//x = x + count_x;
						//y = y + count_y;
						count_dd++;
						if (full) count_big++;
						if (full) break;
					}
					if (full) break;
				}
				//if (full) break;
			}
			if (full) count_big++;
			return ddd;
		}

		private void button1_Click(object sender, EventArgs e) // рисуем картинку
		{

			my_bitmap = new Bitmap(XSize, Ysize);
			int curr_color = 0;

			double maxx = find_max(values1);

			for (var x = 0; x < my_bitmap.Width; x++)
			{
				for (var y = 0; y < my_bitmap.Height; y++)
				{
					curr_color = Convert.ToInt32(250 * Math.Abs(Math.Floor(values1[y, x]) / maxx));
					my_bitmap.SetPixel(x, y, Color.FromArgb(curr_color, 255 - curr_color, 255 - curr_color)); // с помощью SetPixel задаём цвет отдельного пикселя
				}
			}
			pictureBox1.Image = my_bitmap;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var fileContent = string.Empty;
			var filePath = string.Empty;

			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = "c:/A/";
				openFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
				openFileDialog.FilterIndex = 2;
				openFileDialog.RestoreDirectory = true;

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					//Get the path of specified file
					filePath = openFileDialog.FileName;

					//Read the contents of the file into a stream
					var fileStream = openFileDialog.OpenFile();

					using (StreamReader reader = new StreamReader(fileStream))
					{
						fileContent = reader.ReadToEnd();
					}
					button3.Enabled = true;
					button1.Enabled = true;
				}
			}

			// Получить текст файла.
			string whole_file = System.IO.File.ReadAllText(filePath);

			// Разделение на строки.
			whole_file = whole_file.Replace('\n', '\r');
			string[] lines = whole_file.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

			// Посмотрим, сколько строк и столбцов есть.
			int num_rows = lines.Length;
			int num_cols = lines[0].Split(',').Length;

			// Выделим массив данных.
			//string[,] values = new string[num_rows, num_cols];
			double[,] values = new double[num_rows, num_cols];

			// Загрузите массив.
			for (int r = 0; r < num_rows; r++)
			{
				string[] line_r = lines[r].Split(',');
				for (int c = 0; c < num_cols; c++)
				{
					values[r, c] = Convert.ToDouble(line_r[c], CultureInfo.GetCultureInfo("en-US"));            //тут в values хранится массив данных в строках
				}
			}
			values1 = values;                                           //передаем в общую переменную
			XSize = num_cols;
			Ysize = num_rows;
			int max = Convert.ToInt16(find_max(values));
			trackBar1.Maximum = max;
			label2.Text = max.ToString();
		}//считываем данные из файла

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			label1.Text = String.Format("Значение порога: {0}", trackBar1.Value);
		} // считываем верхний порог из trackbar

		private void button3_Click(object sender, EventArgs e)  //рисуем квадратик
		{
			Graphics g = Graphics.FromImage(pictureBox1.Image);
			Pen blackPen = new Pen(Color.Black, 1);
			My_defect[] md;

			md = find_defect(values1, trackBar1.Value);
			for (int i = 0; i < md.Length; i++)
			{
				if (md[i]!=null)
				{
					g.DrawRectangle(blackPen, md[i].y, md[i].x, md[i].len_x, md[i].len_y);   //кординаты верхнего левого угла, длина, высота
					g.DrawString(Convert.ToString(md[i].x)+","+ Convert.ToString(md[i].y), this.Font, Brushes.Black, md[i].y-35, md[i].x-15);
					g.DrawString(Convert.ToString(md[i].len_x), this.Font, Brushes.Black, md[i].y + md[i].len_x / 2-5, md[i].x + md[i].len_y+5);
					g.DrawString(Convert.ToString(md[i].len_y), this.Font, Brushes.Black, md[i].y + md[i].len_x+5, md[i].x + md[i].len_y / 2-5);
					pictureBox1.Image = my_bitmap;
				}
			}
			MessageBox.Show("Найдено "+count_big+" больших области", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}
}

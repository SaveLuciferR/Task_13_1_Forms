using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;

namespace Task_13_1_Forms
{
	public partial class Form1 : Form
	{
		Product[] products;

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			textBox1.Text = "";
			textBox2.Text = "";
			textBox3.Text = "";

			radioButton1.Checked = false;
			radioButton2.Checked = false;
			radioButton3.Checked = false;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			textBox1.Text = "ПРИМЕЧАНИЕ: Файл должен иметь следующую структуру\r\n" +
				"\r\n1 - Если товар является Игрушкой, то должен иметь следующие пункты:\r\n\tТип (Игрушки), Наименование игрушки, Цена игрушки, Производитель игрушки, Материал игрушки, Возрастная категория игрушки\r\n" +
				"2 - Если товар является Книгой, то должен иметь следующие пункты:\r\n\tТип (Книги), Наименование книги, Автор книги, Цена книги, Издтельство книги, Возрастная категория книги\r\n" +
				"3 - Если товар является Спорт-Инвентарем, то должен иметь следующие пункты:\r\n\tТип (Спорт-Инвентарь), Наименование Спорт-Инвентаря, Цена Спорт-Инвентаря, Производитель Спорт-Инвентаря, Возрастная категория Спорт-Инвентаря\r\n" +
				"4 - Если возрастная категория будет меньше или равна 0, то товар будет доступен для всех возростных лиц\r\n" +
				"5 - Если цена товара будет меньше или равна 0, то товар будет считаться бесплатным\r\n" +
				"6 - Каждый пункт о товаре должен начинаться с новой строки\r\n" +
				"7 - Файл не должен иметь лишних пропусков (лишних пустых строк)\n" +
				"8 - Путь к файлу должен иметь слудующий формат: *:\\...\\*\\...\\*.* (Без кавычек)\r\n" +
				"При не соблюдении данных правил, программа может работать некорректно!\r\n";
		}

		private void button4_Click(object sender, EventArgs e)
		{
			try
			{
				products = SetProductsFromFile();
			}
			catch (FormatException)
			{
				textBox3.Text += "Некорректные данные";
			}
			catch (Exception ex)
			{
				textBox3.Text += ex.Message;
			}
		}

		private string[] ReadFile(FileStream fs)
		{
			byte[] buffer = new byte[fs.Length];
			fs.Read(buffer, 0, buffer.Length);
			string textFromFile = Encoding.UTF8.GetString(buffer);
			string[] textSplit = textFromFile.Split('\n');

			/*for (int i = 0; i < textSplit.Length; i++)
			{
				if (textSplit[i] == "\n")
				{
					List<string> temp = new List<string>(textSplit);
					temp.RemoveAt(i);
					textSplit = temp.ToArray();

					i = i == 1 ? i : i--;
				}
			}*/

			for (int i = 0; i < textSplit.Length; i++)
			{
				textSplit[i] = textSplit[i].Trim();
			}

			return textSplit;
		}

		private Product[] SetProductsFromFile()
		{
			Product[] product = null;
			FileStream fileProducts;
			string filePath;

			filePath = textBox2.Text;
			fileProducts = new FileStream(filePath, FileMode.Open, FileAccess.Read);

			if (!fileProducts.CanRead)
			{
				throw new Exception("Введите корректный путь\r\n");
			}

			string[] textSplit = ReadFile(fileProducts);
			fileProducts.Close();

			int countProduct = 0;

			for (int i = 0; i < textSplit.Length; i++)
			{
				if (textSplit[i] == Product.productType[0] || textSplit[i] == Product.productType[1] || textSplit[i] == Product.productType[2])
				{
					countProduct++;
				}
			}

			if (countProduct == 0)
			{
				textBox3.Text += "\r\nВ файле нету продуктов или файл заполнен неверно!\r\n";
				return product;
			}

			product = new Product[countProduct];
			string[] countStr = new string[5];

			int indexForWhile = 0;
			int typeIndex;

			for (int i = 0; i < product.Length; i++)
			{
				typeIndex = indexForWhile;

				for (int j = indexForWhile + 1; j < textSplit.Length; j++)
				{
					if (textSplit[j] == Product.productType[0] || textSplit[j] == Product.productType[1] || textSplit[j] == Product.productType[2])
					{
						indexForWhile = j;
						break;
					}
					for (int l = 0; l < countStr.Length; l++)
					{
						if (l == 4 && textSplit[indexForWhile] == Product.productType[2]) break;
						countStr[l] = textSplit[j];
						j++;
					}
					j--;
				}

				if (textSplit[typeIndex] == "Игрушки") product[i] = new Toy(countStr[0], Convert.ToSingle(countStr[1]), countStr[2], countStr[3], Convert.ToInt32(countStr[4]));
				else if (textSplit[typeIndex] == "Книги") product[i] = new Book(countStr[0], countStr[1], Convert.ToSingle(countStr[2]), countStr[3], Convert.ToInt32(countStr[4]));
				else if (textSplit[typeIndex] == "Спорт-Инвентарь") product[i] = new SportsEquipment(countStr[0], Convert.ToSingle(countStr[1]), countStr[2], Convert.ToInt32(countStr[3]));
				else throw new Exception("Не получилось добавить товар по индексу:" + typeIndex + "\r\nПроверье правописание типа продукта\r\n");
			}

			textBox3.Text += "\r\nДанные о продуктах записаны\r\n";
			return product;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				if (products == null)
				{
					throw new Exception("Нужна запись продуктов их файла!\r\n");
				}

				textBox3.Text += "Вывод всех товаров\r\n";
				Product.GetAllProductsInfo(products, textBox3);
			}
			catch (Exception ex)
			{
				textBox3.Text += ex.Message;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				if (products == null)
				{
					throw new Exception("Нужна запись продуктов из файла!\r\n");
				}
				textBox3.Text += "Вывод товара по типу\r\n";

				if (radioButton1.Checked)
				{
					for(int i = 0; i < products.Length; i++)
					{
						if (products[i].MatchingTheType(Product.productType[0]))
						{
							products[i].GetProductInfo(textBox3);
						}
					}
				}
				else if (radioButton2.Checked)
				{
					for (int i = 0; i < products.Length; i++)
					{
						if (products[i].MatchingTheType(Product.productType[1]))
						{
							products[i].GetProductInfo(textBox3);
						}
					}
				}
				else if (radioButton3.Checked)
				{
					for (int i = 0; i < products.Length; i++)
					{
						if (products[i].MatchingTheType(Product.productType[2]))
						{
							products[i].GetProductInfo(textBox3);
						}
					}
				}
				else
				{
					throw new Exception("Радио-кнопки не выбраны\r\n");
				}
			}
			catch (Exception ex)
			{
				textBox3.Text += ex.Message;
			}
		}
	}

	abstract class Product
	{
		static public string[] productType = new string[] { "Игрушки", "Книги", "Спорт-Инвентарь" };

		static public void GetAllProductsInfo(Product[] product, TextBox textBox)
		{
			for (int i = 0; i < product.Length; i++)
			{
				product[i].GetProductInfo(textBox);
			}
		}

		abstract public void GetProductInfo(TextBox textBox);
		abstract public bool MatchingTheType(string type);
	}

	class Toy : Product
	{
		private string toyName;
		private float toyPrice;
		private string toyManufact;
		private string toyMaterial;
		private int toyDesignedForAge;

		public Toy(string toyName, float toyPrice, string toyManufact, string toyMaterial, int toyDesignedForAge)
		{
			this.toyName = toyName;
			this.toyPrice = toyPrice;
			this.toyManufact = toyManufact;
			this.toyMaterial = toyMaterial;
			this.toyDesignedForAge = toyDesignedForAge;
		}

		override public void GetProductInfo(TextBox textBox)
		{
			textBox.Text += String.Format("\r\nТип товара: {0}\r\n" +
				"Наименование товара: {1}\r\n" +
				"Цена товара: {2}\r\n" +
				"Производитель товара: {3}\r\n" +
				"Материал товара: {4}\r\n" +
				"{5}\r\n",
				"Игрушка", toyName, toyPrice <= 0 ? "Бесплатно" : toyPrice.ToString(), toyManufact, toyMaterial,
				(toyDesignedForAge <= 0 ? "Товар рассчитан для всех возростных лиц" : "Возрастная категория: " + toyDesignedForAge + "+"));
		}

		override public bool MatchingTheType(string type)
		{
			if (type == Product.productType[0])
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	class Book : Product
	{
		private string bookName;
		private string bookAuthor;
		private float bookPrice;
		private string bookPublicher;
		private int bookDesignedForAge;

		public Book(string bookName, string bookAuthor, float bookPrice, string bookPublicher, int bookDesignedForAge)
		{
			this.bookName = bookName;
			this.bookAuthor = bookAuthor;
			this.bookPrice = bookPrice;
			this.bookPublicher = bookPublicher;
			this.bookDesignedForAge = bookDesignedForAge;
		}

		override public void GetProductInfo(TextBox textBox)
		{
			textBox.Text += String.Format("\r\nТип товара: {0}\r\n" +
				"Название книги: {1}\r\n" +
				"Автор книги: {2}\r\n" +
				"Цена книги: {3}\r\n" +
				"Издательство книги: {4}\n" +
				"{5}\r\n",
				"Книга", bookName, bookAuthor, bookPrice <= 0 ? "Бесплатно" : bookPrice.ToString(), bookPublicher,
				(bookDesignedForAge <= 0 ? "Товар рассчитан для всех возростных лиц" : "Возрастная категория: " + bookDesignedForAge + "+"));
		}

		override public bool MatchingTheType(string type)
		{
			if (type == Product.productType[1])
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	class SportsEquipment : Product
	{
		private string sEquipName;
		private float sEquipPrice;
		private string sEquipManufactur;
		private int sEquipDesignedForAge;

		public SportsEquipment(string sEquipName, float sEquipPrice, string sEquipManufactur, int sEquipDesignedForAge)
		{
			this.sEquipName = sEquipName;
			this.sEquipPrice = sEquipPrice;
			this.sEquipManufactur = sEquipManufactur;
			this.sEquipDesignedForAge = sEquipDesignedForAge;
		}

		override public void GetProductInfo(TextBox textBox)
		{
			textBox.Text += String.Format("\nТип товара: {0}\r\n" +
				"Название спорт-инвентаря: {1}\r\n" +
				"Цена спорт-ивентаря: {2}\r\n" +
				"Производитель спорт-инвентаря: {3}\r\n" +
				"{4}\r\n",
				"Спорт-Инвентарь", sEquipName, sEquipPrice <= 0 ? "Бесплатно" : sEquipPrice.ToString(), sEquipManufactur,
				(sEquipDesignedForAge <= 0 ? "Товар рассчитан для всех возростных лиц" : "Возрастная категория: " + sEquipDesignedForAge + "+"));
		}

		override public bool MatchingTheType(string type)
		{
			if (type == Product.productType[2])
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

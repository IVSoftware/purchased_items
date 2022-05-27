using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace purchased_items
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if (!DesignMode)
			{
				// Bind the lists to the UI element to display them.
				dataGridView1.DataSource = PurchasesByUser;
				dataGridView2.DataSource = PurchasedByItem;
				initList1();
			}
		}
		internal BindingList<UserPurchase> PurchasesByUser { get; } = new BindingList<UserPurchase>();
		internal BindingList<PurchasedItem> PurchasedByItem { get; } = new BindingList<PurchasedItem>();

		private void initList1()
		{
			XElement xsource = XElement.Parse(source);
			XElement[] names = xsource.Elements("name").ToArray();
            foreach (XElement xuser in names)
            {
				XElement[] xpurchases = xuser.Elements("item").ToArray();
                foreach (var xpurchase in xpurchases)
                {
					// Every time you add a record to the list, it appears in the View automatically.
					PurchasesByUser.Add(new UserPurchase(name: (string)xuser, purchase: (string)xpurchase));
                }
            }
			var groups = PurchasesByUser.GroupBy(purchase => purchase.Purchase);
            foreach (var group in groups)
            {
				var byItem = new PurchasedItem(group.Key, group.Count());
				PurchasedByItem.Add(byItem);
			}
		}

        const string source =
		@"<purchases>
			<name>John Doe
				<item>Shoes</item>
				<item>T-Shirts</item>
				<item>Jeans</item>
			</name>
			<name>
				Rick Astley
				<item>Baseball bat</item>
				<item>Sandwich</item>
				<item>T-Shirts</item>
				<item>Shoes</item>
			</name>
			<name>
				Jane Doe
				<item>Shoes</item>
				<item>T-Shirts</item>
			</name>
			<name>
				Anonymous
				<item>Laptop</item>
			</name>
			<name>
				Joe Mm.
				<item>Milk</item>
				<item>Shoes</item>
			</name>
		</purchases>";
    }
	internal class UserPurchase
	{
		public UserPurchase(string name, string purchase)
        {
			Name = name;
			Purchase = purchase;
        }
		public string Name { get;  }
		public string Purchase { get;  } 
	}


	internal class PurchasedItem
    {
        public PurchasedItem(string description, int count)
        {
            Description = description;
            Count = count;
        }

        public string Description { get; }
        public int Count { get; }
    }
}

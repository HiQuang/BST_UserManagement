using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BST_UserManagement
{
    public class BinarySearchTree
    {
        public TreeNode Root { get; private set; }

        public void Insert(User user)
        {
            // Kiểm tra nếu đã tồn tại ID
            if (Search(user.ID) != null)
            {
                throw new Exception("ID đã tồn tại! Không thể thêm người dùng trùng ID.");
            }
            TreeNode newNode = new TreeNode(user);
            if (Root == null)
            {
                Root = newNode;
                return;
            }

            TreeNode current = Root;
            TreeNode parent = null;

            while (current != null)
            {
                parent = current;
                if (string.Compare(user.ID, current.Data.ID) < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }

            if (string.Compare(user.ID, parent.Data.ID) < 0)
                parent.Left = newNode;
            else
                parent.Right = newNode;
        }

        public User Search(string id)
        {
            TreeNode current = Root;
            while (current != null)
            {
                int cmp = string.Compare(id, current.Data.ID);
                if (cmp == 0)
                    return current.Data;
                else if (cmp < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }
            return null;
        }

        public void DrawTree(Graphics g, Font font, Brush brush, Pen pen, int width)
        {
            if (Root != null)
            {
                int centerX = width / 2;
                int startY = 60;
                int horizontalSpacing = 80; // khoảng cách ngang giữa các node

                DrawNode(g, Root, centerX, startY, horizontalSpacing, font, brush, pen);
            }
        }

        private void DrawNode(Graphics g, TreeNode node, int x, int y, int offset, Font font, Brush brush, Pen pen)
        {
            if (node == null) return;

            Rectangle rect = new Rectangle(x - 40, y, 80, 40);
            g.FillEllipse(brush, rect);
            g.DrawEllipse(pen, rect);
            g.DrawString(node.Data.ID, font, Brushes.Black, x - 30, y + 10);

            if (node.Left != null)
            {
                g.DrawLine(pen, x, y + 40, x - offset, y + 80);
                DrawNode(g, node.Left, x - offset, y + 80, offset / 2, font, brush, pen);
            }

            if (node.Right != null)
            {
                g.DrawLine(pen, x, y + 40, x + offset, y + 80);
                DrawNode(g, node.Right, x + offset, y + 80, offset / 2, font, brush, pen);
            }
        }

    }
}

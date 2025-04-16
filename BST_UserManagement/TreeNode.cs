using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BST_UserManagement
{
    public class TreeNode
    {
        public User Data { get; set; }
        public TreeNode Left { get; set; }
        public TreeNode Right { get; set; }

        public TreeNode(User user)
        {
            Data = user;
            Left = null;
            Right = null;
        }
    }
}

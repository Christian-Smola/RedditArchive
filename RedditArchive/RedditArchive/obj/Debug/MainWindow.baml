   M S B A M L   `   `   `  ����H  DRedditArchive, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:RedditArchiveRedditArchive  5   	       RedditArchive.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:RedditArchive     5   	   -   5          � Title$  Reddit Archive��5   	   $	��450��6    $	��800��6-    � 
ResizeMode$ CanMinimize=�6Y   .��5	      � .+�5
   
   �� $��Displayed Subreddit:��6   $
��Left=�6/   $��	186,8,0,0q�6J   $	��Top=�6]   �� 5   
   -   5         ��Name$ cboDisplayedSubreddit��5      $��Center=�62   $��0,10,0,0q�6O   $	��Top=�6a   $	��180��6y   6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ 
MyDataGrid��5      $��10,37,10,35q�6'      AutoGenerateColumns .     CanUserAddRows .     AlternatingRowBackground
 ����� � 	Resources 5      =  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType	 �� Property
 �� Value��%�   '        �� 5      "  6   .�5      �� 	 � ��$
 14(�61   �� 5      	 � ��$
 24��6/   �� 5      	 � ��
 �	#FFBDBDBD63   �� 5      	 � ��$
 
0, 0, 1, 1q�68   �� 5      	 � ��

 �   �64   5      5         Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Title��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$	 160 6N   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *Title+6N     5      $
 Text��6%   $	 160 6L    63   �� *Text+6L     5      $ 	Subreddit��6%   $
 Auto 6V    68   �� *	Subreddit+6V     5      $ Poster��6%   $
 Auto 6P    65   �� *Poster+6P     5      $ Score��6%   $
 Auto 6N    64   �� *Score+6N   5  /System.Windows.Controls.DataGridHyperlinkColumn  5      $ Content Link��6*   $	 240 6`    6@   �� *ContentLink+6`     5      $ Comment Link��6*   $	 240 6`    6@   �� *CommentLink+6`   5      �� 5    
   -   5        $��Import Old Posts��5       $��Right=�6L   $��	0,0,10,10q�6h   $��Bottom=�6{   $	��110��6�   5!      5"      
   M S B A M L   `   `   `  ����G  CComparatizer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Comparatizer2Comparatizer2  5          Comparatizer.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      -localclr-namespace:Comparatizer2     5      -   5          �Name$
  Main��5   	    � Title$ 
MainWindow��5      $	��500��6   $	��800��6#   .��5	      � -   5         �Name$ gridMain��5	       � ColumnDefinitions 5
   
   �� 5      �� 5      $��5 �6   �� 5       � RowDefinitions 5   
   �� 5      $	��12* �6   �� 5      $��1* �6   $��50��6(   .+�5   
   �� -   5         ��Name$ 	gbxQAText��5      $��QA Text��6&   .��5      �� -   5         ��Name$ 	rtbQAText��5      $��1��6-    �� 	IsEnabled . $��16(�6V   $
��Auto=�6d   . �5      /� .T�5      J� .*�6    .*�5      �� $�Paste QA Text Here��6   5      5      5      5      �� 5   
   -   5        $ gbxScribeText��5      $��2��6*   $��Scribe Text��6:   .��5      �� -   5        $ rtbScribeText��5      $��2��61    . $��16(�6Z   $
��Auto=�6h   . �5      /� .T�5      J� .*�6    .*�5       �� $�Paste Scribe Text Here��6   5!      5"      5#      5$      �� 5%   
   -   5         ��Name$ 
btnCompare��5%      $��Comparatize��6%   $��3��6;   $��2��6h   $��	100,0,0,0q�6u   �� 5&   
   -   5        $ btnSettings��5&      $��Settings��6&   $��1��69   $
��Left=�6F   $��50��6a   6l   �� 5'   
   -	   5        $ btnDebug��5'      $��Debug��6#   $��1��63   $
��Left=�6@   $��50,0,0,0q�6[   $��50��6m   6x   5(      5)      
   M S B A M L   `   `   `  ���� E  ANposCheck, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:AnalizaAppWpfAnalizaAppWpf      AnalizaAppWpf.Miesiac   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:AnalizaAppWpf     $��17(�  � Title$  Miesiac��$	��450��$	��800��.���  � RowDefinitions �� $��40 ��� $��* ��� $��60 �.+��� -    ��Name$ lblTitle��$��0��&   System.Windows.Controls.DataGrid  -     Name$ dgMonth��$��1��   AutoGenerateColumns .  $��Disabled=�$��Stretch=�$��Stretch=�   CanUserAddRows .     
IsReadOnly .    Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 1* 	   Header$	 Salon��1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 �� *
NumerSklep+  $ 1* $	 Fiskalne��
 �� *BruttoFiskalne+ �� StringFormat$ {0:F2}��  $ 1* $
	 nPOS��
 �� *
BruttoNpos+$ {0:F2}��  $ 1* $	 Fiskalne vs nPOS��
 �� *BruttoFiskalneNpos+$ {0:F2}��   	CellStyle ��  �� 
TargetType*  $System.Windows.Controls.DataGridCell"   �� Triggers ��  �� Binding �� *Status+ �� Value	 Green.a���  �� Property � �� �� Value
 � � ���  �� *Status+ Red.a���  � ��
 �  ����  �� *Status+
 Yellow.a���  � ��
 � �����  �� *Status+ Blue.a���  � ��
 ��  �   RowDetailsTemplate �� .R�� $��0,0,0,30q�$
��Left=�.+��� $��
btnHideRow��-   $	��100��$��17(�.��Ukryj  $��Disabled=� .  ���#88AFD1   HeadersVisibility$ Column=� .   . ���� *Pos+   $ 1* $	 
Stanowisko��
 �� *
Stanowisko+  $ 1* $	 Fiskalne��
 �� *BruttoFiskalne+$ {0:F2}��  $ 1* $
	 nPOS��
 �� *
BruttoNpos+$ {0:F2}��  $ 1* $	 Fiskalne vs nPOS��
 �� *BruttoFiskalneNpos+$ {0:F2}�� �� "   ��  �� *Status+	 Green.a���  � ��
 � � ���  �� *Status+ Red.a���  � ��
 �  ����  �� *Status+
 Yellow.a���  � ��
 � �����  �� *Status+ Blue.a���  � ��
 ��  �� $��2��$��Center=� � ColumnDefinitions �� $	��350 ��� $��* ��� $	��200 �.+�� $��0��.+��� $��Filtr lista sklepów���� -    ��Name$ 	txtFilter��$	��100���� -    ��Name$ btnSave��$��2��$��Zapisz do XLS��$��	0,1,10,10q�
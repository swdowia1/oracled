   M S B A M L   `   `   `  ���� E  ANposCheck, Version=11.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:AnalizaAppWpfAnalizaAppWpf      AnalizaAppWpf.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:AnalizaAppWpf     -   $��17(�  � Title$  
MainWindow��$	��450��$
��1200��.���  � RowDefinitions �� $��40 ��� $��* ��� $��60 �.+�� $��0��.+��� $��Raport dla daty:���� -    ��Name$ cbData�� �� DisplayMemberPath$
 Show��$	��230�� �� SelectedIndex$ 0��$	��Top=�$��Stretch=�$��Center=�$
��Left=� � ToolTip$ 7 ostatnich dni���� -    ��Name$ lblIlosc��$
��Bold"�$�� ���� -    ��Name$ chkCC��$��	10,6,0,17q�$	��CCC���� -   $ chkHalf��$��	10,6,0,17q�$��	HalfPrice���� -    ��Name$ 
btnMiesiac��$��Raporty miesieczne��$��10,0,0,0q�&   System.Windows.Controls.DataGrid  -   	  Name$	 dgSold��$��1��$y�Hidden=�
   AutoGenerateColumns
 .  $��Disabled=�$��Stretch=�$��Stretch=�   CanUserAddRows .     
IsReadOnly .    RowStyle �� Orange2�� �� 
TargetType)  #System.Windows.Controls.DataGridRow"  .���  �� Property �@�ToolTip �� Value �� ���� *
ShopName+ �� Triggers ��  �� Binding �� *Status+ �� Value Gray.a���  � �� �#DEE1E4   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 1*    Header$ Salon��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *
NumerSklep+  $ 1* $ Fiskalne�� �� *BruttoFiskalne+ �� StringFormat$ {0:F2}��  $ 1* $
 Data�� �� *Data+  $ 1* $
 nPOS�� �� *
BruttoNpos+$ {0:F2}��  $	 300 $ Fiskalne vs nPOS�� �� *BruttoFiskalneNpos+$ {0:F2}��   	CellStyle �� *  $System.Windows.Controls.DataGridCell"   ��  �� *Status+	 Green.a���  � ��
 � � ���  �� *Status+ Red.a���  � ��
 �  ����  �� *Status+
 Yellow.a���  � ��
 � �����  �� *Status+ Blue.a���  � ��
 ��  �  $ 1* $ 
Npos Karta�� �� *Karta+$ {0:F2}��  $ 1* $ Terminal�� �� *
Terminal+$ {0:F2}��  $ 1* $ Terminal vs Npos Karta�� �� *KartaTerminal+$ {0:F2}�� �� "   ��  �� *StatusKarta+ Red.a���  � ��
 �  ��   RowDetailsTemplate �� .R�� $��0,0,0,30q�$
��Left=�.+��� $��
btnHideRow��-   $	��100��$��17(�.��Ukryj  $��Disabled=�
 .  ���#88AFD1   HeadersVisibility$ Column=� .   . ���� *
Drukarka+   $ 1* $ 
Stanowisko�� �� *
Stanowisko+  $ 1* $ Fiskalne�� �� *BruttoFiskalne+$ {0:F2}��  $ 1* $
 Data�� �� *Data+  $ 1* $
 nPOS�� �� *
BruttoNpos+$ {0:F2}��  $	 300 $ Fiskalne vs nPOS�� �� *BruttoFiskalneNpos+$ {0:F2}�� �� "   ��  �� *Status+	 Green.a���  � ��
 � � ���  �� *Status+ Red.a���  � ��
 �  ����  �� *Status+
 Yellow.a���  � ��
 � �����  �� *Status+ Blue.a���  � ��
 ��  �  $ 1* $ 
Npos Karta�� �� *Karta+$ {0:F2}��  $ 1* $ Terminal�� �� *
Terminal+$ {0:F2}��  $ 1* $ Terminal vs Npos Karta�� �� *KartaTerminal+$ {0:F2}�� �� "   ��  �� *StatusKarta+ Red.a���  � ��
 �  ���� -	    ��Name$ txtLoad��$��1��$��20q�$	��640��$	��100��$��48(�$
��Bold"�
���   �Ładowanie danych..... � Triggers :�  :� RoutedEvent$ TextBlock.Loaded��.[��� .���� .��W�  �� 
TargetName$ txtLoad��  �� TargetProperty$  (TextBlock.Opacity)�! W� From$	! 1.0X�	" W� To$	" 0.0X�# g� Duration$# 0:0:0.3D�$ g� AutoReverse$ . % g� RepeatBehavior$% Forever��� $��2��$��Center=�& � ColumnDefinitions& �� $	��150 ��� $	��100 ��� $	��210 �$	��300���� $	��450 ��� $��* ��� $	��200 �.+��� $��0��$��Filtr lista sklepów���� -
   ' ��Name$' 	txtFilter��$��1��$	��100���� $��2��._��� .+��� -   $ 
chkNotZero��$	��400��$%��Fiskalne vs nPOS różne od 0  ���� -   $ 
chkNotSold��$%��Otwarte sklepy bez sprzedaży  ���� $��3��
�������
���   �$��1q�( �� CornerRadius$( 8,8,3,3��._�� -   ) �Name$
) wpRB��$��Center=�* � 	Resources* ��%l   '��       '	� ,     �� " ��.���  � ��$ 10,10,10,10q��� " 	�.���  � ��$ 10,10,10,10q�.+�	� -   + 	�Name$+ rbAll��.��Wszystkie sklepy	� -   $+ rbSelect��.��Wybrane sklepy�� -   $��btnFilterShop��.��	Wybierz�� -   $ btnSave��$��5��$��Zapisz do XLS��$��	0,1,10,10q�
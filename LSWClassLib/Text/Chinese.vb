﻿Imports System.Text.RegularExpressions

Namespace Text
    Public Module Chinese
#Region "笔画数据"
        Public gb2312_stroke_count As Integer() = {10, 7, 10, 10, 8, 10, _
    9, 11, 17, 14, 13, 5, _
    13, 10, 12, 15, 10, 6, _
    10, 9, 13, 8, 10, 10, _
    8, 8, 10, 5, 10, 14, _
    16, 9, 12, 12, 15, 15, _
    7, 10, 5, 5, 7, 10, _
    2, 9, 4, 8, 12, 13, _
    7, 10, 7, 21, 10, 8, _
    5, 9, 6, 13, 8, 8, _
    9, 13, 12, 10, 13, 7, _
    10, 10, 8, 8, 7, 8, _
    7, 19, 5, 4, 8, 5, _
    9, 10, 14, 14, 9, 12, _
    15, 10, 15, 12, 12, 8, _
    9, 5, 15, 10, 16, 13, _
    9, 12, 8, 8, 8, 7, _
    15, 10, 13, 19, 8, 13, _
    12, 8, 5, 12, 9, 4, _
    9, 10, 7, 8, 12, 12, _
    10, 8, 8, 5, 11, 11, _
    11, 9, 9, 18, 9, 12, _
    14, 4, 13, 10, 8, 14, _
    13, 14, 6, 10, 9, 4, _
    7, 13, 6, 11, 14, 5, _
    13, 16, 17, 16, 9, 18, _
    5, 12, 8, 9, 9, 8, _
    4, 16, 16, 17, 12, 9, _
    11, 15, 8, 19, 16, 7, _
    15, 11, 12, 16, 13, 10, _
    13, 7, 6, 9, 5, 8, _
    9, 9, 10, 6, 8, 11, _
    15, 8, 10, 8, 12, 9, _
    13, 10, 14, 7, 8, 11, _
    11, 14, 12, 8, 7, 10, _
    2, 10, 7, 11, 4, 5, _
    7, 19, 10, 8, 17, 11, _
    12, 7, 3, 7, 12, 15, _
    8, 11, 11, 14, 16, 8, _
    10, 9, 11, 11, 7, 7, _
    10, 4, 7, 17, 16, 16, _
    15, 11, 9, 8, 12, 8, _
    5, 9, 7, 19, 12, 3, _
    9, 9, 9, 14, 12, 14, _
    7, 9, 8, 8, 10, 10, _
    12, 11, 14, 12, 11, 13, _
    11, 6, 11, 19, 8, 11, _
    6, 9, 11, 4, 11, 7, _
    2, 12, 8, 11, 10, 12, _
    7, 9, 12, 15, 15, 11, _
    7, 8, 4, 7, 15, 12, _
    7, 15, 10, 6, 7, 6, _
    11, 7, 7, 7, 12, 8, _
    15, 10, 9, 16, 6, 7, _
    10, 12, 12, 15, 8, 8, _
    10, 10, 10, 6, 13, 9, _
    11, 6, 7, 6, 6, 10, _
    8, 8, 4, 7, 10, 5, _
    9, 6, 6, 6, 11, 8, _
    8, 13, 12, 14, 13, 13, _
    13, 4, 11, 14, 4, 10, _
    7, 5, 16, 12, 18, 12, _
    13, 12, 9, 13, 10, 12, _
    24, 13, 13, 5, 12, 3, _
    9, 13, 7, 11, 12, 7, _
    9, 12, 15, 7, 6, 6, _
    7, 8, 11, 13, 8, 9, _
    13, 15, 10, 11, 7, 21, _
    18, 11, 11, 9, 14, 14, _
    13, 13, 10, 7, 6, 8, _
    12, 6, 15, 12, 7, 5, _
    4, 5, 11, 11, 15, 17, _
    9, 19, 16, 12, 14, 11, _
    13, 10, 13, 14, 11, 14, _
    7, 6, 3, 14, 15, 12, _
    11, 10, 13, 12, 6, 12, _
    14, 5, 3, 7, 4, 12, _
    17, 9, 9, 5, 9, 11, _
    9, 11, 9, 10, 8, 4, _
    8, 10, 11, 9, 5, 12, _
    7, 11, 11, 8, 11, 11, _
    6, 9, 10, 9, 10, 2, _
    10, 17, 10, 7, 11, 6, _
    8, 15, 11, 12, 11, 15, _
    11, 8, 19, 6, 12, 12, _
    17, 14, 4, 12, 7, 14, _
    8, 10, 11, 7, 10, 14, _
    14, 8, 8, 6, 12, 11, _
    9, 7, 10, 12, 16, 11, _
    13, 13, 9, 8, 16, 9, _
    5, 7, 7, 8, 11, 12, _
    11, 13, 13, 5, 16, 10, _
    2, 11, 6, 8, 10, 12, _
    10, 14, 15, 8, 11, 13, _
    2, 7, 5, 7, 8, 12, _
    13, 8, 4, 6, 5, 5, _
    12, 15, 6, 9, 8, 9, _
    7, 9, 11, 7, 4, 9, _
    7, 10, 12, 10, 13, 9, _
    12, 9, 10, 11, 13, 12, _
    7, 14, 7, 9, 12, 7, _
    14, 12, 14, 9, 11, 12, _
    11, 7, 4, 5, 15, 7, _
    19, 12, 10, 7, 9, 9, _
    12, 11, 9, 6, 6, 9, _
    13, 6, 13, 11, 8, 12, _
    11, 13, 10, 12, 9, 15, _
    6, 10, 10, 4, 7, 12, _
    11, 10, 10, 6, 2, 6, _
    5, 9, 9, 2, 9, 5, _
    9, 12, 6, 4, 9, 8, _
    9, 18, 6, 12, 18, 15, _
    8, 8, 17, 3, 10, 4, _
    7, 8, 8, 5, 7, 7, _
    7, 7, 4, 8, 8, 6, _
    7, 6, 6, 7, 8, 11, _
    8, 11, 3, 8, 10, 10, _
    7, 8, 8, 8, 9, 7, _
    11, 7, 8, 4, 7, 7, _
    12, 7, 10, 8, 6, 8, _
    12, 12, 4, 9, 8, 13, _
    10, 12, 4, 9, 11, 10, _
    5, 13, 6, 8, 4, 7, _
    7, 4, 15, 8, 14, 7, _
    8, 13, 12, 9, 11, 6, _
    9, 8, 10, 11, 13, 11, _
    5, 7, 7, 11, 10, 10, _
    8, 11, 12, 8, 14, 9, _
    11, 18, 12, 9, 12, 5, _
    8, 4, 13, 6, 12, 4, _
    7, 6, 13, 8, 15, 14, _
    8, 7, 13, 9, 11, 12, _
    3, 5, 7, 9, 9, 7, _
    10, 13, 8, 11, 21, 4, _
    6, 9, 9, 7, 7, 7, _
    12, 7, 16, 10, 10, 14, _
    10, 16, 13, 15, 15, 7, _
    10, 14, 12, 4, 11, 10, _
    8, 12, 9, 12, 10, 12, _
    9, 12, 11, 3, 6, 9, _
    10, 13, 10, 7, 8, 19, _
    10, 10, 11, 3, 7, 5, _
    10, 11, 8, 10, 4, 9, _
    3, 6, 7, 9, 7, 6, _
    9, 4, 7, 8, 8, 9, _
    8, 8, 11, 12, 11, 8, _
    14, 7, 8, 8, 8, 13, _
    5, 11, 9, 7, 8, 9, _
    10, 8, 12, 8, 5, 9, _
    14, 9, 13, 8, 8, 8, _
    12, 6, 8, 9, 6, 14, _
    11, 23, 12, 20, 8, 6, _
    3, 10, 13, 8, 6, 11, _
    5, 7, 9, 6, 9, 8, _
    9, 10, 8, 13, 9, 8, _
    12, 13, 12, 12, 10, 8, _
    8, 14, 6, 9, 15, 9, _
    10, 10, 6, 10, 9, 12, _
    14, 7, 12, 7, 11, 12, _
    8, 12, 7, 16, 16, 10, _
    7, 16, 10, 11, 6, 5, _
    5, 8, 10, 17, 17, 14, _
    11, 9, 6, 10, 5, 10, _
    8, 12, 10, 11, 10, 5, _
    8, 7, 6, 11, 13, 9, _
    8, 11, 14, 14, 15, 9, _
    15, 12, 11, 9, 9, 9, _
    10, 7, 15, 16, 9, 8, _
    9, 10, 9, 11, 9, 7, _
    5, 6, 12, 9, 12, 7, _
    9, 10, 6, 8, 5, 8, _
    13, 10, 12, 9, 15, 8, _
    15, 12, 8, 8, 11, 7, _
    4, 7, 4, 7, 9, 6, _
    12, 12, 8, 6, 4, 8, _
    13, 9, 7, 11, 7, 6, _
    8, 10, 7, 12, 10, 11, _
    10, 12, 13, 11, 10, 9, _
    4, 9, 12, 11, 16, 15, _
    17, 9, 11, 12, 13, 10, _
    13, 9, 11, 6, 9, 12, _
    17, 9, 12, 6, 13, 10, _
    15, 5, 12, 11, 10, 11, _
    6, 10, 5, 6, 9, 9, _
    9, 8, 11, 13, 9, 11, _
    17, 9, 6, 4, 10, 8, _
    12, 16, 8, 11, 5, 6, _
    11, 6, 13, 15, 10, 14, _
    6, 5, 9, 16, 4, 7, _
    10, 11, 12, 6, 7, 12, _
    13, 20, 12, 3, 9, 10, _
    6, 7, 13, 6, 9, 2, _
    10, 3, 13, 7, 16, 8, _
    6, 11, 8, 11, 9, 11, _
    11, 4, 5, 9, 7, 7, _
    7, 10, 6, 14, 9, 6, _
    8, 10, 5, 9, 12, 10, _
    5, 10, 11, 15, 6, 9, _
    8, 13, 7, 10, 7, 6, _
    11, 7, 13, 10, 8, 8, _
    6, 12, 9, 11, 9, 14, _
    12, 8, 10, 13, 9, 11, _
    11, 9, 14, 13, 12, 9, _
    4, 13, 15, 6, 10, 10, _
    9, 8, 11, 12, 10, 8, _
    15, 9, 9, 10, 6, 19, _
    12, 10, 9, 6, 6, 13, _
    8, 15, 12, 17, 12, 10, _
    6, 8, 9, 9, 9, 20, _
    12, 11, 11, 8, 11, 9, _
    7, 9, 16, 9, 13, 11, _
    14, 10, 10, 5, 12, 12, _
    11, 9, 11, 12, 6, 14, _
    7, 5, 10, 8, 11, 13, _
    14, 9, 9, 13, 8, 7, _
    17, 7, 9, 10, 4, 9, _
    9, 8, 3, 12, 4, 8, _
    4, 9, 18, 10, 13, 4, _
    13, 7, 13, 10, 13, 7, _
    10, 10, 6, 7, 9, 14, _
    8, 13, 12, 16, 8, 11, _
    14, 13, 8, 4, 19, 12, _
    11, 14, 14, 12, 16, 8, _
    10, 13, 11, 10, 8, 9, _
    12, 12, 7, 5, 7, 9, _
    3, 7, 2, 10, 11, 11, _
    5, 6, 13, 8, 12, 8, _
    17, 8, 8, 10, 8, 8, _
    11, 7, 8, 9, 9, 8, _
    14, 7, 11, 4, 8, 11, _
    15, 13, 10, 5, 11, 8, _
    10, 10, 12, 10, 10, 11, _
    8, 10, 15, 23, 7, 11, _
    10, 17, 9, 6, 6, 9, _
    7, 11, 9, 6, 7, 10, _
    9, 12, 10, 9, 10, 12, _
    8, 5, 9, 4, 12, 13, _
    8, 12, 5, 12, 11, 7, _
    9, 9, 11, 14, 17, 6, _
    7, 4, 8, 6, 9, 10, _
    15, 8, 8, 9, 12, 15, _
    14, 9, 7, 9, 5, 12, _
    7, 8, 9, 10, 8, 11, _
    9, 10, 7, 7, 8, 10, _
    4, 11, 7, 3, 6, 11, _
    9, 10, 13, 8, 14, 7, _
    12, 6, 9, 9, 13, 10, _
    7, 13, 8, 7, 10, 12, _
    6, 12, 7, 10, 8, 11, _
    7, 7, 3, 11, 8, 13, _
    12, 9, 13, 11, 12, 12, _
    12, 8, 8, 10, 7, 9, _
    6, 13, 12, 8, 8, 12, _
    14, 12, 14, 11, 10, 7, _
    13, 13, 11, 9, 8, 16, _
    12, 5, 15, 14, 12, 9, _
    16, 12, 9, 13, 11, 12, _
    10, 11, 8, 10, 10, 10, _
    7, 7, 6, 8, 9, 13, _
    10, 10, 11, 5, 13, 18, _
    16, 15, 11, 17, 9, 16, _
    6, 9, 8, 12, 13, 7, _
    9, 11, 11, 15, 16, 10, _
    10, 13, 11, 7, 7, 15, _
    5, 10, 9, 6, 10, 7, _
    5, 7, 10, 4, 7, 12, _
    8, 9, 12, 5, 11, 7, _
    8, 2, 14, 10, 9, 12, _
    10, 7, 18, 13, 8, 10, _
    8, 11, 11, 12, 10, 9, _
    8, 13, 10, 11, 13, 7, _
    7, 11, 12, 12, 9, 10, _
    15, 11, 14, 7, 16, 14, _
    5, 15, 2, 14, 17, 14, _
    10, 6, 12, 10, 6, 11, _
    12, 8, 17, 16, 9, 7, _
    20, 11, 15, 10, 7, 8, _
    9, 11, 13, 13, 10, 7, _
    11, 10, 7, 10, 8, 11, _
    5, 5, 13, 11, 14, 12, _
    13, 10, 6, 15, 10, 9, _
    4, 5, 11, 8, 11, 16, _
    11, 8, 8, 7, 13, 9, _
    12, 15, 14, 8, 7, 5, _
    11, 7, 8, 11, 7, 8, _
    12, 19, 13, 21, 13, 10, _
    11, 16, 12, 8, 7, 15, _
    7, 6, 11, 8, 10, 15, _
    12, 12, 10, 12, 9, 11, _
    13, 11, 9, 10, 9, 13, _
    7, 7, 11, 11, 7, 8, _
    6, 4, 7, 7, 6, 11, _
    17, 8, 11, 13, 14, 14, _
    13, 12, 9, 9, 9, 6, _
    11, 7, 8, 9, 3, 9, _
    14, 6, 10, 6, 7, 8, _
    6, 9, 15, 14, 12, 13, _
    14, 11, 14, 14, 13, 6, _
    9, 8, 8, 6, 10, 11, _
    8, 13, 4, 5, 10, 5, _
    8, 9, 12, 14, 9, 3, _
    8, 8, 11, 14, 15, 13, _
    7, 9, 12, 14, 7, 9, _
    9, 12, 8, 12, 3, 7, _
    5, 11, 13, 17, 13, 13, _
    11, 11, 8, 11, 15, 19, _
    17, 9, 11, 8, 6, 10, _
    8, 8, 14, 11, 12, 12, _
    10, 11, 11, 7, 9, 10, _
    12, 9, 8, 11, 13, 17, _
    9, 12, 8, 7, 14, 5, _
    5, 8, 5, 11, 10, 9, _
    8, 16, 8, 11, 6, 8, _
    13, 13, 14, 19, 14, 14, _
    16, 15, 20, 8, 5, 10, _
    15, 16, 8, 13, 13, 8, _
    11, 6, 9, 8, 7, 7, _
    8, 5, 13, 14, 13, 12, _
    14, 4, 5, 13, 8, 16, _
    10, 9, 7, 9, 6, 9, _
    7, 6, 2, 5, 9, 8, _
    9, 7, 10, 22, 9, 10, _
    9, 8, 11, 8, 10, 4, _
    14, 10, 8, 16, 10, 8, _
    5, 7, 7, 10, 13, 9, _
    13, 14, 8, 6, 15, 15, _
    11, 8, 10, 14, 5, 7, _
    10, 10, 19, 11, 15, 15, _
    10, 11, 9, 8, 16, 5, _
    8, 8, 4, 7, 9, 7, _
    10, 9, 6, 7, 5, 7, _
    9, 3, 13, 9, 8, 9, _
    17, 20, 10, 10, 8, 9, _
    8, 18, 7, 11, 7, 11, _
    9, 8, 8, 8, 12, 8, _
    11, 12, 11, 12, 9, 19, _
    15, 11, 15, 9, 10, 7, _
    9, 6, 8, 10, 16, 9, _
    7, 8, 7, 9, 10, 12, _
    8, 8, 9, 11, 14, 12, _
    10, 10, 8, 7, 12, 9, _
    10, 8, 11, 15, 12, 13, _
    12, 13, 16, 16, 8, 13, _
    11, 13, 8, 9, 21, 7, _
    8, 15, 12, 9, 11, 12, _
    10, 5, 4, 12, 15, 7, _
    20, 15, 11, 4, 12, 15, _
    14, 16, 11, 14, 16, 9, _
    13, 8, 9, 13, 6, 8, _
    8, 11, 5, 8, 10, 7, _
    9, 8, 8, 11, 11, 10, _
    14, 8, 11, 10, 5, 12, _
    4, 10, 12, 11, 13, 10, _
    6, 10, 12, 10, 14, 19, _
    18, 12, 12, 10, 11, 8, _
    2, 10, 14, 9, 7, 8, _
    12, 8, 8, 11, 11, 10, _
    6, 14, 8, 6, 11, 10, _
    6, 3, 6, 7, 9, 9, _
    16, 4, 6, 7, 7, 8, _
    5, 11, 9, 9, 9, 6, _
    8, 10, 3, 6, 13, 5, _
    12, 11, 16, 10, 10, 9, _
    15, 13, 8, 15, 11, 12, _
    4, 14, 8, 7, 12, 7, _
    14, 14, 12, 7, 16, 14, _
    14, 10, 10, 17, 6, 8, _
    5, 16, 15, 12, 10, 9, _
    10, 4, 8, 5, 8, 9, _
    9, 9, 9, 10, 12, 13, _
    7, 15, 12, 13, 7, 8, _
    9, 9, 10, 10, 11, 16, _
    12, 12, 11, 8, 10, 6, _
    12, 7, 9, 5, 7, 11, _
    7, 5, 9, 8, 12, 4, _
    11, 6, 11, 8, 7, 11, _
    8, 11, 17, 15, 5, 11, _
    23, 6, 16, 10, 6, 11, _
    10, 4, 8, 4, 10, 8, _
    16, 7, 13, 14, 12, 11, _
    12, 13, 12, 16, 5, 9, _
    22, 20, 20, 20, 5, 9, _
    7, 9, 12, 10, 4, 4, _
    2, 7, 7, 6, 4, 3, _
    7, 6, 5, 4, 4, 6, _
    9, 13, 9, 16, 14, 13, _
    10, 9, 4, 12, 9, 6, _
    9, 20, 16, 17, 6, 10, _
    8, 6, 2, 15, 8, 6, _
    15, 13, 12, 7, 10, 8, _
    10, 15, 9, 11, 13, 17, _
    13, 14, 3, 8, 6, 12, _
    10, 13, 8, 12, 12, 6, _
    12, 13, 6, 10, 12, 14, _
    10, 9, 6, 8, 7, 7, _
    13, 11, 13, 12, 10, 9, _
    8, 7, 3, 7, 14, 8, _
    5, 8, 16, 17, 16, 12, _
    6, 10, 15, 14, 6, 11, _
    12, 10, 3, 8, 14, 11, _
    10, 12, 10, 6, 3, 14, _
    4, 10, 7, 8, 11, 11, _
    11, 6, 8, 11, 13, 10, _
    13, 10, 7, 6, 10, 5, _
    8, 7, 7, 11, 10, 8, _
    9, 7, 8, 11, 9, 8, _
    13, 11, 7, 5, 12, 9, _
    4, 11, 9, 11, 12, 9, _
    5, 6, 5, 9, 9, 12, _
    8, 3, 8, 2, 5, 9, _
    7, 4, 9, 9, 8, 7, _
    5, 5, 8, 9, 8, 8, _
    6, 5, 3, 5, 9, 8, _
    9, 14, 10, 8, 9, 13, _
    16, 9, 5, 8, 12, 8, _
    4, 5, 9, 9, 8, 8, _
    6, 4, 9, 6, 7, 11, _
    11, 8, 14, 11, 15, 8, _
    11, 10, 7, 13, 8, 12, _
    11, 12, 4, 12, 11, 15, _
    16, 12, 17, 13, 13, 12, _
    13, 12, 5, 8, 9, 7, _
    6, 9, 14, 11, 13, 14, _
    10, 8, 9, 14, 10, 5, _
    5, 10, 9, 17, 4, 11, _
    10, 4, 13, 12, 7, 17, _
    9, 12, 9, 11, 10, 9, _
    12, 15, 15, 9, 7, 5, _
    5, 6, 13, 6, 13, 5, _
    7, 6, 8, 3, 8, 10, _
    8, 10, 9, 7, 6, 9, _
    12, 15, 16, 14, 7, 12, _
    9, 10, 10, 12, 14, 13, _
    13, 11, 7, 8, 14, 13, _
    14, 9, 11, 11, 10, 21, _
    13, 6, 17, 12, 14, 10, _
    6, 10, 10, 13, 11, 10, _
    14, 11, 10, 12, 8, 13, _
    5, 5, 6, 12, 16, 9, _
    17, 15, 9, 8, 8, 5, _
    10, 11, 4, 8, 7, 7, _
    13, 8, 15, 13, 7, 17, _
    13, 15, 14, 10, 8, 12, _
    10, 14, 11, 5, 9, 6, _
    13, 13, 11, 12, 15, 10, _
    16, 10, 15, 11, 15, 10, _
    11, 10, 13, 10, 11, 10, _
    9, 11, 10, 5, 10, 10, _
    18, 13, 10, 13, 11, 10, _
    15, 12, 12, 15, 16, 12, _
    7, 12, 17, 11, 10, 9, _
    8, 4, 11, 13, 5, 11, _
    9, 14, 12, 9, 7, 8, _
    11, 13, 9, 10, 8, 4, _
    7, 9, 5, 6, 11, 9, _
    9, 9, 12, 10, 10, 13, _
    17, 6, 11, 7, 12, 11, _
    10, 12, 9, 12, 11, 7, _
    5, 10, 5, 7, 9, 8, _
    10, 10, 10, 11, 3, 6, _
    8, 12, 6, 11, 13, 13, _
    13, 14, 9, 7, 4, 17, _
    8, 6, 11, 10, 7, 6, _
    8, 12, 7, 8, 12, 9, _
    9, 12, 9, 9, 4, 10, _
    9, 5, 15, 9, 12, 8, _
    10, 3, 11, 7, 13, 10, _
    11, 12, 11, 8, 11, 3, _
    12, 7, 4, 3, 8, 6, _
    8, 8, 11, 7, 6, 9, _
    20, 13, 6, 4, 7, 10, _
    7, 11, 11, 4, 14, 11, _
    7, 11, 8, 6, 6, 7, _
    7, 5, 14, 8, 9, 9, _
    12, 17, 7, 12, 11, 11, _
    15, 3, 14, 12, 10, 4, _
    9, 7, 7, 14, 10, 6, _
    13, 10, 8, 9, 13, 10, _
    12, 7, 14, 8, 12, 7, _
    7, 7, 9, 4, 6, 9, _
    9, 4, 7, 11, 7, 7, _
    4, 8, 4, 10, 4, 14, _
    6, 9, 7, 5, 13, 11, _
    8, 4, 5, 10, 9, 8, _
    14, 8, 6, 11, 8, 12, _
    15, 6, 13, 10, 12, 10, _
    7, 11, 15, 3, 11, 14, _
    11, 13, 6, 12, 17, 11, _
    10, 3, 13, 12, 11, 9, _
    7, 12, 6, 8, 15, 9, _
    7, 17, 14, 13, 9, 8, _
    9, 3, 12, 10, 6, 11, _
    13, 6, 5, 14, 6, 9, _
    8, 11, 11, 7, 9, 8, _
    13, 9, 9, 8, 13, 7, _
    13, 11, 12, 9, 10, 8, _
    8, 9, 11, 22, 9, 15, _
    17, 12, 3, 12, 10, 8, _
    13, 9, 8, 9, 9, 15, _
    13, 6, 11, 11, 12, 15, _
    9, 10, 18, 12, 10, 10, _
    11, 10, 3, 7, 10, 7, _
    11, 10, 10, 13, 8, 13, _
    15, 15, 6, 9, 13, 6, _
    11, 8, 11, 5, 11, 9, _
    19, 16, 8, 8, 12, 10, _
    16, 7, 12, 8, 7, 13, _
    7, 4, 9, 11, 9, 13, _
    12, 12, 6, 6, 9, 7, _
    6, 6, 16, 8, 7, 8, _
    8, 5, 4, 10, 6, 7, _
    12, 14, 6, 9, 10, 6, _
    13, 12, 7, 10, 10, 14, _
    6, 14, 11, 14, 9, 10, _
    6, 13, 11, 9, 6, 7, _
    10, 9, 12, 12, 11, 11, _
    7, 12, 9, 11, 11, 5, _
    9, 19, 10, 9, 13, 16, _
    8, 5, 11, 6, 9, 14, _
    12, 6, 8, 6, 6, 6, _
    10, 6, 5, 5, 9, 6, _
    6, 8, 9, 10, 7, 3, _
    7, 4, 10, 11, 13, 11, _
    12, 9, 6, 6, 11, 9, _
    11, 10, 11, 10, 7, 9, _
    12, 8, 7, 7, 15, 11, _
    8, 8, 8, 11, 11, 9, _
    14, 10, 12, 16, 6, 9, _
    12, 10, 9, 12, 10, 11, _
    10, 9, 5, 10, 10, 7, _
    6, 8, 8, 6, 9, 6, _
    10, 6, 11, 9, 10, 14, _
    16, 13, 7, 14, 13, 6, _
    13, 11, 12, 9, 9, 10, _
    9, 9, 20, 12, 15, 8, _
    6, 11, 7, 3, 6, 11, _
    5, 5, 6, 12, 8, 11, _
    1, 12, 7, 12, 11, 8, _
    6, 6, 13, 6, 12, 11, _
    5, 10, 14, 7, 8, 9, _
    18, 12, 9, 10, 3, 1, _
    7, 4, 4, 7, 8, 7, _
    6, 3, 7, 17, 11, 13, _
    9, 6, 13, 13, 15, 4, _
    3, 10, 13, 8, 5, 10, _
    7, 6, 17, 11, 8, 9, _
    9, 6, 10, 9, 6, 8, _
    7, 11, 11, 11, 7, 4, _
    4, 11, 5, 8, 15, 11, _
    18, 7, 14, 10, 11, 11, _
    9, 14, 7, 17, 9, 15, _
    13, 12, 9, 9, 8, 7, _
    17, 10, 11, 13, 14, 13, _
    8, 8, 10, 5, 11, 9, _
    5, 9, 6, 11, 7, 4, _
    5, 7, 10, 7, 8, 12, _
    7, 6, 4, 5, 7, 12, _
    9, 2, 5, 6, 11, 3, _
    8, 13, 13, 13, 14, 7, _
    9, 12, 8, 12, 12, 11, _
    11, 4, 10, 8, 3, 6, _
    9, 6, 9, 6, 5, 11, _
    6, 8, 6, 12, 12, 10, _
    12, 13, 11, 9, 8, 13, _
    10, 12, 12, 10, 15, 5, _
    10, 11, 10, 4, 9, 10, _
    10, 12, 14, 7, 7, 10, _
    13, 13, 12, 7, 8, 14, _
    9, 9, 4, 6, 12, 11, _
    9, 8, 12, 4, 10, 10, _
    10, 4, 9, 4, 9, 4, _
    7, 15, 11, 10, 13, 5, _
    5, 10, 6, 10, 9, 7, _
    10, 10, 6, 6, 9, 19, _
    12, 16, 10, 10, 12, 14, _
    17, 12, 19, 8, 6, 16, _
    9, 20, 16, 10, 7, 7, _
    17, 8, 8, 6, 8, 10, _
    9, 15, 15, 12, 16, 4, _
    12, 12, 5, 5, 11, 8, _
    9, 9, 14, 8, 5, 9, _
    7, 14, 10, 6, 10, 10, _
    14, 18, 9, 13, 11, 8, _
    10, 8, 14, 11, 10, 22, _
    9, 5, 9, 10, 12, 11, _
    15, 11, 14, 14, 7, 12, _
    10, 7, 3, 7, 8, 5, _
    8, 16, 13, 8, 9, 7, _
    8, 9, 13, 13, 6, 14, _
    5, 14, 7, 10, 12, 16, _
    8, 13, 14, 7, 10, 9, _
    13, 10, 13, 10, 16, 6, _
    7, 8, 8, 10, 7, 15, _
    10, 15, 6, 13, 9, 11, _
    8, 9, 6, 8, 16, 9, _
    5, 9, 9, 10, 8, 7, _
    6, 8, 4, 7, 14, 8, _
    8, 10, 5, 3, 8, 11, _
    8, 12, 12, 6, 10, 8, _
    7, 9, 4, 11, 5, 6, _
    7, 7, 10, 11, 6, 10, _
    13, 8, 9, 8, 12, 10, _
    13, 8, 8, 11, 12, 8, _
    11, 4, 9, 8, 9, 10, _
    8, 9, 8, 9, 6, 6, _
    6, 8, 6, 9, 7, 12, _
    9, 7, 8, 8, 10, 8, _
    9, 17, 10, 10, 12, 6, _
    11, 10, 8, 10, 6, 10, _
    12, 8, 17, 15, 5, 11, _
    9, 7, 11, 8, 12, 12, _
    7, 8, 9, 8, 7, 4, _
    9, 4, 9, 8, 15, 14, _
    15, 10, 6, 12, 6, 15, _
    6, 7, 12, 13, 9, 14, _
    7, 11, 10, 10, 10, 8, _
    8, 10, 12, 8, 10, 11, _
    11, 7, 9, 9, 9, 10, _
    9, 12, 11, 7, 12, 5, _
    9, 13, 3, 6, 11, 6, _
    18, 12, 15, 8, 11, 9, _
    7, 7, 7, 9, 12, 10, _
    7, 8, 11, 9, 7, 7, _
    8, 10, 20, 16, 15, 12, _
    13, 12, 15, 9, 5, 7, _
    9, 11, 7, 7, 10, 0, _
    0, 0, 0, 0, 3, 3, _
    3, 4, 4, 4, 5, 6, _
    6, 10, 10, 16, 1, 8, _
    1, 2, 3, 4, 4, 5, _
    5, 6, 9, 11, 14, 14, _
    19, 1, 8, 14, 2, 6, _
    4, 7, 7, 11, 14, 4, _
    6, 10, 11, 12, 14, 15, _
    16, 2, 5, 8, 11, 11, _
    15, 8, 7, 2, 4, 6, _
    7, 8, 8, 8, 9, 10, _
    10, 10, 13, 13, 14, 14, _
    15, 16, 2, 8, 2, 4, _
    4, 4, 5, 5, 5, 5, _
    6, 6, 6, 6, 6, 6, _
    6, 6, 6, 7, 7, 7, _
    7, 7, 7, 7, 7, 7, _
    8, 8, 8, 8, 8, 8, _
    8, 8, 8, 8, 8, 8, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 10, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 10, 10, 10, 11, 11, _
    11, 11, 11, 11, 11, 12, _
    12, 12, 13, 14, 14, 14, _
    14, 14, 14, 15, 15, 5, _
    6, 7, 7, 9, 17, 6, _
    8, 4, 12, 16, 17, 18, _
    21, 2, 9, 9, 11, 6, _
    6, 7, 2, 8, 10, 10, _
    11, 12, 12, 12, 13, 16, _
    19, 19, 2, 6, 8, 8, _
    10, 2, 10, 10, 2, 5, _
    5, 5, 6, 6, 6, 7, _
    7, 7, 7, 7, 7, 8, _
    8, 8, 8, 8, 8, 8, _
    8, 8, 8, 8, 9, 9, _
    9, 9, 10, 10, 10, 10, _
    10, 10, 10, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 12, 12, 12, 12, _
    12, 13, 13, 14, 14, 14, _
    15, 15, 19, 2, 8, 2, _
    5, 5, 6, 6, 7, 7, _
    7, 7, 8, 9, 9, 10, _
    10, 10, 11, 11, 11, 16, _
    5, 5, 5, 5, 6, 6, _
    7, 7, 7, 7, 7, 7, _
    8, 8, 8, 8, 8, 8, _
    8, 9, 9, 9, 9, 9, _
    10, 10, 11, 11, 13, 13, _
    13, 14, 14, 16, 19, 17, _
    5, 7, 5, 7, 7, 8, _
    10, 10, 11, 15, 9, 17, _
    20, 2, 2, 6, 10, 2, _
    5, 10, 12, 7, 9, 9, _
    14, 16, 16, 17, 6, 6, _
    6, 6, 6, 6, 6, 7, _
    7, 7, 8, 8, 8, 8, _
    8, 8, 8, 8, 8, 8, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 10, 10, 10, _
    10, 10, 10, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 12, 12, 12, 12, 13, _
    13, 14, 14, 14, 15, 20, _
    21, 22, 3, 5, 5, 6, _
    6, 6, 6, 6, 6, 6, _
    7, 7, 7, 7, 7, 7, _
    7, 7, 7, 7, 7, 7, _
    7, 7, 7, 7, 7, 7, _
    7, 7, 7, 7, 7, 8, _
    8, 8, 8, 8, 8, 8, _
    8, 8, 8, 8, 8, 8, _
    8, 8, 8, 8, 8, 8, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 10, 10, 10, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 10, 10, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 12, _
    12, 12, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 13, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 13, 14, 14, 14, _
    14, 14, 14, 14, 14, 14, _
    14, 14, 15, 15, 15, 15, _
    15, 15, 15, 15, 15, 16, _
    16, 16, 16, 16, 16, 16, _
    16, 16, 17, 17, 17, 17, _
    17, 18, 19, 19, 19, 20, _
    20, 22, 3, 9, 6, 7, _
    9, 9, 10, 10, 11, 3, _
    5, 5, 12, 3, 6, 7, _
    8, 8, 8, 8, 9, 9, _
    9, 10, 10, 10, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    12, 12, 13, 13, 13, 13, _
    13, 13, 13, 13, 14, 14, _
    14, 14, 14, 15, 15, 15, _
    15, 16, 16, 16, 17, 17, _
    19, 23, 25, 3, 7, 8, _
    12, 5, 5, 5, 5, 5, _
    5, 6, 6, 6, 7, 7, _
    7, 7, 7, 7, 7, 7, _
    7, 7, 7, 8, 8, 8, _
    8, 8, 8, 8, 8, 8, _
    8, 8, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 10, 10, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    12, 13, 13, 13, 13, 13, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 13, 13, 14, 14, _
    14, 14, 14, 14, 14, 14, _
    14, 15, 15, 15, 15, 15, _
    15, 15, 15, 15, 15, 15, _
    16, 16, 16, 16, 16, 16, _
    17, 17, 19, 25, 3, 6, _
    6, 7, 7, 8, 9, 10, _
    11, 11, 16, 7, 8, 8, _
    8, 10, 11, 11, 11, 12, _
    14, 14, 15, 15, 6, 6, _
    7, 7, 7, 7, 7, 7, _
    7, 7, 7, 8, 8, 8, _
    8, 8, 8, 8, 8, 8, _
    8, 9, 9, 9, 9, 10, _
    10, 11, 11, 11, 11, 11, _
    11, 11, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    13, 13, 13, 14, 15, 15, _
    17, 17, 19, 3, 7, 8, _
    9, 9, 9, 10, 11, 11, _
    12, 13, 15, 16, 24, 3, _
    3, 5, 6, 6, 6, 7, _
    7, 8, 8, 8, 9, 9, _
    9, 9, 10, 10, 10, 10, _
    10, 10, 10, 11, 11, 11, _
    11, 11, 11, 11, 12, 12, _
    12, 12, 12, 12, 14, 14, _
    15, 15, 16, 17, 20, 6, _
    14, 12, 14, 3, 3, 6, _
    7, 7, 7, 7, 7, 8, _
    9, 10, 10, 11, 12, 12, _
    13, 13, 14, 15, 15, 25, _
    5, 7, 7, 8, 9, 9, _
    11, 11, 11, 11, 12, 13, _
    14, 15, 16, 16, 17, 3, _
    5, 6, 6, 7, 7, 7, _
    7, 7, 7, 7, 7, 7, _
    7, 7, 8, 8, 8, 8, _
    8, 8, 8, 8, 8, 8, _
    8, 9, 9, 9, 9, 9, _
    9, 9, 10, 10, 10, 10, _
    10, 10, 10, 10, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    12, 12, 12, 12, 12, 12, _
    12, 13, 13, 14, 15, 15, _
    15, 16, 16, 18, 8, 17, _
    4, 6, 7, 7, 7, 7, _
    9, 9, 10, 10, 10, 11, _
    11, 11, 11, 11, 11, 12, _
    12, 13, 13, 13, 14, 3, _
    4, 8, 3, 6, 6, 6, _
    7, 7, 7, 7, 7, 7, _
    7, 7, 7, 7, 7, 7, _
    8, 8, 8, 8, 8, 8, _
    8, 8, 8, 8, 8, 8, _
    8, 8, 8, 8, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 10, 10, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    12, 12, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 13, 13, 13, 14, _
    14, 14, 14, 14, 14, 14, _
    14, 14, 14, 14, 14, 14, _
    14, 15, 15, 15, 15, 15, _
    15, 16, 16, 16, 16, 16, _
    16, 17, 17, 17, 17, 17, _
    19, 19, 19, 20, 20, 21, _
    24, 3, 5, 8, 8, 9, _
    10, 12, 13, 14, 14, 15, _
    16, 16, 17, 17, 3, 7, _
    7, 8, 8, 8, 8, 8, _
    8, 8, 9, 9, 10, 10, _
    10, 10, 10, 10, 11, 11, _
    11, 11, 12, 12, 12, 12, _
    13, 13, 13, 13, 15, 15, _
    16, 16, 17, 17, 18, 3, _
    11, 9, 12, 5, 9, 10, _
    10, 12, 14, 15, 21, 8, _
    8, 9, 11, 12, 22, 3, _
    6, 6, 7, 7, 7, 7, _
    7, 7, 7, 7, 7, 7, _
    8, 8, 8, 8, 9, 9, _
    9, 9, 9, 9, 9, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 11, 11, 11, 11, 11, _
    11, 11, 12, 12, 12, 12, _
    13, 13, 13, 13, 13, 13, _
    14, 14, 14, 14, 14, 14, _
    14, 15, 16, 16, 17, 17, _
    20, 5, 9, 7, 8, 12, _
    3, 3, 8, 8, 8, 8, _
    8, 8, 8, 8, 9, 9, _
    9, 10, 11, 11, 11, 11, _
    12, 12, 13, 13, 13, 14, _
    14, 15, 19, 20, 3, 6, _
    6, 6, 6, 6, 7, 7, _
    7, 8, 8, 8, 8, 8, _
    8, 8, 9, 9, 9, 10, _
    10, 10, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 12, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 13, _
    13, 13, 13, 13, 13, 13, _
    13, 14, 14, 14, 14, 14, _
    15, 15, 15, 16, 16, 16, _
    16, 19, 3, 15, 3, 8, _
    10, 6, 6, 8, 8, 8, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 10, 10, 10, 10, _
    10, 10, 10, 10, 10, 11, _
    12, 12, 12, 12, 12, 12, _
    12, 12, 12, 12, 13, 13, _
    13, 13, 13, 14, 14, 15, _
    15, 15, 15, 15, 15, 15, _
    16, 17, 17, 17, 18, 20, _
    20, 13, 13, 14, 7, 7, _
    7, 7, 7, 8, 8, 8, _
    8, 8, 8, 8, 8, 8, _
    8, 8, 8, 8, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    11, 11, 11, 11, 11, 11, _
    11, 12, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    12, 13, 13, 13, 13, 13, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 14, 14, 14, 14, _
    14, 14, 14, 14, 14, 14, _
    14, 14, 14, 15, 15, 15, _
    15, 15, 15, 15, 15, 16, _
    16, 16, 16, 16, 16, 16, _
    16, 16, 16, 16, 17, 17, _
    17, 17, 18, 13, 14, 8, _
    9, 9, 9, 11, 11, 11, _
    12, 12, 14, 16, 7, 8, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 9, 10, 10, 10, _
    10, 11, 12, 12, 12, 12, _
    13, 15, 16, 10, 5, 8, _
    11, 12, 12, 13, 13, 13, _
    14, 14, 8, 9, 12, 16, _
    16, 17, 4, 6, 6, 7, _
    8, 8, 8, 8, 8, 8, _
    8, 9, 9, 9, 9, 9, _
    9, 10, 10, 10, 10, 10, _
    10, 11, 11, 12, 13, 13, _
    14, 14, 16, 18, 18, 20, _
    21, 9, 9, 9, 9, 10, _
    10, 10, 10, 11, 11, 11, _
    12, 12, 14, 9, 10, 11, _
    12, 13, 14, 15, 15, 9, _
    13, 6, 8, 9, 11, 11, _
    12, 12, 12, 13, 14, 10, _
    11, 12, 14, 17, 10, 10, _
    12, 12, 12, 13, 15, 16, _
    16, 22, 5, 6, 7, 7, _
    9, 10, 10, 11, 13, 4, _
    11, 13, 12, 13, 15, 9, _
    15, 6, 7, 7, 7, 8, _
    8, 8, 8, 8, 8, 8, _
    8, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 9, _
    9, 9, 10, 10, 10, 10, _
    10, 10, 10, 10, 10, 11, _
    11, 11, 11, 11, 11, 12, _
    12, 12, 12, 12, 12, 12, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 14, 14, 14, 15, _
    15, 16, 17, 17, 17, 17, _
    17, 16, 7, 11, 12, 13, _
    13, 16, 9, 9, 12, 13, _
    16, 16, 4, 13, 13, 17, _
    12, 15, 16, 8, 10, 10, _
    10, 11, 11, 13, 14, 7, _
    8, 8, 8, 9, 9, 9, _
    9, 9, 10, 10, 11, 11, _
    11, 12, 12, 13, 13, 13, _
    13, 13, 13, 13, 13, 14, _
    15, 15, 15, 15, 16, 16, _
    16, 18, 21, 30, 4, 11, _
    13, 16, 8, 8, 9, 11, _
    12, 4, 7, 8, 8, 9, _
    9, 9, 9, 9, 9, 9, _
    10, 10, 12, 12, 13, 14, _
    16, 21, 7, 7, 9, 10, _
    10, 10, 10, 10, 10, 11, _
    13, 13, 14, 16, 16, 17, _
    17, 24, 4, 6, 8, 9, _
    12, 7, 8, 8, 9, 9, _
    9, 9, 9, 9, 9, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 10, 10, 11, 11, 11, _
    11, 11, 11, 11, 11, 12, _
    13, 13, 13, 13, 13, 14, _
    14, 14, 14, 14, 15, 15, _
    15, 16, 16, 17, 17, 18, _
    19, 18, 21, 11, 12, 17, _
    19, 8, 9, 9, 9, 9, _
    9, 10, 10, 10, 11, 11, _
    11, 11, 12, 12, 12, 12, _
    13, 13, 13, 13, 14, 14, _
    14, 14, 15, 15, 16, 16, _
    16, 17, 18, 7, 8, 9, _
    9, 9, 10, 12, 13, 17, _
    9, 10, 10, 12, 13, 14, _
    14, 16, 17, 17, 10, 16, _
    23, 5, 6, 6, 7, 7, _
    7, 8, 8, 8, 8, 8, _
    8, 9, 9, 9, 9, 9, _
    9, 9, 9, 9, 9, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    12, 13, 13, 13, 13, 13, _
    13, 13, 13, 13, 13, 13, _
    13, 14, 14, 14, 14, 14, _
    14, 14, 14, 14, 14, 14, _
    14, 15, 15, 15, 15, 15, _
    15, 15, 15, 16, 16, 16, _
    16, 16, 16, 16, 16, 17, _
    17, 17, 17, 17, 17, 17, _
    17, 17, 17, 18, 18, 18, _
    19, 20, 14, 9, 12, 13, _
    9, 9, 10, 10, 11, 12, _
    12, 12, 13, 13, 15, 15, _
    16, 17, 18, 22, 9, 11, _
    12, 13, 17, 10, 11, 7, _
    7, 8, 9, 9, 10, 10, _
    10, 10, 10, 10, 11, 11, _
    11, 11, 11, 12, 12, 12, _
    12, 12, 12, 13, 13, 13, _
    13, 13, 14, 14, 14, 14, _
    14, 15, 15, 16, 16, 16, _
    17, 17, 17, 17, 18, 18, _
    22, 5, 7, 7, 8, 8, _
    9, 9, 10, 10, 10, 10, _
    10, 10, 10, 10, 11, 11, _
    12, 12, 12, 12, 12, 12, _
    13, 13, 13, 13, 13, 13, _
    13, 14, 14, 14, 14, 14, _
    14, 14, 15, 15, 15, 15, _
    16, 16, 16, 16, 16, 16, _
    16, 16, 17, 18, 18, 18, _
    18, 21, 23, 11, 12, 8, _
    8, 9, 9, 10, 11, 13, _
    13, 14, 14, 14, 15, 5, _
    8, 9, 9, 9, 9, 10, _
    11, 11, 11, 11, 12, 12, _
    12, 12, 13, 13, 13, 13, _
    13, 13, 14, 14, 14, 14, _
    14, 15, 15, 16, 17, 19, _
    24, 5, 9, 11, 12, 9, _
    6, 9, 10, 12, 12, 13, _
    14, 15, 15, 16, 16, 22, _
    12, 8, 11, 11, 11, 12, _
    15, 16, 12, 9, 10, 10, _
    12, 12, 12, 12, 13, 15, _
    15, 16, 16, 16, 18, 20, _
    21, 6, 10, 7, 8, 9, _
    9, 9, 9, 10, 10, 10, _
    10, 10, 10, 10, 10, 10, _
    10, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 11, _
    12, 12, 12, 12, 12, 12, _
    12, 12, 12, 12, 12, 12, _
    13, 13, 13, 13, 13, 13, _
    13, 13, 14, 14, 14, 14, _
    14, 14, 14, 14, 14, 14, _
    14, 14, 14, 14, 15, 15, _
    15, 15, 15, 15, 15, 15, _
    15, 15, 15, 15, 15, 15, _
    16, 16, 16, 16, 16, 16, _
    16, 16, 16, 16, 17, 17, _
    17, 17, 17, 17, 17, 17, _
    17, 17, 17, 18, 18, 18, _
    18, 19, 19, 19, 19, 20, _
    21, 24, 26, 6, 14, 17, _
    17, 10, 8, 9, 9, 9, _
    10, 10, 10, 10, 10, 11, _
    11, 11, 11, 11, 11, 11, _
    11, 11, 11, 11, 11, 12, _
    12, 12, 12, 12, 12, 13, _
    13, 13, 13, 13, 13, 14, _
    14, 14, 14, 14, 14, 14, _
    14, 14, 14, 14, 14, 15, _
    15, 15, 15, 16, 16, 16, _
    16, 16, 17, 17, 17, 17, _
    17, 17, 18, 18, 18, 19, _
    19, 19, 8, 9, 11, 12, _
    10, 10, 9, 9, 9, 10, _
    10, 10, 10, 11, 11, 11, _
    11, 12, 13, 13, 14, 15, _
    17, 18, 19, 10, 10, 11, _
    13, 13, 19, 11, 11, 13, _
    15, 15, 16, 9, 10, 10, _
    11, 11, 12, 12, 13, 14, _
    14, 14, 15, 15, 15, 15, _
    15, 16, 18, 6, 15, 9, _
    11, 12, 14, 14, 15, 15, _
    16, 17, 6, 12, 14, 14, _
    17, 25, 11, 19, 9, 12, _
    13, 13, 23, 11, 15, 10, _
    11, 9, 10, 10, 10, 12, _
    12, 12, 13, 13, 13, 14, _
    14, 14, 14, 14, 15, 15, _
    16, 16, 16, 17, 17, 18, _
    19, 19, 19, 20, 20, 21, _
    7, 16, 10, 13, 14, 18, _
    18, 10, 10, 11, 11, 11, _
    12, 12, 12, 12, 12, 12, _
    12, 12, 13, 13, 13, 13, _
    13, 13, 13, 14, 14, 15, _
    15, 15, 15, 15, 15, 15, _
    15, 16, 16, 16, 16, 16, _
    16, 16, 16, 17, 17, 17, _
    19, 19, 19, 19, 19, 20, _
    21, 22, 22, 23, 24, 7, _
    12, 13, 13, 17, 17, 11, _
    11, 12, 12, 13, 13, 14, _
    15, 13, 18, 12, 11, 12, _
    12, 14, 14, 16, 16, 16, _
    19, 19, 20, 22, 10, 13, _
    13, 13, 14, 14, 15, 15, _
    17, 8, 12, 20, 8, 10, _
    10, 13, 14, 18, 18, 14, _
    14, 15, 16, 17, 18, 18, _
    21, 24, 12, 12, 13, 13, _
    13, 13, 13, 13, 13, 13, _
    14, 14, 14, 14, 14, 14, _
    14, 14, 15, 15, 15, 15, _
    15, 15, 15, 15, 15, 15, _
    16, 16, 16, 16, 16, 16, _
    16, 16, 16, 16, 16, 16, _
    17, 17, 17, 17, 17, 17, _
    17, 17, 18, 18, 18, 18, _
    18, 19, 19, 19, 19, 19, _
    19, 20, 20, 20, 21, 14, _
    14, 15, 15, 16, 18, 18, _
    18, 19, 19, 13, 13, 14, _
    14, 14, 15, 15, 17, 17, _
    18, 18, 19, 19, 22, 14, _
    14, 15, 16, 16, 17, 19, _
    12, 15, 18, 22, 22, 10, _
    13, 14, 15, 15, 16, 16, _
    16, 18, 19, 20, 23, 25, _
    14, 15, 17, 13, 16, 16, _
    17, 19, 19, 21, 23, 17, _
    17, 17, 18, 18, 19, 20, _
    20, 20, 20, 21, 17, 18, _
    20, 23, 23, 16, 17, 23}
#End Region
        Public Function OnlyChinese(ByVal mText As String) As String
            Return System.Text.RegularExpressions.Regex.Replace(mText, "[^\u4e00-\u9fa5]", "")
        End Function

        ''' <summary>
        ''' 通过编码计算得到该汉字的偏移量,通过偏移量再在上面的笔画列表中
        ''' 得到该汉字的笔画数.
        ''' </summary>
        ''' <param name="c1"></param>
        ''' <param name="c2"></param>
        ''' <returns></returns>
        Public Function GetGB2312StrokeCountM(c1 As Integer, c2 As Integer) As Integer
            Dim OffSet As Integer
            If c1 < &HB0 OrElse c1 > &HF7 OrElse c2 < &HA1 OrElse c2 > &HFE Then
                ' 不是一个有效的GB2312汉字字符
                Return -1
            End If
            OffSet = (c1 - &HB0) * (&HFE - &HA0) + (c2 - &HA1)
            Return gb2312_stroke_count(OffSet)
        End Function

        ''' <summary>
        ''' 获取字符串中所有汉字的笔画总和
        ''' </summary>
        ''' <param name="cnWords">字符串</param>
        ''' <returns></returns>
        Public Function GetStrokeCount(cnWords As String) As Integer
            '去除非中文
            cnWords = Regex.Replace(cnWords, "[^一-龥]", "")
            Dim count As Integer = 0
            Dim i As Integer = 0
            While i < cnWords.Length
                Dim TheArray As Byte() = System.Text.Encoding.[Default].GetBytes(cnWords.Substring(i, 1))
                Dim returnCount As Integer = GetGB2312StrokeCountM(TheArray(0), TheArray(1))
                If returnCount > 0 Then
                    count += returnCount
                End If
                System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
            End While
            Return count
        End Function
    End Module
End Namespace
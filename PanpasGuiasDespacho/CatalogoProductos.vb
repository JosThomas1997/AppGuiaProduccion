Module CatalogoProductos
    ' Catalogo centralizado de tipos de torta y sus codigos de producto.
    ' Antes esta misma lista estaba duplicada en FormProductos.vb, FormProducc_Productos.vb
    ' y FormInformeProducto.vb. Ahora vive en un solo lugar: para agregar, quitar o corregir
    ' un tipo de torta (o sus codigos), solo hay que editar aqui.
    Public ReadOnly tipoTortas As (Texto As String, Codigo As String())() = {
        ("Holandesas", {"1010", "1011", "1012", "1013"}),
        ("Yoghurt", {"1009"}),
        ("Dulce de Leche", {"1028", "1038", "1048", "1058", "1068", "1078", "1088"}),
        ("Chocolate", {"1027", "1037", "1047", "1057", "1067", "1077", "1087"}),
        ("Piña", {"1021", "1031", "1041", "1051", "1061", "1071", "1081"}),
        ("Selva Negra", {"1022", "1032", "1042", "1052", "1062", "1072", "1082"}),
        ("Hoja Manjar", {"1100", "1101", "1102", "1103"}),
        ("Lucuma", {"1023", "1033", "1043", "1053", "1063", "1073", "1083"}),
        ("Tres Leches", {"1110", "1111", "1112"}),
        ("Mocka", {"1024", "1034", "1044", "1054", "1064", "1074", "1084"}),
        ("Panqueque", {"1114", "1116"}),
        ("Durazno", {"1026", "1036", "1046", "1056", "1066", "1076", "1086"}),
        ("Bizcocho Manjar", {"1025", "1035", "1045", "1055", "1065", "1075", "1085"}),
        ("Panpas", {"1096"})
    }
End Module

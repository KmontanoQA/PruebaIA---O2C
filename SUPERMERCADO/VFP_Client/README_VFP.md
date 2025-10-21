# Cliente Visual FoxPro - Sistema ERP O2C

Cliente de ejemplo en Visual FoxPro para consumir la API REST del sistema ERP O2C.

## 📋 Requisitos

- Visual FoxPro 9.0 o superior
- Windows con soporte para WinHTTP
- API REST ejecutándose en `http://localhost:5000`

## 🚀 Instalación

1. Copiar los archivos `.prg` a una carpeta local
2. Asegurarse de que la API esté ejecutándose
3. Ejecutar `login.prg` desde Visual FoxPro

## 📁 Archivos

- **login.prg**: Formulario de autenticación JWT
- **main_menu.prg**: Menú principal con acceso a módulos
- **README_VFP.md**: Esta documentación

## 🔐 Credenciales de Prueba

```
Email: admin@supermercado.com
Password: admin123
```

## 💻 Uso Básico

### 1. Iniciar Sesión

```foxpro
DO login.prg
```

El formulario solicitará email y password. Al autenticarse exitosamente, se almacenará el token JWT en la variable pública `gcToken`.

### 2. Variables Públicas

El sistema utiliza las siguientes variables públicas:

- `gcAPIBaseURL`: URL base de la API (http://localhost:5000/api/v1)
- `gcToken`: Token JWT de autenticación
- `gcUserEmail`: Email del usuario autenticado
- `gcUserRole`: Rol del usuario (Admin, Seller, User)

### 3. Hacer Peticiones HTTP

#### Ejemplo GET con Autenticación

```foxpro
LOCAL loHTTP, lcResponse

loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
loHTTP.Open("GET", gcAPIBaseURL + "/products", .F.)
loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
loHTTP.SetRequestHeader("Content-Type", "application/json")
loHTTP.Send()

IF loHTTP.Status = 200
    lcResponse = loHTTP.ResponseText
    ? lcResponse
ELSE
    ? "Error:", loHTTP.Status, loHTTP.StatusText
ENDIF
```

#### Ejemplo POST - Crear Pedido

```foxpro
LOCAL loHTTP, lcJSON, lcResponse

* Preparar JSON del pedido
TEXT TO lcJSON NOSHOW
{
  "customerId": 1,
  "notes": "Pedido desde VFP",
  "orderLines": [
    {
      "productId": 1,
      "qty": 10
    },
    {
      "productId": 2,
      "qty": 5
    }
  ]
}
ENDTEXT

loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
loHTTP.Open("POST", gcAPIBaseURL + "/orders", .F.)
loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
loHTTP.SetRequestHeader("Content-Type", "application/json")
loHTTP.Send(lcJSON)

IF loHTTP.Status = 201
    lcResponse = loHTTP.ResponseText
    ? "Pedido creado:", lcResponse
ELSE
    ? "Error:", loHTTP.Status
ENDIF
```

## 🔄 Flujo de Trabajo

### Crear y Confirmar Pedido

```foxpro
* 1. Crear pedido
lcJSON = '{"customerId":1,"orderLines":[{"productId":1,"qty":10}]}'
loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
loHTTP.Open("POST", gcAPIBaseURL + "/orders", .F.)
loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
loHTTP.SetRequestHeader("Content-Type", "application/json")
loHTTP.Send(lcJSON)
lcResponse = loHTTP.ResponseText

* 2. Extraer ID del pedido (parseo simple)
lnOrderId = VAL(STREXTRACT(lcResponse, '"id":', ','))

* 3. Confirmar pedido
loHTTP.Open("POST", gcAPIBaseURL + "/orders/" + TRANSFORM(lnOrderId) + "/confirm", .F.)
loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
loHTTP.Send()

IF loHTTP.Status = 200
    ? "Pedido confirmado exitosamente"
ENDIF
```

### Crear Factura desde Pedido

```foxpro
* Preparar datos de factura
lcJSON = '{"orderId":' + TRANSFORM(lnOrderId) + ;
         ',"dueDate":"' + TTOC(DATE() + 30, 3) + '"}'

loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
loHTTP.Open("POST", gcAPIBaseURL + "/invoices", .F.)
loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
loHTTP.SetRequestHeader("Content-Type", "application/json")
loHTTP.Send(lcJSON)

IF loHTTP.Status = 201
    ? "Factura creada exitosamente"
ENDIF
```

## 📊 Obtener Reportes

### Reporte de Ventas

```foxpro
LOCAL ldDateFrom, ldDateTo, lcURL

ldDateFrom = DATE() - 30
ldDateTo = DATE()

lcURL = gcAPIBaseURL + "/reports/sales?dateFrom=" + ;
        TTOC(ldDateFrom, 3) + "&dateTo=" + TTOC(ldDateTo, 3)

loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
loHTTP.Open("GET", lcURL, .F.)
loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
loHTTP.Send()

? loHTTP.ResponseText
```

### Reporte de Cartera

```foxpro
loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
loHTTP.Open("GET", gcAPIBaseURL + "/reports/receivables", .F.)
loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
loHTTP.Send()

? loHTTP.ResponseText
```

## 🛠️ Funciones Auxiliares

### Parsear JSON Simple

```foxpro
FUNCTION ExtractJSONValue(tcJSON, tcKey)
    LOCAL lcValue, lnStart, lnEnd
    
    lnStart = AT('"' + tcKey + '":', tcJSON)
    IF lnStart = 0
        RETURN ""
    ENDIF
    
    lnStart = AT(':"', tcJSON, lnStart) + 2
    lnEnd = AT('"', tcJSON, lnStart)
    
    IF lnEnd > lnStart
        lcValue = SUBSTR(tcJSON, lnStart, lnEnd - lnStart)
        RETURN lcValue
    ENDIF
    
    RETURN ""
ENDFUNC
```

### Manejo de Errores HTTP

```foxpro
FUNCTION HandleHTTPError(toHTTP)
    LOCAL lcMessage
    
    DO CASE
        CASE toHTTP.Status = 401
            lcMessage = "No autorizado. Token inválido o expirado."
        CASE toHTTP.Status = 400
            lcMessage = "Solicitud incorrecta: " + toHTTP.ResponseText
        CASE toHTTP.Status = 404
            lcMessage = "Recurso no encontrado"
        CASE toHTTP.Status = 409
            lcMessage = "Conflicto: " + toHTTP.ResponseText
        CASE toHTTP.Status >= 500
            lcMessage = "Error del servidor: " + toHTTP.StatusText
        OTHERWISE
            lcMessage = "Error HTTP " + TRANSFORM(toHTTP.Status)
    ENDCASE
    
    MESSAGEBOX(lcMessage, 48, "Error")
    RETURN lcMessage
ENDFUNC
```

## 📝 Notas Importantes

1. **Token Expiration**: Los tokens JWT expiran en 1 hora. Implementar renovación automática.

2. **Manejo de Errores**: Siempre verificar `loHTTP.Status` antes de procesar respuestas.

3. **Formato de Fechas**: Usar formato ISO 8601 para fechas: `YYYY-MM-DDTHH:MM:SS`

4. **Encoding**: Asegurarse de que los caracteres especiales estén correctamente codificados.

5. **CORS**: La API tiene CORS habilitado para todos los orígenes en desarrollo.

## 🔍 Debugging

### Ver Request/Response Completo

```foxpro
loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
loHTTP.Open("GET", gcAPIBaseURL + "/products", .F.)
loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
loHTTP.Send()

? "Status:", loHTTP.Status
? "StatusText:", loHTTP.StatusText
? "Response:", loHTTP.ResponseText
? "Headers:", loHTTP.GetAllResponseHeaders()
```

## 📚 Recursos Adicionales

- Documentación API: http://localhost:5000/swagger
- Repositorio: Ver README.md principal
- Soporte: soporte@supermercado.com

## 🐛 Problemas Comunes

### Error: "No se puede crear objeto HTTP"

**Solución**: Verificar que WinHTTP esté instalado en Windows.

### Error 401: Unauthorized

**Solución**: Token expirado o inválido. Hacer login nuevamente.

### Error de Conexión

**Solución**: Verificar que la API esté ejecutándose en http://localhost:5000

---

**Versión:** 1.0.0  
**Compatibilidad:** Visual FoxPro 9.0+

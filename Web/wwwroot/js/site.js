// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Solo numeros
const _MS_PER_DAY = 1000 * 60 * 60 * 24;
$(document).ready(function () {
    $('body').on('keyup', '.input-number', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    var titulo = $("#titleLayout").text();
    $("#titleLayout").text(titulo + " | Inventory Management");

    //Quitar enter en inputs
    $('body').on('keyup keypress', 'input', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            e.preventDefault();
            return false;
        }
    });

    if (isMobile()) {
        $(".btn-group-devided").removeClass("btn-group");
        $(".portlet-title").addClass("text-center");
    }
    else {
        $(".btn-group-devided").addClass("btn-group");
        $(".portlet-title").removeClass("text-center");
    }
});

Date.prototype.withoutTime = function () {
    var d = new Date(this);
    d.setHours(0, 0, 0, 0);
    return d;
}

function isEmail(email) {
    if (email.length > 0) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (!regex.test(email)) {
            swal("¡Advertencia!", "El correo electrónico ingresado no es válido", "/images/menu/error.png");
        }
    }
}

function removeEquals(a) {
    return a.sort().filter(function (item, pos, ary) {
        return !pos || item != ary[pos - 1];
    })
}

function validarCorreo(correo) {
    var emailRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;
    return emailRegex.test(correo);
}

function roundNumber(num, scale) {
    if (!("" + num).includes("e") && scale != 2) {
        return +(Math.round(num + "e+" + scale) + "e-" + scale);
    }
    else if (!("" + num).includes("e") && scale == 2) {
        num = parseFloat(num);
        return Math.round((num + Number.EPSILON) * 100) / 100;
    }
    else {
        var arr = ("" + num).split("e");
        var sig = ""
        if (+arr[1] + scale > 0) {
            sig = "+";
        }
        return +(Math.round(+arr[0] + "e" + sig + (+arr[1] + scale)) + "e-" + scale);
    }
}

function parseNumStr(num, scale, prefix) {
    return parseFloat(num).toFixed(scale).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

function getParams() {
    var filtros = new Object();
    var InfoAdicional = $("#InfoAdicional").val();
    var estadoAnticipo = parseInt($("#estadoAnticipo").val());
    var periodo = parseInt($("#selectPeriodo option:selected").val());
    var fechaInicio = $("#FechaInicio").datepicker("getDate");
    var fechaFinal = $("#FechaFinal").datepicker("getDate");
    var fechaMes = $("#FechaMes").datepicker("getDate");
    var cliente = $("#inputIdCliente").val();
    var proveedor = $("#inputIdProveedor").val();
    var fecha = new Date();
    if (periodo !== 5) {
        fechaInicio = fecha;
        fechaFinal = fecha;
    }

    if (periodo !== 10)
        fechaMes = fecha;

    if (!isNaN(estadoAnticipo)) filtros.estadoAnticipo = estadoAnticipo;
    if (InfoAdicional) filtros.infoAdicional = InfoAdicional;
    filtros.periodo = periodo;
    filtros.fechaInicio = fechaInicio;
    filtros.fechaFinal = fechaFinal;
    filtros.fechaMes = fechaMes;
    if ($("#inputBuscarClienteFiltro").val() === "")
        cliente = 0;

    if ($("#inputBuscarProveedorFiltro").val() === "")
        proveedor = 0;

    filtros.clienteId = cliente;
    filtros.proveedorId = proveedor;
    return JSON.stringify(filtros);
}

function formatDate(d = new Date(), hora = false) {
    if (hora)
        return ('0' + (d.getDate())).slice(-2) + "/" + ('0' + (d.getMonth() + 1)).slice(-2) + "/" + d.getFullYear() + " " + d.toLocaleTimeString();
    return ('0' + (d.getDate())).slice(-2) + "/" + ('0' + (d.getMonth() + 1)).slice(-2) + "/" + d.getFullYear();
}

function reemplazarAcentos(cadena) {
    var chars = {
        "&#xE1;": "a", "&#xE9;": "e", "&#xED;": "i", "&#xF3;": "o", "ú": "u",
        "à": "a", "è": "e", "ì": "i", "ò": "o", "ù": "u", "ñ": "n",
        "&#xC1;": "A", "É": "E", "Í": "I", "Ó": "O", "Ú": "U",
        "À": "A", "È": "E", "Ì": "I", "Ò": "O", "Ù": "U", "Ñ": "N"
    }
    var exprA = "&#xC1;";
    var expra = "&#xE1;";
    var expre = "&#xE9;";
    var expri = "&#xED;";
    var expro = "&#xF3;";
    var res = cadena.replace(exprA, function (e) { return chars[e]; });
    res = res.replace(expra, function (e) { return chars[e]; });
    res = res.replace(expre, function (e) { return chars[e]; });
    res = res.replace(expri, function (e) { return chars[e]; });
    res = res.replace(expro, function (e) { return chars[e]; });

    let words = toWords(res);
    return toCamelCase(words);
}

function toWords(input) {
    var regex = /[A-Z\xC0-\xD6\xD8-\xDE]?[a-z\xDF-\xF6\xF8-\xFF]+|[A-Z\xC0-\xD6\xD8-\xDE]+(?![a-z\xDF-\xF6\xF8-\xFF])|\d+/g;
    return input.match(regex);
}

function toCamelCase(inputArray) {
    let result = "";
    for (let i = 0, len = inputArray.length; i < len; i++) {
        let currentStr = inputArray[i];
        let tempStr = currentStr.toLowerCase();
        if (i != 0) {
            tempStr = tempStr.substr(0, 1).toUpperCase() + tempStr.substr(1);
        }
        result += tempStr;
    }

    return result;
}

function isMobile() {
    try {
        document.createEvent("TouchEvent");
        return true;
    }
    catch (e) {
        return false;
    }
}

function fromEntidadFinanciera(control, url, params = null, value = null) {
    control.empty();
    $.post(url, params, function (data) {
        if (data !== null && !jQuery.isEmptyObject(data)) {
            control.append($('<option />', {
                value: null,
                text: "Seleccione..."
            }));
        }
        $.each(data, function (index, item) {
            control.append($('<option />', {
                value: item.codigo,
                text: item.nombre,
                selected: item.codigo === value
            }));
        });
    });
}

function fromGeonames(control, url, params = null, value = null) {
    control.empty();
    $.post(url, params, function (data) {
        if (data !== null && !jQuery.isEmptyObject(data)) {
            control.append($('<option />', {
                value: null,
                text: ""
            }));
        }
        $.each(data, function (index, item) {
            control.append($('<option />', {
                value: item.id,
                text: item.nombre,
                selected: item.id == value
            }));
        });
    });
}

function dateDiffInDays(a, b) {
    // Discard the time and time-zone information.
    const utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
    const utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());

    return Math.floor(Math.abs(utc2 - utc1) / _MS_PER_DAY);
}

function roundAwayFromZero(value, decimals = 2) {
    const factor = Math.pow(10, decimals);
    const sign = value < 0 ? -1 : 1;

    let [intPart, decPart = ""] = Math.abs(value).toString().split(".");
    decPart = decPart.padEnd(decimals + 1, "0");

    const cut = decPart.slice(0, decimals);
    const nextDigit = parseInt(decPart.charAt(decimals) || "0", 10);

    let result = Number(intPart + "." + cut);

    if (nextDigit >= 5) {
        result += 1 / factor;
    }

    return sign * Number(result.toFixed(decimals));
}

$.fn.getNodeValue = function (node, def = "") {
    try {
        return this[0].getElementsByTagName(node)[0].childNodes[0].nodeValue;
    } catch (e) {
        return def;
    }
}
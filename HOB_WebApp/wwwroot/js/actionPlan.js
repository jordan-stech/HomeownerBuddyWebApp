document.getElementById("steps").onkeyup = function (e) {
    var evt = e ? e : event;
    if ((evt.keyCode && evt.keyCode != 13) || evt.which != 13)
        return;
    var elm = evt.target ? evt.target : evt.srcElement;
    var lines = elm.value.split("\n");
    for (var i = 0; i < lines.length; i++)
        lines[i] = lines[i].replace(/(\d+\.\s|^)/, (i + 1) + ". ");
    elm.value = lines.join("\n");
}  
var n2MediaSelection = (function () {

    var editableMediaControls = {};

    function openMediaSelectorPopup(popupUrl, tbId, popupOptions, preferredSize, availableExtensions) {
	    var tb = document.getElementById(tbId);
	    window.open(popupUrl
			    + '&tbid=' + tbId
			    + '&preferredSize=' + preferredSize
			    + '&selectedUrl=' + encodeURIComponent(tb.value)
			    + '&selectableExtensions=' + availableExtensions,
	    null,
	    popupOptions);
    }

    function initializeEditableMedia() {

        var mcs = $(".EditableMedia"), j, len, node, inp, viewBtn;


        for (j = 0, len = mcs.length; j < len; j += 1) {
            node = mcs[j];
            inp = node.getElementsByTagName("input")[0];
            if (inp) {

                $(inp).on("focus blur keyup paste input change", inputChange);
                viewBtn = node.getElementsByClassName("showLayoverButton")[0];
                editableMediaControls[inp.id] = { isVisible: false, btnShowId: viewBtn };

                if (inp.value) {
                    viewBtn.style.display = "inline-block";
                    editableMediaControls[inp.id].isVisible = true;
                } else {
                    viewBtn.style.display = "none";
                }
            }
        }
    }

    function inputChange(e) {
        var me = e.target;
        showHideViewerButton(me.id, me.value);
    }

    function showHideViewerButton(inputId, doShow) {
    	if (!editableMediaControls[inputId])
    		return;
        if (doShow) {
            if (!editableMediaControls[inputId].isVisible) {
                editableMediaControls[inputId].isVisible = true;
                editableMediaControls[inputId].btnShowId.style.display = "inline-block";
            }
        } else {
            if (editableMediaControls[inputId].isVisible) {
                editableMediaControls[inputId].isVisible = false;
                editableMediaControls[inputId].btnShowId.style.display = "none";
            }
        }
    }

    function clearMediaSelector(inputId) {
        var input = document.getElementById(inputId);
        input.value = "";
        showHideViewerButton(inputId, false);
    }

    function setMediaSelectorValue(inputId, val) {
        var input = document.getElementById(inputId);
        input.value = val;
        showHideViewerButton(inputId, val);
    }

    function showMediaSelectorOverlay(inputId) {
        var url,
            input,
            ovly,
            close,
            isImgBool,
            img,
            noImg;

        input = document.getElementById(inputId);
        if (input === null) { return false; }

        url = input.value;
        if (!url) { return false; }

        isImgBool = new RegExp("\\b(\.[png|gif|jpg|jpeg|webp]+)$", "i").test(url);

        if (!isImgBool) {
            window.open(url, "_blank");
            return false;
        }

        ovly = document.createElement("div");
        close = document.createElement("div");
        noImg = document.createElement("div");
        img = new Image();

        ovly.setAttribute("style", "position:fixed; top:0; left:0; right:0; bottom:0; background:#eee; background:rgba(235,235,235,0.8); z-index:20000;");
        img.setAttribute("style", "position:absolute; top:5%; left:5%; max-width:90%; max-height:90%; width:auto; height:auto; display:block;");
        noImg.setAttribute("style", "position:absolute; top:50%; left:50%; width:100px; height:50px; font-size:20px; color:#ff0000; margin:-25px 0 0 -50px;");
        close.setAttribute("style", "position:absolute; top:20px; right:20px; line-height:35px; height:40px; width:40px; color:black; font-size:30px; text-align:center; background:#ccc;z-index:10; border-radius:20px; cursor:pointer;");

        close.innerHTML = "&times;"
        close.onclick = function (e) {
            document.body.removeChild(document.getElementById("image-overlay"));
        };

        noImg.className = "fa fa-eye-slash";

        ovly.id = "image-overlay";

        img.onload = function () {
            ovly.appendChild(img);
        }
        img.onerror = function () {
            ovly.appendChild(noImg);
        }
        img.src = url + "?v=" + (new Date().getTime());

        ovly.appendChild(close);
        document.body.appendChild(ovly);

        return false;

    }

    return {
        openMediaSelectorPopup: openMediaSelectorPopup,
        clearMediaSelector: clearMediaSelector,
        showMediaSelectorOverlay: showMediaSelectorOverlay,
        setMediaSelectorValue: setMediaSelectorValue,
        initializeEditableMedia: initializeEditableMedia
    }

}());
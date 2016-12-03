var n2MediaBrowser = (function (jQ) {

    var fileBrowser, clickEvent = "click", dp = null;

    function parsePropertiesToPattern(pattern, obj, i) {
        var stemp = String(pattern);

        if (i !== null) { stemp = stemp.replace(new RegExp("{{i}}", "g"), i); }

        for (var property in obj) {
            stemp = stemp.replace(new RegExp("{{" + property + "}}", "g"), obj[property]);
        }

        return stemp;
    };

    ajaxError = function (xhr, status, p3, p4) {
        var err = "Error " + " " + status + " " + p3 + " " + p4;
        if (xhr.responseText && xhr.responseText[0] == "{")
            err = JSON.parse(xhr.responseText).status;
        console.log(err);
    }

    function sizeReadble(size) {
        return size > 1024 ? (size > 1048576 ? (String(Math.round(size / 1048576 * 10) / 10).toLocaleString() + " MBs")
                        : (String(Math.round(size / 1024 * 10) / 10).toLocaleString()) + " KBs")
                        : size + " bytes"
    }

    fileBrowser = {
        initialized: false,
        cur: -1,
        objs: [],
        list: null,
        listLis: null,
        tabs: null,
        tabsCtrl: null,
        tabsCtrlLis: null,
        curTab: 0,
        btn: null,
        btnClose: null,
        btnSearch: null,
        qSearch: null,
        lastSearch: "",
        divUploadSection: null,
        divDetails: null,
        divLayover: null,
        divLayoverCont: null,
        divLayoverUl: null,
        divLayoverContinueBtn: null,
        progressBar: null,
        altTextDirty: false,
        interv: -1,
        continueUploadInit: false,
        upsBtns: [],
        i18Labels: {},
        patternInfoImg: "<label>{{i18date}}</label>{{modified}}<label>{{i18size}}</label>{{size}}<label>{{i18url}}</label>{{url}}",
        patternInfoFile: "<label>{{i18url}}</label>{{url}}",
        preferredSize: "",
        lastPath: "",
        divBreadcrumb: null,
        history: { breadcrumb: "", list: "" },
        ajaxUrl: "",
        selectedUrl: "",
        lblMessage: null,
        lblMessageUpload: null,

        init: function (force) {
            var me = this, fbMainDiv;

            if (me.initialized && !force) return;

            fbMainDiv = document.getElementById("fileBrowser-main-div");
            if (fbMainDiv === null) return;

            me.list = document.getElementById("browser-files-list-ul");
            me.tabs = document.getElementsByClassName("browser-files-section");
            me.tabsCtrl = document.getElementById("tabsCtrl");
            me.btn = document.getElementById("btn-select");
            me.btnSearch = document.getElementById("btn-search");
            me.qSearch = document.getElementById("input-group-q");
            me.btnSearchClean = document.getElementById("btn-search-clean");
            me.divDetails = document.getElementById("info-div-details");
            me.divLayover = document.getElementById("browser-files-layover");
            me.divLayoverCont = document.getElementById("browser-files-layover-cont");
            me.divLayoverUl = document.getElementById("browser-files-layover-ul");
            me.divLayoverContinueBtn = document.getElementById("btn-continue-upload");
            me.progressBar = document.getElementById("file-upload-ajax-progres");
            me.divBreadcrumb = document.getElementById("dirs-breadcrumb");
            me.divUploadSection = document.getElementById("browser-upload-file");
            me.lblMessage = document.getElementById("lblMessage");
            me.lblMessageUpload = document.getElementById("lblMessageUpload");
            me.btnClose = document.getElementById("btn-cancel");

            me.i18Labels = {
                i18size: me.list.getAttribute("data-i18size"),
                i18date: me.list.getAttribute("data-i18date"),
                i18url: me.list.getAttribute("data-i18url"),
                i18keep: me.divLayoverUl.getAttribute("data-i18keep"),
                i18repl: me.divLayoverUl.getAttribute("data-i18repl"),
                i18ignr: me.divLayoverUl.getAttribute("data-i18ignr"),
            };
            me.preferredSize = me.list.getAttribute("data-preferredsize");
            me.lastPath = me.list.getAttribute("data-path");
            me.selectedUrl = me.list.getAttribute("data-selurl");

            if (me.list != null) {
                me.listLis = jQ(me.list).children("li");

                jQ(me.list).click(me.sel);

                jQ(me.btnSearch).on(clickEvent, me.loadDataSearch);
                jQ(me.btnSearchClean).on(clickEvent, me.loadDataSearchClean);
                jQ(me.qSearch).keypress(function (e) {
                    var key = e.which;
                    if(key == 13)  
                    {
                        jQ(me.btnSearch).trigger(clickEvent);
                        e.stopPropagation();
                        e.preventDefault();
                    }
                }); 

                jQ(me.divDetails).on(clickEvent, me.clicksInfoRespond);
                jQ(me.btn).on(clickEvent, me.selectFileToParent);

                jQ(me.divBreadcrumb).click(me.clicksBreadcrumbRespond);
                me.history = { breadcrumb: me.divBreadcrumb.innerHTML, list: me.list.innerHTML };

                me.ajaxUrl = (fileBrowser.list).getAttribute("data-baseajax");
                me.selPrevioursUrl();
            }

            if (me.tabsCtrl != null) {
                me.tabsCtrlLis = (me.tabsCtrl).getElementsByTagName("li"); 
                jQ(me.tabsCtrl).click(me.showTab);
            }

            jQ(me.btnClose).click(me.closeWindow);

            me.initUpload();
            me.initialized = true;

        },
        toggleMainBtn: function (on) {
            !on ? fileBrowser.btn.setAttribute("disabled", "disabled") : fileBrowser.btn.removeAttribute("disabled");
        },
        showTab: function (e) {
            var t = e.target, p;
            if (t.tagName.toUpperCase() !== "A") { return; }
            e.stopPropagation();
            e.preventDefault();
            p = Number(t.href.split("#")[1]);
            if (p === fileBrowser.curTab) { return; }

            fileBrowser.showTabNum(p);
        },
        showTabNum: function (p) {
            var ts = fileBrowser.tabsCtrlLis, c = fileBrowser.curTab, tabs = fileBrowser.tabs;
            //ChangeTab
            tabs[c].style.display = "none";
            jQ(ts[c]).removeClass("active");
            tabs[p].style.display = "block";
            jQ(ts[p]).addClass("active");
            fileBrowser.curTab = p;

            //Deselect any
            if (fileBrowser.cur >= 0) { jQ(fileBrowser.listLis[fileBrowser.cur]).removeClass("selected"); }
            fileBrowser.showInfo(-1);

            if (p === 1) {
                (fileBrowser.progressBar).parentNode.style.display = "none";
                (fileBrowser.progressBar).style.display.width = "0%";
            }
        },
        selPrevioursUrl: function () {
            var j, k, len, lenk, li,
                selUrl = fileBrowser.selectedUrl,
                foundLi, foundEm, imgSizes;

            if (selUrl) {
                for (j = 0, len = fileBrowser.listLis.length; j < len; j += 1) {
                    li = fileBrowser.listLis[j];
                    if (jQ(li).hasClass("dir")) continue;
                    if (jQ(li).data("url") === selUrl) {
                        foundLi = li;
                    } else {
                        imgSizes = jQ("em", li);
                        for (k = 0, lenk = imgSizes.length; k < lenk; k += 1) {
                            if (jQ(imgSizes[k]).data("url") === selUrl) {
                                foundLi = li;
                                foundEm = imgSizes[k];
                            }
                        }
                    }
                }

                if (foundLi) {
                    if (fileBrowser.cur >= 0) {
                        jQ(fileBrowser.listLis[fileBrowser.cur]).removeClass("selected");
                    }
                    jQ(foundLi).addClass("selected");
                    if (foundEm) {
                        jQ(foundEm).siblings().removeClass("selected");
                        jQ(foundEm).addClass("selected");
                    }
                    fileBrowser.showInfo(Number(foundLi.getAttribute("data-i")));
                }
            }
        },
        sel: function (e) {
            var t = e.target, p = e.target.parentNode, tagName = t.tagName.toUpperCase(), type, newDir, dpSize;
            if (tagName !== "IMG" && tagName !== "LI" && tagName !== "SPAN" && tagName !== "EM") { return; }

            e.stopPropagation();

            switch (tagName) {
                case "LI": p = e.target;
                case "IMG":
                case "SPAN":
                    type = jQ(p).hasClass("dir") ? 0 : (jQ(p).hasClass("image") ? 1 : 2);
                    if (type > 0) {
                        if (jQ(p).hasClass("selected")) {
                            jQ(p).removeClass("selected");
                            fileBrowser.showInfo(-1);
                        } else {
                            if (fileBrowser.cur >= 0) {
                                jQ(fileBrowser.listLis[fileBrowser.cur]).removeClass("selected");
                            }
                            jQ(p).addClass("selected");
                            fileBrowser.showInfo(Number(p.getAttribute("data-i")));
                        }
                    } else {
                        newDir = jQ(p).data("url");
                        fileBrowser.loadData(newDir, null);
                    }
                    break;
                case "EM":
                    jQ(t).siblings().removeClass("selected");
                    jQ(t).addClass("selected");
                    fileBrowser.showInfo(fileBrowser.cur);
                    break;
            }
        },
        selectFileToParent: function (e) {
            var cke = (fileBrowser.list).getAttribute("data-ckeditor"),
                ckeFn = (fileBrowser.list).getAttribute("data-ckeditorfuncnum"),
                mediaCtrl = window.tbid;

            //ckEditor
            if (cke !== "" && ckeFn !== "" && dp !== null
                && window.opener && window.opener.CKEDITOR && window.opener.CKEDITOR.tools && window.opener.CKEDITOR.tools.callFunction) {
                window.opener.CKEDITOR.tools.callFunction(ckeFn, dp.url, function () {
                    // Get the reference to a dialog window.
                    var element,
                        dialog = this.getDialog();
                    // Check if this is the Image dialog window.
                    if (dialog.getName() == 'image') {
                        // Get the reference to a text field that holds the "alt" attribute.
                        element = dialog.getContentElement('info', 'txtAlt');
                        // Assign the new value.
                        if (element)
                            element.setValue(dp.altText);
                    }
                });
            }//ckEditor

            //mediaControl
            if (mediaCtrl !== "" && dp !== null && window.opener && window.opener.n2MediaSelection && window.opener.n2MediaSelection.setMediaSelectorValue) {
                window.opener.n2MediaSelection.setMediaSelectorValue(mediaCtrl, dp.url);
            }

            document.cookie = "lastMediaSelection=" + dp.url + "; path=/;";
        
            window.close();
        },
        closeWindow: function (e) {
            e.stopPropagation();
            e.preventDefault();
            window.close();
        },
        clicksInfoRespond: function (e) {
            var t = e.target;
            switch (t.id) {
                case "btn-alt-save":
                    fileBrowser.updateAltText(e);
                    break;
                case "btn-delete-file":
                    fileBrowser.deleteFile(e);
                    break;
            }
        },
        clicksBreadcrumbRespond: function(e){
            var t = e.target, newDir, tagName = t.tagName;
            if (tagName === "LI") {
                newDir = t.getAttribute("data-url");
                fileBrowser.showInfo(-1);
                fileBrowser.loadData(newDir, null);
            }
        },
        showInfo: function (i) {
            var liDp, url, size, date, html = "", me = fileBrowser, imageSizes, ems;

            me.cur = i;
            if (i < 0) {
                me.toggleMainBtn(false);
                dp = null;
            } else {
                me.toggleMainBtn(true);
                liDp = (me.listLis)[i];
                url = liDp.getAttribute("data-url");
                size = Number(liDp.getAttribute("data-size"));
                date = new Date(liDp.getAttribute("data-date"));
                dp = {
                    modified: date.toLocaleDateString() + "<br />" + date.toLocaleTimeString(),
                    size: sizeReadble(size),
                    url: url,
                    i: liDp.getAttribute("data-i"),
                    isImage: liDp.getAttribute("data-isimage") === "true"
                };

                imageSizes = jQ(".image-sizes", liDp)
                if (jQ(imageSizes).length > 0 && jQ("em.selected", imageSizes).length > 0) {
                    ems = jQ("em.selected", imageSizes);
                    dp.size = sizeReadble(Number(jQ(ems).data("size")));
                    dp.url = jQ(ems).data("url");
                }

                html = parsePropertiesToPattern(dp.isImage ? me.patternInfoImg : me.patternInfoFile, dp, dp.i);
                html = parsePropertiesToPattern(html, me.i18Labels);
            }
            (me.divDetails).innerHTML = html;
        },
        loadDataSearchClean: function () {
            if ((fileBrowser.qSearch).value !== "") {
                (fileBrowser.qSearch).value = "";
                (fileBrowser.qSearch).focus();
                fileBrowser.lastSearch = "";
                //Repaint the latest dir
                if (fileBrowser.history.breadcrumb !== "") {
                    fileBrowser.divBreadcrumb.innerHTML = fileBrowser.history.breadcrumb;
                    fileBrowser.list.innerHTML = fileBrowser.history.list;
                    fileBrowser.listLis = jQ(fileBrowser.list).children("li");
                    fileBrowser.lblMessage.style.display = "none";
                }
            }
        },
        loadDataSearch: function (force) {
            var q = (fileBrowser.qSearch).value,
            path = fileBrowser.ajaxUrl,
            firstPath = path + "?query=" + q;

            if (fileBrowser.lastSearch === q || !q) { return; }

            fileBrowser.lastSearch = q;
            (fileBrowser.list).innerHTML = "";
            fileBrowser.listLis = [];
            fileBrowser.loadData(null, q);
            fileBrowser.divBreadcrumb.innerHTML = "*";
        },
        loadData: function (newDir, searchText) {
            var me = fileBrowser,
                ajaxUrl = me.ajaxUrl;
                
            jQ(fileBrowser.list).addClass("loading");
            me.api.getData(ajaxUrl, newDir, searchText, window.selectableExtensions, me.repaintList);
        },
        repaintList: function (data) {
            var i, j, len, len2, dpt, dptImg, lis = [], lisHtml,
                lisImgs, pathSplitted, breadcrumbUl = [], breadcrumber,
                patternFile = "<li data-i=\"{{i}}\" class=\"file\" data-size=\"{{Size}}\" data-date=\"{{Date}}\" data-isimage=\"false\" data-url=\"{{Url}}\" " +
                    "data-name=\"{{Title}}\"><span class=\"file-ic glyphicon glyphicon-file\"></span> " +
                    "<label>{{Title}}</label></li>",
                patternImg = "<li data-i=\"{{i}}\" class=\"file image\" data-size=\"{{Size}}\" data-date=\"{{Date}}\" data-url=\"{{Url}}\" " +
                    "data-name=\"{{Title}}\" data-isimage=\"true\" data-scount=\"{{SCount}}\"  style=\"background-image:url('{{Thumb}}');\" > " +
                    "<label>{{Title}}</label> " +
                    "<div class=\"image-sizes\">{{ImageSizes}}</div></li>",
                patternImgSizes = "<em class=\"{{ClassName}}\" data-size=\"{{Size}}\" data-url=\"{{Url}}\">{{SizeName}}</em>",
                patternDir = "<li data-i=\"{{i}}\" data-url=\"{{Url}}\" class=\"dir\"><span class=\"file-ic glyphicon glyphicon-folder-open\"></span><label>{{Title}}</label></li>",
                startI = 0;

            fileBrowser.showInfo(-1);
            jQ(fileBrowser.list).removeClass("loading");

            if (data.Status && data.Status === "Error") {
                fileBrowser.lblMessage.innerHTML = data.Message;
                fileBrowser.lblMessage.style.display = "block";
                return;
            }
            fileBrowser.lblMessage.style.display = "none";

            //Breadcrumb
            if (data.Path) {
                fileBrowser.lastPath = data.Path;
                pathSplitted = data.Path.split("/");
                pathSplitted[0] = "[root]";
                breadcrumber = "/";
                for (i = 0, len = pathSplitted.length; i < len; i += 1) {
                    if (i > 0 && pathSplitted[i] === "") continue;
                    breadcrumber += i === 0 ? "" : pathSplitted[i] + "/";
                    breadcrumbUl.push("<li data-url=\"" + breadcrumber + "\">" + pathSplitted[i] + "</li>");
                }
                fileBrowser.divBreadcrumb.innerHTML = "<ul>" + breadcrumbUl.join("") + "</ul>";
            }

            //Dirs
            if (data.Dirs) {
                for (i = 0, len = data.Dirs.length; i < len; i += 1) {
                    lis.push(parsePropertiesToPattern(patternDir, data.Dirs[i], i));
                }
                startI = len > 0 ? len : 0;
            }

            //Files
            if (data.Files) {
                for (i = 0, len = data.Files.length; i < len; i += 1) {
                    dpt = data.Files[i];
                    dpt.ImageSizes = "";
                    if (dpt.IsImage && dpt.SCount > 0) {
                        lisImgs = [];
                        dptImg = { "SizeName": "default", "Size": dpt.Size, "Url": dpt.Url, "ClassName": (fileBrowser.preferredSize == "" ? "selected" : "") };
                        lisImgs.push(parsePropertiesToPattern(patternImgSizes, dptImg));
                        for (j = 0, len2 = dpt.SCount; j < len2; j += 1) {
                            dptImg = dpt.Children[j];
                            dptImg.ClassName = fileBrowser.preferredSize == dptImg.SizeName ? "selected" : "";
                            lisImgs.push(parsePropertiesToPattern(patternImgSizes, dptImg));
                        }
                        dpt.ImageSizes = lisImgs.join("");
                    }
                    lis.push(parsePropertiesToPattern(dpt.IsImage ? patternImg : patternFile, dpt, startI + i));
                }

                lisHtml = lis.join("");
                fileBrowser.list.innerHTML = lisHtml;
                fileBrowser.listLis = jQ(fileBrowser.list).children("li");
                fileBrowser.cur = -1;
            }

            if (data.Path) {
                fileBrowser.history = { breadcrumb: fileBrowser.divBreadcrumb.innerHTML, list: lisHtml };
            }

        },
        initUpload: function () {
            var ups = document.getElementsByClassName("file-upload-ajax"),
                j,
                len = ups.length, ctrl, ctrlBtn, tg, tgBtn;

            if (ups.length === 0) return;
            if (!window.FormData) {
                jQ(fileBrowser.divUploadSection).addClass("disallowed");
                return;
            }

            fileBrowser.upsBtns = ups;

            function uploadPrepare(e) {
                clearTimeout(fileBrowser.interv);
                fileBrowser.interv = setTimeout(function () { fileBrowser.upload(e); }, 100);
            }

            for (j = 0; j < len; j += 1) {
                jQ(ups[j]).change(uploadPrepare);

                ctrl = (ups[j]).getAttribute("data-valueid");
                ctrlBtn = document.getElementById(ctrl + "_Btn");
                if (ctrlBtn) {
                    jQ(ctrlBtn).click(function (e) {
                        tg = e.target.getAttribute("data-fire");
                        tgBtn = document.getElementById(tg);
                        jQ(tgBtn).trigger("click");
                    });
                }
            }
        },
        upload: function upload(e) {
            var t = e.target.id,
                files = e.target.files,
                datas,
                len,
                x,
                divImageHolder,
                inputValueId,
                ids = [],
                btnsSubmit,
                qSel = document.querySelectorAll === undefined,
                reqExists, arrNames = [], reqUpload, sNames, overwriteArr = [],
                ajaxUrl = fileBrowser.ajaxUrl,
                maxSizeMessage = "",
                maxSizeBytes = maxSize && maxSize > 0 ? maxSize * 1024 : 0,
                bytesCombined = 0, notUploadingAllFiles = false;

            function saveContext() {
                inputValueId = e.target.getAttribute("data-valueid");
                divImageHolder = document.getElementById("holder-" + inputValueId);
                divImageHolder.innerHTML = "";
                jQ(divImageHolder).addClass("loading");
                if (!qSel) {
                    btnsSubmit = document.querySelectorAll("button[type=submit]");
                    for (x = 0, len = btnsSubmit.length; x < len; x += 1) {
                        btnsSubmit[x].setAttribute("disabled", "disabled");
                    }
                }
            }

            function restoreContext() {
                if (!qSel) {
                    for (x = 0, len = btnsSubmit.length; x < len; x += 1) {
                        btnsSubmit[x].removeAttribute("disabled");
                    }
                }
            }

            function sendFilesToServer() {
                var kk = 0;

                datas = new FormData();
                for (x = 0, len = files.length; x < len; x += 1) {
                    if (files[x].ignore) continue;
                    datas.append("file" + kk, files[x]);
                    kk += 1;
                }
                datas.append("ticket", window.ticket);

                if (kk === 0) {
                    divImageHolder.innerHTML = "";
                    e.target.value = "";
                    jQ(divImageHolder).removeClass("loading");
                    return;
                }

                (fileBrowser.progressBar).parentNode.style.display = "none";
                (fileBrowser.progressBar).style.display.width = "0%";
                jQ(fileBrowser.progressBar).addClass("progress-bar-striped");

                reqUpload = jQ.ajax({
                    type: "POST",
                    url: ajaxUrl + '/uploadFile?selected=' + encodeURI(fileBrowser.lastPath) + '&overwrite=' + overwriteArr.join("|"),
                    contentType: false,
                    processData: false,
                    data: datas,
                });

                function handlerUpload(d) {
                    var per = d.loaded / d.total * 100;
                    (fileBrowser.progressBar).parentNode.style.display = "block";
                    (fileBrowser.progressBar).style.width = Math.round(per) + "%";
                    (fileBrowser.progressBar).innerHTML = Math.round(per) + "%";
                }

                reqUpload.progress(handlerUpload, null);

                reqUpload.done(function (result) {
                    var k, ll, img, lastPath = fileBrowser.lastPath;
                    if (result.Status == "Ok") {
                        divImageHolder.innerHTML = "";
                        jQ(divImageHolder).removeClass("loading");
                        if (document.getElementById(inputValueId)) {
                            document.getElementById(inputValueId).value = ids.join(",");
                        }
                        e.target.value = "";
                        fileBrowser.lastPath = "";
                        fileBrowser.loadData(lastPath, null);
                        jQ(fileBrowser.progressBar).removeClass("progress-bar-striped");
                        setTimeout(function () {
                            fileBrowser.showTabNum(0);
                        }, 1000);

                    } else {
                        fileBrowser.lblMessageUpload.innerHTML = result.Message;
                        fileBrowser.lblMessageUpload.style.display = "block";
                        (fileBrowser.progressBar).parentNode.style.display = "none";
                    }
                    restoreContext();
                });

                reqUpload.fail(ajaxError);
            }

            function chooseOverwriteOptions(er) {
                var tr = er.target, par, sibs;
                if (tr.tagName.toUpperCase() !== "BUTTON") { return; }

                par = tr.parentNode;
                sibs = par.getElementsByTagName("button");
                jQ(tr).siblings().removeClass("active");
                jQ(tr).addClass("active");
                par.setAttribute("data-action", tr.getAttribute("data-action"));
            }

            function continueUpload() {
                var btns = (fileBrowser.divLayoverCont).getElementsByClassName("btn-group-ov-opts"),
                    lenj, lenk,
                    j = 0, k, obj;

                if (btns === null) return;
                lenj = btns.length;
                overwriteArr = [];

                while (files !== null && files.length > 0 && j < lenj) {
                    obj = { a: (btns[j]).getAttribute("data-action"), n: (btns[j]).getAttribute("data-name"), o: (btns[j]).getAttribute("data-oname") };
                    switch (obj.a) {
                        case "keep":
                            break;
                        case "replace":
                            overwriteArr.push(obj.n);
                            break;
                        case "ignore":
                            for (k = 0, lenk = files.length; k < lenk; k += 1) {
                                if (files[k].name === obj.n) files[k].ignore = true;
                            }
                            break;
                    }
                    j += 1;
                }

                sendFilesToServer();
                (fileBrowser.divLayover).style.display = "none";
                (fileBrowser.divLayoverCont).style.display = "none";

            }

            if (files.length > 0) {
                fileBrowser.lblMessageUpload.style.display = "none";

                if (window.FormData !== undefined) {
                    datas = new FormData();
                    bytesCombined = 0;
                    notUploadingAllFiles = false;

                    for (x = 0, len = files.length; x < len; x += 1) {
                        if (maxSizeBytes > 0 && files[x].size > maxSizeBytes) {
                            maxSizeMessage += files[x].name + "\n";
                            files[x].ignore = true;
                        } else {
                            if ((bytesCombined + files[x].size) < maxSizeBytes) {
                                datas.append("file" + x, files[x]);
                                arrNames.push(encodeURI(files[x].name));
                                bytesCombined += files[x].size;
                            } else {
                                notUploadingAllFiles = true;
                                files[x].ignore = true;
                            }
                        }
                    }

                    if (maxSizeMessage) {
                        alert("These files are bigger than the maximum allowed size for this site. Upload smaller files (" +
                            (Math.round(maxSize / 1024 * 10) / 10) + " MBs) or ask your webmaster to increase the limit:\n\n" + maxSizeMessage);
                    }
                    if (notUploadingAllFiles) {
                        alert("The total bytes that can be uploaded each time cannot exceed " + (Math.round(maxSize / 1024 * 10) / 10) +
                            " MBs. Upload files separately or ask your webmaster to increase the limit");
                    }

                    saveContext();

                    reqExists = jQ.ajax({
                        type: "POST",
                        url: ajaxUrl + '/checkIfExists?selected=' + encodeURI(fileBrowser.lastPath),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({ "filenames": arrNames.join("|") })
                    });

                    reqExists.done(function (d) {
                        var ptn = "<li><div class=\"btn-group btn-group-sm btn-group-ov-opts\" role=\"group\" data-name=\"{{name}}\" data-oname=\"{{oname}}\">" +
                        "<button type=\"button\" class=\"btn btn-default active\" data-action=\"keep\">{{i18keep}}</button>" +
                        "<button type=\"button\" class=\"btn btn-default\" data-action=\"replace\">{{i18repl}}</button>" +
                        "<button type=\"button\" class=\"btn btn-default\" data-action=\"ignore\">{{i18ignr}}</button></div>" +
                        "<span class='name'>{{name}}</span></li>",
                        j, lenj, lis = [], btns, btnsHtml;

                        if (d.Status === "Checked") {
                            if (d.Files && d.Files.length > 0) { //Filenames exist on server
                                (fileBrowser.divLayover).style.display = "block";
                                (fileBrowser.divLayoverCont).style.display = "block";
                                for (j = 0, lenj = d.Files.length; j < lenj; j += 1) {
                                    sNames = String(d.Files[j]).split("|");
                                    btnsHtml = parsePropertiesToPattern(ptn, { name: decodeURI(sNames[0]), oname: decodeURI(sNames[1]) });
                                    btnsHtml = parsePropertiesToPattern(btnsHtml, fileBrowser.i18Labels);
                                    lis.push(btnsHtml);
                                }
                                (fileBrowser.divLayoverUl).innerHTML = parsePropertiesToPattern(lis.join(""));

                                btns = (fileBrowser.divLayoverCont).getElementsByClassName("btn-group-ov-opts");
                                for (j = 0, lenj = d.Files.length; j < lenj; j += 1) {
                                    jQ(btns[j]).click(chooseOverwriteOptions);
                                }
                                if (!fileBrowser.continueUploadInit) {
                                    jQ(fileBrowser.divLayoverContinueBtn).click(continueUpload);
                                    fileBrowser.continueUploadInit = true;
                                }
                            } else {
                                sendFilesToServer();
                            }
                        }
                    });

                    reqExists.fail(ajaxError);

                } else {
                    alert("This browser doesn't support HTML5 file uploads!");
                }
            } //files>0

        }, //upload
        deleteFile: function (e) {
            var id = e.target.getAttribute("data-id");
            e.stopPropagation();
            e.preventDefault();

            if (!confirm("Delete file?")) return;

            var reqDelete = jQ.ajax({
                type: "POST",
                url: '/uploadFile/deleteFile?id=' + id
            });

            reqDelete.done(function (d) {
                if (d.status !== "File deleted") {
                    alert(d.status);
                } else {
                    fileBrowser.loadDataSearch(true);
                    fileBrowser.showInfo(-1);
                }
            });

            reqDelete.fail(ajaxError);

        },
        updateAltText: function (e) {
            var id = e.target.getAttribute("data-id"),
                i = e.target.getAttribute("data-i"),
                altText = document.getElementById("alt-text").value,
                altTextHid = document.getElementById("alt-text-hid").value,
                reqSave;

            e.stopPropagation();
            e.preventDefault();

            if (altText === altTextHid) {
                return;
            }

            reqSave = jQ.ajax({
                type: "POST",
                url: '/uploadFile/updateAltText?id=' + id + "&altText=" + encodeURI(altText)
            });

            reqSave.done(function (d) {
                if (d.status !== "File updated") {
                    alert(d.status);
                } else {
                    ((fileBrowser.listLis)[i]).setAttribute("data-alt", altText);
                }
            });

            reqSave.fail(ajaxError);

        },

        //This is the public API
        api: {

            getData: function (ajaxUrl, newDir, searchText, selectableExtensions, callbackFunction) {

                var req,
                    ajaxRequest = "";

                if (!ajaxUrl) { return; }

                if (newDir) {
                    ajaxRequest = ajaxUrl + "?selected=" + encodeURI(newDir) + (selectableExtensions ? "&exts=" + encodeURI(selectableExtensions) : "");
                } else
                    if (searchText) {
                        ajaxRequest = ajaxUrl + "/search?query=" + encodeURI(searchText) + (selectableExtensions ? "&exts=" + encodeURI(selectableExtensions) : "");
                    } else {
                        return;
                    }

                req = jQ.ajax({
                    type: "GET",
                    url: ajaxRequest
                });

                req.done(function (data) {
                    callbackFunction.call(null, data);
                });

                req.fail(ajaxError);
            }//end of getData

        }//end of API
    };

    fileBrowser.init();

    return {
        api: fileBrowser.api
    }


}(jQuery));

//
//http://stackoverflow.com/questions/19126994/what-is-the-cleanest-way-to-get-the-progress-of-jquery-ajax-request
//
(function extendJQueryAjaxWithProgress(window, jQuery, undefined) {
    var $originalAjax = jQuery.ajax;

    jQuery.ajax = function (url, options) {
        if (typeof (url) === 'object') {
            options = url;
            url = undefined;
        }
        options = options || {};

        // Instantiate our own.
        var xmlHttpReq = jQuery.ajaxSettings.xhr();

        // Make it use our own.
        options.xhr = function () {
            return (xmlHttpReq);
        };

        var $newDeferred = jQuery.Deferred();
        var $oldPromise = $originalAjax(url, options)
            .done(function doneWrapper(response, textStatus, jqXhr) {
                return ($newDeferred.resolveWith(this, arguments));
            })
            .fail(function failWrapper(jqXhr, textStatus, error) {
                return ($newDeferred.rejectWith(this, arguments));
            })
            .progress(function progressWrapper() {
                window.console.warn("Whoa, jQuery started actually using deferred progress to report Ajax progress!");
                return ($newDeferred.notifyWith(this, arguments));
            });

        var $newPromise = $newDeferred.promise();

        // Extend our own.
        $newPromise.progress = function (handlerUp, handlerDown) {
            // Download progress
            if (handlerDown)
                xmlHttpReq.addEventListener('progress', function downloadProgress(evt) {
                    // window.console.debug( "download_progress", evt );
                    handlerDown.apply(this, [evt]);
                }, false);

            // Upload progress
            if (handlerUp)
                xmlHttpReq.upload.addEventListener('progress', function uploadProgress(evt) {
                    // window.console.debug( "upload_progress", evt );
                    handlerUp.apply(this, [evt]);
                }, false);

            return (this);
        };

        return ($newPromise);
    };
})(window, jQuery);
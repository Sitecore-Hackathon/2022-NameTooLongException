//NAMETOOLONGEXCEPTION--CUSTOM CODE :: Sitecore Send Identfy UserService
var sendIdentfyUserService = {};
(function () {
    $(document).ready(function () {
        var identifyUserOnFormSubmit = function (e) {
            const formData = new FormData(e.target);
            console.log(formData);
            if (formData["Email"]) {
                //Identify user in Mootrack
                mootrack('identify', formData["Email"]);
                //Identify user in Boxever
                $.cookie('mootrack_email_id', 1);
            }
        }
        for (var i = 0; i < document.forms.length; i++) {
            var form = document.forms[i];
            form.addEventListener("submit", identifyOnFormSubmit, false);
        }
    });
}).apply(sendIdentfyUserService);
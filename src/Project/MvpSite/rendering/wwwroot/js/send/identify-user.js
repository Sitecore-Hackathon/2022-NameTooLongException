//NAMETOOLONGEXCEPTION--CUSTOM CODE :: Sitecore Send Identfy UserService
$(document).ready(function () {
    $(document).on('click', 'form.moosend-subscription-form', function (e) {
        // code
        const data = Object.fromEntries(new FormData(e.currentTarget).entries());
        if (data["Email"]) {
            //Identify user in Mootrack
            mootrack('identify', data["Email"]);
            //STORING in Cookie just as temp demo purpose
            // 1. use can be to get identified user from moosend / Boxever or Okta !! 
            // 2. Based on idendified user, we can if user has submitted Sitecore Send form - using our custom rule !
            $.cookie('mootrack_email_id', data["Email"]);
        }
    });

});

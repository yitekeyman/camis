/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
function valLogin(){
 var valid = true;
    var requiredField = ['username', 'password'];
    for (var i = 0; i < requiredField.length; i++) {
        var arrlist = $('#' + requiredField[i]).val();
        if (arrlist == "" || arrlist == -1)
        {
             $("#login-error").remove();
            $("#login-message").append("<span id='login-error'><i class='fa fa-warning'></i> Required Username & Password</span>");
            valid = false;
        } else
        {
            $("#login-error").remove();
        }
    }

    if (valid)
    {
        $("#login-error").remove();
        $("#login-message").append("<span id='login-error'>You have successfuly loged in</span>");
    }
    }
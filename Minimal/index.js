var loginData = 'grant_type=password&username=User1&password=Password1&client_id=client1';

var xmlhttp = new XMLHttpRequest();
xmlhttp.onreadystatechange = function () {
    if (xmlhttp.readyState === 4 && xmlhttp.status === 200) {
        alert(xmlhttp.responseText);
    }
}
xmlhttp.open("POST", "/token", true);
xmlhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
xmlhttp.send(loginData);
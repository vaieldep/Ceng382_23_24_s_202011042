function show_hide(){
    var temp=document.getElementById("sakla");
    if(temp.style.display==="block"){
      temp.style.display="none"
    }
    else{
      temp.style.display="block"
    }
  }
  function reset(){
    document.getElementById("txtNum1").value = "0";
    document.getElementById("txtNum2").value = "0";
    document.getElementById("txtSum").value = "0";

    var temp=document.getElementById("saklan");
        if(temp.style.display==="block"){
          temp.style.display="none"
        }
        else{
          temp.style.display="block"
        }
}

function add(){

    var Num1 = parseInt (document.getElementById("txtNum1").value);
    var Num2 = parseInt (document.getElementById("txtNum2").value);

    var Sum = Num1+Num2; 
    document.getElementById("txtSum").value=Sum;


    
        var temp=document.getElementById("saklan");
        if(temp.style.display==="block"){
          temp.style.display="none"
        }
        else{
          temp.style.display="block"
        }
}

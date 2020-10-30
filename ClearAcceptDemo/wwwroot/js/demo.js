function logTestOutput(output) {
  var logEntry = "\r\n" + output;
  var textarea = document.getElementById("test-output");
  textarea.value += logEntry;
  textarea.scrollTop = textarea.scrollHeight;
}

function logTestJson(object) {
  logTestOutput(JSON.stringify(object,null,2));
}

document.getElementById("test-clear-output").addEventListener("click", function () {
  var textarea = document.getElementById("test-output");
  textarea.value = "";
});
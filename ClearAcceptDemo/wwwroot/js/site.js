function initHostedFields(fieldToken) {

  var options = {
    fields: {
      card: {
        containerElementId: "card-wrapper"
      },
      expdate: {
        containerElementId: "expdate-wrapper"
      },
      cvv: {
        containerElementId: "cvv-wrapper"
      }
    }
  };

  window.HostedFields.initialise(
    fieldToken,
    options,
    function (error, hostedFields) {
      if (error) {
        logTestOutput("Create Hosted Fields:");
        logTestOutput(JSON.stringify(error, null, 2));
        return;
      }

      hostedFields.on("change",
        function (formStatus) {
          var cardErrorContainer = document.getElementById("card-errors");
          var expdateErrorContainer = document.getElementById("expdate-errors");
          var cvvErrorContainer = document.getElementById("cvv-errors");

          setValidationStatus(formStatus, formStatus.card, cardErrorContainer);
          setValidationStatus(formStatus, formStatus.expDate, expdateErrorContainer);
          if (cvvErrorContainer) {//When channel is MO cvvErrorContainer will not be used
            setValidationStatus(formStatus, formStatus.cvv, cvvErrorContainer);
          }
        });


      function setValidationStatus(formStatus, fieldStatus, input) {
        if (fieldStatus.isValid || (fieldStatus.isPristine && formStatus.isNotSubmited)) {
          input.innerHTML = "";
        } else {
          input.innerHTML = fieldStatus.message;
        }
      }

      var payButton = document.getElementById("pay");
      payButton.addEventListener("click",
        function (event) {
          hostedFields.tokenize(function (token) {
            logTestOutput(JSON.stringify(token, null, 2));
            if (token.TokenId) {
              document.getElementById("temporaryToken").value = token.TokenId;
              document.getElementById("payment-form").submit();
            } else {
              logTestOutput("The value received does not contain a valid Token");
              event.preventDefault();
            }
          });
          event.preventDefault();
        });
    });

}

function logTestOutput(output) {
  var logEntry = "\r\n" + output;
  var textarea = document.getElementById("test-output");
  textarea.value += logEntry;
  textarea.scrollTop = textarea.scrollHeight;
}

document.getElementById("test-clear-output").addEventListener("click", function () {
  var textarea = document.getElementById("test-output");
  textarea.value = "";
});


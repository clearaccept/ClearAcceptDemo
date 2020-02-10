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
      var continueButton = document.getElementById("continue");
      var stepThrough = document.getElementById("step-through");
      var savedCard = document.getElementById("saved-card");
      var savedCardCvv = document.getElementById("saved-card-cvv");
      var savedCardCvvError = document.getElementById("saved-card-cvv-error");

      payButton.addEventListener("click",
        function(event) {
          event.preventDefault();
          if (savedCard && savedCard.selectedIndex > 0) {
            if (savedCardCvv && !savedCardCvv.value) {
              savedCardCvvError.innerHTML = "Cvv is required";
              return;
            }
            document.getElementById("temporaryToken").value = savedCard.options[savedCard.selectedIndex].value;
            document.getElementById("payment-form").submit();
            return;
          }
          const firstName = document.getElementById("PaymentRequest_CustomerInfo_FirstName").value;
          const lastName = document.getElementById("PaymentRequest_CustomerInfo_LastName").value;
          const clientDetails = {
            accountHolderName: firstName + " " + lastName
          }
          hostedFields.tokenize(clientDetails, function (token) {
            clearTestOutput();
            logTestOutput(".tokenize() result: \r\n \r\n" + JSON.stringify(token, null, 2));
            if (token.TokenId) {
              document.getElementById("temporaryToken").value = token.TokenId;
              payButton.hidden = true;
              if (!stepThrough.checked) {
                document.getElementById("payment-form").submit();
              } else {
                continueButton.hidden = false;
              }
            } else {
              logTestOutput("The value received does not contain a valid Token");
              event.preventDefault();
            }
          });
          event.preventDefault();
        });

      continueButton.addEventListener("click",
        function (event) {
          document.getElementById("payment-form").submit();
          continueButton.hidden = true;
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

function clearTestOutput() {
  var textareas = document.getElementsByName("test-outputs");
  textareas.forEach(textarea => {
    textarea.value = "";
  });
}

document.getElementById("test-clear-output").addEventListener("click", function () {
  clearTestOutput();
});


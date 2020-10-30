/** 
 * @desc the methods below are used by the payment page to interact with hosted fields.
 * the goal is to receive a TemporaryToken or a CvvToken from a HostedFields instance, depending on the context.
 * @required a fieldToken that was exchanged for a PaymentRequest by the backend
 * @required the payment channel that the PaymentRequest was created with
*/

// Method called as soon as the page loads
function initHostedFields(token, channel) {
  // Optional: customise hosted fields with your own style
  const customStyle = {
    logoAlignment: 'left',
    base: {
      backgroundColor: '#ffffff',
      borderColor: '#ced4da',
      borderWidth: '1px',
      borderStyle: 'solid',
      borderRadius: '4px',
      fontSize: '12px',
      fontColor: '#495057',
      fontFamily: 'Arial, sans-serif',
      padding: '6px 12px',
      textShadow: 'none',
      fontWeight: '400',
      transition: 'border-color .15s ease-in-out',
      placeHolder: {
        fontColor: '#6c757d',
      },
      cardLogo: {
        transition: 'opacity .5s cubic-bezier(.075, .82, .165, 1), transform .5s cubic-bezier(.075, .82, .165, 1)',
      },
      focus: {
        borderColor: '#80bdff'
      }
    },
    invalid: {
      cardLogo: {
        fill: '#cc2b19'
      },
    }
  };

  // Specify the ID(s) of the HTML element(s) that will hold the hosted field(s)
  const params = {
    channel: channel,
    fieldToken: token,
    options: {
      fields: {
        card: {
          containerElementId: 'card-wrapper'
        },
        expdate: {
          containerElementId: 'expdate-wrapper'
        },
        cvv: {
          containerElementId: 'cvv-wrapper'
        },
      },
      style: customStyle
    },
    cvvOnlyOptions: {
      cvvOnly: true,
      fields: {
        cvv: {
          containerElementId: 'cvv-only-wrapper'
        },
      },
      style: customStyle
    }
  };

  createFields(params);
  listenToPaymentMethodSectorChange();
}

const paymentModeSelector = document.getElementById("payment-selector"),
  paymentModeSelectorContainer = document.getElementById("payment-selector-container"),
  cvvOnlyCardSelector = document.getElementById("saved-card"),
  payButton = document.getElementById("pay");
enableDisablePayButton();

if (cvvOnlyCardSelector) {
  cvvOnlyCardSelector.addEventListener("change", function () {
    const cvvField = document.getElementById("cvv-holder");
    if (cvvField) {
      cvvField.style.visibility = "visible";
    }
    enableDisablePayButton();
  });
}

// Method to initialise the Card, ExpDate and CVV hosted fields
function createFields(params) {

  // Initialise hosted fields using a FieldToken, options and a callback function
  window.HostedFields.initialise(
    params.fieldToken,
    params.options,
    function (error, hostedFields) {
      if (error) {
        logTestOutput("Hosted Fields:");
        logTestOutput(JSON.stringify(error, null, 2));
        return;
      }
      const cardErrorContainer = document.getElementById("card-errors");
      const expdateErrorContainer = document.getElementById("expdate-errors");
      const cvvErrorContainer = document.getElementById("cvv-errors");

      // Listen to hosted fields status changes
      hostedFields.on("change", createValidationHandler(cardErrorContainer, expdateErrorContainer, cvvErrorContainer));

      window.hostedFields = hostedFields;
      setTimeout(showSaveToggleSwitch, 1500);
      hideShowControls(true);
    });

  const permanentToken = document.getElementById("saved-card");
  if (params && params.channel && params.channel.trim().toUpperCase() !== "MO" && permanentToken) {
    // Initialise the CVV-only hosted field if the customer has saved cards 
    // and the payment is not through the Mail-Order channel
    createCvvOnly(params);
    hideShowControls(true);
  }

  // Add a listener to the 'Pay' button
  payButton.addEventListener("click", function (event) {
    const customerDetails = {
      accountHolderName: ''
    };
    const accountHolder = document.getElementById("first-name").value +
      " " +
      document.getElementById("last-name").value;
    customerDetails.accountHolderName = accountHolder;

    // Do nothing if the customer chose to pay using a saved card but did not select one
    if (paymentModeSelector &&
      !paymentModeSelector.checked &&
      cvvOnlyCardSelector && cvvOnlyCardSelector.value.toLowerCase() === "select..") {
      event.preventDefault();
      return;
    }

    setLoader(true);
    if (paymentModeSelector && !paymentModeSelector.checked) {
      // Customer chose to pay using a saved card
      if (window.cvvOnlyHostedFields) {
        // Update the Payment Request with the chosen PermanentToken
        updatePaymentRequest(cvvOnlyCardSelector.value,
          document.getElementById("request-id").value,
          function () {
            // Set any additional data on hosted fields
            window.cvvOnlyHostedFields.setData(customerDetails);
            // Call the tokenize method to get a CvvToken
            window.cvvOnlyHostedFields.tokenize(handleTokenCallback);
          });
      } else {
        updatePaymentRequest(cvvOnlyCardSelector.value,
          document.getElementById("request-id").value, function () {
            document.getElementById("payment-form").submit();
            event.preventDefault();
          });
      }
    } else {
      // Customer chose to pay using a new card
      // Set any additional data on hosted fields
      window.hostedFields.setData(customerDetails);
      // Call the tokenize method to get a TemporaryToken
      window.hostedFields.tokenize(handleTokenCallback);
    }
    event.preventDefault();
  });
};

// Method to initialise the CVV-only hosted field
function createCvvOnly(params) {

  window.HostedFields.initialise(
    params.fieldToken,
    params.cvvOnlyOptions,
    function (error, hostedFields) {
      if (error) {
        logTestOutput("Hosted Fields:");
        logTestOutput(JSON.stringify(error, null, 2));
        return;
      }
      const cvvOnlyErrorContainer = document.getElementById("cvv-only-errors");
      hostedFields.on("change", createValidationHandler(null, null, cvvOnlyErrorContainer));

      window.cvvOnlyHostedFields = hostedFields;
    });
};

// Method to update the PaymentRequest with the selected saved card before calling the tokenise method
function updatePaymentRequest(permanentToken, requestId, callback) {
  document.getElementById("token").value = permanentToken;
  const xhttp = new XMLHttpRequest();
  xhttp.open(
    'POST',
    "home/update?permanentToken=" + permanentToken + "&requestId=" + requestId
  );
  xhttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
  xhttp.onreadystatechange = function () {
    if (xhttp.readyState === XMLHttpRequest.DONE) {
      const response = JSON.parse(xhttp.response);
      if (xhttp.status !== 200 && xhttp.status !== 201) {
        logTestOutput("Error: Unable to update payment request!");
      }
      if (response.toString().toLowerCase().indexOf("errors") !== -1) {
        logTestOutput(response);
      }
      if (callback) {
        callback();
      }
    }
  };
  xhttp.onerror = function (error) {
    logTestOutput(error.toString());
  };

  xhttp.send();
}

// Method to process status changes returned by hosted fields
function createValidationHandler(cardErrorContainer, expdateErrorContainer, cvvErrorContainer) {
  function setValidationStatus(formStatus, fieldStatus, input) {
    if (fieldStatus.isValid || (fieldStatus.isPristine && formStatus.isNotSubmited)) {
      input.innerHTML = "";
    } else {
      input.innerHTML = fieldStatus.message;
      setLoader(false);
    }
  }

  return function (formStatus) {
    if (cardErrorContainer) {
      setValidationStatus(formStatus, formStatus.card, cardErrorContainer);
    }
    if (expdateErrorContainer) {
      setValidationStatus(formStatus, formStatus.expDate, expdateErrorContainer);
    }
    if (cvvErrorContainer) {                //When channel is MO cvvErrorContainer will not be used
      setValidationStatus(formStatus, formStatus.cvv, cvvErrorContainer);
    }
  };
}

// Method to handle the tokenize callback and pass the TemporaryToken 
// or CvvToken to the backend to confirm the PaymentRequest with
function handleTokenCallback(token) {
  logTestOutput(JSON.stringify(token, null, 2));
  if (token.TokenId || token.CvvToken) {
    if (token.TokenId) {
      document.getElementById("token").value = token.TokenId;
    } else {
      document.getElementById("cvv-token").value = token.CvvToken;
    }
    document.getElementById("persist").value = document.getElementById("persist").checked;
    document.getElementById("payment-form").submit();
  } else {
    logTestOutput("The value received does not contain a valid Token");
    event.preventDefault();
  }
  setLoader(false);
}

/** 
  * @desc the methods below are used to control the UI and not necessarily required
*/

function hideShowControls(state) {
  const stateValue = state ? 'visible' : 'hidden';
  if (cvvOnlyCardSelector) {
    cvvOnlyCardSelector.style.visibility = stateValue;
  }
  if (paymentModeSelectorContainer) {
    paymentModeSelectorContainer.style.visibility = stateValue;
  }
}

function listenToPaymentMethodSectorChange() {
  const cvvOnly = document.getElementById("saved-cards");
  const newCard = document.getElementById("new-card");
  const saveCardToggleContainer = document.getElementById("save-card-container");
  if (paymentModeSelector) {
    paymentModeSelector.addEventListener("change", function () {
      if (paymentModeSelector.checked) {
        addRemoveClass(newCard, 'inactive', 'remove');
        addRemoveClass(saveCardToggleContainer, 'inactive', 'remove');
        addRemoveClass(cvvOnly, 'inactive', 'add');
      } else {
        addRemoveClass(cvvOnly, 'inactive', 'remove');
        addRemoveClass(newCard, 'inactive', 'add');
        addRemoveClass(saveCardToggleContainer, 'inactive', 'add');
      }
      enableDisablePayButton();
    });
  }
}

function enableDisablePayButton() {
  if (!payButton) {
    return;
  }
  if (!paymentModeSelector ||
    (paymentModeSelector && paymentModeSelector.checked) ||
    (cvvOnlyCardSelector && cvvOnlyCardSelector.value.toLowerCase() !== "select..")) {
    payButton.removeAttribute('disabled');
  } else {
    payButton.setAttribute('disabled', true);
  }
}

function addRemoveClass(elem, className, action) {
  if (elem) {
    let elemClasses = elem.getAttribute('class');
    const classArray = (elemClasses && elemClasses.split(' ')) || [],
      classPositionIndex = classArray.indexOf(className);
    if (action === 'remove') {
      classArray.splice(classPositionIndex, 1);
      elem.setAttribute('class', classArray.join(' '));
    } else if (action === 'add') {
      if (classPositionIndex === -1) {
        classArray.push(className);
      }
      elem.setAttribute('class', classArray.join(' '));
    }
  }
}

function setLoader(on) {
  //add loader to button
  const payNowLoader = document.getElementById("pay-loader");
  const payNowText = document.getElementById("pay-text");
  if (on) {
    addRemoveClass(payNowLoader, 'hidden', 'remove');
    addRemoveClass(payNowText, 'hidden', 'add');
  } else {
    addRemoveClass(payNowLoader, 'hidden', 'add');
    addRemoveClass(payNowText, 'hidden', 'remove');
  }
}

function showSaveToggleSwitch() {
  const saveToggle = document.getElementById('save-toggle');
  addRemoveClass(saveToggle, 'hidden', 'remove');
}

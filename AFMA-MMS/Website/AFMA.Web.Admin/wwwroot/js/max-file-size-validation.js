  $(document).ready(function () {
    // 1. Define the rule logic (compares actual browser file size against the limit attribute)
    $.validator.addMethod("maxfilesize", function (value, element, param) {
      if (element.files && element.files.length > 0) {
        const maxBytes = parseInt(param);
        const actualBytes = element.files[0].size;
        return actualBytes <= maxBytes;
      }
      return true; // Pass if no file is selected (let [Required] handle it)
    });

    // 2. Register the adapter to map the C# attribute data to jQuery validator
    $.validator.unobtrusive.adapters.add("maxfilesize", ["size"], function (options) {
      options.rules["maxfilesize"] = options.params.size;
      options.messages["maxfilesize"] = options.message;
    });
  });
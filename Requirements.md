# Flatirons Full-Stack Developer Coding Test

## Requirements:

### Global:

- [x] Analyse the requirements
- [x] Create a full-stack web or mobile application
- [x] Frontend (Flutter)
- [x] Backend (NestJS)

---

### Backend:

- [x] Accept upload of CSV files
- [x] Process and store into a database the uploaded file (products)
- [x] When storing a product, get and store multiple exchange rates (at least 5 currencies) from [API](https://github.com/fawazahmed0/exchange-api)
  - All product fields are required and must be present (name;price;expiration)
- [x] Implement an endpoint that returns all the processed rows of product data along with the its currency conversions
- [x] This endpoint should support filtering based on the name, price, and expiration fields
- [x] This endpoint should support sorting based on the name, price, and expiration fields
- [x] The application should support CSV files with up to 200k rows, but easily scale to support more.

---

### Frontend:

- [x] Upload CSV files (file upload input that allows the user to select a CSV file from their device)
- [x] While the file is uploading and being processed, there should be a loading indicator displaying progress of the upload.
- [x] Once the file uploads, a success message should display
- [x] And you should be able to browse a table of the uploaded products

---

### Not in scope (not in the initial requirements):

- Authentication/authorization, security, etc.
- Advanced log and monitoring in the backend
- Change (edit/include/delete) products already uploaded
- Log of invalid lines in the CSV
- API Rate Limiting
- Filtering/sorting records in the frontend
- Store the original CSV for testing/comparison/etc.
- App themes (light/dark mode)
- UX/UI (icons and other visual refinements)
- Cancel an ongoing operation (upload/process)
- MIME type enforcement in the backend (validating only the file extension for now)
- Other business ruiles validation (like "prices should be positive")
- i18n (internationalization/translations)
- Advanced upload techniques (fail recover, etc.)
- Unit/integration tests
- Export data from database to CSV
- Any other requirements not explicitly defined in the "Requirements" section.

---

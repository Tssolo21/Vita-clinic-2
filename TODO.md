# TODO: Replace ItemsControl with DataGrid in Views

## Tasks
- [ ] Update Views/ClientsView.axaml: Replace ItemsControl with DataGrid, define columns for Id, FirstName, LastName, Email, Phone, Status.
- [ ] Update Views/ClientsView.axaml.cs: Add InitializeDataGrid method, update LoadClientsAsync to refresh DataGrid, change FindControl to DataGrid.
- [ ] Update Views/AnimalsView.axaml: Replace ItemsControl with DataGrid, define columns for Id, Name, Species, Breed, DateOfBirth, Weight, Client.FirstName.
- [ ] Update Views/AnimalsView.axaml.cs: Add InitializeDataGrid, update LoadAnimalsAsync, change to DataGrid.
- [ ] Update Views/AppointmentsView.axaml: Replace ItemsControl with DataGrid, define columns for Id, PetName, OwnerName, AppointmentType, AppointmentDateTime, VeterinarianName, Status.
- [ ] Update Views/AppointmentsView.axaml.cs: Add InitializeDataGrid, update LoadAppointmentsAsync, change to DataGrid.
- [ ] Create Views/MedicalRecordsView.axaml: New file with DataGrid, columns for Id, Animal.Name, Diagnosis, Treatment, Medication, NextCheckupDate, RecordDate.
- [ ] Create Views/MedicalRecordsView.axaml.cs: New file with logic similar to other views, including InitializeDataGrid, LoadMedicalRecordsAsync, AddMedicalRecord, etc.
- [ ] Ensure navigation to MedicalRecordsView is added in MainWindow.axaml if needed.
- [ ] Test each view to verify DataGrid displays data correctly and refreshes after add/edit.
- [ ] Check for any compilation errors and fix.

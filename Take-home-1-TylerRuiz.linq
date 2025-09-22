<Query Kind="Statements">
  <Connection>
    <ID>acf520cf-b031-495a-81cd-54e461552bae</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>TYLERS-PC</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>StartTed-2025-Sept</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

//1.

ClubActivities
   .Where(a => a.StartDate >= new DateTime(2025, 1, 1))
   .Where(a => !a.Name.Equals("Btech Club Meeting"))
   .Where(a => !a.CampusVenue.Location.Equals("scheduled room"))
   .OrderBy(a => a.StartDate)
   .Select(a => new
   {
	   StartDate = a.StartDate,
	   Location = a.CampusVenue.Location,
	   Club = a.Club.ClubName,
	   Activity = a.Name
   })
   .Dump();
   
//2.

Programs
	.Select(p => new
	{
		  School = p.SchoolCode == "SAMIT" ? "School of Advanced Media and IT" :
				 p.SchoolCode == "SEET" ? "School of Electrical Engineering Technology" :
				 "Unknown",
		ProgramName = p.ProgramName,
		RequiredCourseCount = p.ProgramCourses
			.Count(pc => pc.Required),	
		OptionalCourseCount = p.ProgramCourses
			.Count(pc => !pc.Required)
	})
	.Where(x => x.RequiredCourseCount >= 22)
	.OrderBy(x => x.ProgramName)
	.Dump();

//3.

Students
	.Where(s => !StudentPayments.Any(sp => sp.StudentNumber == s.StudentNumber)
		&& !s.Countries.CountryName
			  .Equals("Canada"))
	.OrderBy(s => s.LastName)
	.Select(s => new
	{
		StudentNumber = s.StudentNumber,
		CountryName = s.Countries.CountryName,
		FullName = $"{s.FirstName} {s.LastName}",
		ClubMembershipCount =
			ClubMembers.Count(cm => cm.StudentNumber == s.StudentNumber) == 0
				? "None"
				: ClubMembers
					.Count(cm => cm.StudentNumber == s.StudentNumber)
					.ToString()
	})
    .Dump();

//4.

Employees
	.Where(e =>
		e.Position.Description
		 .Equals("Instructor")
		&& e.ReleaseDate == null
		&& ClassOfferings.Any(co => co.EmployeeID == e.EmployeeID)
	)
	.OrderByDescending(e =>
		ClassOfferings.Count(co => co.EmployeeID == e.EmployeeID)
	)
	.ThenBy(e => e.LastName)
	.Select(e => new
	{
		ProgramName = e.Program.ProgramName,
		FullName = $"{e.FirstName} {e.LastName}",
		Workload = ClassOfferings.Count(co => co.EmployeeID == e.EmployeeID) > 24
			? "High"
			: ClassOfferings.Count(co => co.EmployeeID == e.EmployeeID) > 8
				? "med"
				: "Low"
	})
	.Dump();
//5.

Clubs
	.Select(Clubs => new
	{
		Supervisor = Clubs.Employee == null
			? "Unknown"
			: $"{Clubs.Employee.FirstName} {Clubs.Employee.LastName}",
		Club = Clubs.ClubName,
		MemberCount = Clubs.ClubMembers.Count(),
		Activities = Clubs.ClubActivities.Any()
			? Clubs.ClubActivities.Count().ToString()
			: "None Scheduled"
	})
	.OrderByDescending(x => x.MemberCount)
	.Dump();


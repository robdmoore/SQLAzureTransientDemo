INSERT INTO Movie (Title, Year)
	SELECT m1.Title + ' ' + m2.Title as Title, m1.Year + m2.Year as Year
	FROM Movie m1, Movie m2
	WHERE m1.ID != m2.Id;

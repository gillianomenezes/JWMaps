IF OBJECT_ID ( 'GetLastVisitedPeople', 'P' ) IS NOT NULL   
    DROP PROCEDURE GetLastVisitedPeople;  
GO  
CREATE PROCEDURE GetLastVisitedPeople 
@category INT,
@congregationId INT,
@neighbourhood VARCHAR(200)
AS
BEGIN
	SELECT Householders.Id
	FROM Householders
	LEFT JOIN Visits ON Visits.Householder_Id = Householders.Id
	WHERE Householders.Neighbourhood = @neighbourhood AND Householders.CongregationId = @congregationId AND Householders.Category = @category AND Householders.TerritoryMap_Id IS NULL
	GROUP BY Householders.Id
	ORDER BY MAX(Visits.DateOfVisit)
END
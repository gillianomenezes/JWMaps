IF OBJECT_ID ( 'GetNeighbourhoodsByLastVisited', 'P' ) IS NOT NULL   
    DROP PROCEDURE GetNeighbourhoodsByLastVisited;  
GO  
CREATE PROCEDURE GetNeighbourhoodsByLastVisited 
@category INT,
@congregationId INT
AS
BEGIN
	SELECT Householders.Neighbourhood, CASE WHEN MAX(COALESCE(Visits.DateOfVisit, '12/31/2099')) = '12/31/2099' THEN NULL ELSE MAX(Visits.DateOfVisit) END  AS lastVisit FROM Householders
	LEFT JOIN Visits ON Householders.Id = Visits.Householder_Id	
	WHERE Householders.Category = @category AND Householders.CongregationId = @congregationId AND Householders.TerritoryMap_Id IS NULL
	GROUP BY Householders.Neighbourhood
	ORDER BY lastVisit 
END
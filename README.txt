README

COMP4710 Project

This solution contains the implementations of the CLARA and DBSCAN algorithms using 
Microsoft Visual Studio, written in C#. 

CLARA:
run...
-Open the 'COMP4710.sln' file to open the solution that contains both CLARA and DBSCAN
-Select CLARA and click Start (in Debug)
-This runs the algorithm: loops 5 times, creating samples and calculating the one with the
 lowest average dissimilarity, and chooses that one to output.
-Used 5 samples of 100 data points, and 5 medoids 
input...
-The input is a file called 'flights.dat' located in the ./bin/Debug folder and contains 1000 entries
output...
-The output is a file called 'bestSample' that prints the original data but also prints
 the id of the medoid that the data point belongs to.
-This output is shown in the 'CLARA 1000pt plot.xlsx' file
-The plot was made in Microsoft Excel based on this data


DBSCAN
run...
-Open the 'COMP4710.sln' file to open the solution that contains both CLARA and DBSCAN
-Select DBSCAN and click Start (in Debug)
-This runs the DBSCAN algorithm on the input file.
input...
-The input is a file called 'flights.dat' located in the ./bin/Debug folder and contains 1000 entries
-Parameters used for this algorithm were: epsilon = 400, minPoints = 10
output...
-The output is a file called 'densityData.dat' that prints the original data but also prints
 the cluster that the data point belongs to.
-This output is shown in the 'DBSCAN 400eps_10minPts.xlsx' file
-The plot was made in Microsoft Excel based on this data
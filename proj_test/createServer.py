import pyodbc 

server = '(localdb)\mssqllocaldb'
database = 'scrapingTest'
username = 'SIMONCA_RAUL\simon'
password = 'your_password'

# Establishing a connection to the SQL Server
try:
    cnxn = pyodbc.connect(
                      'SERVER='+server+';\
                      DATABASE='+database+';\
                      UID='+username)
    print("Succes")
except ConnectionRefusedError as err:
    print(err)

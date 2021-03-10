import os
import numpy as np
import pandas as pd

import requests
import json

# URL for the web service
scoring_uri = 'http://32f74919-50e0-46cb-9c71-xxxxxxxxx.southeastasia.azurecontainer.io/score'
# If the service is authenticated, set the key or token
key = 'xxxxxxxxxxxxxxxxxxxxxxxxxxx'

# load data from csv file
data_csv = pd.read_csv('./data/data.csv')

data_list = data_csv.values.tolist()
data = {'data': data_list}
# data = {'data': [[2,4,15,3,10],[2,4,15,3,10]]}


# Convert to JSON string
input_data = json.dumps(data)

# Set the content type
headers = {'Content-Type': 'application/json'}
# If authentication is enabled, set the authorization header
headers['Authorization'] = f'Bearer {key}'

# Make the request and display the response
response = requests.post(scoring_uri, input_data, headers=headers)

if response.status_code == 200:
    result = response.text
else:
    print(response.json()) 
    print( "Error code: %d" % ( response.status_code ) ); 
    print( "Message: %s" % ( response.json()['message'] ) ); 
    os.exit()

print(result)

# save the results into csv file
col_name = ['predict']
output = pd.DataFrame(columns=col_name, data=eval(result))
output.to_csv('./data/output.csv', index=False)

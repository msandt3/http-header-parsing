import urllib2

# Set the URL to test here
url = 'http://api.linkedin.com/v1/'


for i in range (1, 51):
	# Build POST request
	request = urllib2.Request(url=url, data="someRandomPOSTData")
	
	statusCode = 0
	
	try:
		response = urllib2.urlopen(request)
		statusCode = response.getcode()
	except urllib2.HTTPError, err:
		statusCode = err.code 
	
	# Python 2 syntax
	print "Try #{0}: {1}".format(i, statusCode)
	
	# Python 3 syntax
	# print("Try #{0}: {1}".format(i, statusCode))

print "The end"
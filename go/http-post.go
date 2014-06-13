package main

import (
    "fmt"
    "net/http"
    "strings"
    "io/ioutil"
)

func main() {

	url := "http://api.linkedin.com/v1"
	payload := `
{
   "some": "random",
   "POST": "data"
}
	`

	for i := 1; i <= 50; i++ {
	    fmt.Printf("Try #%v: ", i)

		client := &http.Client{}
		req, _ := http.NewRequest("POST", url, strings.NewReader(payload))
		req.Header.Add("Content-Type", "application/json")
		resp, err := client.Do(req)

		if err != nil {
		  	fmt.Println(err)
		}

		fmt.Printf("%v \n", resp.Status)

		defer resp.Body.Close()
		_, err2 := ioutil.ReadAll(resp.Body)

		if err2 != nil {
		  	fmt.Println(err2)
		}
	}
}


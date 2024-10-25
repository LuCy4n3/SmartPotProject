from bs4 import BeautifulSoup
from httpx import Client

header = 'cf_chl_3=a3e2e7af4c7fd05; cusess=37bb1dfb5156f8e13aa2465093dfb55b; sr=%2Fplants%2Fview%2F530731%2FBeets-Beta%2F; _pk_id.1.94cf=691054ba98ad415f.1709337196.; _pk_ses.1.94cf=1; cf_clearance=MijJ64Tse15R4jxMkNb0oRN.U41NPeOvTCZ_cnoluNA-1709337196-1.0.1.1-U8PY5gKaN7FWPf0yTBmpGM1PiKGaVAv89G9BD_acNCQ5ccCLZexGFa0mCwhicGDGIjtF5_oxast.7_3K7XTxTQ; usprivacy=1NNN; euconsent-v2=CP6ywkAP6ywkAAKA0AENDgCgAAAAAEPAAAwIAABBqALMNC4gC7IkJCbQMIoEAIgrCAigQAAAAkDRAQAuDAp2BgEusJEAIEUABwQAhABRkACAAASABCIAJAigQAAQCAQAAgAQCAQAMDAAOAC0EAgABAdAxTCgAUCwgSIyIhTAhCgSCAlsoEEoKhBXCAIsMCKARGwUACAJARWAAICxeAwBICViQQJdQbQAAEACAUUoVCKT8wBDgmbLVXiibQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAIAA.YAAAAAAAAAAA; addtl_consent=1~'
additConsent = '1~'
userAgent = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36'
URL = "https://garden.org/plants/view/109680/Potato-Solanum-tuberosum-Yukon-Gold/"

headers = {'Cookie': header,
           'User-Agent': userAgent}
with Client(headers=headers) as client:
    response = client.get(URL)
    soup = BeautifulSoup(response.text, "html.parser")
    if soup.get_text() != "":
        print(soup)
nextElement = 0
for caption in soup.find_all('caption'):
    if caption.get_text() == 'General Plant Information (Edit)':
        print("found it")
        table = caption.find_parent('table', {
            'class': 'table table-striped table-bordered table-hover caption-top simple-table'})
        for element in table.find_all('td'):
            if nextElement == 1:
                print(element.get_text())
            if element.get_text() == "Life cycle:":
                nextElement = 1
                print(element.get_text())
            elif element.get_text() != "Life cycle:":
                nextElement = 0
           


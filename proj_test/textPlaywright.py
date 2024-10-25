from playwright.sync_api import sync_playwright
from playwright.sync_api import sync_playwright

# Launch the browser
with sync_playwright() as p:
    browser = p.chromium.launch()

    # Create a new browser context
    context = browser.new_context()

    # Open a new page within the context
    page = context.new_page()

    # Define custom headers
    header = 'cf_chl_3=a3e2e7af4c7fd05; cusess=37bb1dfb5156f8e13aa2465093dfb55b; sr=%2Fplants%2Fview%2F530731%2FBeets-Beta%2F; _pk_id.1.94cf=691054ba98ad415f.1709337196.; _pk_ses.1.94cf=1; cf_clearance=MijJ64Tse15R4jxMkNb0oRN.U41NPeOvTCZ_cnoluNA-1709337196-1.0.1.1-U8PY5gKaN7FWPf0yTBmpGM1PiKGaVAv89G9BD_acNCQ5ccCLZexGFa0mCwhicGDGIjtF5_oxast.7_3K7XTxTQ; usprivacy=1NNN; euconsent-v2=CP6ywkAP6ywkAAKA0AENDgCgAAAAAEPAAAwIAABBqALMNC4gC7IkJCbQMIoEAIgrCAigQAAAAkDRAQAuDAp2BgEusJEAIEUABwQAhABRkACAAASABCIAJAigQAAQCAQAAgAQCAQAMDAAOAC0EAgABAdAxTCgAUCwgSIyIhTAhCgSCAlsoEEoKhBXCAIsMCKARGwUACAJARWAAICxeAwBICViQQJdQbQAAEACAUUoVCKT8wBDgmbLVXiibQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAIAA.YAAAAAAAAAAA; addtl_consent=1~'
    userAgent = 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36'
    headers1 = {'Cookie': header,
           'User-Agent': userAgent}
    # Navigate to a URL and set custom headers for the request
    URL = "https://garden.org/plants/view/109680/Potato-Solanum-tuberosum-Yukon-Gold/"

    page.goto(URL)
    page.set_extra_http_headers(headers=headers1)

    # Perform actions on the page
    # Grab the element’s inner text

    element_text = page.inner_html()
# Grab multiple elements


    print(element_text)
    # Close the browser
#browser.close()

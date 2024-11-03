import asyncio
from bs4 import BeautifulSoup
from playwright.async_api import async_playwright, TimeoutError as PlaywrightTimeoutError
import time
import random
import openpyxl
import os

pathExcl = "c:/Users/simon/OneDrive/Desktop/proj_test/linksForPlants.xlsx"

# used / instead of \ bcs im trying to use a normal string as a path
workbook = openpyxl.load_workbook(pathExcl)
allLinksWorkbook = openpyxl.Workbook()

plants = allLinksWorkbook.create_chartsheet("plants",1)

async def new_page(link, browser, retry_attempts=3):
    context = await browser.new_context()

    await context.add_cookies([
        {
            'name': 'cusess',
            'value': '98291da5e63b0067f2bf7f9b80b69529',
            'domain': 'garden.org',
            'path': '/'
        },
        {
            'name': 'sr',
            'value': '%2F',
            'domain': 'garden.org',
            'path': '/'
        },
        {
            'name': '_pk_id.1.94cf',
            'value': '3f0c64ea55e7161b.1729536175.',
            'domain': 'garden.org',
            'path': '/'
        },
        {
            'name': 'cf_clearance',
            'value': 'Xyxrc0sYB8J40hqfhpcFw2jk6wYv3qk8rBZZLOxBTn0-1729538939-1.2.1.1-jTU_oct3Tt.75ZDClaOgm5v1_IIje8MZcLzm9eey.c1UTsNKvtrseaNMy5bQaQdNXkWvwflP7je_zQqJCe3DvjU2LEEQC9s6JxCspIkGhauc3D41GVJo3kFqGCOsgJfpSu57MpQUpaRdhHVbFU3IGIdseD0xEGdC.jGcU8CZyXxseKQn2dnH8Imf7.OiYWSPj9j3LR3697NLK0hyectEy8LLbdBMrwKPXM7k..0p6zz6kpnv3EuCZTUKwich9pIxeRXNpB4OMgCh9.6GStZZJAhVgTmkotps26qo9wqvKzwaCKa81D848PQZLuZI0Nc.yWQwzB6J_qTfHRD9z1UFG6.jB_FZ4LgCNawxcLFzWizdPp_stgObhrXGTYbhaRAO',
            'domain': 'garden.org',
            'path': '/'
        }
    ])

    page = await context.new_page()

    headers = {
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36',
        'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7',
        'Accept-Encoding': 'gzip, deflate, br',
        'Accept-Language': 'en,en-US;q=0.9,ro;q=0.8,de;q=0.7',
        'Connection': 'keep-alive',
        'Referer': 'https://medium.com/zenrows/web-scraping-without-getting-blocked-cbafa55d8045',
        'Sec-Ch-Ua': '"Google Chrome";v="129", "Not=A?Brand";v="8", "Chromium";v="129"',
        'Sec-Ch-Ua-Mobile': '?0',
        'Sec-Ch-Ua-Platform': '"Windows"',
        'Sec-Fetch-Dest': 'document',
        'Sec-Fetch-Mode': 'navigate',
        'Sec-Fetch-Site': 'cross-site',
        'Sec-Fetch-User': '?1',
        'Upgrade-Insecure-Requests': '1'
    }

    await page.set_extra_http_headers(headers)

    attempt = 0
    while attempt < retry_attempts:
        try:
            await asyncio.wait_for(page.goto("https://garden.org" + link), timeout=5)
            content = await page.content()
            await page.close()
            await context.close()
            return content
        except asyncio.TimeoutError:
            attempt += 1
            retry_delay = random.uniform(1, 3)
            print(f"Timeout reached. Retrying in {retry_delay:.2f} seconds...")
            await asyncio.sleep(retry_delay)
        except Exception as e:
            print(f"Exception occurred: {e}. Retrying...")
            attempt += 1
            await page.close()
            page = await context.new_page()  # reopen page for new attempt

    await page.close()
    await context.close()
    print(f"Failed to fetch {link} after {retry_attempts} attempts.")
    return None

async def fetch_links(content, link):
    if content is None:
        print(f"Skipping link due to failed fetch: {link['href']}")
        return

    print("Processing content for link:", "https://garden.org" + link['href'])
    soup = BeautifulSoup(content, "html.parser")
    holders = soup.find("table", {"class": "table table-striped table-bordered table-hover caption-top pretty-table"})
    if holders:
        divLink = holders.find_next('div').find("div", {"class": "card-body"})
        if divLink:
            inner_link = divLink.find('a')
            if inner_link:
                print(inner_link['href'])
                currentSheet = workbook.get_sheet_by_name('completed')
                #compl.write("https://garden.org" +inner_link['href']+"\n")
    else:
        print(f"No '_search_and_browse' section found for link: {link['href']}")


async def main():
    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        
        path = "c:/Users/simon/OneDrive/Desktop/proj_test/plants"

        path = os.path.realpath(path)
        print("Folder path:", path)
        try:
            contents = os.listdir(path)
            print("Contents of the folder:")
            for item in contents:
                print(item)
        except FileNotFoundError:
            print("The folder path does not exist.")
            exit(-1)
        except PermissionError:
            print("Permission denied to access the folder.")
            exit(-1)
        
        
        sheet = workbook.get_sheet_by_name('completed')
        if(not sheet):
            print('error in excel')
            exit(-1)
        else :
            print("got data")
        print("trying to read the file..")
        #print(sheet.max_column)
        for row in  sheet.iter_rows(0,sheet.max_row):
            #print(row[0].value+" "+row[1].value)
            plantGroup = row[0].value
            plantGroupLink = row[1].value
            new_folder_path = "c:/Users/simon/OneDrive/Desktop/proj_test/plants/"+plantGroup
            try:
                # Create the folder
                os.mkdir(new_folder_path)
                print("Folder created:", new_folder_path)
            except FileExistsError:
                print("Folder already exists. Recreating:",new_folder_path)
                os.rmdir(new_folder_path)
                os.mkdir(new_folder_path)
            except FileNotFoundError:
                print("The parent directory does not exist.")
            except PermissionError:
                print("Permission denied to create the folder.")

        #new_page('test',browser)
        
asyncio.run(main())


        

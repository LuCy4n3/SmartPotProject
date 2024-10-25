import asyncio
from bs4 import BeautifulSoup
from playwright.async_api import async_playwright
import subprocess
import time

async def new_page(link,browser):
    context = await browser.new_context()

    # Set the cookies from your Postman request
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

    # Create a new page
    page = await context.new_page()

    # Set the headers to emulate Postman
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

    # Set extra HTTP headers
    await page.set_extra_http_headers(headers)

    # Navigate to the target URL
    await page.goto("https://garden.org"+ link)

    # Print the response content
    content = await page.content()
    #print(content)
    context.close()

    return content

async def fetch_links(content, link):

    print("https://garden.org" + link)
    
    # Process the content with BeautifulSoup
    soup = BeautifulSoup(content, "html.parser")
    if soup.get_text() != "":
        holders = soup.find("div", {"id": "_search_and_browse"}).find_next('div')
        if holders:
            divLink = holders.find("div", {"class": "card-body"})
            if divLink:
                link = divLink.find('a')
                if link:
                    print(link['href'])


async def main():
    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        context = await browser.new_context()

        # Set the cookies from your Postman request
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

        # Create a new page
        page = await context.new_page()

        # Set the headers to emulate Postman
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

        # Set extra HTTP headers
        await page.set_extra_http_headers(headers)

        # Navigate to the target URL
        await page.goto("https://garden.org/plants/group/show_list.php")

        # Print the response content
        content = await page.content()
        #print(content)

        soup = BeautifulSoup(content, "html.parser")
        context.close()
        if soup.get_text() != "":
          #print(soup)
          print("SEX cu mama lui dragos")
          for caption in soup.find_all('table'):
            for links in caption.find_all('a'):
                content = await new_page(links['href'],browser)
                await fetch_links(content,links['href'])
                print(links.get_text() + " http://garden.org" + links['href'])
        # Close the browser
        await browser.close()

# Run the script
asyncio.run(main())

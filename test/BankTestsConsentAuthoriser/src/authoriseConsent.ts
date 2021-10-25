import puppeteer = require('puppeteer');
import * as NatWest from './Sandbox/natwest'
import * as Modelo from './Sandbox/modelo'
import * as Lloyds from './Sandbox/lloyds'
import * as VrpHackathon from './Sandbox/vrpHackathon'
import {assert} from './utility'

// Consent variety enum and utility class
export enum ConsentVariety {
    AccountAccessConsent = "AccountAccessConsent",
    DomesticPaymentConsent = "DomesticPaymentConsent",
}

export enum Bank {
    NatWest = "NatWest",
    Modelo = "Modelo",
    Lloyds = "Lloyds",
    VrpHackathon = "VrpHackathon"
}

// Bank user
interface BankUser {
    readonly userNameOrNumber: string;
    readonly password: string;
}

export type ConsentUIInteractions = (
    page: puppeteer.Page,
    navigationPromise: Promise<puppeteer.HTTPResponse | null>,
    consentVariety: ConsentVariety,
    bankUser: BankUser,
) => Promise<void>

// Bank interactions lookup
const bankConsentUIInteractions: { [index in Bank]: ConsentUIInteractions } = {
    NatWest: NatWest.consentUIInteractions,
    Modelo: Modelo.consentUIInteractions,
    Lloyds: Lloyds.consentUIInteractions,
    VrpHackathon: VrpHackathon.consentUIInteractions
}

export async function authoriseConsent(
    authURL: string,
    bank: Bank,
    consentVariety: ConsentVariety,
    bankUser: BankUser,
    puppeteerLaunchOptions: puppeteer.LaunchOptions & puppeteer.BrowserLaunchArgumentOptions & puppeteer.BrowserConnectOptions
) {

    const browser = await puppeteer.launch(puppeteerLaunchOptions)
    const page = await browser.newPage()
    await page.setDefaultTimeout(0) // no timeouts apart from those configured in appsettings.json to aid debugging
    await page.goto(authURL)
    const navigationPromise = page.waitForNavigation()
    const navigationPromiseNetworkIdle = page.waitForNavigation({'waitUntil': 'networkidle0'})
    const consentUIInteractions = bankConsentUIInteractions[bank]
    await consentUIInteractions(page, navigationPromise, consentVariety, bankUser)
    // Wait for redirect web page
    await navigationPromiseNetworkIdle
    await page.waitForSelector('auth-fragment-redirect', {hidden: true})
    const pageStatusJSHandle = await page.waitForFunction('window.pageStatus')
    const pageStatus = await pageStatusJSHandle.jsonValue()
    assert(pageStatus === 'POST of fragment succeeded', 'Redirect page could not capture and pass on parameters in URL fragment')
    await browser.close()
}

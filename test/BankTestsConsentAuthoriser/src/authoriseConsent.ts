import puppeteer = require('puppeteer')
import * as NatWest from './NatWest/natWest'
import * as Modelo from './Modelo/modelo'
import { assert, assertIsDefined } from './utility'

// Consent variety enum and utility class
export enum ConsentVariety {
  AccountAccessConsent = "AccountAccessConsent",
  DomesticPaymentConsent = "DomesticPaymentConsent"
}

export enum Bank {
  NatWest = "NatWest",
  Modelo = "Modelo",
  NewDayAmazon = "NewDayAmazon",
  Monzo = "Monzo",
  Nationwide = "Nationwide",
  BankOfIreland = "BankOfIreland"
}

export type ConsentUIInteractions = (
  page: puppeteer.Page,
  navigationPromise: Promise<puppeteer.Response>,
  consentVariety: ConsentVariety
) => void

// Bank interactions lookup
const bankConsentUIInteractions: { [index in Bank]: ConsentUIInteractions } = {
  NatWest: NatWest.consentUIInteractions,
  Modelo: Modelo.consentUIInteractions,
  NewDayAmazon: NatWest.consentUIInteractions,
  Monzo: NatWest.consentUIInteractions,
  Nationwide: NatWest.consentUIInteractions,
  BankOfIreland: NatWest.consentUIInteractions,
}

export async function authoriseConsent(
  authURL: string,
  bank: Bank,
  consentVariety: ConsentVariety) {

  const consentUIInteractions = bankConsentUIInteractions[bank]

  // const chromePath = String.raw`C:\Program Files (x86)\Google\Chrome\Application\chrome.exe`
  // const extensionPath = String.raw`C:\Users\mark\AppData\Local\Google\Chrome\User Data\Default\Extensions\djeegiggegleadkkbgopoonhjimgehda\0.8.1_0`
  // const args = [
  //   `--disable-extensions-except=${extensionPath}`,
  //   `--load-extension=${extensionPath}`
  // ]
  const browser = await puppeteer.launch({
    // Below options are debug options.
    headless: false,
    //executablePath: chromePath,
    //args: args,
    //slowMo: 100
  })
  const page = await browser.newPage()
  await page.goto(authURL)
  const navigationPromise = page.waitForNavigation()
  const navigationPromiseNetworkIdle = page.waitForNavigation({ 'waitUntil': 'networkidle0' })
  await consentUIInteractions(page, navigationPromise, consentVariety)
  // Wait for redirect web page
  await navigationPromiseNetworkIdle
  await page.waitForSelector('auth-fragment-redirect', { hidden: true, timeout: 10000 })
  const pageStatusJSHandle = await page.waitForFunction('window.pageStatus', { timeout: 10000 })
  const pageStatus = await pageStatusJSHandle.jsonValue()
  assert(pageStatus == 'Fragment post complete.', 'Authorisation redirect error')
  await browser.close()
}

import {ConsentUIInteractions} from "../authoriseConsent"

export const consentUIInteractions: ConsentUIInteractions = async (page, navigationPromise, consentVariety, bankUser) => {

    // Enter user ID and password
    await page.waitForSelector('#FakeLogonUserID')
    await page.type('#FakeLogonUserID', bankUser.userNameOrNumber)

    await page.waitForSelector('#FakeLogonPassword')
    await page.type('#FakeLogonPassword', bankUser.password)
    
    await page.waitForSelector('#FakeLogonContinueButton')
    await page.waitForTimeout(400) // workaround for clicks not registering sometimes
    await page.click('#FakeLogonContinueButton')
    
    await page.waitForNavigation({'waitUntil': 'networkidle0'}) // seems necessary to ensure next selector found
    
    // Select account
    await page.waitForSelector('#\\31 55173-12471731 > .Account__controls > .Account__controls__control > .RadioButton > .RadioButton__label')
    await page.click('#\\31 55173-12471731 > .Account__controls > .Account__controls__control > .RadioButton > .RadioButton__label')

    await page.waitForSelector('#confirm')
    await page.waitForTimeout(400) // workaround for clicks not registering sometimes
    await page.click('#confirm')

    await navigationPromise

    // Confirm consent
    await page.waitForSelector('#ASHESignatureConfirmButton')
    await page.waitForTimeout(400) // workaround for clicks not registering sometimes
    await page.click('#ASHESignatureConfirmButton')

    await navigationPromise

    // Transfer back
    await page.waitForSelector('#transfer')
    await page.waitForTimeout(400) // workaround for clicks not registering sometimes
    await page.click('#transfer')
}

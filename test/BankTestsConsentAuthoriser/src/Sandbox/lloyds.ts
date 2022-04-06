import {ConsentUIInteractions} from "../authoriseConsent"

export const consentUIInteractions: ConsentUIInteractions = async (page, navigationPromise, consentVariety, bankUser) => {

    // Cookie popup
    await page.waitForSelector('.mat-app-background > .cc-window > .cc-compliance > .cc-btn:nth-child(2)')
    await page.click('.mat-app-background > .cc-window > .cc-compliance > .cc-btn:nth-child(2)')

    // User name
    await page.waitForSelector('.mat-form-field #IDToken1')
    await page.click('.mat-form-field #IDToken1')
    await page.type('.mat-form-field #IDToken1', bankUser.userNameOrNumber)

    // Password
    await page.waitForSelector('.mat-form-field #mat-input-1')
    await page.click('.mat-form-field #mat-input-1')
    await page.type('.mat-form-field #mat-input-1', bankUser.password)

    // Next
    await page.waitForSelector('.ng-star-inserted > .ng-star-inserted > .ng-dirty > .ng-star-inserted > .mat-flat-button')
    await page.click('.ng-star-inserted > .ng-star-inserted > .ng-dirty > .ng-star-inserted > .mat-flat-button')
    await navigationPromise

    // Select account
    await page.waitForSelector('.Aligner > #mat-radio-2 > .mat-radio-label > .mat-radio-container > .mat-radio-outer-circle')
    await page.click('.Aligner > #mat-radio-2 > .mat-radio-label > .mat-radio-container > .mat-radio-outer-circle')

    // Proceed
    await page.waitForSelector('.submit-box > div > .paymentButton > .mat-button > .mat-button-wrapper')
    await page.click('.submit-box > div > .paymentButton > .mat-button > .mat-button-wrapper')

    // Confirm
    await page.waitForSelector('#confirm-dialog-submit')
    await page.click('#confirm-dialog-submit')

    await navigationPromise
}
    
import {ConsentUIInteractions} from "../authoriseConsent"

export const consentUIInteractions: ConsentUIInteractions = async (page, navigationPromise, consentVariety, bankUser) => {

    await page.waitForSelector('#username')
    await page.click('#username')
    await page.type('#username', bankUser.userNameOrNumber)
    
    await page.waitForSelector('#otp')
    await page.click('#otp')
    await page.type('#otp', bankUser.password)

    await page.waitForSelector('.form-inner > .submit-btn > .btn-container > .continuebtn > a')
    await page.click('.form-inner > .submit-btn > .btn-container > .continuebtn > a')

    await navigationPromise

}

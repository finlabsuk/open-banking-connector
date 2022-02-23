import {ConsentUIInteractions} from "../authoriseConsent"

export const consentUIInteractions: ConsentUIInteractions = async (page, navigationPromise, consentVariety, bankUser) => {

    await page.waitForSelector('username-component > .content-section > .username-input-box > .input-container > .form-field-input')
    await page.click('username-component > .content-section > .username-input-box > .input-container > .form-field-input')
    await page.type('username-component > .content-section > .username-input-box > .input-container > .form-field-input', bankUser.userNameOrNumber)

    await page.waitForSelector('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(1)')
    await page.click('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(1)')
    await page.type('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(1)', bankUser.password.charAt(0))

    await page.waitForSelector('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(2)')
    await page.click('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(2)')
    await page.type('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(2)', bankUser.password.charAt(1))

    await page.waitForSelector('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(3)')
    await page.click('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(3)')
    await page.type('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(3)', bankUser.password.charAt(2))

    await page.waitForSelector('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(4)')
    await page.click('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(4)')
    await page.type('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(4)', bankUser.password.charAt(3))

    await page.waitForSelector('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(5)')
    await page.click('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(5)')
    await page.type('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(5)', bankUser.password.charAt(4))

    await page.waitForSelector('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(6)')
    await page.click('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(6)')
    await page.type('security-code-component > .content-section > .authentication-code-container > .input-container > .form-field-input:nth-child(6)', bankUser.password.charAt(5))

    await page.waitForSelector('.main-section > security-code-component > .content-section > .button-container > .btn')
    await page.click('.main-section > security-code-component > .content-section > .button-container > .btn')

    // Second screen - don't await navigationPromise as doesn't return
    await page.waitForSelector('.ng-star-inserted > .obie-aisp-section > .ng-star-inserted > .accounts-box > .account-box')
    await page.click('.ng-star-inserted > .obie-aisp-section > .ng-star-inserted > .accounts-box > .account-box')

    await page.waitForSelector('.main-section > .ng-star-inserted > .obie-aisp-section > .button-box > .btn')
    await page.click('.main-section > .ng-star-inserted > .obie-aisp-section > .button-box > .btn')

    await navigationPromise

}

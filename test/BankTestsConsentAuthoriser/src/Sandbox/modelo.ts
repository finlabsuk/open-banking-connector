import {ConsentUIInteractions, ConsentVariety} from "../authoriseConsent"

export const consentUIInteractions: ConsentUIInteractions = async (page, navigationPromise, consentVariety, bankUser) => {

    await page.waitForSelector('#wizardContent > #loginForm #loginName')
    await page.click('#wizardContent > #loginForm #loginName')
    await page.type('#wizardContent > #loginForm #loginName', bankUser.userNameOrNumber)

    await page.waitForSelector('#wizardContent > #loginForm #password')
    await page.click('#wizardContent > #loginForm #password')
    await page.type('#wizardContent > #loginForm #password', bankUser.password)

    await page.waitForSelector('.col-md-9 > #wizardContent > .nav > .nav-item:nth-child(1) > .nav-link')
    await page.click('.col-md-9 > #wizardContent > .nav > .nav-item:nth-child(1) > .nav-link')
    await navigationPromise

    if (consentVariety == ConsentVariety.AccountAccessConsent) {

        await page.select('#selectAccountsPage > #loginForm #accounts', '700004000000000000000002')
        await page.waitForSelector('#loginForm > .form-row > .form-group > #accounts > option:nth-child(2)')
        await page.click('#loginForm > .form-row > .form-group > #accounts > option:nth-child(2)')

    } else {

        await page.waitForSelector('#selectAccountsPage > #loginForm #account')
        await page.click('#selectAccountsPage > #loginForm #account')

    }

    await page.waitForSelector('#wizardContent > #selectAccountsPage > .nav > .nav-item:nth-child(1) > .nav-link')
    await page.click('#wizardContent > #selectAccountsPage > .nav > .nav-item:nth-child(1) > .nav-link')
    await navigationPromise

}

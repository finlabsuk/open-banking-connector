import { ConsentUIInteractions } from "../authoriseConsent"
import { assertIsDefined } from "../utility"

export const consentUIInteractions: ConsentUIInteractions = async (page, navigationPromise, consentVariety, bankUser) => {
  await page.waitForSelector('#customer-number')
  await page.type('#customer-number', bankUser.userNameOrNumber)
  await page.waitForSelector('#customer-number-login')
  await page.click('#customer-number-login')
  await navigationPromise

  // Complete upper row of boxes
  await page.waitForSelector('#pin-1')
  const upperBox1Digit = await page.$eval(
    '#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(2) > div.panel-body > div:nth-child(1) > label',
    el => el.textContent
  );
  assertIsDefined(upperBox1Digit)
  await page.type('#pin-1', upperBox1Digit)
  await page.waitForSelector('#pin-2')
  const upperBox2Digit = await page.$eval(
    '#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(2) > div.panel-body > div:nth-child(2) > label',
    el => el.textContent
  );
  assertIsDefined(upperBox2Digit)
  await page.type('#pin-2', upperBox2Digit)
  await page.waitForSelector('#pin-3')
  const upperBox3Digit = await page.$eval(
    '#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(2) > div.panel-body > div:nth-child(3) > label',
    el => el.textContent
  );
  assertIsDefined(upperBox3Digit)
  await page.type('#pin-3', upperBox3Digit)
  // Complete lower row of boxes
  await page.waitForSelector('#password-1')
  const lowerBox1Digit = await page.$eval(
    '#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(3) > div.panel-body > div:nth-child(1) > label',
    el => el.textContent
  );
  assertIsDefined(lowerBox1Digit)
  await page.type('#password-1', lowerBox1Digit)
  await page.waitForSelector('#password-2')
  const lowerBox2Digit = await page.$eval(
    '#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(3) > div.panel-body > div:nth-child(2) > label',
    el => el.textContent
  );
  assertIsDefined(lowerBox2Digit)
  await page.type('#password-2', lowerBox2Digit)
  await page.waitForSelector('#password-3')
  const lowerBox3Digit = await page.$eval(
    '#kc-form-wrapper > div.screen-group > div.screen-content.step-2 > div:nth-child(3) > div.panel-body > div:nth-child(3) > label',
    el => el.textContent
  );
  assertIsDefined(lowerBox3Digit)
  await page.type('#password-3', lowerBox3Digit)
  // Click continue
  await page.waitForSelector('#login-button')
  await page.click('#login-button')
  await navigationPromise

  // Select accounts then confirm access
  await page.waitForSelector('#account-list > li:nth-child(1) > dl > dd.action.col-size-1 > button')
  await page.click('#account-list > li:nth-child(1) > dl > dd.action.col-size-1 > button')
  await page.waitForSelector('#account-list > li:nth-child(2) > dl > dd.action.col-size-1 > button')
  await page.click('#account-list > li:nth-child(2) > dl > dd.action.col-size-1 > button')
  await page.waitForSelector('#approveButton')
  await page.click('#approveButton')
}


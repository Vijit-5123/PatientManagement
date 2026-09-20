import { test, expect } from '@playwright/test';

test('Unauthenticated user visiting /patients is redirected to login', async ({ page }) => {
  await page.goto('/patients');

  await expect(page).toHaveURL(/\/login/);
  await expect(page.getByTestId('login-submit')).toBeVisible();
});

test('Invalid login credentials show an error message', async ({ page }) => {
  await page.goto('/login');

  await page.getByTestId('login-username').fill('admin');
  await page.getByTestId('login-password').fill('WrongPassword');
  await page.getByTestId('login-submit').click();

  await expect(page.getByTestId('login-error')).toContainText('Invalid username or password.');
  await expect(page).toHaveURL(/\/login/);
});

import { test, expect } from '@playwright/test';

test('Admin login → patients → create → view → edit → delete', async ({ page }) => {
  await page.goto('/login');

  await page.getByTestId('login-username').fill('admin');
  await page.getByTestId('login-password').fill('Admin123!');
  await page.getByTestId('login-submit').click();

  await expect(page).toHaveURL(/\/patients$/);
  await expect(page.getByTestId('current-user')).toContainText('admin (Admin)');

  // Create
  await page.getByTestId('patient-create').click();
  await expect(page).toHaveURL(/\/patients\/new$/);

  const uniqueLastName = `E2E-${Date.now()}`;
  await page.getByTestId('patient-firstName').fill('Playwright');
  await page.getByTestId('patient-lastName').fill(uniqueLastName);
  await page.getByTestId('patient-dateOfBirth').fill('1990-06-15');
  await page.getByTestId('patient-gender').click();
  await page.getByRole('option', { name: 'Other' }).click();
  await page.getByTestId('patient-email').fill('playwright.admin@example.com');
  await page.getByTestId('patient-form-submit').click();

  // View (redirected to detail page after create)
  await expect(page).toHaveURL(/\/patients\/\d+$/);
  await expect(page.getByText(`Playwright ${uniqueLastName}`)).toBeVisible();

  // Edit
  await page.getByTestId('patient-detail-edit').click();
  await expect(page).toHaveURL(/\/patients\/\d+\/edit$/);
  await page.getByTestId('patient-firstName').fill('PlaywrightUpdated');
  await page.getByTestId('patient-form-submit').click();

  await expect(page).toHaveURL(/\/patients\/\d+$/);
  await expect(page.getByText(`PlaywrightUpdated ${uniqueLastName}`)).toBeVisible();

  // Delete
  await page.getByTestId('patient-detail-delete').click();
  await page.getByTestId('confirm-dialog-confirm').click();

  await expect(page).toHaveURL(/\/patients$/);
});
